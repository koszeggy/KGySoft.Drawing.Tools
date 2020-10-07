#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: PixelFormatExtensions.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2020 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution. If not, then this file is considered as
//  an illegal copy.
//
//  Unauthorized copying of this file, via any medium is strictly prohibited.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System.Drawing.Imaging;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal static class PixelFormatExtensions
    {
        #region Methods

        internal static bool CanBeDithered(this PixelFormat dstFormat) => dstFormat.ToBitsPerPixel() <= 16 && dstFormat != PixelFormat.Format16bppGrayScale;
        internal static bool HasAlpha(this PixelFormat pixelFormat) => (pixelFormat & PixelFormat.Alpha) == PixelFormat.Alpha;
        internal static bool IsIndexed(this PixelFormat pixelFormat) => (pixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed;

        #endregion
    }
}