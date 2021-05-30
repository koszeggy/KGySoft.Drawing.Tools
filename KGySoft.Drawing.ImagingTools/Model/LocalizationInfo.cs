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
using System.Globalization;

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    internal class LocalizationInfo
    {
        #region Properties

        public CultureInfo Language { get; set; } = default!;
        public string? Description { get; set; }
        public ResourceLibraries ResourceSets { get; set; } = default!;
        public Version Version { get; set; } = default!;
        public Version ImagingToolsVersion { get; set; } = default!;
        public string? Author { get; set; }

        #endregion
    }
}