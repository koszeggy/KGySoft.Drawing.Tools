#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CWPRETSTRUCT.cs
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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

#endregion

namespace KGySoft.Drawing.ImagingTools.WinApi
{
    /// <summary>
    /// Defines the message parameters passed to a WH_CALLWNDPROCRET hook procedure, HOOKPROC callback function.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "WinAPI")]
    internal struct CWPRETSTRUCT
    {
        #region Fields

        internal IntPtr lResult;
        internal IntPtr lParam;
        internal IntPtr wParam;
        internal uint message;
        internal IntPtr hwnd;

        #endregion
    }
}