#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorSerializationInfo.cs
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

using System.Drawing;
using System.IO;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.GdiPlus.Serialization
{
    internal sealed class ColorSerializationInfo
    {
        #region Properties

        internal Color Color { get; private set; }

        #endregion

        #region Constructors

        internal ColorSerializationInfo(Color color) => Color = color;

        internal ColorSerializationInfo(BinaryReader reader) => ReadFrom(reader);

        #endregion

        #region Methods

        #region Internal Methods

        internal void Write(BinaryWriter bw)
        {
            bw.Write(Color.IsKnownColor);
            bw.Write(Color.IsKnownColor ? (int)Color.ToKnownColor() : Color.ToArgb());
        }

        #endregion

        #region Private Methods

        private void ReadFrom(BinaryReader br)
        {
            Color = br.ReadBoolean()
                ? Color.FromKnownColor((KnownColor)br.ReadInt32())
                : Color.FromArgb(br.ReadInt32());
        }

        #endregion

        #endregion
    }
}
