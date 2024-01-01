#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: WindowsUtils.cs
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
using System.Drawing;
using System.Linq;

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
        private static bool? isWin11OrLater;
        private static bool? isWindows;
        private static bool? isLinux;
        private static bool? isMono;

        #endregion

        #region Properties

        internal static bool IsWindows => isWindows ??= Environment.OSVersion.Platform is PlatformID.Win32NT or PlatformID.Win32Windows;
        internal static bool IsLinux => isLinux ??= Environment.OSVersion.Platform is PlatformID.Unix or (PlatformID)128;
        internal static bool IsMono => isMono ??= Type.GetType("Mono.Runtime") != null;

        internal static bool IsVistaOrLater
        {
            get
            {
                if (isVistaOrLater.HasValue)
                    return isVistaOrLater.Value;

                OperatingSystem os = Environment.OSVersion;
                if (os.Platform != PlatformID.Win32NT)
                {
                    isVistaOrLater = false;
                    return false;
                }

                isVistaOrLater = os.Version >= new Version(6, 0, 5243);
                return isVistaOrLater.Value;
            }
        }

        internal static bool IsWindows8OrLater
        {
            get
            {
                if (isWin8OrLater.HasValue)
                    return isWin8OrLater.Value;

                OperatingSystem os = Environment.OSVersion;
                if (os.Platform != PlatformID.Win32NT)
                {
                    isWin8OrLater = false;
                    return false;
                }

                isWin8OrLater = os.Version >= new Version(6, 2, 9200);
                return isWin8OrLater.Value;
            }
        }

        internal static bool IsWindows11OrLater
        {
            get
            {
                if (isWin11OrLater.HasValue)
                    return isWin11OrLater.Value;

                OperatingSystem osVer = Environment.OSVersion;
                if (osVer.Platform != PlatformID.Win32NT)
                {
                    isWin11OrLater = false;
                    return false;
                }

#if NETCOREAPP
                isWin11OrLater = osVer.Version >= new Version(10, 0, 22000);
#else
                // .NET Framework never returns a higher version than Windows 8, so we need to access the Registry
                const string path = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
                const string key = "CurrentBuild";
                try
                {
                    using RegistryKey? reg = Registry.LocalMachine.OpenSubKey(path);
                    isWin11OrLater = reg?.GetValueNames().Contains(key) == true
                        && reg.GetValue(key) is string value
                        && Int32.TryParse(value, out int buildNumber)
                        && buildNumber >= 22000;
                }
                catch (Exception e) when (!e.IsCritical())
                {
                    isWin11OrLater = false;
                }
#endif
                return isWin11OrLater.Value; 
            }
        }

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
            using (Graphics screen = Graphics.FromHwnd(handle))
                return new PointF(screen.DpiX, screen.DpiY);
        }

        #endregion

        #endregion
    }
}
