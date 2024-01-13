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

using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Core.Serialization
{
    internal sealed class PaletteSerializationInfo
    {
        #region Fields

        private readonly IPalette? palette;

        #endregion

        #region Properties

        internal CustomPaletteInfo? PaletteInfo { get; private set; }

        #endregion

        #region Constructors

        internal PaletteSerializationInfo(IPalette palette) => this.palette = palette;

        internal PaletteSerializationInfo(BinaryReader reader) => ReadFrom(reader);

        #endregion

        #region Methods

        #region Internal Methods

        internal void Write(BinaryWriter bw)
        {
            bw.Write(palette!.GetType().Name);

            int count = palette.Count;
            bw.Write(count);
            for (int i = 0; i < count; i++)
                new ColorSerializationInfo(palette[i]).Write(bw);
        }

        #endregion

        #region Private Methods

        private void ReadFrom(BinaryReader br)
        {
            PaletteInfo = new CustomPaletteInfo { Type = br.ReadString() };
            int count = br.ReadInt32();
            for (int i = 0; i < count; i++)
                PaletteInfo.Entries.Add(new ColorSerializationInfo(br).ColorInfo!);
        }

        #endregion

        #endregion
    }
}
