#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CustomObjectInfoBase.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2025 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System.Collections.Generic;

using KGySoft.Collections;

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    /// <summary>
    /// Represents a base class for displayable objects with custom properties.
    /// </summary>
    public abstract class CustomObjectInfoBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the type represented by this <see cref="CustomObjectInfoBase"/> instance.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Gets a dictionary that can be populated by custom attributes that will be displayed as debug information.
        /// </summary>
        public IDictionary<string, string> CustomAttributes { get; } = new StringKeyedDictionary<string>();

        #endregion
    }
}