#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CustomPaletteVisualizerViewModel.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2025 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System.Drawing;
using System.Linq;

using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Reflection;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal sealed class CustomPaletteVisualizerViewModel : PaletteVisualizerViewModel, IViewModel<CustomPaletteInfo?>
    {
        #region Fields

        private CustomPaletteInfo? paletteInfo;

        #endregion

        #region Constructors

        internal CustomPaletteVisualizerViewModel(CustomPaletteInfo? paletteInfo)
        {
            ReadOnly = true;
            ResetPaletteInfo(paletteInfo);
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override ColorVisualizerViewModel GetSelectedColorViewModel(int index)
        {
            var result = new CustomColorVisualizerViewModel((uint)index < (uint)(paletteInfo?.Entries.Count ?? 0) ? paletteInfo?.Entries[index] : null);
            result.Type ??= paletteInfo?.EntryType;
            result.SelectedIndex = index;
            return result;
        }

        #endregion

        #region Private Methods

        private void ResetPaletteInfo(CustomPaletteInfo? model)
        {
            paletteInfo = model;
            Palette = paletteInfo?.Entries.Select(ci => ci.DisplayColor.ToColor()).ToArray() ?? Reflector.EmptyArray<Color>();
            Type = paletteInfo?.Type;
        }

        #endregion

        #region Explicitly Implemented Interface Methods

        CustomPaletteInfo? IViewModel<CustomPaletteInfo?>.GetEditedModel() => null; // read-only
        bool IViewModel<CustomPaletteInfo?>.TrySetModel(CustomPaletteInfo? model) => TryInvokeSync(() => ResetPaletteInfo(model));

        #endregion

        #endregion
    }
}