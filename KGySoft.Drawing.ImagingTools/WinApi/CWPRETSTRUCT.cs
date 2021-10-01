#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CWPRETSTRUCT.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Runtime.InteropServices;

#endregion

namespace KGySoft.Drawing.ImagingTools.WinApi
{
    [StructLayout(LayoutKind.Sequential)]
    // ReSharper disable once InconsistentNaming
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