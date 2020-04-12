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
using KGySoft.CoreLibraries;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class PaletteVisualizerViewModel : ViewModelBase, IViewModel<Color[]>
    {
        #region Properties

        #region Internal Properties

        internal Color[] Palette { get => Get<Color[]>(); set => Set(value.Clone()); }
        internal int Count { get => Get<int>(); set => Set(value); }
        internal bool ReadOnly { get => Get<bool>(); set => Set(value); }

        #endregion

        #region Protected Properties

        protected override bool AffectsModifiedState(string propertyName) => false; // set explicitly

        #endregion

        #endregion

        #region Methods

        #region Internal Methods

        internal override void ViewLoaded()
        {
            base.ViewLoaded();
            if (Palette.IsNullOrEmpty())
            {
                ReadOnly = true;
                ShowInfo(Res.InfoMessagePaletteEmpty);
                CloseViewCallback?.Invoke();
            }
        }

        #endregion

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

        #region Explicitly Implemented Interface Properties

        Color[] IViewModel<Color[]>.GetEditedModel() => Palette;

        #endregion

        #endregion
    }
}
