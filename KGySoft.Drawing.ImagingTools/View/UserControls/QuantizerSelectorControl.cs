﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: QuantizerSelectorControl.cs
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

using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal sealed partial class QuantizerSelectorControl : MvvmBaseUserControl<QuantizerSelectorViewModel>
    {
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
            // not for ViewModel.Parameters.PropertyChanged because it is not triggered for expanded properties such as collection elements
            CommandBindings.Add(ViewModel!.ResetQuantizer)
                .AddSource(pgParameters, nameof(pgParameters.PropertyValueChanged));
        }

        private void InitPropertyBindings()
        {
            // will not change so not as an actual binding
            cmbQuantizer.DataSource = ViewModel!.Quantizers;

            // VM.Parameters -> pgParameters.SelectedObject
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.Parameters), nameof(pgParameters.SelectedObject), pgParameters);

            // cmbQuantizer.SelectedValue -> VM.SelectedQuantizer
            CommandBindings.AddPropertyBinding(cmbQuantizer, nameof(cmbQuantizer.SelectedValue), nameof(ViewModel.SelectedQuantizer), ViewModel);
        }

        #endregion

        #endregion
    }
}
