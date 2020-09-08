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

        internal Image Image { get => Get<Image>(); set => Set(value); }
        internal bool AutoZoom { get => Get(true); set => Set(value); }
        internal bool SmoothZooming { get => Get(true); set => Set(value); }
        internal bool ButtonsEnabled { get => Get<bool>(); set => Set(value); }

        #endregion

        #region Methods

        protected override bool AffectsModifiedState(string propertyName) => false;

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == nameof(Image))
                ButtonsEnabled = e.NewValue != null;
        }

        #endregion
    }
}