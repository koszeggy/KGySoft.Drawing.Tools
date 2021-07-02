#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ProgressFooter.cs
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
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    /// <summary>
    /// A <see cref="Panel"/> with a progress bar that can be updated from any thread.
    /// </summary>
    internal class ProgressFooter<TProgress> : AutoMirrorPanel
    {
        #region Fields

        private readonly object syncRoot = new object();
        private readonly bool visualStyles = Application.RenderWithVisualStyles;
        private readonly Label lblProgress;
        private readonly ProgressBar pbProgress;
        private readonly Timer timer;

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

                lblProgress.Visible = pbProgress.Visible = value;
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

        protected override Padding DefaultPadding => new Padding(3, 3, 8, 3);

        #endregion

        #endregion

        #region Constructors

        protected ProgressFooter()
        {
            Dock = DockStyle.Bottom;
            lblProgress = new Label
            {
                Name = nameof(lblProgress),
                Dock = DockStyle.Left,
                TextAlign = ContentAlignment.MiddleLeft
            };
            pbProgress = new ProgressBar
            {
                Name = nameof(pbProgress),
                Dock = DockStyle.Fill,
                RightToLeftLayout = true,
            };
            Controls.AddRange(new Control[] { pbProgress, lblProgress });
            timer = new Timer { Interval = 30 };

            if (DesignMode)
                return;

            ProgressVisible = false;
            lblProgress.TextChanged += lblProgress_TextChanged;
            timer.Tick += timer_Tick;
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            // Fixing high DPI appearance on Mono
            PointF scale;
            if (OSUtils.IsMono && (scale = this.GetScale()) != new PointF(1f, 1f))
                Height = (int)(22 * scale.Y);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                timer.Dispose();

            lblProgress.TextChanged -= lblProgress_TextChanged;
            timer.Tick -= timer_Tick;
            base.Dispose(disposing);
        }

        #endregion

        #region Event handlers
#pragma warning disable IDE1006 // Naming Styles

        private void lblProgress_TextChanged(object? sender, EventArgs e) => lblProgress.Width = lblProgress.PreferredWidth;

        private void timer_Tick(object? sender, EventArgs e) => UpdateProgress();

#pragma warning restore IDE1006 // Naming Styles
        #endregion

        #endregion
    }
}