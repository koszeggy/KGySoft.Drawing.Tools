#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorPaletteVisualizerViewModel.cs
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
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;

using KGySoft.Drawing.Imaging;
using KGySoft.Reflection;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class ColorPaletteVisualizerViewModel : PaletteVisualizerViewModel, IViewModel<ColorPalette>
    {
        #region Fields

        #region Static Fields

        private static readonly FieldAccessor? flagsField = typeof(ColorPalette)
            .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
            .FirstOrDefault(f => f.FieldType == typeof(int)) is FieldInfo fi ? FieldAccessor.GetAccessor(fi) : null;

        #endregion

        #region Instance Fields

        private PaletteFlags flags;

        #endregion

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

            int len = entries.Length;
            var result = (ColorPalette)Reflector.CreateInstance(typeof(ColorPalette), len);
            entries.CopyTo(result.Entries, 0);

            if (flagsField != null)
            {
                var palette = new Palette(entries);
                var paletteFlags = flags & PaletteFlags.Halftone;
                if (palette.HasAlpha)
                    paletteFlags |= PaletteFlags.HasAlpha;
                if (palette.IsGrayscale)
                    paletteFlags |= PaletteFlags.GrayScale;
                flagsField.SetInstanceValue(result, (int)paletteFlags);
            }

            return result;
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
