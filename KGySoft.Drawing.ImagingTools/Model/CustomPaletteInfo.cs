﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CustomPaletteInfo.cs
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

using System.Collections.Generic;

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    /// <summary>
    /// Represents a descriptor for any palette type.
    /// </summary>
    public class CustomPaletteInfo : CustomObjectInfoBase
    {
        #region Properties

        /// <summary>
        /// Gets the palette entries to display.
        /// </summary>
        public IList<CustomColorInfo> Entries { get; } = new List<CustomColorInfo>();

        #endregion
    }
}