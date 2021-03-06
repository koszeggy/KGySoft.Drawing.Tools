﻿#region Copyright

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
using System.Drawing.Drawing2D;
using System.IO;

using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Serialization.Binary;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Serialization
{
    internal sealed class GraphicsSerializationInfo : IDisposable
    {
        #region Properties

        internal GraphicsInfo GraphicsInfo { get; private set; } = default!;

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

        public void Dispose() => GraphicsInfo.Dispose();

        #endregion

        #region Internal Methods

        internal void Write(BinaryWriter bw)
        {
            // 1. Bitmap
            SerializationHelper.WriteImage(bw, GraphicsInfo.GraphicsImage!);

            // 2. Transformation matrix
            BinarySerializer.SerializeByWriter(bw, GraphicsInfo.Transform!.Elements);

            // 3. Meta
            bw.Write(GraphicsInfo.OriginalVisibleClipBounds.X);
            bw.Write(GraphicsInfo.OriginalVisibleClipBounds.Y);
            bw.Write(GraphicsInfo.OriginalVisibleClipBounds.Width);
            bw.Write(GraphicsInfo.OriginalVisibleClipBounds.Height);
            bw.Write(GraphicsInfo.TransformedVisibleClipBounds.X);
            bw.Write(GraphicsInfo.TransformedVisibleClipBounds.Y);
            bw.Write(GraphicsInfo.TransformedVisibleClipBounds.Width);
            bw.Write(GraphicsInfo.TransformedVisibleClipBounds.Height);
            bw.Write((int)GraphicsInfo.PageUnit);
            bw.Write(GraphicsInfo.Resolution.X);
            bw.Write(GraphicsInfo.Resolution.Y);
        }

        #endregion

        #region Private Methods

        private void ReadFrom(BinaryReader br)
        {
            var result = new GraphicsInfo();

            // 1. Bitmap
            result.GraphicsImage = (Bitmap)SerializationHelper.ReadImage(br);

            // 2. Transformation matrix
            var elements = (float[])BinarySerializer.DeserializeByReader(br)!;
            result.Transform = new Matrix(elements[0], elements[1], elements[2], elements[3], elements[4], elements[5]);

            // 3. Meta
            result.OriginalVisibleClipBounds = new Rectangle(br.ReadInt32(), br.ReadInt32(), br.ReadInt32(), br.ReadInt32());
            result.TransformedVisibleClipBounds = new RectangleF(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            result.PageUnit = (GraphicsUnit)br.ReadInt32();
            result.Resolution = new PointF(br.ReadSingle(), br.ReadSingle());

            GraphicsInfo = result;
        }

        #endregion

        #endregion
    }
}
