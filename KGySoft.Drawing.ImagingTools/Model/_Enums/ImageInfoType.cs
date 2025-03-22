#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageInfoType.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2025 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    /// <summary>
    /// Represents the image type held by an <see cref="ImageInfo"/> instance.
    /// </summary>
    public enum ImageInfoType
    {
        /// <summary>
        /// Represents an empty <see cref="ImageInfo"/> without any image.
        /// </summary>
        None,

        /// <summary>
        /// The image represents a single frame of image.
        /// </summary>
        SingleImage,

        /// <summary>
        /// The image represents an image of multiple pages.
        /// </summary>
        Pages,

        /// <summary>
        /// The image has multiple resolutions.
        /// </summary>
        MultiRes,

        /// <summary>
        /// The image represents an animation.
        /// </summary>
        Animation,

        /// <summary>
        /// The image is an icon of one or more images.
        /// </summary>
        Icon,
    }
}
