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
#if !NET5_0_OR_GREATER
using System.Security;
using System.Collections.Specialized;
using System.Drawing;
using System.Reflection;

using KGySoft.Drawing.ImagingTools.WinApi;
#endif
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
        #region Constants

#if !NET5_0_OR_GREATER
        // ReSharper disable once InconsistentNaming
        private const int WM_NCHITTEST = 0x0084;
#endif

        #endregion

        #region Fields

#if !NET5_0_OR_GREATER
        private static readonly BitVector32.Section formStateRenderSizeGrip = OSUtils.IsWindows
            ? (BitVector32.Section)Reflector.GetField(typeof(Form), "FormStateRenderSizeGrip")!
            : default;
        private static FieldAccessor? formStateField;
#endif

        #endregion

        #region Properties

#if !NET5_0_OR_GREATER
        private BitVector32 FormState => (BitVector32)(formStateField ??=
            FieldAccessor.GetAccessor(typeof(Form).GetField("formState", BindingFlags.Instance | BindingFlags.NonPublic)!)).Get(this)!;
#endif

        #endregion

        #region Constructors

        static BaseForm()
        {
#if NETFRAMEWORK
            Type? dpiHelper = Reflector.ResolveType(typeof(Form).Assembly, "System.Windows.Forms.DpiHelper");
            if (dpiHelper == null)
                return;

            // Turning off WinForms auto resize logic to prevent interferences.
            // Occurs when executed as visualizer debugger and devenv.exe.config contains some random DpiAwareness
            Reflector.TrySetField(dpiHelper, "isInitialized", true);
            Reflector.TrySetField(dpiHelper, "enableHighDpi", false); 
#endif
        }

        #endregion

        #region Methods

        #region Protected Methods

#if !NET5_0_OR_GREATER
        protected override void WndProc(ref Message m)
        {
            if (!OSUtils.IsWindows)
            {
                base.WndProc(ref m);
                return;
            }

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
#endif

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                Events.Dispose();
        }

        #endregion

        #region Private Methods

#if !NET5_0_OR_GREATER
        /// <summary>
        /// Bugfix: When size grip is visible, and form is above and left of the primary monitor, form cannot be dragged anymore due to forced diagonal resizing.
        /// Note: Needed only below .NET 5.0 because I fixed this directly in WinForms repository: https://github.com/dotnet/winforms/pull/2032/commits
        /// </summary>
        [SecuritySafeCritical]
        private void WmNCHitTest(ref Message m)
        {
            if (FormState[formStateRenderSizeGrip] != 0)
            {
                // Here is the bug in original code: LParam contains two shorts. Without the cast negative values are positive ints
                Point pt = new Point(m.LParam.GetSignedLoWord(), m.LParam.GetSignedHiWord());
                User32.ScreenToClient(this, ref pt);
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
#endif

        #endregion

        #endregion
    }
}
