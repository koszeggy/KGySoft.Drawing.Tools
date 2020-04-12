#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: GraphicsSerializationInfo.cs
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

using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using KGySoft.Drawing.DebuggerVisualizers.Model;
using KGySoft.Serialization.Binary;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Serialization
{
    internal sealed class GraphicsSerializationInfo : IDisposable
    {
        #region Properties

        internal GraphicsInfo GraphicsInfo { get; private set; }

        #endregion

        #region Constructors

        internal GraphicsSerializationInfo(Graphics graphics)
        {
            GraphicsInfo = new GraphicsInfo(graphics);
        }

        internal GraphicsSerializationInfo(Stream stream)
        {
            ReadFrom(new BinaryReader(stream));
        }

        #endregion

        #region Methods

        #region Public Methods

        public void Dispose() => GraphicsInfo?.Dispose();

        #endregion

        #region Internal Methods

        internal void Write(BinaryWriter bw)
        {
            // 1. Bitmap
            SerializationHelper.WriteImage(bw, GraphicsInfo.Data);

            // 2. Transformation matrix
            BinarySerializer.SerializeByWriter(bw, GraphicsInfo.Elements);

            // 3. Visible clip in pixels without transformation (with identity matrix)
            bw.Write(BinarySerializer.SerializeValueType(GraphicsInfo.VisibleRect));

            // 4. Info (as seen by user's transformation)
            bw.Write(GraphicsInfo.SpecialInfo);
        }

        #endregion

        #region Private Methods

        private void ReadFrom(BinaryReader br)
        {
            var result = new GraphicsInfo();

            // 1. Bitmap
            result.Data = (Bitmap)SerializationHelper.ReadImage(br);

            // 2. Transformation matrix
            result.Elements = (float[])BinarySerializer.DeserializeByReader(br);

            // 3. Visible rect in pixels
            result.VisibleRect = (Rectangle)BinarySerializer.DeserializeValueType(typeof(Rectangle), br.ReadBytes(Marshal.SizeOf(typeof(Rectangle))));

            // 4. Info
            result.SpecialInfo = br.ReadString();

            GraphicsInfo = result;
        }

        #endregion

        #endregion
    }
}
