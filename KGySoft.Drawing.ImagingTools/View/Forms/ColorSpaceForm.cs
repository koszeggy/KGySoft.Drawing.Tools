#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorSpaceForm.cs
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

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    internal partial class ColorSpaceForm : MvvmBaseForm<ColorSpaceViewModel>
    {
        #region Constructors

        #region Internal Constructors

        internal ColorSpaceForm(ColorSpaceViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
            AcceptButton = okCancelButtons.OKButton;
            CancelButton = okCancelButtons.CancelButton;
        }

        #endregion

        #region Private Constructors

        private ColorSpaceForm() : this(null)
        {
            // this ctor is just for the designer
        }

        #endregion

        #endregion

        #region Methods

        #region Protected Methods

        protected override void ApplyResources()
        {
            base.ApplyResources();
            Icon = Properties.Resources.Palette;
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
            CommandBindings.Add(ViewModel.ApplyCommand, ViewModel.ApplyCommandState)
                .AddSource(okCancelButtons.OKButton, nameof(okCancelButtons.OKButton.Click));
            CommandBindings.Add(ViewModel.CancelCommand)
                .AddSource(okCancelButtons.CancelButton, nameof(okCancelButtons.CancelButton.Click));
        }

        private void InitPropertyBindings()
        {
            // simple initializations rather than bindings because these will not change:
            cmbPixelFormat.DataSource = ViewModel.PixelFormats;
            quantizerSelector.ViewModel = ViewModel.QuantizerSelectorViewModel;
            dithererSelector.ViewModel = ViewModel.DithererSelectorViewModel;
            previewImage.ViewModel = ViewModel.PreviewImageViewModel;

            // VM.SelectedPixelFormat -> cmbPixelFormat.SelectedItem (cannot use two-way for SelectedItem because there is no SelectedItemChanged event)
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.SelectedPixelFormat), nameof(cmbPixelFormat.SelectedItem), cmbPixelFormat);

            // cmbPixelFormat.SelectedItem -> VM.SelectedPixelFormat (cannot use two-way for SelectedValue because ValueMember is not set)
            CommandBindings.AddPropertyBinding(cmbPixelFormat, nameof(cmbPixelFormat.SelectedValue), nameof(ViewModel.SelectedPixelFormat), ViewModel);

            // VM.UseQuantizer <-> gbQuantizer.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.UseQuantizer), gbQuantizer, nameof(gbQuantizer.Checked));

            // VM.UseDitherer <-> gbDitherer.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.UseDitherer), gbDitherer, nameof(gbDitherer.Checked));
        }

        #endregion

        #endregion
    }
}
