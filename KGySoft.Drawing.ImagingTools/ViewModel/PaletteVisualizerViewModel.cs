#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: PaletteVisualizerViewModel.cs
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
using System.Collections.Generic;
using System.Drawing;

using KGySoft.ComponentModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class PaletteVisualizerViewModel : ViewModelBase, IViewModel<Color[]>
    {
        #region Properties

        #region Internal Properties

        // ReSharper disable once ConstantConditionalAccessQualifier - not cloning if value is null
        internal Color[] Palette { get => Get<Color[]>(); init => Set(value?.Clone() ?? throw new ArgumentNullException(nameof(value), PublicResources.ArgumentNull)); }
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
            if (Palette.Length == 0)
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
                var palette = (IList<Color>)e.NewValue!;
                Count = palette.Count;
            }
        }

        #endregion

        #region Explicitly Implemented Interface Properties

        Color[] IViewModel<Color[]>.GetEditedModel() => Palette;

        #endregion

        #endregion
    }
}
