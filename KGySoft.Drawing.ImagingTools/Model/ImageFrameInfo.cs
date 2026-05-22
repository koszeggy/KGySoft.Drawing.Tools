#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageFrameInfo.cs
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

using System;
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
        /// Gets or sets the image of this frame.
        /// </summary>
        public Bitmap? Image { get => Get<Bitmap?>(); set => Set(value); }

        /// <summary>
        /// If the corresponding image represents an animation, then gets or sets the duration belongs to this frame.
        /// </summary>
        public int Duration { get => Get<int>(); set => Set(value); }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageFrameInfo"/> class from an <see cref="Image"/>.
        /// </summary>
        /// <param name="image">The image that contains the image of the current frame.</param>
        public ImageFrameInfo(Bitmap? image)
        {
            Image = image;
            InitMeta(image);
        }

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Gets or creates the image from the <see cref="ImageInfoBase.Icon"/> property if this instance represents an icon image
        /// and the <see cref="Image"/> property is <see langword="null"/>.
        /// </summary>
        /// <returns>An <see cref="System.Drawing.Image"/> that represents the image of this <see cref="ImageFrameInfo"/> instance.
        /// When a new image is created, then the return value will be the new value of the <see cref="Image"/> property as well.</returns>
        /// <exception cref="InvalidOperationException">The object is in an invalid state (the <see cref="ValidatingObjectBase.IsValid"/> property returns <see langword="false"/>).</exception>
        public override Image GetCreateImage()
        {
            Image? image = Image;
            if (image != null)
                return image;

            if (!IsValid)
            {
                ValidationResult error = ValidationResults.Errors[0];
                throw new InvalidOperationException(PublicResources.PropertyMessage(error.PropertyName, error.Message));
            }

            return Image = Icon!.ToAlphaBitmap();
        }

        /// <summary>
        /// Gets or creates the icon if this instance represents an icon and the <see cref="ImageInfoBase.Icon"/> property is <see langword="null"/>.
        /// </summary>
        /// <returns>An <see cref="Icon"/> that represents the possible icon of this <see cref="ImageFrameInfo"/> instance.
        /// When a new icon is created, then the return value will be the new value of the <see cref="ImageInfoBase.Icon"/> property as well.</returns>
        /// <exception cref="InvalidOperationException">The object is in an invalid state (the <see cref="ValidatingObjectBase.IsValid"/> property returns <see langword="false"/>).</exception>
        public override Icon GetCreateIcon()
        {
            Icon? icon = Icon;
            if (icon != null)
                return icon;

            if (!IsValid)
            {
                ValidationResult error = ValidationResults.Errors[0];
                throw new InvalidOperationException(PublicResources.PropertyMessage(error.PropertyName, error.Message));
            }

            // locking on the image, because it might be used by the UI
            Bitmap image = Image!;
            lock (image)
                return Icon = image.ToIcon();
        }

        #endregion

        #region Internal Methods

        internal override Image? GetImage() => Image;

        #endregion

        #region Protected Methods

        /// <inheritdoc/>
        protected override ValidationResultsCollection DoValidation() => Image == null && Icon == null
            ? new ValidationResultsCollection { new(nameof(Image), Res.ErrorMessageImageInfoEmpty) }
            : ValidationResultsCollection.Empty;

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;
            if (disposing)
                Image?.Dispose();
            base.Dispose(disposing);
        }

        #endregion

        #endregion
    }
}