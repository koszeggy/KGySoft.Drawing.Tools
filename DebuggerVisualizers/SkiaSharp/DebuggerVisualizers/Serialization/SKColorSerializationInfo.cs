#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: SKColorSerializationInfo.cs
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
using KGySoft.Drawing.SkiaSharp;

using SkiaSharp;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.SkiaSharp.Serialization
{
    internal sealed class SKColorSerializationInfo
    {
        #region Fields

        private readonly SKColor color;

        #endregion

        #region Properties

        internal CustomColorInfo? ColorInfo { get; private set; }

        #endregion

        #region Constructors

        internal SKColorSerializationInfo(SKColor color) => this.color = color;
        internal SKColorSerializationInfo(BinaryReader reader) => ReadFrom(reader);

        #endregion

        #region Methods

        #region Internal Methods

        internal void Write(BinaryWriter bw)
        {
            // 1. Name (reflects original SKColor representation)
            bw.Write(color.ToString());

            // 2. Actual color value
            bw.Write(color.ToColor32().ToArgb());
        }

        #endregion

        #region Private Methods

        private void ReadFrom(BinaryReader br)
        {
            ColorInfo = new CustomColorInfo
            {
                Type = nameof(SKColor),

                // 1. Name
                Name = br.ReadString(),

                // 2. Color value
                DisplayColor = Color32.FromArgb(br.ReadInt32()),
            };
        }

        #endregion

        #endregion
    }
}
