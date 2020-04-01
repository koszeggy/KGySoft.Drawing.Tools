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

using KGySoft.CoreLibraries;

#region Used Namespaces

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

using KGySoft.ComponentModel;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

#region Used Aliases

using Encoder = System.Drawing.Imaging.Encoder;

#endregion

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class ImageVisualizerViewModel : ViewModelBase, IViewModel<ImageReference>, IViewModel<Image>, IViewModel<Icon>
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

        private ImageData mainImage;
        private Icon underlyingIcon;
        private int currentFrame = -1;
        private ImageData[] frames;
        private ImageTypes imageTypes = ImageTypes.All;
        private bool isOpenFilterUpToDate;
        private Size currentIconSize;
        private bool deferSettingCompoundStateImage;

        #endregion

        #endregion

        #region Properties

        #region Internal Properties

        internal virtual Image Image
        {
            get => mainImage?.Image;
            set => SetImage(value, null);
        }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        internal Icon Icon
        {
            get => underlyingIcon;
            set
            {
                if (underlyingIcon == value)
                    return;
                SetImage(null, value);
            }
        }

        internal ImageTypes ImageTypes
        {
            get => imageTypes;
            set
            {
                if (imageTypes == value)
                    return;
                imageTypes = value;
                isOpenFilterUpToDate = false;
            }
        }

        internal Image PreviewImage { get => Get<Image>(); set => Set(value); }
        internal bool ReadOnly { get => Get<bool>(); set => Set(value); } // TODO: no change detection is needed, View is initialized once by this
        internal string FileName { get => Get<string>(); set => Set(value); }
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
        internal Func<Guid, Image> GetCompoundViewIconCallback { get => Get<Func<Guid, Image>>(); set => Set(value); }

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
        protected virtual bool IsPaletteReadOnly => mainImage.RawFormat == ImageFormat.Icon.Guid || ReadOnly;

        #endregion

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

        internal override void ViewCreated()
        {
            InitAutoZoom();
            if (deferSettingCompoundStateImage && SetCompoundViewCommandState.GetValueOrDefault<bool>(stateVisible))
                SetCompoundViewCommandStateImage();
        }

        internal void InitFromSingleImage(ImageData imageData, Icon newIcon)
        {
            mainImage?.Dispose();
            underlyingIcon?.Dispose();
            underlyingIcon = newIcon;
            FreeFrames();
            mainImage = imageData;
            SetModified(false);
            InitAutoZoom();
            InitSingleImage();
        }

        internal void InitFromFrames(ImageData mainImage, ImageData[] frameImages, Icon newIcon)
        {
            this.mainImage?.Dispose();
            underlyingIcon?.Dispose();
            underlyingIcon = newIcon;
            FreeFrames();

            if (mainImage == null)
                mainImage = frameImages[0];

            this.mainImage = mainImage;
            frames = frameImages;
            SetModified(false);
            InitAutoZoom();

            if (frameImages == null || frameImages.Length <= 1)
                InitSingleImage();
            else
                InitMultiImage();
        }

        #endregion

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == nameof(IsCompoundView))
            {
                if (frames == null || frames.Length <= 1 || mainImage.RawFormat == ImageFormat.Tiff.Guid)
                    return;
                ResetCompoundState();
                return;
            }

            if (e.PropertyName == nameof(ViewImagePreviewSize))
                OnViewImagePreviewSizeChanged();
        }

        protected virtual void UpdateInfo()
        {
            if (mainImage?.Image == null && frames == null)
            {
                TitleCaption = Res.TitleNoImage;
                InfoText = null;
                return;
            }

            ImageData img = GetCurrentImage();
            StringBuilder sb = new StringBuilder();
            sb.Append(Res.TitleType(GetTypeName()));
            if (mainImage.Image is Bitmap)
                sb.Append(Res.TitleSize(GetSize()));
            sb.Append(GetFrameInfo(true));

            TitleCaption = sb.ToString();
            sb.Length = 0;
            sb.Append(Res.InfoImage(GetTypeName(),
                GetSize(),
                img.PixelFormat,
                RawFormatToString(img.RawFormat),
                img.HorizontalRes, img.VerticalRes,
                GetFrameInfo(false)));

            if (img.Image is Bitmap)
            {
                sb.AppendLine();
                sb.Append(Res.InfoPalette(img.Palette.Length));
            }

            InfoText = sb.ToString();
        }

        protected ImageData GetCurrentImage()
        {
            if (frames == null || frames.Length <= 1 || currentFrame < 0 || IsAutoPlaying)
                return mainImage;
            return frames[currentFrame];
        }

        protected virtual bool OpenFile(string path)
        {
            try
            {
                var ms = new MemoryStream(File.ReadAllBytes(path));
                FromStream(ms, Path.GetExtension(path).Equals(".ico", StringComparison.OrdinalIgnoreCase));

                // File name is set if the replaced image type can be returned from file.
                // Null is set if the image should be serialized.
                SetModified(true);
                FileName = Notification == null ? path : null;
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
            FileName = String.Empty; // empty file name indicates that the image has been cleared
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                FreeFrames();

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void SetImage(Image image, Icon icon)
        {
            ImageData mainImage;
            ImageData[] frames = null;
            currentIconSize = Size.Empty;
            if (icon != null)
                ImageData.FromIcon(icon, false, out mainImage, out frames);
            else if (image != null)
                ImageData.FromImage(image, false, out mainImage, out frames);
            else
                mainImage = new ImageData(null, true);

            if (frames == null)
                InitFromSingleImage(mainImage, icon);
            else
                InitFromFrames(mainImage, frames, icon);
        }

        private void InitMultiImage()
        {
            if (mainImage.RawFormat == ImageFormat.Gif.Guid)
                SetCompoundViewCommandState[stateToolTipText] = Res.TooltipTextCompoundAnimation;
            else if (mainImage.RawFormat == ImageFormat.Icon.Guid)
                SetCompoundViewCommandState[stateToolTipText] = Res.TooltipTextCompoundMultiSize;
            else
                SetCompoundViewCommandState[stateToolTipText] = Res.TooltipTextCompoundMultiPage;

            SetCompoundViewCommandStateImage();
            SetCompoundViewCommandState[stateVisible] = true;
            ResetCompoundState();
        }

        private void SetCompoundViewCommandStateImage()
        {
            Func<Guid, Image> callback = GetCompoundViewIconCallback;
            deferSettingCompoundStateImage = callback == null;
            if (callback != null)
                SetCompoundViewCommandState[stateImage] = callback?.Invoke(mainImage.RawFormat);
        }

        private void InitSingleImage()
        {
            currentFrame = -1;
            SetCompoundViewCommandState[stateVisible] = false;
            PreviewImage = mainImage.Image;
            ImageChanged();
        }

        private void ImageChanged()
        {
            ImageData image = GetCurrentImage();
            ShowPaletteCommandState.Enabled = image.Palette.Length > 0;
            SaveFileCommandState.Enabled = image.Image != null;
            ClearCommandState.Enabled = image.Image != null && !ReadOnly;
            UpdateInfo();
        }

        private void FreeFrames()
        {
            if (frames == null)
                return;
            foreach (ImageData frame in frames)
                frame.Dispose();
            frames = null;
        }

        private string GetFrameInfo(bool singleLine)
        {
            if (frames == null || frames.Length <= 1)
                return String.Empty;

            StringBuilder result = new StringBuilder();
            if (singleLine)
                result.Append("; ");

            if (currentFrame != -1 && !IsAutoPlaying)
                result.Append(Res.InfoCurrentFrame(currentFrame + 1, frames.Length));
            else
                result.Append(Res.InfoFramesCount(frames.Length));

            if (!singleLine)
                result.AppendLine();
            return result.ToString();
        }

        private string GetSize()
        {
            if (mainImage.RawFormat == ImageFormat.Icon.Guid && frames != null && frames.Length > 1 && currentFrame == -1)
                return currentIconSize.ToString();
            return GetCurrentImage().Size.ToString();
        }

        private string GetTypeName()
        {
            if (underlyingIcon != null)
                return typeof(Icon).Name;
            Image img = GetCurrentImage().Image;
            return img?.GetType().Name ?? typeof(Bitmap).Name;
        }

        private void FromStream(Stream stream, bool appearsIcon)
        {
            Icon icon = null;

            // icon is allowed and the content seems to be an icon
            // (this block is needed only for Windows XP: Icon Bitmap with PNG throws an exception but initializing from icon will succeed)
            if (appearsIcon && (imageTypes & ImageTypes.Icon) == ImageTypes.Icon)
            {
                try
                {
                    icon = Icons.FromStream(stream);
                    SetImage(null, icon);
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
            if ((imageTypes & (ImageTypes.Bitmap | ImageTypes.Metafile)) == (ImageTypes.Bitmap | ImageTypes.Metafile))
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
            else if (imageTypes == ImageTypes.Metafile)
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
            else if ((imageTypes & (ImageTypes.Bitmap | ImageTypes.Icon)) != ImageTypes.None)
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
                    Notification = Res.NotificationMetafileAsBitmap;
            }

            // icon is allowed and an image has been loaded
            if (image != null && (imageTypes & ImageTypes.Icon) != ImageTypes.None)
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
                else if (imageTypes == ImageTypes.Icon)
                {
                    Bitmap iconImage = image as Bitmap ?? new Bitmap(image);
                    icon = iconImage.ToIcon();
                    iconImage.Dispose();
                    Notification = Res.NotificationImageAsIcon;
                }
            }

            SetImage(image, icon);
        }

        private void ResetCompoundState()
        {
            bool isCompound = IsCompoundView;

            // handle as separated images
            if (!isCompound || mainImage.RawFormat == ImageFormat.Tiff.Guid)
            {
                currentFrame = 0;
                IsAutoPlaying = false;
                NextImageCommandState.Enabled = true;
                PrevImageCommandState.Enabled = false;
                PreviewImage = frames[0].Image;
                ImageChanged();
                return;
            }

            // handle as compound image
            NextImageCommandState.Enabled = PrevImageCommandState.Enabled = false;
            bool autoPlaying = frames[0].Duration != 0 && mainImage.Image == null;
            ICommandState timerState = AdvanceAnimationCommandState;
            IsAutoPlaying = autoPlaying;
            if (autoPlaying)
            {
                currentFrame = 0;
                PreviewImage = frames[0].Image;
                timerState[stateInterval] = frames[0].Duration;
            }
            else
            {
                currentFrame = -1;
                PreviewImage = mainImage.Image ?? frames[0].Image;
                if (mainImage != null && mainImage.RawFormat == ImageFormat.Icon.Guid)
                    UpdateIconImage();
            }

            timerState.Enabled = IsAutoPlaying;
            ImageChanged();
        }

        private void OnViewImagePreviewSizeChanged()
        {
            var image = mainImage;
            if (image == null || image.RawFormat != ImageFormat.Icon.Guid || currentFrame != -1 || frames == null)
                return;
            UpdateIconImage();
            UpdateInfo();
        }

        private void UpdateIconImage()
        {
            Size origSize = currentIconSize;
            Size clientSize = ViewImagePreviewSize;
            int desiredSize = Math.Min(clientSize.Width, clientSize.Height);
            if (desiredSize < 1 && !origSize.IsEmpty)
                return;

            // Starting with Windows Vista it would work that we draw the compound image in a new Bitmap with desired size and read the Size afterwards
            // but that requires always a new bitmap and does not work in Windows XP
            desiredSize = Math.Max(desiredSize, 1);
            ImageData desiredImage = frames.Aggregate((acc, i) => i.Size == acc.Size && i.BitsPerPixel > acc.BitsPerPixel || Math.Abs(i.Size.Width - desiredSize) < Math.Abs(acc.Size.Width - desiredSize) ? i : acc);
            currentIconSize = desiredImage.Size;
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
            ImageData currentImage = GetCurrentImage();

            // saving as composite icon
            if (currentImage == mainImage)
            {
                // when used as debugger, icon is always created from stream so it has raw data and Save can be used safely.
                // But when icon is set via Icon property it can be an unmanaged icon
                if (underlyingIcon != null && !IsModified)
                    underlyingIcon.SaveHighQuality(stream);
                // single image icon without raw data
                else if (frames == null || frames.Length <= 1)
                {
                    using (Icon i = Icons.Combine((Bitmap)mainImage.Image))
                        i.Save(stream);
                }
                // multi image icon without raw data
                else
                {
                    using (Icon i = Icons.Combine(frames.Select(f => (Bitmap)f.Image).ToArray()))
                        i.Save(stream);
                }

                stream.Flush();
                return;
            }

            // saving a single icon image
            if (underlyingIcon != null)
            {
                using (Icon i = underlyingIcon.ExtractIcon(currentFrame))
                    i.Save(stream);
            }
            else
            {
                using (Icon i = Icons.Combine((Bitmap)currentImage.Image))
                    i.Save(stream);
            }

            stream.Flush();
        }

        private void SaveMetafile(string fileName, bool asWmf)
        {
            using (Stream stream = File.Create(fileName))
            {
                ((Metafile)mainImage.Image).Save(stream, asWmf);
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
            ImageData currentImage = GetCurrentImage();
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
                // TODO: Allow dithering on SaveAs dialog (which replaces palette), or implement quantizing and dithering in this tool
                currentImage.Image.SaveAsGif(stream, false);
            }
        }

        private void SaveAnimGif(Stream stream) => ShowError(Res.ErrorMessageAnimGifNotSupported);

        private void SaveMultipageTiff(string fileName)
        {
            using (Stream stream = File.Create(fileName))
                GetTiffPages().SaveAsMultipageTiff(stream);
        }

        private IEnumerable<Image> GetTiffPages()
        {
            // first image will be disposed by destructor... disposing it at the end of the method would not work (because of adding Flush), and iterator method cannot have out parameter...
            bool first = true;
            foreach (ImageData frame in frames)
            {
                Image page = frame.Image;
                bool convertFormat = frame.PixelFormat != frame.Image.PixelFormat && frame.RawFormat == ImageFormat.Tiff.Guid;
                if (convertFormat)
                {
                    Color[] palette = null;
                    int bpp = frame.BitsPerPixel;
                    if (bpp <= 8 && page is Bitmap bmpPage)
                        palette = bmpPage.GetColors(1 << bpp);

                    page = frame.Image.ConvertPixelFormat(frame.PixelFormat, palette);
                }

                yield return page;

                if (convertFormat && !first)
                    page.Dispose();

                first = false;
            }
        }

        private void SetOpenFilter()
        {
            if (isOpenFilterUpToDate || imageTypes == ImageTypes.None)
                return;

            StringBuilder sb = new StringBuilder();
            StringBuilder sbImages = new StringBuilder();
            foreach (ImageCodecInfo codecInfo in decoderCodecs)
            {
                if (imageTypes == ImageTypes.Metafile && codecInfo.FormatID != ImageFormat.Wmf.Guid && codecInfo.FormatID != ImageFormat.Emf.Guid)
                    continue;

                if (sb.Length != 0)
                    sb.Append('|');
                sb.Append($"{codecInfo.FormatDescription} {Res.TextFiles}|{codecInfo.FilenameExtension.ToLowerInvariant()}");
                if (sbImages.Length != 0)
                    sbImages.Append(';');
                sbImages.Append(codecInfo.FilenameExtension);
            }

            OpenFileFilter = $"{(imageTypes == ImageTypes.Metafile ? Res.TextMetafiles : Res.TextImages)} ({sbImages})|{sbImages}|{sb}|{Res.TextAllFiles} (*.*)|*.*";
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

            if (underlyingIcon != null || mainImage.RawFormat == ImageFormat.Icon.Guid)
                sb.Append($"|{Res.TextIcon} {Res.TextFileFormat}|*.ico");
            else if (mainImage.Image is Metafile)
            {
                sb.Append($"|WMF {Res.TextFileFormat}|*.wmf");
                if (mainImage.RawFormat == ImageFormat.Emf.Guid)
                    sb.Append($"|EMF {Res.TextFileFormat}|*.emf");
            }

            SaveFileFilter = sb.ToString();

            // selecting appropriate format
            if (underlyingIcon != null || mainImage.RawFormat == ImageFormat.Icon.Guid)
            {
                SaveFileFilterIndex = encoderCodecs.Length + 1;
                SaveFileDefaultExtension = "ico";
            }
            else if (mainImage.Image is Metafile)
            {
                SaveFileFilterIndex = encoderCodecs.Length
                                      + (mainImage.RawFormat == ImageFormat.Wmf.Guid ? 1 : 2);
                SaveFileDefaultExtension = mainImage.RawFormat == ImageFormat.Wmf.Guid ? "wmf" : "emf";
            }
            else
            {
                int posPng = 0;
                int index = 0;
                bool found = false;
                for (int i = 0; i < encoderCodecs.Length; i++)
                {
                    if (mainImage.RawFormat == encoderCodecs[i].FormatID)
                    {
                        index = i + 1;
                        found = true;
                        break;
                    }

                    if (encoderCodecs[i].FormatDescription == "PNG")
                        posPng = i + 1;
                }

                // if encoder not found, selecting png encoder
                SaveFileFilterIndex = found ? index : posPng;

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
            if (mainImage?.Image == null)
            {
                SetAutoZoomCommandState.Enabled = AutoZoom = false;
                return;
            }

            if (mainImage.Image is Metafile)
            {
                SetAutoZoomCommandState.Enabled = AutoZoom = true;
                return;
            }

            SetAutoZoomCommandState.Enabled = true;
            Rectangle workingArea = GetScreenRectangleCallback?.Invoke() ?? default;
            Size screenSize = workingArea.Size;
            Size viewSize = GetViewSizeCallback?.Invoke() ?? default;
            Size padding = viewSize - ViewImagePreviewSize;
            Size desiredSize = mainImage.Size + padding;
            if (desiredSize.Width <= screenSize.Width && desiredSize.Height <= screenSize.Height)
            {
                AutoZoom = false;
                ApplyViewSizeCallback?.Invoke(new Size(Math.Max(desiredSize.Width, viewSize.Width), Math.Max(desiredSize.Height, viewSize.Height)));
            }
            else
                AutoZoom = true;
        }

        private ImageReference GetImageReference()
        {
            Debug.Assert(IsModified, "Image reference is requested when image has not been changed");
            string fileName = FileName;

            // the image has been edited and should be saved
            if (IsModified && fileName == null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    if (underlyingIcon != null)
                    {
                        SaveIcon(ms);
                        return new ImageReference(imageTypes == ImageTypes.Icon ? imageTypes : ImageTypes.Bitmap, ms.ToArray());
                    }

                    Debug.Assert(!(mainImage.Image is Metafile), "A metafile is not expected to be changed");
                    if (frames != null && frames.Length > 1)
                    {
                        if (mainImage.RawFormat == ImageFormat.Tiff.Guid)
                            GetTiffPages().SaveAsMultipageTiff(ms);
                        else
                            SaveAnimGif(ms);
                    }
                    else
                        mainImage.Image.Save(ms, ImageFormat.Png);

                    return new ImageReference(ImageTypes.Bitmap, ms.ToArray());
                }
            }

            // passing image by filename (even when image has been cleared)
            ImageTypes imageType;
            if (underlyingIcon != null && imageTypes == ImageTypes.Icon)
                imageType = ImageTypes.Icon;
            else if (mainImage?.Image == null)
                imageType = ImageTypes.None;
            else if (mainImage.Image is Metafile)
                imageType = ImageTypes.Metafile;
            else
                imageType = ImageTypes.Bitmap;
            return new ImageReference(imageType, FileName);
        }

        #endregion

        #region Explicit Interface Implementations

        ImageReference IViewModel<ImageReference>.GetEditedModel() => GetImageReference();
        Image IViewModel<Image>.GetEditedModel() => Image;
        Icon IViewModel<Icon>.GetEditedModel() => Icon;

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
            if (mainImage == null)
                return;

            SetSaveFilter();
            string fileName = SelectFileToSaveCallback?.Invoke();
            if (fileName == null)
                return;

            int filterIndex = SaveFileFilterIndex;
            try
            {
                // icon
                if (filterIndex == encoderCodecs.Length + 1 && (underlyingIcon != null || mainImage.RawFormat == ImageFormat.Icon.Guid))
                    SaveIcon(fileName);
                // metafile
                else if (filterIndex >= encoderCodecs.Length + 1 && mainImage.Image is Metafile)
                    SaveMetafile(fileName, filterIndex == encoderCodecs.Length + 1);
                // JPG
                else if (encoderCodecs[filterIndex - 1].FormatID == ImageFormat.Jpeg.Guid)
                    SaveJpeg(fileName, encoderCodecs[filterIndex - 1]);
                // Multipage tiff
                else if (frames != null && frames.Length > 1 && encoderCodecs[filterIndex - 1].FormatID == ImageFormat.Tiff.Guid
                    && (mainImage.RawFormat == ImageFormat.Tiff.Guid && IsCompoundView))
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
            currentFrame++;
            if (currentFrame >= frames.Length)
                currentFrame = 0;
            AdvanceAnimationCommandState[stateInterval] = frames[currentFrame].Duration;
            PreviewImage = frames[currentFrame].Image;
        }

        private void OnPrevImageCommand()
        {
            if (frames == null || frames.Length <= 1 || currentFrame <= 0)
                return;

            PreviewImage = frames[--currentFrame].Image;
            PrevImageCommandState.Enabled = currentFrame > 0;
            NextImageCommandState.Enabled = true;
            ImageChanged();
        }

        private void OnNextImageCommand()
        {
            if (frames == null || frames.Length <= 1 || currentFrame >= frames.Length)
                return;

            PreviewImage = frames[++currentFrame].Image;
            PrevImageCommandState.Enabled = true;
            NextImageCommandState.Enabled = currentFrame < frames.Length - 1;
            ImageChanged();
        }

        private void OnShowPaletteCommand()
        {
            ImageData currentImage = GetCurrentImage();
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
                    Image newImage = currentImage.Image.ConvertPixelFormat(currentImage.PixelFormat, newPalette);
                    currentImage.Image.Dispose();
                    PreviewImage = currentImage.Image = newImage;
                    palette = newImage.Palette;
                }
                else
                {
                    for (int i = 0; i < newPalette.Length; i++)
                        palette.Entries[i] = newPalette[i];
                }

                currentImage.Image.Palette = palette; // the preview changes only if we apply the palette
                currentImage.Palette = palette.Entries; // the actual palette will be taken from here
                FileName = null;
                SetModified(true);
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
