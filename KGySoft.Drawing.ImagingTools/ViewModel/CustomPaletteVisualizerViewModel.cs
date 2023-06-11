#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CustomPaletteVisualizerViewModel.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2023 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System.Linq;

using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal sealed class CustomPaletteVisualizerViewModel : PaletteVisualizerViewModel
    {
        #region Fields

        private readonly CustomPaletteInfo paletteInfo;

        #endregion

        #region Constructors

        internal CustomPaletteVisualizerViewModel(CustomPaletteInfo paletteInfo)
        {
            this.paletteInfo = paletteInfo;
            ReadOnly = true;
            Palette = paletteInfo.Entries.Select(ci => ci.DisplayColor.ToColor()).ToArray();
            Type = paletteInfo.Type;
        }

        #endregion

        #region Methods

        protected override ColorVisualizerViewModel GetSelectedColorViewModel(int index)
            => new CustomColorVisualizerViewModel(paletteInfo.Entries[index])
            {
                SelectedIndex = index,
            };

        #endregion
    }
}