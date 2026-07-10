#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: BITMAPV5HEADER.cs
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
    internal struct BITMAPV5HEADER
    {
        #region Fields

        internal uint bV5Size;
        internal int bV5Width;
        internal int bV5Height;
        internal ushort bV5Planes;
        internal ushort bV5BitCount;
        internal uint bV5Compression;
        internal uint bV5SizeImage;
        internal int bV5XPelsPerMeter;
        internal int bV5YPelsPerMeter;
        internal uint bV5ClrUsed;
        internal uint bV5ClrImportant;
        internal uint bV5RedMask;
        internal uint bV5GreenMask;
        internal uint bV5BlueMask;
        internal uint bV5AlphaMask;
        internal uint bV5CSType;
        internal CIEXYZTRIPLE bV5Endpoints;
        internal uint bV5GammaRed;
        internal uint bV5GammaGreen;
        internal uint bV5GammaBlue;
        internal uint bV5Intent;
        internal uint bV5ProfileData;
        internal uint bV5ProfileSize;
        internal uint bV5Reserved;

        #endregion
    }
}