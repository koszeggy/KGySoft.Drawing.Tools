#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CustomBitmapInfo.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2022 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;

using KGySoft.Drawing.Imaging;

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    /// <summary>
    /// Represents a descriptor for any bitmap that can represented by an <see cref="IReadableBitmapData"/>.
    /// </summary>
    public sealed class CustomBitmapInfo : CustomObjectInfoBase, IDisposable
    {
        #region Fields

        private readonly bool disposeBitmapData;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the bitmap data.
        /// </summary>
        public IReadableBitmapData? BitmapData { get; set; }
        
        /// <summary>
        /// Gets or sets whether the size of <see cref="BitmapData"/> represents the actual pixel size of the custom bitmap.
        /// </summary>
        public bool ShowPixelSize { get; set; }
        
        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of the <see cref="CustomBitmapInfo"/> class.
        /// </summary>
        /// <param name="disposeBitmapData"><see langword="true"/> to dispose <see cref="BitmapData"/> when this instance is disposed; otherwise, <see langword="false"/>.</param>
        public CustomBitmapInfo(bool disposeBitmapData) => this.disposeBitmapData = disposeBitmapData;

        #endregion

        #region Methods

        /// <summary>
        /// Releases the resources held by this instance.
        /// </summary>
        public void Dispose()
        {
            if (disposeBitmapData)
            {
                BitmapData?.Dispose();
                BitmapData = null;
            }
        }

        #endregion
    }
}
