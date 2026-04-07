#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: NotificationLabel.cs
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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.WinApi;
using KGySoft.WinForms;
using KGySoft.WinForms.Controls;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    internal class NotificationLabel : AdvancedLabel
    {
        #region Properties

        [AllowNull]
        public override string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
                Visible = !String.IsNullOrEmpty(value);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Image? Image
        {
            get => base.Image;
            set => base.Image = value;
        }

        #endregion

        #region Constructors

        public NotificationLabel()
        {
            AutoSize = true;
            BorderStyle = AdvancedBorderStyle.FixedSingle;
            BackColor = Color.FromArgb(255, 255, 128);
            ForeColor = Color.Black;
            TextAlign = ContentAlignment.MiddleLeft;
            ImageAlign = ContentAlignment.MiddleRight;
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            ResetIcon();
        }


        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            Visible = false;
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case Constants.WM_DPICHANGED_BEFOREPARENT:
                    base.WndProc(ref m);
                    ResetIcon();
                    return;

                default:
                    base.WndProc(ref m);
                    return;
            }
        }

        #endregion

        #region Private Methods

        private void ResetIcon()
        {
            Image? prevImage = Image;
            Image = Icons.SystemWarning.ToScaledBitmap(this.GetScale());
            prevImage?.Dispose();
        }

        #endregion

        #endregion
    }
}
