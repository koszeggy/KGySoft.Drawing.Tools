#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CountColorsForm.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2024 - All Rights Reserved
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

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    internal partial class CountColorsForm : MvvmBaseForm
    {
        #region Properties

        private new CountColorsViewModel ViewModel => (CountColorsViewModel)base.ViewModel;

        #endregion

        #region Constructors

        #region Internal Constructors

        internal CountColorsForm(CountColorsViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
            AcceptButton = CancelButton = btnClose;
        }

        #endregion

        #region Private Constructors

        private CountColorsForm() : this(null!)
        {
            // this ctor is just for the designer
        }

        #endregion

        #endregion

        #region Methods

        #region Protected Methods

        protected override void ApplyResources()
        {
            Icon = Properties.Resources.Palette;
            base.ApplyResources();
        }

        protected override void ApplyViewModel()
        {
            InitCommandBindings();
            InitPropertyBindings();
            base.ApplyViewModel();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            ViewModel.CancelIfRunning();
            base.OnFormClosing(e);
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
            CommandBindings.Add(ViewModel.CancelCommand)
                .AddSource(btnClose, nameof(btnClose.Click));
        }

        private void InitPropertyBindings()
        {
            // VM.DisplayText <-> lblCountColorsStatus.Text
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.DisplayText), nameof(lblCountColorsStatus.Text), lblCountColorsStatus);

            // VM.IsProcessing -> progress.ProgressVisible
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.IsProcessing), nameof(progress.ProgressVisible), progress);

            // VM.Progress -> progress.Progress
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.Progress), nameof(progress.Progress), progress);
        }

        #endregion

        #endregion
    }
}
