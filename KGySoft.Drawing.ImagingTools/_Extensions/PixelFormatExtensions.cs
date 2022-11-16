#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: PixelFormatExtensions.cs
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

using System.Drawing.Imaging;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal static class PixelFormatExtensions
    {
        #region Constants
        
        internal const PixelFormat Format32bppCmyk = (PixelFormat)0x200F;

        #endregion

        #region Methods

        internal static bool CanBeDithered(this PixelFormat dstFormat) => dstFormat.ToBitsPerPixel() <= 16 && dstFormat != PixelFormat.Format16bppGrayScale;

        // ReSharper disable BitwiseOperatorOnEnumWithoutFlags
        internal static bool HasAlpha(this PixelFormat pixelFormat) => (pixelFormat & PixelFormat.Alpha) == PixelFormat.Alpha;
        internal static bool IsIndexed(this PixelFormat pixelFormat) => (pixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed;
        internal static bool IsWide(this PixelFormat pixelFormat) => (pixelFormat & PixelFormat.Extended) == PixelFormat.Extended;
        // ReSharper restore BitwiseOperatorOnEnumWithoutFlags

        #endregion
    }
}