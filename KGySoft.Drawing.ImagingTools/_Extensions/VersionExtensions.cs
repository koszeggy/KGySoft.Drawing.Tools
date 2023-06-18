#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: VersionExtensions.cs
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

using System;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal static class VersionExtensions
    {
        #region Methods

        internal static bool NormalizedEquals(this Version? version, Version? other)
        {
            if (version is null && other is null)
                return true;
            if (version is null || other is null)
                return false;
            return version.Major == other.Major
                && version.Minor == other.Minor
                && (version.Build == other.Build || version.Build <= 0 && other.Build <= 0)
                && (version.Revision == other.Revision || version.Revision <= 0 && other.Revision <= 0);
        }

        #endregion
    }
}