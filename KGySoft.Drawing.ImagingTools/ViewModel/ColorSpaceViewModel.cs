#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorSpaceViewModel.cs
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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.Imaging;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class ColorSpaceViewModel : ViewModelBase, IViewModel<Bitmap>, IValidatingObject
    {
        #region Fields

        private readonly Bitmap originalImage;
        private readonly PixelFormat originalPixelFormat;
        private readonly bool originalHasAlpha;

        private bool initializing = true;
        private bool keepResult;

        #endregion

        #region Events

        internal event EventHandler<EventArgs<ValidationResultsCollection>> ValidationResultsChanged
        {
            add => ValidationResultsChangedHandler += value;
            remove => ValidationResultsChangedHandler -= value;
        }

        #endregion

        #region Properties

        #region Public Properties

        public bool IsValid { get => Get<bool>(); set => Set(value); }
        public ValidationResultsCollection ValidationResults { get => Get(DoValidation); set => Set(value); }

        #endregion

        #region Internal Properties

        internal PreviewImageViewModel PreviewImageViewModel => Get(() => new PreviewImageViewModel());
        internal QuantizerSelectorViewModel QuantizerSelectorViewModel => Get(() => new QuantizerSelectorViewModel());
        internal DithererSelectorViewModel DithererSelectorViewModel => Get(() => new DithererSelectorViewModel());

        internal PixelFormat[] PixelFormats => Get(() => Enum<PixelFormat>.GetValues().Where(pf => pf.IsValidFormat()).OrderBy(pf => pf & PixelFormat.Max).ToArray());
        internal PixelFormat PixelFormat { get => Get<PixelFormat>(); set => Set(value); }

        internal bool UseQuantizer { get => Get<bool>(); set => Set(value); }
        internal bool UseDitherer { get => Get<bool>(); set => Set(value); }

        internal ICommand ApplyCommand => Get(() => new SimpleCommand(OnApplyCommand));
        internal ICommand CancelCommand => Get(() => new SimpleCommand(OnCancelCommand));

        internal ICommandState ApplyCommandState => Get(() => new CommandState());

        #endregion

        #region Private Properties

        private Exception ConvertPixelFormatError { get => Get<Exception>(); set => Set(value); }
        private EventHandler<EventArgs<ValidationResultsCollection>> ValidationResultsChangedHandler { get => Get<EventHandler<EventArgs<ValidationResultsCollection>>>(); set => Set(value); }

        #endregion

        #endregion

        #region Constructors

        internal ColorSpaceViewModel(Bitmap image)
        {
            originalImage = image ?? throw new ArgumentNullException(nameof(image), PublicResources.ArgumentNull);
            originalPixelFormat = image.PixelFormat;
            originalHasAlpha = originalPixelFormat.HasAlpha() || originalPixelFormat.IsIndexed() && image.Palette.Entries.Any(c => c.A < Byte.MaxValue);
            PreviewImageViewModel previewImage = PreviewImageViewModel;
            previewImage.PropertyChanged += PreviewImage_PropertyChanged;
            QuantizerSelectorViewModel.PropertyChanged += Selector_PropertyChanged;
            DithererSelectorViewModel.PropertyChanged += Selector_PropertyChanged;

            previewImage.Image = image;
            PixelFormat = image.PixelFormat;
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName.In(nameof(PixelFormat), nameof(UseQuantizer), nameof(UseDitherer)))
            {
                if (initializing)
                    return;

                Validate();
                GeneratePreview();
                return;
            }

            if (e.PropertyName == nameof(ValidationResults))
            {
                var validationResults = (ValidationResultsCollection)e.NewValue;
                IsValid = !validationResults.HasErrors;
                ValidationResultsChangedHandler?.Invoke(this, new EventArgs<ValidationResultsCollection>(validationResults));
                return;
            }

            if (e.PropertyName == nameof(ConvertPixelFormatError))
                Validate();
        }

        // IsModified is set explicitly from PreviewImage_PropertyChanged
        protected override bool AffectsModifiedState(string propertyName) => false;

        internal override void ViewLoaded()
        {
            initializing = false;
            base.ViewLoaded();
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;
            if (disposing)
            {
                Image preview = PreviewImageViewModel.Image;

                // These disposals remove every subscriptions as well
                PreviewImageViewModel?.Dispose();
                QuantizerSelectorViewModel?.Dispose();
                DithererSelectorViewModel?.Dispose();

                if (!ReferenceEquals(originalImage, preview) && !keepResult)
                    preview?.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void GeneratePreview()
        {
            // checking whether a new generate should be added
            PixelFormat pixelFormat = PixelFormat;
            bool useQuantizer = UseQuantizer;
            bool useDitherer = UseDitherer;
            IQuantizer quantizer = useQuantizer ? QuantizerSelectorViewModel.Quantizer : null;
            IDitherer ditherer = useDitherer ? DithererSelectorViewModel.Ditherer : null;
            ConvertPixelFormatError = null;

            // error - null
            if (useQuantizer && quantizer == null || useDitherer && ditherer == null || !IsValid)
            {
                SetPreview(null);
                return;
            }

            // original image
            if (pixelFormat == originalPixelFormat && quantizer == null && ditherer == null)
            {
                SetPreview(originalImage);
                return;
            }

            // generating a new image
            Bitmap preview = null;
            try
            {
                preview = originalImage.ConvertPixelFormat(pixelFormat, quantizer, ditherer);
            }
            catch (Exception e) when (!e.IsCriticalGdi())
            {
                ConvertPixelFormatError = e;
            }

            SetPreview(preview);
        }

        private void SetPreview(Bitmap image)
        {
            PreviewImageViewModel preview = PreviewImageViewModel;
            Image toDispose = preview.Image;
            preview.Image = image;
            if (toDispose != null && toDispose != originalImage)
                toDispose.Dispose();
        }

        private void Validate() => ValidationResults = DoValidation();

        private ValidationResultsCollection DoValidation()
        {
            // Note that !UseQuantizer and Quantizer == null are not interchangeable.
            // UseQuantizer indicates whether the selector is checked while Quantizer is not null if an instance could be successfully created
            bool useQuantizer = UseQuantizer;
            bool useDitherer = UseDitherer;
            PixelFormat pixelFormat = PixelFormat;
            IQuantizer quantizer = useQuantizer ? QuantizerSelectorViewModel.Quantizer : null;
            IDitherer ditherer = useDitherer ? DithererSelectorViewModel.Ditherer : null;
            Exception convertError = ConvertPixelFormatError;
            Exception quantizerError = useQuantizer ? QuantizerSelectorViewModel.CreateQuantizerError : null;
            Exception dithererError = useDitherer ? DithererSelectorViewModel.CreateDithererError : null;
            int bpp = pixelFormat.ToBitsPerPixel();
            int originalBpp = originalPixelFormat.ToBitsPerPixel();
            int? bppHint = quantizer?.PixelFormatHint.ToBitsPerPixel();
            var result = new ValidationResultsCollection();

            // errors
            if (!pixelFormat.IsSupportedNatively())
                result.AddError(nameof(PixelFormat), Res.ErrorMessagePixelFormatNotSupported(pixelFormat));

            if (quantizerError != null)
                result.AddError(nameof(QuantizerSelectorViewModel.Quantizer), Res.ErrorMessageFailedToInitializeQuantizer(quantizerError.Message));
            else if (bppHint <= 8 && bppHint > bpp)
                result.AddError(nameof(QuantizerSelectorViewModel.Quantizer), Res.ErrorMessageQuantizerPaletteTooLarge(pixelFormat, quantizer.PixelFormatHint, 1 << bpp));

            if (dithererError != null)
                result.AddError(nameof(DithererSelectorViewModel.Ditherer), Res.ErrorMessageFailedToInitializeDitherer(dithererError.Message));

            if (convertError != null)
                result.AddError(nameof(PreviewImageViewModel.Image), Res.ErrorMessageFailedToGeneratePreview(convertError.Message));

            //if (result.HasErrors)
            //    return result;

            // warnings
            if (!useQuantizer && originalPixelFormat != pixelFormat && originalPixelFormat.IsWide() && pixelFormat.IsWide())
                result.AddWarning(nameof(PixelFormat), Res.WarningMessageWideConversionLoss(originalPixelFormat));

            if (bppHint > bpp)
                result.AddWarning(nameof(QuantizerSelectorViewModel.Quantizer), Res.WarningMessageQuantizerTooWide(pixelFormat, quantizer.PixelFormatHint));

            if (bppHint == 32 && ditherer != null)
                result.AddWarning(nameof(DithererSelectorViewModel.Ditherer), Res.WarningMessageDithererNoAlphaGradient);

            // information
            if (bppHint < bpp)
                result.AddInfo(nameof(PixelFormat), Res.InfoMessagePixelFormatUnnecessarilyWide(quantizer.PixelFormatHint));

            if (!useQuantizer)
            {
                if (bpp <= 8)
                    result.AddInfo(nameof(QuantizerSelectorViewModel.Quantizer), Res.InfoMessagePaletteAutoSelected(1 << bpp, pixelFormat));
                else if (ditherer != null && pixelFormat.CanBeDithered())
                    result.AddInfo(nameof(QuantizerSelectorViewModel.Quantizer), Res.InfoMessageQuantizerAutoSelected(pixelFormat));
                else if (!useDitherer && originalHasAlpha && !pixelFormat.HasAlpha())
                    result.AddInfo(nameof(QuantizerSelectorViewModel.Quantizer), Res.InfoMessageAlphaTurnsBlack);
            }
            else if (bppHint > originalBpp)
                result.AddInfo(nameof(QuantizerSelectorViewModel.Quantizer), Res.InfoMessageQuantizerMayHaveNoEffect);
            else if (bppHint == 32 && originalBpp >= 32 && !originalPixelFormat.HasAlpha())
                result.AddInfo(nameof(QuantizerSelectorViewModel.Quantizer), Res.InfoMessageArgbQuantizerHasNoEffect);

            if (bpp < originalBpp && !useDitherer && pixelFormat.CanBeDithered())
                result.AddInfo(nameof(DithererSelectorViewModel.Ditherer), Res.InfoMessageUseDithererToPreserveDetails(originalPixelFormat));
            else if (!useQuantizer && ditherer != null && !pixelFormat.CanBeDithered())
                result.AddInfo(nameof(DithererSelectorViewModel.Ditherer), Res.InfoMessageDithererIgnored(pixelFormat));

            return result;
        }

        #endregion

        #region Event Handlers

        private void PreviewImage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var vm = (PreviewImageViewModel)sender;

            // preview image has been changed: updating IsModified accordingly
            if (e.PropertyName == nameof(vm.Image))
            {
                Image image = vm.Image;
                SetModified(image != null && originalImage != image);

                // no need to check whether generating is in progress because otherwise it would not be set
                ApplyCommandState.Enabled = IsModified;
                return;
            }
        }

        private void Selector_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.In(nameof(QuantizerSelectorViewModel.Quantizer), nameof(QuantizerSelectorViewModel.CreateQuantizerError),
                nameof(DithererSelectorViewModel.Ditherer), nameof(DithererSelectorViewModel.CreateDithererError)))
            {
                Validate();
            }

            if (e.PropertyName == nameof(QuantizerSelectorViewModel.Quantizer) && UseQuantizer
                || e.PropertyName == nameof(DithererSelectorViewModel.Ditherer) && UseDitherer)
            {
                GeneratePreview();
            }
        }

        #endregion

        #region Command Handlers

        private void OnCancelCommand()
        {
            // TODO: cancel pending generate
            SetModified(false);
            CloseViewCallback?.Invoke();
        }

        private void OnApplyCommand()
        {
            // TODO: wait for pending generate but better if it cannot happen
            keepResult = true;
            CloseViewCallback?.Invoke();
        }

        #endregion

        #region Explicitly Implemented Interface Methods

        Bitmap IViewModel<Bitmap>.GetEditedModel() => PreviewImageViewModel.Image as Bitmap;

        #endregion

        #endregion
    }
}
