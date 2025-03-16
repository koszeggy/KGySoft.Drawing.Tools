#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: OkCancelButtons.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2024 - All Rights Reserved
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
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal sealed partial class OkCancelButtons : BaseUserControl
    {
        #region Fields

        private bool isApplyVisible;
        private bool areDefaultButtonsVisible = true;

        #endregion

        #region Properties

        #region Public Properties

        [DefaultValue(false)]
        public bool ApplyButtonVisible
        {
            get => isApplyVisible;
            set
            {
                if (isApplyVisible == value)
                    return;
                ApplyButton.Visible = isApplyVisible = value;
            }
        }

        #endregion

        #region Internal Properties

        internal Button OKButton => btnOK;
        internal Button CancelButton => btnCancel;
        internal Button ApplyButton => btnApply;

        internal bool DefaultButtonsVisible
        {
            get => areDefaultButtonsVisible;
            set
            {
                if (areDefaultButtonsVisible == value)
                    return;
                OKButton.Visible = CancelButton.Visible = areDefaultButtonsVisible = value;
            }
        }

        #endregion

        #endregion

        #region Constructors

        public OkCancelButtons() => InitializeComponent();

        #endregion

        #region Methods

        protected override void OnLoad(EventArgs e)
        {
            // Fixing high DPI appearance on Mono
            PointF scale;
            if (OSUtils.IsMono && (scale = this.GetScale()) != new PointF(1f, 1f))
            {
                Height = (int)(35 * scale.Y);
                var referenceButtonSize = new Size(75, 23);
                OKButton.Size = referenceButtonSize.Scale(scale);
                CancelButton.Size = referenceButtonSize.Scale(scale);
                ApplyButton.Size = referenceButtonSize.Scale(scale);
            }

            base.OnLoad(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();
            base.Dispose(disposing);
        }

        #endregion
    }
}