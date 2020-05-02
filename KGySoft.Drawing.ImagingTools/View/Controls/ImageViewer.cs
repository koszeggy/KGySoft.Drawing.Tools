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
using KGySoft.Drawing.ImagingTools.WinApi;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    /// <summary>
    /// Represents an image display control with zooming. Does not support implicit animation.
    /// </summary>
    internal partial class ImageViewer : Control
    {
        #region Enumerations

        [Flags]
        private enum InvalidateFlags
        {
            None,
            Sizes = 1,
            PreviewImage = 1 << 1,
            All = Sizes | PreviewImage
        }

        #endregion

        #region Fields

        #region Static Fields

        private static readonly PixelFormat[] convertedFormats = { PixelFormat.Format16bppGrayScale, PixelFormat.Format48bppRgb, PixelFormat.Format64bppArgb, PixelFormat.Format64bppPArgb };
        private static readonly Size referenceScrollSize = new Size(32, 32);

        #endregion

        #region Instance Fields

        private Image image;
        private Image previewImage;
        private Rectangle targetRectangle;
        private Rectangle clientRectangle;
        private bool antiAliasing;
        private bool autoZoom;
        private float zoom = 1;
        private Size scrollbarSize;
        private Size imageSize;
        private bool isMetafile;
        private bool sbHorizontalVisible;
        private bool sbVerticalVisible;
        private int scrollFractionVertical;
        private int scrollFractionHorizontal;

        #endregion

        #region Events

        internal event EventHandler ZoomChanged
        {
            add => Events.AddHandler(nameof(ZoomChanged), value);
            remove => Events.RemoveHandler(nameof(ZoomChanged), value);
        }

        #endregion

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

        internal bool AutoZoom
        {
            get => autoZoom;
            set
            {
                if (autoZoom == value)
                    return;
                autoZoom = value;
                if (!autoZoom && !isMetafile)
                    zoom = 1f;

                Invalidate(InvalidateFlags.Sizes | (IsMetafilePreviewNeeded ? InvalidateFlags.PreviewImage : InvalidateFlags.None));
            }
        }

        internal float Zoom
        {
            get => zoom;
            set => SetZoom(value);
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
                cp.Style |= Constants.WS_BORDER;
                return cp;
            }
        }

        #endregion

        #region Private Properties

        private bool IsMetafilePreviewNeeded => isMetafile && antiAliasing;

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
            Invalidate(InvalidateFlags.Sizes | (IsMetafilePreviewNeeded && autoZoom ? InvalidateFlags.PreviewImage : InvalidateFlags.None));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (image == null || e.ClipRectangle.Width <= 0 || e.ClipRectangle.Height <= 0)
                return;

            if (targetRectangle.IsEmpty)
                AdjustSizes();
            if (!targetRectangle.IsEmpty)
                PaintImage(e.Graphics);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            switch (ModifierKeys)
            {
                // zoom
                case Keys.Control:
                    if (autoZoom)
                        return;
                    float delta = (float)e.Delta / SystemInformation.MouseWheelScrollDelta / 5;
                    ApplyZoomChange(delta);
                    break;

                // vertical scroll
                case Keys.None:
                    VerticalScroll(e.Delta);
                    break;
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.HWnd != Handle)
                return;

            switch (m.Msg)
            {
                case Constants.WM_MOUSEHWHEEL:
                    HorizontalScroll(-(short)((m.WParam.ToInt64() >> 16) & 0xffff));
                    m.Result = new IntPtr(1);
                    break;
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
            if (disposing)
                Events.Dispose();
        }

        #endregion

        #region Private Methods

        private void SetImage(Image value)
        {
            Invalidate(InvalidateFlags.All);
            image = value;
            isMetafile = image is Metafile;
            imageSize = image?.Size ?? default;
        }

        private void VerticalScroll(int delta)
        {
            // When scrolling by mouse, delta is always +-120 so this will be a small change on the scrollbar.
            // But we collect the fractional changes caused by the touchpad scrolling so it will not be lost either.
            int totalDelta = scrollFractionVertical + delta * sbVertical.SmallChange;
            scrollFractionVertical = totalDelta % SystemInformation.MouseWheelScrollDelta;
            int newValue = sbVertical.Value - totalDelta / SystemInformation.MouseWheelScrollDelta;
            if (newValue < sbVertical.Minimum)
                newValue = sbVertical.Minimum;
            else if (newValue > sbVertical.Maximum - sbVertical.LargeChange + 1)
                newValue = sbVertical.Maximum - sbVertical.LargeChange + 1;

            sbVertical.Value = newValue;
        }

        private void HorizontalScroll(int delta)
        {
            // When scrolling by mouse, delta is always +-120 so this will be a small change on the scrollbar.
            // But we collect the fractional changes caused by the touchpad scrolling so it will not be lost either.
            int totalDelta = scrollFractionHorizontal + delta * sbVertical.SmallChange;
            scrollFractionHorizontal = totalDelta % SystemInformation.MouseWheelScrollDelta;
            int newValue = sbHorizontal.Value - totalDelta / SystemInformation.MouseWheelScrollDelta;
            if (newValue < sbHorizontal.Minimum)
                newValue = sbHorizontal.Minimum;
            else if (newValue > sbHorizontal.Maximum - sbHorizontal.LargeChange + 1)
                newValue = sbHorizontal.Maximum - sbHorizontal.LargeChange + 1;

            sbHorizontal.Value = newValue;
        }

        private void Invalidate(InvalidateFlags flags)
        {
            if ((flags & InvalidateFlags.Sizes) != InvalidateFlags.None)
                targetRectangle = default;
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
                targetRectangle = Rectangle.Empty;
                return;
            }

            Point targetLocation;
            Size scaledSize;
            if (autoZoom)
            {
                zoom = Math.Min((float)clientSize.Width / imageSize.Width, (float)clientSize.Height / imageSize.Height);
                scaledSize = new Size((int)(imageSize.Width * zoom), (int)(imageSize.Height * zoom));
                targetLocation = new Point(Math.Max(0, (clientSize.Width >> 1) - (scaledSize.Width >> 1)),
                    Math.Max(0, (clientSize.Height >> 1) - (scaledSize.Height >> 1)));

                targetRectangle = new Rectangle(targetLocation, scaledSize);
                clientRectangle = new Rectangle(Point.Empty, clientSize);
                sbHorizontal.Visible = sbVertical.Visible = sbHorizontalVisible = sbVerticalVisible = false;
                return;
            }

            scaledSize = imageSize.Scale(zoom);

            // scrollbars visibility
            sbHorizontalVisible = scaledSize.Width > clientSize.Width
                || scaledSize.Width > clientSize.Width - scrollbarSize.Width && scaledSize.Height > clientSize.Height - scrollbarSize.Height;
            sbVerticalVisible = scaledSize.Height > clientSize.Height
                || scaledSize.Height > clientSize.Width - scrollbarSize.Width && scaledSize.Height > clientSize.Height - scrollbarSize.Height;

            if (sbHorizontalVisible)
                clientSize.Height -= scrollbarSize.Height;
            if (sbVerticalVisible)
                clientSize.Width -= scrollbarSize.Width;
            if (clientSize.Width < 1 || clientSize.Height < 1)
            {
                targetRectangle = Rectangle.Empty;
                return;
            }

            targetLocation = new Point((clientSize.Width >> 1) - (scaledSize.Width >> 1),
                (clientSize.Height >> 1) - (scaledSize.Height >> 1));

            targetRectangle = new Rectangle(targetLocation, scaledSize);
            clientRectangle = new Rectangle(Point.Empty, clientSize);

            // both scrollbars
            if (sbHorizontalVisible && sbVerticalVisible)
            {
                sbHorizontal.Dock = sbVertical.Dock = DockStyle.None;
                sbHorizontal.Width = clientSize.Width;
                sbHorizontal.Top = clientSize.Height;
                sbVertical.Height = clientSize.Height;
                sbVertical.Left = clientSize.Width;
            }
            // horizontal scrollbar
            else if (sbHorizontalVisible)
            {
                sbHorizontal.Dock = DockStyle.Bottom;
            }
            // vertical scrollbar
            else if (sbVerticalVisible)
            {
                sbVertical.Dock = DockStyle.Right;
            }

            // adjust scrollbar values
            if (sbHorizontalVisible)
            {
                sbHorizontal.Minimum = targetRectangle.X;
                sbHorizontal.Maximum = targetRectangle.Right;
                sbHorizontal.LargeChange = clientSize.Width;
                sbHorizontal.SmallChange = this.ScaleSize(referenceScrollSize).Width;
                sbHorizontal.Value = Math.Min(sbHorizontal.Value, sbHorizontal.Maximum - sbHorizontal.LargeChange);
            }

            if (sbVerticalVisible)
            {
                sbVertical.Minimum = targetRectangle.Y;
                sbVertical.Maximum = targetRectangle.Bottom;
                sbVertical.LargeChange = clientSize.Height;
                sbVertical.SmallChange = this.ScaleSize(referenceScrollSize).Height;
                sbVertical.Value = Math.Min(sbVertical.Value, sbVertical.Maximum - sbVertical.LargeChange);
            }

            sbHorizontal.Visible = sbHorizontalVisible;
            sbVertical.Visible = sbVerticalVisible;

            clientRectangle = new Rectangle(Point.Empty, clientSize);
            targetRectangle = new Rectangle(targetLocation, scaledSize);
        }

        private void PaintImage(Graphics g)
        {
            if (previewImage == null)
                GeneratePreview();
            g.IntersectClip(clientRectangle);
            Rectangle dest = targetRectangle;
            if (sbHorizontalVisible)
                dest.X -= sbHorizontal.Value;
            if (sbVerticalVisible)
                dest.Y -= sbVertical.Value;

            g.InterpolationMode = antiAliasing ? InterpolationMode.HighQualityBicubic : InterpolationMode.NearestNeighbor;
            //g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawImage(previewImage, dest);
        }

        private void GeneratePreview()
        {
            if (image == null)
                return;

            // Converting non supported or too memory consuming and slow pixel formats
            if (image.PixelFormat.In(convertedFormats))
            {
                previewImage = image.ConvertPixelFormat(PixelFormat.Format32bppPArgb);
                return;
            }

            if (IsMetafilePreviewNeeded)
            {
                // here we generate a scaled preview
                previewImage = ((Metafile)image).ToBitmap(targetRectangle.Size, antiAliasing);
                return;
            }

            previewImage = image;
        }

        private void ApplyZoomChange(float delta)
        {
            if (delta == 0f)
                return;
            delta += 1;
            SetZoom(zoom * delta);
        }

        private void SetZoom(float value)
        {
            if (zoom == value)
                return;

            float minZoom = 1f / Math.Min(imageSize.Width, imageSize.Height);
            if (value < minZoom)
                value = minZoom;

            Size screenSize = Screen.GetBounds(this).Size;
            float maxZoom;

            if (isMetafile)
            {
                // For metafiles the max zoom is between 1x and 2x screen size. 2x screen size is allowed if that is below 10,000 pixels
                const int maxMetafileSize = 10_000;
                maxZoom = Math.Max(
                        Math.Min(Math.Max(screenSize.Width, maxMetafileSize), screenSize.Width << 1),
                        Math.Min(Math.Max(screenSize.Height, maxMetafileSize), screenSize.Height << 1))
                    / (float)Math.Max(imageSize.Width, imageSize.Height);
            }
            else
            {
                // For bitmaps the default maximum size is image size * 10 (adjusted with DPI) but at least screen size x 2 
                PointF scale = this.GetScale();
                maxZoom = Math.Max(
                    Math.Max(scale.X * 10, (screenSize.Width << 1) / (float)imageSize.Width),
                    Math.Max(scale.Y * 10, (screenSize.Height << 1) / (float)imageSize.Height));
            }

            if (value > maxZoom)
                value = maxZoom;

            zoom = value;
            Invalidate(InvalidateFlags.Sizes | (IsMetafilePreviewNeeded ? InvalidateFlags.PreviewImage : InvalidateFlags.None));
            OnZoomChanged(EventArgs.Empty);
        }

        private void OnZoomChanged(EventArgs e) => Events.GetHandler<EventHandler>(nameof(ZoomChanged))?.Invoke(this, e);

        #endregion

        #region Event handlers

        private void sbHorizontal_ValueChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void sbVertical_ValueChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        #endregion

        #endregion
    }
}
