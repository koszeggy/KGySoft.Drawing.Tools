#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ProgressStatusStrip.cs
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
    /// <summary>
    /// A <see cref="StatusStrip"/> with a progress bar that can be updated from any thread.
    /// </summary>
    internal partial class ProgressStatusStrip<TProgress> : StatusStrip
    {
        #region Fields

        private readonly object syncRoot = new object();
        private readonly bool visualStyles = Application.RenderWithVisualStyles;

        private bool progressVisible = true; // so ctor change will have effect at run-time
        private TProgress? progress;

        #endregion

        #region Properties

        #region Internal Properties
        
        [SuppressMessage("ReSharper", "LocalizableElement", Justification = "Whitespace")]
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "ReSharper issue")]
        internal virtual bool ProgressVisible
        {
            get => progressVisible;
            set
            {
                if (progressVisible == value)
                    return;
                progressVisible = value;
                if (value)
                {
                    Progress = default;
                    UpdateProgress();
                }

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

        internal TProgress? Progress
        {
            get
            {
                lock (syncRoot)
                    return progress;
            }
            set
            {
                lock (syncRoot)
                    progress = value;
            }
        }

        #endregion

        #region Protected Properties

        protected virtual void UpdateProgress() => throw new InvalidOperationException(Res.InternalError($"{nameof(UpdateProgress)} is not overridden"));

        protected string ProgressText { set => lblProgress.Text = value; }
        protected ProgressBarStyle ProgressStyle { set => pbProgress.Style = value; }
        protected int Maximum { set => pbProgress.Maximum = value; }

        protected int Value
        {
            set
            {
                // Workaround for progress bar on Vista and above where it advances very slow
                if (OSUtils.IsVistaOrLater && visualStyles && value > pbProgress.Value && value < pbProgress.Maximum)
                    pbProgress.Value = value + 1;
                pbProgress.Value = value;
            }
        }

        #endregion

        #endregion

        #region Constructors

        public ProgressStatusStrip()
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

        #endregion

        #region Event handlers
#pragma warning disable IDE1006 // Naming Styles

        private void lblProgress_TextChanged(object? sender, EventArgs e) => AdjustSize();
        private void lblProgress_VisibleChanged(object? sender, EventArgs e) => AdjustSize();
        private void DrawingProgressStatusStrip_SizeChanged(object? sender, EventArgs e) => AdjustSize();

        private void timer_Tick(object? sender, EventArgs e) => UpdateProgress();

#pragma warning restore IDE1006 // Naming Styles
        #endregion

        #endregion
    }
}