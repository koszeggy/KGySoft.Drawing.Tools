#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: WindowsUtils.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2019 - All Rights Reserved
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

namespace KGySoft.Drawing.ImagingTools
{
    internal static class OSUtils
    {
        #region Fields

        private static bool? isVistaOrLater;
        private static bool? isWin8OrLater;
        private static bool? isWindows;
        private static bool? isLinux;
        private static bool? isMono;

        #endregion

        #region Properties

        internal static bool IsWindows => isWindows ??= Environment.OSVersion.Platform.In(PlatformID.Win32NT, PlatformID.Win32Windows);
        internal static bool IsLinux => isLinux ??= Environment.OSVersion.Platform.In(PlatformID.Unix, (PlatformID)128);
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
