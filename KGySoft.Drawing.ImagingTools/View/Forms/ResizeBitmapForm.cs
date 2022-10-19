﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ResizeBitmapForm.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    internal partial class ResizeBitmapForm : TransformBitmapFormBase
    {
        #region Properties

        private new ResizeBitmapViewModel ViewModel => (ResizeBitmapViewModel)base.ViewModel;

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

        private static object FormatPercentage(object value) => value is 0f ? String.Empty : ((float)value * 100f).ToString("F0", LanguageSettings.FormattingLanguage);
        private static object ParsePercentage(object value) => Single.TryParse((string)value, NumberStyles.Number, LanguageSettings.FormattingLanguage, out float result) ? result / 100f : 0f;
        private static object FormatInteger(object value) => value is 0 ? String.Empty : ((int)value).ToString("F0", LanguageSettings.FormattingLanguage);
        private static object ParseInteger(object value) => Int32.TryParse((string)value, NumberStyles.Integer, LanguageSettings.FormattingLanguage, out int result) ? result : 0;

        #endregion

        #region Instance Methods

        #region Protected Methods

        protected override void OnLoad(EventArgs e)
        {
            // Fixing high DPI appearance on Mono
            PointF scale;
            if (OSUtils.IsMono && (scale = this.GetScale()) != new PointF(1f, 1f))
                tblNewSize.ColumnStyles[0].Width = (int)(100 * scale.X);

            base.OnLoad(e);
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

        #endregion

        #region Private Methods

        private void InitPropertyBindings()
        {
            // simple initializations rather than bindings because these will not change:
            cmbScalingMode.DataSource = ViewModel.ScalingModes;

            // ViewModel.ScalingMode -> cmbScalingMode.SelectedItem (cannot use two-way for SelectedItem because there is no SelectedItemChanged event)
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.ScalingMode), nameof(cmbScalingMode.SelectedItem), cmbScalingMode);

            // cmbScalingMode.SelectedValue -> ViewModel.ScalingMode (cannot use two-way for SelectedValue because ValueMember is not set)
            CommandBindings.AddPropertyBinding(cmbScalingMode, nameof(cmbScalingMode.SelectedValue), nameof(ViewModel.ScalingMode), ViewModel);

            // ViewModel.KeepAspectRatio <-> chbMaintainAspectRatio.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.KeepAspectRatio), chbMaintainAspectRatio, nameof(chbMaintainAspectRatio.Checked));

            //  rbByPercentage.Checked <-> ViewModel.ByPercentage -> txtWidthPercent.Enabled, txtHeightPercent.Enabled
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.ByPercentage), rbByPercentage, nameof(rbByPercentage.Checked));
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.ByPercentage), nameof(Enabled), txtWidthPercent, txtHeightPercent);

            // rbByPixels.Checked <-> ViewModel.ByPixels -> txtWidthPx.Enabled, txtHeightPx.Enabled
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.ByPixels), rbByPixels, nameof(rbByPixels.Checked));
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.ByPixels), nameof(Enabled), txtWidthPx, txtHeightPx);

            // Regular WinForms binding behaves a bit better because it does not clear the currently edited text box on parse error
            // but it fails to sync the other properties properly on Mono so using KGy SOFT binding in Mono systems.

            // ViewModel.WidthRatio <-> txtWidthPercent.Text
            if (OSUtils.IsMono)
                CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.WidthRatio), txtWidthPercent, nameof(txtWidthPercent.Text), FormatPercentage!, ParsePercentage!);
            else
                AddWinFormsBinding(nameof(ViewModel.WidthRatio), txtWidthPercent, nameof(txtWidthPercent.Text), FormatPercentage, ParsePercentage);

            // ViewModel.HeightRatio <-> txtHeightPercent.Text
            if (OSUtils.IsMono)
                CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.HeightRatio), txtHeightPercent, nameof(txtHeightPercent.Text), FormatPercentage!, ParsePercentage!);
            else
                AddWinFormsBinding(nameof(ViewModel.HeightRatio), txtHeightPercent, nameof(txtHeightPercent.Text), FormatPercentage, ParsePercentage);

            // ViewModel.Width <-> txtWidthPx.Text
            if (OSUtils.IsMono)
                CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.Width), txtWidthPx, nameof(txtWidthPx.Text), FormatInteger!, ParseInteger!);
            else
                AddWinFormsBinding(nameof(ViewModel.Width), txtWidthPx, nameof(txtWidthPx.Text), FormatInteger, ParseInteger);

            // ViewModel.Height <-> txtHeightPx.Text
            if (OSUtils.IsMono)
                CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.Height), txtHeightPx, nameof(txtHeightPx.Text), FormatInteger!, ParseInteger!);
            else
                AddWinFormsBinding(nameof(ViewModel.Height), txtHeightPx, nameof(txtHeightPx.Text), FormatInteger, ParseInteger);
        }

        private void AddWinFormsBinding(string sourceName, IBindableComponent target, string propertyName, Func<object, object> format, Func<object, object> parse)
        {
            var binding = new Binding(propertyName, ViewModel, sourceName, true, DataSourceUpdateMode.OnPropertyChanged);
            binding.Format += (_, e) => e.Value = format.Invoke(e.Value!);
            binding.Parse += (_, e) => e.Value = parse.Invoke(e.Value!);
            target.DataBindings.Add(binding);
        }

        #endregion

        #endregion

        #endregion
    }
}
