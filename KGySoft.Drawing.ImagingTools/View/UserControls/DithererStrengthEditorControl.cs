#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DithererStrengthEditorControl.cs
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
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.Design;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal partial class DithererStrengthEditorControl : BaseUserControl
    {
        #region Fields

        private readonly IWindowsFormsEditorService? editorService;
        private readonly float originalValue;

        #endregion

        #region Properties

        internal float Value { get; private set; }

        #endregion

        #region Constructors

        #region Internal Constructors

        internal DithererStrengthEditorControl(IWindowsFormsEditorService editorService, float value) : this()
        {
            this.editorService = editorService;
            if (DesignMode)
                return;
            trackBar.ValueChanged += TrackBar_ValueChanged;
            okCancelButtons.CancelButton.Click += CancelButton_Click;
            okCancelButtons.OKButton.Click += OKButton_Click;
            okCancelButtons.ApplyStaticStringResources();

            originalValue = value;
            trackBar.Value = value >= 0f && value <= 1f ? (int)(value * 100) : 0;
            UpdateLabel();
        }

        #endregion

        #region Private Constructors

        private DithererStrengthEditorControl()
        {
            InitializeComponent();
        }

        #endregion

        #endregion

        #region Methods

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();
            trackBar.ValueChanged -= TrackBar_ValueChanged;
            okCancelButtons.CancelButton.Click -= CancelButton_Click;
            okCancelButtons.OKButton.Click -= OKButton_Click;
            base.Dispose(disposing);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            // special handling for detecting Escape because without this the change is applied
            if (keyData == Keys.Escape)
                okCancelButtons.CancelButton.PerformClick();
            return base.ProcessDialogKey(keyData);
        }

        #endregion

        #region Private Methods

        private void UpdateLabel() => lblValue.Text = Value <= 0f ? Res.TextAuto : Value.ToString("F2", CultureInfo.CurrentCulture);

        #endregion

        #region Event handlers

        private void TrackBar_ValueChanged(object? sender, EventArgs e)
        {
            Value = trackBar.Value / 100f;
            UpdateLabel();
        }

        private void OKButton_Click(object? sender, EventArgs e)
        {
            editorService?.CloseDropDown();
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            Value = originalValue;
            editorService?.CloseDropDown();
        }

        #endregion

        #endregion
    }
}
