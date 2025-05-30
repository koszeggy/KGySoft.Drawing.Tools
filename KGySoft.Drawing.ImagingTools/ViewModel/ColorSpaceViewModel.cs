﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorSpaceViewModel.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2025 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.Imaging;
using KGySoft.Threading;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class ColorSpaceViewModel : TransformBitmapViewModelBase
    {
        #region Nested Classes

        private sealed class GenerateTask : GenerateTaskBase
        {
            #region Fields

            private Bitmap? sourceBitmap;
            private bool isSourceCloned;

            #endregion

            #region Properties

            internal PixelFormat PixelFormat { get; }
            internal IQuantizer? Quantizer { get; }
            internal IDitherer? Ditherer { get; }

            #endregion

            #region Constructors

            internal GenerateTask(PixelFormat pixelFormat, IQuantizer? quantizer, IDitherer? ditherer)
            {
                PixelFormat = pixelFormat;
                Quantizer = quantizer;
                Ditherer = ditherer;
            }

            #endregion

            #region Methods

            internal override void Initialize(Bitmap source, bool isInUse)
            {
                // Locking on source image to avoid "bitmap region is already locked" if the UI is painting the image when we clone it.
                // This works this way because UI can repaint the image any time and is also locks the image for that period.
                // Another solution could be if we used a clone of the original image but it is better to avoid using multiple clones.
                isSourceCloned = isInUse;
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
            }

            internal override IAsyncResult BeginGenerate(AsyncConfig asyncConfig)
                => sourceBitmap!.BeginConvertPixelFormat(PixelFormat, Quantizer, Ditherer, asyncConfig);

            internal override Bitmap? EndGenerate(IAsyncResult asyncResult) => asyncResult.EndConvertPixelFormat();

            internal override void SetCompleted()
            {
                if (sourceBitmap != null)
                {
                    if (isSourceCloned)
                    {
                        sourceBitmap.Dispose();
                        sourceBitmap = null;
                    }
                    else
                        Monitor.Exit(sourceBitmap);
                }

                base.SetCompleted();
            }

            protected override void Dispose(bool disposing)
            {
                if (IsDisposed)
                    return;
                if (disposing)
                {
                    if (isSourceCloned)
                        sourceBitmap?.Dispose();
                }

                base.Dispose(disposing);
            }

            #endregion
        }

        #endregion

        #region Fields

        private readonly PixelFormat originalPixelFormat;
        private readonly bool originalHasAlpha;

        #endregion

        #region Properties

        #region Internal Properties

        internal QuantizerSelectorViewModel QuantizerSelectorViewModel => Get(() => new QuantizerSelectorViewModel());
        internal DithererSelectorViewModel DithererSelectorViewModel => Get(() => new DithererSelectorViewModel());

        // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
        internal PixelFormat[] PixelFormats => Get(() => Enum<PixelFormat>.GetValues().Where(pf => pf.IsValidFormat()).OrderBy(pf => pf & PixelFormat.Max).ToArray());
        internal PixelFormat PixelFormat { get => Get<PixelFormat>(); set => Set(value); }

        internal bool ChangePixelFormat { get => Get<bool>(); set => Set(value); }
        internal bool UseQuantizer { get => Get<bool>(); set => Set(value); }
        internal bool UseDitherer { get => Get<bool>(); set => Set(value); }

        #endregion

        #region Protected Properties

        protected override bool AreSettingsChanged => ChangePixelFormat || UseQuantizer || UseDitherer;

        #endregion

        #region Private Properties

        private PixelFormat OriginalPixelFormat => originalPixelFormat == PixelFormatExtensions.Format32bppCmyk ? PixelFormat.Format24bppRgb : originalPixelFormat;

        #endregion

        #endregion

        #region Constructors

        internal ColorSpaceViewModel(Bitmap image) : base(image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image), PublicResources.ArgumentNull);
            originalPixelFormat = image.PixelFormat;
            originalHasAlpha = originalPixelFormat.HasAlpha() || originalPixelFormat.IsIndexed() && image.Palette.Entries.Any(c => c.A < Byte.MaxValue);
            QuantizerSelectorViewModel.PropertyChanged += Selector_PropertyChanged;
            DithererSelectorViewModel.PropertyChanged += Selector_PropertyChanged;

            PixelFormat = PixelFormats[0];
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            switch (e.PropertyName)
            {
                case nameof(PixelFormat):
                case nameof(ChangePixelFormat):
                    QuantizerSelectorViewModel.PixelFormat = ChangePixelFormat ? PixelFormat : originalPixelFormat;
                    break;
                case nameof(UseDitherer):
                    QuantizerSelectorViewModel.UseDithering = (bool)e.NewValue!;
                    break;
            }
        }

        protected override ValidationResultsCollection DoValidation()
        {
            ValidationResultsCollection result = base.DoValidation();

            // Note that !UseQuantizer and Quantizer == null are not interchangeable.
            // UseQuantizer indicates whether the selector is checked while Quantizer is not null if an instance could be successfully created
            bool changePixelFormat = ChangePixelFormat;
            bool useQuantizer = UseQuantizer;
            bool useDitherer = UseDitherer;
            PixelFormat pixelFormat = changePixelFormat ? PixelFormat : originalPixelFormat;
            IQuantizer? quantizer = useQuantizer ? QuantizerSelectorViewModel.Quantizer : null;
            IDitherer? ditherer = useDitherer ? DithererSelectorViewModel.Ditherer : null;
            Exception? quantizerError = useQuantizer ? QuantizerSelectorViewModel.CreateQuantizerError : null;
            Exception? dithererError = useDitherer ? DithererSelectorViewModel.CreateDithererError : null;
            int bpp = pixelFormat.ToBitsPerPixel();
            int originalBpp = originalPixelFormat.ToBitsPerPixel();
            int? bppHint = quantizer?.PixelFormatHint.ToBitsPerPixel();
            PixelFormat quantizerPixelFormat = quantizer == null ? default : quantizer.PixelFormatHint.ToPixelFormat();

            // errors
            if (pixelFormat != originalPixelFormat && !pixelFormat.IsSupportedNatively()) // IsSupportedNatively returns false for CMYK, that's why the check for the original
                result.AddError(nameof(PixelFormat), Res.ErrorMessagePixelFormatNotSupported(pixelFormat));

            if (quantizerError != null)
                result.AddError(nameof(QuantizerSelectorViewModel.Quantizer), Res.ErrorMessageFailedToInitializeQuantizer(quantizerError.Message));
            else if (bppHint <= 8 && bppHint > bpp)
                result.AddError(nameof(QuantizerSelectorViewModel.Quantizer), Res.ErrorMessageQuantizerPaletteTooLarge(pixelFormat, quantizerPixelFormat, 1 << bpp));

            if (dithererError != null)
                result.AddError(nameof(DithererSelectorViewModel.Ditherer), Res.ErrorMessageFailedToInitializeDitherer(dithererError.Message));

            //if (result.HasErrors)
            //    return result;

            // warnings
            if (!useQuantizer && originalPixelFormat != pixelFormat && originalPixelFormat.IsWide() && bpp > 32)
                result.AddWarning(nameof(PixelFormat), Res.WarningMessageWideConversionLoss(originalPixelFormat));

            if (bppHint > bpp)
                result.AddWarning(nameof(QuantizerSelectorViewModel.Quantizer), Res.WarningMessageQuantizerTooWide(pixelFormat, quantizerPixelFormat));

            if (bppHint == 32 && ditherer != null)
                result.AddWarning(nameof(DithererSelectorViewModel.Ditherer), Res.WarningMessageDithererNoAlphaGradient);

            // information
            if (changePixelFormat && pixelFormat == originalPixelFormat)
                result.AddInfo(nameof(PixelFormat), Res.InfoMessageSamePixelFormat);
            if (bppHint < bpp)
                result.AddInfo(nameof(PixelFormat), Res.InfoMessagePixelFormatUnnecessarilyWide(quantizerPixelFormat));

            if (!useQuantizer)
            {
                if (bpp <= 8 && bpp < originalBpp)
                    result.AddInfo(nameof(QuantizerSelectorViewModel.Quantizer), Res.InfoMessagePaletteAutoSelected(1 << bpp, pixelFormat));
                else if (changePixelFormat && ditherer != null && pixelFormat.CanBeDithered() && !(bpp <= 8 && bpp >= originalBpp))
                    result.AddInfo(nameof(QuantizerSelectorViewModel.Quantizer), Res.InfoMessageQuantizerAutoSelected(pixelFormat));
                else if (changePixelFormat && !useDitherer && originalHasAlpha && !pixelFormat.HasAlpha())
                    result.AddInfo(nameof(QuantizerSelectorViewModel.Quantizer), Res.InfoMessageAlphaTurnsBlack);
            }
            else if (bppHint > originalBpp)
                result.AddInfo(nameof(QuantizerSelectorViewModel.Quantizer), Res.InfoMessageQuantizerMayHaveNoEffect);
            else if (bppHint == 32 && originalBpp == 32 && !originalPixelFormat.HasAlpha())
                result.AddInfo(nameof(QuantizerSelectorViewModel.Quantizer), Res.InfoMessageArgbQuantizerHasNoEffect);

            if (bppHint < originalBpp && !useDitherer && quantizerPixelFormat.CanBeDithered())
            {
                if (QuantizerSelectorViewModel.SelectedQuantizer!.Method.Name != nameof(PredefinedColorsQuantizer.Grayscale))
                    result.AddInfo(nameof(DithererSelectorViewModel.Ditherer), Res.InfoMessageQuantizerCanBeDithered(originalPixelFormat));
            }
            else if (bpp < originalBpp && !useDitherer && pixelFormat.CanBeDithered())
                result.AddInfo(nameof(DithererSelectorViewModel.Ditherer), Res.InfoMessagePixelFormatCanBeDithered(originalPixelFormat));
            else if (!useQuantizer && ditherer != null && !pixelFormat.CanBeDithered())
                result.AddInfo(nameof(DithererSelectorViewModel.Ditherer), Res.InfoMessageDithererIgnored(pixelFormat));

            return result;
        }

        protected override bool AffectsPreview(string propertyName)
            => propertyName is nameof(PixelFormat) or nameof(ChangePixelFormat) or nameof(UseQuantizer) or nameof(UseDitherer);

        protected override GenerateTaskBase CreateGenerateTask()
            => new GenerateTask(ChangePixelFormat ? PixelFormat : OriginalPixelFormat,
                UseQuantizer ? QuantizerSelectorViewModel.Quantizer : null,
                UseDitherer ? DithererSelectorViewModel.Ditherer : null);

        protected override bool MatchesSettings(GenerateTaskBase task)
        {
            var t = (GenerateTask)task;
            return t.PixelFormat == (ChangePixelFormat ? PixelFormat : OriginalPixelFormat)
                && t.Quantizer == (UseQuantizer ? QuantizerSelectorViewModel.Quantizer : null)
                && t.Ditherer == (UseDitherer ? DithererSelectorViewModel.Ditherer : null);
        }

        protected override bool MatchesOriginal(GenerateTaskBase task)
        {
            var t = (GenerateTask)task;
            return t.PixelFormat == originalPixelFormat && t.Quantizer == null && t.Ditherer == null;
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;
            if (disposing)
            {
                // These disposals remove every subscriptions as well
                QuantizerSelectorViewModel.Dispose();
                DithererSelectorViewModel.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Event Handlers

        private void Selector_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(QuantizerSelectorViewModel.Quantizer) or nameof(QuantizerSelectorViewModel.CreateQuantizerError)
                or nameof(DithererSelectorViewModel.Ditherer) or nameof(DithererSelectorViewModel.CreateDithererError))
            {
                Validate();
            }

            if (e.PropertyName == nameof(QuantizerSelectorViewModel.Quantizer) && UseQuantizer
                || e.PropertyName == nameof(DithererSelectorViewModel.Ditherer) && UseDitherer)
            {
                BeginGeneratePreview();
            }
        }

        #endregion

        #endregion
    }
}
