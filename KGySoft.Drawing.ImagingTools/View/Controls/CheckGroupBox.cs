#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CheckGroupBox.cs
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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

using KGySoft.CoreLibraries;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    internal partial class CheckGroupBox : GroupBox, ICustomLocalizable
    {
        #region Events

        internal event EventHandler CheckedChanged
        {
            add => Events.AddHandler(nameof(CheckedChanged), value);
            remove => Events.RemoveHandler(nameof(CheckedChanged), value);
        }

        #endregion

        #region Properties

        #region Public Properties

        [Localizable(true)]
        public new string Text
        {
            get => checkBox.Text;
            set => checkBox.Text = value;
        }

        [DefaultValue(true)]
        public bool Checked
        {
            get => checkBox.Checked;
            set => checkBox.Checked = value;
        }

        #endregion

        #region Internal Properties

        internal CheckBox CheckBox => checkBox;

        #endregion

        #region Protected Properties

        protected override Padding DefaultPadding => new Padding(3, 5, 3, 3);

        #endregion

        #endregion

        #region Constructors

        [SuppressMessage("ReSharper", "LocalizableElement", Justification = "Whitespace")]
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "ReSharper issue")]
        public CheckGroupBox()
        {
            InitializeComponent();
            Controls.Add(checkBox);
            checkBox.SizeChanged += CheckBox_SizeChanged;

            // Vista or later: using System FlayStyle so animation is enabled with theming and text is not misplaced with classic themes
            bool visualStylesEnabled = Application.RenderWithVisualStyles;
            checkBox.FlatStyle = OSUtils.IsMono ? FlatStyle.Standard
                : OSUtils.IsVistaOrLater ? FlatStyle.System
                // Windows XP: Using standard style with themes so CheckBox color can be set correctly, and using System with classic theme for good placement
                : visualStylesEnabled ? FlatStyle.Standard : FlatStyle.System;

            // GroupBox.FlayStyle must be the same as CheckBox; otherwise, System appearance would be transparent
            FlatStyle = checkBox.FlatStyle;
            checkBox.CheckedChanged += CheckBox_CheckedChanged;

            // making sure there is enough space before the CheckBox at every DPI
            base.Text = "   ";

            // Making sure that text color is correct with themes; may not work with System style)
            if (visualStylesEnabled)
                checkBox.ForeColor = new VisualStyleRenderer(VisualStyleElement.Button.GroupBox.Normal).GetColor(ColorProperty.TextColor);
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (DesignMode || e.Control.In(checkBox, contentPanel))
                return;

            // Linux/Mono workaround: prevent disabling ErrorProvider's user control when the content is disabled
            if (!OSUtils.IsWindows && e.Control.GetType().DeclaringType == typeof(ErrorProvider))
                return;

            // when not in design mode, adding custom controls to a panel so we can toggle its Enabled with preserving their original state
            contentPanel.Parent ??= this;
            e.Control.Parent = contentPanel;
        }

        protected virtual void OnCheckedChanged(EventArgs e) => (Events[nameof(CheckedChanged)] as EventHandler)?.Invoke(this, e);

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            ResetCheckBoxLocation();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                Events.Dispose();
            }

            checkBox.CheckedChanged -= CheckBox_CheckedChanged;
            checkBox.SizeChanged -= CheckBox_SizeChanged;
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void ResetCheckBoxLocation()
            => checkBox.Left = RightToLeft == RightToLeft.No
                ? (int)(10 * this.GetScale().X)
                : Width - checkBox.Width - (int)(10 * this.GetScale().X);

        #endregion

        #region Event handlers

        private void CheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            // Toggling the Enabled state of the content. This method preserves the original Enabled state of the controls.
            contentPanel.Enabled = checkBox.Checked;
            OnCheckedChanged(EventArgs.Empty);
        }

        private void CheckBox_SizeChanged(object? sender, EventArgs e) => ResetCheckBoxLocation();

        #endregion

        #region Explicitly Implemented Interface Methods

        void ICustomLocalizable.ApplyStringResources(ToolTip? toolTip)
        {
            string? name = Name;
            if (String.IsNullOrEmpty(name))
                return;

            // Self properties
            Res.ApplyStringResources(this, name);

            // tool tip: forwarding to the check box
            if (toolTip != null)
            {
                string? value = Res.GetStringOrNull(name + "." + ControlExtensions.ToolTipPropertyName);
                toolTip.SetToolTip(checkBox, value);
            }

            // children: only contentPanel controls so checkBox is skipped (otherwise, could be overwritten by checkbox.Name)
            foreach (Control child in contentPanel.Controls)
                child.ApplyStringResources(toolTip);
        }

        #endregion

        #endregion
    }
}
