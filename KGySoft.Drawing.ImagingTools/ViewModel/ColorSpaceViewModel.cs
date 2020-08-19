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
using System.Collections.Generic;
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
    internal class ColorSpaceViewModel : ViewModelBase, IViewModel<Bitmap>
    {
        #region Fields

        private readonly Bitmap originalImage;
        private readonly PixelFormat originalPixelFormat;

        private bool initializing = true;
        private bool keepResult;

        #endregion

        #region Properties

        internal PreviewImageViewModel PreviewImageViewModel => Get(() => new PreviewImageViewModel());
        internal QuantizerSelectorViewModel QuantizerSelectorViewModel => Get(() => new QuantizerSelectorViewModel());
        internal DithererSelectorViewModel DithererSelectorViewModel => Get(() => new DithererSelectorViewModel());
        
        internal PixelFormat[] PixelFormats => Get(() => Enum<PixelFormat>.GetValues().Where(pf => pf.IsValidFormat()).OrderBy(pf => pf & PixelFormat.Max).ToArray());
        internal PixelFormat SelectedPixelFormat { get => Get<PixelFormat>(); set => Set(value); }

        internal bool UseQuantizer { get => Get<bool>(); set => Set(value); }
        internal bool UseDitherer { get => Get<bool>(); set => Set(value); }

        internal ICommand ApplyCommand => Get(() => new SimpleCommand(OnApplyCommand));
        internal ICommand CancelCommand => Get(() => new SimpleCommand(OnCancelCommand));

        internal ICommandState ApplyCommandState => Get(() => new CommandState());

        #endregion

        #region Constructors

        internal ColorSpaceViewModel(Bitmap image)
        {
            originalImage = image ?? throw new ArgumentNullException(nameof(image), PublicResources.ArgumentNull);
            originalPixelFormat = image.PixelFormat;
            PreviewImageViewModel previewImage = PreviewImageViewModel;
            previewImage.PropertyChanged += PreviewImage_PropertyChanged;
            QuantizerSelectorViewModel.PropertyChanged += Selector_PropertyChanged;
            DithererSelectorViewModel.PropertyChanged += Selector_PropertyChanged;

            previewImage.Image = image;
            SelectedPixelFormat = image.PixelFormat;
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName.In(nameof(SelectedPixelFormat), nameof(UseQuantizer), nameof(UseDitherer)))
            {
                if (!initializing)
                    GeneratePreview();
                return;
            }
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
            PixelFormat pixelFormat = SelectedPixelFormat;
            bool useQuantizer = UseQuantizer;
            bool useDitherer = UseDitherer;
            IQuantizer quantizer = useQuantizer ? QuantizerSelectorViewModel.Quantizer : null;
            IDitherer ditherer = useDitherer ? DithererSelectorViewModel.Ditherer : null;

            // error - null
            if (useQuantizer && quantizer == null || useDitherer && ditherer == null)
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
            SetPreview(originalImage.ConvertPixelFormat(pixelFormat, quantizer, ditherer));
        }

        private void SetPreview(Bitmap image)
        {
            PreviewImageViewModel preview = PreviewImageViewModel;
            Image toDispose = preview.Image;
            preview.Image = image;
            if (toDispose != null && toDispose != originalImage)
                toDispose.Dispose();
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
            if (e.PropertyName == nameof(QuantizerSelectorViewModel.Quantizer) && UseQuantizer
                || e.PropertyName == nameof(DithererSelectorViewModel.Ditherer) && UseDitherer)
            {
                GeneratePreview();
                return;
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
