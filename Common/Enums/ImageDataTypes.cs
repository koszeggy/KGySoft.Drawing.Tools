﻿namespace KGySoft.DebuggerVisualizers.Common
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

        /// <summary>
        /// Stream contains an icon, then ImageData instances without images.
        /// </summary>
        Icon
    }
}
