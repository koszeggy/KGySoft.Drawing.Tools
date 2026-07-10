#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ProgressFooter.cs
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

#region Used Namespaces

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using KGySoft.WinForms;
using KGySoft.WinForms.Controls;

#endregion

#region Used Aliases

using Timer = System.Windows.Forms.Timer;

#endregion

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    /// <summary>
    /// A <see cref="Panel"/> with a progress bar that can be updated from any thread.
    /// </summary>
    internal class ProgressFooter<TProgress> : AutoMirrorPanel
    {
        #region Constants

        private const int refHeight = 20;

        #endregion

        #region Fields

        #region Static Fields

        [SuppressMessage("ReSharper", "StaticMemberInGenericType", Justification = "There are only two contstructed types, so doesn't really matter")]
        private static readonly Padding pnlProgressReferencePadding = new Padding(3, 3, 8, 3);

        #endregion

        #region Instance Fields

        private readonly Lock syncRoot = new();
        private readonly AdvancedLabel lblProgress;
        private readonly AdvancedProgressBar pbProgress;
        private readonly Panel pnlProgress;
        private readonly Timer timer;

        private bool progressVisible = true; // so ctor change will have effect at run-time
        private TProgress? progress;

        #endregion

        #endregion

        #region Properties

        #region Internal Properties
        
        [SuppressMessage("ReSharper", "LocalizableElement", Justification = "Whitespace")]
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
        protected bool IsMarquee { set => pbProgress.IsMarquee = value; }
        protected int Maximum { set => pbProgress.Maximum = value; }

        protected int Value
        {
            set
            {
                // Workaround for progress bar on Vista and above where it advances very slowly
                if (pbProgress.Style == AdvancedProgressBarStyle.System && ThemeColors.RenderWithVisualStyles && OSHelper.IsWindowsVistaOrLater && value > pbProgress.Value && value < pbProgress.Maximum)
                    pbProgress.Value = value + 1;
                pbProgress.Value = value;
            }
        }

        #endregion

        #endregion

        #region Constructors

        protected ProgressFooter()
        {
            Dock = DockStyle.Bottom;
            lblProgress = new AdvancedLabel
            {
                Name = nameof(lblProgress),
                Dock = DockStyle.Left,
                TextAlign = ContentAlignment.MiddleLeft
            };
            pnlProgress = new Panel
            {
                Name = nameof(pnlProgress),
                Dock = DockStyle.Fill,
                Padding = pnlProgressReferencePadding
            };
            pbProgress = new AdvancedProgressBar
            {
                Name = nameof(pbProgress),
                Dock = DockStyle.Fill,
                RightToLeftLayout = true,
            };
            pnlProgress.Controls.Add(pbProgress);
            Controls.AddRange([pnlProgress, lblProgress]);
            timer = new Timer { Interval = 30 };

            // DesignMode is false in the constructor
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            ProgressVisible = false;
            lblProgress.TextChanged += lblProgress_TextChanged;
            timer.Tick += timer_Tick;
        }

        #endregion

        #region Methods

        #region Internal Methods

        internal void AdjustSizes()
        {
            SuspendLayout();
            try
            {
                PointF scale = this.GetScale();
                Height = refHeight.Scale(scale.Y);
                pnlProgress.Padding = pnlProgressReferencePadding.Scale(scale);
                if (ProgressVisible)
                    ResetLabel();
            }
            finally
            {
                ResumeLayout();
            }
        }

        #endregion

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                timer.Dispose();

            lblProgress.TextChanged -= lblProgress_TextChanged;
            timer.Tick -= timer_Tick;
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods
        
        private void ResetLabel()
        {
            lblProgress.Width = 0;
            lblProgress.Width = lblProgress.PreferredWidth;
        }

        #endregion

        #region Event handlers

#pragma warning disable IDE1006 // Naming Styles

        private void lblProgress_TextChanged(object? sender, EventArgs e) => ResetLabel();
        private void timer_Tick(object? sender, EventArgs e) => UpdateProgress();

#pragma warning restore IDE1006 // Naming Styles
        #endregion

        #endregion
    }
}