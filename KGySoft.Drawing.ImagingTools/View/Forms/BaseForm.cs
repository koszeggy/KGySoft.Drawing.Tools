#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: BaseForm.cs
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
using System.Collections.Specialized;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using KGySoft.Reflection;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    /// <summary>
    /// Copied from the KGySoft.Controls project for the resizing issue fix but used also for common DPI handling.
    /// </summary>
    internal class BaseForm : Form
    {
        #region NativeMethods class

        private static class NativeMethods
        {
            #region Methods

            [DllImport("user32.dll")]
            internal static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);

            #endregion
        }

        #endregion

        #region Constants

        private const int WM_NCHITTEST = 0x0084;

        #endregion

        #region Fields

        private static readonly BitVector32.Section formStateRenderSizeGrip = (BitVector32.Section)Reflector.GetField(typeof(Form), "FormStateRenderSizeGrip");
        private static FieldAccessor formStateField;

        #endregion

        #region Properties

        private BitVector32 FormState => (BitVector32)(formStateField ?? (formStateField = FieldAccessor.GetAccessor(typeof(Form).GetField("formState", BindingFlags.Instance | BindingFlags.NonPublic)))).Get(this);

        #endregion

        #region Constructors

        static BaseForm()
        {
            Type dpiHelper = Reflector.ResolveType(typeof(Form).Assembly, "System.Windows.Forms.DpiHelper");
            if (dpiHelper == null)
                return;

            // Turning off WinForms auto resize logic to prevent interferences.
            // Occurs when executed as visualizer debugger and devenv.exe.config contains some random DpiAwareness
            Reflector.TrySetField(dpiHelper, "isInitialized", true);
            Reflector.TrySetField(dpiHelper, "enableHighDpi", false);
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    WmNCHitTest(ref m);
                    return;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Bugfix: When size grip is visible, and form is above and left of the primary monitor, form cannot be dragged anymore due to forced diagonal resizing.
        /// Note: will be fixed in .NET Core (see also https://github.com/dotnet/winforms/issues/1504)
        /// </summary>
        private void WmNCHitTest(ref Message m)
        {
            if (FormState[formStateRenderSizeGrip] != 0)
            {
                // Here is the bug in original code: LParam contains two shorts. Without the cast negative values are positive ints
                int x = (short)(m.LParam.ToInt32() & 0xffff);
                int y = (short)((m.LParam.ToInt32() >> 16) & 0xffff);
                Point pt = new Point(x, y);
                NativeMethods.ScreenToClient(Handle, ref pt);
                Size clientSize = ClientSize;
                if (pt.X >= clientSize.Width - 16 && pt.Y >= clientSize.Height - 16 && clientSize.Height >= 16)
                {
                    m.Result = IsMirrored ? (IntPtr)16 : ((IntPtr)17);
                    return;
                }
            }

            DefWndProc(ref m);
            if (AutoSizeMode == AutoSizeMode.GrowAndShrink)
            {
                int result = (int)m.Result;
                if (result >= 10 && result <= 17)
                    m.Result = (IntPtr)18;
            }
        }

        #endregion

        #endregion
    }
}
