#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdvancedToolStripSplitButton.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
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

namespace KGySoft.Drawing.ImagingTools.View.Components
{
    /// <summary>
    /// A <see cref="ToolStripSplitButton"/> whose button part can be checked and the default item can automatically be changed.
    /// </summary>
    // NOTE: The properly scaled arrow and the checked appearance is rendered by ScalingToolStripMenuRenderer, while
    // the drop-down button size is adjusted in ScalingToolStrip for all ToolStripSplitButtons
    internal class AdvancedToolStripSplitButton : ToolStripSplitButton
    {
        #region Fields

        private bool isChecked;
        private bool autoChangeDefaultItem;

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

        [DefaultValue(false)]
        public bool AutoChangeDefaultItem
        {
            get => autoChangeDefaultItem;
            set
            {
                if (value == autoChangeDefaultItem)
                    return;
                autoChangeDefaultItem = value;
                if (value && DropDownItems.Count > 0)
                    SetDefaultItem(DropDownItems[0]);
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

        #region Public Methods

        public override Size GetPreferredSize(Size constrainingSize)
        {
            if (Owner.Orientation == Orientation.Horizontal)
                return base.GetPreferredSize(constrainingSize);

            // with vertical orientation the image is too small
            Size result = base.GetPreferredSize(constrainingSize);
            return new Size(result.Width + Owner.ScaleWidth(2), result.Height);
        }

        #endregion

        #region Internal Methods

        internal void SetDefaultItem(ToolStripItem item)
        {
            DefaultItem = item;
            Image = item.Image;
            Text = item.Text;
        }

        #endregion

        #region Protected Methods

        protected override void OnButtonClick(EventArgs e)
        {
            if (CheckOnClick)
                Checked = !Checked;
            if (OSUtils.IsMono)
                DefaultItem?.PerformClick();
            else
                base.OnButtonClick(e);
        }

        protected virtual void OnCheckedChanged(EventArgs e) => (Events[nameof(CheckedChanged)] as EventHandler)?.Invoke(this, e);

        protected override void OnDropDownItemClicked(ToolStripItemClickedEventArgs e)
        {
            base.OnDropDownItemClicked(e);
            if (autoChangeDefaultItem && DefaultItem != e.ClickedItem)
                SetDefaultItem(e.ClickedItem);
        }

        #endregion

        #endregion
    }
}
