#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageVisualizerViewModel.cs
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

#region Used Namespaces

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

#region Used Aliases

using Encoder = System.Drawing.Imaging.Encoder;

#endregion

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class ImageVisualizerViewModel : ViewModelBase, IViewModel<ImageInfo>, IViewModel<Image>, IViewModel<Icon>, IViewModel<Bitmap>, IViewModel<Metafile>
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
        private int currentFrame = -1;
        private bool isOpenFilterUpToDate;
        private Size currentResolution;
        private bool deferSettingCompoundStateImage;

        #endregion

        #endregion

        #region Properties

        #region Internal Properties

        internal Image Image
        {
            get => imageInfo.GetCreateImage();
            set => SetImageInfo(new ImageInfo(value));
        }

        internal Icon Icon
        {
            get => imageInfo.Icon;
            set => SetImageInfo(new ImageInfo(value));
        }

        internal ImageInfo ImageInfo
        {
            get => imageInfo;
            set => SetImageInfo(value ?? new ImageInfo(ImageInfoType.None));
        }

        internal Image PreviewImage { get => Get<Image>(); set => Set(value); }
        internal bool ReadOnly { get => Get<bool>(); set => Set(value); }
        internal string TitleCaption { get => Get<string>(); set => Set(value); }
        internal string InfoText { get => Get<string>(); set => Set(value); }
        internal string Notification { get => Get<string>(); set => Set(value); }
        internal bool AutoZoom { get => Get<bool>(); set => Set(value); }
        internal bool IsCompoundView { get => Get(true); set => Set(value); }
        internal bool IsAutoPlaying { get => Get<bool>(); set => Set(value); }
        internal Size ViewImagePreviewSize { get => Get<Size>(); set => Set(value); }
        internal string OpenFileFilter { get => Get<string>(); set => Set(value); }
        internal string SaveFileFilter { get => Get<string>(); set => Set(value); }
        internal int SaveFileFilterIndex { get => Get<int>(); set => Set(value); }
        internal string SaveFileDefaultExtension { get => Get<string>(); set => Set(value); }

        internal Func<Rectangle> GetScreenRectangleCallback { get => Get<Func<Rectangle>>(); set => Set(value); }
        internal Func<Size> GetViewSizeCallback { get => Get<Func<Size>>(); set => Set(value); }
        internal Action<Size> ApplyViewSizeCallback { get => Get<Action<Size>>(); set => Set(value); }
        internal Func<string> SelectFileToOpenCallback { get => Get<Func<string>>(); set => Set(value); }
        internal Func<string> SelectFileToSaveCallback { get => Get<Func<string>>(); set => Set(value); }
        internal Action UpdatePreviewImageCallback { get => Get<Action>(); set => Set(value); }
        internal Func<ImageInfoType, Image> GetCompoundViewIconCallback { get => Get<Func<ImageInfoType, Image>>(); set => Set(value); }

        internal ICommandState SetAutoZoomCommandState => Get(() => new CommandState());
        internal ICommandState OpenFileCommandState => Get(() => new CommandState());
        internal ICommandState SaveFileCommandState => Get(() => new CommandState { Enabled = false });
        internal ICommandState ClearCommandState => Get(() => new CommandState { Enabled = false });
        internal ICommandState SetCompoundViewCommandState => Get(() => new CommandState { [stateVisible] = false });
        internal ICommandState AdvanceAnimationCommandState => Get(() => new CommandState());
        internal ICommandState PrevImageCommandState => Get(() => new CommandState());
        internal ICommandState NextImageCommandState => Get(() => new CommandState());
        internal ICommandState ShowPaletteCommandState => Get(() => new CommandState { Enabled = false });

        internal ICommand SetAutoZoomCommand => Get(() => new SimpleCommand<bool>(OnSetAutoZoomCommand));
        internal ICommand OpenFileCommand => Get(() => new SimpleCommand(OnOpenFileCommand));
        internal ICommand SaveFileCommand => Get(() => new SimpleCommand(OnSaveFileCommand));
        internal ICommand ClearCommand => Get(() => new SimpleCommand(OnClearCommand));
        internal ICommand SetCompoundViewCommand => Get(() => new SimpleCommand<bool>(OnSetCompoundViewCommand));
        internal ICommand AdvanceAnimationCommand => Get(() => new SimpleCommand(OnAdvanceAnimationCommand));
        internal ICommand PrevImageCommand => Get(() => new SimpleCommand(OnPrevImageCommand));
        internal ICommand NextImageCommand => Get(() => new SimpleCommand(OnNextImageCommand));
        internal ICommand ShowPaletteCommand => Get(() => new SimpleCommand(OnShowPaletteCommand));
        internal ICommand ManageInstallationsCommand => Get(() => new SimpleCommand(OnManageInstallationsCommand));

        #endregion

        #region Protected Properties

        protected override bool AffectsModifiedState(string propertyName) => false; // set explicitly
        protected virtual bool IsPaletteReadOnly => imageInfo.Type == ImageInfoType.Icon || ReadOnly;

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

        #endregion

        #region Instance Methods

        #region Internal Methods

        internal override void ViewLoaded()
        {
            InitAutoZoom();
            if (deferSettingCompoundStateImage && SetCompoundViewCommandState.GetValueOrDefault<bool>(stateVisible))
                SetCompoundViewCommandStateImage();
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
                case nameof(ViewImagePreviewSize):
                    OnViewImagePreviewSizeChanged();
                    break;
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
            StringBuilder sb = new StringBuilder();
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

        protected virtual bool OpenFile(string path)
        {
            try
            {
                FromFile(path);
                SetModified(true);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(Res.ErrorMessageFailedToLoadFile(ex.Message));
                return false;
            }
        }

        protected virtual void Clear()
        {
            Image = null;
            SetModified(true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                imageInfo.Dispose();
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void SetImageInfo(ImageInfo value)
        {
            Debug.Assert(value != null);
            ValidateImageInfo(value);

            currentResolution = Size.Empty;
            imageInfo.Dispose();
            imageInfo = value;
            SetModified(false);
            InitAutoZoom();

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
            switch (imageInfo.Type)
            {
                case ImageInfoType.Pages:
                    SetCompoundViewCommandState[stateToolTipText] = Res.TooltipTextCompoundMultiPage;
                    break;
                case ImageInfoType.Animation:
                    SetCompoundViewCommandState[stateToolTipText] = Res.TooltipTextCompoundAnimation;
                    break;
                default:
                    SetCompoundViewCommandState[stateToolTipText] = Res.TooltipTextCompoundMultiSize;
                    break;
            }

            SetCompoundViewCommandStateImage();
            SetCompoundViewCommandState[stateVisible] = true;
            ResetCompoundState();
        }

        private ImageInfoBase GetCurrentImage()
        {
            if (!imageInfo.HasFrames || currentFrame < 0 || IsAutoPlaying)
                return imageInfo;
            return imageInfo.Frames[currentFrame];
        }


        private void SetCompoundViewCommandStateImage()
        {
            Func<ImageInfoType, Image> callback = GetCompoundViewIconCallback;
            deferSettingCompoundStateImage = callback == null;
            if (callback != null)
                SetCompoundViewCommandState[stateImage] = callback?.Invoke(imageInfo.Type);
        }

        private void ImageChanged()
        {
            ImageInfoBase image = GetCurrentImage();
            ShowPaletteCommandState.Enabled = image.Palette.Length > 0;
            SaveFileCommandState.Enabled = imageInfo.Type != ImageInfoType.None;
            ClearCommandState.Enabled = imageInfo.Type != ImageInfoType.None && !ReadOnly;
            UpdateInfo();
        }

        private void ReadOnlyChanged()
        {
            bool readOnly = ReadOnly;
            OpenFileCommandState.Enabled = !readOnly;
            ClearCommandState.Enabled = !readOnly && imageInfo.Type != ImageInfoType.None;
        }

        private string GetFrameInfo(bool singleLine)
        {
            if (!imageInfo.HasFrames)
                return String.Empty;

            StringBuilder result = new StringBuilder();
            if (singleLine)
                result.Append("; ");

            if (currentFrame != -1 && !IsAutoPlaying)
                result.Append(Res.InfoCurrentFrame(currentFrame + 1, imageInfo.Frames.Length));
            else
                result.Append(Res.InfoFramesCount(imageInfo.Frames.Length));

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
                return typeof(Icon).Name;
            Image img = GetCurrentImage().Image;
            return img?.GetType().Name ?? typeof(Bitmap).Name;
        }

        private void FromFile(string fileName)
        {
            var stream = new MemoryStream(File.ReadAllBytes(fileName));
            bool appearsIcon = Path.GetExtension(fileName).Equals(".ico", StringComparison.OrdinalIgnoreCase);

            Icon icon = null;

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
                catch (Exception)
                {
                    // failed to open as an icon: fallback to usual paths
                    stream.Position = 0L;
                }
            }

            Image image = null;

            // bitmaps and metafiles are both allowed
            if ((imageTypes & (AllowedImageTypes.Bitmap | AllowedImageTypes.Metafile)) == (AllowedImageTypes.Bitmap | AllowedImageTypes.Metafile))
            {
                try
                {
                    image = Image.FromStream(stream);
                }
                catch (Exception e)
                {
                    throw new ArgumentException(Res.ErrorMessageNotAnImageStream(e.Message), nameof(stream), e);
                }
            }
            // metafiles only
            else if (imageTypes == AllowedImageTypes.Metafile)
            {
                try
                {
                    image = new Metafile(stream);
                }
                catch (Exception e)
                {
                    throw new ArgumentException(Res.ErrorMessageNotAMetafileStream(e.Message), nameof(stream), e);
                }
            }
            // bitmaps or icons
            else if ((imageTypes & (AllowedImageTypes.Bitmap | AllowedImageTypes.Icon)) != AllowedImageTypes.None)
            {
                try
                {
                    image = new Bitmap(stream);
                }
                catch (Exception e)
                {
                    throw new ArgumentException(Res.ErrorMessageNotABitmapStream(e.Message), nameof(stream), e);
                }

                if (image.RawFormat.Guid == ImageFormat.MemoryBmp.Guid)
                {
                    Notification = Res.NotificationMetafileAsBitmap;
                    fileName = null;
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
                    catch (Exception e)
                    {
                        throw new ArgumentException(Res.ErrorMessageNotAnIconStream(e.Message), nameof(stream), e);
                    }
                }

                // not icon was loaded, though icon is the only supported format: converting to icon
                else if (imageTypes == AllowedImageTypes.Icon)
                {
                    Bitmap iconImage = image as Bitmap ?? new Bitmap(image);
                    icon = iconImage.ToIcon();
                    iconImage.Dispose();
                    Notification = Res.NotificationImageAsIcon;
                    fileName = null;
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
            imageInfo.FileName = fileName;
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
                PreviewImage = imageInfo.Frames[0].Image;
                ImageChanged();
                return;
            }

            // handle as compound image
            NextImageCommandState.Enabled = PrevImageCommandState.Enabled = false;
            bool autoPlaying = imageInfo.Type == ImageInfoType.Animation;
            ICommandState timerState = AdvanceAnimationCommandState;
            IsAutoPlaying = autoPlaying;
            PreviewImage = imageInfo.Frames[0].Image;
            if (autoPlaying)
            {
                currentFrame = 0;
                timerState[stateInterval] = imageInfo.Frames[0].Duration;
            }
            else
            {
                currentFrame = -1;
                if (imageInfo.IsMultiRes)
                    UpdateMultiResImage();
            }

            timerState.Enabled = autoPlaying;
            ImageChanged();
        }

        private void OnViewImagePreviewSizeChanged()
        {
            if (!imageInfo.IsMultiRes || currentFrame != -1)
                return;
            UpdateMultiResImage();
            UpdateInfo();
        }

        private void UpdateMultiResImage()
        {
            Debug.Assert(imageInfo.IsMultiRes);
            Size origSize = currentResolution;
            Size clientSize = ViewImagePreviewSize;
            int desiredSize = Math.Min(clientSize.Width, clientSize.Height);
            if (desiredSize < 1 && !origSize.IsEmpty)
                return;

            // Starting with Windows Vista it would work that we draw the compound image in a new Bitmap with desired size and read the Size afterwards
            // but that requires always a new bitmap and does not work in Windows XP
            desiredSize = Math.Max(desiredSize, 1);
            ImageFrameInfo desiredImage = imageInfo.Frames.Aggregate((acc, i) => i.Size == acc.Size && i.BitsPerPixel > acc.BitsPerPixel || Math.Abs(i.Size.Width - desiredSize) < Math.Abs(acc.Size.Width - desiredSize) ? i : acc);
            currentResolution = desiredImage.Size;
            if (PreviewImage != desiredImage.Image)
                PreviewImage = desiredImage.Image;
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
                    using (Icon i = Icons.Combine(imageInfo.Frames.Select(f => (Bitmap)f.Image).ToArray()))
                        i.Save(stream);
                }
                // single image icon without raw data
                else
                {
                    using (Icon i = Icons.Combine((Bitmap)imageInfo.Image))
                        i.Save(stream);
                }

                stream.Flush();
                return;
            }

            // saving a single icon image
            if (imageInfo.Icon != null)
            {
                using (Icon i = imageInfo.Icon.ExtractIcon(currentFrame))
                    i.Save(stream);
            }
            else
            {
                using (Icon i = Icons.Combine((Bitmap)GetCurrentImage().Image))
                    i.Save(stream);
            }

            stream.Flush();
        }

        private void SaveMetafile(string fileName, bool asWmf)
        {
            using (Stream stream = File.Create(fileName))
            {
                ((Metafile)imageInfo.Image).Save(stream, asWmf);
                stream.Flush();
            }
        }

        private void SaveJpeg(string fileName, ImageCodecInfo jpegEncoder)
        {
            using (Stream stream = File.Create(fileName))
            using (EncoderParameters encoderParams = new EncoderParameters(1))
            {
                encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 95L);
                GetCurrentImage().Image.Save(stream, jpegEncoder, encoderParams);
            }
        }

        private void SaveGif(string fileName)
        {
            ImageInfoBase currentImage = GetCurrentImage();
            int realBpp = currentImage.Image.GetBitsPerPixel();
            int theoreticBpp = currentImage.BitsPerPixel;

            using (Stream stream = File.Create(fileName))
            {
                // we know that image is indexed but in .NET it is loaded as hi-res (eg. GIF frames, icon images)
                if (theoreticBpp <= 8 && realBpp > 8 && currentImage.Image is Bitmap bmp)
                {
                    Color[] palette = bmp.GetColors(1 << theoreticBpp);
                    bmp.SaveAsGif(stream, palette);
                    return;
                }

                // saving with or without original palette with transparency - works also for animGif if no change has been made (and now even the palette cannot be changed due to the GIF decoder)
                // TODO: if changing frames/palette will be available, SaveAnimGif if needed
                // TODO: Allow dithering on SaveAs dialog (which replaces palette)
                currentImage.Image.SaveAsGif(stream);
            }
        }

        private void SaveMultipageTiff(string fileName)
        {
            using (Stream stream = File.Create(fileName))
                imageInfo.Frames.Select(f => f.Image).SaveAsMultipageTiff(stream);
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
                sb.Append($"{codecInfo.FormatDescription} {Res.TextFiles}|{codecInfo.FilenameExtension.ToLowerInvariant()}");
                if (sbImages.Length != 0)
                    sbImages.Append(';');
                sbImages.Append(codecInfo.FilenameExtension);
            }

            OpenFileFilter = $"{(imageTypes == AllowedImageTypes.Metafile ? Res.TextMetafiles : Res.TextImages)} ({sbImages})|{sbImages}|{sb}|{Res.TextAllFiles} (*.*)|*.*";
            isOpenFilterUpToDate = true;
        }

        private void SetSaveFilter()
        {
            // enlisting encoders
            StringBuilder sb = new StringBuilder();
            foreach (ImageCodecInfo codecInfo in encoderCodecs)
            {
                if (sb.Length != 0)
                    sb.Append("|");
                sb.Append($"{codecInfo.FormatDescription} {Res.TextFileFormat}|{codecInfo.FilenameExtension.ToLowerInvariant()}");
            }

            bool isEmf = false;
            if (imageInfo.IsMultiRes)
                sb.Append($"|{Res.TextIcon} {Res.TextFileFormat}|*.ico");
            else if (imageInfo.IsMetafile)
            {
                sb.Append($"|WMF {Res.TextFileFormat}|*.wmf");
                isEmf = imageInfo.RawFormat == ImageFormat.Emf.Guid;
                if (isEmf)
                    sb.Append($"|EMF {Res.TextFileFormat}|*.emf");
            }

            SaveFileFilter = sb.ToString();

            // selecting appropriate format
            if (imageInfo.IsMultiRes)
            {
                SaveFileFilterIndex = encoderCodecs.Length + 1;
                SaveFileDefaultExtension = "ico";
            }
            else if (imageInfo.IsMetafile)
            {
                SaveFileFilterIndex = encoderCodecs.Length + (isEmf ? 2 : 1);
                SaveFileDefaultExtension = isEmf ? "emf" : "wmf";
            }
            else
            {
                int posPng = 0;
                int index = 0;
                bool found = false;
                for (int i = 0; i < encoderCodecs.Length; i++)
                {
                    if (imageInfo.RawFormat == encoderCodecs[i].FormatID)
                    {
                        index = i + 1;
                        found = true;
                        break;
                    }

                    if (encoderCodecs[i].FormatDescription == "PNG")
                        posPng = i + 1;
                }

                // if encoder not found, selecting png encoder
                index = found ? index : posPng;
                SaveFileFilterIndex = index;

                // setting default extension
                string ext = encoderCodecs[index - 1].FilenameExtension;
                int sep = ext.IndexOf(';');
                if (sep > 0)
                    ext = ext.Substring(0, sep);

                SaveFileDefaultExtension = ext.Substring(ext.IndexOf('.') + 1).ToLowerInvariant();
            }
        }

        private void InitAutoZoom()
        {
            if (GetCurrentImage() == null)
            {
                SetAutoZoomCommandState.Enabled = AutoZoom = false;
                return;
            }

            if (imageInfo.IsMetafile)
            {
                SetAutoZoomCommandState.Enabled = AutoZoom = true;
                return;
            }

            SetAutoZoomCommandState.Enabled = true;
            Rectangle workingArea = GetScreenRectangleCallback?.Invoke() ?? default;
            Size screenSize = workingArea.Size;
            Size viewSize = GetViewSizeCallback?.Invoke() ?? default;
            Size padding = viewSize - ViewImagePreviewSize;
            Size desiredSize = imageInfo.Size + padding;
            if (desiredSize.Width <= screenSize.Width && desiredSize.Height <= screenSize.Height)
            {
                AutoZoom = false;
                ApplyViewSizeCallback?.Invoke(new Size(Math.Max(desiredSize.Width, viewSize.Width), Math.Max(desiredSize.Height, viewSize.Height)));
            }
            else
                AutoZoom = true;
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
        }

        #endregion

        #region Explicitly Implemented Interface Methods

        Image IViewModel<Image>.GetEditedModel() => (Image)Image?.Clone();
        Icon IViewModel<Icon>.GetEditedModel() => Icon?.Clone() as Icon;
        Bitmap IViewModel<Bitmap>.GetEditedModel() => Image?.Clone() as Bitmap;
        Metafile IViewModel<Metafile>.GetEditedModel() => Image?.Clone() as Metafile;
        ImageInfo IViewModel<ImageInfo>.GetEditedModel() => imageTypes == AllowedImageTypes.Icon ? imageInfo.AsIcon() : imageInfo.AsImage();

        #endregion

        #region Command Handlers

        private void OnSetAutoZoomCommand(bool newValue) => AutoZoom = newValue;

        private void OnOpenFileCommand()
        {
            SetOpenFilter();
            string fileName = SelectFileToOpenCallback?.Invoke();
            if (fileName == null)
                return;

            Notification = null;
            OpenFile(fileName);
        }

        private void OnSaveFileCommand()
        {
            if (imageInfo.Type == ImageInfoType.None)
                return;

            SetSaveFilter();
            string fileName = SelectFileToSaveCallback?.Invoke();
            if (fileName == null)
                return;

            int filterIndex = SaveFileFilterIndex;
            try
            {
                // icon
                if (filterIndex == encoderCodecs.Length + 1 && imageInfo.IsMultiRes)
                    SaveIcon(fileName);
                // metafile
                else if (filterIndex >= encoderCodecs.Length + 1 && imageInfo.IsMetafile)
                    SaveMetafile(fileName, filterIndex == encoderCodecs.Length + 1);
                // JPG
                else if (encoderCodecs[filterIndex - 1].FormatID == ImageFormat.Jpeg.Guid)
                    SaveJpeg(fileName, encoderCodecs[filterIndex - 1]);
                // Multipage tiff
                else if (imageInfo.HasFrames && encoderCodecs[filterIndex - 1].FormatID == ImageFormat.Tiff.Guid
                    && (imageInfo.Type == ImageInfoType.Pages && IsCompoundView))
                {
                    SaveMultipageTiff(fileName);
                }
                // gif
                else if (encoderCodecs[filterIndex - 1].FormatID == ImageFormat.Gif.Guid)
                    SaveGif(fileName);
                else
                    GetCurrentImage().Image.Save(fileName, encoderCodecs[filterIndex - 1], null);
            }
            catch (Exception ex)
            {
                ShowError(Res.ErrorMessageFailedToSaveImage(ex.Message));
            }
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
            ImageFrameInfo[] frames = imageInfo.Frames;
            if (currentFrame >= frames.Length)
                currentFrame = 0;
            AdvanceAnimationCommandState[stateInterval] = frames[currentFrame].Duration;
            PreviewImage = frames[currentFrame].Image;
        }

        private void OnPrevImageCommand()
        {
            if (!imageInfo.HasFrames || currentFrame <= 0)
                return;

            PreviewImage = imageInfo.Frames[--currentFrame].Image;
            PrevImageCommandState.Enabled = currentFrame > 0;
            NextImageCommandState.Enabled = true;
            ImageChanged();
        }

        private void OnNextImageCommand()
        {
            ImageFrameInfo[] frames = imageInfo.Frames;
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
            if (currentImage == null || currentImage.Palette.Length == 0)
                return;

            using (IViewModel<Color[]> vmPalette = ViewModelFactory.FromPalette(currentImage.Palette, IsPaletteReadOnly))
            {
                ShowChildViewCallback?.Invoke(vmPalette);
                if (!vmPalette.IsModified)
                    return;

                // apply changes
                ColorPalette palette = currentImage.Image.Palette;
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

                currentImage.Image.Palette = palette; // the preview changes only if we apply the palette
                currentImage.Palette = palette.Entries; // the actual palette will be taken from here
                InvalidateImage();
                UpdatePreviewImageCallback.Invoke();
            }
        }

        private void OnManageInstallationsCommand()
        {
            using (IViewModel viewModel = ViewModelFactory.CreateManageInstallations(Files.GetExecutingPath()))
                ShowChildViewCallback?.Invoke(viewModel);
        }

        #endregion

        #endregion

        #endregion
    }
}
