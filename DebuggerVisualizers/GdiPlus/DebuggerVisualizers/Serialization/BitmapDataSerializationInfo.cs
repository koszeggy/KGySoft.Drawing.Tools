#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: BitmapDataSerializationInfo.cs
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
using System.Drawing.Imaging;
using System.IO;

using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.GdiPlus.Serialization
{
    internal class BitmapDataSerializationInfo : IDisposable
    {
        #region Properties

        internal BitmapDataInfo BitmapDataInfo { get; private set; } = default!;

        #endregion

        #region Constructors

        internal BitmapDataSerializationInfo(BinaryReader reader) => ReadFrom(reader);

        internal BitmapDataSerializationInfo(BitmapData bitmapData) => BitmapDataInfo = new BitmapDataInfo(bitmapData);

        #endregion

        #region Methods

        #region Public Methods

        public void Dispose() => BitmapDataInfo.Dispose();

        #endregion

        #region Internal Methods

        internal void Write(BinaryWriter bw)
        {
            // 1. Bitmap
            SerializationHelper.WriteImage(bw, BitmapDataInfo.BackingImage!);

            // 2. Data
            BitmapData data = BitmapDataInfo.BitmapData!;
            bw.Write(data.Width);
            bw.Write(data.Height);
            bw.Write(data.Stride);
            bw.Write((int)data.PixelFormat);
        }

        #endregion

        #region Private Methods

        private void ReadFrom(BinaryReader br)
        {
            var result = new BitmapDataInfo();

            // 1. Bitmap
            result.BackingImage = (Bitmap)SerializationHelper.ReadImage(br);

            // 2. Data
            result.BitmapData = new BitmapData
            {
                Width = br.ReadInt32(),
                Height = br.ReadInt32(),
                Stride = br.ReadInt32(),
                PixelFormat = (PixelFormat)br.ReadInt32()
            };

            BitmapDataInfo = result;
        }

        #endregion

        #endregion
    }
}
