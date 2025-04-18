#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: WindowsUtils.cs
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
using System.Drawing;

#if NETFRAMEWORK
using Microsoft.Win32;
#endif

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal static class OSUtils
    {
        #region Fields

        private static bool? isVistaOrLater;
        private static bool? isWin8OrLater;
        private static bool? isWin10OrLater;
        private static bool? isWindows10Build1903OrLater;
        private static bool? isWin11OrLater;
        private static bool? isWindows;
        private static bool? isLinux;
        private static bool? isMono;
        private static Version? windowsVersion;

        #endregion

        #region Properties

        internal static bool IsWindows => isWindows ??= Environment.OSVersion.Platform is PlatformID.Win32NT or PlatformID.Win32Windows;
        internal static bool IsLinux => isLinux ??= Environment.OSVersion.Platform is PlatformID.Unix or (PlatformID)128;
        internal static bool IsMono => isMono ??= Type.GetType("Mono.Runtime") != null;

        internal static bool IsVistaOrLater
            => isVistaOrLater ??= GetWindowsVersion() is Version version && version >= new Version(6, 0, 5243);

        internal static bool IsWindows8OrLater
            => isWin8OrLater ??= GetWindowsVersion() is Version version && version >= new Version(6, 2, 9200);

        internal static bool IsWindows10OrLater
            // In fact, the October 2018 release, version 1809, the first one with dark theme support
            => isWin10OrLater ??= GetWindowsVersion() is Version version && version >= new Version(10, 0, 17763);

        internal static bool IsWindows10Build1903OrLater
            => isWindows10Build1903OrLater ??= GetWindowsVersion() is Version version && version >= new Version(10, 0, 18362);

        internal static bool IsWindows11OrLater
            => isWin11OrLater ??= GetWindowsVersion() is Version version && version >= new Version(10, 0, 22000);

        internal static PointF SystemScale => GetScale(IntPtr.Zero);
        internal static PointF SystemDpi => GetDpiForHwnd(IntPtr.Zero);

        #endregion

        #region Methods

        #region Internal Methods

        internal static PointF GetScale(IntPtr handle)
        {
            var dpi = GetDpiForHwnd(handle);
            return new PointF(dpi.X / 96f, dpi.Y / 96f);
        }

        #endregion

        #region Private Methods

        private static PointF GetDpiForHwnd(IntPtr handle)
        {
            using Graphics screen = Graphics.FromHwnd(handle);
            return new PointF(screen.DpiX, screen.DpiY);
        }

        private static Version? GetWindowsVersion()
        {
            if (windowsVersion is not null)
                return windowsVersion;
            OperatingSystem osVer = Environment.OSVersion;
            if (osVer.Platform != PlatformID.Win32NT)
                return null;

#if NETCOREAPP
            windowsVersion = osVer.Version;
#else
            if (osVer.Version != new Version(6, 2, 9200, 0))
                windowsVersion = osVer.Version;
            else
            {
                // .NET Framework never returns a higher version than Windows 8, so we need to access the Registry
                // NOTE: This could be fixed by an app.manifest file with supportedOS element, but it would only when ImagingTools is executed directly.
                const string path = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
                const string keyLcuVer = "LCUVer";
                const string keyMajor = "CurrentMajorVersionNumber";
                const string keyMinor = "CurrentMinorVersionNumber";
                const string keyBuild = "CurrentBuild";
                const int defaultMajor = 10;
                const int defaultMinor = 0;
                try
                {
                    using RegistryKey? reg = Registry.LocalMachine.OpenSubKey(path);
                    if (reg == null)
                        windowsVersion = osVer.Version;
                    else if (reg.GetValue(keyLcuVer) is string versionString && VersionExtensions.TryParse(versionString, out Version? version))
                        windowsVersion = version;
                    else if (reg.GetValue(keyBuild) is string build && Int32.TryParse(build, out int buildNumber))
                        windowsVersion = new Version(reg.GetValue(keyMajor, defaultMajor) is int major ? major : defaultMajor,
                            reg.GetValue(keyMinor, defaultMinor) is int minor ? minor : defaultMinor,
                            buildNumber);
                    else
                        windowsVersion = osVer.Version;
                }
                catch (Exception e) when (!e.IsCritical())
                {
                    windowsVersion = osVer.Version;
                }
            }
#endif
            return windowsVersion;
        }

        #endregion

        #endregion
    }
}
