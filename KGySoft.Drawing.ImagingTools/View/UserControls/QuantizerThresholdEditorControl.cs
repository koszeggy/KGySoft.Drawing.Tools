#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: QuantizerThresholdEditorControl.cs
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
    internal partial class QuantizerThresholdEditorControl : BaseUserControl
    {
        #region Fields

        private readonly IWindowsFormsEditorService? editorService;
        private readonly byte originalValue;

        #endregion

        #region Properties

        internal byte Value { get; private set; }

        #endregion

        #region Constructors

        #region Internal Constructors

        internal QuantizerThresholdEditorControl(IWindowsFormsEditorService editorService, byte value) : this()
        {
            this.editorService = editorService;
            if (DesignMode)
                return;
            trackBar.ValueChanged += TrackBar_ValueChanged;
            okCancelButtons.CancelButton.Click += CancelButton_Click;
            okCancelButtons.OKButton.Click += OKButton_Click;
            okCancelButtons.ApplyStringResources();

            trackBar.Value = originalValue = value;
        }

        #endregion

        #region Private Constructors

        private QuantizerThresholdEditorControl()
        {
            RightToLeft = LanguageSettings.DisplayLanguage.TextInfo.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
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

        #region Event handlers

        private void TrackBar_ValueChanged(object? sender, EventArgs e)
        {
            Value = (byte)trackBar.Value;
            lblValue.Text = Value.ToString(CultureInfo.CurrentCulture);
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
