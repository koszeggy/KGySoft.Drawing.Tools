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

using System.Drawing;

using KGySoft.ComponentModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class ColorSpaceViewModel : ViewModelBase, IViewModel<Bitmap>
    {
        #region Properties

        internal Bitmap Bitmap { get => Get<Bitmap>(); set => Set(value); }
        internal PreviewImageViewModel PreviewImageViewModel => Get(() => new PreviewImageViewModel());
        internal QuantizerSelectorViewModel QuantizerSelectorViewModel => Get(() => new QuantizerSelectorViewModel());
        internal DithererSelectorViewModel DithererSelectorViewModel => Get(() => new DithererSelectorViewModel());
        internal bool ChangePixelFormat { get => Get(true); set => Set(value); }
        internal bool UseQuantizer { get => Get<bool>(); set => Set(value); }
        internal bool UseDitherer { get => Get<bool>(); set => Set(value); }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == nameof(Bitmap))
            {
                PreviewImageViewModel.Image = (Bitmap)e.NewValue;
                return;
            }
        }

        protected override bool AffectsModifiedState(string propertyName) => propertyName == nameof(Bitmap);

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;
            if (disposing)
            {
                PreviewImageViewModel?.Dispose();
                QuantizerSelectorViewModel?.Dispose();
                DithererSelectorViewModel?.Dispose();

                // Disposing image only if it has been re-generated and was not discarded in the end
                if (IsModified)
                    Bitmap?.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Explicitly Implemented Interface Methods

        Bitmap IViewModel<Bitmap>.GetEditedModel()
        {
            // indicating that the generated image should not be disposed
            SetModified(false);
            return Bitmap;
        }

        #endregion

        #endregion
    }
}
