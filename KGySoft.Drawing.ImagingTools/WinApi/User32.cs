#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: User32.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2025 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
#if !NET5_0_OR_GREATER
using System.Drawing;
#endif
using System.Runtime.InteropServices;
using System.Security;
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

        [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass", Justification = "Outer class methods are never called from here.")]
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

            /// <summary>
            /// Sets the current value of a specified Desktop Window Manager (DWM) attribute applied to a window.
            /// </summary>
            /// <param name="hwnd">An HWND specifying the handle to the window for which the attribute value is to be set.</param>
            /// <param name="data">A pointer to a WINDOWCOMPOSITIONATTRIBDATA structure describing which attribute to set and its new value.</param>
            /// <returns>TRUE if the function succeeds; otherwise, FALSE.</returns>
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool SetWindowCompositionAttribute(IntPtr hwnd, ref WINDOWCOMPOSITIONATTRIBDATA data);

            /// <summary>
            /// Adds a new entry or changes an existing entry in the property list of the specified window.
            /// The function adds a new entry to the list if the specified character string does not exist already in the list.
            /// The new entry contains the string and the handle. Otherwise, the function replaces the string's current handle with the specified handle.
            /// </summary>
            /// <param name="hWnd">A handle to the window whose property list receives the new entry.</param>
            /// <param name="lpString">A null-terminated string or an atom that identifies a string.
            /// If this parameter is an atom, it must be a global atom created by a previous call to the GlobalAddAtom function.
            /// The atom must be placed in the low-order word of lpString; the high-order word must be zero.</param>
            /// <param name="propertyValue">A handle to the data to be copied to the property list. The data handle can identify any value useful to the application.</param>
            /// <returns></returns>
            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool SetProp(IntPtr hWnd, string lpString, IntPtr propertyValue);

            /// <summary>
            /// Retrieves a handle to the foreground window (the window with which the user is currently working).
            /// The system assigns a slightly higher priority to the thread that creates the foreground window than it does to other threads.
            /// </summary>
            /// <returns>The return value is a handle to the foreground window. The foreground window can be NULL in certain circumstances, such as when a window is losing activation.</returns>
            [DllImport("user32.dll")]
            internal static extern IntPtr GetForegroundWindow();

            /// <summary>
            /// Sends the specified message to a window or windows.
            /// The SendMessage function calls the window procedure for the specified window and does not return until the window procedure has processed the message.
            /// To send a message and return immediately, use the SendMessageCallback or SendNotifyMessage function.
            /// To post a message to a thread's message queue and return immediately, use the PostMessage or PostThreadMessage function.
            /// </summary>
            /// <param name="hWnd">A handle to the window whose window procedure will receive the message. If this parameter is HWND_BROADCAST ((HWND)0xffff),
            /// the message is sent to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows;
            /// but the message is not sent to child windows.
            /// Message sending is subject to UIPI. The thread of a process can send messages only to message queues of threads in processes of lesser or equal integrity level.</param>
            /// <param name="msg">The message to be sent.</param>
            /// <param name="wParam">Additional message-specific information.</param>
            /// <param name="lParam">Additional message-specific information.</param>
            /// <returns>The return value specifies the result of the message processing; it depends on the message sent.</returns>
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern uint SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

            /// <summary>
            /// The GetWindowDC function retrieves the device context (DC) for the entire window, including title bar, menus, and scroll bars.
            /// A window device context permits painting anywhere in a window, because the origin of the device context is the upper-left corner of the window instead of the client area.
            /// GetWindowDC assigns default attributes to the window device context each time it retrieves the device context. Previous attributes are lost. 
            /// </summary>
            /// <param name="hWnd">Handle to the window with a device context that is to be retrieved. If this value is NULL, GetWindowDC retrieves the device context for the entire screen.</param>
            /// <returns>If the function succeeds, the return value is a handle to a device context for the specified window.
            /// If the function fails, the return value is NULL, indicating an error or an invalid hWnd parameter. 
            /// </returns>
            [DllImport("user32.dll")]
            internal extern static IntPtr GetWindowDC(IntPtr hWnd);

            /// <summary>
            /// The ReleaseDC function releases a device context (DC), freeing it for use by other applications. The effect of the ReleaseDC function depends on the type of DC. It frees only common and window DCs. It has no effect on class or private DCs.
            /// </summary>
            /// <param name="hWnd">A handle to the window whose DC is to be released.</param>
            /// <param name="hDC">A handle to the DC to be released.</param>
            /// <returns>The return value indicates whether the DC was released. If the DC was released, the return value is 1.
            /// If the DC was not released, the return value is zero.</returns>
            /// <remarks>
            /// The application must call the ReleaseDC function for each call to the GetWindowDC function and for each call to the GetDC function that retrieves a common DC.
            /// An application cannot use the ReleaseDC function to release a DC that was created by calling the CreateDC function; instead, it must use the DeleteDC function. ReleaseDC must be called from the same thread that called GetDC.</remarks>
            [DllImport("user32.dll")]
            internal static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

            /// <summary>
            /// Changes the size, position, and Z order of a child, pop-up, or top-level window.
            /// These windows are ordered according to their appearance on the screen.
            /// The topmost window receives the highest rank and is the first window in the Z order.
            /// </summary>
            /// <param name="hWnd">A handle to the window.</param>
            /// <param name="hWndInsertAfter">A handle to the window to precede the positioned window in the Z order. This parameter must be a window handle or one of the following values.
            /// HWND_BOTTOM
            /// Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost window, the window loses its topmost status and is placed at the bottom of all other windows.
            /// HWND_NOTOPMOST
            /// Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect if the window is already a non-topmost window.
            /// HWND_TOP
            /// Places the window at the top of the Z order.
            /// HWND_TOPMOST
            /// Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated.
            /// </param>
            /// <param name="X">Specifies the new position of the left side of the window, in client coordinates.</param>
            /// <param name="Y">Specifies the new position of the top of the window, in client coordinates.</param>
            /// <param name="cx">Specifies the new width of the window, in pixels.</param>
            /// <param name="cy">Specifies the new height of the window, in pixels.</param>
            /// <param name="uFlags">Specifies the window sizing and positioning flags. This parameter can be a combination of the following values. 
            /// <para>SWP_ASYNCWINDOWPOS:
            /// If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request. </para>
            /// <para>SWP_DEFERERASE:
            /// Prevents generation of the WM_SYNCPAINT message. </para>
            /// <para>SWP_DRAWFRAME:
            /// Draws a frame (defined in the window's class description) around the window.</para>
            /// <para>SWP_FRAMECHANGED:
            /// Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.</para>
            /// <para>SWP_HIDEWINDOW:
            /// Hides the window.</para>
            /// <para>SWP_NOACTIVATE:
            /// Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).</para>
            /// <para>SWP_NOCOPYBITS:
            /// Discards the entire contents of the client area. If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.</para>
            /// <para>SWP_NOMOVE:
            /// Retains the current position (ignores X and Y parameters).</para>
            /// <para>SWP_NOOWNERZORDER:
            /// Does not change the owner window's position in the Z order.</para>
            /// <para>SWP_NOREDRAW:
            /// Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved. When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.</para>
            /// <para>SWP_NOREPOSITION:
            /// Same as the SWP_NOOWNERZORDER flag.</para>
            /// <para>SWP_NOSENDCHANGING:
            /// Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</para>
            /// <para>SWP_NOSIZE:
            /// Retains the current size (ignores the cx and cy parameters).</para>
            /// <para>SWP_NOZORDER:
            /// Retains the current Z order (ignores the hWndInsertAfter parameter).</para>
            /// <para>SWP_SHOWWINDOW
            /// Displays the window.</para>
            /// </param>
            /// <returns>If the function succeeds, the return value is nonzero.
            /// If the function fails, the return value is zero. To get extended error information, call GetLastError.</returns>
            [DllImport("user32.dll")]
            internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

            /// <summary>
            /// Retrieves information about the specified combo box.
            /// </summary>
            /// <param name="hwndCombo">A handle to the combo box.</param>
            /// <param name="pcbi">A pointer to a COMBOBOXINFO structure that receives the information. You must set COMBOBOXINFO.cbSize before calling this function.</param>
            /// <returns>If the function succeeds, the return value is nonzero.
            /// If the function fails, the return value is zero. To get extended error information, call GetLastError.</returns>
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool GetComboBoxInfo(IntPtr hwndCombo, ref COMBOBOXINFO pcbi);

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

        internal static void SetControlText(IntPtr handle, string text) => NativeMethods.SetWindowText(handle, text);

        internal static void SetCaptionTheme(IntPtr handle, bool isDarkTheme)
        {
            if (!OSUtils.IsWindows10OrLater)
                return;

            // Windows 10 1903 or later
            if (OSUtils.IsWindows10Build1903OrLater)
            {
                int attributeValueBufferSize = sizeof(int);
                IntPtr attributeValueBuffer = Marshal.AllocHGlobal(attributeValueBufferSize);
                Marshal.WriteInt32(attributeValueBuffer, isDarkTheme ? 1 : 0);

                try
                {
                    WINDOWCOMPOSITIONATTRIBDATA windowCompositionAttributeData = new()
                    {
                        Attrib = WINDOWCOMPOSITIONATTRIB.WCA_USE_DARK_MODE_COLORS,
                        pvData = attributeValueBuffer,
                        cbData = (uint)attributeValueBufferSize
                    };

                    NativeMethods.SetWindowCompositionAttribute(handle, ref windowCompositionAttributeData);
                }
                finally
                {
                    Marshal.FreeHGlobal(attributeValueBuffer);
                }
            }
            // Windows 10 1809 only
            else
                NativeMethods.SetProp(handle, "UseImmersiveDarkModeColors", new IntPtr(isDarkTheme ? 1 : 0));

            if (OSUtils.IsWindows11OrLater)
                return;

            // Invalidating the caption area to force the system to redraw it. Needed when the theme is changed and the window is already visible.
            // In Windows 11 this is not needed anymore. If we still do it, the caption area is changed immediately, without the fade effect.
            bool isActivated = handle == NativeMethods.GetForegroundWindow();
            NativeMethods.SendMessage(handle, Constants.WM_NCACTIVATE, new IntPtr(isActivated ? 0 : 1), IntPtr.Zero);
            NativeMethods.SendMessage(handle, Constants.WM_NCACTIVATE, new IntPtr(isActivated ? 1 : 0), IntPtr.Zero);
        }

        internal static uint SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam) => NativeMethods.SendMessage(hWnd, msg, wParam, lParam);

        internal static IntPtr GetWindowDC(IntPtr hWnd) => NativeMethods.GetWindowDC(hWnd);

        internal static bool ReleaseDC(IntPtr hWnd, IntPtr hDC) => NativeMethods.ReleaseDC(hWnd, hDC);

        internal static bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags)
            => NativeMethods.SetWindowPos(hWnd, hWndInsertAfter, X, Y, cx, cy, uFlags);

        internal static bool GetComboBoxInfo(IntPtr hwndCombo, ref COMBOBOXINFO pcbi)
        {
            try
            {
                return NativeMethods.GetComboBoxInfo(hwndCombo, ref pcbi);
            }
            catch (Exception e) when (!e.IsCritical())
            {
                return false;
            }
        }

        #endregion
    }
}
