#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageViewer.cs
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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using KGySoft.CoreLibraries;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    /// <summary>
    /// Represents an image display control with zooming. Does not support implicit animation.
    /// </summary>
    internal partial class ImageViewer : Control
    {
        [Flags]
        private enum InvalidateFlags
        {
            None,
            Sizes = 1,
            PreviewImage = 1 << 1,
            All = Sizes | PreviewImage
        }

        private static readonly PixelFormat[] convertedFormats = { PixelFormat.Format16bppGrayScale, PixelFormat.Format48bppRgb, PixelFormat.Format64bppArgb, PixelFormat.Format64bppPArgb };

        private bool IsMetafilePreviewNeeded => isMetafile && (!autoZoom || antiAliasing);

        #region Constants

        private const int WS_BORDER = 0x00800000;

        #endregion

        #region Fields

        private Image image;
        private Image previewImage;
        private Rectangle destRectangle;
        private Rectangle sourceRectangle;
        private bool antiAliasing;
        private bool autoZoom;
        private float zoom = 1;
        private Size scrollbarSize;
        private Size imageSize;
        private bool isMetafile;

        #endregion

        #region Properties

        #region Internal Properties

        internal Image Image
        {
            get => image;
            set
            {
                if (image == value)
                    return;
                SetImage(value);
            }
        }

        private void SetImage(Image value)
        {
            Invalidate(InvalidateFlags.All);
            image = value;
            isMetafile = image is Metafile;
            imageSize = image?.Size ?? default;
            if (value != null)
            {
                sbHorizontal.Maximum = value.Width - 1;
                sbVertical.Maximum = value.Height - 1;
            }
        }

        internal bool AutoZoom
        {
            get => autoZoom;
            set
            {
                if (autoZoom == value)
                    return;
                autoZoom = value;
                if (!autoZoom)
                {
                    if (!isMetafile || zoom > 1f)
                        zoom = 1f;
                }

                Invalidate(InvalidateFlags.Sizes | (isMetafile ? InvalidateFlags.PreviewImage : InvalidateFlags.None));
            }
        }


        internal bool AntiAliasing
        {
            get => antiAliasing;
            set
            {
                if (antiAliasing == value)
                    return;
                antiAliasing = value;
                Invalidate(isMetafile ? InvalidateFlags.PreviewImage : InvalidateFlags.None);
            }
        }

        #endregion

        #region Protected Properties

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;

                // Fixed single border
                cp.Style |= WS_BORDER;
                return cp;
            }
        }

        #endregion

        #endregion

        #region Constructors

        internal ImageViewer()
        {
            InitializeComponent();

            SetStyle(ControlStyles.Selectable | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            scrollbarSize = new Size(SystemInformation.VerticalScrollBarWidth, SystemInformation.HorizontalScrollBarHeight);
            sbVertical.Width = scrollbarSize.Width;
            sbHorizontal.Height = scrollbarSize.Height;

            sbVertical.ValueChanged += sbVertical_ValueChanged;
            sbHorizontal.ValueChanged += sbHorizontal_ValueChanged;
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Invalidate(InvalidateFlags.Sizes | (IsMetafilePreviewNeeded ? InvalidateFlags.PreviewImage : InvalidateFlags.None));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (image == null || e.ClipRectangle.Width <= 0 || e.ClipRectangle.Height <= 0)
                return;

            if (destRectangle.IsEmpty)
                AdjustSizes();
            if (!destRectangle.IsEmpty)
                PaintImage(e.Graphics);

            //if (previewImage == null)
            //{
            //    GeneratePreview();
            //    if (previewRectangle.IsEmpty)
            //        return;
            //}

            //e.Graphics.DrawImage(previewImage, previewRectangle);


        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            // zoom
            if (ModifierKeys == Keys.Control)
            {
                if (autoZoom)
                    return;
                float delta = (float)e.Delta / SystemInformation.MouseWheelScrollDelta / 5;
                Zoom(delta);
                return;
            }


        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            sbVertical.ValueChanged -= sbVertical_ValueChanged;
            sbHorizontal.ValueChanged -= sbHorizontal_ValueChanged;

            if (disposing)
            {
                components?.Dispose();
                if (image != previewImage)
                    previewImage?.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void Invalidate(InvalidateFlags flags)
        {
            if ((flags & InvalidateFlags.Sizes) != InvalidateFlags.None)
                destRectangle = default;
            if ((flags & InvalidateFlags.PreviewImage) != InvalidateFlags.None)
            {
                if (image != previewImage)
                    previewImage?.Dispose();
                previewImage = null;
            }

            Invalidate();
        }

        private void AdjustSizes()
        {
            Size clientSize = ClientSize;
            if (clientSize.Width < 1 || clientSize.Height < 1)
            {
                destRectangle = Rectangle.Empty;
                return;
            }

            Point sourceLocation = sourceRectangle.Location;
            Point targetLocation;
            Size scaledSize;
            if (autoZoom)
            {
                zoom = Math.Min((float)clientSize.Width / imageSize.Width, (float)clientSize.Height / imageSize.Height);
                scaledSize = new Size((int)(imageSize.Width * zoom), (int)(imageSize.Height * zoom));
                targetLocation = new Point(Math.Max(0, (clientSize.Width >> 1) - (scaledSize.Width >> 1)),
                    Math.Max(0, (clientSize.Height >> 1) - (scaledSize.Height >> 1)));

                sourceRectangle = new Rectangle(Point.Empty, imageSize);
                destRectangle = new Rectangle(targetLocation, scaledSize);
                sbHorizontal.Visible = sbVertical.Visible = false;
                return;
            }

            scaledSize = imageSize.Scale(zoom);

            // scrollbars visibility
            bool sbHorizontalVisible = scaledSize.Width > clientSize.Width
                || scaledSize.Width > clientSize.Width - scrollbarSize.Width && scaledSize.Height > clientSize.Height - scrollbarSize.Height;
            bool sbVerticalVisible = scaledSize.Height > clientSize.Height
                || scaledSize.Height > clientSize.Width - scrollbarSize.Width && scaledSize.Height > clientSize.Height - scrollbarSize.Height;

            if (sbHorizontalVisible)
                clientSize.Height -= scrollbarSize.Height;
            if (sbVerticalVisible)
                clientSize.Width -= scrollbarSize.Width;

            //float ratio = Math.Min((float)scaledSize.Width / sourceSize.Width, (float)scaledSize.Height / sourceSize.Height);
            //Size targetSize = new Size((int)(sourceSize.Width * ratio), (int)(sourceSize.Height * ratio));
            targetLocation = new Point(Math.Max(0, (clientSize.Width >> 1) - (scaledSize.Width >> 1)),
                Math.Max(0, (clientSize.Height >> 1) - (scaledSize.Height >> 1)));

            Size sourceSize = imageSize;
            Size targetSize = scaledSize;

            // no scrollbars
            if (!sbHorizontalVisible && !sbVerticalVisible)
            {
                // the whole source is visible
                sourceLocation = Point.Empty;
            }
            else
            {
                // both scrollbars
                if (sbHorizontalVisible && sbVerticalVisible)
                {
                    sbHorizontal.Dock = sbVertical.Dock = DockStyle.None;
                    sbHorizontal.Width = clientSize.Width;
                    sbHorizontal.Top = clientSize.Height;
                    sbVertical.Height = clientSize.Height;
                    sbVertical.Left = clientSize.Width;

                    targetSize.Width = Math.Min(targetSize.Width, clientSize.Width);
                    targetSize.Height = Math.Min(targetSize.Height, clientSize.Height);

                    // the whole target is filled except scrollbars area
                    targetLocation = Point.Empty;
                }
                // horizontal scrollbar
                else if (sbHorizontalVisible)
                {
                    sbHorizontal.Dock = DockStyle.Bottom;
                    targetSize.Width = clientSize.Width;

                    // vertically the whole source is visible
                    sourceLocation.Y = 0;
                }
                // vertical scrollbar
                else
                {
                    sbVertical.Dock = DockStyle.Right;
                    targetSize.Height = clientSize.Height;

                    // horizontally the whole source is visible
                    sourceLocation.X = 0;
                }

                sourceSize = targetSize.Scale(1 / zoom);
            }

            // adjust scrollbar values
            if (sbHorizontalVisible)
            {
                sbHorizontal.LargeChange = Math.Max(1, sourceSize.Width);
                sourceLocation.X = Math.Min(imageSize.Width - sourceSize.Width, sbHorizontal.Value);
                sbHorizontal.Value = sourceLocation.X;
            }

            if (sbVerticalVisible)
            {
                sbVertical.LargeChange = Math.Max(1, sourceSize.Height);
                sourceLocation.Y = Math.Min(imageSize.Height - sourceSize.Height, sbVertical.Value);
                sbVertical.Value = sourceLocation.Y;
            }

            sbHorizontal.Visible = sbHorizontalVisible;
            sbVertical.Visible = sbVerticalVisible;

            if (targetSize.Width <= 0 || targetSize.Height <= 0)
            {
                destRectangle = Rectangle.Empty;
                return;
            }

            sourceRectangle = new Rectangle(sourceLocation, sourceSize);
            destRectangle = new Rectangle(targetLocation, targetSize);
        }

        private void PaintImage(Graphics g)
        {
            // 2. From the clone of the original: needs one clone
            if (previewImage == null)
                GeneratePreview();

            g.InterpolationMode = antiAliasing ? InterpolationMode.HighQualityBicubic : InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            if (sourceRectangle.Size == imageSize)
                g.DrawImage(previewImage, destRectangle);
            else if (IsMetafilePreviewNeeded)
                g.DrawImage(previewImage, destRectangle, new RectangleF(sourceRectangle.X * zoom, sourceRectangle.Y * zoom, sourceRectangle.Width * zoom, sourceRectangle.Height * zoom), GraphicsUnit.Pixel);
            else
                g.DrawImage(previewImage, destRectangle, sourceRectangle, GraphicsUnit.Pixel);

            //// 1. Directly from original image: fails for icons (no partial copy is possible: icon get stretched) - can be kept when the whole source is shown
            //g.DrawImage(image, destRectangle, sourceRectangle, GraphicsUnit.Pixel);

            //// 2. From the clone of the original: needs one clone
            //if (previewImage == null)
            //    previewImage = new Bitmap(image);
            //g.DrawImage(previewImage, destRectangle, sourceRectangle, GraphicsUnit.Pixel);

            //// 3. Generating only the shown part: works but a new image is generated even when just scrolling
            //// Needs InvalidatePreview on scrolling, zooming
            //if (previewImage == null)
            //{
            //    previewImage = new Bitmap(sourceRectangle.Width, sourceRectangle.Height, PixelFormat.Format32bppPArgb);
            //    image.DrawInto(previewImage, sourceRectangle, Point.Empty);
            //}
            //g.DrawImage(previewImage, destRectangle, new Rectangle(Point.Empty, sourceRectangle.Size), GraphicsUnit.Pixel);

            //// 4. Generate actual zoomed image: crashes when large image is zoomed
            //// Needs InvalidatePreview on zooming
            //if (previewImage == null)
            //    previewImage = new Bitmap(image, image.Size.Scale(zoom));
            //g.DrawImage(previewImage, destRectangle, new RectangleF(sourceRectangle.X * zoom, sourceRectangle.Y * zoom, sourceRectangle.Width * zoom, sourceRectangle.Height * zoom), GraphicsUnit.Pixel);
        }

        private void GeneratePreview()
        {
            if (image == null)
                return;

            // For icons Graphics.DrawImage does not work with partial source
            if (image.RawFormat.Guid == ImageFormat.Icon.Guid
                // Converting non supported or too memory consuming and slow pixel formats
                || image.PixelFormat.In(convertedFormats))
            {
                previewImage = image.ConvertPixelFormat(PixelFormat.Format32bppPArgb);
                return;
            }

            if (IsMetafilePreviewNeeded)
            {
                // here we generate a scaled preview
                previewImage = ((Metafile)image).ToBitmap(autoZoom ? destRectangle.Size : imageSize.Scale(zoom), antiAliasing);
                return;
            }

            previewImage = image;
        }

        //private void GeneratePreview()
        //{
        //    Rectangle clientRect = ClientRectangle;
        //    if (clientRect.Width < 1 || clientRect.Height < 1)
        //    {
        //        previewRectangle = Rectangle.Empty;
        //        return;
        //    }

        //    Size sourceSize = image.Size;
        //    Rectangle sourceRect = new Rectangle(Point.Empty, sourceSize);
        //    Size targetSize = autoZoom ? clientRect.Size : sourceSize.Scale(zoom);

        //    float ratio = Math.Min((float)targetSize.Width / sourceSize.Width, (float)targetSize.Height / sourceSize.Height);
        //    targetSize = new Size((int)(sourceSize.Width * ratio), (int)(sourceSize.Height * ratio));
        //    Point targetLocation = new Point((clientRect.Width >> 1) - (targetSize.Width >> 1), (clientRect.Height >> 1) - (targetSize.Height >> 1));

        //    previewRectangle = new Rectangle(targetLocation, targetSize);
        //    previewImage = image switch
        //    {
        //        Bitmap bmp => ResizeBitmap(bmp, targetSize),
        //        Metafile metafile => metafile.ToBitmap(targetSize, antiAliasing),
        //        _ => throw new InvalidOperationException(Res.InternalError($"Unexpected image type: {image.GetType()}"))
        //    };
        //}

        //private Bitmap ResizeBitmap(Bitmap bmp, Size targetSize)
        //{
        //    var result = new Bitmap(targetSize.Width, targetSize.Height);
        //    Size origSize = bmp.Size;
        //    using (Graphics graphics = Graphics.FromImage(result))
        //    {
        //        graphics.InterpolationMode = antiAliasing || targetSize.Width < origSize.Width ? InterpolationMode.HighQualityBicubic : InterpolationMode.NearestNeighbor;
        //        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        //        graphics.DrawImage(bmp, new Rectangle(Point.Empty, targetSize), new Rectangle(Point.Empty, origSize), GraphicsUnit.Pixel);
        //        graphics.Flush();
        //        return result;
        //    }

        //}

        private void Zoom(float delta)
        {
            if (delta == 0f)
                return;
            delta += 1;
            float newValue = zoom * delta;

            Size scaledSize = imageSize.Scale(newValue);
            if (scaledSize.Width < 1 || scaledSize.Height < 1)
                return;

            Size screenSize = Screen.GetBounds(this).Size;

            // in case of a metafile the maximum size is the larger value of screen size (which can be larger than client size) and image size but image size is limited to 10,000
            if (isMetafile && (scaledSize.Width > Math.Max(screenSize.Width, Math.Min(imageSize.Width, 10_000)) || scaledSize.Height > Math.Max(screenSize.Height, Math.Min(imageSize.Height, 10_000)))
                // inc case of a bitmap the default maximum size is image size * 10 but it can be increased to screen size
                || !isMetafile && (scaledSize.Width > Math.Max(screenSize.Width, imageSize.Width * 10) || scaledSize.Height > Math.Max(screenSize.Height, imageSize.Height * 10)))
            {
                return;
            }

            if (zoom == newValue)
                return;
            zoom = newValue;
            Invalidate(InvalidateFlags.Sizes | (IsMetafilePreviewNeeded ? InvalidateFlags.PreviewImage : InvalidateFlags.None));
        }

        #endregion

        private void sbHorizontal_ValueChanged(object sender, EventArgs e)
        {
            sourceRectangle.X = sbHorizontal.Value;
            Invalidate();
        }

        private void sbVertical_ValueChanged(object sender, EventArgs e)
        {
            sourceRectangle.Y = sbVertical.Value;
            Invalidate();
        }

        #endregion
    }
}
