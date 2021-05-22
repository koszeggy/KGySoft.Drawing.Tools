#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorVisualizerViewModel.cs
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
    internal class ColorVisualizerViewModel : ViewModelBase, IViewModel<Color>
    {
        #region Properties

        internal Color Color { get => Get<Color>(); set => Set(value); }
        internal bool ReadOnly { get => Get<bool>(); set => Set(value); }

        #endregion

        #region Methods

        #region Public Methods
        
        public Color GetEditedModel() => Color;

        #endregion

        #region Protected Methods

        protected override void ApplyDisplayLanguage() => OnPropertyChanged(new PropertyChangedExtendedEventArgs(Color, Color, nameof(Color)));

        #endregion

        #endregion
    }
}