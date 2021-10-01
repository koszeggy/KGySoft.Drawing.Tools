#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DithererSelectorControl.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal sealed partial class DithererSelectorControl : MvvmBaseUserControl<DithererSelectorViewModel>
    {
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
            // not for ViewModel.Parameters.PropertyChanged because it is not triggered for expanded properties such as collection elements
            CommandBindings.Add(ViewModel!.ResetDitherer)
                .AddSource(pgParameters, nameof(pgParameters.PropertyValueChanged));
        }

        private void InitPropertyBindings()
        {
            // will not change so not as an actual binding
            cmbDitherer.DataSource = ViewModel!.Ditherers;

            // VM.Parameters -> pgParameters.SelectedObject
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.Parameters), nameof(pgParameters.SelectedObject), pgParameters);

            // cmbDitherer.SelectedValue -> VM.SelectedDitherer
            CommandBindings.AddPropertyBinding(cmbDitherer, nameof(cmbDitherer.SelectedValue), nameof(ViewModel.SelectedDitherer), ViewModel);
        }

        #endregion

        #endregion
    }
}
