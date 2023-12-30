#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: QuantizerSelectorControl.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2023 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal sealed partial class QuantizerSelectorControl : MvvmBaseUserControl
    {
        #region Properties

        internal new QuantizerSelectorViewModel? ViewModel
        {
            get => (QuantizerSelectorViewModel?)base.ViewModel;
            set => base.ViewModel = value;
        }

        #endregion

        #region Constructors

        public QuantizerSelectorControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        #region Protected Methods

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
            CommandBindings.AddPropertyChangedHandlerBinding(ViewModel!, ResetParentSize, nameof(ViewModel.SelectedQuantizer));
        }

        private void InitPropertyBindings()
        {
            // will not change so not as an actual binding
            cmbQuantizer.DataSource = ViewModel!.Quantizers;

            // BackColor
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.BackColorEnabled), nameof(tblBackColor.Enabled), tblBackColor);

            // AlphaThreshold
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.AlphaThresholdVisible), nameof(tblAlphaThreshold.Visible), tblAlphaThreshold);
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.AlphaThresholdEnabled), nameof(tblAlphaThreshold.Enabled), tblAlphaThreshold);

            // WhiteThreshold
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.WhiteThresholdVisible), nameof(tblWhiteThreshold.Visible), tblWhiteThreshold);

            // Number of colors
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.NumColorsVisible), nameof(tblNumColors.Visible), tblNumColors);

            // DirectMapping
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.DirectMappingVisible), nameof(chbDirectMapping.Visible), chbDirectMapping);

            // BitLevel
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.BitLevelVisible), nameof(tblBitLevel.Visible), tblBitLevel);

            // cmbQuantizer.SelectedValue -> VM.SelectedQuantizer (intentionally last so visibilities are already bound)
            CommandBindings.AddPropertyBinding(cmbQuantizer, nameof(cmbQuantizer.SelectedValue), nameof(ViewModel.SelectedQuantizer), ViewModel);
        }

        private void ResetParentSize()
        {
            Control? parent = Parent?.Parent;
            if (parent == null)
                return;

            int height = 0;
            foreach (Control control in Controls)
            {
                if (!control.Visible)
                    continue;

                height += control.Height;
            }

            parent.Height = height + (parent.Height - parent.DisplayRectangle.Height);
        }

        #endregion

        #endregion
    }
}
