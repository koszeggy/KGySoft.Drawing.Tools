#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CustomColorInfo.cs
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

using System.Collections.Generic;

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

        /// <summary>
        /// Gets or sets custom color components. If set, then the regular ARGB color components will not be displayed.
        /// If there are more than four components, then they will be displayed as if they have been added
        /// to <see cref="CustomObjectInfoBase.CustomAttributes"/>. The order of the components are guaranteed to be preserved.
        /// </summary>
        public KeyValuePair<string, string>[]? CustomColorComponents { get; set; }

        #endregion
    }
}