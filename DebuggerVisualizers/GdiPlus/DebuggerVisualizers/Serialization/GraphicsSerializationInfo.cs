#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: GraphicsSerializationInfo.cs
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

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.GdiPlus.Serialization
{
    internal sealed class GraphicsSerializationInfo : IDisposable
    {
        #region Properties

        internal GraphicsInfo GraphicsInfo { get; private set; } = default!;

        #endregion

        #region Constructors

        internal GraphicsSerializationInfo(Graphics graphics) => GraphicsInfo = new GraphicsInfo(graphics);

        internal GraphicsSerializationInfo(BinaryReader reader) => ReadFrom(reader);

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
            Matrix matrix = GraphicsInfo.Transform!;
            bw.Write(matrix.IsIdentity);
            if (!matrix.IsIdentity)
            {
                foreach (float element in matrix.Elements)
                    bw.Write(element);
            }

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
            result.Transform = br.ReadBoolean()
                ? new Matrix()
                : new Matrix(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

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
