#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CountColorsControl.cs
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

using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal partial class CountColorsControl : MvvmBaseUserControl
    {
        #region Fields

        private ParentViewProperties? parentProperties;
        
        #endregion

        #region Properties

        #region Internal Properties

        internal override ParentViewProperties ParentViewProperties => parentProperties ??= new ParentViewProperties
        {
            Name = "CountColorsForm",
            BorderStyle = FormBorderStyle.FixedDialog,
            Icon = Properties.Resources.Palette,
            AcceptButton = btnClose,
            CancelButton = btnClose,
            ClosingCallback = (_,_) => ViewModel.CancelIfRunning()
        };

        #endregion

        #region Private Properties

        private new CountColorsViewModel ViewModel => (CountColorsViewModel)base.ViewModel!;

        #endregion

        #endregion

        #region Constructors

        #region Internal Constructors

        internal CountColorsControl(CountColorsViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }

        #endregion

        #region Private Constructors

        private CountColorsControl() : this(null!)
        {
            // this ctor is just for the designer
        }

        #endregion

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
            if (IsDisposed)
                return;

            // Cancelling here may be required if this control is not parented by an MvvmParentForm
            ViewModel.CancelIfRunning();
            parentProperties = null;
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
