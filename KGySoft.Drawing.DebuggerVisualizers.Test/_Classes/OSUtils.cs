#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: WindowsUtils.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;

using KGySoft.CoreLibraries;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Test
{
    internal static class OSUtils
    {
        #region Fields

        private static bool? isWindows;
        private static bool? isMono;

        #endregion

        #region Properties

        internal static bool IsWindows => isWindows ??= Environment.OSVersion.Platform.In(PlatformID.Win32NT, PlatformID.Win32Windows);
        internal static bool IsMono => isMono ??= Type.GetType("Mono.Runtime") != null;

        #endregion
    }
}
