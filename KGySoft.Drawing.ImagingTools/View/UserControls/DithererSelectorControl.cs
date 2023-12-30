﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DithererSelectorControl.cs
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
    internal sealed partial class DithererSelectorControl : MvvmBaseUserControl
    {
        #region Properties

        internal new DithererSelectorViewModel? ViewModel
        {
            get => (DithererSelectorViewModel?)base.ViewModel;
            set => base.ViewModel = value;
        }

        #endregion

        #region Constructors

        public DithererSelectorControl()
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
            CommandBindings.AddPropertyChangedHandlerBinding(ViewModel!, ResetParentSize, nameof(ViewModel.SelectedDitherer));
        }

        private void InitPropertyBindings()
        {
            // will not change so not as an actual binding
            cmbDitherer.DataSource = ViewModel!.Ditherers;

            // Strength
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.StrengthVisible), nameof(tblStrength.Visible), tblStrength);

            // Serpentine processing
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.SerpentineProcessingVisible), nameof(chbSerpentineProcessing.Visible), chbSerpentineProcessing);

            // By Brightness
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.ByBrightnessVisible), nameof(chbByBrightness.Visible), chbByBrightness);

            // Seed
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.SeedVisible), nameof(tblSeed.Visible), tblSeed);

            // cmbDitherer.SelectedValue -> VM.SelectedDitherer (intentionally last so visibilities are already bound)
            CommandBindings.AddPropertyBinding(cmbDitherer, nameof(cmbDitherer.SelectedValue), nameof(ViewModel.SelectedDitherer), ViewModel);
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
