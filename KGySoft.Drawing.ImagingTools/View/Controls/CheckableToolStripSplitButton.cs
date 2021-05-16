#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CheckableToolStripSplitButton.cs
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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    /// <summary>
    /// A <see cref="ToolStripSplitButton"/> whose button part can be checked.
    /// </summary>
    // NOTE: Unlike ToolStripDropDownButton, ToolStripSplitButton is scaled well so no special handling is needed here
    // The properly scaled arrow and the checked appearance is rendered by ScalingToolStripMenuRenderer
    internal class CheckableToolStripSplitButton : ToolStripSplitButton
    {
        #region Fields

        private bool isChecked;

        #endregion

        #region Properties

        [DefaultValue(false)]
        public bool CheckOnClick { get; set; }

        [DefaultValue(false)]
        public bool Checked
        {
            get => isChecked;
            set
            {
                if (value == isChecked)
                    return;
                isChecked = value;
                OnCheckedChanged(EventArgs.Empty);
                Invalidate();
            }
        }

        #endregion

        #region Events

        public event EventHandler CheckedChanged
        {
            add => Events.AddHandler(nameof(CheckedChanged), value);
            remove => Events.RemoveHandler(nameof(CheckedChanged), value);
        }

        #endregion

        #region Methods

        protected override void OnButtonClick(EventArgs e)
        {
            if (CheckOnClick)
                Checked = !Checked;
            base.OnButtonClick(e);
        }

        protected virtual void OnCheckedChanged(EventArgs e) => (Events[nameof(CheckedChanged)] as EventHandler)?.Invoke(this, e);

        public override Size GetPreferredSize(Size constrainingSize)
        {
            if (Owner.Orientation == Orientation.Horizontal)
                return base.GetPreferredSize(constrainingSize);
            Size result = base.GetPreferredSize(constrainingSize);

            return new Size(result.Width, result.Height) + Owner.ScaleSize(new Size(2, 0));
        }

        #endregion
    }
}
