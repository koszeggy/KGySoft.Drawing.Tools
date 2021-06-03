#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: VersionExtensions.cs
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

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal static class VersionExtensions
    {
        #region Methods

        internal static Version Normalize(this Version version)
            => version.Revision >= 0 ? version
                : version.Build >= 0 ? new Version(version.Major, version.Minor, version.Build, 0)
                : new Version(version.Major, version.Minor, version.Build, version.Revision);

        #endregion
    }
}