#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ResizeBitmapForm.cs
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

using System;
using System.Globalization;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    internal partial class ResizeBitmapForm : TransformBitmapFormBase
    {
        #region Properties

        private ResizeBitmapViewModel VM => (ResizeBitmapViewModel)ViewModel;

        #endregion

        #region Constructors

        #region Internal Constructors

        internal ResizeBitmapForm(ResizeBitmapViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
            ValidationMapping[nameof(viewModel.Width)] = lblWidthPercent;
            ValidationMapping[nameof(viewModel.Height)] = lblHeightPercent;
        }

        #endregion

        #region Private Constructors

        private ResizeBitmapForm() : this(null!)
        {
            // this ctor is just for the designer
        }

        #endregion

        #endregion

        #region Methods

        #region Static Methods

        private static object FormatPercentage(object value) => value is 0f ? String.Empty : ((float)value * 100f).ToString("F0", CultureInfo.CurrentCulture);
        private static object ParsePercentage(object value) => Single.TryParse((string)value, NumberStyles.Number, CultureInfo.CurrentCulture, out float result) ? result / 100f : 0f;
        private static object FormatInteger(object value) => value is 0 ? String.Empty : ((int)value).ToString("F0", CultureInfo.CurrentCulture);
        private static object ParseInteger(object value) => Int32.TryParse((string)value, NumberStyles.Integer, CultureInfo.CurrentCulture, out int result) ? result : 0;

        #endregion

        #region Instance Methods

        #region Protected Methods

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

        #endregion

        #region Private Methods

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

            // Regular WinForms binding behaves a bit better because it does not clear the currently edited text box on parse error
            // but it fails to sync the other properties properly on Linux/Mono so using KGy SOFT binding on non-Windows systems.

            // VM.WidthRatio <-> txtWidthPercent.Text
            if (OSUtils.IsWindows)
                AddWinFormsBinding(nameof(VM.WidthRatio), txtWidthPercent, nameof(txtWidthPercent.Text), FormatPercentage, ParsePercentage);
            else
                CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(VM.WidthRatio), txtWidthPercent, nameof(txtWidthPercent.Text), FormatPercentage!, ParsePercentage!);

            // VM.HeightRatio <-> txtHeightPercent.Text
            if (OSUtils.IsWindows)
                AddWinFormsBinding(nameof(VM.HeightRatio), txtHeightPercent, nameof(txtHeightPercent.Text), FormatPercentage, ParsePercentage);
            else
                CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(VM.HeightRatio), txtHeightPercent, nameof(txtHeightPercent.Text), FormatPercentage!, ParsePercentage!);

            // VM.Width <-> txtWidthPx.Text
            if (OSUtils.IsWindows)
                AddWinFormsBinding(nameof(VM.Width), txtWidthPx, nameof(txtWidthPx.Text), FormatInteger, ParseInteger);
            else
                CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(VM.Width), txtWidthPx, nameof(txtWidthPx.Text), FormatInteger!, ParseInteger!);

            // VM.Height <-> txtHeightPx.Text
            if (OSUtils.IsWindows)
                AddWinFormsBinding(nameof(VM.Height), txtHeightPx, nameof(txtHeightPx.Text), FormatInteger, ParseInteger);
            else
                CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(VM.Height), txtHeightPx, nameof(txtHeightPx.Text), FormatInteger!, ParseInteger!);
        }

        private void AddWinFormsBinding(string sourceName, IBindableComponent target, string propertyName, Func<object, object> format, Func<object, object> parse)
        {
            var binding = new Binding(propertyName, ViewModel, sourceName, true, DataSourceUpdateMode.OnPropertyChanged);
            binding.Format += (_, e) => e.Value = format.Invoke(e.Value);
            binding.Parse += (_, e) => e.Value = parse.Invoke(e.Value);
            target.DataBindings.Add(binding);
        }

        #endregion

        #endregion

        #endregion
    }
}
