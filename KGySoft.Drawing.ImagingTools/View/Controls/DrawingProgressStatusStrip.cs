#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DrawingProgressStatusStrip.cs
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
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    internal partial class DrawingProgressStatusStrip : StatusStrip
    {
        #region Fields

        private readonly bool visualStyles = Application.RenderWithVisualStyles;
        private bool progressVisible = true; // so ctor change will have effect at run-time
        private DrawingProgress? displayedProgress;

        #endregion

        #region Properties

        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Whitespace")]
        [SuppressMessage("ReSharper", "LocalizableElement", Justification = "Whitespace")]
        internal bool ProgressVisible
        {
            get => progressVisible;
            set
            {
                if (progressVisible == value)
                    return;
                progressVisible = value;
                if (value)
                    UpdateProgress(default);

                // In Windows we don't make label invisible but changing text to a space to prevent status strip height change
                if (OSUtils.IsWindows)
                {
                    pbProgress.Visible = progressVisible;
                    if (!progressVisible)
                        lblProgress.Text = " ";
                }
                // On Linux we let the progress bar remain visible for the same reason and to prevent (sort of) appearing ugly thick black areas
                else
                {
                    lblProgress.Visible = progressVisible;
                    if (!progressVisible)
                    {
                        pbProgress.Style = ProgressBarStyle.Blocks;
                        pbProgress.Value = 0;
                    }

                    AdjustSize();
                }

                timer.Enabled = value;
            }
        }

        internal DrawingProgress Progress { get; set; }

        #endregion

        #region Constructors

        public DrawingProgressStatusStrip()
        {
            InitializeComponent();
            if (DesignMode)
                return;
            this.FixAppearance();
            ProgressVisible = false;
            SizeChanged += DrawingProgressStatusStrip_SizeChanged;
            lblProgress.TextChanged += lblProgress_TextChanged;
            lblProgress.VisibleChanged += lblProgress_VisibleChanged;
            timer.Tick += timer_Tick;
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();

            SizeChanged -= DrawingProgressStatusStrip_SizeChanged;
            lblProgress.TextChanged -= lblProgress_TextChanged;
            lblProgress.VisibleChanged -= lblProgress_VisibleChanged;
            timer.Tick -= timer_Tick;
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void AdjustSize() =>
            pbProgress.Width = Width - (lblProgress.Visible ? lblProgress.Width - lblProgress.Margin.Horizontal : 0) - pbProgress.Margin.Horizontal - 2;

        private void UpdateProgress(DrawingProgress progress)
        {
            if (progress == displayedProgress)
                return;

            if (displayedProgress?.OperationType != progress.OperationType)
                lblProgress.Text = Res.Get(progress.OperationType);
            if (progress.MaximumValue == 0)
                pbProgress.Style = ProgressBarStyle.Marquee;
            else
            {
                pbProgress.Style = ProgressBarStyle.Blocks;
                pbProgress.Maximum = progress.MaximumValue;

                // Workaround for progress bar on Vista and above where it advances very slow
                if (OSUtils.IsVistaOrLater && visualStyles && progress.CurrentValue > pbProgress.Value && progress.CurrentValue < progress.MaximumValue)
                    pbProgress.Value = progress.CurrentValue + 1;
                pbProgress.Value = progress.CurrentValue;
            }

            displayedProgress = progress;
        }

        #endregion

        #region Event handlers
#pragma warning disable IDE1006 // Naming Styles

        private void lblProgress_TextChanged(object sender, EventArgs e) => AdjustSize();
        private void lblProgress_VisibleChanged(object sender, EventArgs e) => AdjustSize();
        private void DrawingProgressStatusStrip_SizeChanged(object sender, EventArgs e) => AdjustSize();

        private void timer_Tick(object sender, EventArgs e) => UpdateProgress(Progress);

#pragma warning restore IDE1006 // Naming Styles
        #endregion

        #endregion
    }
}