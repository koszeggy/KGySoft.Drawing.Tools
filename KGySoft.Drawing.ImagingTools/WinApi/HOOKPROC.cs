#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: HOOKPROC.cs
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

#endregion

namespace KGySoft.Drawing.ImagingTools.WinApi
{
    /// <summary>
    /// An application-defined or library-defined callback function used with the SetWindowsHookEx function. The system calls this function after the SendMessage function is called.
    /// The hook procedure can examine the message; it cannot modify it.
    /// The HOOKPROC type defines a pointer to this callback function. CallWndRetProc is a placeholder for the application-defined or library-defined function name.
    /// </summary>
    /// <param name="nCode">Specifies whether the hook procedure must process the message. If nCode is HC_ACTION, the hook procedure must process the message.
    /// If nCode is less than zero, the hook procedure must pass the message to the CallNextHookEx function without further processing and must return the value returned by CallNextHookEx.</param>
    /// <param name="wParam">Specifies whether the message is sent by the current process. If the message is sent by the current process, it is nonzero; otherwise, it is NULL.</param>
    /// <param name="lParam">A pointer to a <see cref="CWPRETSTRUCT"/> structure that contains details about the message.</param>
    /// <returns></returns>
    internal delegate IntPtr HOOKPROC(int nCode, IntPtr wParam, IntPtr lParam);
}