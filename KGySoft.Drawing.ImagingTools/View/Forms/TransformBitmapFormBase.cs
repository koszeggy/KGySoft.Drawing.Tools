#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: TransformBitmapFormBase.cs
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
using System.Linq;
using System.Windows.Forms;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    internal partial class TransformBitmapFormBase : MvvmBaseForm<TransformBitmapViewModelBase>
    {
        #region Properties

        protected Dictionary<string, Control> ValidationMapping { get; }
        protected ErrorProvider ErrorProvider => errorProvider;
        protected ErrorProvider WarningProvider => warningProvider;
        protected ErrorProvider InfoProvider => infoProvider;

        #endregion

        #region Constructors

        #region Protected Constructors

        protected TransformBitmapFormBase(TransformBitmapViewModelBase viewModel)
            : base(viewModel)
        {
            InitializeComponent();
            AcceptButton = okCancelButtons.OKButton;
            CancelButton = okCancelButtons.CancelButton;

            errorProvider.SetIconAlignment(previewImage.ImageViewer, ErrorIconAlignment.MiddleLeft);
            ValidationMapping = new Dictionary<string, Control>
            {
                [nameof(viewModel.PreviewImageViewModel.PreviewImage)] = previewImage.ImageViewer,
            };
        }

        #endregion

        #region Private Constructors

        private TransformBitmapFormBase() : this(null)
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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Alt | Keys.Z:
                    previewImage.AutoZoom = !previewImage.AutoZoom;
                    return true;
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
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
            previewImage.ViewModel = ViewModel.PreviewImageViewModel;

            // VM.IsGenerating -> progress.ProgressVisible
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.IsGenerating), nameof(progress.ProgressVisible), progress);

            // VM.Progress -> progress.Progress
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.Progress), nameof(progress.Progress), progress);
        }

        #endregion

        #region Command handlers

        private void OnValidationResultsChangedCommand(ICommandSource<EventArgs<ValidationResultsCollection>> src)
        {
            foreach (KeyValuePair<string, Control> mapping in ValidationMapping)
            {
                IReadOnlyList<ValidationResult> validationResults = src.EventArgs.EventData[mapping.Key];
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
