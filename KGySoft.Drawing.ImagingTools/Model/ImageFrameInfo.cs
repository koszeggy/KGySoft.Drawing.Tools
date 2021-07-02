#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageFrameInfo.cs
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
        /// Initializes a new instance of the <see cref="ImageFrameInfo"/> class from a <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="bitmap">The bitmap that contains the image of the current frame.</param>
        public ImageFrameInfo(Bitmap? bitmap)
        {
            Image = bitmap;
            InitMeta(bitmap);
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
        protected override ValidationResultsCollection DoValidation()
        {
            ValidationResultsCollection result = base.DoValidation();
            if (Image == null)
                result.AddError(nameof(Image), PublicResources.ArgumentNull);
            return result;
        }

        #endregion
    }
}