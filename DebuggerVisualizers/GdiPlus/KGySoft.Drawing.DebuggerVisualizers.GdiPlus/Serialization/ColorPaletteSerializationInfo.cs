#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorSerializationInfo.cs
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

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using KGySoft.Reflection;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.GdiPlus.Serialization
{
    internal sealed class ColorPaletteSerializationInfo
    {
        #region Properties

        internal ColorPalette Palette { get; private set; } = default!;

        #endregion

        #region Constructors

        internal ColorPaletteSerializationInfo(ColorPalette palette) => Palette = palette;

        internal ColorPaletteSerializationInfo(BinaryReader reader) => ReadFrom(reader);

        #endregion

        #region Methods

        #region Internal Methods

        internal void Write(BinaryWriter bw)
        {
            Color[] entries = Palette.Entries;
            bw.Write(entries.Length);
            foreach (Color entry in entries)
            {
                bw.Write(entry.IsKnownColor);
                bw.Write(entry.IsKnownColor ? (int)entry.ToKnownColor() : entry.ToArgb());
            }
        }

        #endregion

        #region Private Methods

        private void ReadFrom(BinaryReader br)
        {
            int len = br.ReadInt32();

            Palette = (ColorPalette)Reflector.CreateInstance(typeof(ColorPalette), len);
            Color[] entries = Palette.Entries;
            for (int i = 0; i < len; i++)
            {
                entries[i] = br.ReadBoolean()
                    ? Color.FromKnownColor((KnownColor)br.ReadInt32())
                    : Color.FromArgb(br.ReadInt32());
            }
        }

        #endregion

        #endregion
    }
}
