#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageInfoBase.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2025 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;

using KGySoft.ComponentModel;
using KGySoft.Reflection;

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    /// <summary>
    /// Represents a base descriptor class for debugging an <see cref="System.Drawing.Image"/> or <see cref="Icon"/> instance.
    /// </summary>
    /// <seealso cref="ImageInfo" />
    /// <seealso cref="ImageFrameInfo" />
    public abstract class ImageInfoBase : ValidatingObjectBase
    {
        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the image to be displayed or saved
        /// when debugging the corresponding <see cref="System.Drawing.Image"/> or <see cref="Icon"/> instance.
        /// </summary>
        public Image? Image { get => Get<Image?>(); set => Set(value); }

        /// <summary>
        /// Gets or sets the horizontal resolution to be displayed
        /// when debugging the corresponding <see cref="System.Drawing.Image"/> or <see cref="Icon"/> instance.
        /// </summary>
        public float HorizontalRes { get => Get<float>(); set => Set(value); }

        /// <summary>
        /// Gets or sets the vertical resolution to be displayed
        /// when debugging the corresponding <see cref="System.Drawing.Image"/> or <see cref="Icon"/> instance.
        /// </summary>
        public float VerticalRes { get => Get<float>(); set => Set(value); }

        /// <summary>
        /// Gets or sets the size to be displayed
        /// when debugging the corresponding <see cref="System.Drawing.Image"/> or <see cref="Icon"/> instance.
        /// </summary>
        public Size Size { get => Get<Size>(); set => Set(value); }

        /// <summary>
        /// Gets or sets the pixel format to be displayed
        /// when debugging the corresponding <see cref="System.Drawing.Image"/> or <see cref="Icon"/> instance.
        /// </summary>
        public PixelFormat PixelFormat { get => Get<PixelFormat>(); set => Set(value); }

        /// <summary>
        /// Gets or sets the palette color entries to be displayed
        /// when debugging the corresponding <see cref="System.Drawing.Image"/> or <see cref="Icon"/> instance.
        /// </summary>
        [AllowNull]
        public Color[] Palette { get => Get(Reflector.EmptyArray<Color>()); set => Set(value ?? Reflector.EmptyArray<Color>()); }

        /// <summary>
        /// Gets or sets the ID of the raw format to be displayed
        /// when debugging the corresponding <see cref="System.Drawing.Image"/> or <see cref="Icon"/> instance.
        /// </summary>
        public Guid RawFormat { get => Get<Guid>(); set => Set(value); }

        #endregion

        #region Internal Properties

        internal int BitsPerPixel => PixelFormat.ToBitsPerPixel();

        #endregion

        #endregion

        #region Construction and Destruction

        #region Constructors

        private protected ImageInfoBase()
        {
        }

        private protected ImageInfoBase(ImageInfoBase other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other), PublicResources.ArgumentNull);

            if (other.Image is Image image)
                Image = (Image)image.Clone();
            if (other.Palette.Length > 0)
                Palette = (Color[])Palette.Clone();
            HorizontalRes = other.HorizontalRes;
            VerticalRes = other.VerticalRes;
            PixelFormat = other.PixelFormat;
            RawFormat = other.RawFormat;
        }

        #endregion

        #region Destructor


        /// <inheritdoc/>
        ~ImageInfoBase() => Dispose(false);

        #endregion

        #endregion

        #region Methods

        #region Protected Methods

        /// <summary>
        /// Releases the resources held by this instance.
        /// </summary>
        /// <param name="disposing"><see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;
            if (disposing)
                Image?.Dispose();
            base.Dispose(disposing);
        }

        #endregion

        #region Private Protected Methods

        private protected void InitMeta(Image? image)
        {
            if (image == null)
                return;

            Size = image.Size;
            HorizontalRes = image.HorizontalResolution;
            VerticalRes = image.VerticalResolution;
            PixelFormat = image.PixelFormat;
            Palette = image is Metafile ? Reflector.EmptyArray<Color>() : image.Palette.Entries;
            RawFormat = image.RawFormat.Guid;
        }

        #endregion

        #endregion
    }
}
