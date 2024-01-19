#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: PathHelper.cs
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

using System.IO;
using System.Linq;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal static class PathHelper
    {
        #region Fields

        private static readonly char[] invalidPathChars = Path.GetInvalidPathChars().Concat(new[] { '*', '?' }).ToArray();

        #endregion

        #region Methods

        internal static bool HasInvalidChars(string? path) => path?.IndexOfAny(invalidPathChars) >= 0;

        #endregion
    }
}