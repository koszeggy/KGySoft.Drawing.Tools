﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorSerializationInfo.cs
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

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using KGySoft.Reflection;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Serialization
{
    internal sealed class ColorPaletteSerializationInfo
    {
        #region Properties

        internal ColorPalette Palette { get; private set; }

        #endregion

        #region Constructors

        internal ColorPaletteSerializationInfo(ColorPalette palette)
        {
            Palette = palette;
        }

        [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "False alarm, the stream must not be disposed and the leaveOpen parameter is not available on every targeted platform")]
        internal ColorPaletteSerializationInfo(Stream stream)
        {
            ReadFrom(new BinaryReader(stream));
        }

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
