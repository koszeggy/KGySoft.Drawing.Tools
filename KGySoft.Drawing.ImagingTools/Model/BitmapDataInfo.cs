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

namespace KGySoft.Drawing.ImagingTools.Model
{
    public sealed class BitmapDataInfo : IDisposable
    {
        #region Properties

        public Bitmap BackingImage { get; set; }
        public BitmapData BitmapData { get; set; }

        #endregion

        #region Constructors

        public BitmapDataInfo()
        {
        }

        public BitmapDataInfo(BitmapData bitmapData)
        {
            BackingImage = new Bitmap(bitmapData.Width, bitmapData.Height, bitmapData.Stride, bitmapData.PixelFormat, bitmapData.Scan0);
            BitmapData = bitmapData;
        }

        #endregion

        #region Methods

        public void Dispose() => BackingImage?.Dispose();

        #endregion
    }
}