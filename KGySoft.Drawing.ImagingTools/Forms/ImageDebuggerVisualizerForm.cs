#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageDebuggerVisualizerForm.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2019 - All Rights Reserved
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using KGySoft.CoreLibraries;

#endregion

#region Used Aliases

using Encoder = System.Drawing.Imaging.Encoder;
using Images = KGySoft.Drawing.ImagingTools.Properties.Resources;

#endregion

#endregion

namespace KGySoft.Drawing.ImagingTools.Forms
{
    internal partial class ImageDebuggerVisualizerForm : BaseForm
    {
        #region ButtonRenderer class

        private sealed class ButtonRenderer : ToolStripProfessionalRenderer
        {
            #region Methods

            protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
            {
                ToolStripButton button = e.Item as ToolStripButton;
                if (button != null && button.Checked && button.Enabled)
                    e.Graphics.Clear(ProfessionalColors.ButtonSelectedGradientMiddle);

                base.OnRenderButtonBackground(e);
            }

            #endregion
        }

        #endregion

        #region Fields

        #region Static Fields

        private static readonly ImageCodecInfo[] encoderCodecs = ImageCodecInfo.GetImageEncoders();
        private static readonly ImageCodecInfo[] decoderCodecs = ImageCodecInfo.GetImageDecoders();

        #endregion

        #region Instance Fields

        private ImageData image;
        private Icon icon;
        private int currentFrame;
        private ImageData[] frames;
        private bool isManualPlaying;
        private string fileName;
        private bool readOnly;
        private ImageTypes imageTypes = ImageTypes.All;
        private bool isOpenFilterUpToDate;
        private bool isUpToDate;
        private bool autoZoom;
        private Size currentIconSize;

        #endregion

        #endregion

        #region Properties

        #region Internal Properties

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        internal virtual Image Image
        {
            get => image?.Image;
            set
            {
                if (image != null && image.Image == value)
                    return;
                SetImage(value, null);
            }
        }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        internal new Icon Icon
        {
            get => icon;
            set
            {
                if (icon == value)
                    return;
                SetImage(null, value);
            }
        }

        /// <summary>
        /// Gets or sets the supported image types (affects the types in open dialog)
        /// </summary>
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

        /// <summary>
        /// Gets whether the image has been replaced or edited.
        /// </summary>
        internal bool IsModified => !isUpToDate || fileName != null;

        internal bool ReadOnly
        {
            get => readOnly;
            set
            {
                if (readOnly == value)
                    return;
                readOnly = value;
                btnClear.Visible = btnOpen.Visible = !readOnly;
            }
        }

        #endregion

        #region Protected Properties

        protected string Notification
        {
            set => lblNotification.Text = value;
        }

        protected virtual bool IsPaletteReadOnly => image.RawFormat == ImageFormat.Icon.Guid;

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageDebuggerVisualizerForm"/> class.
        /// </summary>
        public ImageDebuggerVisualizerForm()
        {
            InitializeComponent();
            base.Icon = Images.ImagingTools;
            btnAutoZoom.Image = Images.Magnifier;
            btnSave.Image = Images.Save;
            btnOpen.Image = Images.Browse;
            btnClear.Image = Images.Clear;
            btnCompound.Image = Images.Merge;
            btnPrev.Image = Images.Prev;
            btnNext.Image = Images.Next;
            btnColorSettings.Image = Images.Palette;
            btnConfiguration.Image = Images.Gear;
            tsMenu.Renderer = new ButtonRenderer();
            lblNotification.Text = null;

            SetImage(null, null);
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
                return "MemoryBMP";
            if (imageFormat.Equals(ImageFormat.Bmp.Guid))
                return "Bmp";
            if (imageFormat.Equals(ImageFormat.Emf.Guid))
                return "Emf";
            if (imageFormat.Equals(ImageFormat.Wmf.Guid))
                return "Wmf";
            if (imageFormat.Equals(ImageFormat.Gif.Guid))
                return "Gif";
            if (imageFormat.Equals(ImageFormat.Jpeg.Guid))
                return "Jpeg";
            if (imageFormat.Equals(ImageFormat.Png.Guid))
                return "Png";
            if (imageFormat.Equals(ImageFormat.Tiff.Guid))
                return "Tiff";
            if (imageFormat.Equals(ImageFormat.Exif.Guid))
                return "Exif";
            if (imageFormat.Equals(ImageFormat.Icon.Guid))
                return "Icon";
            return "Unknown format: " + imageFormat;
        }

        #endregion

        #region Instance Methods

        #region Internal Methods

        internal void InitFromSingleImage(ImageData imageData, Icon newIcon)
        {
            image?.Dispose();
            icon?.Dispose();
            icon = newIcon;
            btnSave.Enabled = btnClear.Enabled = imageData.Image != null;
            FreeFrames();
            image = imageData;
            isUpToDate = true;
            InitAutoZoom();
            InitSingleImage();
        }

        internal void InitFromFrames(ImageData mainImage, ImageData[] frameImages, Icon newIcon)
        {
            image?.Dispose();
            icon?.Dispose();
            icon = newIcon;
            btnSave.Enabled = btnClear.Enabled = true;
            FreeFrames();

            if (mainImage == null)
                mainImage = frameImages[0];

            image = mainImage;
            frames = frameImages;
            isUpToDate = true;
            InitAutoZoom();

            if (frameImages == null || frameImages.Length <= 1)
                InitSingleImage();
            else
                InitMultiImage();
        }

        internal ImageReference GetImageReference()
        {
            Debug.Assert(IsModified, "Image reference is requested when image has not been changed");

            // the image has been edited and should be saved
            if (!isUpToDate)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    if (icon != null)
                    {
                        SaveIcon(ms);
                        return new ImageReference(imageTypes == ImageTypes.Icon ? imageTypes : ImageTypes.Bitmap, ms.ToArray());
                    }

                    Debug.Assert(!(image.Image is Metafile), "A metafile is not expected to be changed");
                    if (frames != null && frames.Length > 1)
                    {
                        if (image.RawFormat == ImageFormat.Tiff.Guid)
                            GetTiffPages().SaveAsMultipageTiff(ms);
                        else
                            SaveAnimGif(ms);
                    }
                    else
                        image.Image.Save(ms, ImageFormat.Png);

                    return new ImageReference(ImageTypes.Bitmap, ms.ToArray());
                }
            }

            // passing image by filename (even when image has been cleared)
            ImageTypes imageType;
            if (icon != null && imageTypes == ImageTypes.Icon)
                imageType = ImageTypes.Icon;
            else if (image?.Image == null)
                imageType = ImageTypes.None;
            else if (image.Image is Metafile)
                imageType = ImageTypes.Metafile;
            else
                imageType = ImageTypes.Bitmap;
            return new ImageReference(imageType, fileName);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            Load -= ImageDebuggerVisualizerForm_Load;
            Resize -= ImageDebuggerVisualizerForm_Resize;
            btnAutoZoom.CheckedChanged -= btnAutoZoom_CheckedChanged;
            miDeafult.Click -= miBackColor_Click;
            miWhite.Click -= miBackColor_Click;
            miBlack.Click -= miBackColor_Click;
            btnSave.Click -= btnSave_Click;
            btnOpen.Click -= btnOpen_Click;
            btnClear.Click -= btnClear_Click;
            btnCompound.Click -= btnCompound_Click;
            btnPrev.Click -= btnPrev_Click;
            btnNext.Click -= btnNext_Click;
            pbImage.SizeChanged -= pbImage_SizeChanged;
            timerPlayer.Tick -= timerPlayer_Tick;
            miShowPalette.Click -= miShowPalette_Click;
            txtInfo.TextChanged -= txtInfo_TextChanged;
            txtInfo.Enter -= txtInfo_Enter;
            btnConfiguration.Click -= btnConfiguration_Click;

            if (disposing)
                components?.Dispose();

            if (disposing)
            {
                // freeing images (this form works always with copies, and gives back copies do
                // disposing images is alright)
                image?.Dispose();
                icon?.Dispose();
                if (frames != null)
                    FreeFrames();
            }

            base.Dispose(disposing);
        }

        protected virtual void UpdateInfo()
        {
            if ((image == null || image.Image == null) && frames == null)
            {
                Text = "No Image";
                txtInfo.Clear();
                return;
            }

            ImageData img = GetCurrentImage();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Type: {0}", GetTypeName());
            if (image.Image is Bitmap)
                sb.AppendFormat("; Size: {0}", GetSize());
            sb.Append(GetFrameInfo(true));

            Text = sb.ToString();
            sb.Length = 0;
            sb.AppendFormat("Type: {1}{0}Size: {2}{0}{7}Pixel Format: {3}{0}Raw format: {4}{0}Resolution: {5} x {6} dpi",
                Environment.NewLine,
                GetTypeName(),
                GetSize(),
                Enum<PixelFormat>.ToString(img.PixelFormat),
                RawFormatToString(img.RawFormat),
                img.HorizontalRes, img.VerticalRes,
                GetFrameInfo(false));

            if (img.Image is Bitmap)
            {
                sb.AppendLine();
                sb.AppendFormat("Palette count: {0}", img.Palette.Length);
            }

            txtInfo.Text = sb.ToString();
        }

        protected ImageData GetCurrentImage()
        {
            if (frames == null || frames.Length <= 1 || currentFrame < 0 || isManualPlaying)
                return image;
            return frames[currentFrame];
        }

        protected virtual void ImageChanged()
        {
            miShowPalette.Enabled = GetCurrentImage().Palette.Length > 0;
            UpdateInfo();
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
            btnCompound.Visible = true;
            if (image.RawFormat == ImageFormat.Gif.Guid)
                btnCompound.ToolTipText = @"Toggles whether image is handled as a single image.
• When checked, animation will play and saving as GIF saves the whole animation.
• When not checked, frame navigation will be enabled and saving saves only the selected frame.";
            else if (image.RawFormat == ImageFormat.Icon.Guid)
                btnCompound.ToolTipText = @"Toggles whether icon is handled as a single image.
• When checked, auto sizing displays always the best fitting icon and saving as Icon saves every image.
• When not checked, icon images can be explored by navigation and saving saves the selected image only.";
            else
                btnCompound.ToolTipText = @"Toggles whether image is handled as a compound image.
• When checked, saving as TIFF saves every page.
• When not checked, saving saves always the selected page.";

            sepFrames.Visible = btnPrev.Visible = btnNext.Visible = true;
            ResetCompoundState();
        }

        private void InitSingleImage()
        {
            currentFrame = -1;
            sepFrames.Visible = btnPrev.Visible = btnNext.Visible = btnCompound.Visible = false;
            pbImage.Image = image.Image;
            ImageChanged();
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

            if (currentFrame != -1 && !isManualPlaying)
                result.AppendFormat("Current Image: {0}/{1}", currentFrame + 1, frames.Length);
            else
                result.AppendFormat("Images: {0}", frames.Length);

            if (!singleLine)
                result.AppendLine();
            return result.ToString();
        }

        private string GetSize()
        {
            if (image.RawFormat == ImageFormat.Icon.Guid && frames != null && frames.Length > 1 && currentFrame == -1)
                return currentIconSize.ToString();
            return GetCurrentImage().Size.ToString();
        }

        private string GetTypeName()
        {
            if (icon != null)//(image.Image.RawFormat.Guid == ImageFormat.Icon.Guid)
                return "Icon";
            Image img = GetCurrentImage().Image;
            return img?.GetType().Name ?? typeof(Bitmap).Name;
        }

        private void FromStream(Stream stream, bool appearsIcon)
        {
            Icon icon = null;

            // only icon is allowed or the content seems to be an icon
            // (this block is needed only for Windows XP: Icon Bitmap with PNG throws an exception but initializing from icon will succeed)
            if (appearsIcon || imageTypes == ImageTypes.Icon)
            {
                try
                {
                    icon = Icons.FromStream(stream);
                    SetImage(null, icon);
                    return;
                }
                catch (Exception)
                {
                    stream.Position = 0L;
                }
            }

            Image image = null;

            // bitmaps and metafiles are both allowed
            if ((imageTypes & (ImageTypes.Bitmap | ImageTypes.Metafile)) == (ImageTypes.Bitmap | ImageTypes.Metafile))
                image = Image.FromStream(stream);
            // metafiles only
            else if (imageTypes == ImageTypes.Metafile)
                image = new Metafile(stream);
            // bitmaps or icons
            else if ((imageTypes & (ImageTypes.Bitmap | ImageTypes.Icon)) != ImageTypes.None)
            {
                image = new Bitmap(stream);
                if (image.RawFormat.Guid == ImageFormat.MemoryBmp.Guid)
                    Notification = "The loaded metafile has been converted to Bitmap. To load it as a Metafile, choose the Image Debugger Visualizer instead.";
            }

            // icon is allowed and an image has been loaded
            if (image != null && (imageTypes & ImageTypes.Icon) != ImageTypes.None)
            {
                // the loaded format is icon: loading as icon
                if (image.RawFormat.Guid == ImageFormat.Icon.Guid)
                {
                    stream.Position = 0L;
                    icon = new Icon(stream);
                }

                // not icon was loaded, though icon is the only supported format: converting to icon
                else if (imageTypes == ImageTypes.Icon)
                {
                    string warning = "The loaded image has been converted to Icon";
                    Bitmap iconImage = image as Bitmap;
                    if (iconImage == null)
                        iconImage = new Bitmap(image, 256, 256);
                    else if (image.Width > 256 || image.Height > 256)
                    {
                        Bitmap newIconImage = iconImage.Resize(new Size(256, 256), true);
                        iconImage.Dispose();
                        iconImage = newIconImage;
                        warning += " and has been resized";
                    }

                    icon = Icons.Combine(iconImage);
                    iconImage.Dispose();
                    Notification = warning;
                }
            }

            SetImage(image, icon);
        }

        private void ResetCompoundState()
        {
            // handle as separated images
            if (!btnCompound.Checked || image.RawFormat == ImageFormat.Tiff.Guid)
            {
                currentFrame = 0;
                timerPlayer.Enabled = isManualPlaying = false;
                btnNext.Enabled = true;
                btnPrev.Enabled = false;
                pbImage.Image = frames[0].Image;
                ImageChanged();
                return;
            }

            // handle as compound image
            btnNext.Enabled = btnPrev.Enabled = false;
            isManualPlaying = frames[0].Duration != 0 && image.Image == null;
            if (!isManualPlaying)
            {
                currentFrame = -1;
                pbImage.Image = image.Image ?? frames[0].Image;
                if (image != null && image.RawFormat == ImageFormat.Icon.Guid)
                    UpdateIconImage();
            }
            else
            {
                currentFrame = 0;
                pbImage.Image = frames[0].Image;
                timerPlayer.Interval = frames[0].Duration;
            }

            timerPlayer.Enabled = isManualPlaying;
            ImageChanged();
        }

        private void UpdateIconImage()
        {
            Size origSize = currentIconSize;
            int desiredSize = Math.Min(pbImage.ClientSize.Width, pbImage.ClientSize.Height);
            if (desiredSize < 1 && !origSize.IsEmpty)
                return;

            // Starting with Windows Vista it would work that we draw the compound image in a new Bitmap with desired size and read the Size afterwards
            // but that requires always a new bitmap and does not work in Windows XP
            desiredSize = Math.Max(desiredSize, 1);
            ImageData desiredImage = frames.Aggregate((acc, i) => i.Size == acc.Size && i.Image.GetBitsPerPixel() > acc.Image.GetBitsPerPixel() || Math.Abs(i.Size.Width - desiredSize) < Math.Abs(acc.Size.Width - desiredSize) ? i : acc);
            currentIconSize = desiredImage.Size;
            if (pbImage.Image != desiredImage.Image)
                pbImage.Image = desiredImage.Image;
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
            if (currentImage == image)
            {
                // when used as debugger, icon is always created from stream so it has raw data and Save can be used safely.
                // But when icon is set via Icon property it can be unmanaged icon
                if (icon != null && isUpToDate)
                    icon.SaveHighQuality(stream);
                // single image icon without raw data
                else if (frames == null || frames.Length <= 1)
                {
                    using (Icon i = Icons.Combine((Bitmap)image.Image))
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
            if (icon != null)
            {
                using (Icon i = icon.ExtractIcon(currentFrame))
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
                ((Metafile)image.Image).Save(stream, asWmf);
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
            int theoreticBpp = Image.GetPixelFormatSize(currentImage.PixelFormat);

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

        private void SaveAnimGif(Stream stream)
        {
            Dialogs.ErrorMessage("Saving animated GIF has not been implemented yet");
            return;
            throw new NotImplementedException("animgif");
        }

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
                    int bpp = Image.GetPixelFormatSize(frame.PixelFormat);
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
                sb.AppendFormat("{0} files|{1}", codecInfo.FormatDescription, codecInfo.FilenameExtension.ToLowerInvariant());
                if (sbImages.Length != 0)
                    sbImages.Append(';');
                sbImages.Append(codecInfo.FilenameExtension);
            }

            dlgOpen.Filter = String.Format("{0} ({1})|{1}|{2}|All files (*.*)|*.*", imageTypes == ImageTypes.Metafile ? "Metafiles" : "Images", sbImages, sb);
            isOpenFilterUpToDate = true;
        }

        private void InitAutoZoom()
        {
            if (image == null || image.Image == null || image.Image is Metafile)
            {
                btnAutoZoom.Visible = false;
                btnAutoZoom.Checked = true;
                return;
            }

            btnAutoZoom.Visible = true;
            Rectangle workingArea = Screen.FromHandle(Handle).WorkingArea;
            Size screenSize = workingArea.Size;
            Size padding = Size - pbImage.ClientSize;
            Size desiredSize = image.Size + padding;
            if (desiredSize.Width <= screenSize.Width && desiredSize.Height <= screenSize.Height)
            {
                btnAutoZoom.Checked = false;
                Size = new Size(Math.Max(desiredSize.Width, Width), Math.Max(desiredSize.Height, Height));
                if (Top < workingArea.Top)
                    Top = workingArea.Top;
                if (Left < workingArea.Left)
                    Left = workingArea.Left;
                if (Bottom > workingArea.Bottom)
                    Top = workingArea.Bottom - Height;
                if (Right > workingArea.Right)
                    Left = workingArea.Right - Width;
            }
            else
                btnAutoZoom.Checked = true;
        }

        private void Open()
        {
            SetOpenFilter();
            if (dlgOpen.ShowDialog(this) != DialogResult.OK)
                return;

            try
            {
                Notification = null;
                var ms = new MemoryStream(File.ReadAllBytes(dlgOpen.FileName));
                FromStream(ms, Path.GetExtension(dlgOpen.FileName).Equals(".ico", StringComparison.OrdinalIgnoreCase));
                isUpToDate = !lblNotification.Visible;
                fileName = isUpToDate ? dlgOpen.FileName : null;
            }
            catch (Exception ex)
            {
                Dialogs.ErrorMessage("Could not load file due to an error: {0}", ex.Message);
            }
        }

        private void Save()
        {
            if (image == null)
                return;

            // enlisting encoders
            StringBuilder sb = new StringBuilder();
            foreach (ImageCodecInfo codecInfo in encoderCodecs)
            {
                if (sb.Length != 0)
                    sb.Append("|");
                sb.AppendFormat("{0} format|{1}", codecInfo.FormatDescription, codecInfo.FilenameExtension.ToLowerInvariant());
            }

            if (icon != null || image.RawFormat == ImageFormat.Icon.Guid)
                sb.Append("|Icon format|*.ico");
            else if (image.Image is Metafile)
            {
                sb.Append("|Windows Metafile format|*.wmf");
                if (image.RawFormat == ImageFormat.Emf.Guid)
                    sb.Append("|Enhanced Metafile format|*.emf");
            }

            dlgSave.Filter = sb.ToString();

            // selecting appropriate format
            if (icon != null || image.RawFormat == ImageFormat.Icon.Guid)
            {
                dlgSave.FilterIndex = encoderCodecs.Length + 1;
                dlgSave.DefaultExt = "ico";
            }
            else if (image.Image is Metafile)
            {
                dlgSave.FilterIndex = encoderCodecs.Length
                    + (image.RawFormat == ImageFormat.Wmf.Guid ? 1 : 2);
                dlgSave.DefaultExt = image.RawFormat == ImageFormat.Wmf.Guid ? "wmf" : "emf";
            }
            else
            {
                int posPng = 0;
                bool found = false;
                for (int i = 0; i < encoderCodecs.Length; i++)
                {
                    if (image.RawFormat == encoderCodecs[i].FormatID)
                    {
                        dlgSave.FilterIndex = i + 1;
                        found = true;
                        break;
                    }

                    if (encoderCodecs[i].FormatDescription == "PNG")
                        posPng = i + 1;
                }

                // if encoder not found, selecting png encoder
                if (!found)
                    dlgSave.FilterIndex = posPng;

                // setting default extension
                string ext = encoderCodecs[dlgSave.FilterIndex - 1].FilenameExtension;
                int sep = ext.IndexOf(';');
                if (sep > 0)
                    ext = ext.Substring(0, sep);

                dlgSave.DefaultExt = ext.Substring(ext.IndexOf('.') + 1).ToLowerInvariant();
            }

            // showing the dialog
            if (dlgSave.ShowDialog(this) != DialogResult.OK)
                return;

            try
            {
                // icon
                if (dlgSave.FilterIndex == encoderCodecs.Length + 1 && (icon != null || image.RawFormat == ImageFormat.Icon.Guid))
                    SaveIcon(dlgSave.FileName);
                // metafile
                else if (dlgSave.FilterIndex >= encoderCodecs.Length + 1 && image.Image is Metafile)
                    SaveMetafile(dlgSave.FileName, dlgSave.FilterIndex == encoderCodecs.Length + 1);
                // JPG
                else if (encoderCodecs[dlgSave.FilterIndex - 1].FormatID == ImageFormat.Jpeg.Guid)
                    SaveJpeg(dlgSave.FileName, encoderCodecs[dlgSave.FilterIndex - 1]);
                // Multipage tiff
                else if (frames != null && frames.Length > 1 && encoderCodecs[dlgSave.FilterIndex - 1].FormatID == ImageFormat.Tiff.Guid
                    && (image.RawFormat == ImageFormat.Tiff.Guid && btnCompound.Checked))
                {
                    SaveMultipageTiff(dlgSave.FileName);
                }
                // gif
                else if (encoderCodecs[dlgSave.FilterIndex - 1].FormatID == ImageFormat.Gif.Guid)
                    SaveGif(dlgSave.FileName);
                else
                    GetCurrentImage().Image.Save(dlgSave.FileName, encoderCodecs[dlgSave.FilterIndex - 1], null);
            }
            catch (Exception ex)
            {
                Dialogs.ErrorMessage("Could not save image due to an error: {0}", ex.Message);
            }
        }

        private void AdjustSize()
        {
            if (pbImage.Height < 16)
            {
                txtInfo.Height = ClientSize.Height - tsMenu.Height - 16 - splitter.Height;
                PerformLayout();
            }
        }

        #endregion

        #region Event handlers
        //ReSharper disable InconsistentNaming

        private void btnOpen_Click(object sender, EventArgs e) => Open();
        private void btnSave_Click(object sender, EventArgs e) => Save();
        private void btnAutoZoom_CheckedChanged(object sender, EventArgs e)
        {
            autoZoom = btnAutoZoom.Checked;
            pbImage.SizeMode = autoZoom ? PictureBoxSizeMode.Zoom : PictureBoxSizeMode.CenterImage;
        }

        private void miBackColor_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in miBackColor.DropDownItems)
                item.Checked = item == sender;

            if (sender == miDeafult)
                pbImage.BackColor = SystemColors.Control;
            else if (sender == miWhite)
                pbImage.BackColor = Color.White;
            else
                pbImage.BackColor = Color.Black;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Image = null;
            fileName = String.Empty; // empty string indicates that image has been cleared
        }

        private void btnCompound_Click(object sender, EventArgs e)
        {
            if (frames == null || frames.Length <= 1 || image.RawFormat == ImageFormat.Tiff.Guid)
                return;

            ResetCompoundState();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (frames == null || frames.Length <= 1 || currentFrame <= 0)
                return;

            pbImage.Image = frames[--currentFrame].Image;
            btnPrev.Enabled = currentFrame > 0;
            btnNext.Enabled = true;
            ImageChanged();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (frames == null || frames.Length <= 1 || currentFrame >= frames.Length)
                return;

            pbImage.Image = frames[++currentFrame].Image;
            btnPrev.Enabled = true;
            btnNext.Enabled = currentFrame < frames.Length - 1;
            ImageChanged();
        }

        private void pbImage_SizeChanged(object sender, EventArgs e)
        {
            AdjustSize();
            if (image != null && image.RawFormat == ImageFormat.Icon.Guid && currentFrame == -1 && frames != null)
            {
                UpdateIconImage();
                UpdateInfo();
            }
        }

        private void timerPlayer_Tick(object sender, EventArgs e)
        {
            if (!isManualPlaying)
            {
                timerPlayer.Enabled = false;
                return;
            }

            // playing with duration
            currentFrame++;
            if (currentFrame >= frames.Length)
                currentFrame = 0;
            timerPlayer.Interval = frames[currentFrame].Duration;
            pbImage.Image = frames[currentFrame].Image;
        }

        private void miShowPalette_Click(object sender, EventArgs e)
        {
            ImageData currentImage = GetCurrentImage();
            if (currentImage == null || currentImage.Palette.Length == 0)
                return;

            using (PaletteVisualizerForm frm = new PaletteVisualizerForm())
            {
                bool isReadOnly = IsPaletteReadOnly;
                IList<Color> colors = isReadOnly ? (IList<Color>)Array.AsReadOnly(currentImage.Palette) : currentImage.Palette;
                frm.Palette = colors;
                frm.ShowDialog(this);

                // apply changes
                if (frm.PaletteChanged)
                {
                    ColorPalette palette = currentImage.Image.Palette;
                    if (palette.Entries.Length != currentImage.Palette.Length)
                    {
                        Image newImage = currentImage.Image.ConvertPixelFormat(currentImage.PixelFormat, currentImage.Palette);
                        currentImage.Image.Dispose();
                        pbImage.Image = currentImage.Image = newImage;
                        palette = newImage.Palette;
                    }

                    for (int i = 0; i < currentImage.Palette.Length; i++)
                        palette.Entries[i] = currentImage.Palette[i];

                    currentImage.Image.Palette = palette;
                    isUpToDate = false;
                    fileName = null;
                    pbImage.Invalidate();
                }
            }
        }

        private void txtInfo_TextChanged(object sender, EventArgs e) => txtInfo.SelectionLength = 0;
        private void ImageDebuggerVisualizerForm_Load(object sender, EventArgs e) => InitAutoZoom();
        private void txtInfo_Enter(object sender, EventArgs e) => txtInfo.SelectionLength = 0;
        private void ImageDebuggerVisualizerForm_Resize(object sender, EventArgs e) => AdjustSize();

        private void btnConfiguration_Click(object sender, EventArgs e)
        {
            using (var form = new ManageInstallationsForm())
                form.ShowDialog(this);
        }

        //ReSharper restore InconsistentNaming
        #endregion

        #endregion

        #endregion
    }
}
