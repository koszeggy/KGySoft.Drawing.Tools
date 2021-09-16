#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ResizeBitmapViewModel.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.Imaging;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class ResizeBitmapViewModel : TransformBitmapViewModelBase
    {
        #region Nested classes

        private sealed class ResizeTask : GenerateTaskBase
        {
            #region Fields

            private Bitmap? sourceBitmap;
            private Bitmap? targetBitmap;
            private bool isSourceCloned;
            private IReadableBitmapData? sourceBitmapData;
            private IReadWriteBitmapData? targetBitmapData;

            #endregion

            #region Properties

            internal Size Size { get; }
            internal ScalingMode ScalingMode { get; }

            #endregion

            #region Constructors

            internal ResizeTask(Size size, ScalingMode scalingMode)
            {
                Size = size;
                ScalingMode = scalingMode;
            }

            #endregion

            #region Methods

            #region Internal Methods

            internal override void Initialize(Bitmap source, bool isInUse)
            {
                // this must be the first line to prevent disposing source if next lines fail
                isSourceCloned = isInUse;
                PixelFormat origPixelFormat = source.PixelFormat;
                PixelFormat pixelFormat = origPixelFormat.HasAlpha() || origPixelFormat.IsIndexed() && source.Palette.Entries.Any(c => c.A != Byte.MaxValue)
                    ? PixelFormat.Format32bppPArgb
                    : PixelFormat.Format24bppRgb;
                targetBitmap = new Bitmap(Size.Width, Size.Height, pixelFormat);
                targetBitmapData = targetBitmap.GetReadWriteBitmapData();

                // Locking on source image to avoid "bitmap region is already locked" if the UI is painting the image when we clone it.
                // This works this way because UI can repaint the image any time and is also locks the image for that period.
                // Another solution could be if we used a clone of the original image but it is better to avoid using multiple clones.
                if (isInUse)
                {
                    // if image is in use (in the view of this VM) we lock it only for a short time to prevent the UI freezing
                    lock (source)
                        sourceBitmap = source.CloneCurrentFrame();
                }
                else
                {
                    // If no direct use could be detected using a long-term lock to spare a clone.
                    // It is still needed because the image still can be used in the main V/VM.
                    Monitor.Enter(source);
                    sourceBitmap = source;
                }

                sourceBitmapData = sourceBitmap.GetReadableBitmapData();
            }

            internal override IAsyncResult BeginGenerate(AsyncConfig asyncConfig)
                => sourceBitmapData!.BeginDrawInto(targetBitmapData!, new Rectangle(0, 0, sourceBitmapData!.Width, sourceBitmapData.Height),
                    new Rectangle(0, 0, targetBitmapData!.Width, targetBitmapData.Height),
                    scalingMode: ScalingMode, asyncConfig: asyncConfig);

            internal override Bitmap? EndGenerate(IAsyncResult asyncResult)
            {
                asyncResult.EndDrawInto();

                // If there was no exception returning result and clearing the field to prevent disposing.
                // The caller will take care of disposing if the operation was canceled and the result is discarded.
                Bitmap? bmp = targetBitmap;
                targetBitmap = null;
                return bmp;
            }

            internal override void SetCompleted()
            {
                sourceBitmapData?.Dispose();
                sourceBitmapData = null;
                if (isSourceCloned)
                    sourceBitmap?.Dispose();
                else if (sourceBitmap != null)
                    Monitor.Exit(sourceBitmap);
                sourceBitmap = null;

                targetBitmapData?.Dispose();
                targetBitmapData = null;
                base.SetCompleted();
            }

            #endregion

            #region Protected Methods

            protected override void Dispose(bool disposing)
            {
                if (IsDisposed)
                    return;
                if (disposing)
                {
                    sourceBitmapData?.Dispose();
                    if (isSourceCloned)
                        sourceBitmap?.Dispose();
                    targetBitmapData?.Dispose();
                    targetBitmap?.Dispose();
                }

                base.Dispose(disposing);
            }

            #endregion

            #endregion
        }

        #endregion

        #region Fields

        private readonly float aspectRatio;

        private Size originalSize;
        private bool adjustingWidth;
        private bool adjustingHeight;

        #endregion

        #region Properties

        #region Public Properties

        // The binding needs these to be public
        public int Width { get => Get(originalSize.Width); set => Set(value); }
        public int Height { get => Get(originalSize.Height); set => Set(value); }
        public float WidthRatio { get => Get(1f); set => Set(value); }
        public float HeightRatio { get => Get(1f); set => Set(value); }

        #endregion

        #region Internal Properties

        internal bool KeepAspectRatio { get => Get(true); set => Set(value); }
        internal bool ByPercentage { get => Get(true); set => Set(value); }
        internal bool ByPixels { get => Get(false); set => Set(value); }
        internal ScalingMode[] ScalingModes => Get(Enum<ScalingMode>.GetValues);
        internal ScalingMode ScalingMode { get => Get<ScalingMode>(); set => Set(value); }

        #endregion

        #region Protected Properties

        protected override bool AreSettingsChanged => !KeepAspectRatio || Width != originalSize.Width || Height != originalSize.Height || ScalingMode != ScalingMode.Auto;

        #endregion

        #endregion

        #region Constructors

        internal ResizeBitmapViewModel(Bitmap image) : base(image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image), PublicResources.ArgumentNull);
            originalSize = image.Size;
            aspectRatio = (float)originalSize.Width / originalSize.Height;
        }

        #endregion

        #region Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            switch (e.PropertyName)
            {
                case nameof(KeepAspectRatio):
                    if (e.NewValue is false)
                        break;
                    Width = (int)(Height * aspectRatio);
                    break;

                case nameof(ByPercentage):
                    if (e.NewValue is true)
                        ByPixels = false;
                    break;

                case nameof(ByPixels):
                    if (e.NewValue is true)
                        ByPercentage = false;
                    break;

                case nameof(Width):
                    if (!ByPixels)
                        break;
                    int width = (int)e.NewValue!;
                    WidthRatio = width <= 0 ? 0f : (float)width / originalSize.Width;
                    if (!KeepAspectRatio || adjustingWidth)
                        break;
                    adjustingHeight = true;
                    try
                    {
                        Height = (int)(width / aspectRatio);
                    }
                    finally
                    {
                        adjustingHeight = false;
                    }

                    break;

                case nameof(WidthRatio):
                    if (!ByPercentage)
                        break;
                    float widthRatio = (float)e.NewValue!;
                    Width = widthRatio <= 0f ? 0 : (int)(originalSize.Width * widthRatio);
                    if (!KeepAspectRatio || adjustingWidth)
                        break;
                    adjustingHeight = true;
                    try
                    {
                        HeightRatio = widthRatio;
                    }
                    finally
                    {
                        adjustingHeight = false;
                    }

                    break;

                case nameof(Height):
                    if (!ByPixels)
                        break;
                    int height = (int)e.NewValue!;
                    HeightRatio = height <= 0 ? 0f : (float)height / originalSize.Height;
                    if (!KeepAspectRatio || adjustingHeight)
                        break;
                    adjustingWidth = true;
                    try
                    {
                        Width = (int)(Height * aspectRatio);
                    }
                    finally
                    {
                        adjustingWidth = false;
                    }

                    break;

                case nameof(HeightRatio):
                    if (!ByPercentage)
                        break;
                    float heightRatio = (float)e.NewValue!;
                    Height = heightRatio <= 0f ? 0 : (int)(originalSize.Height * heightRatio);
                    if (!KeepAspectRatio || adjustingHeight)
                        break;
                    adjustingWidth = true;
                    try
                    {
                        WidthRatio = heightRatio;
                    }
                    finally
                    {
                        adjustingWidth = false;
                    }

                    break;
            }
        }

        protected override ValidationResultsCollection DoValidation()
        {
            ValidationResultsCollection result = base.DoValidation();
            if (Width <= 0)
                result.AddError(nameof(Width), Res.ErrorMessageValueMustBeGreaterThan(0f));
            if (Height <= 0)
                result.AddError(nameof(Height), Res.ErrorMessageValueMustBeGreaterThan(0f));
            return result;
        }

        protected override bool AffectsPreview(string propertyName)
            => propertyName.In(nameof(Width), nameof(Height), nameof(ScalingMode));

        protected override GenerateTaskBase CreateGenerateTask()
            => new ResizeTask(new Size(Width, Height), ScalingMode);

        protected override bool MatchesSettings(GenerateTaskBase task)
        {
            var t = (ResizeTask)task;
            return t.Size == new Size(Width, Height) && t.ScalingMode == ScalingMode;
        }

        protected override bool MatchesOriginal(GenerateTaskBase task)
        {
            var t = (ResizeTask)task;
            return t.Size == originalSize;
        }

        #endregion
    }
}
