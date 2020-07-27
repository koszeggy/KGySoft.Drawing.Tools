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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;

using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.WinApi;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    /// <summary>
    /// Represents an image display control with zooming. Does not support implicit animation.
    /// </summary>
    internal partial class ImageViewer : BaseControl
    {
        #region Nested types

        #region Enumerations

        [Flags]
        private enum InvalidateFlags
        {
            None,
            Sizes = 1,
            DisplayImage = 1 << 1,
            All = Sizes | DisplayImage
        }

        #endregion

        #region AsyncAntiAliasedMetafileGenerator class

        private sealed class AsyncAntiAliasedMetafileGenerator : IDisposable
        {
            #region Nested classes

            private sealed class GenerateTask
            {
                #region Fields

                internal Metafile Source;
                internal Image ReferenceImage;
                internal Size Size;
                internal volatile bool IsCanceled;

                #endregion
            }

            #endregion

            #region Fields

            private readonly ImageViewer owner;

            private GenerateTask runningTask;
            private GenerateTask pendingTask;
            private Metafile sourceClone;

            #endregion

            #region Constructors

            internal AsyncAntiAliasedMetafileGenerator(ImageViewer owner) => this.owner = owner;

            #endregion

            #region Methods

            #region Public Methods

            public void Dispose()
            {
                CancelPendingGenerate();
                FreeReferenceClone();
            }

            #endregion

            #region Internal Methods

            internal void BeginGenerate()
            {
                CancelPendingGenerate();
                Metafile metafile = owner.image as Metafile;
                Debug.Assert(metafile != null && owner.IsSmoothMetafileNeeded, "Metafile expected");

                // A clone must be created; otherwise, we might get an "object is used elsewhere" error from paint
                if (sourceClone == null)
                    sourceClone = (Metafile)metafile.Clone();
                var newTask = new GenerateTask { Source = sourceClone, ReferenceImage = metafile, Size = owner.targetRectangle.Size };

                lock (this) // it's fine, this is a private class
                {
                    if (runningTask == null)
                    {
                        runningTask = newTask;
                        ThreadPool.QueueUserWorkItem(DoGenerate, newTask);
                        return;
                    }

                    runningTask.IsCanceled = true;
                    pendingTask = newTask;
                }
            }

            internal void FreeReferenceClone()
            {
                sourceClone?.Dispose();
                sourceClone = null;
            }

            #endregion

            #region Private Methods

            private void CancelPendingGenerate()
            {
                lock (this) // it's fine, this is a private class
                {
                    if (runningTask == null)
                        return;
                    runningTask.IsCanceled = true;
                    pendingTask = null;
                }
            }

            /// <summary>
            /// Generates the display image on a pool thread.
            /// </summary>
            private void DoGenerate(object state)
            {
                Debug.WriteLine($"Generating on thread #{Thread.CurrentThread.ManagedThreadId}");
                var task = (GenerateTask)state;
                do
                {
                    Bitmap result = task.IsCanceled ? null : task.Source.ToBitmap(task.Size, true, false);
                    lock (this)
                    {
                        if (task.IsCanceled)
                        {
                            if (pendingTask == null)
                            {
                                Debug.WriteLine("Task canceled without continuation");
                                runningTask = null;
                                return;
                            }

                            Debug.WriteLine("Continuing with pending task");
                            task = runningTask = pendingTask;
                            pendingTask = null;
                            continue;
                        }

                        if (task.ReferenceImage == owner.image && task.Size == owner.targetRectangle.Size)
                        {
                            Debug.WriteLine("Applying generated result");

                            // not freeing the original image because it is always the original image here
                            owner.displayImage = result;
                            owner.Invalidate();
                        }

                        runningTask = null;
                        return;
                    }

                } while (task != null);
            }

            #endregion

            #endregion
        }

        #endregion

        #endregion

        #region Fields

        #region Static Fields

        private static readonly PixelFormat[] convertedFormats = { PixelFormat.Format16bppGrayScale, PixelFormat.Format48bppRgb, PixelFormat.Format64bppArgb, PixelFormat.Format64bppPArgb };
        private static readonly Size referenceScrollSize = new Size(32, 32);

        #endregion

        #region Instance Fields

        private Image image;
        private volatile Image displayImage;
        private Rectangle targetRectangle;
        private Rectangle clientRectangle;
        private bool smoothZooming;
        private bool autoZoom;
        private float zoom = 1;
        private Size scrollbarSize;
        private Size imageSize;
        private bool isMetafile;
        private bool sbHorizontalVisible;
        private bool sbVerticalVisible;
        private int scrollFractionVertical;
        private int scrollFractionHorizontal;
        private AsyncAntiAliasedMetafileGenerator antiAliasedMetafileGenerator;

        #endregion

        #endregion

        #region Events

        internal event EventHandler ZoomChanged
        {
            add => Events.AddHandler(nameof(ZoomChanged), value);
            remove => Events.RemoveHandler(nameof(ZoomChanged), value);
        }

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
                    SetZoom(1f);

                Invalidate(InvalidateFlags.Sizes | (IsSmoothMetafileNeeded ? InvalidateFlags.DisplayImage : InvalidateFlags.None));
            }
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
                Invalidate(isMetafile ? InvalidateFlags.DisplayImage : InvalidateFlags.None);
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

        private AsyncAntiAliasedMetafileGenerator AntiAliasedMetafileGenerator
        {
            get
            {
                if (antiAliasedMetafileGenerator == null)
                    Interlocked.CompareExchange(ref antiAliasedMetafileGenerator, new AsyncAntiAliasedMetafileGenerator(this), null);
                return antiAliasedMetafileGenerator;
            }
        }

        private bool IsSmoothMetafileNeeded => isMetafile && smoothZooming;

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
            Invalidate(InvalidateFlags.Sizes | (IsSmoothMetafileNeeded && autoZoom ? InvalidateFlags.DisplayImage : InvalidateFlags.None));
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

        protected override void OnMouseHWheel(HandledMouseEventArgs e)
        {
            base.OnMouseHWheel(e);

            // horizontal scroll
            if (ModifierKeys == Keys.None)
                HorizontalScroll(-e.Delta);
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            sbVertical.ValueChanged -= sbVertical_ValueChanged;
            sbHorizontal.ValueChanged -= sbHorizontal_ValueChanged;

            if (disposing)
            {
                antiAliasedMetafileGenerator?.Dispose();
                FreeDisplayImage();
                components?.Dispose();
            }

            base.Dispose(disposing);
            if (disposing)
                Events.Dispose();
        }

        #endregion

        #region Private Methods

        private void SetImage(Image value)
        {
            FreeDisplayImage(); // Called also from Invalidate but must be called also from here until image is replaced
            image = value;
            isMetafile = image is Metafile;
            imageSize = image?.Size ?? default;
            antiAliasedMetafileGenerator?.FreeReferenceClone();
            Invalidate(InvalidateFlags.All);
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
                AdjustSizes();

            if ((flags & InvalidateFlags.DisplayImage) != InvalidateFlags.None)
            {
                FreeDisplayImage();
                if (IsSmoothMetafileNeeded)
                    AntiAliasedMetafileGenerator.BeginGenerate();
            }

            Invalidate();
        }

        private void FreeDisplayImage()
        {
            Image toFree = displayImage;
            if (toFree != image)
                toFree?.Dispose();

            displayImage = null;
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
            //Debug.Assert(displayImage != null, "Display image should not be null here");

            if (displayImage == null)
                GenerateDisplayImage();
            g.IntersectClip(clientRectangle);
            Rectangle dest = targetRectangle;
            if (sbHorizontalVisible)
                dest.X -= sbHorizontal.Value;
            if (sbVerticalVisible)
                dest.Y -= sbVertical.Value;
            g.InterpolationMode = smoothZooming && !isMetafile ? InterpolationMode.HighQualityBicubic : InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawImage(displayImage, dest);
        }

        private void GenerateDisplayImage()
        {
            Debug.Assert(image != null, "Image is not expected to be null here");

            // Converting non supported or too memory consuming and slow pixel formats
            if (image.PixelFormat.In(convertedFormats))
            {
                displayImage = image.ConvertPixelFormat(PixelFormat.Format32bppPArgb);
                return;
            }

            // Raw icons: converting because icons are handled oddly by GDI+, for example, the first column has half pixel width
            if (image is Bitmap bmp && bmp.RawFormat.Guid == ImageFormat.Icon.Guid)
            {
                displayImage = bmp.CloneCurrentFrame();
                return;
            }

            if (IsSmoothMetafileNeeded)
            {
                displayImage = image;
                return;
            }

            displayImage = image;
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
            if (zoom == value || autoZoom)
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
                maxZoom = Math.Max(
                    Math.Max(scale.X * 10, (screenSize.Width << 1) / (float)imageSize.Width),
                    Math.Max(scale.Y * 10, (screenSize.Height << 1) / (float)imageSize.Height));
            }

            if (value > maxZoom)
                value = maxZoom;

            if (zoom == value)
                return;

            zoom = value;
            Invalidate(InvalidateFlags.Sizes | (IsSmoothMetafileNeeded ? InvalidateFlags.DisplayImage : InvalidateFlags.None));
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
