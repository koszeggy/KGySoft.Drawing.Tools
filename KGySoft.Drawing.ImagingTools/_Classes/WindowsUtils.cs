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
using System.Runtime.InteropServices;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal static class WindowsUtils
    {
        #region NativeMethods class

        private static class NativeMethods
        {
            #region Constants

            internal const int LOGPIXELSX = 88;

            #endregion

            #region Methods

            /// <summary>
            /// Retrieves device-specific information for the specified device.
            /// </summary>
            /// <param name="hdc">A handle to the DC.</param>
            /// <param name="nIndex">The item to be returned.</param>
            [DllImport("gdi32.dll")]
            internal static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

            #endregion
        }

        #endregion

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

        internal static int SystemDpi => GetControlDpi(IntPtr.Zero);

        #endregion

        #region Methods

        private static int GetControlDpi(IntPtr handle)
        {
            using (Graphics screen = Graphics.FromHwnd(handle))
            {
                IntPtr hdc = screen.GetHdc();
                try
                {
                    return NativeMethods.GetDeviceCaps(hdc, NativeMethods.LOGPIXELSX);
                }
                finally
                {
                    screen.ReleaseHdc(hdc);
                }
            }
        }

        #endregion
    }
}
