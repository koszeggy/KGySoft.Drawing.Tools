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
using System.Collections.Generic;

using KGySoft.Collections;
using KGySoft.Drawing.Imaging;

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    /// <summary>
    /// Represents a descriptor for any bitmap that can represented by an <see cref="IReadableBitmapData"/>.
    /// </summary>
    public sealed class CustomBitmapInfo : IDisposable
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
        /// Gets or sets the name of the type represented by the <see cref="BitmapData"/> property.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets whether the size of <see cref="BitmapData"/> represents the actual pixel size of the custom bitmap.
        /// </summary>
        public bool ShowPixelSize { get; set; }

        /// <summary>
        /// Gets a dictionary that can be populated by custom attributes that will be displayed as debug information.
        /// </summary>
        public IDictionary<string, string> CustomAttributes { get; } = new StringKeyedDictionary<string>();

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
