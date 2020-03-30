#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageDataTypes.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2019 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution. If not, then this file is considered as
//  an illegal copy.
//
//  Unauthorized copying of this file, via any medium is strictly prohibited.
///////////////////////////////////////////////////////////////////////////////

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers
{
    internal enum ImageDataTypes : byte
    {
        /// <summary>
        /// Stream contains a single image data
        /// </summary>
        SingleImage,

        /// <summary>
        /// Stream contains separated pages, no compound image is available (first page is shown instead)
        /// </summary>
        Pages,

        /// <summary>
        /// First image data contains no image, further ones contain icon images.
        /// First image should be built after deserialization.
        /// Bitmap can be saved as ICO.
        /// </summary>
        IconBitmap,

        /// <summary>
        /// First image data contains no image, further ones contain animation frames.
        /// Animation should built (or imitated) after deserialization.
        /// </summary>
        Animation,
    }
}
