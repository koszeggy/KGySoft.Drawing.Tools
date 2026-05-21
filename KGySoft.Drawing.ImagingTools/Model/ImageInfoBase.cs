#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageInfoBase.cs
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
        /// Gets or sets an <see cref="System.Drawing.Icon"/> instance associated with this <see cref="ImageInfoBase"/> instance.
        /// If this instance is an <see cref="ImageInfo"/>, this property may contain a multi resolution <see cref="System.Drawing.Icon"/>.
        /// </summary>
        public Icon? Icon { get => Get<Icon?>(); set => Set(value); }

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

        #region Methods

        #region Public Methods

        /// <summary>
        /// Gets or creates the image of this <see cref="ImageInfoBase"/>.
        /// </summary>
        /// <returns>An <see cref="System.Drawing.Image"/> that represents the image of this <see cref="ImageInfoBase"/> instance.</returns>
        /// <exception cref="InvalidOperationException">The object is in an invalid state (the <see cref="ValidatingObjectBase.IsValid"/> property returns <see langword="false"/>).</exception>
        public abstract Image? GetCreateImage();

        /// <summary>
        /// Gets or creates the icon of this <see cref="ImageInfoBase"/>.
        /// </summary>
        /// <returns>An <see cref="System.Drawing.Icon"/> that represents the icon of this <see cref="ImageInfo"/> instance.</returns>
        /// <exception cref="InvalidOperationException">The object is in an invalid state (the <see cref="ValidatingObjectBase.IsValid"/> property returns <see langword="false"/>).</exception>
        public abstract Icon? GetCreateIcon();

        #endregion

        #region Internal Methods

        internal Bitmap? GetCreateBitmap() => GetCreateImage() as Bitmap;
        internal abstract Image? GetImage();

        #endregion

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
                Icon?.Dispose();

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
