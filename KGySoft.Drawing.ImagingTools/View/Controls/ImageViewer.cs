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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;

using KGySoft.CoreLibraries;
using KGySoft.Drawing.Imaging;
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
                internal Metafile ReferenceImage;
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
            private Bitmap currentPreview;
            private Metafile referenceOfCurrentPreview;

            #endregion

            #region Constructors

            internal AsyncAntiAliasedMetafileGenerator(ImageViewer owner) => this.owner = owner;

            #endregion

            #region Methods
#pragma warning disable CS1690 // Accessing a member on a field of a marshal-by-reference class may cause a runtime exception - false alarm, owner is never a remote object

            #region Public Methods

            public void Dispose()
            {
                CancelPendingGenerate();
                Free();
            }

            #endregion

            #region Internal Methods

            internal void BeginGenerate()
            {
                CancelPendingGenerate();
                Metafile metafile = owner.image as Metafile;
                Debug.Assert(metafile != null && owner.IsSmoothMetafileNeeded, "Metafile with smooth preview expected");

                lock (this) // it's fine, this is a private class
                {
                    // checking if we already have the preview
                    if (TrySetPreview(metafile, owner.targetRectangle.Size))
                    {
                        if (runningTask != null)
                        {
                            runningTask.IsCanceled = true;
                            runningTask = null;
                        }

                        return;
                    }
                }

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

            internal void CancelPendingGenerate()
            {
                lock (this) // it's fine, this is a private class
                {
                    if (runningTask == null)
                        return;
                    runningTask.IsCanceled = true;
                    pendingTask = null;
                }
            }

            internal bool IsHandled(Image reference) => reference == referenceOfCurrentPreview;

            internal void Free()
            {
                sourceClone?.Dispose();
                sourceClone = null;

                FreeCachedPreview();
            }

            #endregion

            #region Private Methods

            private bool TrySetPreview(Metafile reference, Size size)
            {
                if (referenceOfCurrentPreview != null && reference != referenceOfCurrentPreview)
                {
                    Debug.Assert(currentPreview != owner.displayImage, "If image has been replaced in owner, its display image is not expected to be cached here");
                    FreeCachedPreview();
                    return false;
                }

                if (currentPreview?.Size != size)
                    return false;

                Debug.WriteLine($"Re-using pregenerated preview of size {size.Width}x{size.Height}");
                if (owner.displayImage == currentPreview)
                    return true;
                owner.FreeDisplayImage();
                owner.displayImage = currentPreview;
                owner.Invalidate();
                return true;
            }

            private void FreeCachedPreview()
            {
                lock (this)
                {
                    Bitmap toFree = currentPreview;
                    if (toFree != null)
                    {
                        if (toFree == owner.displayImage)
                        {
                            Debug.Fail("It is not expected that cached preview is in use when freeing it");
                            owner.displayImage = null;
                            owner.Invalidate();
                        }

                        toFree.Dispose();
                    }

                    currentPreview = null;
                    referenceOfCurrentPreview = null;
                }
            }

            /// <summary>
            /// Generates the display image on a pool thread.
            /// </summary>
            private void DoGenerate(object state)
            {
                var task = (GenerateTask)state;
                do
                {
                    lock (this)
                    {
                        // checking if we already have the preview
                        if (!task.IsCanceled)
                        {
                            if (TrySetPreview(task.ReferenceImage, task.Size))
                            {
                                runningTask = null;
                                return;
                            }

                            // Before creating the preview releasing previous cached result. It is important to free it here, before checking the free memory.
                            FreeCachedPreview();
                        }
                    }

                    Bitmap result = null;
                    if (!task.IsCanceled)
                    {
                        // For the resizing large managed buffer of source.Height * target.Width of ColorF (16 bytes) is allocated internally. To be safe we count with the doubled sizes.
                        Size doubledSize = new Size(task.Size.Width << 1, task.Size.Height << 1);
                        long managedPressure = doubledSize.Width * doubledSize.Height * 16;
                        if (!MemoryHelper.CanAllocate(managedPressure))
                        {
                            Debug.WriteLine($"Discarding task because there is no {managedPressure:N0} bytes of available managed memory");
                            task.IsCanceled = true;
                        }

                        if (!task.IsCanceled)
                        {
                            // MetafileExtensions.ToBitmap does the same if anti aliasing is requested but this way the process can be canceled
                            Debug.WriteLine($"Generating anti aliased image {task.Size.Width}x{task.Size.Height} on thread #{Thread.CurrentThread.ManagedThreadId}");
                            Bitmap doubled = null;
                            try
                            {
                                doubled = new Bitmap(task.Source, task.Size.Width << 1, task.Size.Height << 1);
                                if (!task.IsCanceled)
                                {
                                    result = new Bitmap(task.Size.Width, task.Size.Height, PixelFormat.Format32bppPArgb);
                                    using IReadableBitmapData src = doubled.GetReadableBitmapData();
                                    using IReadWriteBitmapData dst = result.GetReadWriteBitmapData();

                                    // not using Task and await, because this method's signature must match the WaitCallback delegate, and we want to be compatible with .NET 3.5, too
                                    IAsyncResult asyncResult = src.BeginDrawInto(dst, new Rectangle(Point.Empty, doubled.Size), new Rectangle(Point.Empty, result.Size),
                                        // ReSharper disable once AccessToModifiedClosure - intended, if IsCanceled is modified we need to return its modified value
                                        asyncConfig: new AsyncConfig { IsCancelRequestedCallback = () => task.IsCanceled, ThrowIfCanceled = false });

                                    // as we are already on a pool thread this is not a UI blocking call
                                    asyncResult.AsyncWaitHandle.WaitOne();

                                    // This will throw an exception if resizing failed (it also allocate.
                                    BitmapDataExtensions.EndDrawInto(asyncResult);
                                }
                            }
                            catch (Exception e) when (!e.IsCriticalGdi())
                            {
                                // Despite all of the preconditions the memory could not be allocated or some other error occurred (yes, we catch even OutOfMemoryException here)
                                // NOTE: practically we always can recover from here: we simply don't use a generated preview and the worker thread can be finished
                                task.IsCanceled = true;
                            }
                            finally
                            {
                                doubled?.Dispose();
                                if (task.IsCanceled)
                                {
                                    result?.Dispose();
                                    result = null;
                                }
                            }
                        }
                    }

                    lock (this)
                    {
                        // setting latest cache
                        if (result != null)
                        {
                            // it can be set again here due to a lost race
                            FreeCachedPreview();
                            currentPreview = result;
                            referenceOfCurrentPreview = task.ReferenceImage;
                        }

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
                            Debug.Assert(owner.displayImage == null || owner.displayImage == owner.image, "Display image is not the same as the original one: dispose is necessary");

                            // not freeing the display image because it is always the original image here
                            owner.displayImage = result;
                            owner.Invalidate();
                        }

                        runningTask = null;
                        return;
                    }

                } while (task != null);
            }

            #endregion

#pragma warning restore CS1690 // Accessing a member on a field of a marshal-by-reference class may cause a runtime exception
            #endregion
        }

        #endregion

        #endregion

        #region Fields

        #region Static Fields

        private static readonly PixelFormat[] convertedFormats = OSUtils.IsWindows
            // Windows: these are either not supported by Graphics or are very slow
            ? new[] { PixelFormat.Format16bppGrayScale, PixelFormat.Format48bppRgb, PixelFormat.Format64bppArgb, PixelFormat.Format64bppPArgb }
            // Non Windows (eg. Mono/Linux): these are not supported by Graphics
            : new[] { PixelFormat.Format16bppRgb555, PixelFormat.Format16bppRgb565 };

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
        private bool isApplyingZoom;

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

                Invalidate(InvalidateFlags.Sizes | (IsSmoothMetafileNeeded && autoZoom ? InvalidateFlags.DisplayImage : InvalidateFlags.None));
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

        public ImageViewer()
        {
            InitializeComponent();

            SetStyle(ControlStyles.Selectable | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            scrollbarSize = new Size(SystemInformation.VerticalScrollBarWidth, SystemInformation.HorizontalScrollBarHeight);
            sbVertical.Width = scrollbarSize.Width;
            sbHorizontal.Height = scrollbarSize.Height;

            sbVertical.ValueChanged += ScrollbarValueChanged;
            sbHorizontal.ValueChanged += ScrollbarValueChanged;
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

            sbVertical.ValueChanged -= ScrollbarValueChanged;
            sbHorizontal.ValueChanged -= ScrollbarValueChanged;

            if (disposing)
            {
                FreeDisplayImage();
                antiAliasedMetafileGenerator?.Dispose();
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
            antiAliasedMetafileGenerator?.Free();
            Invalidate(InvalidateFlags.All);

            // making sure image is not under or overzoomed
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
                else
                    antiAliasedMetafileGenerator?.CancelPendingGenerate();
            }

            Invalidate();
        }

        private void FreeDisplayImage()
        {
            Image toFree = displayImage;
            displayImage = null;
            if (toFree != image && antiAliasedMetafileGenerator?.IsHandled(image) != true)
                toFree?.Dispose();
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

        [SuppressMessage("Reliability", "CA2002:Do not lock on objects with weak identity", Justification = "False alarm, displayImage is not a remote object")]
        private void PaintImage(Graphics g)
        {
            if (displayImage == null)
                GenerateDisplayImage();
            g.IntersectClip(clientRectangle);
            Rectangle dest = targetRectangle;
            if (sbHorizontalVisible)
                dest.X -= sbHorizontal.Value;
            if (sbVerticalVisible)
                dest.Y -= sbVertical.Value;
            g.InterpolationMode = !isMetafile && (smoothZooming || zoom < 1f) ? InterpolationMode.HighQualityBicubic : InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // Locking on display image so if it is the same as the original image, which is also locked when accessing its bitmap data
            // the "bitmap region is already locked" can be avoided. Important: this cannot be ensured without locking here internally because
            // OnPaint can occur any time after invalidating.
            lock (displayImage)
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

            if (zoom == value)
                return;

            zoom = value;
            Invalidate(InvalidateFlags.Sizes | (IsSmoothMetafileNeeded ? InvalidateFlags.DisplayImage : InvalidateFlags.None));
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

        private void OnZoomChanged(EventArgs e) => Events.GetHandler<EventHandler>(nameof(ZoomChanged))?.Invoke(this, e);

        #endregion

        #region Event handlers

        private void ScrollbarValueChanged(object sender, EventArgs e) => Invalidate();

        #endregion

        #endregion
    }
}
