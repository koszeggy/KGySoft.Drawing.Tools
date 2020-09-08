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
using System.Diagnostics;
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
        #region Nested Classes

        private sealed class GenerateTask
        {
            #region Fields

            internal volatile bool IsCanceled;
            internal IAsyncResult AsyncResult;

            #endregion

            #region Methods

            internal void WaitForCompletion()
            {
                if (AsyncResult.IsCompleted)
                    return;

                try
                {
                    AsyncResult.AsyncWaitHandle.WaitOne();
                }
                catch (ObjectDisposedException)
                {
                    // it can happen that the task has just been completed after querying IsCompleted but this part
                    // must not be in a lock because then EndGeneratePreview could possibly never end
                }
            }

            #endregion
        }

        #endregion

        #region Fields

        private readonly Bitmap originalImage;
        private readonly PixelFormat originalPixelFormat;
        private readonly bool originalHasAlpha;
        private readonly object syncRoot = new object();

        private bool initializing = true;
        private bool keepResult;
        private GenerateTask activeTask;
        private DrawingProgressManager drawingProgressManager;

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

        internal bool ChangePixelFormat { get => Get<bool>(); set => Set(value); }
        internal bool UseQuantizer { get => Get<bool>(); set => Set(value); }
        internal bool UseDitherer { get => Get<bool>(); set => Set(value); }

        internal ICommand ApplyCommand => Get(() => new SimpleCommand(OnApplyCommand));
        internal ICommand CancelCommand => Get(() => new SimpleCommand(OnCancelCommand));
        internal ICommandState ApplyCommandState => Get(() => new CommandState { Enabled = false });

        internal bool IsGenerating { get => Get<bool>(); set => Set(value); }
        internal DrawingProgress Progress { get => Get<DrawingProgress>(); set => Set(value); }

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
                case nameof(UseQuantizer):
                case nameof(UseDitherer):
                    if (initializing)
                        return;
                    Validate();
                    BeginGeneratePreview();
                    return;

                case nameof(ValidationResults):
                    var validationResults = (ValidationResultsCollection)e.NewValue;
                    IsValid = !validationResults.HasErrors;
                    ValidationResultsChangedHandler?.Invoke(this, new EventArgs<ValidationResultsCollection>(validationResults));
                    return;
                
                case nameof(ConvertPixelFormatError):
                    Validate();
                    return;

                case nameof(IsModified):
                case nameof(IsGenerating):
                    ApplyCommandState.Enabled = IsModified && !IsGenerating;
                    return;
            }
        }

        // IsModified is set explicitly from PreviewImage_PropertyChanged
        protected override bool AffectsModifiedState(string propertyName) => false;

        internal override void ViewLoaded()
        {
            // could be in constructor but we only need it when there is a view
            drawingProgressManager = new DrawingProgressManager(p => Progress = p);
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

                drawingProgressManager = null;
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void BeginGeneratePreview()
        {
            IsGenerating = true;

            // canceling any current generating in progress (not nullifying ActiveTask because that could enable the Apply command,
            // but it will done in EndGeneratePreview if no new task is added in this method)
            GenerateTask canceledTask = CancelGeneratePreview();

            // checking whether a new generate should be added
            PixelFormat pixelFormat = ChangePixelFormat ? PixelFormat : originalPixelFormat;
            bool useQuantizer = UseQuantizer;
            bool useDitherer = UseDitherer;
            IQuantizer quantizer = useQuantizer ? QuantizerSelectorViewModel.Quantizer : null;
            IDitherer ditherer = useDitherer ? DithererSelectorViewModel.Ditherer : null;
            ConvertPixelFormatError = null;

            // error - null
            if (useQuantizer && quantizer == null || useDitherer && ditherer == null || !IsValid)
            {
                SetPreview(null);
                IsGenerating = false;
                return;
            }

            // original image
            if (pixelFormat == originalPixelFormat && quantizer == null && ditherer == null)
            {
                SetPreview(originalImage);
                IsGenerating = false;
                return;
            }

            // waiting for the cancellation end to prevent the possible "The image is locked elsewhere" error
            canceledTask?.WaitForCompletion();

            // generating a new image
            lock (syncRoot)
            {
                // using Begin/EndConvertPixelFormat instead of await ConvertPixelFormatAsync so it is compatible even with .NET 3.5
                var newTask = new GenerateTask();
                newTask.AsyncResult = originalImage.BeginConvertPixelFormat(pixelFormat, quantizer, ditherer,
                    new AsyncConfig
                    {
                        IsCancelRequestedCallback = () => newTask.IsCanceled,
                        ReturnDefaultIfCanceled = true,
                        State = newTask,
                        CompletedCallback = EndGeneratePreview,
                        Progress = drawingProgressManager
                    });

                activeTask = newTask;
            }
        }

        private void EndGeneratePreview(IAsyncResult asyncResult)
        {
            Bitmap result = null;
            Exception error = null;
            var task = (GenerateTask)asyncResult.AsyncState;

            try
            {
                result = ImageExtensions.EndConvertPixelFormat(asyncResult);
            }
            catch (Exception e) when (!e.IsCriticalGdi())
            {
                error = e;
            }

            lock (syncRoot)
            {
                try
                {
                    if (task.IsCanceled)
                    {
                        result?.Dispose();
                        return;
                    }

                    // the execution of this method will be marshaled back to the UI thread
                    void Action()
                    {
                        if (error != null)
                            ConvertPixelFormatError = error;
                        SetPreview(result);
                        IsGenerating = false;
                    }

                    SynchronizedInvokeCallback?.Invoke(Action);
                }
                finally
                {
                    activeTask = null;
                }
            }
        }

        private GenerateTask CancelGeneratePreview()
        {
            GenerateTask runningTask;
            lock (syncRoot)
                runningTask = activeTask;
            if (runningTask != null)
                runningTask.IsCanceled = true;
            return runningTask;
        }

        private void SetPreview(Bitmap image)
        {
            PreviewImageViewModel preview = PreviewImageViewModel;
            Image toDispose = preview.Image;
            preview.Image = image;
            if (toDispose != null && toDispose != originalImage)
                toDispose.Dispose();
        }

        private void Validate()
        {
            if (initializing)
                return;
            ValidationResults = DoValidation();
        }

        private ValidationResultsCollection DoValidation()
        {
            // Note that !UseQuantizer and Quantizer == null are not interchangeable.
            // UseQuantizer indicates whether the selector is checked while Quantizer is not null if an instance could be successfully created
            bool changePixelFormat = ChangePixelFormat;
            bool useQuantizer = UseQuantizer;
            bool useDitherer = UseDitherer;
            PixelFormat pixelFormat = changePixelFormat ? PixelFormat : originalPixelFormat;
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
            if (changePixelFormat && pixelFormat == originalPixelFormat)
                result.AddInfo(nameof(PixelFormat), Res.InfoMessageSamePixelFormat);
            if (bppHint < bpp)
                result.AddInfo(nameof(PixelFormat), Res.InfoMessagePixelFormatUnnecessarilyWide(quantizer.PixelFormatHint));

            if (!useQuantizer)
            {
                if (bpp <= 8 && bpp < originalBpp)
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

            if (bppHint < originalBpp && !useDitherer && quantizer.PixelFormatHint.CanBeDithered())
                result.AddInfo(nameof(DithererSelectorViewModel.Ditherer), Res.InfoMessageQuantizerCanBeDithered(originalPixelFormat));
            else if (bpp < originalBpp && !useDitherer && pixelFormat.CanBeDithered())
                result.AddInfo(nameof(DithererSelectorViewModel.Ditherer), Res.InfoMessagePixelFormatCanBeDithered(originalPixelFormat));
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
                BeginGeneratePreview();
            }
        }

        #endregion

        #region Command Handlers

        private void OnCancelCommand()
        {
            // canceling any pending generate and waiting for finishing so no "image is locked elsewhere" will come from the main form for the original image
            CancelGeneratePreview()?.WaitForCompletion();
            SetModified(false);
            CloseViewCallback?.Invoke();
        }

        private void OnApplyCommand()
        {
            Debug.Assert(!IsGenerating);
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
