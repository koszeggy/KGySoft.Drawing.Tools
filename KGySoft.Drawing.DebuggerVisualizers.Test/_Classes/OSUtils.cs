#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: WindowsUtils.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2020 - All Rights Reserved
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
using System.Drawing;

using KGySoft.CoreLibraries;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Test
{
    internal static class OSUtils
    {
        #region Fields

        private static bool? isWindows;

        #endregion

        #region Properties

        internal static bool IsWindows => isWindows ??= Environment.OSVersion.Platform.In(PlatformID.Win32NT, PlatformID.Win32Windows);

        #endregion
    }
}
