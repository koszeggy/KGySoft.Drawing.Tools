#region Copyright

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
using System.IO;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Serialization
{
    internal sealed class ColorSerializationInfo
    {
        #region Properties

        internal Color Color { get; private set; }

        #endregion

        #region Constructors

        internal ColorSerializationInfo(Color color)
        {
            Color = color;
        }

        [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "False alarm, the stream must not be disposed and the leaveOpen parameter is not available on every targeted platform")]
        internal ColorSerializationInfo(Stream stream)
        {
            ReadFrom(new BinaryReader(stream));
        }

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
