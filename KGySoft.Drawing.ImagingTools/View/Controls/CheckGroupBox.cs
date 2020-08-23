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
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    internal partial class CheckGroupBox : GroupBox
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

        #region Protected Properties

        protected override Padding DefaultPadding => new Padding(3, 5, 3, 3);

        #endregion

        #endregion

        #region Constructors

        public CheckGroupBox()
        {
            InitializeComponent();
            Controls.Add(checkBox);

            // Left should be 10 at 100% but only 8 at 175%, etc.
            checkBox.Left = Math.Max(1, 13 - (int)(this.GetScale().X * Padding.Left));

            // Vista or later: System FlayStyle so animation is enabled with theming and while text is not misplaced with classic themes
            bool visualStylesEnabled = Application.RenderWithVisualStyles;
            checkBox.FlatStyle = OSUtils.IsVistaOrLater ? FlatStyle.System
                // Windows XP: Using standard style with themes so CheckBox color can be set correctly, and using System with classic theme for good placement
                : OSUtils.IsWindows ? visualStylesEnabled ? FlatStyle.Standard : FlatStyle.System
                // Non-windows (eg. Mono/Linux): Standard for best placement
                : FlatStyle.Standard;

            // GroupBox.FlayStyle must be the same as CheckBox; otherwise, System appearance would be transparent
            base.FlatStyle = checkBox.FlatStyle;
            checkBox.CheckedChanged += this.CheckBox_CheckedChanged;

            // making sure there is enough space before the CheckBox at every DPI
            base.Text = "   ";

            // Making sure that text color is correct with themes (may not work with System style)
            if (visualStylesEnabled)
                checkBox.ForeColor = new VisualStyleRenderer(VisualStyleElement.Button.GroupBox.Normal).GetColor(ColorProperty.TextColor);
        }

        #endregion

        #region Methods

        #region Protected Methods
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                Events.Dispose();
            }

            checkBox.CheckedChanged -= CheckBox_CheckedChanged;
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void OnCheckedChanged(EventArgs e) => (Events[nameof(CheckedChanged)] as EventHandler)?.Invoke(this, e);

        #endregion

        #region Event handlers

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Known issues: toggling Checked disables/enables every top-level control regardless of their initial state
            // Dynamically added controls are ignored
            bool enabled = checkBox.Checked;
            foreach (Control control in Controls)
            {
                if (control == checkBox)
                    continue;
                control.Enabled = enabled;
            }

            OnCheckedChanged(EventArgs.Empty);
        }

        #endregion

        #endregion
    }
}
