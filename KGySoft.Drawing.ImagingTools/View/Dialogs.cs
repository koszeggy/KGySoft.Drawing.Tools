#region Copyright

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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.WinApi;

#endregion

namespace KGySoft.Drawing.ImagingTools.View
{
    internal static class Dialogs
    {
        #region Fields

        private static readonly IntPtr windowHook;

        private static readonly HOOKPROC callWndRetProc = CallWndRetProc;
        private static readonly EnumChildProc enumChildProc = EnumChildProc;

        #endregion

        #region Constructors

        static Dialogs()
        {
            if (!OSUtils.IsWindows || OSUtils.IsMono)
                return;
            windowHook = User32.HookCallWndRetProc(callWndRetProc);
            if (windowHook != IntPtr.Zero)
                AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }

        #endregion

        #region Methods

        #region Internal Methods

        internal static void ErrorMessage(string message) => Show(message, Res.TitleError, MessageBoxButtons.OK, MessageBoxIcon.Error);
        internal static void InfoMessage(string message) => Show(message, Res.TitleInformation, MessageBoxButtons.OK, MessageBoxIcon.Information);
        internal static void WarningMessage(string message) => Show(message, Res.TitleWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        
        internal static bool ConfirmMessage(string message, bool isYesDefault = true)
            => Show(message, Res.TitleConfirmation, MessageBoxButtons.YesNo, MessageBoxIcon.Question, isYesDefault ? MessageBoxDefaultButton.Button1 : MessageBoxDefaultButton.Button2) == DialogResult.Yes;

        internal static bool? CancellableConfirmMessage(string message, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
            => Show(message, Res.TitleConfirmation, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, defaultButton) switch
            {
                DialogResult.Yes => true,
                DialogResult.No => false,
                _ => null
            };

        #endregion

        #region Private Methods

        private static DialogResult Show(string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
            => MessageBox.Show(message, caption, buttons, icon, defaultButton, LanguageSettings.DisplayLanguage.TextInfo.IsRightToLeft ? MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading : default);

        private static IntPtr CallWndRetProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var msg = (CWPRETSTRUCT)Marshal.PtrToStructure(lParam, typeof(CWPRETSTRUCT));
                if (msg.message == Constants.WM_INITDIALOG)
                {
                    string name = User32.GetClassName(msg.hwnd);
                    if (name == Constants.ClassNameDialogBox)
                        User32.EnumChildWindows(msg.hwnd, enumChildProc);
                }
            }

            return User32.CallNextHook(nCode, wParam, lParam);
        }

        private static bool EnumChildProc(IntPtr hWnd, IntPtr lParam)
        {
            string className = User32.GetClassName(hWnd);
            int id = User32.GetDialogControlId(hWnd);
            User32.SetWindowText(hWnd, $"{className}.{id}");
            return true;
        }

        #endregion

        #region Event Handlers

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Debug.Assert(windowHook != IntPtr.Zero);
            if (windowHook != IntPtr.Zero)
                User32.UnhookWindowsHook(windowHook);
        }

        #endregion

        #endregion
    }
}
