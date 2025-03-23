#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: BitmapPaletteSerializationInfo.cs
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

using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using KGySoft.Drawing.DebuggerVisualizers.Serialization;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Wpf.Serialization
{
    internal sealed class BitmapPaletteSerializationInfo : CustomPaletteSerializationInfo
    {
        #region Constructors

        internal BitmapPaletteSerializationInfo(BitmapPalette palette) => PaletteInfo = GetPaletteInfo(palette);

        internal BitmapPaletteSerializationInfo(BinaryReader reader)
            : base(reader)
        {
        }

        #endregion

        #region Methods

        internal static CustomPaletteInfo? GetPaletteInfo(BitmapPalette? palette)
        {
            if (palette == null)
                return null;

            var result = new CustomPaletteInfo
            {
                Type = palette.GetType().Name,
                EntryType = nameof(Color)
            };

            foreach (Color color in palette.Colors)
                result.Entries.Add(ColorSerializationInfo.GetColorInfo(color, false));

            return result;
        }

        #endregion
    }
}
