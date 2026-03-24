#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Dialogs.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2026 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

#region Used Namespaces

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.WinApi;
using KGySoft.WinForms.Components;

#endregion

#region Used Aliases

#if NET5_0_OR_GREATER
using TaskDialog = KGySoft.WinForms.Components.TaskDialog;
using TaskDialogButton = KGySoft.WinForms.Components.TaskDialogButton;
#endif

#endregion

#endregion

namespace KGySoft.Drawing.ImagingTools.View
{
    internal static class Dialogs
    {
        #region Nested Types

        #region DialogType enum
        
        private enum DialogType
        {
            //SingleButtonMessageBox,
            //MultiButtonMessageBox,
            ColorDialog,
            FolderDialog
        }

        #region EnumerationContext struct
        
        private struct DialogContext
        {
            #region Fields

            internal DialogType DialogType;
            internal int CustomStaticId;
            internal bool AllowCustomStaticLocalization;

            #endregion
        }

        #endregion

        #endregion

        #endregion

        #region Fields

        // These delegates are stored as a field to prevent their possible garbage collection while used by P/Invoke call.
        private static readonly HOOKPROC callWndRetProc = CallWndRetProc;
        private static readonly EnumChildProc enumChildProc = EnumChildProc;

        private static DialogContext dialogContext;
        private static ColorDialog? colorDialog;
        private static FolderBrowserDialog? folderDialog;

        #endregion

        #region Methods

        #region Internal Methods

        internal static void ErrorMessage(string message) => ShowMessage(message, Res.TitleError, TaskDialogStandardButtonFlags.OK, TaskDialogStandardIcons.Error);
        internal static void InfoMessage(string message) => ShowMessage(message, Res.TitleInformation, TaskDialogStandardButtonFlags.OK, TaskDialogStandardIcons.Information);
        internal static void WarningMessage(string message) => ShowMessage(message, Res.TitleWarning, TaskDialogStandardButtonFlags.OK, TaskDialogStandardIcons.Warning);
        
        internal static bool ConfirmMessage(string message, bool isYesDefault = true)
            => ShowMessage(message, Res.TitleConfirmation, TaskDialogStandardButtonFlags.Yes | TaskDialogStandardButtonFlags.No, TaskDialogStandardIcons.Question, isYesDefault ? 0 : 1) == 0;

        internal static bool? CancellableConfirmMessage(string message, int defaultButton = 0)
            => ShowMessage(message, Res.TitleConfirmation, TaskDialogStandardButtonFlags.Yes | TaskDialogStandardButtonFlags.No | TaskDialogStandardButtonFlags.Cancel,
                    TaskDialogStandardIcons.Question, defaultButton) switch
                {
                    0 => true,
                    1 => false,
                    _ => null
                };

        internal static Color? PickColor(Color? selectedColor = default)
        {
            colorDialog ??= new ColorDialog { /*AnyColor = true,*/ FullOpen = true };
            if (selectedColor.HasValue)
                colorDialog.Color = selectedColor.Value;

            // On Windows hooking messages to be able to localize the dialog texts
            IntPtr windowHook = IntPtr.Zero;
            if (OSUtils.IsWindows && !OSUtils.IsMono)
            {
                windowHook = User32.HookCallWndRetProc(callWndRetProc);
                dialogContext = new DialogContext
                {
                    DialogType = DialogType.ColorDialog,
                    AllowCustomStaticLocalization = true
                };
            }

            DialogResult result = colorDialog.ShowDialog();

            if (windowHook != IntPtr.Zero)
                User32.UnhookWindowsHook(windowHook);

            return result == DialogResult.OK ? colorDialog.Color : null;
        }

        internal static string? SelectFolder(string? selectedPath = null)
        {
            folderDialog ??= new FolderBrowserDialog { ShowNewFolderButton = true };
            if (selectedPath != null)
                folderDialog.SelectedPath = selectedPath;

            // On Windows hooking messages to be able to localize the dialog texts
            IntPtr windowHook = IntPtr.Zero;
            if (OSUtils.IsWindows && !OSUtils.IsMono)
            {
                windowHook = User32.HookCallWndRetProc(callWndRetProc);
                dialogContext = new DialogContext
                {
                    DialogType = DialogType.FolderDialog,
                    AllowCustomStaticLocalization = true
                };
            }

            DialogResult result = folderDialog.ShowDialog();

            if (windowHook != IntPtr.Zero)
                User32.UnhookWindowsHook(windowHook);

            return result == DialogResult.OK ? folderDialog.SelectedPath : null;
        }

        #endregion

        #region Private Methods

        private static int ShowMessage(string message, string caption, TaskDialogStandardButtonFlags buttons, TaskDialogStandardIcons icon, int defaultButton = 0)
        {
            // ReSharper disable once UsingStatementResourceInitialization - false alarm, these property setters do not throw exceptions
            using var taskDialog = new TaskDialog
            {
                Caption = caption,
                //StandardButtons = buttons,
                //DefaultStandardButton = defaultButton,
                Icon = icon,
                Message = message,
                ForceCompatibilityMode = true, // so we can apply theme changes
                //Options = TaskDialogOptions.TranslateStandardButtons,
            };

            if (Res.IsRightToLeft)
                taskDialog.Options |= TaskDialogOptions.RightToLeftLayout;

            // Adding the buttons as custom ones. This makes possible to use local resources instead of the ones in KGySoft.WinForms
            foreach (TaskDialogStandardButtonFlags flag in buttons.GetFlags())
                taskDialog.Buttons.Add(new TaskDialogButton(Res.Get($"btn{flag}.Text")));
            taskDialog.Buttons[defaultButton].IsDefault = true;

            SynchronizationContext context = SynchronizationContext.Current!;
            taskDialog.Created += TaskDialog_Created;
            ThemeColors.ThemeChanged += ThemeColors_ThemeChanged;

            int selectedButtonIndex;
            try
            {
                taskDialog.Show(GetOwner(), out selectedButtonIndex, out var _, out var _);
            }
            finally
            {
                ThemeColors.ThemeChanged -= ThemeColors_ThemeChanged;
                taskDialog.Created -= TaskDialog_Created; // though it's removed on dispose
            }

            return selectedButtonIndex;

            #region Local Methods

            // ReSharper disable InconsistentNaming - event handlers
            static void TaskDialog_Created(object? sender, EventArgs e) => ApplyTheme((TaskDialog)sender!);

            [SuppressMessage("ReSharper", "AccessToDisposedClosure", Justification = "False alarm, event is unsubscribed on disposing")]
            void ThemeColors_ThemeChanged(object? sender, EventArgs e) => context.Send(_ => ApplyTheme(taskDialog), null);
            // ReSharper restore InconsistentNaming

            static IWin32Window? GetOwner()
            {
                if (Form.ActiveForm is Form form)
                    return form;
                if (!OSUtils.IsWindows)
                    return null;
                IntPtr hwnd = User32.GetActiveWindow();
                return hwnd == IntPtr.Zero ? null : new OwnerWindowHandle(hwnd);
            }

            static void ApplyTheme(TaskDialog td)
            {
                if (!ThemeColors.RenderWithVisualStyles || ThemeColors.HighContrast || !ThemeColors.IsBaseThemeEverChanged)
                    return;

                Control? form = Control.FromHandle(td.Handle);
                if (form == null)
                    return;

                // header, root colors
                form.ApplyTheme();

                // These controls have explicitly set colors that we need to override
                form.Controls["pnlDividerMainBottom"]?.BackColor = ThemeColors.TaskDialogDivider;
                Control? pnlMain = form.Controls["pnlMain"];
                Debug.Assert(pnlMain != null);
                if (pnlMain != null)
                {
                    pnlMain.BackColor = ThemeColors.Window;
                    pnlMain.ForeColor = ThemeColors.WindowText;
                    pnlMain.Controls["pnlMainIcon"]?.Controls["pnlMainIconBackground"]?.BackColor = ThemeColors.Window;
                }
            }

            #endregion
        }

        private static IntPtr CallWndRetProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var msg = (CWPRETSTRUCT)Marshal.PtrToStructure(lParam, typeof(CWPRETSTRUCT))!;
                if (msg.message == Constants.WM_INITDIALOG)
                {
                    string name = User32.GetClassName(msg.hwnd);
                    if (name == Constants.ClassNameDialogBox)
                    {
                        // Localizing non-MessageBox captions
                        if (dialogContext.DialogType == DialogType.ColorDialog)
                            User32.SetControlText(msg.hwnd, Res.TitleColorDialog);
                        else if (dialogContext.DialogType == DialogType.FolderDialog)
                            User32.SetControlText(msg.hwnd, Res.TitleFolderDialog);

                        // Enumerating the child controls by another WinAPI call
                        User32.EnumChildWindows(msg.hwnd, enumChildProc);
                    }
                }
            }

            return User32.CallNextHook(nCode, wParam, lParam);
        }

        private static bool EnumChildProc(IntPtr hWnd, IntPtr lParam)
        {
            string className = User32.GetClassName(hWnd);
            int id = User32.GetDialogControlId(hWnd);
            if (id == 0)
                return true;

            // Controls with id 65535 may duplicate on some dialogs. Usually these contain custom message but on color dialog
            // these are also constant labels so we assign incremental negative ids for them.
            if (id == UInt16.MaxValue && className == Constants.ClassNameStatic)
            {
                if (!dialogContext.AllowCustomStaticLocalization)
                    return true;
                id = --dialogContext.CustomStaticId;
            }
            //// If there is a single OK button in a MessageBox it has the same id as a Cancel button.
            //else if (dialogContext.DialogType == DialogType.SingleButtonMessageBox && id == Constants.IDCANCEL && className == Constants.ClassNameButton)
            //    id = Constants.IDOK;

            string? text = Res.GetStringOrNull($"{dialogContext.DialogType}.{className}.{id}") ?? Res.GetStringOrNull($"{className}.{id}");
            if (text != null)
                User32.SetControlText(hWnd, text);
            return true;
        }

        #endregion

        #endregion
    }
}
