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

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    internal sealed partial class ColorSpaceForm : MvvmBaseForm<ColorSpaceViewModel>
    {
        #region Fields

        private readonly Dictionary<string, Control> validationMapping;

        #endregion

        #region Constructors

        #region Internal Constructors

        internal ColorSpaceForm(ColorSpaceViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
            AcceptButton = okCancelButtons.OKButton;
            CancelButton = okCancelButtons.CancelButton;
            errorProvider.SetIconAlignment(previewImage.ImageViewer, ErrorIconAlignment.MiddleLeft);
            validationMapping = new Dictionary<string, Control>
            {
                [nameof(viewModel.PixelFormat)] = gbPixelFormat.CheckBox,
                [nameof(viewModel.QuantizerSelectorViewModel.Quantizer)] = gbQuantizer.CheckBox,
                [nameof(viewModel.DithererSelectorViewModel.Ditherer)] = gbDitherer.CheckBox,
                [nameof(viewModel.PreviewImageViewModel.Image)] = previewImage.ImageViewer,
            };

            foreach (Control control in validationMapping.Values.Where(c => c is CheckBox))
            {
                errorProvider.SetIconAlignment(control, ErrorIconAlignment.TopRight);
                warningProvider.SetIconAlignment(control, ErrorIconAlignment.TopRight);
                infoProvider.SetIconAlignment(control, ErrorIconAlignment.TopRight);
            }
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
            errorProvider.Icon = Icons.SystemError.ToScaledIcon();
            warningProvider.Icon = Icons.SystemWarning.ToScaledIcon();
            infoProvider.Icon = Icons.SystemInformation.ToScaledIcon();
        }

        protected override void ApplyViewModel()
        {
            InitCommandBindings();
            InitPropertyBindings();
            base.ApplyViewModel();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // if user (or system) closes the window without pressing cancel we need to execute the cancel command
            if (DialogResult != DialogResult.OK && e.CloseReason != CloseReason.None)
                okCancelButtons.CancelButton.PerformClick();
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
            // ViewModel commands
            CommandBindings.Add(ViewModel.ApplyCommand, ViewModel.ApplyCommandState)
                .AddSource(okCancelButtons.OKButton, nameof(okCancelButtons.OKButton.Click));
            CommandBindings.Add(ViewModel.CancelCommand)
                .AddSource(okCancelButtons.CancelButton, nameof(okCancelButtons.CancelButton.Click));
            
            // View commands
            CommandBindings.Add<EventArgs<ValidationResultsCollection>>(OnValidationResultsChangedCommand)
                .AddSource(ViewModel, nameof(ViewModel.ValidationResultsChanged));
        }

        private void InitPropertyBindings()
        {
            // simple initializations rather than bindings because these will not change:
            cmbPixelFormat.DataSource = ViewModel.PixelFormats;
            quantizerSelector.ViewModel = ViewModel.QuantizerSelectorViewModel;
            dithererSelector.ViewModel = ViewModel.DithererSelectorViewModel;
            previewImage.ViewModel = ViewModel.PreviewImageViewModel;

            // VM.ChangePixelFormat <-> gbPixelFormat.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.ChangePixelFormat), gbPixelFormat, nameof(gbPixelFormat.Checked));

            // VM.SelectedPixelFormat -> cmbPixelFormat.SelectedItem (cannot use two-way for SelectedItem because there is no SelectedItemChanged event)
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.PixelFormat), nameof(cmbPixelFormat.SelectedItem), cmbPixelFormat);

            // cmbPixelFormat.SelectedItem -> VM.SelectedPixelFormat (cannot use two-way for SelectedValue because ValueMember is not set)
            CommandBindings.AddPropertyBinding(cmbPixelFormat, nameof(cmbPixelFormat.SelectedValue), nameof(ViewModel.PixelFormat), ViewModel);

            // VM.UseQuantizer <-> gbQuantizer.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.UseQuantizer), gbQuantizer, nameof(gbQuantizer.Checked));

            // VM.UseDitherer <-> gbDitherer.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.UseDitherer), gbDitherer, nameof(gbDitherer.Checked));
        }

        #endregion

        #region Command handlers

        private void OnValidationResultsChangedCommand(ICommandSource<EventArgs<ValidationResultsCollection>> src)
        {
            foreach (KeyValuePair<string, Control> mapping in validationMapping)
            {
                var validationResults = src.EventArgs.EventData[mapping.Key];
                ValidationResult error = validationResults.FirstOrDefault(vr => vr.Severity == ValidationSeverity.Error);
                ValidationResult warning = error == null ? validationResults.FirstOrDefault(vr => vr.Severity == ValidationSeverity.Warning) : null;
                ValidationResult info = error == null && warning == null ? validationResults.FirstOrDefault(vr => vr.Severity == ValidationSeverity.Information) : null;
                errorProvider.SetError(mapping.Value, error?.Message);
                warningProvider.SetError(mapping.Value, warning?.Message);
                infoProvider.SetError(mapping.Value, info?.Message);
            }
        }

        #endregion

        #endregion
    }
}
