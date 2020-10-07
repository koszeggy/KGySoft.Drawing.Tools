#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: PreviewImageViewModel.cs
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
    internal class PreviewImageViewModel : ViewModelBase
    {
        #region Properties

        internal Image OriginalImage { get => Get<Image>(); set => Set(value); }
        internal Image PreviewImage { get => Get<Image>(); set => Set(value); }
        internal Image DisplayImage { get => Get<Image>(); set => Set(value); }
        internal bool AutoZoom { get => Get(true); set => Set(value); }
        internal bool SmoothZooming { get => Get(true); set => Set(value); }
        internal bool ShowOriginal { get => Get<bool>(); set => Set(value); }
        internal bool ZoomEnabled { get => Get<bool>(); set => Set(value); }
        internal bool ShowOriginalEnabled { get => Get(true); set => Set(value); }

        #endregion

        #region Methods

        protected override bool AffectsModifiedState(string propertyName) => false;

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            switch (e.PropertyName)
            {
                case nameof(PreviewImage):
                    if (!ShowOriginal)
                        DisplayImage = PreviewImage;
                    return;
                case nameof(OriginalImage):
                    if (ShowOriginal)
                        DisplayImage = PreviewImage;
                    return;
                case nameof(DisplayImage):
                    ZoomEnabled = e.NewValue != null;
                    return;
                case nameof(ShowOriginal):
                    DisplayImage = e.NewValue is true ? OriginalImage : PreviewImage;
                    return;
            }
        }

        #endregion
    }
}