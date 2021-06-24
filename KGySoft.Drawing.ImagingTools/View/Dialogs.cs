﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Dialogs.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2019 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution. If not, then this file is considered as
//  an illegal copy.
//
//  Unauthorized copying of this file, via any medium is strictly prohibited.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.WinApi;

#endregion

namespace KGySoft.Drawing.ImagingTools.View
{
    internal static class Dialogs
    {
        #region Nested Types

        #region DialogType enum
        
        private enum DialogType
        {
            MessageBoxSingleButton,
            MessageBoxMoreButtons,
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

        internal static void ErrorMessage(string message) => ShowMessageBox(message, Res.TitleError, MessageBoxButtons.OK, MessageBoxIcon.Error);
        internal static void InfoMessage(string message) => ShowMessageBox(message, Res.TitleInformation, MessageBoxButtons.OK, MessageBoxIcon.Information);
        internal static void WarningMessage(string message) => ShowMessageBox(message, Res.TitleWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        
        internal static bool ConfirmMessage(string message, bool isYesDefault = true)
            => ShowMessageBox(message, Res.TitleConfirmation, MessageBoxButtons.YesNo, MessageBoxIcon.Question, isYesDefault ? MessageBoxDefaultButton.Button1 : MessageBoxDefaultButton.Button2) == DialogResult.Yes;

        internal static bool? CancellableConfirmMessage(string message, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
            => ShowMessageBox(message, Res.TitleConfirmation, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, defaultButton) switch
            {
                DialogResult.Yes => true,
                DialogResult.No => false,
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

        private static DialogResult ShowMessageBox(string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
        {
            MessageBoxOptions options = Res.DisplayLanguage.TextInfo.IsRightToLeft ? MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading : default;
            IntPtr windowHook = IntPtr.Zero;

            // On Windows hooking messages to be able to localize the buttons
            if (OSUtils.IsWindows && !OSUtils.IsMono)
            {
                windowHook = User32.HookCallWndRetProc(callWndRetProc);
                dialogContext = new DialogContext
                {
                    DialogType = buttons == MessageBoxButtons.OK ? DialogType.MessageBoxSingleButton : DialogType.MessageBoxMoreButtons
                };
            }

            DialogResult result = MessageBox.Show(message, caption, buttons, icon, defaultButton, options);

            if (windowHook != IntPtr.Zero)
                User32.UnhookWindowsHook(windowHook);

            return result;
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

            // Controls with id 65535 may duplicate on some dialogs. Usually these contain custom message but on color dialog
            // these are also constant labels so we assign an incremental id for these.
            if (id == UInt16.MaxValue && className == Constants.ClassNameStatic)
            {
                if (!dialogContext.AllowCustomStaticLocalization)
                    return true;
                className = $"{className}.{dialogContext.DialogType}";
                id = dialogContext.CustomStaticId;
                dialogContext.CustomStaticId += 1;
            }
            // If there is a single OK button in a MessageBox it has the same id as a Cancel button.
            else if (dialogContext.DialogType == DialogType.MessageBoxSingleButton && id == Constants.IDCANCEL && className == Constants.ClassNameButton)
                id = Constants.IDOK;

            // Now the resource id is either "className.id" or "className.DialogType.id"
            string? text = Res.GetStringOrNull($"{className}.{id}");
            if (text != null)
                User32.SetControlText(hWnd, text);
            return true;
        }

        #endregion

        #endregion
    }
}
