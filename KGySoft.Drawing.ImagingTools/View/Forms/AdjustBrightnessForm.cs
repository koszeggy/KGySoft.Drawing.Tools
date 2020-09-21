using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.ViewModel;

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    internal partial class AdjustBrightnessForm : MvvmBaseForm<AdjustBrightnessViewModel>
    {
        internal AdjustBrightnessForm(AdjustBrightnessViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
            AcceptButton = okCancelButtons.OKButton;
            CancelButton = okCancelButtons.CancelButton;
        }

        private AdjustBrightnessForm() : this(null)
        {
            // this ctor is just for the designer
        }

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

        private void InitCommandBindings()
        {
            // ViewModel commands
            CommandBindings.Add(ViewModel.ApplyCommand, ViewModel.ApplyCommandState)
                .AddSource(okCancelButtons.OKButton, nameof(okCancelButtons.OKButton.Click));
            CommandBindings.Add(ViewModel.CancelCommand)
                .AddSource(okCancelButtons.CancelButton, nameof(okCancelButtons.CancelButton.Click));
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

            // VM.IsGenerating -> progress.ProgressVisible
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.IsGenerating), nameof(progress.ProgressVisible), progress);

            // VM.Progress -> progress.Progress
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.Progress), nameof(progress.Progress), progress);
        }
    }
}
