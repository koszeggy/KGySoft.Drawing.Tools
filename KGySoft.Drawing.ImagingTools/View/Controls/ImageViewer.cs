﻿#region Copyright

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
using System.Threading;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.WinApi;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    /// <summary>
    /// Represents an image display control with zooming. Does not support implicit animation.
    /// </summary>
    internal partial class ImageViewer : BaseControl
    {
        #region InvalidateFlags enum

        [Flags]
        private enum InvalidateFlags
        {
            None,
            Sizes = 1,
            DisplayImage = 1 << 1,
            Image = 1 << 2,
            All = Sizes | DisplayImage | Image
        }

        #endregion

        #region Fields

        #region Static Fields

        private static readonly Size referenceScrollSize = new Size(32, 32);

        #endregion

        #region Instance Fields

        private readonly DisplayImageGenerator displayImageGenerator;

        private Image? image;
        private Rectangle targetRectangle;
        private Rectangle clientRectangle;
        private float zoom = 1;
        private Size scrollbarSize;
        private Size imageSize;
        private PixelFormat pixelFormat;

        private bool isMetafile;
        private bool smoothZooming;
        private bool autoZoom;
        private bool sbHorizontalVisible;
        private bool sbVerticalVisible;
        private bool isApplyingZoom;
        private bool isDragging;

        private int scrollFractionVertical;
        private int scrollFractionHorizontal;
        private Size draggingOrigin;
        private Point scrollingOrigin;

        #endregion

        #endregion

        #region Events

        internal event EventHandler? AutoZoomChanged
        {
            add => Events.AddHandler(nameof(AutoZoomChanged), value);
            remove => Events.RemoveHandler(nameof(AutoZoomChanged), value);
        }

        internal event EventHandler? ZoomChanged
        {
            add => Events.AddHandler(nameof(ZoomChanged), value);
            remove => Events.RemoveHandler(nameof(ZoomChanged), value);
        }

        #endregion

        #region Properties

        #region Internal Properties

        internal Image? Image
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
            set => SetAutoZoom(value, true);
        }

        internal float Zoom
        {
            get => zoom;
            set => SetZoom(value);
        }

        internal bool SmoothZooming
        {
            get => smoothZooming;
            set
            {
                if (smoothZooming == value)
                    return;
                smoothZooming = value;
                Invalidate(InvalidateFlags.DisplayImage);
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

        #endregion

        #endregion

        #region Constructors

        public ImageViewer()
        {
            InitializeComponent();

            SetStyle(ControlStyles.Selectable | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            scrollbarSize = new Size(SystemInformation.VerticalScrollBarWidth, SystemInformation.HorizontalScrollBarHeight);
            sbVertical.Width = scrollbarSize.Width;
            sbHorizontal.Height = scrollbarSize.Height;

            sbVertical.ValueChanged += ScrollbarValueChanged;
            sbHorizontal.ValueChanged += ScrollbarValueChanged;

            displayImageGenerator = new DisplayImageGenerator(this);
        }

        #endregion

        #region Methods

        #region Internal Methods

        /// <summary>
        /// Should be called when image content is changed while image reference remains the same (eg. rotation, palette change)
        /// </summary>
        internal void UpdateImage()
        {
            if (image == null)
                return;

            var flags = InvalidateFlags.Image | InvalidateFlags.DisplayImage;
            Size newImageSize;
            lock (image)
            {
                newImageSize = image.Size;
                pixelFormat = image.PixelFormat;
            }

            if (newImageSize != imageSize)
            {
                imageSize = newImageSize;
                flags |= InvalidateFlags.Sizes;
            }

            Invalidate(flags);
        }

        internal void IncreaseZoom()
        {
            SetAutoZoom(false, false);
            ApplyZoomChange(0.25f);
        }

        internal void DecreaseZoom()
        {
            SetAutoZoom(false, false);
            ApplyZoomChange(-0.25f);
        }

        internal void ResetZoom()
        {
            if (zoom.Equals(1f))
                return;
            AutoZoom = false;
            Zoom = 1f;
        }

        #endregion

        #region Protected Methods

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Invalidate(InvalidateFlags.Sizes | (autoZoom ? InvalidateFlags.DisplayImage : InvalidateFlags.None));
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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Up:
                    VerticalScroll(SystemInformation.MouseWheelScrollDelta);
                    return true;
                case Keys.Down:
                    VerticalScroll(-SystemInformation.MouseWheelScrollDelta);
                    return true;
                case Keys.Left:
                    HorizontalScroll(SystemInformation.MouseWheelScrollDelta);
                    return true;
                case Keys.Right:
                    HorizontalScroll(-SystemInformation.MouseWheelScrollDelta);
                    return true;
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
            if (!(sbHorizontalVisible || sbVerticalVisible) || (e.Button & MouseButtons.Left) == MouseButtons.None)
                return;
            isDragging = true;
            draggingOrigin = new Size(e.Location);
            scrollingOrigin = new Point(sbHorizontal.Value, sbVertical.Value);
            Cursor = Cursors.HandGrab;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if ((e.Button & MouseButtons.Left) == MouseButtons.None)
                return;
            isDragging = false;
            Cursor = sbHorizontalVisible || sbVerticalVisible ? Cursors.HandOpen : null;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!isDragging)
                return;
            Point distance = e.Location - draggingOrigin;
            if (sbHorizontalVisible && distance.X != 0)
                sbHorizontal.SetValueSafe(scrollingOrigin.X - distance.X);
            if (sbVerticalVisible && distance.Y != 0)
                sbVertical.SetValueSafe(scrollingOrigin.Y - distance.Y);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            switch (ModifierKeys)
            {
                // zoom
                case Keys.Control:
                    if (autoZoom)
                        SetAutoZoom(false, false);
                    float delta = (float)e.Delta / SystemInformation.MouseWheelScrollDelta / 5;
                    ApplyZoomChange(delta);
                    break;

                // vertical scroll
                case Keys.None:
                    VerticalScroll(e.Delta);
                    break;
            }
        }

        protected override void OnMouseHWheel(HandledMouseEventArgs e)
        {
            base.OnMouseHWheel(e);

            // horizontal scroll
            if (ModifierKeys == Keys.None)
                HorizontalScroll(-e.Delta);
        }

        protected override void OnRightToLeftChanged(EventArgs e) => AdjustSizes();

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            sbVertical.ValueChanged -= ScrollbarValueChanged;
            sbHorizontal.ValueChanged -= ScrollbarValueChanged;

            if (disposing)
                displayImageGenerator.Dispose();

            base.Dispose(disposing);
            if (disposing)
                Events.Dispose();
        }

        #endregion

        #region Private Methods

        private void SetImage(Image? value)
        {
            image = value;
            isMetafile = image is Metafile;
            imageSize = image?.Size ?? default;
            pixelFormat = image?.PixelFormat ?? default;
            Invalidate(InvalidateFlags.All);

            // making sure image is not under or over-zoomed
            if (!autoZoom && !isMetafile)
                SetZoom(zoom);
        }

        private void VerticalScroll(int delta)
        {
            // When scrolling by mouse, delta is always +-120 so this will be a small change on the scrollbar.
            // But we collect the fractional changes caused by the touchpad scrolling so it will not be lost either.
            int totalDelta = scrollFractionVertical + delta * sbVertical.SmallChange;
            scrollFractionVertical = totalDelta % SystemInformation.MouseWheelScrollDelta;
            int newValue = sbVertical.Value - totalDelta / SystemInformation.MouseWheelScrollDelta;
            sbVertical.SetValueSafe(newValue);
        }

        private void HorizontalScroll(int delta)
        {
            // When scrolling by mouse, delta is always +-120 so this will be a small change on the scrollbar.
            // But we collect the fractional changes caused by the touchpad scrolling so it will not be lost either.
            int totalDelta = scrollFractionHorizontal + delta * sbVertical.SmallChange;
            scrollFractionHorizontal = totalDelta % SystemInformation.MouseWheelScrollDelta;
            int newValue = sbHorizontal.Value - totalDelta / SystemInformation.MouseWheelScrollDelta;
            sbHorizontal.SetValueSafe(newValue);
        }

        private void Invalidate(InvalidateFlags flags)
        {
            if ((flags & InvalidateFlags.Sizes) != InvalidateFlags.None)
                AdjustSizes();

            if ((flags & InvalidateFlags.Image) != InvalidateFlags.None)
                displayImageGenerator.InvalidateImages();

            // this relies on the new calculated sizes so must come after adjusting sizes
            if ((flags & InvalidateFlags.DisplayImage) != InvalidateFlags.None)
                displayImageGenerator.InvalidateDisplayImage();

            Invalidate();
        }

        private void AdjustSizes()
        {
            if (imageSize.IsEmpty)
            {
                sbHorizontal.Visible = sbVertical.Visible = sbHorizontalVisible = sbVerticalVisible = false;
                targetRectangle = Rectangle.Empty;
                return;
            }

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
                scaledSize = imageSize.Scale(zoom);
                targetLocation = new Point(Math.Max(0, (clientSize.Width >> 1) - (scaledSize.Width >> 1)),
                    Math.Max(0, (clientSize.Height >> 1) - (scaledSize.Height >> 1)));

                targetRectangle = new Rectangle(targetLocation, scaledSize);
                clientRectangle = new Rectangle(Point.Empty, clientSize);
                sbHorizontal.Visible = sbVertical.Visible = sbHorizontalVisible = sbVerticalVisible = false;
                Cursor = null;
                return;
            }

            scaledSize = imageSize.Scale(zoom);

            // scrollbars visibility
            sbHorizontalVisible = scaledSize.Width > clientSize.Width
                || scaledSize.Width > clientSize.Width - scrollbarSize.Width && scaledSize.Height > clientSize.Height;
            sbVerticalVisible = scaledSize.Height > clientSize.Height
                || scaledSize.Height > clientSize.Height - scrollbarSize.Height && scaledSize.Width > clientSize.Width;

            if (sbHorizontalVisible)
                clientSize.Height -= scrollbarSize.Height;
            if (sbVerticalVisible)
                clientSize.Width -= scrollbarSize.Width;
            if (clientSize.Width < 1 || clientSize.Height < 1)
            {
                targetRectangle = Rectangle.Empty;
                return;
            }

            Point clientLocation = Point.Empty;
            targetLocation = new Point((clientSize.Width >> 1) - (scaledSize.Width >> 1),
                (clientSize.Height >> 1) - (scaledSize.Height >> 1));

            bool isRtl = RightToLeft == RightToLeft.Yes;

            // both scrollbars
            if (sbHorizontalVisible && sbVerticalVisible)
            {
                sbHorizontal.Dock = sbVertical.Dock = DockStyle.None;
                sbHorizontal.Width = clientSize.Width;
                sbHorizontal.Top = clientSize.Height;
                sbHorizontal.Left = isRtl ? scrollbarSize.Width : 0;
                sbVertical.Height = clientSize.Height;
                sbVertical.Left = isRtl ? 0 : clientSize.Width;
            }
            // horizontal scrollbar
            else if (sbHorizontalVisible)
            {
                sbHorizontal.Dock = DockStyle.Bottom;
            }
            // vertical scrollbar
            else if (sbVerticalVisible)
            {
                sbVertical.Dock = isRtl ? DockStyle.Left : DockStyle.Right;
            }

            // adjust scrollbar values
            if (sbHorizontalVisible)
            {
                sbHorizontal.Minimum = targetLocation.X;
                sbHorizontal.Maximum = targetLocation.X + scaledSize.Width;
                sbHorizontal.LargeChange = clientSize.Width;
                sbHorizontal.SmallChange = this.ScaleSize(referenceScrollSize).Width;
                sbHorizontal.Value = Math.Min(sbHorizontal.Value, sbHorizontal.Maximum - sbHorizontal.LargeChange);
            }

            if (sbVerticalVisible)
            {
                if (isRtl)
                {
                    targetLocation.X += scrollbarSize.Width;
                    clientLocation.X = scrollbarSize.Width;
                }

                sbVertical.Minimum = targetLocation.Y;
                sbVertical.Maximum = targetLocation.Y + scaledSize.Height;
                sbVertical.LargeChange = clientSize.Height;
                sbVertical.SmallChange = this.ScaleSize(referenceScrollSize).Height;
                sbVertical.Value = Math.Min(sbVertical.Value, sbVertical.Maximum - sbVertical.LargeChange);
            }

            sbHorizontal.Visible = sbHorizontalVisible;
            sbVertical.Visible = sbVerticalVisible;
            Cursor = sbHorizontalVisible || sbVerticalVisible ? Cursors.HandOpen : null;
            isDragging = false;

            clientRectangle = new Rectangle(clientLocation, clientSize);
            targetRectangle = new Rectangle(targetLocation, scaledSize);
            if (!isRtl || !sbVerticalVisible)
                return;

            clientRectangle.X = scrollbarSize.Width;
        }

        private void PaintImage(Graphics g)
        {
            g.IntersectClip(clientRectangle);
            Rectangle dest = targetRectangle;
            if (sbHorizontalVisible)
                dest.X -= sbHorizontal.Value;
            if (sbVerticalVisible)
                dest.Y -= sbVertical.Value;

            //// metafile or smoothing is off (smoothed metafile is generated async so it replaces the aliased result after some delay): NN
            //g.InterpolationMode = isMetafile || !smoothZooming ? InterpolationMode.NearestNeighbor
            //    // large zoom or small shrunk image: BC because these cases it's not so slow
            //    : zoom >= 4f || zoom < 1f && imageSize.Width <= sizeThreshold && imageSize.Height <= sizeThreshold ? InterpolationMode.HighQualityBicubic
            //    // small zoom: BL for large images to prevent heavy lagging; otherwise, BC
            //    : zoom > 1f ? imageSize.Width > sizeThreshold || imageSize.Height > sizeThreshold ? InterpolationMode.HighQualityBilinear : InterpolationMode.HighQualityBicubic
            //    // anything else, including large shrunk images: NN (the good quality preview is generated async so it replaces the NN result after some delay)
            //    : InterpolationMode.NearestNeighbor;

            g.InterpolationMode = isMetafile || !smoothZooming ? InterpolationMode.NearestNeighbor : InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // This lock ensures that no disposed image is painted. The generator also locks on itself when frees the cached preview.
            lock (displayImageGenerator)
            {
                // Locking on display image so if it is the same as the original image, which is also locked when accessing its bitmap data
                // the "bitmap region is already locked" can be avoided. Important: this cannot be ensured without locking here internally because
                // OnPaint can occur any time after invalidating.
                Image toDraw = displayImageGenerator.GetDisplayImage();
                bool useLock = image == toDraw;
                if (useLock)
                    Monitor.Enter(toDraw);
                try
                {
                    g.DrawImage(toDraw, dest);
                }
                catch (Exception e) when (!e.IsCriticalGdi())
                {
                    // it is still possible that image is in use without lock,
                    // in which case we simply re-invalidate the control and waiting for another chance to paint
                    Invalidate();
                }
                finally
                {
                    if (useLock)
                        Monitor.Exit(toDraw);
                }
            }
        }

        private void ApplyZoomChange(float delta)
        {
            if (delta.Equals(0f))
                return;
            delta += 1;
            SetZoom(zoom * delta);
        }

        private void SetAutoZoom(bool value, bool resetIfBitmap)
        {
            if (autoZoom == value)
                return;
            autoZoom = value;
            if (resetIfBitmap && !autoZoom && !isMetafile)
                SetZoom(1f);

            Invalidate(InvalidateFlags.Sizes | (autoZoom ? InvalidateFlags.DisplayImage : InvalidateFlags.None));
            OnAutoZoomChanged(EventArgs.Empty);
        }

        private void SetZoom(float value)
        {
            if (autoZoom || isApplyingZoom)
                return;

            float minZoom = image == null ? 1f : 1f / Math.Min(imageSize.Width, imageSize.Height);
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
                maxZoom = image == null ? 1f : Math.Max(
                    Math.Max(scale.X * 10, (screenSize.Width << 1) / (float)imageSize.Width),
                    Math.Max(scale.Y * 10, (screenSize.Height << 1) / (float)imageSize.Height));
            }

            if (value > maxZoom)
                value = maxZoom;

            if (zoom.Equals(value))
                return;

            zoom = value;
            Invalidate(InvalidateFlags.Sizes | InvalidateFlags.DisplayImage);
            isApplyingZoom = true;
            try
            {
                OnZoomChanged(EventArgs.Empty);
            }
            finally
            {
                isApplyingZoom = false;
            }
        }

        private void OnAutoZoomChanged(EventArgs e) => Events.GetHandler<EventHandler>(nameof(AutoZoomChanged))?.Invoke(this, e);
        private void OnZoomChanged(EventArgs e) => Events.GetHandler<EventHandler>(nameof(ZoomChanged))?.Invoke(this, e);

        #endregion

        #region Event handlers

        private void ScrollbarValueChanged(object? sender, EventArgs e) => Invalidate();

        #endregion

        #endregion
    }
}
