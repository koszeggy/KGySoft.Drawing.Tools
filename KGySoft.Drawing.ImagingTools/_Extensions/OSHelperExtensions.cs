#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: OSHelperExtensions.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2026 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;

using KGySoft.WinForms;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal static class OSHelperExtensions
    {
        extension(OSHelper)
        {
            #region Properties

            internal static bool IsWindowsMono => OSHelper.IsFrameworkMono && OSHelper.IsWindows;
            internal static bool IsLinuxMono => OSHelper.IsFrameworkMono && !OSHelper.IsWindows;

            internal static bool IsWindows10Build1903OrLater
                => isWindows10Build1903OrLater ??= OSHelper.GetWindowsVersion() is Version version && version >= new Version(10, 0, 18362);
            
            #endregion
        }

        #region Fields

        private static bool? isWindows10Build1903OrLater;

        #endregion
    }
}
