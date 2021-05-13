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
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;
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

        #region InvalidateFlags enum

        [Flags]
        private enum InvalidateFlags
        {
            None,
            Sizes = 1,
            DisplayImage = 1 << 1,
            All = Sizes | DisplayImage
        }

        #endregion

        #region PreviewGenerator class

        private sealed class PreviewGenerator : IDisposable
        {
            #region Nested classes

            private sealed class GenerateTask : AsyncTaskBase
            {
                #region Fields

                internal Image? SourceImage;
                internal Size Size;

                #endregion
            }

            #endregion

            #region Fields

            private readonly ImageViewer owner;
            private readonly object syncRootGenerate = new object();

            private bool enabled;
            private GenerateTask? activeTask;

            private Image? sourceClone;
            private Image? safeDefaultImage; // The default image displayed when no generated preview is needed or while generation is in progress
            private bool isClonedSafeDefaultImage;
            private Size requestedSize;

            private volatile Image? displayImage; // The actual displayed image. If not null, it is either equals safeDefaultImage or currentPreview.
            private volatile Bitmap? cachedDisplayImage; // The lastly generated display image. Can be unused but is cached until a next preview is generated.
            private Size currentCachedDisplayImage; // just to cache cachedDisplayImage.Size, because accessing currentPreview can lead to "object is used elsewhere" error

            #endregion

            #region Constructors

            internal PreviewGenerator(ImageViewer owner)
            {
                this.owner = owner;
                enabled = true;
            }

            #endregion

            #region Methods
#pragma warning disable CS1690 // Accessing a member on a field of a marshal-by-reference class may cause a runtime exception - false alarm, owner is never a remote object

            #region Public Methods

            public void Dispose() => Free();

            #endregion

            #region Internal Methods

            internal Image? GetDisplayImage(bool generateSyncIfNull)
            {
                Image? result = displayImage;
                if (result != null || !generateSyncIfNull)
                    return result;

                if (safeDefaultImage == null)
                {
                    Debug.Assert(owner.image != null, "Image is not expected to be null here");
                    Image image = owner.image!;

                    // Locking on display image so if it is the same as the original image, which is also locked when accessing its bitmap data
                    // the "bitmap region is already locked" can be avoided. Important: this cannot be ensured without locking here internally because
                    // OnPaint can occur any time after invalidating.
                    lock (image)
                    {
                        PixelFormat pixelFormat = image.PixelFormat;

                        try
                        {
                            // Converting non supported or too slow pixel formats
                            if (pixelFormat.In(convertedFormats) || pixelFormat != PixelFormat.Format32bppPArgb && (image.Width > generateThreshold || image.Height > generateThreshold))
                            {
                                safeDefaultImage = image.ConvertPixelFormat(PixelFormat.Format32bppPArgb);
                                isClonedSafeDefaultImage = true;
                            }

                            // Raw icons: converting because icons are handled oddly by GDI+, for example, the first column has half pixel width
                            else if (image is Bitmap bmp && bmp.RawFormat.Guid == ImageFormat.Icon.Guid)
                            {
                                isClonedSafeDefaultImage = true;
                                safeDefaultImage = bmp.CloneCurrentFrame();
                            }
                            else
                                safeDefaultImage = image;
                        }
                        catch (Exception e) when (!e.IsCriticalGdi())
                        {
                            // for converted formats trying again with more compact formats
                            if (pixelFormat.In(convertedFormats))
                            {
                                try
                                {
                                    safeDefaultImage = pixelFormat == PixelFormat.Format16bppGrayScale
                                        ? image.ConvertPixelFormat(PixelFormat.Format8bppIndexed, PredefinedColorsQuantizer.Grayscale())
                                        : image.ConvertPixelFormat(pixelFormat.HasAlpha() ? PixelFormat.Format32bppPArgb : PixelFormat.Format24bppRgb);
                                    isClonedSafeDefaultImage = true;
                                }
                                catch (Exception eInner) when (!eInner.IsCriticalGdi())
                                {
                                    // If pixel format is not supported at all then we let rendering die; otherwise, it may work but slowly or with visual glitches
                                    isClonedSafeDefaultImage = false;
                                    safeDefaultImage = image;
                                }
                            }
                            else
                            {
                                // It may happen if no clone could be created (maybe on low memory)
                                // Here it is not so important after all because we wanted to use it for performance reasons.
                                isClonedSafeDefaultImage = false;
                                safeDefaultImage = image;
                            }
                        }
                    }
                }

                // it is possible that we have a displayImage now but if not we return the default
                if (displayImage == null)
                    Interlocked.CompareExchange(ref displayImage, safeDefaultImage, null);
                return displayImage;
            }

            internal void BeginGenerateDisplayImage()
            {
                CancelRunningGenerate();
                if (!enabled)
                    return;

                Image? image = owner.image;
                if (image == null)
                {
                    Debug.Assert(cachedDisplayImage == null && displayImage == null);
                    return;
                }

                Size size = owner.targetRectangle.Size;
                bool isGenerateNeeded = owner.isMetafile
                    ? owner.smoothZooming
                    : owner.smoothZooming && owner.zoom < 1f && (owner.imageSize.Width >= generateThreshold || owner.imageSize.Height >= generateThreshold);

                if (!isGenerateNeeded || size.Width < 1 || size.Height < 1)
                {
                    displayImage = safeDefaultImage;
                    return;
                }

                requestedSize = size;
                ThreadPool.QueueUserWorkItem(DoGenerate!, new GenerateTask { SourceImage = sourceClone, Size = size });
            }

            internal void Free()
            {
                CancelRunningGenerate();
                WaitForPendingGenerate();
                requestedSize = default;
                displayImage = null;
                sourceClone?.Dispose();
                sourceClone = null;
                if (isClonedSafeDefaultImage)
                    safeDefaultImage?.Dispose();
                safeDefaultImage = null;
                isClonedSafeDefaultImage = false;
                FreeCachedPreview();
                enabled = true;
            }

            #endregion

            #region Private Methods

            private void CancelRunningGenerate()
            {
                GenerateTask? runningTask = activeTask;
                if (runningTask == null)
                    return;
                runningTask.IsCanceled = true;
            }

            private void WaitForPendingGenerate()
            {
                // In a non-UI thread it should be in a lock
                GenerateTask? runningTask = activeTask;
                if (runningTask == null)
                    return;
                runningTask.WaitForCompletion();
                runningTask.Dispose();
                activeTask = null;
            }

            private bool TrySetPreview(Image? reference, Size size)
            {
                if (sourceClone != null && reference != sourceClone)
                {
                    Debug.Assert(cachedDisplayImage != displayImage, "If image has been replaced in owner, its display image is not expected to be cached here");
                    FreeCachedPreview();
                    return false;
                }

                // we don't free generated preview here maybe it can be re-used later (eg. toggling metafile smooth zooming)
                if (currentCachedDisplayImage != size)
                    return false;

                if (displayImage == cachedDisplayImage)
                    return true;

                Debug.WriteLine($"Re-using pregenerated preview of size {size.Width}x{size.Height}");
                displayImage = cachedDisplayImage;
                owner.Invalidate();
                return true;
            }

            private void FreeCachedPreview()
            {
                lock (this) // It is alright, this is a private class. ImageViewer also locks on this instance when obtains display image so this ensures that no disposed image is painted.
                {
                    if (displayImage != null && displayImage == cachedDisplayImage)
                    {
                        displayImage = null;
                        owner.Invalidate();
                    }

                    Bitmap? toFree = cachedDisplayImage;
                    cachedDisplayImage = null;
                    toFree?.Dispose();
                    currentCachedDisplayImage = default;
                }
            }

            private void DoGenerate(object state)
            {
                var task = (GenerateTask)state;

                // this is a fairly large lock ensuring that only one generate task is running at once
                lock (syncRootGenerate)
                {
                    // lost race
                    if (task.SourceImage != sourceClone || task.Size != requestedSize || !enabled)
                    {
                        task.Dispose();
                        return;
                    }

                    // checking if we already have the preview
                    if (!task.IsCanceled)
                    {
                        if (TrySetPreview(task.SourceImage, task.Size))
                        {
                            task.Dispose();
                            return;
                        }

                        // Before creating the preview releasing previous cached result. It is important to free it here, before checking the free memory.
                        FreeCachedPreview();
                    }

                    if (task.SourceImage == null)
                    {
                        Debug.Assert(sourceClone == null && owner.image != null);
                        Image image = owner.image!;

                        // As OnPaint can occur any time in the UI thread we lock on it. See also PaintImage.
                        lock (image)
                        {
                            try
                            {
                                // A clone must be created to use the image without locking later on and getting an "object is used elsewhere" error from paint.
                                // This is created synchronously so it can be used as a reference in the generating tasks.
                                if (owner.isMetafile)
                                    sourceClone = (Metafile)image.Clone();
                                else
                                {
                                    PixelFormat pixelFormat = image.PixelFormat;
                                    var bmp = (Bitmap)image;

                                    // clone is tried to be compact, fast and compatible
                                    sourceClone = pixelFormat.In(PixelFormat.Format32bppArgb, PixelFormat.Format32bppPArgb, PixelFormat.Format64bppArgb, PixelFormat.Format64bppPArgb) ? bmp.ConvertPixelFormat(PixelFormat.Format32bppPArgb)
                                        : pixelFormat.In(PixelFormat.Format24bppRgb, PixelFormat.Format32bppRgb, PixelFormat.Format48bppRgb) ? bmp.ConvertPixelFormat(PixelFormat.Format24bppRgb)
                                        : pixelFormat == PixelFormat.Format16bppGrayScale ? bmp.ConvertPixelFormat(PixelFormat.Format8bppIndexed, PredefinedColorsQuantizer.Grayscale())
                                        : pixelFormat.In(convertedFormats) ? bmp.ConvertPixelFormat(PixelFormat.Format32bppPArgb)
                                        : bmp.CloneCurrentFrame();
                                }

                                task.SourceImage = sourceClone;
                            }
                            catch (Exception e) when (!e.IsCriticalGdi())
                            {
                                // Disabling preview generation if we could not create the clone (eg. on low memory)
                                // It will be re-enabled when owner.Image is reset.
                                enabled = false;
                                sourceClone?.Dispose();
                                sourceClone = null;
                                return;
                            }
                        }
                    }

                    Debug.Assert(activeTask?.IsCanceled != false);
                    WaitForPendingGenerate();
                    Debug.Assert(activeTask == null);

                    // from now on the task can be canceled
                    activeTask = task;

                    try
                    {
                        Bitmap? result = null;
                        try
                        {
                            if (!task.IsCanceled)
                                result = task.SourceImage is Metafile ? GenerateMetafilePreview(task) : GenerateBitmapPreview(task);
                        }
                        finally
                        {
                            task.SetCompleted();
                        }

                        if (result != null)
                        {
                            // setting latest cache (even if the task has been canceled since the generating the completed result)
                            currentCachedDisplayImage = task.Size;
                            cachedDisplayImage = result;
                        }

                        if (task.IsCanceled)
                            return;

                        Debug.WriteLine("Applying generated result");
                        Debug.Assert(displayImage == null || displayImage == safeDefaultImage || displayImage == owner.image, "Display image is not the same as the original one: dispose is necessary");

                        // not freeing the display image because it is always the original image here
                        displayImage = result;
                        owner.Invalidate();
                    }
                    finally
                    {
                        task.Dispose();
                        activeTask = null;
                    }
                }
            }

            private static Bitmap? GenerateMetafilePreview(GenerateTask task)
            {
                // For the resizing large managed buffer of source.Height * target.Width of ColorF (16 bytes) is allocated internally. To be safe we count with the doubled sizes.
                Size doubledSize = new Size(task.Size.Width << 1, task.Size.Height << 1);
                long managedPressure = doubledSize.Width * doubledSize.Height * 16;
                if (!MemoryHelper.CanAllocate(managedPressure))
                {
                    Debug.WriteLine($"Discarding task because there is no {managedPressure:N0} bytes of available managed memory");
                    task.IsCanceled = true;
                }

                if (task.IsCanceled)
                    return null;
                // MetafileExtensions.ToBitmap does the same if anti aliasing is requested but this way the process can be canceled
                Debug.WriteLine($"Generating anti aliased image {task.Size.Width}x{task.Size.Height} on thread #{Thread.CurrentThread.ManagedThreadId}");
                Bitmap? result = null;
                Bitmap? doubled = null;
                try
                {
                    doubled = new Bitmap(task.SourceImage!, task.Size.Width << 1, task.Size.Height << 1);
                    if (!task.IsCanceled)
                    {
                        result = new Bitmap(task.Size.Width, task.Size.Height, PixelFormat.Format32bppPArgb);
                        using IReadableBitmapData src = doubled.GetReadableBitmapData();
                        using IReadWriteBitmapData dst = result.GetReadWriteBitmapData();

                        // not using Task and await, because this method's signature must match the WaitCallback delegate, and we want to be compatible with .NET 3.5, too
                        IAsyncResult asyncResult = src.BeginDrawInto(dst, new Rectangle(Point.Empty, doubled.Size), new Rectangle(Point.Empty, result.Size),
                            // ReSharper disable once AccessToModifiedClosure - intended, if IsCanceled is modified we need to return its modified value
                            asyncConfig: new AsyncConfig { IsCancelRequestedCallback = () => task.IsCanceled, ThrowIfCanceled = false });

                        // As we are already on a pool thread this is not a UI blocking call
                        // This will throw an exception if resizing failed (resizing also allocates a large amount of memory).
                        asyncResult.EndDrawInto();
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

                return result;
            }

            private static Bitmap? GenerateBitmapPreview(GenerateTask task)
            {
                // BitmapExtensions.Resize does the same but this way the process can be canceled
                Debug.WriteLine($"Generating smoothed image {task.Size.Width}x{task.Size.Height} on thread #{Thread.CurrentThread.ManagedThreadId}");

                Bitmap? result = null;
                try
                {
                    result = new Bitmap(task.Size.Width, task.Size.Height, PixelFormat.Format32bppPArgb);
                    using IReadableBitmapData src = ((Bitmap)task.SourceImage!).GetReadableBitmapData();
                    using IReadWriteBitmapData dst = result.GetReadWriteBitmapData();
                    var cfg = new AsyncConfig { IsCancelRequestedCallback = () => task.IsCanceled, ThrowIfCanceled = false, MaxDegreeOfParallelism = Environment.ProcessorCount >> 1 };

                    // Not using Task and await, because this method's signature must match the WaitCallback delegate, and we want to be compatible with .NET 3.5, too.
                    // As we are already on a pool thread the End... call does not block the UI.
                    var srcRect = new Rectangle(Point.Empty, task.SourceImage!.Size);
                    var dstRect = new Rectangle(Point.Empty, task.Size);
                    if (srcRect == dstRect)
                    {
                        IAsyncResult asyncResult = src.BeginCopyTo(dst, srcRect, Point.Empty, asyncConfig: cfg);
                        asyncResult.EndCopyTo();
                    }
                    else
                    {
                        IAsyncResult asyncResult = src.BeginDrawInto(dst, srcRect, dstRect, asyncConfig: cfg);
                        asyncResult.EndDrawInto();
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
                    if (task.IsCanceled)
                    {
                        result?.Dispose();
                        result = null;
                    }
                }

                return result;
            }

            #endregion

#pragma warning restore CS1690 // Accessing a member on a field of a marshal-by-reference class may cause a runtime exception
            #endregion
        }

        #endregion

        #endregion

        #region Constants

        private const int generateThreshold = 1000;

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

        private readonly PreviewGenerator previewGenerator;

        private Image? image;
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
        private bool isApplyingZoom;
        private bool isDragging;
        private Size draggingOrigin;
        private Point scrollingOrigin;

        #endregion

        #endregion

        #region Events

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
            set
            {
                if (autoZoom == value)
                    return;
                autoZoom = value;
                if (!autoZoom && !isMetafile)
                    SetZoom(1f);

                Invalidate(InvalidateFlags.Sizes | (autoZoom ? InvalidateFlags.DisplayImage : InvalidateFlags.None));
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

            previewGenerator = new PreviewGenerator(this);
        }

        #endregion

        #region Methods

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
                previewGenerator.Dispose();

            base.Dispose(disposing);
            if (disposing)
                Events.Dispose();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Should be called when image content is changed
        /// </summary>
        internal void UpdateImage()
        {
            if (image == null)
                return;

            // can happen when image is rotated
            if (image.Size != imageSize || !ReferenceEquals(image, previewGenerator.GetDisplayImage(false)))
                SetImage(image);
            else
                Invalidate();
        }

        #endregion

        #region Private Methods

        private void SetImage(Image? value)
        {
            previewGenerator.Free();
            image = value;
            isMetafile = image is Metafile;
            imageSize = image?.Size ?? default;
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

            if ((flags & InvalidateFlags.DisplayImage) != InvalidateFlags.None)
                previewGenerator.BeginGenerateDisplayImage();

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
            Cursor = sbHorizontalVisible || sbVerticalVisible ? Cursors.HandOpen : null;
            isDragging = false;

            clientRectangle = new Rectangle(Point.Empty, clientSize);
            targetRectangle = new Rectangle(targetLocation, scaledSize);
        }

        private void PaintImage(Graphics g)
        {
            g.IntersectClip(clientRectangle);
            Rectangle dest = targetRectangle;
            if (sbHorizontalVisible)
                dest.X -= sbHorizontal.Value;
            if (sbVerticalVisible)
                dest.Y -= sbVertical.Value;
            g.InterpolationMode = !isMetafile && (smoothZooming && zoom > 1f || smoothZooming && zoom < 1f && imageSize.Width < generateThreshold && imageSize.Height < generateThreshold) ? InterpolationMode.HighQualityBicubic : InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // This lock ensures that no disposed image is painted. The generator also locks on itself when frees the cached preview.
            lock (previewGenerator)
            {
                // Locking on display image so if it is the same as the original image, which is also locked when accessing its bitmap data
                // the "bitmap region is already locked" can be avoided. Important: this cannot be ensured without locking here internally because
                // OnPaint can occur any time after invalidating.
                Image toDraw = previewGenerator.GetDisplayImage(true)!;
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

        private void OnZoomChanged(EventArgs e) => Events.GetHandler<EventHandler>(nameof(ZoomChanged))?.Invoke(this, e);

        #endregion

        #region Event handlers

        private void ScrollbarValueChanged(object? sender, EventArgs e) => Invalidate();

        #endregion

        #endregion
    }
}
