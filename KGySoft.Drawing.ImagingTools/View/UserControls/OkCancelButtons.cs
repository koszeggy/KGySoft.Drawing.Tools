#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: OkCancelButtons.cs
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

        #endregion

        #endregion

        #region Constructors

        public OkCancelButtons()
        {
            AutoScaleMode = AutoScaleMode.Inherit;
            InitializeComponent();
        }

        #endregion

        #region Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();
            base.Dispose(disposing);
        }

        #endregion
    }
}