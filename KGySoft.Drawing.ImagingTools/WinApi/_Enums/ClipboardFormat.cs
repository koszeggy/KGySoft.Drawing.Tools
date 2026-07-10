#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ClipboardFormat.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2026 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Suppressions

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

#endregion

namespace KGySoft.Drawing.ImagingTools.WinApi
{
    internal enum ClipboardFormat : uint
    {
        None,

        CF_TEXT = 1u,
        CF_BITMAP = 2u,
        CF_METAFILEPICT = 3u,
        CF_SYLK = 4u,
        CF_DIF = 5u,
        CF_TIFF = 6u,
        CF_OEMTEXT = 7u,
        CF_DIB = 8u,
        CF_PALETTE = 9u,
        CF_PENDATA = 10u,
        CF_RIFF = 11u,
        CF_WAVE = 12u,
        CF_UNICODETEXT = 13u,
        CF_ENHMETAFILE = 14u,
        CF_HDROP = 15u,
        CF_LOCALE = 16u,
        CF_DIBV5 = 17u,
    }
}