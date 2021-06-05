#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: LocalizationInfo.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    /// <summary>
    /// Represents the metadata of localized resources.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Setter accessors are needed for the deserializer")]
    [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "ReSharper issue")]
    public class LocalizationInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the culture name of the localization that matches the <see cref="CultureInfo.Name"/> property of the represented culture.
        /// </summary>
        public string CultureName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the description of the localization.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the version number of the <c>KGySoft.Drawing.ImagingTools.exe</c> assembly this localization belongs to.
        /// </summary>
        public Version ImagingToolsVersion { get; set; } = default!;

        /// <summary>
        /// Gets or sets the author of the localization.
        /// </summary>
        public string? Author { get; set; }

        /// <summary>
        /// Gets or sets the class libraries whose resources are covered by this localization.
        /// </summary>
        public LocalizableLibraries ResourceSets { get; set; } = default!;

        #endregion
    }
}