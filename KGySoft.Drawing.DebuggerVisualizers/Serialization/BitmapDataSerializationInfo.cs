#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: BitmapDataSerializationInfo.cs
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
using System.Drawing.Imaging;
using System.IO;

using KGySoft.Drawing.DebuggerVisualizers.Model;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Serialization
{
    internal class BitmapDataSerializationInfo : IDisposable
    {
        #region Properties

        internal BitmapDataInfo BitmapDataInfo { get; private set; }

        #endregion

        #region Constructors

        internal BitmapDataSerializationInfo(Stream stream)
        {
            ReadFrom(new BinaryReader(stream));
        }

        internal BitmapDataSerializationInfo(BitmapData bitmapData)
        {
            BitmapDataInfo = new BitmapDataInfo(bitmapData);
        }

        #endregion

        #region Methods

        #region Public Methods

        public void Dispose() => BitmapDataInfo?.Dispose();

        #endregion

        #region Internal Methods

        internal void Write(BinaryWriter bw)
        {
            // 1. Bitmap
            SerializationHelper.WriteImage(bw, BitmapDataInfo.Data);

            // 2. Info
            bw.Write(BitmapDataInfo.SpecialInfo);
        }

        #endregion

        #region Private Methods

        private void ReadFrom(BinaryReader br)
        {
            var result = new BitmapDataInfo();

            // 1. Bitmap
            result.Data = (Bitmap)SerializationHelper.ReadImage(br);

            // 2. Info
            result.SpecialInfo = br.ReadString();

            BitmapDataInfo = result;
        }

        #endregion

        #endregion
    }
}
