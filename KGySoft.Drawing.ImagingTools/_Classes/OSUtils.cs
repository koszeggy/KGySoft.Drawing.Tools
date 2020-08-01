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

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal static class OSUtils
    {
        #region Fields

        private static bool? isVistaOrLater;

        #endregion

        #region Properties

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
