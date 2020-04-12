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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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

        public Image GetCreateImage()
        {
            Image image = Image;
            if (image != null)
                return image;
            if (Type == ImageInfoType.None || !(Type == ImageInfoType.Icon || HasFrames))
                return null;
            image = GenerateImage();
            return Image = image;
        }

        public Icon GetCreateIcon()
        {
            Icon icon = Icon;
            if (icon != null)
                return icon;
            if (Type == ImageInfoType.None)
                return null;
            icon = GenerateIcon();
            return Icon = icon;
        }

        #endregion

        #region Internal Methods

        internal ImageInfo AsIcon()
        {
            if (!Type.In(ImageInfoType.None, ImageInfoType.Icon))
                Type = ImageInfoType.Icon;
            return this;
        }

        internal ImageInfo AsImage()
        {
            if (Type == ImageInfoType.Icon)
                Type = HasFrames ? ImageInfoType.MultiRes : ImageInfoType.SingleImage;
            return this;
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
            if (IsDisposed)
                return;

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
            #region Local Methods

            static void InitIconMeta(IconInfo iconInfo, ImageInfoBase imageInfo)
            {
                imageInfo.PixelFormat = iconInfo.BitsPerPixel == 1 ? PixelFormat.Format1bppIndexed
                    : iconInfo.BitsPerPixel == 4 ? PixelFormat.Format4bppIndexed
                    : iconInfo.BitsPerPixel == 8 ? PixelFormat.Format8bppIndexed
                    : iconInfo.BitsPerPixel == 24 ? PixelFormat.Format24bppRgb
                    : PixelFormat.Format32bppArgb;
                imageInfo.Palette = iconInfo.Palette;
                imageInfo.Size = iconInfo.Size;
                imageInfo.RawFormat = iconInfo.IsCompressed ? ImageFormat.Png.Guid : ImageFormat.Bmp.Guid;
            }

            #endregion

            if (icon == null)
                return;

            Icon = icon;
            Type = ImageInfoType.Icon;
            PointF defaultDpi = WindowsUtils.SystemDpi;
            HorizontalRes = defaultDpi.X;
            VerticalRes = defaultDpi.Y;

            // obtaining icon images in original format
            IconInfo[] iconInfo = icon.GetIconInfo();

            if (iconInfo.Length == 1)
            {
                InitIconMeta(iconInfo[0], this);
                RawFormat = ImageFormat.Icon.Guid;
                return;
            }

            Bitmap[] iconImages = icon.ExtractBitmaps();
            Debug.Assert(iconInfo.Length == iconImages.Length);
            var frames = new ImageFrameInfo[iconInfo.Length];
            for (int i = 0; i < frames.Length; i++)
            {
                frames[i] = new ImageFrameInfo(iconImages[i]);
                InitIconMeta(iconInfo[i], frames[i]);
            }

            Frames = frames;

            // determining data for the compound icon
            Size = Frames.Aggregate(Size.Empty, (size, image) => size.Width >= image.Size.Width ? size : image.Size);
            PixelFormat = Frames.Aggregate(PixelFormat.Undefined, (pf, image) => pf.ToBitsPerPixel() >= image.PixelFormat.ToBitsPerPixel() ? pf : image.PixelFormat);
            RawFormat = ImageFormat.Icon.Guid;
        }

        private Image GenerateImage()
        {
            if (!IsValid)
            {
                ValidationResult error = ValidationResults.Errors[0];

                // ReSharper disable once LocalizableElement - the message comes from resource
                throw new InvalidOperationException($"{error.PropertyName}: {error.Message}");
            }

            Debug.Assert(HasFrames || Type == ImageInfoType.Icon, "Frames or icon are expected here");
            switch (Type)
            {
                case ImageInfoType.Pages:
                    var ms = new MemoryStream();
                    Frames.Select(f => f.Image).SaveAsMultipageTiff(ms);
                    ms.Position = 0;
                    return new Bitmap(ms);

                case ImageInfoType.MultiRes:
                case ImageInfoType.Icon:
                    try
                    {
                        return GetCreateIcon().ToMultiResBitmap();
                    }
                    catch (ArgumentException)
                    {
                        // In Windows XP it can happen that multi-res bitmap throws an exception even if PNG images are uncompressed
                        return GetCreateIcon().ExtractNearestBitmap(new Size(UInt16.MaxValue, UInt16.MaxValue), PixelFormat.Format32bppArgb);
                    }

                case ImageInfoType.Animation:
                    throw new NotSupportedException(Res.ErrorMessageAnimGifNotSupported);

                default:
                    throw new InvalidOperationException(Res.InternalError($"Unexpected type in {nameof(GenerateImage)}: {Type}"));
            }
        }

        private Icon GenerateIcon()
        {
            if (!IsValid)
            {
                ValidationResult error = ValidationResults.Errors[0];

                // ReSharper disable once LocalizableElement - the message comes from resource
                throw new InvalidOperationException($"{error.PropertyName}: {error.Message}");
            }

            return !HasFrames ? Image.ToIcon() : Icons.Combine(Frames.Select(f => (Bitmap)f.Image));
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
