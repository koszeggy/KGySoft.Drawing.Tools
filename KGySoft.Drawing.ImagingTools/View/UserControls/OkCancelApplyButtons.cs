#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: OkCancelApplyButtons.cs
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

using System.Windows.Forms;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal class OkCancelApplyButtons : OkCancelButtons
    {
        #region Properties

        internal Button ApplyButton { get; }

        #endregion

        #region Constructors

        public OkCancelApplyButtons()
        {
            pnlButtons.SuspendLayout();
            pnlButtons.ColumnCount = 3;
            foreach (ColumnStyle columnStyle in pnlButtons.ColumnStyles)
                columnStyle.Width = 33.3f;
            pnlButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3f));
            ApplyButton = new Button
            {
                Anchor = AnchorStyles.None,
                FlatStyle = FlatStyle.System,
                Name = "btnApply",
                TabIndex = 2,
                Text = @"btnApply",
                UseVisualStyleBackColor = true

            };
            pnlButtons.Controls.Add(ApplyButton, 2, 0);
            pnlButtons.ResumeLayout(false);
        }

        #endregion
    }
}