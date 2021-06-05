#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: LocalizableLibraries.cs
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

namespace KGySoft.Drawing.ImagingTools.Model
{
    /// <summary>
    /// Represents the known class libraries with localizable resources.
    /// </summary>
    [Flags]
    public enum LocalizableLibraries
    {
        /// <summary>
        /// Represents none of the localizable libraries.
        /// </summary>
        None,

        /// <summary>
        /// Represents the <c>KGySoft.CoreLibraries.dll</c> assembly.
        /// </summary>
        CoreLibraries = 1,

        /// <summary>
        /// Represents the <c>KGySoft.Drawing.dll</c> assembly.
        /// </summary>
        DrawingLibraries = 1 << 1,

        /// <summary>
        /// Represents the <c>KGySoft.Drawing.ImagingTools.exe</c> assembly.
        /// </summary>
        ImagingTools = 1 << 2
    }
}