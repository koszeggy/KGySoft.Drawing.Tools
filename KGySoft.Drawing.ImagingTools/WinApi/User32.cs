#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: User32.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
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
using System.ComponentModel;
#if !NET5_0_OR_GREATER
using System.Drawing;
#endif
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
#if !NET5_0_OR_GREATER
using System.Windows.Forms;
#endif

#endregion

namespace KGySoft.Drawing.ImagingTools.WinApi
{
    [SecurityCritical]
    internal static class User32
    {
        #region NativeMethods class

        private static class NativeMethods
        {
            #region Methods

#if !NET5_0_OR_GREATER
            /// <summary>
            /// The ScreenToClient function converts the screen coordinates of a specified point on the screen to client-area coordinates.
            /// </summary>
            /// <param name="hWnd">A handle to the window whose client area will be used for the conversion.</param>
            /// <param name="lpPoint">A pointer to a POINT structure that specifies the screen coordinates to be converted.</param>
            /// <returns>If the function succeeds, the return value is nonzero.
            /// If the function fails, the return value is zero.</returns>
            [DllImport("user32.dll")]
            internal static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);
#endif

            /// <summary>
            /// Installs an application-defined hook procedure into a hook chain. You would install a hook procedure to monitor the system for certain types of events.
            /// These events are associated either with a specific thread or with all threads in the same desktop as the calling thread.
            /// </summary>
            /// <param name="idHook">The type of hook procedure to be installed. This parameter can be one of the WH_ values.</param>
            /// <param name="lpfn">A pointer to the hook procedure. If the dwThreadId parameter is zero or specifies the identifier of a thread created by a different process,
            /// the lpfn parameter must point to a hook procedure in a DLL. Otherwise, lpfn can point to a hook procedure in the code associated with the current process.</param>
            /// <param name="hInstance">A handle to the DLL containing the hook procedure pointed to by the lpfn parameter.
            /// The hMod parameter must be set to NULL if the dwThreadId parameter specifies a thread created by the current process and if the hook procedure is within
            /// the code associated with the current process.</param>
            /// <param name="threadId">The identifier of the thread with which the hook procedure is to be associated. For desktop apps, if this parameter is zero,
            /// the hook procedure is associated with all existing threads running in the same desktop as the calling thread.</param>
            /// <returns>If the function succeeds, the return value is the handle to the hook procedure.
            /// If the function fails, the return value is NULL. To get extended error information, call GetLastError.</returns>
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern IntPtr SetWindowsHookEx(int idHook, HOOKPROC lpfn, IntPtr hInstance, uint threadId);

            /// <summary>
            /// Removes a hook procedure installed in a hook chain by the <see cref="SetWindowsHookEx"/> function.
            /// </summary>
            /// <param name="idHook">A handle to the hook to be removed.
            /// This parameter is a hook handle obtained by a previous call to <see cref="SetWindowsHookEx"/>.</param>
            /// <returns>If the function succeeds, the return value is nonzero.
            /// If the function fails, the return value is zero.To get extended error information, call GetLastError.</returns>
            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool UnhookWindowsHookEx(IntPtr idHook);

            /// <summary>
            /// Passes the hook information to the next hook procedure in the current hook chain.
            /// A hook procedure can call this function either before or after processing the hook information.
            /// </summary>
            /// <param name="hhk">This parameter is ignored.</param>
            /// <param name="nCode">The hook code passed to the current hook procedure.
            /// The next hook procedure uses this code to determine how to process the hook information.</param>
            /// <param name="wParam">The wParam value passed to the current hook procedure.
            /// The meaning of this parameter depends on the type of hook associated with the current hook chain.</param>
            /// <param name="lParam">The lParam value passed to the current hook procedure.
            /// The meaning of this parameter depends on the type of hook associated with the current hook chain.</param>
            /// <returns>This value is returned by the next hook procedure in the chain. The current hook procedure must also return this value.
            /// The meaning of the return value depends on the hook type. For more information, see the descriptions of the individual hook procedures.</returns>
            [DllImport("user32.dll")]
            internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

            /// <summary>
            /// Retrieves the name of the class to which the specified window belongs.
            /// </summary>
            /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
            /// <param name="lpClassName">The class name string.</param>
            /// <param name="nMaxCount">The length of the lpClassName buffer, in characters.
            /// The buffer must be large enough to include the terminating null character; otherwise, the class name string is truncated to nMaxCount-1 characters.</param>
            /// <returns>If the function succeeds, the return value is the number of characters copied to the buffer, not including the terminating null character.
            /// If the function fails, the return value is zero. To get extended error information, call GetLastError function.</returns>
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern int GetClassName(IntPtr hWnd, [Out]char[] lpClassName, int nMaxCount);

            /// <summary>
            /// Enumerates the child windows that belong to the specified parent window by passing the handle to each child window, in turn, to an application-defined callback function.
            /// EnumChildWindows continues until the last child window is enumerated or the callback function returns FALSE.
            /// </summary>
            /// <param name="hWndParent">A handle to the parent window whose child windows are to be enumerated. If this parameter is NULL, this function is equivalent to EnumWindows.</param>
            /// <param name="lpEnumFunc">A pointer to an application-defined callback function. For more information, see EnumChildProc.</param>
            /// <param name="lParam">An application-defined value to be passed to the callback function.</param>
            /// <returns>The return value is not used.</returns>
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool EnumChildWindows(IntPtr hWndParent, EnumChildProc lpEnumFunc, IntPtr lParam);

            /// <summary>
            /// Retrieves the identifier of the specified control.
            /// </summary>
            /// <param name="hWnd">A handle to the control.</param>
            /// <returns>If the function succeeds, the return value is the identifier of the control.
            /// If the function fails, the return value is zero.An invalid value for the hWnd parameter, for example, will cause the function to fail.To get extended error information, call GetLastError.</returns>
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern int GetDlgCtrlID(IntPtr hWnd);

            /// <summary>
            /// Changes the text of the specified window's title bar (if it has one). If the specified window is a control, the text of the control is changed.
            /// However, SetWindowText cannot change the text of a control in another application.
            /// </summary>
            /// <param name="hWnd">A handle to the window or control whose text is to be changed.</param>
            /// <param name="lpString">The new title or control text.</param>
            /// <returns>If the function succeeds, the return value is nonzero.
            /// If the function fails, the return value is zero.To get extended error information, call GetLastError.</returns>
            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool SetWindowText(IntPtr hWnd, string lpString);

            #endregion
        }

        #endregion

        #region Fields

        /// <summary>
        /// Used in <see cref="GetClassName"/>, length is enough for the longest possible name with null terminator as described here: https://docs.microsoft.com/en-us/windows/win32/winmsg/about-window-classes
        /// </summary>
        private static readonly char[] classNameBuf = new char[11];

        #endregion

        #region Methods

#if !NET5_0_OR_GREATER
        internal static void ScreenToClient(Control control, ref Point point) => NativeMethods.ScreenToClient(control.Handle, ref point);
#endif

        internal static IntPtr HookCallWndRetProc(HOOKPROC hookProc)
            => NativeMethods.SetWindowsHookEx(Constants.WH_CALLWNDPROCRET, hookProc, IntPtr.Zero, Kernel32.GetCurrentThreadId());

        internal static void UnhookWindowsHook(IntPtr hook) => NativeMethods.UnhookWindowsHookEx(hook);

        internal static IntPtr CallNextHook(int code, IntPtr wParam, IntPtr lParam)
            => NativeMethods.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);

        internal static string GetClassName(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                throw new ArgumentNullException(nameof(handle), PublicResources.ArgumentNull);
            int length = NativeMethods.GetClassName(handle, classNameBuf, classNameBuf.Length);
            if (length == 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());
            return new String(classNameBuf, 0, length);
        }

        internal static void EnumChildWindows(IntPtr handle, EnumChildProc enumProc) => NativeMethods.EnumChildWindows(handle, enumProc, IntPtr.Zero);

        internal static int GetDialogControlId(IntPtr handle) => NativeMethods.GetDlgCtrlID(handle);

        internal static bool SetWindowText(IntPtr handle, string text) => NativeMethods.SetWindowText(handle, text);

        #endregion
    }
}
