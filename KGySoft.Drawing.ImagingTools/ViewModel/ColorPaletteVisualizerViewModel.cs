#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorPaletteVisualizerViewModel.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2026 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System.Drawing;
using System.Drawing.Imaging;

using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Reflection;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class ColorPaletteVisualizerViewModel : PaletteVisualizerViewModel, IViewModel<ColorPalette>
    {
        #region Fields

        private PaletteFlags flags;

        #endregion

        #region Constructors

        internal ColorPaletteVisualizerViewModel(ColorPalette palette)
        {
            Palette = palette.Entries;
            Type = nameof(ColorPalette);
            flags = (PaletteFlags)palette.Flags;
        }

        #endregion

        #region Methods

        ColorPalette IViewModel<ColorPalette>.GetEditedModel()
        {
            Color[] entries = base.GetEditedModel();
            var paletteFlags = flags & PaletteFlags.Halftone;

            // On Mono, the flags actually matter, so (re)calculating them accurately based on the entries.
            var palette = new Palette(entries);
            if (palette.HasAlpha)
                paletteFlags |= PaletteFlags.HasAlpha;
            if (palette.IsGrayscale)
                paletteFlags |= PaletteFlags.GrayScale;
            return Accessors.CreateColorPalette(entries, paletteFlags);
        }

        bool IViewModel<ColorPalette>.TrySetModel(ColorPalette model)
        {
            if (!TrySetModel(model.Entries))
                return false;

            flags = (PaletteFlags)model.Flags;
            return true;
        }

        #endregion
    }
}
