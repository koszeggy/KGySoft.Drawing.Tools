#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageInfo.cs
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.Imaging;
using KGySoft.WinForms;

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

        #region Fields

        private MetafileType? metafileType;

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets the type of the stored image.
        /// </summary>
        public ImageInfoType Type { get; private set; }

        /// <summary>
        /// Gets or sets the image to be displayed or saved
        /// when debugging the corresponding <see cref="System.Drawing.Image"/> or <see cref="System.Drawing.Icon"/> instance.
        /// </summary>
        public Image? Image { get => Get<Image?>(); set => Set(value); }

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
        public bool HasFrames => !Frames.IsNullOrEmpty() && Type is ImageInfoType.Pages or ImageInfoType.Animation or ImageInfoType.MultiRes or ImageInfoType.Icon;

        #endregion

        #region Internal Properties

        internal bool IsMultiRes => !Frames.IsNullOrEmpty() && Type is ImageInfoType.MultiRes or ImageInfoType.Icon;
        internal bool IsMetafile => Image is Metafile;
        
        // Not like other properties, because it always can be restored from the metafile, even after (de)serialization
        internal MetafileType MetafileType
        {
            get
            {
                if (metafileType == null)
                {
                    try
                    {
                        metafileType = (Image as Metafile)?.GetMetafileHeader().Type ?? default;
                    }
                    catch (Exception e) when (!e.IsCriticalGdi())
                    {
                        metafileType = MetafileType.Invalid;
                    }
                }

                return metafileType.Value;
            }
        }

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
        /// <param name="cloneFrames"><see langword="true"/> to clone each frame when initializing <see cref="Frames"/>; otherwise, <see langword="false"/>.
        /// When <see langword="false"/>, then <see cref="ImageFrameInfo.Image"/> will not be set in <see cref="Frames"/> so you must extract the corresponding frames from
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
        /// <param name="cloneImages"><see langword="true"/>to clone each frame when initializing <see cref="Frames"/>; otherwise, <see langword="false"/>.
        /// When <see langword="false"/>, then <see cref="ImageInfoBase.Icon"/> will not be set in <see cref="Frames"/> so you must extract the corresponding frames from
        /// the specified <paramref name="icon"/> manually to access the actual frame content.</param>
        public ImageInfo(Icon? icon, bool cloneImages)
        {
            InitFromIcon(icon, cloneImages);
            SetModified(false);
        }

        #endregion

        #region Methods

        #region Static Methods

        [return: NotNullIfNotNull(nameof(data))]
        internal static ImageInfo? EnsureFormat(object? data, AllowedImageTypes allowedTypes, bool allowMultiFrame = true, bool detectAlpha = false)
        {
            #region Local Methods

            static void TryRestoreAlpha(ref Bitmap bitmap)
            {
                Debug.Assert(bitmap.PixelFormat == PixelFormat.Format32bppRgb);
                Bitmap? result = null;

                // Using BitmapDataFactory rather than GetReadableBitmapData, so we can reinterpret the pixel format.
                // It's important that we lock with the actual format so the content is not copied.
                //using IReadableBitmapData bitmapData = bitmap.GetReadableBitmapData();
                var bitmapData = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                try
                {
                    using IReadableBitmapData reinterpretedBitmapData = BitmapDataFactory.CreateBitmapData(bitmapData.Scan0, bitmap.Size, bitmapData.Stride, KnownPixelFormat.Format32bppArgb);
                    Color32 firstPixel = reinterpretedBitmapData.GetColor32(0, 0);
                    if (firstPixel.A is not (Byte.MinValue or Byte.MaxValue))
                    {
                        result = reinterpretedBitmapData.ToBitmap();
                        return;
                    }

                    // First pixel is opaque: assuming no alpha unless there is a pixel with alpha
                    // First pixel is transparent: if the whole bitmap seems to be completely transparent, we assume no alpha
                    IReadableBitmapDataRowMovable row = reinterpretedBitmapData.FirstRow;
                    do
                    {
                        for (int x = 0; x < row.Width; x++)
                        {
                            if (row[x].A != firstPixel.A)
                            {
                                result = reinterpretedBitmapData.ToBitmap();
                                return;
                            }
                        }
                    } while (row.MoveNextRow());
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                    if (result != null)
                    {
                        bitmap.Dispose();
                        bitmap = result;
                    }
                }
            }

            static Metafile BitmapToMetafile(Bitmap bitmap)
            {
                using Graphics refGraph = Graphics.FromHwnd(IntPtr.Zero);
                IntPtr hdc = refGraph.GetHdc();
                var rect = new RectangleF(Point.Empty, bitmap.Size);

                // NOTE: Using EmfOnly type would allow to select the interpolation at rendering time, but if the bitmap has alpha, it may cause ugly black edges when
                //       drawing into a larger bitmap. Also, drawing an enlarged metafile of EmfOnly type clips the rightmost column and the bottom row.
                //       On the other hand, EmfPlusDual stores the interpolation mode in the metafile, which cannot be changed at rendering time.
                //       Using NN interpolation here, because others look blurry even on 100% zoom.
                //       This the way an enlarged result will remain pixelated even with smoothing, but at least the rendering will be faster. Smoothing will still work for shrinking.
                var result = new Metafile(hdc, rect, MetafileFrameUnit.Pixel, EmfType.EmfPlusDual, "BitmapAsEmf");
                using (var g = Graphics.FromImage(result))
                {
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.DrawImage(bitmap, rect);
                }

                refGraph.ReleaseHdc(hdc); //cleanup
                return result;
            }

            static Metafile IconToMetafile(Icon icon)
            {
                using Bitmap bitmap = icon.ToAlphaBitmap();
                return BitmapToMetafile(bitmap);
            }

            static Icon MetafileToIcon(Metafile metafile)
            {
                Size size = metafile.Size;
                int max = Math.Max(size.Width, size.Height);
                if (max > 256)
                {
                    float scale = 256f / max;
                    size = size.Scale(scale);
                    if (size.Width < 1)
                        size.Width = 1;
                    if (size.Height < 1)
                        size.Height = 1;
                }

                // not using the Bitmap(Image, Size) constructor, see the details in AsBitmap
                var bitmap = new Bitmap(size.Width, size.Height);
                using var g = Graphics.FromImage(bitmap);
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(metafile, new Rectangle(Point.Empty, size));
                return bitmap.ToIcon(Math.Min(256, max), true);
            }

            #endregion

            // Special handling if the metafile has a bitmap raw format (may happen on deserialization on .NET Runtime 2.x).
            // In such case the actual type switches to Bitmap after cloning
            if (data is Metafile mf && !mf.RawFormat.In(ImageFormat.Wmf, ImageFormat.Emf))
            {
                data = mf.Clone();
                mf.Dispose();
            }

            switch (data)
            {
                case null:
                    return null;

                case Metafile metafile:
                    if ((allowedTypes & AllowedImageTypes.Metafile) != AllowedImageTypes.None)
                        return new ImageInfo(metafile);

                    if ((allowedTypes & AllowedImageTypes.Bitmap) != AllowedImageTypes.None)
                    {
                        Bitmap bitmap = metafile.AsBitmap();
                        metafile.Dispose();
                        return new ImageInfo(bitmap);
                    }

                    Debug.Assert((allowedTypes & AllowedImageTypes.Icon) != 0);
                    //Icon asIcon = metafile.ToIcon(Math.Min(256, Math.Max(size.Width, size.Height)), true); - TODO: now this fails for huge metafiles
                    Icon asIcon = MetafileToIcon(metafile);
                    metafile.Dispose();
                    return new ImageInfo(asIcon);

                case Bitmap bitmap:
                    if (detectAlpha && bitmap.PixelFormat == PixelFormat.Format32bppRgb)
                        TryRestoreAlpha(ref bitmap);

                    if ((allowedTypes & AllowedImageTypes.Bitmap) != AllowedImageTypes.None)
                    {
                        if (!allowMultiFrame && bitmap.FrameDimensionsList is Guid[] { Length: > 0 } dimensions && bitmap.GetFrameCount(new FrameDimension(dimensions[0])) > 1)
                        {
                            Bitmap frame = bitmap.CloneCurrentFrame();
                            bitmap.Dispose();
                            return new ImageInfo(frame);
                        }

                        Debug.Assert(bitmap.RawFormat.Guid != ImageFormat.Icon.Guid);
                        return new ImageInfo(bitmap);
                    }

                    Size size = bitmap.Size;
                    if ((allowedTypes & AllowedImageTypes.Icon) != 0)
                    {
                        Icon icon = bitmap.ToIcon(Math.Min(256, Math.Max(size.Width, size.Height)), true);
                        bitmap.Dispose();
                        return new ImageInfo(icon);
                    }

                    Debug.Assert((allowedTypes & AllowedImageTypes.Metafile) != 0);
                    Metafile asMetafile = BitmapToMetafile(bitmap);
                    bitmap.Dispose();
                    return new ImageInfo(asMetafile);

                case Icon icon:
                    if ((allowedTypes & AllowedImageTypes.Icon) != AllowedImageTypes.None)
                        return new ImageInfo(icon);

                    if ((allowedTypes & AllowedImageTypes.Bitmap) != AllowedImageTypes.None)
                    {
                        Bitmap bitmap = OSHelper.IsWindows && allowMultiFrame ? icon.ToMultiResBitmap() : icon.ToAlphaBitmap();
                        icon.Dispose();
                        return new ImageInfo(bitmap);
                    }

                    Debug.Assert((allowedTypes & AllowedImageTypes.Metafile) != 0);
                    asMetafile = IconToMetafile(icon);
                    icon.Dispose();
                    return new ImageInfo(asMetafile);

                default:
                    throw new InvalidOperationException(Res.InternalError($"Unexpected type in EnsureFormat: {data.GetType()}"));
            }
        }

        #endregion

        #region Instance Methods

        #region Public Methods

        /// <summary>
        /// Gets the <see cref="Image"/> property, or generates an <see cref="System.Drawing.Image"/> from
        /// the <see cref="ImageInfoBase.Icon"/> or <see cref="Frames"/> properties.
        /// </summary>
        /// <returns>An <see cref="System.Drawing.Image"/> that represents the possible compound image of this <see cref="ImageInfo"/> instance.
        /// When a new image is created, then the return value will be the new value of the <see cref="Image"/> property as well.</returns>
        /// <exception cref="InvalidOperationException">The object is in an invalid state (the <see cref="ValidatingObjectBase.IsValid"/> property returns <see langword="false"/>).</exception>
        public override Image? GetCreateImage()
        {
            Image? image = Image;
            if (image != null)
                return image;
            if (Type == ImageInfoType.None || !(Type == ImageInfoType.Icon || HasFrames))
                return null;
            return Image = GenerateImage();
        }

        /// <summary>
        /// Gets or creates the icon if this instance represents an icon and the <see cref="ImageInfoBase.Icon"/> property is <see langword="null"/>.
        /// </summary>
        /// <returns>An <see cref="Icon"/> that represents the possible icon of this <see cref="ImageInfo"/> instance.
        /// When a new icon is created, then the return value will be the new value of the <see cref="ImageInfoBase.Icon"/> property as well.</returns>
        /// <exception cref="InvalidOperationException">The object is in an invalid state (the <see cref="ValidatingObjectBase.IsValid"/> property returns <see langword="false"/>).</exception>
        public override Icon? GetCreateIcon()
        {
            Icon? icon = Icon;
            if (icon != null)
                return icon;
            if (Type == ImageInfoType.None)
                return null;
            return Icon = GenerateIcon();
        }

        #endregion

        #region Internal Methods

        internal override Image? GetImage() => Image;

        internal ImageInfo AsIcon()
        {
            if (Type is not (ImageInfoType.None or ImageInfoType.Icon))
                Type = ImageInfoType.Icon;
            return this;
        }

        internal ImageInfo AsImage()
        {
            if (Type == ImageInfoType.Icon)
                Type = HasFrames ? ImageInfoType.MultiRes : ImageInfoType.SingleImage;
            return this;
        }

        // The following methods are needed when processing frames on a background thread, as the image might be accessed in the UI thread concurrently.
        // The view cooperatively locks on the images as well to avoid a possible "bitmap region is already locked" error.
        // Otherwise, they could be simply replaced by a lambda, selecting the Image/Icon of the frames.

        internal IEnumerable<Image> IterateFrameImages(AsyncTaskBase? task = null)
        {
            if (Frames is not ImageFrameInfo[] frames)
                yield break;
            foreach (ImageFrameInfo frame in frames)
            {
                if (task?.IsCanceled == true)
                    yield break;
                Bitmap image = frame.GetCreateBitmap()!;
                lock (image) // it's alright, the lock is released when moving to the next frame
                    yield return image;
            }
        }

        internal IEnumerable<Icon> IterateFrameIcons(AsyncTaskBase task)
        {
            // single icon
            if (Frames is not ImageFrameInfo[] frames)
            {
                yield return GetCreateIcon()!;
                yield break;
            }

            // multi-res icon
            foreach (ImageFrameInfo frame in frames)
            {
                if (task.IsCanceled)
                    yield break;
                yield return frame.GetCreateIcon(); // possible lock on the image occurs in the call
            }
        }

        internal IEnumerable<IReadableBitmapData> IterateFramesBitmapData(AsyncTaskBase task)
        {
            if (Frames is not ImageFrameInfo[] frames)
                yield break;
            foreach (ImageFrameInfo frame in frames)
            {
                if (task.IsCanceled)
                    yield break;

                // Due to yield return both the lock and the using is released when moving to the next frame. 
                Bitmap bitmap = frame.GetCreateBitmap()!;
                lock (bitmap)
                {
                    using IReadableBitmapData bitmapData = bitmap.GetReadableBitmapData();
                    yield return bitmapData;
                }
            }
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
            bool hasFrames = !frames.IsNullOrEmpty();

            if (hasFrames)
            {
                if (frames!.Contains(null))
                    result.AddError(nameof(Frames), PublicResources.ArgumentContainsNull);
                else if (Type is ImageInfoType.Pages or ImageInfoType.Animation && frames!.Any(f => f.Image == null))
                    result.AddError(nameof(Frames), Res.ErrorMessageImageInfoEmptyFrameImage);
                else if (Type is ImageInfoType.Icon && frames!.Any(f => f.Image == null && f.Icon == null))
                    result.AddError(nameof(Frames), Res.ErrorMessageImageInfoEmptyFrameIcon);
            }
            else
            {
                if (Type is ImageInfoType.Pages or ImageInfoType.Animation or ImageInfoType.MultiRes)
                    result.AddError(nameof(Frames), PublicResources.CollectionEmpty);
                if (Image == null && Icon == null)
                    result.AddError(nameof(Icon), Res.ErrorMessageImageInfoEmpty);
            }

            return result;
        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == nameof(Image))
                metafileType = null;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;
            if (disposing)
            {
                Image?.Dispose();
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
                dimension = dimensions[0] == FrameDimension.Page.Guid ? FrameDimension.Page
                    : dimensions[0] == FrameDimension.Time.Guid ? FrameDimension.Time
                    : dimensions[0] == FrameDimension.Resolution.Guid ? FrameDimension.Resolution
                    : null;
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
                frames[frame] = new ImageFrameInfo(cloneFrames ? bitmap.CloneCurrentFrame() : bitmap) { RawFormat = RawFormat };
                if (times != null)
                {
                    int startIndex = frame << 2;
                    int duration;
                    if (times.Length >= startIndex + 4)
                        duration = BitConverter.ToInt32(times, frame << 2);
                    else // Mono/libgdiplus: the delay can be queried for each frame separately
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
                imageInfo.PixelFormat = iconInfo.BitsPerPixel switch
                {
                    1 => PixelFormat.Format1bppIndexed,
                    4 => PixelFormat.Format4bppIndexed,
                    8 => PixelFormat.Format8bppIndexed,
                    24 => PixelFormat.Format24bppRgb,
                    _ => PixelFormat.Format32bppArgb
                };
                imageInfo.Palette = iconInfo.Palette;
                imageInfo.Size = iconInfo.Size;
                imageInfo.RawFormat = iconInfo.IsCompressed ? ImageFormat.Png.Guid : ImageFormat.Bmp.Guid;
            }

            #endregion

            if (icon == null)
                return;

            Icon = icon;
            Type = ImageInfoType.Icon;
            PointF scale = ScaleHelper.SystemScale;
            HorizontalRes = scale.X * ScaleHelper.DefaultDpi;
            VerticalRes = scale.Y * ScaleHelper.DefaultDpi;

            // obtaining icon images in original format
            IconInfo[] iconInfo = icon.GetIconInfo();

            if (iconInfo.Length == 1)
            {
                InitIconMeta(iconInfo[0], this);
                RawFormat = ImageFormat.Icon.Guid;
                return;
            }

            Icon?[]? iconImages = cloneImages ? icon.ExtractIcons() : null;
            Debug.Assert(!cloneImages || iconInfo.Length == iconImages!.Length);
            var frames = new ImageFrameInfo[iconInfo.Length];
            for (int i = 0; i < frames.Length; i++)
            {
                frames[i] = new ImageFrameInfo(null) { Icon = iconImages?[i] };
                InitIconMeta(iconInfo[i], frames[i]);

                // In Windows XP all icon images are uncompressed so displaying just Icon
                if (!OSHelper.IsWindowsVistaOrLater)
                    RawFormat = ImageFormat.Icon.Guid;
            }

            Frames = frames;

            // determining data for the compound icon
            Size = frames.Aggregate(Size.Empty, (size, frame) => size.Width >= frame.Size.Width ? size : frame.Size);
            PixelFormat = Frames.Aggregate(PixelFormat.Undefined, (pf, frame) => pf.ToBitsPerPixel() >= frame.PixelFormat.ToBitsPerPixel() ? pf : frame.PixelFormat);
            RawFormat = ImageFormat.Icon.Guid;
        }

        private Bitmap GenerateImage()
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
                    IterateFrameImages().SaveAsMultipageTiff(ms);
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

                // Unlike in VM.SaveGif, we use SaveAsAnimatedGif here rather than the lower-level encoder, because this operation is not canceled
                case ImageInfoType.Animation:
                    ms = new MemoryStream();
                    IterateFrameImages().SaveAsAnimatedGif(ms, Frames!.Select(f => TimeSpan.FromMilliseconds(f.Duration)));
                    ms.Position = 0;
                    return new Bitmap(ms);

                default:
                    throw new InvalidOperationException(Res.InternalError($"Unexpected type in {nameof(GenerateImage)}: {Type}"));
            }
        }

        private Icon GenerateIcon()
        {
            Debug.Assert(Icon == null);

            if (!IsValid)
            {
                ValidationResult error = ValidationResults.Errors[0];
                throw new InvalidOperationException(PublicResources.PropertyMessage(error.PropertyName, error.Message));
            }

            if (Frames is ImageFrameInfo[] { Length: > 0 } frames)
                return Icons.Combine(frames.Select(f => f.GetCreateIcon()));

            Image image = Image!;
            lock (image)
                return image.ToIcon();
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

        #endregion
    }
}
