#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageViewer.DisplayImageGenerator.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2024 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;

using KGySoft.CoreLibraries;
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Reflection;
using KGySoft.Threading;

#endregion

#region Suppressions

#pragma warning disable CS1690 // Accessing a member on a field of a marshal-by-reference class may cause a runtime exception - false alarm, ImageViewer is never a remote object.

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

                internal Bitmap SourceBitmap = default!;
                internal bool InvalidateOwner;

                #endregion
            }

            #endregion

            #region GenerateResizedImageTask class

            private sealed class GenerateResizedImageTask : AsyncTaskBase
            {
                #region Fields

                internal Image SourceImage = default!;
                internal Size Size;

                #endregion
            }

            #endregion

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
            /// These formats are so slow that it is still faster to generate a 32bpp clone first than display them directly.
            /// </summary>
            private static readonly PixelFormat[] slowFormats = OSUtils.IsWindows
                ? new[] { PixelFormat.Format48bppRgb, PixelFormat.Format64bppArgb, PixelFormat.Format64bppArgb }
                : Reflector.EmptyArray<PixelFormat>();

            #endregion

            #region Instance Fields

            private readonly ImageViewer owner;

            private volatile bool disposed;

            /// <summary>
            /// true if generator can generate new content. Turned off on low memory or by <see cref="Free"/>. Invalidating the image enables it again.
            /// </summary>
            private bool enabled;

            private GenerateDefaultImageTask? generateDefaultImageTask;
            private GenerateResizedImageTask? generateResizedImageTask;

            /// <summary>
            /// The default image to be displayed when no resized display image is needed or while its generation is in progress.
            /// Set by <see cref="GenerateDefaultImage"/>. If <see cref="isDefaultImageCloned"/> is true, then contains
            /// - A fast PARGB32 clone of the original image it that is a Bitmap
            /// - A clone of the original image if that is a Metafile so the original image will not be blocked to generate resized images
            /// Otherwise, it is the same reference as the owner.Image.
            /// If <see cref="enabled"/> is false, then may contain the original image even if it cannot be displayed.
            /// </summary>
            private volatile Image? defaultDisplayImage;
            private volatile bool isDefaultImageCloned;

            /// <summary>
            /// If not null, contains the last cached size-adjusted display image.
            /// It is not disposed immediately when a new size (<see cref="requestedSize"/>) is started to be generated
            /// so it can be re-used when toggling smooth zooming.
            /// </summary>
            private volatile Bitmap? resizedDisplayImage;

            /// <summary>
            /// Just to cache <see cref="resizedDisplayImage"/>.Size,
            /// because accessing it on <see cref="resizedDisplayImage"/> without locking can lead to "object is used elsewhere" error.
            /// </summary>
            private Size resizedDisplayImageSize;

            /// <summary>
            /// The currently requested size of the size adjusted image. If it is the same as <see cref="resizedDisplayImageSize"/>,
            /// then <see cref="resizedDisplayImage"/> can be displayed.
            /// </summary>
            private Size requestedSize;

            #endregion

            #endregion

            #region Properties

            // This is alright, this class is private.
            internal object SyncRoot => this;

            #endregion

            #region Constructors

            internal DisplayImageGenerator(ImageViewer owner) => this.owner = owner;

            #endregion

            #region Methods

            #region Static Methods

            private static void CancelRunningGenerate(AsyncTaskBase? task)
            {
                if (task == null)
                    return;
                task.IsCanceled = true;
            }

            private static void WaitForPendingGenerate(AsyncTaskBase? task)
            {
                if (task == null)
                    return;
                task.WaitForCompletion();
                task.Dispose();
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

            internal void InvalidateImages()
            {
                // This cancels all tasks and disposes every generating resources
                Free();
                Debug.Assert(generateDefaultImageTask?.IsCanceled != false && generateResizedImageTask?.IsCanceled != false);

                // (Re-)enabling generating images
                enabled = true;
            }

            internal void InvalidateDisplayImage()
            {
                // Just canceling possible running generate. Not even clearing the possible already generated image.
                // A new task will be started if a new paint explicitly requires it, in which case the last image can be re-used if possible.
                CancelRunningGenerate(generateResizedImageTask);
            }

            internal (Image?, InterpolationMode) GetDisplayImage()
            {
                Debug.Assert(owner.image != null);
                InterpolationMode interpolationMode = InterpolationMode.NearestNeighbor;

                // 1.) Returning with a size adjusted display image
                if (owner.smoothZooming && resizedDisplayImageSize == owner.targetRectangle.Size)
                    return (resizedDisplayImage, interpolationMode);

                // 2.) Checking if there is an already available default image. It might have to be resized on painting.
                Image? result = defaultDisplayImage;

                // Smoothing Bitmap: leaving NearestNeighbor if a resized image is expected to be generated;
                // otherwise, using some interpolation to be applied during painting
                if (!owner.isMetafile && owner.smoothZooming)
                {
                    float zoom = owner.zoom;
                    Size size = owner.imageSize;

                    // >4x zoom or shrunk image that is not greater than generating threshold: using HighQualityBicubic 
                    if (zoom >= 4f || zoom < 1f && size.Width <= sizeThreshold && size.Height <= sizeThreshold)
                        interpolationMode = InterpolationMode.HighQualityBicubic;
                    // 1-4x zoom: HighQualityBilinear for large images to prevent heavy lagging; otherwise, HighQualityBicubic
                    else if (zoom > 1f)
                        interpolationMode = size.Width > sizeThreshold || size.Height > sizeThreshold ? InterpolationMode.HighQualityBilinear : InterpolationMode.HighQualityBicubic;
                    // Shrinking of larger images if generating is disabled: applying a hopefully-not-too-slow fallback interpolation
                    else if (!enabled && zoom < 1f)
                        interpolationMode = owner.targetRectangle.Width > sizeThreshold || owner.targetRectangle.Height > sizeThreshold ? InterpolationMode.Bilinear : InterpolationMode.Bicubic;
                }

                // 3.) Starting to generate cached images if needed
                if (enabled)
                {
                    if (result == null)
                        BeginGenerateDefaultDisplayImageIfNeeded();

                    BeginGenerateResizedDisplayImageIfNeeded();
                }

                // 4.) Returning either a generated display or the original image
                if (result != null)
                    // here we already have a default display image we can return with
                    return (result, interpolationMode);

                // Waiting for the display image to be generated if pixel format is not supported,
                // or it is so slow (>= 48bpp) that it is faster to wait for the converted image and paint the existing one.
                // Note: Not using async because this project targets also .NET 3.5 and the image is locked anyway also in paint
                if (owner.pixelFormat.In(unsupportedFormats) || owner.pixelFormat.In(slowFormats))
                    generateDefaultImageTask?.WaitForCompletion();

                // Too low memory: turning off image generation and freeing up resources.
                if (!enabled)
                {
                    Free();

                    // Assigning by original image to defaultDisplayImage so even large >= 48bpp images will be drawn directly.
                    defaultDisplayImage = owner.image;
                }

                // Unless a default image has been generated in the meantime we return with the original image, or null, if its pixel format is not supported.
                result = defaultDisplayImage ?? owner.image;
                if (ReferenceEquals(result, owner.image) && owner.pixelFormat.In(unsupportedFormats))
                    result = null;

                return (result, interpolationMode);
            }

            #endregion

            #region Private Methods

            private void Free()
            {
                // disabling to prevent starting new tasks while freeing resources
                enabled = false;
                CancelRunningGenerate(generateDefaultImageTask);
                CancelRunningGenerate(generateResizedImageTask);

                WaitForPendingGenerate(generateDefaultImageTask);
                WaitForPendingGenerate(generateResizedImageTask);

                requestedSize = default;

                lock (SyncRoot)
                {
                    if (isDefaultImageCloned)
                        defaultDisplayImage?.Dispose();
                    defaultDisplayImage = null;
                    isDefaultImageCloned = false;

                    resizedDisplayImageSize = default;
                    resizedDisplayImage?.Dispose();
                    resizedDisplayImage = null;
                }
            }

            private void BeginGenerateDefaultDisplayImageIfNeeded()
            {
                Debug.Assert(owner.image != null && owner.pixelFormat != default);

                // A task is already running or the display image is already generated.
                if (isDefaultImageCloned || generateDefaultImageTask != null)
                    return;

                Image image = owner.image!;
                Bitmap? bitmap = image as Bitmap;

                // Metafile: The default image is the same as the original. If anti-aliased images are required, a clone is created on demand from that task
                // Bitmap: generating a new default image for unsupported formats,
                bool isGenerateNeeded = bitmap != null && (owner.pixelFormat.In(unsupportedFormats)
                    // for non-PARGB32 images larger than 256x256 - note: leaving even slow formats unconverted below sizeThreshold / 4
                    || owner.pixelFormat != PixelFormat.Format32bppPArgb && (owner.imageSize.Width > sizeThreshold >> 2 || owner.imageSize.Height > sizeThreshold >> 2)
                    // and for native icons: converting because icons are handled oddly by GDI+, for example, the first column has half pixel width
                    || bitmap.RawFormat.Guid == ImageFormat.Icon.Guid);

                if (!isGenerateNeeded)
                {
                    // A generated default image is set from another thread so handling possible concurrency.
                    if (defaultDisplayImage == null)
                        Interlocked.CompareExchange(ref defaultDisplayImage, image, null);
                    return;
                }

                var task = new GenerateDefaultImageTask
                {
                    SourceBitmap = bitmap!,
                    InvalidateOwner = bitmap!.RawFormat.Guid == ImageFormat.Icon.Guid
                };
                generateDefaultImageTask = task;
                ThreadPool.QueueUserWorkItem(GenerateDefaultImage!, task);
            }

            private void BeginGenerateResizedDisplayImageIfNeeded()
            {
                Debug.Assert(owner.image != null && owner.pixelFormat != default);

                // Metafile: If smoothing edges is enabled
                // Bitmap: If smoothing resize is enabled, the image is shrunk and image size is larger than 1024x1024
                bool isGenerateNeeded = owner.smoothZooming && (owner.isMetafile
                    || owner.zoom < 1f && (owner.imageSize.Width > sizeThreshold || owner.imageSize.Height > sizeThreshold));

                // Not canceling the possible generate task here. It will call an invalidate in the end and we can see whether we use the result.
                Size size = owner.targetRectangle.Size;
                if (!isGenerateNeeded || size.Width < 1 || size.Height < 1)
                {
                    requestedSize = default;
                    return;
                }

                requestedSize = size;
                GenerateResizedImageTask? task = generateResizedImageTask;
                if (task != null)
                {
                    // If there is already a running generate task
                    if (!task.IsCanceled)
                    {
                        // It is generating the same size: we keep it
                        if (task.Size == size)
                            return;

                        // We just initiate cancellation but not awaiting the completion.
                        task.IsCanceled = true;
                    }

                    // We do not await the task (we are in a lock here that is used in the task, too).
                    // Instead, we invalidate the owner so another paint will be triggered some time later. Hopefully the task will have been finished by that time.
                    owner.Invalidate();
                    return;
                }

                Debug.Assert(generateResizedImageTask == null);
                task = new GenerateResizedImageTask
                {
                    SourceImage = owner.image!,
                    Size = size
                };

                generateResizedImageTask = task;
                ThreadPool.QueueUserWorkItem(GenerateResizedImage!, task);
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
                        task.IsCanceled = true;

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

                        // As we are already on a pool thread the End... call does not block the UI. It's still not the same as CopyTo() due to cancellation support.
                        asyncResult.EndCopyTo();
                    }
                    catch (Exception)
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
                            result?.Dispose();
                            result = null;
                        }
                    }

                    return result;
                }

                #endregion

                var task = (GenerateDefaultImageTask)state;

                try
                {
                    // canceled or lost race
                    if (task.IsCanceled || isDefaultImageCloned || task.SourceBitmap != owner.image || !enabled || disposed)
                        return;

                    Bitmap? result = null;

                    // Locking on the image to avoid the possible "bitmap region is already locked" issue.
                    // Until the default image is generated, it is locked during the paint, too.
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

            private void GenerateResizedImage(object state)
            {
                #region Local Methods

                static Bitmap? GenerateResizedMetafile(GenerateResizedImageTask task, ref bool enabled)
                {
                    // For the resizing large managed buffer of source.Height * target.Width of ColorF (16 bytes) is allocated internally. To be safe we count with the doubled sizes.
                    Size doubledSize = new Size(task.Size.Width << 1, task.Size.Height << 1);
                    long managedPressure = doubledSize.Width * doubledSize.Height * 16;
                    if (!MemoryHelper.CanAllocate(managedPressure))
                        task.IsCanceled = true;

                    if (task.IsCanceled)
                        return null;

                    // MetafileExtensions.ToBitmap does the same if anti aliasing is requested but this way the process can be canceled
                    Bitmap? result = null;
                    Bitmap? doubled = null;
                    try
                    {
                        doubled = new Bitmap(task.SourceImage, task.Size.Width << 1, task.Size.Height << 1);

                        if (!task.IsCanceled)
                        {
                            result = new Bitmap(task.Size.Width, task.Size.Height, PixelFormat.Format32bppPArgb);
                            using IReadableBitmapData src = doubled.GetReadableBitmapData();
                            using IReadWriteBitmapData dst = result.GetReadWriteBitmapData();

                            // not using Task and await we want to be compatible with .NET 3.5
                            IAsyncResult asyncResult = src.BeginDrawInto(dst, 
                                new Rectangle(Point.Empty, doubled.Size),
                                new Rectangle(Point.Empty, task.Size),
                                asyncConfig: new AsyncConfig
                                {
                                    IsCancelRequestedCallback = () => task.IsCanceled,
                                    ThrowIfCanceled = false,
                                    MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 2)
                                });

                            // As we are already on a pool thread this is not a UI blocking call
                            asyncResult.EndDrawInto();
                        }
                    }
                    catch (Exception)
                    {
                        // Despite all of the preconditions the memory could not be allocated or some other error occurred (yes, we catch even OutOfMemoryException here)
                        // NOTE: practically we always can recover from here: we simply don't use a generated preview and the worker thread can be finished
                        task.IsCanceled = true;
                        enabled = false;
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

                static Bitmap? GenerateResizedBitmap(GenerateResizedImageTask task, ref bool enabled)
                {
                    // BitmapExtensions.Resize does the same but this way the process can be canceled
                    Bitmap? result = null;
                    try
                    {
                        result = new Bitmap(task.Size.Width, task.Size.Height, PixelFormat.Format32bppPArgb);
                        lock (task.SourceImage)
                        {
                            using IReadableBitmapData src = ((Bitmap)task.SourceImage).GetReadableBitmapData();
                            using IReadWriteBitmapData dst = result.GetReadWriteBitmapData();

                            // not using Task and await we want to be compatible with .NET 3.5
                            IAsyncResult asyncResult = src.BeginDrawInto(dst,
                                new Rectangle(Point.Empty, task.SourceImage.Size),
                                new Rectangle(Point.Empty, task.Size), 
                                asyncConfig: new AsyncConfig
                                {
                                    IsCancelRequestedCallback = () => task.IsCanceled,
                                    ThrowIfCanceled = false,
                                    MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 2)
                                });

                            // As we are already on a pool thread the End... call does not block the UI.
                            asyncResult.EndDrawInto();
                        }
                    }
                    catch (Exception)
                    {
                        // Despite all of the preconditions the memory could not be allocated or some other error occurred (yes, we catch even OutOfMemoryException here)
                        // NOTE: practically we always can recover from here: we simply don't use a generated preview and the worker thread can be finished
                        task.IsCanceled = true;
                        enabled = false;
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

                var task = (GenerateResizedImageTask)state;

                try
                {
                    // canceled or lost race
                    if (task.IsCanceled || task.SourceImage != owner.image || task.Size != requestedSize || !enabled || disposed)
                        return;

                    // returning if we already have the result
                    if (task.Size == resizedDisplayImageSize)
                    {
                        owner.Invalidate();
                        return;
                    }

                    // Before creating the preview releasing previous cached result. It is important to free it here, before checking the free memory.
                    // The lock ensures that no disposed image is displayed
                    lock (SyncRoot)
                    {
                        resizedDisplayImageSize = default;
                        resizedDisplayImage?.Dispose();
                        resizedDisplayImage = null;
                    }

                    // 1.) If there is no cloned display image generating that one first so the UI can use that while the original image will be free to create the resized images from.
                    if (!isDefaultImageCloned)
                    {
                        // The clone is just being generated. Invalidating and returning to come back later.
                        if (defaultDisplayImage == null || generateDefaultImageTask != null)
                        {
                            owner.Invalidate();
                            return;
                        }

                        Debug.Assert(ReferenceEquals(owner.image, defaultDisplayImage), "If isDefaultImageCloned is false, then defaultDisplayImage is expected to be the original instance here.");
                        Debug.Assert(owner.isMetafile || owner.pixelFormat == PixelFormat.Format32bppPArgb, "Clone is expected to be missing for metafiles and 32bpp PARGB bitmaps only.");
                        Image clone;

                        // This may block the UI in OnPaint but once the clone is created OnPaint will use that instead of the original image.
                        lock (task.SourceImage)
                        {
                            try
                            {
                                // we do not allow canceling this part because this would be started again and again
                                clone = owner.image is Bitmap bitmap
                                    ? bitmap.ConvertPixelFormat(PixelFormat.Format32bppPArgb)
                                    : (Image)owner.image.Clone();
                            }
                            catch (Exception)
                            {
                                enabled = false;
                                return;
                            }
                        }

                        defaultDisplayImage = clone;
                        isDefaultImageCloned = true;
                    }

                    if (task.IsCanceled)
                        return;

                    // 2.) Generating the size-adjusted display image
                    Bitmap? result = null;
                    try
                    {
                        if (!task.IsCanceled)
                            result = task.SourceImage is Metafile ? GenerateResizedMetafile(task, ref enabled) : GenerateResizedBitmap(task, ref enabled);
                    }
                    finally
                    {
                        task.SetCompleted();
                    }

                    // setting latest cache (even if the task has been canceled as we have a completed result)
                    if (result != null)
                    {
                        resizedDisplayImage = result;
                        resizedDisplayImageSize = task.Size;
                    }

                    if (task.IsCanceled)
                        return;

                    owner.Invalidate();
                }
                finally
                {
                    task.Dispose();
                    generateResizedImageTask = null;
                }
            }

            #endregion

            #endregion

            #endregion
        }

        #endregion
    }
}