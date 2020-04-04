#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageInfo.cs
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
using System.Linq;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    public sealed class ImageInfo : ImageInfoBase
    {
        #region Fields
        
        private Icon icon;

        #endregion

        #region Properties

        #region Public Properties

        public ImageInfoType Type { get; private set; }

        public Icon Icon { get => Get<Icon>(); set => Set(value); }

        public string FileName { get => Get<string>(); set => Set(value); }

        public ImageFrameInfo[] Frames { get => Get<ImageFrameInfo[]>(); set => Set(value); }
        
        public bool IsMultiRes => !Frames.IsNullOrEmpty() && Type.In(ImageInfoType.MultiRes, ImageInfoType.Icon);
        public bool HasFrames => !Frames.IsNullOrEmpty() && Type.In(ImageInfoType.Pages, ImageInfoType.Animation, ImageInfoType.MultiRes, ImageInfoType.Icon);

        #endregion

        #region Internal Properties

        internal bool IsMetafile => Image is Metafile;

        #endregion

        #endregion

        #region Constructors

        public ImageInfo(ImageInfoType imageType)
        {
            if (!imageType.IsDefined())
                throw new ArgumentOutOfRangeException(nameof(imageType), PublicResources.EnumOutOfRange(imageType));
            Type = imageType;
        }

        public ImageInfo(Image image)
        {
            InitFromImage(image);
            SetModified(false);
        }

        public ImageInfo(Icon icon)
        {
            InitFromIcon(icon);
            SetModified(false);
        }

        #endregion

        #region Methods

        #region Public Methods

        public Image GenerateImage()
        {
            throw new NotImplementedException("GenerateImage");
        }

        public Icon GenerateIcon()
        {
            throw new NotImplementedException("GenerateIcon");
            // TODO: exception if not Icon? Maybe it can be generated in any case
            // Maybe not even needed: SaveIcon generates it from frames if needed (and maybe even cleaner so)
        }

        #endregion

        #region Protected Methods

        protected override ValidationResultsCollection DoValidation()
        {
            if (Type == ImageInfoType.None)
                return new ValidationResultsCollection();

            ValidationResultsCollection result = base.DoValidation();
            ImageFrameInfo[] frames = Frames;
            if (Type.In(ImageInfoType.Pages, ImageInfoType.Animation, ImageInfoType.MultiRes))
            {
                if (frames.IsNullOrEmpty())
                    result.AddError(nameof(Frames), PublicResources.CollectionEmpty);
                else if (frames.Any(f => f.Image == null))
                    result.AddError(nameof(Frames), PublicResources.ArgumentContainsNull);
            }

            bool hasFrames = HasFrames;
            if (Type == ImageInfoType.Icon)
            {
                if (Icon == null && !hasFrames)
                    result.AddError(nameof(Icon), PublicResources.ArgumentNull);
            }
            else if (Image == null && !hasFrames)
                result.AddError(nameof(Image), PublicResources.ArgumentNull);

            return result;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Icon?.Dispose();
                FreeFrames();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void InitFromImage(Image image)
        {
            if (image == null)
                return;

            InitMeta(image);
            Image = image;
            Bitmap bmp = image as Bitmap;
            ImageFrameInfo[] frames;

            // icon
            if (bmp != null && RawFormat == ImageFormat.Icon.Guid)
            {
                Bitmap[] iconImages = bmp.ExtractBitmaps();
                if (iconImages.Length == 1)
                {
                    iconImages[0].Dispose();
                    Type = ImageInfoType.SingleImage;
                    return;
                }

                frames = new ImageFrameInfo[iconImages.Length];
                Frames = frames;
                for (int i = 0; i < iconImages.Length; i++)
                    frames[i] = new ImageFrameInfo(iconImages[i]) { RawFormat = RawFormat };

                // selecting the maximum size for the compound icon
                Size = frames.Aggregate(Size.Empty, (size, i) => size.Width >= i.Size.Width ? size : i.Size);
                Type = ImageInfoType.MultiRes;
                return;
            }

            // other image: check if it has multiple frames
            FrameDimension dimension = null;
            Guid[] dimensions = image.FrameDimensionsList;
            if (dimensions.Length > 0)
            {
                if (dimensions[0] == FrameDimension.Page.Guid)
                    dimension = FrameDimension.Page;
                else if (dimensions[0] == FrameDimension.Time.Guid)
                    dimension = FrameDimension.Time;
                else if (dimensions[0] == FrameDimension.Resolution.Guid)
                    dimension = FrameDimension.Resolution;
            }

            // single image, unknown dimension or not bitmap
            int frameCount = dimension != null ? image.GetFrameCount(dimension) : 0;
            if (frameCount <= 1 || dimension == null || bmp == null)
            {
                Type = ImageInfoType.SingleImage;
                return;
            }

            // multiple frames
            byte[] times = null;
            Bitmap bitmap = (Bitmap)image;
            Type = dimension == FrameDimension.Time ? ImageInfoType.Animation
                : dimension == FrameDimension.Page ? ImageInfoType.Pages
                : ImageInfoType.MultiRes;

            // in case of animation there is a compound image
            if (dimension == FrameDimension.Time)
                times = image.GetPropertyItem(0x5100).Value;

            frames = new ImageFrameInfo[frameCount];
            Frames = frames;
            for (int frame = 0; frame < frameCount; frame++)
            {
                image.SelectActiveFrame(dimension, frame);
                frames[frame] = new ImageFrameInfo(bitmap.CloneCurrentFrame()) { RawFormat = RawFormat };
                if (times != null)
                {
                    int duration = BitConverter.ToInt32(times, frame << 2);
                    duration = duration == 0 ? 100 : duration * 10;
                    frames[frame].Duration = duration;
                }
            }

            image.SelectActiveFrame(dimension, 0);
        }

        private void InitFromIcon(Icon icon)
        {
            if (icon == null)
                return;

            Icon = icon;
            Type = ImageInfoType.Icon;

            // obtaining icon images in original format
            Bitmap[] iconImagesOrigFormat = icon.ExtractBitmaps(true);

            if (iconImagesOrigFormat.Length == 1)
            {
                InitMeta(iconImagesOrigFormat[0]);
                iconImagesOrigFormat[0].Dispose();
                RawFormat = ImageFormat.Icon.Guid;

                // TODO: On demand
                //// when single-image icon, creating image from bitmap to avoid bad quality of system icons (pixel format will be system dependent)
                //PreviewImage = icon.ToAlphaBitmap();
                return;
            }

            // TODO: not needed, not used
            //// generating image from icon
            //try
            //{
            //    PreviewImage = icon.ToMultiResBitmap();
            //    InitMeta(PreviewImage);
            //}
            //catch (ArgumentException)
            //{
            //    // In Windows XP it can happen that multi-res bitmap throws an exception even if PNG images are uncompressed
            //    PreviewImage = icon.ExtractNearestBitmap(new Size(UInt16.MaxValue, UInt16.MaxValue), PixelFormat.Format32bppArgb);
            //    InitMeta(PreviewImage);
            //    RawFormat = ImageFormat.Icon.Guid;
            //}

            Bitmap[] iconImagesAlpha = icon.ExtractBitmaps();
            Frames = new ImageFrameInfo[iconImagesOrigFormat.Length];
            for (int i = 0; i < Frames.Length; i++)
            {
                Frames[i] = new ImageFrameInfo(iconImagesOrigFormat[i]) { Image = iconImagesAlpha[i] };
                bool isCompressed = icon.IsCompressed(i);
                RawFormat = isCompressed ? ImageFormat.Png.Guid : ImageFormat.Bmp.Guid;
                // setting raw format explicitly as it is at icons
                int bpp = icon.GetBitsPerPixel(i);
                Frames[i].PixelFormat = bpp == 1 ? PixelFormat.Format1bppIndexed
                    : bpp == 4 ? PixelFormat.Format4bppIndexed
                    : bpp == 8 ? PixelFormat.Format8bppIndexed
                    : bpp == 24 ? PixelFormat.Format24bppRgb
                    : !isCompressed ? PixelFormat.Format32bppRgb
                    : PixelFormat.Format32bppArgb;
                iconImagesOrigFormat[i].Dispose();
            }

            // selecting the maximum size for the compound icon
            Size = Frames.Aggregate(Size.Empty, (size, image) => size.Width >= image.Size.Width ? size : image.Size);
        }

        private void FreeFrames()
        {
            ImageFrameInfo[] frames = Frames;
            if (frames == null)
                return;
            foreach (ImageFrameInfo frame in frames)
                frame.Dispose();
            Frames = null;
        }

        #endregion

        #endregion
    }
}
