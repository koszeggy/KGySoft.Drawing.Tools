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
    /// <summary>
    /// Represents a descriptor for a <see cref="System.Drawing.Imaging.BitmapData"/> instance that can be used
    /// to display arbitrary debug information.
    /// </summary>
    public sealed class BitmapDataInfo : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets or sets a <see cref="Bitmap"/> that represents the content of the <see cref="BitmapData"/>.
        /// </summary>
        public Bitmap? BackingImage { get; set; }

        /// <summary>
        /// Gets or sets the bitmap data.
        /// </summary>
        public BitmapData? BitmapData { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes an empty instance of the <see cref="BitmapDataInfo"/> class.
        /// The properties are expected to be initialized individually.
        /// </summary>
        public BitmapDataInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapDataInfo"/> class from a <see cref="System.Drawing.Imaging.BitmapData"/> instance.
        /// </summary>
        /// <param name="bitmapData">The bitmap data.</param>
        public BitmapDataInfo(BitmapData bitmapData)
        {
            if (bitmapData == null)
                throw new ArgumentNullException(nameof(bitmapData), PublicResources.ArgumentNull);
            BackingImage = new Bitmap(bitmapData.Width, bitmapData.Height, bitmapData.Stride, bitmapData.PixelFormat, bitmapData.Scan0);
            BitmapData = bitmapData;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Releases the resources held by this instance.
        /// </summary>
        public void Dispose() => BackingImage?.Dispose();

        #endregion
    }
}