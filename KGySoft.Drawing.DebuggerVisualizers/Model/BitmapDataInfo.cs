#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: BitmapDataInfo.cs
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

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Model
{
    internal sealed class BitmapDataInfo : IDisposable
    {
        #region Properties

        internal Bitmap Data { get; set; }
        internal string SpecialInfo { get; set; }

        #endregion

        #region Constructors

        internal BitmapDataInfo()
        {
        }

        internal BitmapDataInfo(BitmapData bitmapData)
        {
            Data = new Bitmap(bitmapData.Width, bitmapData.Height, bitmapData.Stride, bitmapData.PixelFormat, bitmapData.Scan0);
            SpecialInfo = String.Format("Size: {1}{0}Stride: {2} bytes{0}Pixel Format: {3}",
                Environment.NewLine, new Size(bitmapData.Width, bitmapData.Height), bitmapData.Stride, bitmapData.PixelFormat);
        }

        #endregion

        #region Methods

        public void Dispose() => Data?.Dispose();

        #endregion
    }
}