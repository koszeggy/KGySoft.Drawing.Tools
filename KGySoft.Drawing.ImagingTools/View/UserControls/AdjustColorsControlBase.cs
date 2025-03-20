#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdjustColorsControlBase.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2025 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Drawing;

using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal partial class AdjustColorsControlBase : TransformBitmapControlBase
    {
        #region Properties

        private new AdjustColorsViewModelBase ViewModel => (AdjustColorsViewModelBase)base.ViewModel!;

        #endregion

        #region Constructors

        #region Protected Constructors

        protected AdjustColorsControlBase(AdjustColorsViewModelBase viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }

        #endregion

        #region Private Constructors

        private AdjustColorsControlBase() : this(null!)
        {
            // this ctor is just for the designer
        }

        #endregion

        #endregion

        #region Methods

        #region Protected Methods

        protected override void InitParentProperties(ParentViewProperties properties)
        {
            base.InitParentProperties(properties);
            properties.MinimumSize = new Size(250, 250);
            properties.Icon = Properties.Resources.Colors;
        }

        protected override void OnLoad(EventArgs e)
        {
            // Fixing high DPI appearance on Mono
            PointF scale;
            if (OSUtils.IsMono && (scale = this.GetScale()) != new PointF(1f, 1f))
            {
                pnlCheckBoxes.Height = (int)(25 * scale.Y);
                btnReset.Width = (int)(64 * scale.X);
                lblValue.Width = (int)(35 * scale.X);
            }

            base.OnLoad(e);
        }

        protected override void ApplyViewModel()
        {
            if (OSUtils.IsMono)
                pnlCheckBoxes.Height = this.ScaleHeight(25);
            InitCommandBindings();
            InitPropertyBindings();
            base.ApplyViewModel();
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
                components?.Dispose();
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void InitCommandBindings()
        {
            // ViewModel commands
            CommandBindings.Add(ViewModel.ResetCommand, ViewModel.ResetCommandState)
                .AddSource(btnReset, nameof(btnReset.Click));
        }

        private void InitPropertyBindings()
        {
            // simple initializations rather than bindings because these will not change:
            trackBar.Minimum = ViewModel.MinValue;
            trackBar.Maximum = ViewModel.MaxValue;
            trackBar.TickFrequency = trackBar.LargeChange = (trackBar.Maximum - trackBar.Minimum) / 20;

            // ViewModel.ColorChannels <-> chbRed.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.ColorChannels), chbRed, nameof(chbRed.Checked),
                channels => ((ColorChannels)channels!).HasFlag<ColorChannels>(ColorChannels.R),
                flag => flag is true ? ViewModel.ColorChannels | ColorChannels.R : ViewModel.ColorChannels & ~ColorChannels.R);

            // ViewModel.ColorChannels <-> chbGreen.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.ColorChannels), chbGreen, nameof(chbGreen.Checked),
                channels => ((ColorChannels)channels!).HasFlag<ColorChannels>(ColorChannels.G),
                flag => flag is true ? ViewModel.ColorChannels | ColorChannels.G : ViewModel.ColorChannels & ~ColorChannels.G);

            // ViewModel.ColorChannels <-> chbBlue.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.ColorChannels), chbBlue, nameof(chbBlue.Checked),
                channels => ((ColorChannels)channels!).HasFlag<ColorChannels>(ColorChannels.B),
                flag => flag is true ? ViewModel.ColorChannels | ColorChannels.B : ViewModel.ColorChannels & ~ColorChannels.B);

            // ViewModel.Value <-> trackBar.Value
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.Value), trackBar, nameof(trackBar.Value));

            // ViewModel.Value -> lblValue.Text
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.Value), nameof(lblValue.Text), v => ((int)v! / 100f).ToString("F2", LanguageSettings.FormattingLanguage), lblValue);
        }

        #endregion

        #endregion
    }
}
