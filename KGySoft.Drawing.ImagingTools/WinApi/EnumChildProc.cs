#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: EnumChildProc.cs
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

#endregion

namespace KGySoft.Drawing.ImagingTools.WinApi
{
    /// <summary>
    /// An application-defined callback function used with the EnumChildWindows function. It receives the child window handles.
    /// The WNDENUMPROC type defines a pointer to this callback function. EnumChildProc is a placeholder for the application-defined function name.
    /// </summary>
    /// <param name="hWnd">A handle to a child window of the parent window specified in EnumChildWindows.</param>
    /// <param name="lParam">The application-defined value given in EnumChildWindows.</param>
    /// <returns>To continue enumeration, the callback function must return TRUE; to stop enumeration, it must return FALSE.</returns>
    internal delegate bool EnumChildProc(IntPtr hWnd, IntPtr lParam);
}