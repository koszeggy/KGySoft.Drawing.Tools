#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: LocalizationInfo.cs
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
    public class LocalizationInfo
    {
        #region Fields

        private Version? version;

        #endregion

        #region Properties

        #region Public Properties

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
        public string ImagingToolsVersion { get; set; } = default!;

        /// <summary>
        /// Gets or sets the author of the localization.
        /// </summary>
        public string? Author { get; set; }

        /// <summary>
        /// Gets or sets the class libraries whose resources are covered by this localization.
        /// </summary>
        public LocalizableLibraries ResourceSets { get; set; } = default!;

        #endregion

        #region Internal Properties

        // Note: Not a public property because XML deserialization may fail in very special circumstances
        // (eg. executing as debugger visualizer in Windows 7 with VS2015).
        internal Version Version
        {
            get
            {
                if (version is null)
                {
                    try
                    {
                        version = new Version(ImagingToolsVersion);
                    }
                    catch (Exception e) when (!e.IsCritical())
                    {
                        version = new Version();
                    }
                }

                return version;
            }
        }

        #endregion

        #endregion
    }
}