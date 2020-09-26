using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KGySoft.Drawing.ImagingTools.ViewModel;

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    internal partial class ResizeBitmapForm : TransformBitmapFormBase
    {
        #region Properties

        // this would not be needed if designer had better generics support
        private ResizeBitmapViewModel VM => (ResizeBitmapViewModel)ViewModel;

        #endregion

        internal ResizeBitmapForm(ResizeBitmapViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();

            ValidationMapping[nameof(viewModel.Width)] = lblWidth;
            ValidationMapping[nameof(viewModel.Height)] = lblHeight;
            foreach (Control control in ValidationMapping.Values.Where(c => c is Label))
                ErrorProvider.SetIconAlignment(control, ErrorIconAlignment.MiddleLeft);
        }

        private ResizeBitmapForm() : this(null)
        {
            // this ctor is just for the designer
        }

        protected override void ApplyResources()
        {
            Icon = Properties.Resources.Resize;
            base.ApplyResources();
        }

        protected override void ApplyViewModel()
        {
            InitPropertyBindings();
            base.ApplyViewModel();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();
            base.Dispose(disposing);
        }

        private void InitPropertyBindings()
        {
            // simple initializations rather than bindings because these will not change:
            cmbScalingMode.DataSource = VM.ScalingModes;

            // VM.ScalingMode -> cmbScalingMode.SelectedItem (cannot use two-way for SelectedItem because there is no SelectedItemChanged event)
            CommandBindings.AddPropertyBinding(ViewModel, nameof(VM.ScalingMode), nameof(cmbScalingMode.SelectedItem), cmbScalingMode);

            // cmbScalingMode.SelectedValue -> VM.ScalingMode (cannot use two-way for SelectedValue because ValueMember is not set)
            CommandBindings.AddPropertyBinding(cmbScalingMode, nameof(cmbScalingMode.SelectedValue), nameof(VM.ScalingMode), ViewModel);

            // VM.KeepAspectRatio <-> chbMaintainAspectRatio.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(VM.KeepAspectRatio), chbMaintainAspectRatio, nameof(chbMaintainAspectRatio.Checked));

            //  rbByPercentage.Checked <-> VM.ByPercentage -> txtWidthPercent.Enabled, txtHeightPercent.Enabled
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(VM.ByPercentage), rbByPercentage, nameof(rbByPercentage.Checked));
            CommandBindings.AddPropertyBinding(ViewModel, nameof(VM.ByPercentage), nameof(Enabled), txtWidthPercent, txtHeightPercent);

            // rbByPixels.Checked <-> VM.ByPixels -> txtWidthPx.Enabled, txtHeightPx.Enabled
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(VM.ByPixels), rbByPixels, nameof(rbByPixels.Checked));
            CommandBindings.AddPropertyBinding(ViewModel, nameof(VM.ByPixels), nameof(Enabled), txtWidthPx, txtHeightPx);

            // VM.WidthRatio <-> txtWidthPercent.Text
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(VM.WidthRatio), txtWidthPercent, nameof(txtWidthPercent.Text),
                FormatPercentage, ParsePercentage);

            // VM.HeightRatio <-> txtHeightPercent.Text
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(VM.HeightRatio), txtHeightPercent, nameof(txtHeightPercent.Text),
                FormatPercentage, ParsePercentage);

            // VM.Width <-> txtWidthPx.Text
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(VM.Width), txtWidthPx, nameof(txtWidthPx.Text),
                FormatInteger, ParseInteger);

            // VM.Height <-> txtHeightPx.Text
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(VM.Height), txtHeightPx, nameof(txtHeightPx.Text),
                FormatInteger, ParseInteger);
        }

        private object FormatPercentage(object value) => (((float)value) * 100f).ToString("N2", CultureInfo.CurrentCulture);
        private object ParsePercentage(object value) => Single.TryParse((string)value, NumberStyles.Number, CultureInfo.CurrentCulture, out float result) ? result / 100f : 0f;

        private object FormatInteger(object value) => ((int)value).ToString("N0", CultureInfo.CurrentCulture);
        private object ParseInteger(object value) => Int32.TryParse((string)value, NumberStyles.Number, CultureInfo.CurrentCulture, out int result) ? result : 0;
    }
}
