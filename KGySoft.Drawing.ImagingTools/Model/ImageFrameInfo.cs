#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageFrameInfo.cs
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

using System.Drawing;
using KGySoft.ComponentModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    /// <summary>
    /// Represents a frame of a multi-frame image.
    /// </summary>
    /// <seealso cref="ImageInfo" />
    /// <seealso cref="ImageInfoBase" />
    public sealed class ImageFrameInfo : ImageInfoBase
    {
        #region Properties

        /// <summary>
        /// If the corresponding image represents an animation, then gets or sets the duration belongs to this frame.
        /// </summary>
        public int Duration { get => Get<int>(); set => Set(value); }

        #endregion

        #region Constructors

        #region Public Constructors
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageFrameInfo"/> class from an <see cref="Image"/>.
        /// </summary>
        /// <param name="image">The image that contains the image of the current frame.</param>
        public ImageFrameInfo(Image? image)
        {
            Image = image;
            InitMeta(image);
        }

        #endregion

        #region Internal Constructors

        internal ImageFrameInfo(ImageFrameInfo other) : base(other)
        {
            Duration = other.Duration;
        }

        #endregion

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override ValidationResultsCollection DoValidation() => Image == null
            ? new ValidationResultsCollection { new(nameof(Image), PublicResources.PropertyNull(nameof(Image))) }
            : ValidationResultsCollection.Empty;

        #endregion
    }
}