#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdjustColorsFormBase.cs
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

using System.Globalization;

using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    internal partial class AdjustColorsFormBase : TransformBitmapFormBase
    {
        #region Properties

        // this would not be needed if designer had better generics support
        private AdjustColorsViewModelBase VM => (AdjustColorsViewModelBase)ViewModel;

        #endregion

        #region Constructors

        #region Protected Constructors

        protected AdjustColorsFormBase(AdjustColorsViewModelBase viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }

        #endregion

        #region Private Constructors

        private AdjustColorsFormBase() : this(null)
        {
            // this ctor is just for the designer
        }

        #endregion

        #endregion

        #region Methods

        #region Protected Methods

        protected override void ApplyResources()
        {
            Icon = Properties.Resources.Colors;
            base.ApplyResources();
        }

        protected override void ApplyViewModel()
        {
            InitCommandBindings();
            InitPropertyBindings();
            base.ApplyViewModel();
        }

        protected override void Dispose(bool disposing)
        {
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
            trackBar.Minimum = (int)(VM.MinValue * 100f);
            trackBar.Maximum = (int)(VM.MaxValue * 100f);
            trackBar.TickFrequency = trackBar.LargeChange = (trackBar.Maximum - trackBar.Minimum) / 20;

            // VM.ColorChannels <-> chbRed.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(VM.ColorChannels), chbRed, nameof(chbRed.Checked),
                channels => ((ColorChannels)channels).HasFlag<ColorChannels>(ColorChannels.R),
                flag => flag is true ? VM.ColorChannels | ColorChannels.R : VM.ColorChannels & ~ColorChannels.R);

            // VM.ColorChannels <-> chbGreen.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(VM.ColorChannels), chbGreen, nameof(chbGreen.Checked),
                channels => ((ColorChannels)channels).HasFlag<ColorChannels>(ColorChannels.G),
                flag => flag is true ? VM.ColorChannels | ColorChannels.G : VM.ColorChannels & ~ColorChannels.G);

            // VM.ColorChannels <-> chbBlue.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(VM.ColorChannels), chbBlue, nameof(chbBlue.Checked),
                channels => ((ColorChannels)channels).HasFlag<ColorChannels>(ColorChannels.B),
                flag => flag is true ? VM.ColorChannels | ColorChannels.B : VM.ColorChannels & ~ColorChannels.B);

            // VM.Value <-> trackBar.Value
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(VM.Value), trackBar, nameof(trackBar.Value),
                value => (int)((float)value * 100),
                value => (int)value / 100f);

            // VM.Value -> lblValue.Text
            CommandBindings.AddPropertyBinding(ViewModel, nameof(VM.Value), nameof(lblValue.Text), v => ((float)v).ToString("F2", CultureInfo.CurrentCulture), lblValue);
        }

        #endregion

        #endregion
    }
}
