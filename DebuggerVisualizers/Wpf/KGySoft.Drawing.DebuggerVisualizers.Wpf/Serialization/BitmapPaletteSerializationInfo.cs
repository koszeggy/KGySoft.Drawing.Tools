#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: BitmapPaletteSerializationInfo.cs
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

using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Wpf.Serialization
{
    internal sealed class BitmapPaletteSerializationInfo
    {
        #region Fields

        private readonly BitmapPalette? palette;

        #endregion

        #region Properties

        internal CustomPaletteInfo? PaletteInfo { get; private set; }

        #endregion

        #region Constructors

        internal BitmapPaletteSerializationInfo(BitmapPalette palette) => this.palette = palette;

        internal BitmapPaletteSerializationInfo(BinaryReader reader) => ReadFrom(reader);

        #endregion

        #region Methods

        #region Internal Methods

        internal void Write(BinaryWriter bw)
        {
            IList<Color>? colors = palette!.Colors;
            bw.Write(colors.Count);
            foreach (Color color in colors)
                new ColorSerializationInfo(color).Write(bw);
        }

        #endregion

        #region Private Methods

        private void ReadFrom(BinaryReader br)
        {
            PaletteInfo = new CustomPaletteInfo { Type = nameof(BitmapPalette) };
            int count = br.ReadInt32();
            for (int i = 0; i < count; i++)
                PaletteInfo.Entries.Add(new ColorSerializationInfo(br).ColorInfo!);
        }

        #endregion

        #endregion
    }
}
