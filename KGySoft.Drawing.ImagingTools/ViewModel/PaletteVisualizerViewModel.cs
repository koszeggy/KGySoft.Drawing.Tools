#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: PaletteVisualizerViewModel.cs
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

using System.Collections.Generic;
using System.Drawing;

using KGySoft.ComponentModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class PaletteVisualizerViewModel : ViewModelBase
    {
        #region Properties

        #region Internal Properties

        internal IList<Color> Palette { get => Get<IList<Color>>(); set => Set(value); }
        internal int Count { get => Get<int>(); set => Set(value); }
        internal bool ReadOnly { get => Get<bool>(); set => Set(value); }

        #endregion

        #region Protected Properties

        protected override bool AffectsModifiedState(string propertyName) => false; // set explicitly

        #endregion

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);

            // Palette -> Count
            if (e.PropertyName == nameof(Palette))
            {
                var palette = (IList<Color>)e.NewValue;
                Count = palette?.Count ?? 0;
            }
        }

        #endregion

        #endregion
    }
}
