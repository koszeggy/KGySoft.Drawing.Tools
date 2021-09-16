#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageVisualizerViewModel.cs
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

using System.Diagnostics.CodeAnalysis;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
#if !NET35
using System.Runtime.Versioning; 
#endif
using System.Text;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class ImageVisualizerViewModel : ViewModelBase, IViewModel<ImageInfo>, IViewModel<Image?>, IViewModel<Icon?>, IViewModel<Bitmap?>, IViewModel<Metafile?>
    {
        #region Constants

        private const string stateImage = "Image";
        private const string stateToolTipText = "ToolTipText";
        private const string stateVisible = "Visible";
        private const string stateInterval = "Interval";

        #endregion

        #region Fields

        #region Static Fields
        
        private static readonly ImageCodecInfo[] encoderCodecs = ImageCodecInfo.GetImageEncoders();
        private static readonly ImageCodecInfo[] decoderCodecs = ImageCodecInfo.GetImageDecoders();

        #endregion

        #region Instance Fields

        private readonly AllowedImageTypes imageTypes;

        private ImageInfo imageInfo = new ImageInfo(ImageInfoType.None);
        private bool keepAliveImageInfo;
        private int currentFrame = -1;
        private bool isOpenFilterUpToDate;
        private Size currentResolution;
        private bool deferUpdateInfo;
        private string? notificationId;

        #endregion

        #endregion

        #region Properties

        #region Internal Properties

        internal Image? Image
        {
            get => imageInfo.GetCreateImage();
            set => SetImageInfo(new ImageInfo(value));
        }

        internal Icon? Icon
        {
            get => imageInfo.Icon;
            set => SetImageInfo(new ImageInfo(value));
        }

        [AllowNull]
        internal ImageInfo ImageInfo
        {
            get => imageInfo;
            set => SetImageInfo(value ?? new ImageInfo(ImageInfoType.None));
        }

        internal Image? PreviewImage { get => Get<Image?>(); set => Set(value); }
        internal bool ReadOnly { get => Get<bool>(); set => Set(value); }
        internal string? TitleCaption { get => Get<string?>(); set => Set(value); }
        internal string? InfoText { get => Get<string?>(); set => Set(value); }
        internal string? Notification { get => Get<string?>(); private set => Set(value); }
        internal bool AutoZoom { get => Get<bool>(); set => Set(value); }
        internal float Zoom { get => Get(1f); set => Set(value); }
        internal bool SmoothZooming { get => Get<bool>(); set => Set(value); }
        internal bool IsCompoundView { get => Get(true); set => Set(value); }
        internal bool IsAutoPlaying { get => Get<bool>(); set => Set(value); }
        internal string? OpenFileFilter { get => Get<string?>(); set => Set(value); }
        internal string? SaveFileFilter { get => Get<string?>(); set => Set(value); }
        internal int SaveFileFilterIndex { get => Get<int>(); set => Set(value); }
        internal string? SaveFileDefaultExtension { get => Get<string?>(); set => Set(value); }

        internal Func<Rectangle>? GetScreenRectangleCallback { get => Get<Func<Rectangle>?>(); set => Set(value); }
        internal Func<Size>? GetViewSizeCallback { get => Get<Func<Size>?>(); set => Set(value); }
        internal Func<Size>? GetImagePreviewSizeCallback { get => Get<Func<Size>?>(); set => Set(value); }
        internal Action<Size>? ApplyViewSizeCallback { get => Get<Action<Size>?>(); set => Set(value); }
        internal Func<string?>? SelectFileToOpenCallback { get => Get<Func<string?>?>(); set => Set(value); }
        internal Func<string?>? SelectFileToSaveCallback { get => Get<Func<string?>?>(); set => Set(value); }
        internal Action? UpdatePreviewImageCallback { get => Get<Action?>(); set => Set(value); }
        internal Func<ImageInfoType, Image>? GetCompoundViewIconCallback { get => Get<Func<ImageInfoType, Image>?>(); set => Set(value); }

        internal ICommandState SetAutoZoomCommandState => Get(() => new CommandState());
        internal ICommandState SetSmoothZoomingCommandState => Get(() => new CommandState());
        internal ICommandState OpenFileCommandState => Get(() => new CommandState());
        internal ICommandState SaveFileCommandState => Get(() => new CommandState { Enabled = false });
        internal ICommandState ClearCommandState => Get(() => new CommandState { Enabled = false });
        internal ICommandState SetCompoundViewCommandState => Get(() => new CommandState { [stateVisible] = false });
        internal ICommandState AdvanceAnimationCommandState => Get(() => new CommandState());
        internal ICommandState PrevImageCommandState => Get(() => new CommandState());
        internal ICommandState NextImageCommandState => Get(() => new CommandState());
        internal ICommandState ShowPaletteCommandState => Get(() => new CommandState { Enabled = false });
        internal ICommandState CountColorsCommandState => Get(() => new CommandState { Enabled = false });
        internal ICommandState EditBitmapCommandState => Get(() => new CommandState { Enabled = false });

        internal ICommand SetAutoZoomCommand => Get(() => new SimpleCommand<bool>(OnSetAutoZoomCommand));
        internal ICommand SetSmoothZoomingCommand => Get(() => new SimpleCommand<bool>(OnSetSmoothZoomingCommand));
        internal ICommand ViewImagePreviewSizeChangedCommand => Get(() => new SimpleCommand(OnViewImagePreviewSizeChangedCommand));
        internal ICommand OpenFileCommand => Get(() => new SimpleCommand(OnOpenFileCommand));
        internal ICommand SaveFileCommand => Get(() => new SimpleCommand(OnSaveFileCommand));
        internal ICommand ClearCommand => Get(() => new SimpleCommand(OnClearCommand));
        internal ICommand SetCompoundViewCommand => Get(() => new SimpleCommand<bool>(OnSetCompoundViewCommand));
        internal ICommand AdvanceAnimationCommand => Get(() => new SimpleCommand(OnAdvanceAnimationCommand));
        internal ICommand PrevImageCommand => Get(() => new SimpleCommand(OnPrevImageCommand));
        internal ICommand NextImageCommand => Get(() => new SimpleCommand(OnNextImageCommand));
        internal ICommand ShowPaletteCommand => Get(() => new SimpleCommand(OnShowPaletteCommand));
        internal ICommand ManageInstallationsCommand => Get(() => new SimpleCommand(OnManageInstallationsCommand));
        internal ICommand SetLanguageCommand => Get(() => new SimpleCommand(OnSetLanguageCommand));
        internal ICommand RotateLeftCommand => Get(() => new SimpleCommand(OnRotateLeftCommand));
        internal ICommand RotateRightCommand => Get(() => new SimpleCommand(OnRotateRightCommand));
        internal ICommand ResizeBitmapCommand => Get(() => new SimpleCommand(OnResizeBitmapCommand));
        internal ICommand AdjustColorSpaceCommand => Get(() => new SimpleCommand(OnAdjustColorSpaceCommand));
        internal ICommand CountColorsCommand => Get(() => new SimpleCommand(OnCountColorsCommand));
        internal ICommand AdjustBrightnessCommand => Get(() => new SimpleCommand(OnAdjustBrightnessCommand));
        internal ICommand AdjustContrastCommand => Get(() => new SimpleCommand(OnAdjustContrastCommand));
        internal ICommand AdjustGammaCommand => Get(() => new SimpleCommand(OnAdjustGammaCommand));
        internal ICommand ShowAboutCommand => Get(() => new SimpleCommand(OnShowAboutCommand));
        internal ICommand VisitWebSiteCommand => Get(() => new SimpleCommand(() => Process.Start("https://kgysoft.net")));
        internal ICommand VisitGitHubCommand => Get(() => new SimpleCommand(() => Process.Start("https://github.com/koszeggy/KGySoft.Drawing.Tools")));
        internal ICommand VisitMarketplaceCommand => Get(() => new SimpleCommand(() => Process.Start("https://marketplace.visualstudio.com/items?itemName=KGySoft.drawing-debugger-visualizers")));
        internal ICommand SubmitResourcesCommand => Get(() => new SimpleCommand(() => Process.Start("https://github.com/koszeggy/KGySoft.Drawing.Tools/issues/new?assignees=&labels=&template=submit-resources.md&title=%5BRes%5D")));
        internal ICommand ShowEasterEggCommand => Get(() => new SimpleCommand(() => ShowInfo(Res.InfoMessageEasterEgg)));

        #endregion

        #region Protected Properties

        protected override bool AffectsModifiedState(string propertyName) => false; // set explicitly
        protected virtual bool IsPaletteReadOnly => imageInfo.Type == ImageInfoType.Icon || ReadOnly;
        protected virtual bool IsDebuggerVisualizer => true;

        #endregion

        #endregion

        #region Constructors

        internal ImageVisualizerViewModel(AllowedImageTypes imageTypes = AllowedImageTypes.All)
        {
            this.imageTypes = imageTypes;
        }

        #endregion

        #region Methods

        #region Static Methods

        /// <summary>
        /// ImageFormat.ToString uses == instead of Equals, which returns only a guid in most cases.
        /// </summary>
        private static string RawFormatToString(Guid imageFormat)
        {
            if (imageFormat.Equals(ImageFormat.MemoryBmp.Guid))
                return nameof(ImageFormat.MemoryBmp);
            if (imageFormat.Equals(ImageFormat.Bmp.Guid))
                return nameof(ImageFormat.Bmp);
            if (imageFormat.Equals(ImageFormat.Emf.Guid))
                return nameof(ImageFormat.Emf);
            if (imageFormat.Equals(ImageFormat.Wmf.Guid))
                return nameof(ImageFormat.Wmf);
            if (imageFormat.Equals(ImageFormat.Gif.Guid))
                return nameof(ImageFormat.Gif);
            if (imageFormat.Equals(ImageFormat.Jpeg.Guid))
                return nameof(ImageFormat.Jpeg);
            if (imageFormat.Equals(ImageFormat.Png.Guid))
                return nameof(ImageFormat.Png);
            if (imageFormat.Equals(ImageFormat.Tiff.Guid))
                return nameof(ImageFormat.Tiff);
            if (imageFormat.Equals(ImageFormat.Exif.Guid))
                return nameof(ImageFormat.Exif);
            if (imageFormat.Equals(ImageFormat.Icon.Guid))
                return nameof(ImageFormat.Icon);
            return Res.InfoUnknownFormat(imageFormat);
        }

        private static bool TryLoadCustom(MemoryStream stream, [MaybeNullWhen(false)]out Image image)
        {
            const int bdatHeader = 0x54414442; // "BDAT"
            image = null;

            long pos = stream.Position;
            if (pos > stream.Length - 4)
                return false;

            var reader = new BinaryReader(stream);
            var head = reader.ReadInt32();
            stream.Position = pos;

            if (head != bdatHeader)
                return false;
            using IReadWriteBitmapData bitmapData = BitmapDataFactory.Load(stream);
            image = bitmapData.ToBitmap();
            return true;
        }

        #endregion

        #region Instance Methods

        #region Internal Methods

        internal override void ViewLoaded()
        {
            InitAutoZoom(true);
            if (deferUpdateInfo)
            {
                if (SetCompoundViewCommandState.GetValueOrDefault<bool>(stateVisible))
                    SetCompoundViewCommandStateImage();
                UpdateIfMultiResImage();
            }

            base.ViewLoaded();
        }

        #endregion

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            switch (e.PropertyName)
            {
                case nameof(IsCompoundView):
                    if (imageInfo.HasFrames && imageInfo.Type != ImageInfoType.Pages)
                        ResetCompoundState();
                    return;
                case nameof(ReadOnly):
                    ReadOnlyChanged();
                    return;
                case nameof(AutoZoom):
                    UpdateMultiResImage();
                    return;
            }
        }

        protected virtual void UpdateInfo()
        {
            if (imageInfo.Type == ImageInfoType.None)
            {
                TitleCaption = Res.TitleNoImage;
                InfoText = null;
                return;
            }

            ImageInfoBase currentImage = GetCurrentImage();
            var sb = new StringBuilder();
            sb.Append(Res.TitleType(GetTypeName()));
            if (!imageInfo.IsMetafile)
            {
                sb.Append(Res.TextSeparator);
                sb.Append(Res.TitleSize(GetSize()));
            }
            sb.Append(GetFrameInfo(true));

            TitleCaption = sb.ToString();
            sb.Length = 0;
            sb.Append(Res.InfoImage(GetTypeName(),
                GetSize(),
                currentImage.PixelFormat,
                RawFormatToString(currentImage.RawFormat),
                currentImage.HorizontalRes, currentImage.VerticalRes,
                GetFrameInfo(false)));

            if (!imageInfo.IsMetafile)
            {
                sb.AppendLine();
                sb.Append(Res.InfoPalette(currentImage.Palette.Length));
            }

            InfoText = sb.ToString();
        }

        protected virtual void OpenFile()
        {
            SetOpenFilter();
            string? fileName = SelectFileToOpenCallback?.Invoke();
            if (fileName == null)
                return;

            SetNotification(null);
            OpenFile(fileName);
        }

        protected void SetNotification(string? resourceId)
        {
            notificationId = resourceId;
            UpdateNotification();
        }

        protected virtual bool OpenFile(string path)
        {
            try
            {
                FromFile(path);
                SetModified(IsDebuggerVisualizer);
                return true;
            }
            catch (Exception e) when (!e.IsCritical())
            {
                ShowError(Res.ErrorMessageFailedToLoadFile(e.Message));
                return false;
            }
        }

        protected virtual bool SaveFile(string fileName, string selectedFormat)
        {
            ImageCodecInfo? encoder = encoderCodecs.FirstOrDefault(e => selectedFormat.Equals(e.FilenameExtension, StringComparison.OrdinalIgnoreCase));

            try
            {
                // BMP
                if (encoder?.FormatID == ImageFormat.Bmp.Guid)
                    GetCurrentImage().Image!.SaveAsBmp(fileName);
                // JPEG
                else if (encoder?.FormatID == ImageFormat.Jpeg.Guid)
                    GetCurrentImage().Image!.SaveAsJpeg(fileName, 95);
                // GIF
                else if (encoder?.FormatID == ImageFormat.Gif.Guid)
                    GetCurrentImage().Image!.SaveAsGif(fileName);
                // Tiff
                else if (encoder?.FormatID == ImageFormat.Tiff.Guid)
                {
                    if (imageInfo.HasFrames && IsCompoundView)
                    {
                        using (Stream stream = File.Create(fileName))
                            imageInfo.Frames!.Select(f => f.Image!).SaveAsMultipageTiff(stream);
                    }
                    else
                        GetCurrentImage().Image!.SaveAsTiff(fileName);
                }
                // PNG
                else if (encoder?.FormatID == ImageFormat.Png.Guid)
                    GetCurrentImage().Image!.SaveAsPng(fileName);
                // icon
                else if (selectedFormat == "*.ico")
                    SaveIcon(fileName);
                // windows metafile
                else if (selectedFormat == "*.wmf" && imageInfo.IsMetafile)
                    ((Metafile)imageInfo.Image!).SaveAsWmf(fileName);
                // enhanced metafile
                else if (selectedFormat == "*.emf" && imageInfo.IsMetafile)
                    ((Metafile)imageInfo.Image!).SaveAsEmf(fileName);
                // Some unrecognized encoder - we assume it can handle every pixel format
                else if (encoder != null)
                    GetCurrentImage().Image!.Save(fileName, encoder, null);
                else if (selectedFormat == "*.bdat")
                    SaveBitmapData(fileName);
                else
                    throw new InvalidOperationException(Res.InternalError($"Unexpected format without encoder: {selectedFormat}"));

                return true;
            }
            catch (Exception e) when (!e.IsCriticalGdi())
            {
                ShowError(Res.ErrorMessageFailedToSaveImage(e.Message));
                return false;
            }
        }

        protected virtual void Clear()
        {
            Image = null;
            SetModified(IsDebuggerVisualizer);
        }

        protected override void ApplyDisplayLanguage()
        {
            isOpenFilterUpToDate = false;
            UpdateSmoothZoomingTooltip();
            UpdateNotification();
            UpdateInfo();
            if (imageInfo.HasFrames)
                UpdateCompoundToolTip();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !keepAliveImageInfo)
                imageInfo.Dispose();
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void SetImageInfo(ImageInfo value)
        {
            ValidateImageInfo(value);

            currentResolution = Size.Empty;
            imageInfo.Dispose();
            imageInfo = value;
            SetModified(false);
            PreviewImage = null;
            InitAutoZoom(false);

            if (value.HasFrames)
                InitMultiImage();
            else
                InitSingleImage();
        }

        private void ValidateImageInfo(ImageInfo value)
        {
            // validating the image info itself
            if (!value.IsValid)
            {
                ValidationResult error = value.ValidationResults.Errors[0];

                // ReSharper disable once LocalizableElement - the message comes from resource
                throw new ArgumentException($"{error.PropertyName}: {error.Message}", nameof(value));
            }

            bool valid = value.Type == ImageInfoType.None
                || value.IsMetafile && imageTypes.HasFlag<AllowedImageTypes>(AllowedImageTypes.Metafile)
                || value.Type == ImageInfoType.Icon && imageTypes.HasFlag<AllowedImageTypes>(AllowedImageTypes.Icon)
                || !value.Type.In(ImageInfoType.None, ImageInfoType.Icon) && !value.IsMetafile && imageTypes.HasFlag<AllowedImageTypes>(AllowedImageTypes.Bitmap);

            if (!valid)
                throw new ArgumentException(PublicResources.ArgumentOutOfRange, nameof(value));
        }

        private void InitSingleImage()
        {
            currentFrame = -1;
            SetCompoundViewCommandState[stateVisible] = false;
            IsAutoPlaying = false;
            PreviewImage = imageInfo.GetCreateImage();
            ImageChanged();
        }

        private void InitMultiImage()
        {
            UpdateCompoundToolTip();
            SetCompoundViewCommandStateImage();
            SetCompoundViewCommandState[stateVisible] = true;
            ResetCompoundState();
        }

        private void UpdateCompoundToolTip() => SetCompoundViewCommandState[stateToolTipText] = imageInfo.Type switch
        {
            ImageInfoType.Pages => Res.TooltipTextCompoundMultiPage,
            ImageInfoType.Animation => Res.TooltipTextCompoundAnimation,
            _ => Res.TooltipTextCompoundMultiSize
        };

        private ImageInfoBase GetCurrentImage()
        {
            if (!imageInfo.HasFrames || currentFrame < 0 || IsAutoPlaying)
                return imageInfo;
            return imageInfo.Frames![currentFrame];
        }

        private bool IsSingleImageShown() => imageInfo.Type != ImageInfoType.None && !imageInfo.HasFrames
            || currentFrame >= 0 && !IsAutoPlaying;

        private void SetCompoundViewCommandStateImage()
        {
            Func<ImageInfoType, Image>? callback = GetCompoundViewIconCallback;
            deferUpdateInfo |= callback == null;
            if (callback != null)
                SetCompoundViewCommandState[stateImage] = callback.Invoke(imageInfo.Type);
        }

        private void ImageChanged()
        {
            ImageInfoBase image = GetCurrentImage();
            ShowPaletteCommandState.Enabled = image.Palette.Length > 0;
            SaveFileCommandState.Enabled = imageInfo.Type != ImageInfoType.None;
            ClearCommandState.Enabled = imageInfo.Type != ImageInfoType.None && !ReadOnly;
            EditBitmapCommandState.Enabled = CanEditImage();
            CountColorsCommandState.Enabled = imageInfo.Type != ImageInfoType.None && !imageInfo.IsMetafile && IsSingleImageShown();
            UpdateInfo();
        }

        private void ReadOnlyChanged()
        {
            bool readOnly = ReadOnly;
            OpenFileCommandState.Enabled = !readOnly;
            ClearCommandState.Enabled = !readOnly && imageInfo.Type != ImageInfoType.None;
            EditBitmapCommandState.Enabled = CanEditImage();
        }

        private bool CanEditImage() => imageInfo.Type != ImageInfoType.None && !ReadOnly && !imageInfo.IsMetafile && IsSingleImageShown();

        private string GetFrameInfo(bool singleLine)
        {
            if (!imageInfo.HasFrames)
                return String.Empty;

            var result = new StringBuilder();
            if (singleLine)
                result.Append("; ");

            if (currentFrame != -1 && !IsAutoPlaying)
                result.Append(Res.InfoCurrentFrame(currentFrame + 1, imageInfo.Frames!.Length));
            else
                result.Append(Res.InfoFramesCount(imageInfo.Frames!.Length));

            if (!singleLine)
                result.AppendLine();
            return result.ToString();
        }

        private Size GetSize()
        {
            if (imageInfo.IsMultiRes && imageInfo.HasFrames && currentFrame == -1)
                return currentResolution;
            return GetCurrentImage().Size;
        }

        private string GetTypeName()
        {
            if (imageInfo.Type == ImageInfoType.Icon)
                return nameof(System.Drawing.Icon);
            Image? img = GetCurrentImage().Image;
            return img?.GetType().Name ?? nameof(Bitmap);
        }

        private void FromFile(string fileName)
        {
            var stream = new MemoryStream(File.ReadAllBytes(fileName));
            bool appearsIcon = Path.GetExtension(fileName).Equals(".ico", StringComparison.OrdinalIgnoreCase);

            Icon? icon = null;

            // icon is allowed and the content seems to be an icon
            // (this block is needed only for Windows XP: Icon Bitmap with PNG throws an exception but initializing from icon will succeed)
            if (appearsIcon && (imageTypes & AllowedImageTypes.Icon) == AllowedImageTypes.Icon)
            {
                try
                {
                    icon = Icons.FromStream(stream);
                    Icon = icon;
                    imageInfo.FileName = fileName;
                    return;
                }
                catch (Exception e) when (!e.IsCritical())
                {
                    // failed to open as an icon: fallback to usual paths
                    stream.Position = 0L;
                }
            }

            Image? image = null;
            string? openedFileName = fileName;

            // bitmaps and metafiles are both allowed
            if ((imageTypes & (AllowedImageTypes.Bitmap | AllowedImageTypes.Metafile)) == (AllowedImageTypes.Bitmap | AllowedImageTypes.Metafile))
            {
                try
                {
                    image = LoadImage(stream);
                }
                catch (Exception e) when (!e.IsCriticalGdi())
                {
                    throw new ArgumentException(Res.ErrorMessageNotAnImageFile(e.Message), nameof(fileName), e);
                }
            }
            // metafiles only
            else if (imageTypes == AllowedImageTypes.Metafile)
            {
                try
                {
                    image = new Metafile(stream);
                }
                catch (Exception e) when (!e.IsCriticalGdi())
                {
                    throw new ArgumentException(Res.ErrorMessageNotAMetafile(e.Message), nameof(fileName), e);
                }
            }
            // bitmaps or icons
            else if ((imageTypes & (AllowedImageTypes.Bitmap | AllowedImageTypes.Icon)) != AllowedImageTypes.None)
            {
                try
                {
                    image = LoadImage(stream);
                }
                catch (Exception e) when (!e.IsCriticalGdi())
                {
                    throw new ArgumentException(Res.ErrorMessageNotABitmapFile(e.Message), nameof(fileName), e);
                }

                if (image.RawFormat.Guid == ImageFormat.MemoryBmp.Guid)
                {
                    SetNotification(Res.NotificationMetafileAsBitmapId);
                    openedFileName = null;
                }
            }

            // icon is allowed and an image has been loaded
            if (image != null && (imageTypes & AllowedImageTypes.Icon) != AllowedImageTypes.None)
            {
                // the loaded format is icon: loading as icon
                if (image.RawFormat.Guid == ImageFormat.Icon.Guid)
                {
                    stream.Position = 0L;
                    try
                    {
                        icon = new Icon(stream);
                    }
                    catch (Exception e) when (!e.IsCriticalGdi())
                    {
                        throw new ArgumentException(Res.ErrorMessageNotAnIconFile(e.Message), nameof(fileName), e);
                    }
                }

                // not icon was loaded, though icon is the only supported format: converting to icon
                else if (imageTypes == AllowedImageTypes.Icon)
                {
                    Bitmap iconImage = image as Bitmap ?? new Bitmap(image);
                    icon = iconImage.ToIcon();
                    iconImage.Dispose();
                    SetNotification(Res.NotificationImageAsIconId);
                    openedFileName = null;
                }
            }

            if (icon != null)
            {
                Icon = icon;
                image?.Dispose();
            }
            else
                Image = image;

            // null will be assigned if the image has been converted (see notifications)
            imageInfo.FileName = openedFileName;
        }

        private Image LoadImage(MemoryStream stream)
        {
            if (TryLoadCustom(stream, out Image? image))
                return image;

            // bitmaps and metafiles are both allowed
            if ((imageTypes & (AllowedImageTypes.Bitmap | AllowedImageTypes.Metafile)) == (AllowedImageTypes.Bitmap | AllowedImageTypes.Metafile))
                return Image.FromStream(stream);

            // as Bitmap
            Debug.Assert(imageTypes != AllowedImageTypes.Metafile, "This method is not expected to be called if only metafiles are allowed");
            return new Bitmap(stream);
        }

        private void ResetCompoundState()
        {
            Debug.Assert(imageInfo.HasFrames);
            bool isCompound = IsCompoundView;

            // handle as separated images
            if (!isCompound || imageInfo.Type == ImageInfoType.Pages)
            {
                currentFrame = 0;
                IsAutoPlaying = false;
                NextImageCommandState.Enabled = true;
                PrevImageCommandState.Enabled = false;
                PreviewImage = imageInfo.Frames![0].Image;
                ImageChanged();
                return;
            }

            // handle as compound image
            NextImageCommandState.Enabled = PrevImageCommandState.Enabled = false;
            bool autoPlaying = imageInfo.Type == ImageInfoType.Animation;
            ICommandState timerState = AdvanceAnimationCommandState;
            IsAutoPlaying = autoPlaying;
            PreviewImage = imageInfo.Frames![0].Image;
            if (autoPlaying)
            {
                currentFrame = 0;
                timerState[stateInterval] = imageInfo.Frames[0].Duration;
            }
            else
            {
                currentFrame = -1;
                UpdateMultiResImage();
            }

            timerState.Enabled = autoPlaying;
            ImageChanged();
        }

        private void UpdateMultiResImage()
        {
            if (!imageInfo.IsMultiRes || currentFrame != -1)
                return;
            Size origSize = currentResolution;
            Func<Size>? callback = GetImagePreviewSizeCallback;
            deferUpdateInfo |= callback == null;
            if (callback == null)
                return;
            Size clientSize = callback.Invoke();
            int desiredSize = Math.Min(clientSize.Width, clientSize.Height);
            if (desiredSize < 1 && !origSize.IsEmpty)
                return;

            // Starting with Windows Vista it would work that we draw the compound image in a new Bitmap with desired size and read the Size afterwards
            // but that requires always a new bitmap and does not work in Windows XP
            desiredSize = Math.Max(desiredSize, 1);
            float zoom = AutoZoom ? 1f : Zoom;
            ImageFrameInfo desiredImage = imageInfo.Frames!.Aggregate((acc, i)
                => i.Size == acc.Size && i.BitsPerPixel > acc.BitsPerPixel
                || Math.Abs(i.Size.Width * zoom - desiredSize) < Math.Abs(acc.Size.Width * zoom - desiredSize) ? i : acc);
            currentResolution = desiredImage.Size;
            if (PreviewImage != desiredImage.Image)
                PreviewImage = desiredImage.Image;
        }

        private void UpdateIfMultiResImage()
        {
            if (!imageInfo.IsMultiRes || currentFrame != -1)
                return;
            UpdateMultiResImage();
            UpdateInfo();
        }

        private void SaveIcon(string fileName)
        {
            using (Stream stream = File.Create(fileName))
                SaveIcon(stream);
        }

        private void SaveIcon(Stream stream)
        {
            // saving as composite icon
            if (IsCompoundView)
            {
                // when used as debugger, icon is always created from stream so it has raw data and Save can be used safely.
                // But when icon is set via Icon property it can be an unmanaged icon
                if (imageInfo.Icon != null && !IsModified)
                    imageInfo.Icon.SaveAsIcon(stream);
                // multi image icon without raw data
                else if (imageInfo.HasFrames)
                {
                    using (Icon i = Icons.Combine(imageInfo.Frames!.Select(f => (Bitmap)f.Image!).ToArray()))
                        i.Save(stream);
                }
                // single image icon without raw data
                else
                {
                    using (Icon i = Icons.Combine((Bitmap)imageInfo.Image!))
                        i.Save(stream);
                }

                stream.Flush();
                return;
            }

            // saving a single icon image
            if (imageInfo.Icon != null)
            {
                using (Icon i = imageInfo.Icon.ExtractIcon(currentFrame)!)
                    i.Save(stream);
            }
            else
            {
                using (Icon i = Icons.Combine((Bitmap)GetCurrentImage().Image!))
                    i.Save(stream);
            }

            stream.Flush();
        }

        private void SaveBitmapData(string fileName)
        {
            using Stream stream = File.Create(fileName);
            Image image = IsCompoundView ? imageInfo.GetCreateImage()! : GetCurrentImage().Image!;
            Bitmap bmp = image as Bitmap ?? new Bitmap(image);
            try
            {
                using IReadableBitmapData bitmapData = bmp.GetReadableBitmapData();
                bitmapData.Save(stream);
            }
            finally
            {
                if (!ReferenceEquals(image, bmp))
                    bmp.Dispose();
            }
        }

        private void SetOpenFilter()
        {
            if (isOpenFilterUpToDate || imageTypes == AllowedImageTypes.None)
                return;

            StringBuilder sb = new StringBuilder();
            StringBuilder sbImages = new StringBuilder();
            foreach (ImageCodecInfo codecInfo in decoderCodecs)
            {
                if (imageTypes == AllowedImageTypes.Metafile && codecInfo.FormatID != ImageFormat.Wmf.Guid && codecInfo.FormatID != ImageFormat.Emf.Guid)
                    continue;

                if (sb.Length != 0)
                    sb.Append('|');
                sb.Append($"{codecInfo.FormatDescription} {Res.TextFiles}|{codecInfo.FilenameExtension?.ToLowerInvariant()}");
                if (sbImages.Length != 0)
                    sbImages.Append(';');
                sbImages.Append(codecInfo.FilenameExtension?.ToLowerInvariant());
            }

            if ((imageTypes & AllowedImageTypes.Bitmap) != AllowedImageTypes.None)
            {
                sb.Append($"|{Res.TextRaw} {Res.TextFileFormat}|*.bdat");
                sbImages.Append(";*.bdat");
            }

            OpenFileFilter = $"{(imageTypes == AllowedImageTypes.Metafile ? Res.TextMetafiles : Res.TextImages)} ({sbImages})|{sbImages}|{sb}|{Res.TextAllFiles} (*.*)|*.*";
            isOpenFilterUpToDate = true;
        }

        private void SetSaveFilter()
        {
            #region Local Methods
            
            static string GetFirstExtension(string extensions)
            {
                int sep = extensions.IndexOf(';');
                if (sep > 0)
                    extensions = extensions.Substring(0, sep);
                return extensions.Substring(extensions.IndexOf('.') + 1).ToLowerInvariant();
            }

            #endregion

            // enlisting encoders
            var sb = new StringBuilder();
            foreach (ImageCodecInfo codecInfo in encoderCodecs)
            {
                if (sb.Length != 0)
                    sb.Append('|');
                sb.Append($"{codecInfo.FormatDescription} {Res.TextFileFormat}|{codecInfo.FilenameExtension?.ToLowerInvariant()}");
            }

            bool isEmf = false;
            sb.Append($"|{Res.TextIcon} {Res.TextFileFormat}|*.ico");
            if (imageInfo.IsMetafile)
            {
                sb.Append($"|WMF {Res.TextFileFormat}|*.wmf");
                isEmf = imageInfo.RawFormat == ImageFormat.Emf.Guid;
                if (isEmf)
                    sb.Append($"|EMF {Res.TextFileFormat}|*.emf");
            }

            sb.Append($"|{Res.TextRaw} {Res.TextFileFormat}|*.bdat");
            string filter = sb.ToString();

            // selecting appropriate format
            string? ext = null;
            if (imageInfo.IsMultiRes)
                ext = "ico";
            else if (imageInfo.IsMetafile)
                ext = isEmf ? "emf" : "wmf";
            else
            {
                // looking for a matching built-in encoder
                bool isPngSupported = false;
                bool found = false;
                foreach (ImageCodecInfo encoder in encoderCodecs)
                {
                    if (encoder.FormatID == imageInfo.RawFormat)
                    {
                        ext = GetFirstExtension(encoder.FilenameExtension ?? String.Empty);
                        found = true;
                        break;
                    }

                    if (!isPngSupported && encoder.FormatID == ImageFormat.Png.Guid)
                        isPngSupported = true;
                }

                // no matching encoder found: using either PNG, the first one in the list or icon
                if (!found)
                {
                    ext = isPngSupported ? "png"
                        : encoderCodecs.Length > 0 ? GetFirstExtension(encoderCodecs[0].FilenameExtension ?? String.Empty)
                        : "ico";
                }
            }

            SaveFileFilter = filter;
            SaveFileDefaultExtension = ext;
            SaveFileFilterIndex = (filter.Split('|').IndexOf(item => item.Contains("*." + ext, StringComparison.OrdinalIgnoreCase)) >> 1) + 1;
        }

        private void InitAutoZoom(bool viewLoading)
        {
            UpdateSmoothZoomingTooltip();
            if (imageInfo.Type == ImageInfoType.None)
            {
                SetAutoZoomCommandState.Enabled = AutoZoom = false;
                return;
            }

            SetAutoZoomCommandState.Enabled = SetSmoothZoomingCommandState.Enabled = true;

            // metafile: we always turn on auto zoom and preserve current smooth zooming
            if (imageInfo.IsMetafile)
            {
                AutoZoom = true;
                return;
            }

            // if we are just opening a new image we don't auto toggle AutoZoom and SmoothZooming anymore
            if (!viewLoading)
            {
                if (!AutoZoom)
                    Zoom = 1f;
                return;
            }

            Rectangle workingArea = GetScreenRectangleCallback?.Invoke() ?? default;
            if (workingArea.IsEmpty)
                return;

            Size screenSize = workingArea.Size;
            Size viewSize = GetViewSizeCallback?.Invoke() ?? default;
            Size padding = viewSize - GetImagePreviewSizeCallback?.Invoke() ?? default;
            Size desiredSize = imageInfo.Size + padding;

            if (desiredSize.Width <= screenSize.Width && desiredSize.Height <= screenSize.Height)
            {
                // for icons turning on auto zoom so shrinking the view will not cause twitching as the scrollbar appears and disappears
                AutoZoom = imageInfo.IsMultiRes;
                ApplyViewSizeCallback?.Invoke(new Size(Math.Max(desiredSize.Width, viewSize.Width), Math.Max(desiredSize.Height, viewSize.Height)));
                Zoom = 1f;
            }
            else
            {
                // image is too large to fit
                AutoZoom = SmoothZooming = true;
            }
        }

        private void InvalidateImage()
        {
            SetModified(true);
            imageInfo.FileName = null;
            if (imageInfo.HasFrames)
            {
                imageInfo.Image = null;
                imageInfo.Icon = null;
            }

            UpdatePreviewImageCallback?.Invoke();
        }

        private bool CheckSaveExtension(string fileName)
        {
            string actualExt = Path.GetExtension(fileName).ToUpperInvariant();
            string[] filters = SaveFileFilter!.Split('|');
            int filterIndex = SaveFileFilterIndex;
            string suggestedExt = filters[((filterIndex - 1) << 1) + 1].ToUpperInvariant();
            if (suggestedExt.Split(';').Contains('*' + actualExt))
                return true;
            return Confirm(Res.ConfirmMessageSaveFileExtension(Path.GetFileName(fileName), filters[(filterIndex - 1) << 1]), false);
        }

        private void SetCurrentImage(Bitmap? image)
        {
            // replacing the whole image (non-compound one)
            if (GetCurrentImage() == imageInfo)
            {
                Debug.Assert(!imageInfo.HasFrames);
                if (!ReferenceEquals(imageInfo.Image, image))
                    imageInfo.Dispose();
                imageInfo = new ImageInfo(image);
                PreviewImage = imageInfo.GetCreateImage();
            }
            // replacing the current frame only
            else
            {
                Debug.Assert(currentFrame >= 0 && !IsAutoPlaying);
                ImageFrameInfo[] frames = imageInfo.Frames!;
                ImageFrameInfo origFrame = frames[currentFrame];
                frames[currentFrame] = new ImageFrameInfo(image) { Duration = origFrame.Duration };
                if (!ReferenceEquals(origFrame.Image, image))
                    origFrame.Dispose();
                PreviewImage = frames[currentFrame].Image;
            }

            InvalidateImage();
            ImageChanged();
        }

        private void EditBitmap(Func<Bitmap, IViewModel<Bitmap?>> createViewModel)
        {
            Debug.Assert(imageInfo.Type != ImageInfoType.None && !imageInfo.IsMetafile, "Non-metafile image is expected");

            ImageInfoBase image = GetCurrentImage();

            Debug.Assert(image.Image is Bitmap, "Existing bitmap image is expected");
            using (IViewModel<Bitmap?> viewModel = createViewModel.Invoke((Bitmap)image.Image!))
            {
                ShowChildViewCallback?.Invoke(viewModel);
                if (viewModel.IsModified)
                    SetCurrentImage(viewModel.GetEditedModel());
            }
        }

        private void RotateBitmap(RotateFlipType direction)
        {
            Debug.Assert(imageInfo.Type != ImageInfoType.None && !imageInfo.IsMetafile, "Non-metafile image is expected");
            ImageInfoBase image = GetCurrentImage();
            Debug.Assert(image.Image is Bitmap, "Existing bitmap image is expected");

            // must be in a lock because it can be in use in the UI (where it is also locked)
            lock (image.Image!)
                image.Image.RotateFlip(direction);
            SetCurrentImage((Bitmap)image.Image);
        }

        private void UpdateSmoothZoomingTooltip()
            => SetSmoothZoomingCommandState[stateToolTipText] =
                imageInfo.Type == ImageInfoType.None ? null
                : imageInfo.IsMetafile ? Res.TooltipTextSmoothMetafile
                : Res.TooltipTextSmoothBitmap;

        private void UpdateNotification() => Notification = notificationId == null ? null : Res.Get(notificationId);

        #endregion

        #region Explicitly Implemented Interface Methods

        Image? IViewModel<Image?>.GetEditedModel() => Image?.Clone() as Image;
        Icon? IViewModel<Icon?>.GetEditedModel() => Icon?.Clone() as Icon;
        Bitmap? IViewModel<Bitmap?>.GetEditedModel() => Image?.Clone() as Bitmap;
        Metafile? IViewModel<Metafile?>.GetEditedModel() => Image?.Clone() as Metafile;
        ImageInfo IViewModel<ImageInfo>.GetEditedModel()
        {
            keepAliveImageInfo = true;
            return imageTypes == AllowedImageTypes.Icon ? imageInfo.AsIcon() : imageInfo.AsImage();
        }

        #endregion

        #region Command Handlers

        private void OnSetAutoZoomCommand(bool newValue) => AutoZoom = newValue;

        private void OnSetSmoothZoomingCommand(bool newValue) => SmoothZooming = newValue;

        private void OnViewImagePreviewSizeChangedCommand() => UpdateIfMultiResImage();

        private void OnOpenFileCommand() => OpenFile();

        private void OnSaveFileCommand()
        {
            if (imageInfo.Type == ImageInfoType.None)
                return;

            SetSaveFilter();
            string? fileName;
            do
            {
                fileName = SelectFileToSaveCallback?.Invoke();
                if (fileName == null)
                    return;
            } while (!CheckSaveExtension(fileName));

            int filterIndex = SaveFileFilterIndex;
            string selectedFormat = SaveFileFilter!.Split('|')[((filterIndex - 1) << 1) + 1];

            SaveFile(fileName, selectedFormat);
        }

        private void OnClearCommand() => Clear();

        private void OnSetCompoundViewCommand(bool isCompound) => IsCompoundView = isCompound;

        private void OnAdvanceAnimationCommand()
        {
            if (!IsAutoPlaying)
            {
                AdvanceAnimationCommandState.Enabled = false;
                return;
            }

            // playing with duration
            Debug.Assert(imageInfo.HasFrames);
            currentFrame++;
            ImageFrameInfo[] frames = imageInfo.Frames!;
            if (currentFrame >= frames.Length)
                currentFrame = 0;
            AdvanceAnimationCommandState[stateInterval] = frames[currentFrame].Duration;
            PreviewImage = frames[currentFrame].Image;
        }

        private void OnPrevImageCommand()
        {
            if (!imageInfo.HasFrames || currentFrame <= 0)
                return;

            PreviewImage = imageInfo.Frames![--currentFrame].Image;
            PrevImageCommandState.Enabled = currentFrame > 0;
            NextImageCommandState.Enabled = true;
            ImageChanged();
        }

        private void OnNextImageCommand()
        {
            ImageFrameInfo[] frames = imageInfo.Frames!;
            if (!imageInfo.HasFrames || currentFrame >= frames.Length)
                return;

            PreviewImage = frames[++currentFrame].Image;
            PrevImageCommandState.Enabled = true;
            NextImageCommandState.Enabled = currentFrame < frames.Length - 1;
            ImageChanged();
        }

        private void OnShowPaletteCommand()
        {
            ImageInfoBase currentImage = GetCurrentImage();
            if (currentImage.Palette.Length == 0)
                return;

            using (IViewModel<Color[]> vmPalette = ViewModelFactory.FromPalette(currentImage.Palette, IsPaletteReadOnly))
            {
                ShowChildViewCallback?.Invoke(vmPalette);
                if (!vmPalette.IsModified)
                    return;

                // apply changes
                ColorPalette palette = currentImage.Image!.Palette;
                Color[] newPalette = vmPalette.GetEditedModel();

                // even if the length of the palette is not edited it can happen that the preview image is ARGB32
                if (palette.Entries.Length != newPalette.Length)
                {
                    // using the original palette for the conversion before applying the new colors
                    Image newImage = currentImage.Image.ConvertPixelFormat(currentImage.PixelFormat, currentImage.Palette);
                    currentImage.Image.Dispose();
                    PreviewImage = currentImage.Image = newImage;
                    palette = newImage.Palette;
                }

                for (int i = 0; i < newPalette.Length; i++)
                    palette.Entries[i] = newPalette[i];

                // must be in a lock because it can be in use in the UI (where it is also locked)
                lock (currentImage.Image)
                    currentImage.Image.Palette = palette; // the preview changes only if we apply the palette
                currentImage.Palette = palette.Entries; // the actual palette will be taken from here
                InvalidateImage();
            }
        }

        private void OnManageInstallationsCommand()
        {
            using (IViewModel viewModel = ViewModelFactory.CreateManageInstallations(Files.GetExecutingPath()))
                ShowChildViewCallback?.Invoke(viewModel);
        }

        private void OnCountColorsCommand()
        {
            Debug.Assert(imageInfo.Type != ImageInfoType.None && !imageInfo.IsMetafile, "Non-metafile image is expected");

            ImageInfoBase image = GetCurrentImage();

            Debug.Assert(image.Image is Bitmap, "Existing bitmap image is expected");
            using IViewModel<int?> viewModel = ViewModelFactory.CreateCountColors((Bitmap)image.Image!);
            ShowChildViewCallback?.Invoke(viewModel);

            // this prevents the viewModel from disposing until before the view is completely finished (on cancel, for example)
            var _ = viewModel.GetEditedModel();
        }

        private void OnRotateLeftCommand() => RotateBitmap(RotateFlipType.Rotate270FlipNone);
        private void OnRotateRightCommand() => RotateBitmap(RotateFlipType.Rotate90FlipNone);
        private void OnResizeBitmapCommand() => EditBitmap(ViewModelFactory.CreateResizeBitmap);
        private void OnAdjustColorSpaceCommand() => EditBitmap(ViewModelFactory.CreateAdjustColorSpace);
        private void OnAdjustBrightnessCommand() => EditBitmap(ViewModelFactory.CreateAdjustBrightness);
        private void OnAdjustContrastCommand() => EditBitmap(ViewModelFactory.CreateAdjustContrast);
        private void OnAdjustGammaCommand() => EditBitmap(ViewModelFactory.CreateAdjustGamma);

        private void OnSetLanguageCommand()
        {
            using IViewModel viewModel = ViewModelFactory.CreateLanguageSettings();
            ShowChildViewCallback?.Invoke(viewModel);
        }

        private void OnShowAboutCommand()
        {
            Assembly asm = GetType().Assembly;

#if NET35
            const string frameworkName = ".NET Framework 3.5"; 
#else
            TargetFrameworkAttribute attr = (TargetFrameworkAttribute)Attribute.GetCustomAttribute(asm, typeof(TargetFrameworkAttribute))!;
            string frameworkName = attr.FrameworkDisplayName is { Length: > 0 } name ? name : attr.FrameworkName;
#endif
            ShowInfo(Res.InfoMessageAbout(asm.GetName().Version!, frameworkName, DateTime.Now.Year));
        }

        #endregion

        #endregion

        #endregion
    }
}
