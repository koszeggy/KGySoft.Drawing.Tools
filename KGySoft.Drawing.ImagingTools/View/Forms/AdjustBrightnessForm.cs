#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdjustBrightnessForm.cs
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
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    internal partial class AdjustBrightnessForm : MvvmBaseForm<AdjustBrightnessViewModel>
    {
        #region Fields

        private readonly Dictionary<string, Control> validationMapping;

        #endregion

        #region Constructors

        #region Internal Constructors

        internal AdjustBrightnessForm(AdjustBrightnessViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
            AcceptButton = okCancelButtons.OKButton;
            CancelButton = okCancelButtons.CancelButton;

            errorProvider.SetIconAlignment(previewImage.ImageViewer, ErrorIconAlignment.MiddleLeft);
            validationMapping = new Dictionary<string, Control>
            {
                [nameof(viewModel.PreviewImageViewModel.Image)] = previewImage.ImageViewer,
            };
        }

        #endregion

        #region Private Constructors

        private AdjustBrightnessForm() : this(null)
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
            CommandBindings.Add(ViewModel.ResetCommand, ViewModel.ResetCommandState)
                .AddSource(btnReset, nameof(btnReset.Click));

            // View commands
            CommandBindings.Add<EventArgs<ValidationResultsCollection>>(OnValidationResultsChangedCommand)
                .AddSource(ViewModel, nameof(ViewModel.ValidationResultsChanged));
        }

        private void InitPropertyBindings()
        {
            // simple initializations rather than bindings because these will not change:
            previewImage.ViewModel = ViewModel.PreviewImageViewModel;

            // VM.ColorChannels <-> chbRed.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.ColorChannels), chbRed, nameof(chbRed.Checked),
                channels => ((ColorChannels)channels).HasFlag<ColorChannels>(ColorChannels.R),
                flag => flag is true ? ViewModel.ColorChannels | ColorChannels.R : ViewModel.ColorChannels & ~ColorChannels.R);

            // VM.ColorChannels <-> chbGreen.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.ColorChannels), chbGreen, nameof(chbGreen.Checked),
                channels => ((ColorChannels)channels).HasFlag<ColorChannels>(ColorChannels.G),
                flag => flag is true ? ViewModel.ColorChannels | ColorChannels.G : ViewModel.ColorChannels & ~ColorChannels.G);

            // VM.ColorChannels <-> chbBlue.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.ColorChannels), chbBlue, nameof(chbBlue.Checked),
                channels => ((ColorChannels)channels).HasFlag<ColorChannels>(ColorChannels.B),
                flag => flag is true ? ViewModel.ColorChannels | ColorChannels.B : ViewModel.ColorChannels & ~ColorChannels.B);

            // VM.Value <-> trackBar.Value
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.Value), trackBar, nameof(trackBar.Value),
                value => (int)((float)value * 100),
                value => (int)value / 100f);

            // VM.Value -> lblValue.Text
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.Value), nameof(lblValue.Text), v => ((float)v).ToString("F2", CultureInfo.CurrentCulture), lblValue);

            // VM.IsGenerating -> progress.ProgressVisible
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.IsGenerating), nameof(progress.ProgressVisible), progress);

            // VM.Progress -> progress.Progress
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.Progress), nameof(progress.Progress), progress);
        }

        #endregion

        #region Command handlers

        private void OnValidationResultsChangedCommand(ICommandSource<EventArgs<ValidationResultsCollection>> src)
        {
            foreach (KeyValuePair<string, Control> mapping in validationMapping)
            {
                var validationResults = src.EventArgs.EventData[mapping.Key];
                ValidationResult error = validationResults.FirstOrDefault(vr => vr.Severity == ValidationSeverity.Error);
                errorProvider.SetError(mapping.Value, error?.Message);
            }
        }

        #endregion

        #endregion
    }
}
