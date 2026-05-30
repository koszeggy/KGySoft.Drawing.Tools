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

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.ViewModel;
using KGySoft.Drawing.ImagingTools.WinApi;
using KGySoft.WinForms;

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

        // The following message dialogs use KGySoft.WinForms TaskDialogs to support theming and dynamic localization.
        // If an argument is Func<string>, it is also reevaluated on language change.

        internal static void ErrorMessage(IView? owner, string resourceId, object[]? args)
        {
            using IViewModel vm = ViewModelFactory.CreateErrorMessage(resourceId, args);
            ViewFactory.ShowDialog(vm, owner);
        }

        internal static void InfoMessage(IView? owner, string resourceId, object[]? args)
        {
            using IViewModel vm = ViewModelFactory.CreateInfoMessage(resourceId, args);
            ViewFactory.ShowDialog(vm, owner);
        }

        internal static void WarningMessage(IView? owner, string resourceId, object[]? args)
        {
            using IViewModel vm = ViewModelFactory.CreateWarningMessage(resourceId, args);
            ViewFactory.ShowDialog(vm, owner);
        }

        internal static bool ConfirmMessage(IView? owner, string resourceId, object[]? args, bool isYesDefault)
        {
            using IViewModel<int> vm = ViewModelFactory.CreateConfirmMessage(resourceId, args, isYesDefault);
            ViewFactory.ShowDialog(vm, owner);
            return vm.GetEditedModel() == 0;
        }

        internal static bool? CancellableConfirmMessage(IView? owner, string resourceId, object[]? args, int defaultButton)
        {
            using IViewModel<int> vm = ViewModelFactory.CreateCancellableConfirmMessage(resourceId, args, defaultButton);
            ViewFactory.ShowDialog(vm, owner);
            return vm.GetEditedModel() switch
            {
                0 => true,
                1 => false,
                _ => null
            };
        }

        internal static Color? PickColor(Color? selectedColor = default)
        {
            colorDialog ??= new ColorDialog { /*AnyColor = true,*/ FullOpen = true };
            if (selectedColor.HasValue)
                colorDialog.Color = selectedColor.Value;

            // On Windows hooking messages to be able to localize the dialog texts
            IntPtr windowHook = IntPtr.Zero;
            if (OSHelper.IsWindows && !OSHelper.IsFrameworkMono)
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
            if (OSHelper.IsWindows && !OSHelper.IsFrameworkMono)
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
            //// Needed for classic MessageBoxes. Restore if reverting from TaskDialogs
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
