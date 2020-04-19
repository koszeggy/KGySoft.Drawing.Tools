#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageInfoBase.cs
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

using System;
using System.Drawing;
using System.Drawing.Imaging;
using KGySoft.ComponentModel;
using KGySoft.Reflection;

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    public class ImageInfoBase : ValidatingObjectBase
    {
        #region Fields
        
        private Color[] palette;

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the actual image to save or work with.
        /// </summary>
        public Image Image { get => Get<Image>(); set => Set(value); }

        public float HorizontalRes { get => Get<float>(); set => Set(value); }

        public float VerticalRes { get => Get<float>(); set => Set(value); }

        public Size Size { get => Get<Size>(); set => Set(value); }

        public PixelFormat PixelFormat { get => Get<PixelFormat>(); set => Set(value); }

        public Color[] Palette { get => Get(Reflector.EmptyArray<Color>()); set => Set(value ?? Reflector.EmptyArray<Color>()); }

        public Guid RawFormat { get => Get<Guid>(); set => Set(value); }

        #endregion

        #region Internal Properties

        internal int BitsPerPixel => Image.GetPixelFormatSize(PixelFormat);

        #endregion

        #endregion

        #region Destructor

        ~ImageInfoBase()
        {
            Dispose(false);
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected void InitMeta(Image image)
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

        protected override ValidationResultsCollection DoValidation() => new ValidationResultsCollection();

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
