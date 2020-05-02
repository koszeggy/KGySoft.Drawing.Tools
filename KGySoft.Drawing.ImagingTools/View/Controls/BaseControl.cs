#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: BaseControl.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2020 - All Rights Reserved
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
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.WinApi;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    internal class BaseControl : Control
    {
        #region Events

        internal event EventHandler<HandledMouseEventArgs> MouseHWheel
        {
            add => Events.AddHandler(nameof(MouseHWheel), value);
            remove => Events.RemoveHandler(nameof(MouseHWheel), value);
        }

        #endregion

        #region Methods

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                // Horizontal scroll
                case Constants.WM_MOUSEHWHEEL:
                    HandledMouseEventArgs args = new HandledMouseEventArgs(MouseButtons.None, 0,
                            m.LParam.GetSignedLoWord(), m.LParam.GetSignedHiWord(), m.WParam.GetSignedHiWord());
                    OnMouseHWheel(args);
                    m.Result = new IntPtr(args.Handled ? 0 : 1);
                    if (args.Handled)
                        return;
                    DefWndProc(ref m);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        protected virtual void OnMouseHWheel(HandledMouseEventArgs e) => Events.GetHandler<EventHandler<HandledMouseEventArgs>>(nameof(MouseHWheel))?.Invoke(this, e);

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                Events.Dispose();
        }

        #endregion
    }
}
