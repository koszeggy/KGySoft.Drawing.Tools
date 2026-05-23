#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdvancedToolStripSplitButton.cs
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
using System.Drawing;
using System.Windows.Forms;

using KGySoft.WinForms;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Components
{
    /// <summary>
    /// A <see cref="ToolStripSplitButton"/> with some additional features and fixes:
    /// - Checked property: the button part can be checked
    /// - CheckOnClick property
    /// - AutoChangeDefaultItem: clicking an item changes the default item
    /// - ButtonEnabled: allows disabling the button part only. When DefaultItem is set, it automatically reflects the Enabled property of the default item
    /// - DefaultItem: Like in the base, but with fixed OnDefaultItemChanged handling:
    ///   in the base the DefaultItem still returns the old value when the OnDefaultItemChanged method executes.
    /// - OnDefaultItemChanged: Sets button Image/Text/ToolTipText/Enabled properties from the default item
    /// </summary>
    // NOTE: The properly scaled arrow and the checked/disabled appearance is rendered by AdvancedToolStripRenderer, while
    // the drop-down button size is adjusted in AdvancedToolStrip for all ToolStripSplitButtons
    internal class AdvancedToolStripSplitButton : ToolStripSplitButton
    {
        #region Fields

        private bool isChecked;
        private bool autoChangeDefaultItem;
        private bool buttonEnabled = true;
        private bool suppressChanged;
        private ToolStripItem? lastDefaultItem;

        #endregion

        #region Properties

        #region Public Properties

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
                    DefaultItem = DropDownItems[0];
            }
        }

        [DefaultValue(true)]
        public bool ButtonEnabled
        {
            get => buttonEnabled;
            set
            {
                if (value == buttonEnabled)
                    return;
                buttonEnabled = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ToolStripItem? DefaultItem
        {
            get => base.DefaultItem;
            set
            {
                if (base.DefaultItem == value)
                    return;

                // Fixing OnDefaultItemChanged handling. In base, it is called BEFORE actually changing DefaultItem.
                suppressChanged = true;
                base.DefaultItem = value;
                suppressChanged = false;
                OnDefaultItemChanged(EventArgs.Empty);
            }
        }

        #endregion

        #region Internal Properties

        internal bool IsDropDownHovered { get; set; }

        #endregion

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
            if (Owner == null || Owner.Orientation == Orientation.Horizontal)
                return base.GetPreferredSize(constrainingSize);

            // with vertical orientation the image is too small
            Size result = base.GetPreferredSize(constrainingSize);
            return new Size(result.Width + Owner.ScaleWidth(2), result.Height);
        }

        #endregion

        #region Protected Methods

        protected override void OnButtonClick(EventArgs e)
        {
            if (!ButtonEnabled)
                return;
            if (CheckOnClick)
                Checked = !Checked;
            if (OSHelper.IsFrameworkMono)
                DefaultItem?.PerformClick();
            else
                base.OnButtonClick(e);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (Enabled && keyData is Keys.Enter or Keys.Space)
            {
                if (ButtonEnabled)
                    PerformButtonClick();
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        protected override bool ProcessMnemonic(char charCode) => !ButtonEnabled || base.ProcessMnemonic(charCode);

        protected virtual void OnCheckedChanged(EventArgs e) => (Events[nameof(CheckedChanged)] as EventHandler)?.Invoke(this, e);

        protected override void OnDropDownItemClicked(ToolStripItemClickedEventArgs e)
        {
            base.OnDropDownItemClicked(e);
            if (autoChangeDefaultItem && DefaultItem != e.ClickedItem && e.ClickedItem != null)
                DefaultItem = e.ClickedItem;
        }

        protected override void OnDefaultItemChanged(EventArgs e)
        {
            if (suppressChanged)
                return;

            lastDefaultItem?.EnabledChanged -= DefaultItemEnabledChanged;
            lastDefaultItem = DefaultItem;
            lastDefaultItem?.EnabledChanged += DefaultItemEnabledChanged;
            if (lastDefaultItem != null)
            {
                Image = lastDefaultItem.Image;
                Text = lastDefaultItem.Text;
                ToolTipText = lastDefaultItem.ToolTipText;
                ButtonEnabled = lastDefaultItem.Enabled;
            }

            base.OnDefaultItemChanged(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            bool isDropDownHovered = DropDownButtonBounds.Contains(e.Location);
            if (!ButtonEnabled && isDropDownHovered != IsDropDownHovered)
                Invalidate(DropDownButtonBounds);
            IsDropDownHovered = isDropDownHovered;
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            IsDropDownHovered = false;
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            IsDropDownHovered = DropDownButtonBounds.Contains(e.Location);
            if (ButtonEnabled || IsDropDownHovered)
                base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            IsDropDownHovered = DropDownButtonBounds.Contains(e.Location);
            if (ButtonEnabled || IsDropDownHovered)
                base.OnMouseUp(e);
        }

        protected override void Dispose(bool disposing)
        {
            lastDefaultItem?.EnabledChanged -= DefaultItemEnabledChanged;
            base.Dispose(disposing);
        }

        #endregion

        #region Event Handlers

        private void DefaultItemEnabledChanged(object? sender, EventArgs e)
        {
            Debug.Assert(DefaultItem == lastDefaultItem);
            ButtonEnabled = DefaultItem?.Enabled == true;
        }

        #endregion

        #endregion
    }
}
