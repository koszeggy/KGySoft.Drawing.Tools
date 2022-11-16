#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageInfo.cs
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

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    /// <summary>
    /// Represents a descriptor for an <see cref="Image"/> or <see cref="System.Drawing.Icon"/> instance that can be used
    /// to display arbitrary debug information for images.
    /// </summary>
    /// <seealso cref="ImageInfoBase" />
    public sealed class ImageInfo : ImageInfoBase
    {
        #region Constants

        private const int propertyTagFrameDelay = 0x5100; // https://docs.microsoft.com/en-us/dotnet/api/system.drawing.imaging.propertyitem.id

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets the type of the stored image.
        /// </summary>
        public ImageInfoType Type { get; private set; }

        /// <summary>
        /// Gets or sets an <see cref="System.Drawing.Icon"/> instance associated with this <see cref="ImageInfo"/> instance.
        /// </summary>
        public Icon? Icon { get => Get<Icon?>(); set => Set(value); }

        /// <summary>
        /// Gets or sets a file name associated with this <see cref="ImageInfo"/> instance.
        /// </summary>
        public string? FileName { get => Get<string?>(); set => Set(value); }

        /// <summary>
        /// If this <see cref="ImageInfo"/> instance represents a multi-frame image, then gets or sets the frames belong to the image.
        /// </summary>
        public ImageFrameInfo[]? Frames { get => Get<ImageFrameInfo[]?>(); set => Set(value); }

        /// <summary>
        /// Gets whether this instance represents a multi-frame image and has frames.
        /// </summary>
        public bool HasFrames => !Frames.IsNullOrEmpty() && Type.In(ImageInfoType.Pages, ImageInfoType.Animation, ImageInfoType.MultiRes, ImageInfoType.Icon);

        #endregion

        #region Internal Properties

        internal bool IsMultiRes => !Frames.IsNullOrEmpty() && Type.In(ImageInfoType.MultiRes, ImageInfoType.Icon);
        internal bool IsMetafile => Image is Metafile;

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes an empty instance of the <see cref="ImageInfo"/> class.
        /// The properties are expected to be initialized individually. Use the <see cref="ValidatingObjectBase.IsValid"/>
        /// property to check whether this instance is initialized properly.
        /// </summary>
        /// <param name="imageType">Type of the image.</param>
        public ImageInfo(ImageInfoType imageType)
        {
            if (!imageType.IsDefined())
                throw new ArgumentOutOfRangeException(nameof(imageType), PublicResources.EnumOutOfRange(imageType));
            Type = imageType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageInfo"/> class from an <see cref="Image"/>.
        /// </summary>
        /// <param name="image">The image to be used for the initialization.</param>
        public ImageInfo(Image? image) : this(image, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageInfo"/> class from an <see cref="Image"/>.
        /// </summary>
        /// <param name="image">The image to be used for the initialization.</param>
        /// <param name="cloneFrames"><see langword="true"/>&#160;to clone each frame when initializing <see cref="Frames"/>; otherwise, <see langword="false"/>.
        /// When <see langword="false"/>, then <see cref="ImageInfoBase.Image"/> will not be set in <see cref="Frames"/> so you must extract the corresponding frames from
        /// the specified <paramref name="image"/> manually to access the actual frame content.</param>
        public ImageInfo(Image? image, bool cloneFrames)
        {
            InitFromImage(image, cloneFrames);
            SetModified(false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageInfo"/> class from an <see cref="System.Drawing.Icon"/>.
        /// </summary>
        /// <param name="icon">The icon to be used for the initialization.</param>
        public ImageInfo(Icon? icon) : this(icon, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageInfo"/> class from an <see cref="System.Drawing.Icon"/>.
        /// </summary>
        /// <param name="icon">The icon to be used for the initialization.</param>
        /// <param name="cloneImages"><see langword="true"/>&#160;to clone each frame when initializing <see cref="Frames"/>; otherwise, <see langword="false"/>.
        /// When <see langword="false"/>, then <see cref="ImageInfoBase.Image"/> will not be set in <see cref="Frames"/> so you must extract the corresponding frames from
        /// the specified <paramref name="icon"/> manually to access the actual frame content.</param>
        public ImageInfo(Icon? icon, bool cloneImages)
        {
            InitFromIcon(icon, cloneImages);
            SetModified(false);
        }

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Gets or creates the compound image from the <see cref="Frames"/> if this instance represents a multi-frame image
        /// and the <see cref="ImageInfoBase.Image"/> property is <see langword="null"/>.
        /// </summary>
        /// <returns>An <see cref="Image"/> that represents the possible compound image of this <see cref="ImageInfo"/> instance.
        /// When a new image is created, then the return value will be the new value of the <see cref="ImageInfoBase.Image"/> property as well.</returns>
        /// <exception cref="InvalidOperationException">The object is in an invalid state (the <see cref="ValidatingObjectBase.IsValid"/> property returns <see langword="false"/>).</exception>
        public Image? GetCreateImage()
        {
            Image? image = Image;
            if (image != null)
                return image;
            if (Type == ImageInfoType.None || !(Type == ImageInfoType.Icon || HasFrames))
                return null;
            image = GenerateImage();
            return Image = image;
        }

        /// <summary>
        /// Gets or creates the icon if this instance represents an icon and the <see cref="Icon"/> property is <see langword="null"/>.
        /// </summary>
        /// <returns>An <see cref="Icon"/> that represents the possible icon of this <see cref="ImageInfo"/> instance.
        /// When a new icon is created, then the return value will be the new value of the <see cref="Icon"/> property as well.</returns>
        /// <exception cref="InvalidOperationException">The object is in an invalid state (the <see cref="ValidatingObjectBase.IsValid"/> property returns <see langword="false"/>).</exception>
        public Icon? GetCreateIcon()
        {
            Icon? icon = Icon;
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

        /// <inheritdoc/>
        protected override ValidationResultsCollection DoValidation()
        {
            if (Type == ImageInfoType.None)
                return ValidationResultsCollection.Empty;

            var result = new ValidationResultsCollection();
            ImageFrameInfo[]? frames = Frames;
            if (Type.In(ImageInfoType.Pages, ImageInfoType.Animation, ImageInfoType.MultiRes))
            {
                if (frames.IsNullOrEmpty())
                    result.AddError(nameof(Frames), PublicResources.CollectionEmpty);
                else if (frames!.Any(f => f.Image == null))
                    result.AddError(nameof(Frames), PublicResources.ArgumentContainsNull);
            }

            bool hasFrames = HasFrames;
            if (Type == ImageInfoType.Icon)
            {
                if (Icon == null && Image == null && !hasFrames)
                    result.AddError(nameof(Icon), PublicResources.PropertyNull(nameof(Icon)));
            }
            else if (Image == null && !hasFrames)
                result.AddError(nameof(Image), PublicResources.PropertyNull(nameof(Image)));

            return result;
        }

        /// <inheritdoc/>
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

        [SuppressMessage("ReSharper", "PossibleUnintendedReferenceComparison",
            Justification = "The dimension variable is compared with the references we set earlier")]
        private void InitFromImage(Image? image, bool cloneFrames)
        {
            if (image == null)
                return;

            InitMeta(image);
            Image = image;
            Bitmap? bmp = image as Bitmap;
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
            FrameDimension? dimension = null;
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
            byte[]? times = null;
            Bitmap bitmap = (Bitmap)image;
            Type = dimension == FrameDimension.Time ? ImageInfoType.Animation
                : dimension == FrameDimension.Page ? ImageInfoType.Pages
                : ImageInfoType.MultiRes;

            // in case of animation there is a compound image
            if (dimension == FrameDimension.Time)
                times = image.GetPropertyItem(propertyTagFrameDelay)?.Value;

            frames = new ImageFrameInfo[frameCount];
            Frames = frames;
            for (int frame = 0; frame < frameCount; frame++)
            {
                image.SelectActiveFrame(dimension, frame);
                frames[frame] = new ImageFrameInfo(cloneFrames ? bitmap.CloneCurrentFrame() : image) { RawFormat = RawFormat };
                if (times != null)
                {
                    int startIndex = frame << 2;
                    int duration;
                    if (times.Length >= startIndex + 4)
                        duration = BitConverter.ToInt32(times, frame << 2);
                    else // Mono/libgdiplus: the delay can be queried all frames separately
                    {
                        byte[]? time = image.GetPropertyItem(propertyTagFrameDelay)?.Value;
                        duration = time?.Length >= 4 ? BitConverter.ToInt32(time, 0) : 0;
                    }
                    duration = duration == 0 ? 100 : duration * 10;
                    frames[frame].Duration = duration;
                }

                // preventing frame from disposing if it was not cloned
                if (!cloneFrames)
                    frames[frame].Image = null;
            }

            image.SelectActiveFrame(dimension, 0);
        }

        private void InitFromIcon(Icon? icon, bool cloneImages)
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
            PointF defaultDpi = OSUtils.SystemDpi;
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

            Bitmap?[]? iconImages = cloneImages ? icon.ExtractBitmaps() : null;
            Debug.Assert(!cloneImages || iconInfo.Length == iconImages!.Length);
            var frames = new ImageFrameInfo[iconInfo.Length];
            for (int i = 0; i < frames.Length; i++)
            {
                frames[i] = new ImageFrameInfo(iconImages?[i]);
                InitIconMeta(iconInfo[i], frames[i]);

                // In Windows XP all icon images are uncompressed so displaying just Icon
                if (!OSUtils.IsVistaOrLater)
                    RawFormat = ImageFormat.Icon.Guid;
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
                throw new InvalidOperationException(PublicResources.PropertyMessage(error.PropertyName, error.Message));
            }

            Debug.Assert(HasFrames || Type == ImageInfoType.Icon, "Frames or icon are expected here");
            switch (Type)
            {
                case ImageInfoType.Pages:
                    var ms = new MemoryStream();
                    Frames!.Select(f => f.Image!).SaveAsMultipageTiff(ms);
                    ms.Position = 0;
                    return new Bitmap(ms);

                case ImageInfoType.MultiRes:
                case ImageInfoType.Icon:
                    try
                    {
                        return GetCreateIcon()!.ToMultiResBitmap();
                    }
                    catch (ArgumentException)
                    {
                        // In Windows XP it can happen that multi-res bitmap throws an exception even if PNG images are uncompressed
                        return GetCreateIcon()!.ExtractNearestBitmap(new Size(UInt16.MaxValue, UInt16.MaxValue), PixelFormat.Format32bppArgb);
                    }

                case ImageInfoType.Animation:
                    ms = new MemoryStream();
                    ImageFrameInfo[] frames = Frames!;
                    frames.Select(f => f.Image!).SaveAsAnimatedGif(ms, frames.Select(f => TimeSpan.FromMilliseconds(f.Duration)));
                    ms.Position = 0;
                    return new Bitmap(ms);

                default:
                    throw new InvalidOperationException(Res.InternalError($"Unexpected type in {nameof(GenerateImage)}: {Type}"));
            }
        }

        private Icon GenerateIcon()
        {
            if (!IsValid)
            {
                ValidationResult error = ValidationResults.Errors[0];
                throw new InvalidOperationException(PublicResources.PropertyMessage(error.PropertyName, error.Message));
            }

            return !HasFrames ? Image!.ToIcon() : Icons.Combine(Frames!.Select(f => (Bitmap)f.Image!));
        }

        private void FreeFrames()
        {
            ImageFrameInfo[]? frames = Frames;
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
