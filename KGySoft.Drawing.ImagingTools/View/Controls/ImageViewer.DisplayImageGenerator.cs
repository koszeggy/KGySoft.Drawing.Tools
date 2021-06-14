#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageViewer.DisplayImageGenerator.cs
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
using System.Drawing.Imaging;
using System.Threading;

using KGySoft.CoreLibraries;
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Reflection;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    internal partial class ImageViewer
    {
        #region PreviewGenerator class

        private sealed class DisplayImageGenerator : IDisposable
        {
            #region Nested classes

            #region GenerateDefaultImageTask class
            
            private sealed class GenerateDefaultImageTask : AsyncTaskBase
            {
                #region Fields

                internal Bitmap SourceBitmap;
                internal bool InvalidateOwner;

                #endregion
            }

            #endregion

            // TODO
            //private sealed class GenerateTask : AsyncTaskBase
            //{
            //    //#region Fields

            //    //internal Image? SourceImage;
            //    //internal Size Size;

            //    //#endregion
            //}

            #endregion

            #region Constants

            private const int sizeThreshold = 1024;

            #endregion

            #region Fields

            #region Static Fields

            /// <summary>
            /// These formats are not are not supported by Graphics even though a Bitmap can use them.
            /// On Linux/Mono some formats are completely unsupported but they do not appear here.
            /// </summary>
            private static readonly PixelFormat[] unsupportedFormats = OSUtils.IsWindows
                ? new[] { PixelFormat.Format16bppGrayScale }
                : new[] { PixelFormat.Format16bppRgb555, PixelFormat.Format16bppRgb565 };

            /// <summary>
            /// These formats are so slow that if .
            /// </summary>
            private static readonly PixelFormat[] slowFormats = OSUtils.IsWindows
                ? new[] { PixelFormat.Format48bppRgb, PixelFormat.Format64bppArgb, PixelFormat.Format64bppArgb }
                : Reflector.EmptyArray<PixelFormat>();

            #endregion

            #region Instance Fields

            private readonly ImageViewer owner;
            private readonly object syncRootGenerate = new object();

            private volatile bool disposed;

            /// <summary>
            /// true if generator can generate new content. Turned off on low memory or by <see cref="Free"/>. Invalidating the image enables it again.
            /// </summary>
            private bool enabled;

            private AsyncTaskBase? generateDefaultImageTask;
            private AsyncTaskBase? generateDisplayImageTask;

            /// <summary>
            /// The default image to be displayed when no generated preview is needed or while generation is in progress.
            /// Set by <see cref="GenerateDefaultImage"/>. If <see cref="isDefaultImageCloned"/> is true, then contains
            /// - A fast PARGB32 clone of the original image it that is a Bitmap
            /// - A clone of the original image if that is a Metafile so the original image will not be blocked to generate resized images
            /// Otherwise, it is the same reference as the owner.Image.
            /// If not null, then always contains a compatible, displayable image.
            /// </summary>
            private volatile Image? defaultDisplayImage;
            private bool isDefaultImageCloned;

            // TODO
            //private Image? sourceClone; // Used for creating the resized preview. Just to avoid "image is used elsewhere"
            //private Size requestedSize;

            private volatile Image? adjustedDisplayImage; // The actual displayed image. If not null, it is either equals safeDefaultImage or cachedDisplayImage.
            //private volatile Bitmap? cachedDisplayImage; // The lastly generated display image. Can be unused but is cached until a next preview is generated.
            //private Size currentCachedDisplayImage; // just to cache cachedDisplayImage.Size, because accessing currentPreview can lead to "object is used elsewhere" error

            #endregion

            #endregion

            #region Constructors

            internal DisplayImageGenerator(ImageViewer owner)
            {
                this.owner = owner;
                enabled = true;
            }

            #endregion

            #region Methods

            #region Static Methods

            private static void CancelRunningGenerate(AsyncTaskBase? task)
            {
                if (task == null)
                    return;
                task.IsCanceled = true;
            }

            private static void WaitForPendingGenerate(ref AsyncTaskBase? task)
            {
                if (task == null)
                    return;
                task.WaitForCompletion();
                task.Dispose();
                task = null;
            }

            #endregion

            #region Instance Methods

            #region Public Methods

            public void Dispose()
            {
                if (disposed)
                    return;
                Free();
                disposed = true;
            }

            #endregion

            #region Internal Methods

            internal Image? GetDisplayImage()
            {
                #region Local Methods

                void SetOrGenerateDefaultDisplayImage()
                {
                    Debug.Assert(owner.image != null && owner.pixelFormat != default);
                    if (!enabled)
                        return;

                    Image image = owner.image!;
                    Bitmap? bitmap = image as Bitmap;

                    // Metafile: The default image is the same as the original. If anti-aliased images are required, a clone is created on demand from that task
                    // Bitmap: generating a new default image for unsupported formats,
                    bool isGenerateNeeded = bitmap != null && (owner.pixelFormat.In(unsupportedFormats)
                        // non-PARGB32 images larger than 256x256 - note: leaving even slow formats unconverted below sizeThreshold / 4
                        || owner.pixelFormat != PixelFormat.Format32bppPArgb && (owner.imageSize.Width > sizeThreshold >> 2 || owner.imageSize.Height > sizeThreshold >> 2)
                        // Raw icons: converting because icons are handled oddly by GDI+, for example, the first column has half pixel width
                        || bitmap.RawFormat.Guid == ImageFormat.Icon.Guid);

                    if (!isGenerateNeeded)
                    {
                        // A generated default image is set from another thread so handling possible concurrency.
                        if (defaultDisplayImage == null)
                            Interlocked.CompareExchange(ref defaultDisplayImage, image, null);
                        return;
                    }

                    // We check if there is a generate in progress. If not, then we start a new one (if it has just been finished).
                    AsyncTaskBase? task = generateDefaultImageTask;
                    if (task != null)
                        return;

                    task = new GenerateDefaultImageTask
                    {
                        SourceBitmap = bitmap!,
                        InvalidateOwner = bitmap!.RawFormat.Guid == ImageFormat.Icon.Guid
                    };
                    generateDefaultImageTask = task;
                    ThreadPool.QueueUserWorkItem(GenerateDefaultImage, task);
                }

                #endregion

                Debug.Assert(owner.image != null);

                // 1.) There is a size adjusted display image=
                Image? result = adjustedDisplayImage;
                if (result != null)
                    return result;

                // 2.) Checking if there is an already available (fast) default image
                result = defaultDisplayImage;
                if (result != null)
                    return result;

                SetOrGenerateDefaultDisplayImage();

                // Waiting for the task to be finished if pixel format is not supported,
                // or it is so slow (>= 48bpp) that it is faster to wait for the converted image and paint that one.
                // Note: Not using async because this project targets also .NET 3.5 and the image is locked anyway
                if (owner.pixelFormat.In(unsupportedFormats) || owner.pixelFormat.In(slowFormats))
                    generateDefaultImageTask?.WaitForCompletion();

                // 3.) Returning possibly generated display image or falling back to the original image
                if (!enabled)
                {
                    Free();

                    // So next time we will return sooner, at 2.)
                    return defaultDisplayImage = owner.image;
                }

                return adjustedDisplayImage ?? defaultDisplayImage ?? owner.image;
            }

            /// <summary>
            /// Invalidates everything and resets the default image if it is not needed to be generated.
            /// </summary>
            internal void InvalidateImages()
            {
                // This cancels all tasks and disposes every generating resources
                Free();
                Debug.Assert(generateDefaultImageTask == null);

                // (Re-)enabling generating images
                enabled = true;

                // TODO
                //Image? image = owner.image;
                //if (image == null)
                //    return;

                //Debug.Assert(owner.pixelFormat != default, "PixelFormat must be known if image is not null");
                //// Metafile: no generating needed, the default image is the same as the original
                //// (for Metafiles a clone is created on demand is anti-aliased images have to be created to prevent locking on the displayed image)
                //if (image is not Bitmap bitmap)
                //{
                //    defaultImage = image;
                //    return;
                //}

                //// Bitmap: generating a new default image for unsupported formats,
                //bool isGenerateNeeded = owner.pixelFormat.In(unsupportedFormats)
                //    // non-PARGB32 images larger than 256x256 - note: leaving even slow formats unconverted below sizeThreshold / 4
                //    || owner.pixelFormat != PixelFormat.Format32bppPArgb && (owner.imageSize.Width > sizeThreshold >> 2 || owner.imageSize.Height > sizeThreshold >> 2)
                //    // Raw icons: converting because icons are handled oddly by GDI+, for example, the first column has half pixel width
                //    || bitmap.RawFormat.Guid == ImageFormat.Icon.Guid;

                //// If generate is needed, then leaving defaultImage null and starting a generate when an actual Paint is triggered.
                //// Starting the task here would just be slower anyway because it locks the image, which wouldn't let the Paint draw the original image until it ends.
                //if (!isGenerateNeeded)
                //    defaultImage = image;
            }

            /// <summary>
            /// Invalidates the display image and if required, starts to generate a new one
            /// </summary>
            internal void InvalidateDisplayImage()
            {
                // todo: mainly the old BeginGenerateDisplayImage
                // TODO: handle possible defaultImage generation
            }

            // TODO
            //internal void BeginGenerateDisplayImage()
            //{
            //    CancelRunningGenerate();
            //    if (!enabled)
            //        return;

            //    // TODO
            //    //Image? image = owner.image;
            //    //if (image == null)
            //    //{
            //    //    Debug.Assert(cachedDisplayImage == null && displayImage == null);
            //    //    return;
            //    //}

            //    //Size size = owner.targetRectangle.Size;
            //    //bool isGenerateNeeded = owner.isMetafile
            //    //    ? owner.smoothZooming
            //    //    : owner.smoothZooming && owner.zoom < 1f && (owner.imageSize.Width > sizeThreshold || owner.imageSize.Height > sizeThreshold);

            //    //if (!isGenerateNeeded || size.Width < 1 || size.Height < 1)
            //    //{
            //    //    displayImage = safeDefaultImage;
            //    //    return;
            //    //}

            //    //requestedSize = size;
            //    //ThreadPool.QueueUserWorkItem(DoGenerate!, new GenerateTask { SourceImage = sourceClone, Size = size });
            //}

            #endregion

            #region Private Methods

            private void Free()
            {
                // to prevent starting new tasks while freeing resources
                enabled = false;
                CancelRunningGenerate(generateDefaultImageTask);
                CancelRunningGenerate(generateDisplayImageTask);

                lock (syncRootGenerate)
                {
                    WaitForPendingGenerate(ref generateDefaultImageTask);
                    WaitForPendingGenerate(ref generateDisplayImageTask);

                    // TODO
                    //requestedSize = default;
                    //displayImage = null;
                    //sourceClone?.Dispose();
                    //sourceClone = null;

                    // ImageViewer also locks on the private generator when obtains display image so this ensures that no disposed image is painted.
                    lock (this)
                    {
                        if (isDefaultImageCloned)
                            defaultDisplayImage?.Dispose();
                        defaultDisplayImage = null;
                        isDefaultImageCloned = false;
                    }

                    // TODO
                    //FreeCachedPreview();
                }
            }

            private void GenerateDefaultImage(object state)
            {
                #region Local Methods

                static Bitmap? DoGenerateDefaultImage(GenerateDefaultImageTask task, ref bool enabled)
                {
                    Size size = task.SourceBitmap.Size;

                    // skipping generating clone if there is not enough memory and it would only serve performance
                    // x4: because we want to convert to 32bpp
                    long managedPressure = size.Width * size.Height * 4;
                    if (!MemoryHelper.CanAllocate(managedPressure) && !task.SourceBitmap.PixelFormat.In(unsupportedFormats))
                    {
                        Debug.WriteLine($"Discarding task because there is no {managedPressure:N0} bytes of available managed memory");
                        task.IsCanceled = true;
                    }

                    Debug.WriteLine($"Generating fast default image on thread #{Thread.CurrentThread.ManagedThreadId}");
                    Bitmap? result = null;
                    try
                    {
                        result = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppPArgb);
                        using IReadableBitmapData src = task.SourceBitmap.GetReadableBitmapData();
                        using IWritableBitmapData dst = result.GetWritableBitmapData();

                        // here allowing to use max parallelization as the original image is locked anyway
                        var cfg = new AsyncConfig { IsCancelRequestedCallback = () => task.IsCanceled, ThrowIfCanceled = false };

                        // Not using Task and await because we want to be compatible with .NET 3.5, too.
                        IAsyncResult asyncResult = src.BeginCopyTo(dst, asyncConfig: cfg);

                        // As we are already on a pool thread the End... call does not block the UI.
                        asyncResult.EndCopyTo();
                    }
                    catch (Exception e) when (!e.IsCriticalGdi())
                    {
                        // Despite all of the preconditions the memory could not be allocated or some other error occurred (yes, we catch even OutOfMemoryException here)
                        // NOTE: practically we always can recover from here: we simply don't use a generated clone and the worker thread can be finished
                        task.IsCanceled = true;
                        enabled = false;
                    }
                    finally
                    {
                        if (task.IsCanceled)
                        {
                            Debug.WriteLine("Canceled default image generate");
                            result?.Dispose();
                            result = null;
                        }
                    }

                    return result;
                }

                #endregion

                var task = (GenerateDefaultImageTask)state;

                // ensuring that only one generate task is running at once
                lock (syncRootGenerate)
                {
                    // lost race
                    if (!enabled || task.SourceBitmap != owner.image || disposed)
                    {
                        task.Dispose();
                        return;
                    }

                    // from now on the task can be canceled
                    generateDefaultImageTask = task;

                    try
                    {
                        Bitmap? result = null;

                        // unless already replaced, this is the same as the owner.image so locking on it to avoid the possible "bitmap region is already locked" issue.
                        lock (task.SourceBitmap)
                        {
                            try
                            {
                                // Generating the actual result. IsCanceled might be true if the lock above could not be immediately acquired
                                if (!task.IsCanceled)
                                    result = DoGenerateDefaultImage(task, ref enabled);
                            }
                            finally
                            {
                                task.SetCompleted();
                            }
                        }

                        if (result == null || task.IsCanceled)
                            return;

                        defaultDisplayImage = result;
                        isDefaultImageCloned = true;

                        // only for icons because otherwise the appearance is the same
                        if (task.InvalidateOwner)
                            owner.Invalidate();
                    }
                    finally
                    {
                        task.Dispose();
                        generateDefaultImageTask = null;
                    }
                }
            }

            // TODO
            //private bool TrySetPreview(Image? reference, Size size)
            //{
            //    if (sourceClone != null && reference != sourceClone)
            //    {
            //        Debug.Assert(cachedDisplayImage != displayImage, "If image has been replaced in owner, its display image is not expected to be cached here");
            //        FreeCachedPreview();
            //        return false;
            //    }

            //    // we don't free generated preview here maybe it can be re-used later (eg. toggling metafile smooth zooming)
            //    if (currentCachedDisplayImage != size)
            //        return false;

            //    if (displayImage == cachedDisplayImage)
            //        return true;

            //    Debug.WriteLine($"Re-using pregenerated preview of size {size.Width}x{size.Height}");
            //    displayImage = cachedDisplayImage;
            //    owner.Invalidate();
            //    return true;
            //}

            //private void FreeCachedPreview()
            //{
            //    lock (this) // It is alright, this is a private class. ImageViewer also locks on this instance when obtains display image so this ensures that no disposed image is painted.
            //    {
            //        if (displayImage != null && displayImage == cachedDisplayImage)
            //        {
            //            displayImage = null;
            //            owner.Invalidate();
            //        }

            //        Bitmap? toFree = cachedDisplayImage;
            //        cachedDisplayImage = null;
            //        toFree?.Dispose();
            //        currentCachedDisplayImage = default;
            //    }
            //}

            //private void DoGenerate(object state)
            //{
            //    var task = (GenerateTask)state;

            //    // this is a fairly large lock ensuring that only one generate task is running at once
            //    lock (syncRootGenerate)
            //    {
            //        // lost race
            //        if (!enabled || task.SourceImage != sourceClone || task.Size != requestedSize)
            //        {
            //            task.Dispose();
            //            return;
            //        }

            //        // checking if we already have the preview
            //        if (!task.IsCanceled)
            //        {
            //            if (TrySetPreview(task.SourceImage, task.Size))
            //            {
            //                task.Dispose();
            //                return;
            //            }

            //            // Before creating the preview releasing previous cached result. It is important to free it here, before checking the free memory.
            //            FreeCachedPreview();
            //        }

            //        if (task.SourceImage == null)
            //        {
            //            Debug.Assert(sourceClone == null && owner.image != null);
            //            Image image = owner.image!;

            //            // As OnPaint can occur any time in the UI thread we lock on it. See also PaintImage.
            //            lock (image)
            //            {
            //                try
            //                {
            //                    // A clone must be created to use the image without locking later on and getting an "object is used elsewhere" error from paint.
            //                    // This is created synchronously so it can be used as a reference in the generating tasks.
            //                    if (owner.isMetafile)
            //                        sourceClone = (Metafile)image.Clone();
            //                    else
            //                    {
            //                        PixelFormat pixelFormat = image.PixelFormat;
            //                        var bmp = (Bitmap)image;

            //                        // clone is tried to be compact, fast and compatible
            //                        sourceClone = pixelFormat.In(PixelFormat.Format32bppArgb, PixelFormat.Format32bppPArgb, PixelFormat.Format64bppArgb, PixelFormat.Format64bppPArgb) ? bmp.ConvertPixelFormat(PixelFormat.Format32bppPArgb)
            //                            : pixelFormat.In(PixelFormat.Format24bppRgb, PixelFormat.Format32bppRgb, PixelFormat.Format48bppRgb) ? bmp.ConvertPixelFormat(PixelFormat.Format24bppRgb)
            //                            : pixelFormat == PixelFormat.Format16bppGrayScale ? bmp.ConvertPixelFormat(PixelFormat.Format8bppIndexed, PredefinedColorsQuantizer.Grayscale())
            //                            : pixelFormat.In(convertedFormats) ? bmp.ConvertPixelFormat(PixelFormat.Format32bppPArgb)
            //                            : bmp.CloneCurrentFrame();
            //                    }

            //                    task.SourceImage = sourceClone;
            //                }
            //                catch (Exception e) when (!e.IsCriticalGdi())
            //                {
            //                    // Disabling preview generation if we could not create the clone (eg. on low memory)
            //                    // It will be re-enabled when owner.Image is reset.
            //                    enabled = false;
            //                    sourceClone?.Dispose();
            //                    sourceClone = null;
            //                    return;
            //                }
            //            }
            //        }

            //        Debug.Assert(activeTask?.IsCanceled != false);
            //        WaitForPendingGenerate();
            //        Debug.Assert(activeTask == null);

            //        // from now on the task can be canceled
            //        activeTask = task;

            //        try
            //        {
            //            Bitmap? result = null;
            //            try
            //            {
            //                if (!task.IsCanceled)
            //                    result = task.SourceImage is Metafile ? GenerateMetafilePreview(task) : GenerateBitmapPreview(task);
            //            }
            //            finally
            //            {
            //                task.SetCompleted();
            //            }

            //            if (result != null)
            //            {
            //                // setting latest cache (even if the task has been canceled since the generating the completed result)
            //                currentCachedDisplayImage = task.Size;
            //                cachedDisplayImage = result;
            //            }

            //            if (task.IsCanceled)
            //                return;

            //            Debug.WriteLine("Applying generated result");
            //            Debug.Assert(displayImage == null || displayImage == safeDefaultImage || displayImage == owner.image, "Display image is not the same as the original one: dispose is necessary");

            //            // not freeing the display image because it is always the original image here
            //            displayImage = result;
            //            owner.Invalidate();
            //        }
            //        finally
            //        {
            //            task.Dispose();
            //            activeTask = null;
            //        }
            //    }
            //}

            //private static Bitmap? GenerateMetafilePreview(GenerateTask task)
            //{
            //    // For the resizing large managed buffer of source.Height * target.Width of ColorF (16 bytes) is allocated internally. To be safe we count with the doubled sizes.
            //    Size doubledSize = new Size(task.Size.Width << 1, task.Size.Height << 1);
            //    long managedPressure = doubledSize.Width * doubledSize.Height * 16;
            //    if (!MemoryHelper.CanAllocate(managedPressure))
            //    {
            //        Debug.WriteLine($"Discarding task because there is no {managedPressure:N0} bytes of available managed memory");
            //        task.IsCanceled = true;
            //    }

            //    if (task.IsCanceled)
            //        return null;
            //    // MetafileExtensions.ToBitmap does the same if anti aliasing is requested but this way the process can be canceled
            //    Debug.WriteLine($"Generating anti aliased image {task.Size.Width}x{task.Size.Height} on thread #{Thread.CurrentThread.ManagedThreadId}");
            //    Bitmap? result = null;
            //    Bitmap? doubled = null;
            //    try
            //    {
            //        doubled = new Bitmap(task.SourceImage!, task.Size.Width << 1, task.Size.Height << 1);
            //        if (!task.IsCanceled)
            //        {
            //            result = new Bitmap(task.Size.Width, task.Size.Height, PixelFormat.Format32bppPArgb);
            //            using IReadableBitmapData src = doubled.GetReadableBitmapData();
            //            using IReadWriteBitmapData dst = result.GetReadWriteBitmapData();

            //            // not using Task and await, because this method's signature must match the WaitCallback delegate, and we want to be compatible with .NET 3.5, too
            //            IAsyncResult asyncResult = src.BeginDrawInto(dst, new Rectangle(Point.Empty, doubled.Size), new Rectangle(Point.Empty, result.Size),
            //                // ReSharper disable once AccessToModifiedClosure - intended, if IsCanceled is modified we need to return its modified value
            //                asyncConfig: new AsyncConfig { IsCancelRequestedCallback = () => task.IsCanceled, ThrowIfCanceled = false });

            //            // As we are already on a pool thread this is not a UI blocking call
            //            // This will throw an exception if resizing failed (resizing also allocates a large amount of memory).
            //            asyncResult.EndDrawInto();
            //        }
            //    }
            //    catch (Exception e) when (!e.IsCriticalGdi())
            //    {
            //        // Despite all of the preconditions the memory could not be allocated or some other error occurred (yes, we catch even OutOfMemoryException here)
            //        // NOTE: practically we always can recover from here: we simply don't use a generated preview and the worker thread can be finished
            //        task.IsCanceled = true;
            //    }
            //    finally
            //    {
            //        doubled?.Dispose();
            //        if (task.IsCanceled)
            //        {
            //            result?.Dispose();
            //            result = null;
            //        }
            //    }

            //    return result;
            //}

            //private static Bitmap? GenerateBitmapPreview(GenerateTask task)
            //{
            //    // BitmapExtensions.Resize does the same but this way the process can be canceled
            //    Debug.WriteLine($"Generating smoothed image {task.Size.Width}x{task.Size.Height} on thread #{Thread.CurrentThread.ManagedThreadId}");

            //    Bitmap? result = null;
            //    try
            //    {
            //        result = new Bitmap(task.Size.Width, task.Size.Height, PixelFormat.Format32bppPArgb);
            //        using IReadableBitmapData src = ((Bitmap)task.SourceImage!).GetReadableBitmapData();
            //        using IReadWriteBitmapData dst = result.GetReadWriteBitmapData();
            //        var cfg = new AsyncConfig { IsCancelRequestedCallback = () => task.IsCanceled, ThrowIfCanceled = false, MaxDegreeOfParallelism = Environment.ProcessorCount >> 1 };

            //        // Not using Task and await, because this method's signature must match the WaitCallback delegate, and we want to be compatible with .NET 3.5, too.
            //        // As we are already on a pool thread the End... call does not block the UI.
            //        var srcRect = new Rectangle(Point.Empty, task.SourceImage!.Size);
            //        var dstRect = new Rectangle(Point.Empty, task.Size);
            //        if (srcRect == dstRect)
            //        {
            //            IAsyncResult asyncResult = src.BeginCopyTo(dst, srcRect, Point.Empty, asyncConfig: cfg);
            //            asyncResult.EndCopyTo();
            //        }
            //        else
            //        {
            //            IAsyncResult asyncResult = src.BeginDrawInto(dst, srcRect, dstRect, asyncConfig: cfg);
            //            asyncResult.EndDrawInto();
            //        }
            //    }
            //    catch (Exception e) when (!e.IsCriticalGdi())
            //    {
            //        // Despite all of the preconditions the memory could not be allocated or some other error occurred (yes, we catch even OutOfMemoryException here)
            //        // NOTE: practically we always can recover from here: we simply don't use a generated preview and the worker thread can be finished
            //        task.IsCanceled = true;
            //    }
            //    finally
            //    {
            //        if (task.IsCanceled)
            //        {
            //            result?.Dispose();
            //            result = null;
            //        }
            //    }

            //    return result;
            //}

            #endregion

            #endregion

            #endregion
        }

        #endregion
    }
}