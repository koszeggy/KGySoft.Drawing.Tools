#if !NET5_0_OR_GREATER
#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: User32.cs
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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;

#endregion

namespace KGySoft.Drawing.ImagingTools.WinApi
{
    [SecurityCritical]
    internal static class User32
    {
        #region Nested classes

        #region NativeMethods class

        private static class NativeMethods
        {
            #region Methods

            [DllImport("user32.dll")]
            internal static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);

            #endregion
        }

        #endregion

        #endregion

        #region Methods

        internal static void ScreenToClient(Control control, ref Point point) => NativeMethods.ScreenToClient(control.Handle, ref point);

        #endregion
    }
}
#endif