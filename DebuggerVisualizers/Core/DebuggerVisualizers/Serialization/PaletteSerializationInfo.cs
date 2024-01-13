#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: PaletteSerializationInfo.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2024 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System.IO;

using KGySoft.Drawing.DebuggerVisualizers.Serialization;
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Core.Serialization
{
    internal sealed class PaletteSerializationInfo : CustomPaletteSerializationInfo
    {
        #region Constructors

        internal PaletteSerializationInfo(IPalette palette) => PaletteInfo = GetPaletteInfo(palette);

        internal PaletteSerializationInfo(BinaryReader reader)
            : base(reader)
        {
        }

        #endregion

        #region Methods

        internal static CustomPaletteInfo? GetPaletteInfo(IPalette? palette)
        {
            if (palette == null)
                return null;

            var result = new CustomPaletteInfo
            {
                Type = palette.GetType().Name,
                EntryType = nameof(Color32)
            };

            for (int i = 0; i < palette.Count; i++)
                result.Entries.Add(ColorSerializationInfo.GetColorInfo(palette[i], false));
            return result;
        }

        #endregion
    }
}
