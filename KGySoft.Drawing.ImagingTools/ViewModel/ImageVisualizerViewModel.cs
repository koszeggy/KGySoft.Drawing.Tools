#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageVisualizerViewModel.cs
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

using System.Diagnostics.CodeAnalysis;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
#if !NET35
using System.Runtime.Versioning; 
#endif
using System.Text;
using System.Threading;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class ImageVisualizerViewModel : ViewModelBase<ImageInfo>, IViewModel<Image?>, IViewModel<Icon?>, IViewModel<Bitmap?>, IViewModel<Metafile?>
    {
        #region Nested Classes
        
        #region OpenTask class

        private sealed class OpenTask : AsyncTaskContext
        {
            #region Fields

            internal string FileName = null!;

            #endregion
        }

        #endregion

        #region SaveTask class

        private sealed class SaveTask : AsyncTaskContext
        {
            #region Fields

            internal string FileName = null!;
            internal string SelectedFormat = null!;
            internal ImageInfoBase ToSave = null!;
            internal int CurrentFrame; // needed when a compound image is saved in a single frame format

            #endregion
        }

        #endregion

        #region CopyTask class

        private sealed class CopyTask : AsyncTaskContext
        {
            #region Fields

            internal ImageInfoBase ToCopy = null!;

            #endregion
        }

        #endregion

        #region PasteTask class

        private sealed class PasteTask : AsyncTaskContext
        {
            #region Fields

            internal AllowedImageTypes AllowedTypes;
            internal bool AllowMultiFrame;
            internal bool PrevEnabled;
            internal bool NextEnabled;

            #endregion
        }

        #endregion

        #endregion

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

        /// <summary>
        /// Indicates that the current <see cref="imageInfo"/> instance has been returned by <see cref="GetEditedModel"/> so it should be kept alive.
        /// If <see langword="true"/>, then the <see cref="imageInfo"/> will not be disposed when a new image is set or when the view model is closed.
        /// </summary>
        private bool keepAliveImageInfo;

        private bool initialized;
        private bool isOpenFilterUpToDate;
        private bool deferUpdateInfo;
        private ImageInfo imageInfo = new ImageInfo(ImageInfoType.None);
        private int currentFrame = -1;
        private Size currentResolution;
        private string? notificationId;
        private volatile AsyncTaskContext? activeTask;

        #endregion

        #endregion

        #region Properties

        #region Internal Properties

        internal Image? Image
        {
            get => ImageInfo.GetCreateImage();
            set => SetImageInfo(new ImageInfo(value), true);
        }

        internal Icon? Icon
        {
            get => ImageInfo.Icon;
            set => SetImageInfo(new ImageInfo(value), true);
        }

        [AllowNull]
        internal ImageInfo ImageInfo
        {
            get
            {
                Debug.Assert(activeTask?.IsCompleted != false, "An active task is not expected here. Make sure it is completed by the time this property is accessed on the UI thread.");
                return imageInfo;
            }

            set => SetImageInfo(value ?? new ImageInfo(ImageInfoType.None), true);
        }

        internal Image? PreviewImage { get => Get<Image?>(); set => Set(value); }
        internal string? TitleCaption { get => Get<string?>(); set => Set(value); }
        internal string? InfoText { get => Get<string?>(); set => Set(value); }
        internal string? Notification { get => Get<string?>(); private set => Set(value); }
        internal bool AutoZoom { get => Get<bool>(); set => Set(value); }
        internal float Zoom { get => Get(1f); set => Set(value); }
        internal bool SmoothZooming { get => Get<bool>(); set => Set(value); }
        internal bool IsCompoundView { get => Get<bool>(); set => Set(value); }
        internal bool IsAutoPlaying { get => Get<bool>(); set => Set(value); }
        internal string? OpenFileFilter { get => Get<string?>(); set => Set(value); }
        internal string? SaveFileFilter { get => Get<string?>(); set => Set(value); }
        internal int SaveFileFilterIndex { get => Get<int>(); set => Set(value); }
        internal string? SaveFileDefaultExtension { get => Get<string?>(); set => Set(value); }
        internal bool IsAsyncTaskRunning => activeTask != null;

        internal Func<Rectangle>? GetScreenRectangleCallback { get => Get<Func<Rectangle>?>(); set => Set(value); }
        internal Func<Size>? GetViewSizeCallback { get => Get<Func<Size>?>(); set => Set(value); }
        internal Func<Size>? GetImagePreviewSizeCallback { get => Get<Func<Size>?>(); set => Set(value); }
        internal Func<Size, bool>? ApplyViewSizeCallback { get => Get<Func<Size, bool>?>(); set => Set(value); }
        internal Func<string?>? SelectFileToOpenCallback { get => Get<Func<string?>?>(); set => Set(value); }
        internal Func<string?>? SelectFileToSaveCallback { get => Get<Func<string?>?>(); set => Set(value); }
        internal Action? UpdatePreviewImageCallback { get => Get<Action?>(); set => Set(value); }
        internal Func<ImageInfoType, Image>? GetCompoundViewIconCallback { get => Get<Func<ImageInfoType, Image>?>(); set => Set(value); }

        internal ICommandState SetAutoZoomCommandState => Get(() => new CommandState());
        internal ICommandState SetSmoothZoomingCommandState => Get(() => new CommandState());
        internal ICommandState OpenFileCommandState => Get(() => new CommandState());
        internal ICommandState SaveFileCommandState => Get(() => new CommandState { Enabled = false });
        internal ICommandState ClearCommandState => Get(() => new CommandState { Enabled = false });
        internal ICommandState CopyCommandState => Get(() => new CommandState { Enabled = false });
        internal ICommandState PasteCommandState => Get(() => new CommandState { Enabled = false });
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
        internal ICommand CopyCommand => Get(() => new SimpleCommand(OnCopyCommand));
        internal ICommand PasteCommand => Get(() => new SimpleCommand(OnPasteCommand));
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
        internal ICommand VisitWebSiteCommand => Get(() => new SimpleCommand(() => PathHelper.OpenUrl("https://kgysoft.net")));
        internal ICommand VisitGitHubCommand => Get(() => new SimpleCommand(() => PathHelper.OpenUrl("https://github.com/koszeggy/KGySoft.Drawing.Tools")));
        internal ICommand VisitMarketplaceCommand => Get(() => new SimpleCommand(() => PathHelper.OpenUrl("https://marketplace.visualstudio.com/items?itemName=KGySoft.drawing-debugger-visualizers")));
        internal ICommand SubmitResourcesCommand => Get(() => new SimpleCommand(() => PathHelper.OpenUrl("https://github.com/koszeggy/KGySoft.Drawing.Tools/issues/new?assignees=&labels=&template=submit-resources.md&title=%5BRes%5D")));
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

        #region Public Methods

        public override ImageInfo GetEditedModel()
        {
            keepAliveImageInfo = true;
            return imageTypes == AllowedImageTypes.Icon ? ImageInfo.AsIcon() : ImageInfo.AsImage();
        }

        public override bool TrySetModel(ImageInfo model) => TryInvokeSync(() => SetImageInfo(model, false));

        #endregion

        #region Internal Methods

        internal override void ViewLoaded()
        {
            if (deferUpdateInfo)
            {
                if (SetCompoundViewCommandState.GetValueOrDefault<bool>(stateVisible))
                    SetCompoundViewCommandStateImage();
                UpdateIfMultiResImage();
            }

            if (!ReadOnly)
                ClipboardHelper.ClipboardChanged += ClipboardHelper_ClipboardChanged;
            if (imageInfo.Type == ImageInfoType.None)
                ResetEnabledStates();

            base.ViewLoaded();
        }

        internal override void ViewShown()
        {
            // Not in ViewLoaded, because we may adjust the view size, which requires it to be fully initialized, especially with custom DPI settings.
            InitDefaults();
            initialized = true;
        }

        internal override void ViewUnloading()
        {
            if (!ReadOnly)
                ClipboardHelper.ClipboardChanged -= ClipboardHelper_ClipboardChanged;
            Configuration.SaveSettings();
        }

        internal void CancelPendingTask() => activeTask?.Cancel();

        #endregion

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            #region Local Methods

            void PersistCompoundView()
            {
                Configuration.CompoundView = e.NewValue is true;
                PersistAutoZoom(AutoZoom);
                PersistSmoothZooming(SmoothZooming);

            }

            void PersistAutoZoom(bool value)
            {
                if (imageInfo.Type == ImageInfoType.None || imageInfo.IsMetafile)
                    return;
                if (imageInfo.IsMultiRes && IsCompoundView)
                    Configuration.AutoZoomMultiResIcon = value;
                else if (value)
                {
                    Configuration.AutoZoomBitmap = true;
                    Configuration.AutoShrinkLargeBitmap = true;
                }
                else
                {
                    Configuration.AutoZoomBitmap = false;

                    // turning off auto zoom: separating the preference for small and large bitmaps
                    Size size = GetSize(); // it's never a compound icon here
                    Size imageViewerSize = GetImagePreviewSizeCallback?.Invoke() ?? size;
                    if (size.Width > imageViewerSize.Width || size.Height > imageViewerSize.Height)
                        Configuration.AutoShrinkLargeBitmap = false;
                }
            }

            void PersistSmoothZooming(bool value)
            {
                if (imageInfo.Type == ImageInfoType.None)
                    Configuration.SmoothZoomingDefault = value;
                else if (imageInfo.IsMetafile)
                    Configuration.SmoothZoomingMetafile = value;
                else if (imageInfo.IsMultiRes && IsCompoundView)
                    Configuration.SmoothZoomingMultiResIcon = value;
                else
                    Configuration.SmoothZoomingBitmap = value;
            }

            #endregion

            base.OnPropertyChanged(e);
            switch (e.PropertyName)
            {
                case nameof(IsCompoundView):
                    if (imageInfo.HasFrames && imageInfo.Type != ImageInfoType.Pages)
                        ResetCompoundState();
                    if (initialized)
                        PersistCompoundView();
                    return;
                
                case nameof(ReadOnly):
                    if (IsViewLoaded)
                        ResetEnabledStates();
                    return;
                
                case nameof(AutoZoom):
                    UpdateMultiResImage();
                    if (initialized && imageInfo.Type != ImageInfoType.None && !imageInfo.IsMetafile)
                        PersistAutoZoom(e.NewValue is true);
                    return;

                case nameof(SmoothZooming) when initialized:
                    PersistSmoothZooming(e.NewValue is true);
                    return;

                case nameof(IsBusy):
                    ResetEnabledStates();
                    return;
            }
        }

        protected void SetImageInfo(ImageInfo value, bool resetPreview)
        {
            ValidateImageInfo(value);

            Debug.Assert(activeTask?.IsCompleted != false, "An active task is not expected here. Make sure it is completed by the time this method is called on the UI thread.");
            currentResolution = Size.Empty;
            if (!keepAliveImageInfo)
                imageInfo.Dispose();
            imageInfo = value;
            keepAliveImageInfo = false;
            SetModified(false);
            if (resetPreview)
                PreviewImage = null;

            UpdateSmoothZoomingTooltip();
            SetAutoZoomCommandState.Enabled = SetSmoothZoomingCommandState.Enabled = imageInfo.Type != ImageInfoType.None;
            if (IsViewLoaded)
                AdjustZoom(resetPreview);
            else
                IsCompoundView = Configuration.CompoundView;

            if (value.HasFrames)
                InitMultiImage();
            else
                InitSingleImage();
        }

        protected virtual void UpdateInfo()
        {
            if (imageInfo.Type == ImageInfoType.None)
            {
                TitleCaption = Res.TitleNoImage;
                InfoText = null;
                return;
            }

            ImageInfoBase currentImage = GetCurrentImageInfo();
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
                currentImage.PixelFormat == PixelFormatExtensions.Format32bppCmyk ? nameof(PixelFormatExtensions.Format32bppCmyk) : currentImage.PixelFormat.ToString<PixelFormat>(),
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

        protected virtual bool OnFileOpening() => true;

        protected void OpenFile(string fileName)
        {
            IsBusy = true;
            SetNotification(null);
            var task = new OpenTask { FileName = fileName };
            activeTask = task;
            ThreadPool.QueueUserWorkItem(DoOpenFile, task);
        }

        protected virtual void OnFileOpened(string fileName)
        {
        }

        protected virtual void OnFileSaved(string fileName, string selectedFormat)
        {
        }

        protected void SetNotification(string? resourceId)
        {
            notificationId = resourceId;
            UpdateNotification();
        }

        protected virtual void Clear()
        {
            Image = null;
            SetModified(IsDebuggerVisualizer);
        }

        protected virtual bool IsPaletteAvailable() => GetCurrentImageInfo().Palette.Length > 0;

        protected virtual void ShowPalette()
        {
            ImageInfoBase currentImage = GetCurrentImageInfo();
            if (currentImage.Palette.Length == 0)
                return;

            using IViewModel<Color[]> vmPalette = ViewModelFactory.FromPalette(currentImage.Palette, IsPaletteReadOnly);
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
            if (IsDisposed)
                return;
            
            if (disposing)
            {
                CancelPendingTask();
                if (!keepAliveImageInfo)
                    imageInfo.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void ValidateImageInfo(ImageInfo value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), PublicResources.ArgumentNull);

            // validating the image info itself
            if (!value.IsValid)
            {
                ValidationResult error = value.ValidationResults.Errors[0];
                throw new ArgumentException(PublicResources.PropertyMessage(error.PropertyName, error.Message), nameof(value));
            }

            bool valid = value.Type == ImageInfoType.None
                || value.IsMetafile && imageTypes.HasFlag<AllowedImageTypes>(AllowedImageTypes.Metafile)
                || value.Type == ImageInfoType.Icon && imageTypes.HasFlag<AllowedImageTypes>(AllowedImageTypes.Icon)
                || value.Type is not (ImageInfoType.None or ImageInfoType.Icon) && !value.IsMetafile && imageTypes.HasFlag<AllowedImageTypes>(AllowedImageTypes.Bitmap);

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
            ImageInfoType.Pages => Res.ToolTipTextCompoundMultiPage,
            ImageInfoType.Animation => Res.ToolTipTextCompoundAnimation,
            _ => Res.ToolTipTextCompoundMultiSize
        };

        private ImageInfoBase GetCurrentImageInfo(bool preferCompoundPages = false)
        {
            if (!imageInfo.HasFrames || currentFrame < 0 || IsAutoPlaying)
                return imageInfo;

            // For pages, currentFrame is never < 0, so it depends on the use case whether we need an actual frame (display) or the whole image (save/copy)
            if (preferCompoundPages && imageInfo.Type == ImageInfoType.Pages && IsCompoundView)
                return imageInfo;

            return imageInfo.Frames![currentFrame];
        }

        private void SetCompoundViewCommandStateImage()
        {
            Func<ImageInfoType, Image>? callback = GetCompoundViewIconCallback;
            deferUpdateInfo |= callback == null;
            if (callback != null)
                SetCompoundViewCommandState[stateImage] = callback.Invoke(imageInfo.Type);
        }

        private void ResetEnabledStates()
        {
            bool isBusy = IsBusy;
            bool isReadOnly = ReadOnly;
            bool isLoaded = imageInfo.Type != ImageInfoType.None;
            bool isSingleImageShown = isLoaded && !imageInfo.HasFrames || currentFrame >= 0 && !IsAutoPlaying;
            OpenFileCommandState.Enabled = !isReadOnly && !isBusy;
            SaveFileCommandState.Enabled = isLoaded && !isBusy;
            ClearCommandState.Enabled = isLoaded && !isReadOnly && !isBusy;
            CopyCommandState.Enabled = isLoaded && !isBusy;
            PasteCommandState.Enabled = !isReadOnly && !isBusy && ClipboardHelper.HasFormat(imageTypes);
            ShowPaletteCommandState.Enabled = !isBusy && IsPaletteAvailable();
            EditBitmapCommandState.Enabled = isLoaded && !isReadOnly && !isBusy && !imageInfo.IsMetafile && isSingleImageShown;
            CountColorsCommandState.Enabled = isLoaded && !isBusy && !imageInfo.IsMetafile && isSingleImageShown;
        }

        private void ImageChanged()
        {
            ResetEnabledStates();
            UpdateInfo();
        }

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
            return GetCurrentImageInfo().Size;
        }

        private string GetTypeName()
        {
            if (imageInfo.Type == ImageInfoType.Icon)
                return nameof(System.Drawing.Icon);
            Image? img = GetCurrentImageInfo().Image;
            return img?.GetType().Name ?? nameof(Bitmap);
        }

        private void OpenFile()
        {
            Debug.Assert(!IsBusy && activeTask == null);
            if (!OnFileOpening())
                return;
            SetOpenFilter();
            string? fileName = SelectFileToOpenCallback?.Invoke();
            if (fileName != null)
                OpenFile(fileName);
        }

        [SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly",
            Justification = "False alarm, argument name 'fileName' is the actual parameter name of the caller method.")]
        private void DoOpenFile(object? state)
        {
            var task = (OpenTask)state!;
            string fileName = task.FileName;
            Exception? error = null;
            try
            {
                var stream = new MemoryStream(File.ReadAllBytes(fileName));
                bool appearsIcon = Path.GetExtension(fileName).Equals(".ico", StringComparison.OrdinalIgnoreCase);
                string? openedFileName = fileName;
                string? notification = null;
                ImageInfo? result = null;
                bool isCustom = false;

                // icon is allowed and the content seems to be an icon
                // (this block is needed only for Windows XP: Icon Bitmap with PNG throws an exception but initializing from icon will succeed)
                if (appearsIcon && (imageTypes & AllowedImageTypes.Icon) == AllowedImageTypes.Icon)
                {
                    try
                    {
                        result = new ImageInfo(Icons.FromStream(stream));
                    }
                    catch (Exception e) when (!e.IsCritical())
                    {
                        // failed to open as an icon: fallback to usual paths
                        stream.Position = 0L;
                    }
                }

                if (result == null)
                {
                    // bitmaps and metafiles are both allowed
                    if ((imageTypes & (AllowedImageTypes.Bitmap | AllowedImageTypes.Metafile)) == (AllowedImageTypes.Bitmap | AllowedImageTypes.Metafile))
                    {
                        try
                        {
                            result = new ImageInfo(LoadImage(stream, out isCustom));
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
                            result = new ImageInfo(new Metafile(stream));
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
                            result = new ImageInfo(LoadImage(stream, out isCustom));
                        }
                        catch (Exception e) when (!e.IsCriticalGdi())
                        {
                            throw new ArgumentException(Res.ErrorMessageNotABitmapFile(e.Message), nameof(fileName), e);
                        }

                        if (!isCustom && result.RawFormat == ImageFormat.MemoryBmp.Guid)
                        {
                            notification = Res.NotificationMetafileAsBitmapId;
                            openedFileName = null;
                        }
                    }

                    // icon is allowed and an image has been loaded
                    if (result?.Image is Image image && (imageTypes & AllowedImageTypes.Icon) != AllowedImageTypes.None)
                    {
                        // the loaded format is icon: reloading as icon (can make a difference if the icon contains multiple color depths of the same resolution)
                        if (result.RawFormat == ImageFormat.Icon.Guid)
                        {
                            stream.Position = 0L;
                            try
                            {
                                var newResult = new ImageInfo(new Icon(stream));
                                result.Dispose();
                                result = newResult;
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
                            var newResult = new ImageInfo(iconImage.ToIcon());
                            iconImage.Dispose();
                            result.Dispose();
                            result = newResult;
                            notification = Res.NotificationImageAsIconId;
                            openedFileName = null;
                        }
                    }
                }

                // null will be assigned if the image has been converted (see notifications), or when it has a custom format so Image.FromFile cannot handle it
                result?.FileName = !isCustom ? openedFileName : null;

                task.SetCompleted();

                TryInvokeSync(() =>
                {
                    ImageInfo = result;
                    SetNotification(notification);
                    SetModified(IsDebuggerVisualizer);
                    OnFileOpened(fileName); // using the original file name here, even if it was converted internally
                });
            }
            catch (Exception e) when (!e.IsCriticalGdi())
            {
                // As we are on a remote thread, just capturing the error here, and if the UI still exists, marshaling the handling back to the UI thread.
                error = e;
            }
            finally
            {
                task.Dispose();
                activeTask = null;
                TryInvokeSync(() =>
                {
                    IsBusy = false;
                    if (error != null)
                        ShowError(Res.ErrorMessageFailedToLoadFile(error.Message));
                });
            }
        }

        private Image LoadImage(MemoryStream stream, out bool isCustom)
        {
            isCustom = false;
            if (TryLoadCustom(stream, out Image? image))
            {
                isCustom = true;
                return image;
            }

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

        private void SaveFile()
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
            IsBusy = true;
            var task = new SaveTask
            {
                FileName = fileName,
                SelectedFormat = selectedFormat,
                ToSave = GetCurrentImageInfo(true),
                CurrentFrame = currentFrame
            };
            activeTask = task;
            ThreadPool.QueueUserWorkItem(DoSaveFile, task);
        }

        private void DoSaveFile(object? state)
        {
            #region Local Methods

            static Image? GetImage(SaveTask task)
            {
                if (task.IsCanceled)
                    return null;

                if (task.ToSave is not ImageInfo info)
                    return task.ToSave.Image;

                // For TIFF, this always gets the current frame.
                if (info.Type == ImageInfoType.Pages && task.CurrentFrame >= 0)
                    return info.Frames![task.CurrentFrame].Image;

                // For icons/GIF this gets the compound image if exists, or the first frame.
                // Not generating the compound image here, as this method is expected to be used for single-frame formats only.
                return info.HasFrames
                    ? info.Image ?? info.Frames![0].Image ?? throw new InvalidOperationException(Res.InternalError("A frame was null. Only the serializer should initialize such ImageInfo."))
                    : task.ToSave.Image;
            }

            static void SaveGif(SaveTask task)
            {
                if (task.IsCanceled)
                    return;

                if (task.ToSave.Image is Image image)
                {
                    image.SaveAsGif(task.FileName);
                    return;
                }

                // Encoding a new GIF animation: if the image originally is not an animation, using 1s delay for each frames
                if (task.ToSave is not ImageInfo imageInfo)
                    return;

                // TODO: cancellable iteration
                ImageFrameInfo[] frames = imageInfo.Frames!;
                frames.Select(f => f.Image!).SaveAsAnimatedGif(task.FileName, imageInfo.Type == ImageInfoType.Animation
                    ? frames.Select(f => TimeSpan.FromMilliseconds(f.Duration))
                    : new[] { TimeSpan.FromSeconds(1) });
            }

            static void SaveTiff(SaveTask task)
            {
                if (task.ToSave is ImageInfo imageInfo && imageInfo.HasFrames)
                    // TODO: cancellable iteration
                    imageInfo.Frames!.Select(f => f.Image!).SaveAsMultipageTiff(task.FileName);
                else
                    GetImage(task)?.SaveAsTiff(task.FileName);
            }

            static void SaveIcon(SaveTask task)
            {
                using Stream stream = File.Create(task.FileName);

                // TODO: new ImageInfoBase.Icon property, save that in the first place

                if (task.ToSave is not ImageInfo imageInfo)
                    task.ToSave.Image!.SaveAsIcon(stream);
                else
                {
                    // Preferring the Icon property, if exists. SaveAsIcon ensures the good quality even if the icon has no raw stream internally.
                    if (imageInfo.Icon is Icon icon)
                        icon.SaveAsIcon(stream);
                    else if (imageInfo.HasFrames)
                    {
                        // TODO: cancellable iterator (or combining one by one), restoring original pixel format if raw format is icon (result.Combine(frame.Icon) or use result.Combine(frame.Image, Color.Transparent))
                        using Icon i = Icons.Combine(imageInfo.Frames!.Select(f => (Bitmap)f.Image!));
                        i.Save(stream);
                    }
                }

                stream.Flush();
            }

            static void SaveBitmapData(SaveTask task)
            {
                using Stream stream = File.Create(task.FileName);
                Image? image = GetImage(task);
                if (image == null)
                    return;
                Bitmap bmp = image as Bitmap ?? new Bitmap(image);
                try
                {
                    // TODO: cancellation, Begin/End save
                    using IReadableBitmapData bitmapData = bmp.GetReadableBitmapData();
                    bitmapData.Save(stream);
                }
                finally
                {
                    if (!ReferenceEquals(image, bmp))
                        bmp.Dispose();
                }
            }

            #endregion

            var task = (SaveTask)state!;
            (string fileName, string selectedFormat, ImageInfoBase toSave) = (task.FileName, task.SelectedFormat, task.ToSave);
            Exception? error = null;

            try
            {
                ImageCodecInfo? encoder = encoderCodecs.FirstOrDefault(e => selectedFormat.Equals(e.FilenameExtension, StringComparison.OrdinalIgnoreCase));

                // BMP
                if (encoder?.FormatID == ImageFormat.Bmp.Guid)
                    GetImage(task)?.SaveAsBmp(fileName);
                // JPEG
                else if (encoder?.FormatID == ImageFormat.Jpeg.Guid)
                    GetImage(task)?.SaveAsJpeg(fileName, 95);
                // GIF
                else if (encoder?.FormatID == ImageFormat.Gif.Guid)
                    SaveGif(task);
                // Tiff
                else if (encoder?.FormatID == ImageFormat.Tiff.Guid)
                    SaveTiff(task);
                // PNG
                else if (encoder?.FormatID == ImageFormat.Png.Guid)
                    GetImage(task)?.SaveAsPng(fileName);
                // icon
                else if (selectedFormat == "*.ico")
                    SaveIcon(task);
                // windows metafile
                else if (selectedFormat == "*.wmf")
                    (GetImage(task) as Metafile)?.SaveAsWmf(fileName);
                // enhanced metafile
                else if (selectedFormat == "*.emf")
                    (GetImage(task) as Metafile)?.SaveAsEmf(fileName);
                // Some unrecognized encoder - we assume it can handle every pixel format
                else if (encoder != null)
                    GetImage(task)?.Save(fileName, encoder, null);
                else if (selectedFormat == "*.bdat")
                    SaveBitmapData(task);
                else
                    throw new InvalidOperationException(Res.InternalError($"Unexpected format without encoder: {selectedFormat}"));
            }
            catch (Exception e) when (!e.IsCriticalGdi())
            {
                // As we are on a remote thread, just capturing the error here, and if the UI still exists, marshaling the handling back to the UI thread.
                error = e;
            }
            finally
            {
                if (task.IsCanceled && File.Exists(task.FileName))
                {
                    try
                    {
                        File.Delete(task.FileName);
                    }
                    catch (Exception e) when (!e.IsCritical())
                    {
                    }
                }

                task.Dispose();
                activeTask = null;
                TryInvokeSync(() =>
                {
                    IsBusy = false;
                    if (error != null)
                        ShowError(Res.ErrorMessageFailedToSaveImage(error.Message));
                    else if (!task.IsCanceled)
                        OnFileSaved(fileName, selectedFormat);
                });
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

        /// <summary>
        /// Similar to InitDefaults, but executed when a new file is loaded or the image is updated
        /// </summary>
        private void AdjustZoom(bool resetZoom)
        {
            if (imageInfo.Type == ImageInfoType.None)
            {
                SetAutoZoomCommandState.Enabled = AutoZoom = false;
                return;
            }

            // metafile: turning on auto zoom for a new file, and preserving current smooth zooming
            if (imageInfo.IsMetafile)
            {
                if (resetZoom)
                    AutoZoom = true;
                return;
            }

            // if we are just opening a new image we don't auto toggle AutoZoom and SmoothZooming anymore
            if (!AutoZoom && resetZoom)
                Zoom = 1f;
        }

        /// <summary>
        /// Similar to AdjustZoom, but works from Configuration, and attempts to initialize view size as well (for classic visualizers)
        /// </summary>
        private void InitDefaults()
        {
            Debug.Assert(!initialized);
            if (imageInfo.Type == ImageInfoType.None)
            {
                SetAutoZoomCommandState.Enabled = AutoZoom = false;
                SmoothZooming = Configuration.SmoothZoomingDefault;
                return;
            }

            // metafile: we always turn on auto zoom, and get last smoothing for metafiles (true by default)
            if (imageInfo.IsMetafile)
            {
                AutoZoom = true;
                SmoothZooming = Configuration.SmoothZoomingMetafile;
                return;
            }

            bool isMultiRes = imageInfo.IsMultiRes && IsCompoundView;
            bool smoothZooming = isMultiRes && IsCompoundView ? Configuration.SmoothZoomingMultiResIcon : Configuration.SmoothZoomingBitmap;
            bool autoZoom = isMultiRes ? Configuration.AutoZoomMultiResIcon : Configuration.AutoZoomBitmap;
            SmoothZooming = smoothZooming;

            // trying to auto-size the view so the image fits into it without shrinking or showing the scrollbars
            Rectangle workingArea = GetScreenRectangleCallback?.Invoke() ?? default;
            if (workingArea.IsEmpty)
            {
                AutoZoom = autoZoom;
                return;
            }

            Size screenSize = workingArea.Size;
            Size viewSize = GetViewSizeCallback?.Invoke() ?? default;
            Size imageViewerSize = GetImagePreviewSizeCallback?.Invoke() ?? default;
            Size padding = viewSize - imageViewerSize;
            Size desiredSize = GetCurrentImageInfo().Size + padding; // not GetSize so the max size is returned for an icon in compound view

            if (desiredSize.Width <= screenSize.Width && desiredSize.Height <= screenSize.Height)
            {
                // for icons forcing auto zoom first, so shrinking the view will not cause twitching as the scrollbar appears and disappears
                AutoZoom = autoZoom || isMultiRes;
                if (ApplyViewSizeCallback?.Invoke(new Size(Math.Max(desiredSize.Width, viewSize.Width), Math.Max(desiredSize.Height, viewSize.Height))) == true
                    || imageViewerSize.Width >= imageInfo.Size.Width && imageViewerSize.Height >= imageInfo.Size.Height)
                {
                    if (imageInfo.IsMultiRes && !autoZoom)
                        AutoZoom = autoZoom;
                    if (!autoZoom)
                        Zoom = 1f;

                    return;
                }
            }
            
            // image is too large to fit: when auto zoom is requested (considering shrinking as well), forcing smooth zooming
            autoZoom |= Configuration.AutoShrinkLargeBitmap;
            AutoZoom = autoZoom;
            if (autoZoom)
                SmoothZooming = true;
            else
                Zoom = 1f;
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
            if (GetCurrentImageInfo() == imageInfo)
            {
                Debug.Assert(!imageInfo.HasFrames, "To replace the whole compound image, set ImageInfo instead");
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
                imageInfo.Icon?.Dispose();
                imageInfo.Icon = null;
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

            ImageInfoBase image = GetCurrentImageInfo();

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
            ImageInfoBase image = GetCurrentImageInfo();
            Debug.Assert(image.Image is Bitmap, "Existing bitmap image is expected");
            var bmp = (Bitmap)image.Image!;
            Bitmap? clone = null;

            // must be in a lock because it can be in use in the UI (where it is also locked)
            lock (bmp)
            {
                if (bmp.PixelFormat != PixelFormatExtensions.Format32bppCmyk)
                    bmp.RotateFlip(direction);
                else
                {
                    clone = bmp.CloneCurrentFrame();
                    clone.RotateFlip(direction);
                }
            }

            SetCurrentImage(clone ?? bmp);
        }

        private void UpdateSmoothZoomingTooltip()
            => SetSmoothZoomingCommandState[stateToolTipText] =
                imageInfo.Type == ImageInfoType.None ? null
                : imageInfo.IsMetafile ? Res.ToolTipTextSmoothMetafile
                : Res.ToolTipTextSmoothBitmap;

        private void UpdateNotification() => Notification = notificationId == null ? null : Res.Get(notificationId);

        private void CopyToClipboard()
        {
            Debug.Assert(!IsBusy && activeTask == null);
            IsBusy = true;
            var task = new CopyTask { ToCopy = GetCurrentImageInfo(true) };
            activeTask = task;
            ThreadPool.QueueUserWorkItem(DoCopyToClipboard, task);
        }

        private void DoCopyToClipboard(object? state)
        {
            var task = (CopyTask)state!;
            try
            {
                if (task.IsCanceled)
                    return;
                ClipboardHelper.CopyToClipboard(task.ToCopy, task);
            }
            catch (Exception e) when (!e.IsCritical())
            {
                task.SetCompleted(); // just to complete it before showing the message dialog
                TryInvokeSync(() => ShowWarning(Res.WarningMessageCannotCopyClipboard));
            }
            finally
            {
                task.Dispose();
                activeTask = null;
                TryInvokeSync(() => IsBusy = false);
            }
        }

        private void PasteFromClipboard()
        {
            Debug.Assert(!IsBusy && activeTask == null);
            IsBusy = true;
            ImageInfoBase currentImage = GetCurrentImageInfo(true);
            var task = new PasteTask
            {
                AllowedTypes = currentImage is ImageInfo ? imageTypes : AllowedImageTypes.Bitmap,
                AllowMultiFrame = currentImage is ImageInfo,
                PrevEnabled = PrevImageCommandState.Enabled,
                NextEnabled = NextImageCommandState.Enabled
            };

            // Unlike for other async tasks, disabling compound/prev/next for pasting so compound view or current frame remains the same until the end of the operation.
            SetCompoundViewCommandState.Enabled = false;
            PrevImageCommandState.Enabled = NextImageCommandState.Enabled = false;
            activeTask = task;
            ThreadPool.QueueUserWorkItem(DoPasteFromClipboard, task);
        }

        [SuppressMessage("ReSharper", "AccessToDisposedClosure", Justification = "False alarm, task is not accessed after disposing it")]
        private void DoPasteFromClipboard(object? state)
        {
            var task = (PasteTask)state!;
            ImageInfo? result = null;
            bool success = false;
            try
            {
                if (task.IsCanceled)
                    return;
                result = ClipboardHelper.TryPasteFromClipboard(task.AllowedTypes, task.AllowMultiFrame, task);

                // It must be completed before handling the rest on the UI thread. From now on, nothing is done in the worker thread, but the nullification.
                // UI callbacks must be expected not to be executed at all, if the UI has been closed.
                task.SetCompleted();

                success = TryInvokeSync(() =>
                {
                    if (IsDisposed)
                        return;
                    if (result == null)
                    {
                        ShowWarning(Res.WarningMessageCannotPasteClipboard);
                        return;
                    }

                    if (task.AllowMultiFrame)
                        ImageInfo = result;
                    else
                    {
                        Debug.Assert(result.Image is Bitmap, "Pasting a frame is always expected as a bitmap");
                        SetCurrentImage(result.Image as Bitmap);

                        result.Image = null;
                        result.Dispose();
                    }

                    SetCompoundViewCommandState.Enabled = true;
                    PrevImageCommandState.Enabled = task.PrevEnabled;
                    NextImageCommandState.Enabled = task.NextEnabled;
                    SetModified(true);
                });
            }
            catch (Exception e) when (!e.IsCriticalGdi())
            {
            }   
            finally
            {
                if (!success && task.AllowMultiFrame && !ReferenceEquals(imageInfo, result))
                    result?.Dispose();
                task.Dispose();
                activeTask = null;
                TryInvokeSync(() => IsBusy = false);
            }
        }

        #endregion

        #region Explicitly Implemented Interface Methods

        Image? IViewModel<Image?>.GetEditedModel() => Image?.Clone() as Image;
        Icon? IViewModel<Icon?>.GetEditedModel() => Icon?.Clone() as Icon;
        Bitmap? IViewModel<Bitmap?>.GetEditedModel() => Image?.Clone() as Bitmap;
        Metafile? IViewModel<Metafile?>.GetEditedModel() => Image?.Clone() as Metafile;
        bool IViewModel<Image?>.TrySetModel(Image? model) => TryInvokeSync(() => SetImageInfo(new ImageInfo(model), false));
        bool IViewModel<Icon?>.TrySetModel(Icon? model) => TryInvokeSync(() => SetImageInfo(new ImageInfo(model), false));
        bool IViewModel<Bitmap?>.TrySetModel(Bitmap? model) => ((IViewModel<Image?>)this).TrySetModel(model);
        bool IViewModel<Metafile?>.TrySetModel(Metafile? model) => ((IViewModel<Image?>)this).TrySetModel(model);

        #endregion

        #region Command Handlers

        private void OnSetAutoZoomCommand(bool newValue) => AutoZoom = newValue;
        private void OnSetSmoothZoomingCommand(bool newValue) => SmoothZooming = newValue;
        private void OnViewImagePreviewSizeChangedCommand() => UpdateIfMultiResImage();
        private void OnOpenFileCommand() => OpenFile();
        private void OnSaveFileCommand() => SaveFile();
        private void OnClearCommand() => Clear();
        private void OnCopyCommand() => CopyToClipboard();
        private void OnPasteCommand() => PasteFromClipboard();

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

        private void OnShowPaletteCommand() => ShowPalette();

        private void OnManageInstallationsCommand()
        {
            using (IViewModel viewModel = ViewModelFactory.CreateManageInstallations(Files.GetExecutingPath()))
                ShowChildViewCallback?.Invoke(viewModel);
        }

        private void OnCountColorsCommand()
        {
            Debug.Assert(imageInfo.Type != ImageInfoType.None && !imageInfo.IsMetafile, "Non-metafile image is expected");

            ImageInfoBase image = GetCurrentImageInfo();

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

        #region Event Handlers

        private void ClipboardHelper_ClipboardChanged(object? sender, EventArgs e)
        {
            if (!IsBusy)
                TryInvokeSync(() => PasteCommandState.Enabled = ClipboardHelper.HasFormat(imageTypes));
        }

        #endregion

        #endregion

        #endregion
    }
}
