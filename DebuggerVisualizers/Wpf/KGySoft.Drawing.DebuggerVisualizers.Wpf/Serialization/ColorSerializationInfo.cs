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

using System.IO;
using System.Windows.Media;

using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Wpf.Serialization
{
    internal sealed class ColorSerializationInfo
    {
        #region Fields

        private readonly Color color;

        #endregion

        #region Properties

        internal CustomColorInfo? ColorInfo { get; private set; }

        #endregion

        #region Constructors

        internal ColorSerializationInfo(Color color) => this.color = color;

        internal ColorSerializationInfo(BinaryReader reader) => ReadFrom(reader);

        #endregion

        #region Methods

        #region Internal Methods

        internal void Write(BinaryWriter bw)
        {
            // 1. Name (reflects sRGB, scRGB, ColorContext creations)
            bw.Write(color.ToString());

            // 2. Color value (ARGB as bytes is enough, this is just for the display color)
            bw.Write(color.A);
            bw.Write(color.R);
            bw.Write(color.G);
            bw.Write(color.B);
        }

        #endregion

        #region Private Methods

        private void ReadFrom(BinaryReader br)
        {
            ColorInfo = new CustomColorInfo
            {
                Type = nameof(Color),

                // 1. Name
                Name = br.ReadString(),
                
                // 2. Color value
                DisplayColor = new Color32(br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte()),
            };
        }

        #endregion

        #endregion
    }
}