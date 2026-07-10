#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CIEXYZ.cs
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

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

#endregion

namespace KGySoft.Drawing.ImagingTools.WinApi
{
    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("ReSharper", "IdentifierTypo", Justification = "WinAPI")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "WinAPI")]
    internal struct CIEXYZ
    {
        #region Fields

        internal uint ciexyzX; //FXPT2DOT30
        internal uint ciexyzY; //FXPT2DOT30
        internal uint ciexyzZ; //FXPT2DOT30

        #endregion
    }
}