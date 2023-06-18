#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CustomColorInfo.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2023 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using KGySoft.Drawing.Imaging;

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    /// <summary>
    /// Represents a descriptor for any color type.
    /// </summary>
    public sealed class CustomColorInfo : CustomObjectInfoBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the color to display.
        /// </summary>
        public Color32 DisplayColor { get; set; }

        /// <summary>
        /// Gets or sets a specific display name for the color.
        /// </summary>
        public string? Name { get; set; }

        #endregion
    }
}