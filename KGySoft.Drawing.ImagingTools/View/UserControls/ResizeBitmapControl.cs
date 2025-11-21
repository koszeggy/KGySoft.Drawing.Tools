#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ResizeBitmapControl.cs
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

using System;
using System.Drawing;
using System.Globalization;

using KGySoft.ComponentModel;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal partial class ResizeBitmapControl : TransformBitmapControlBase
    {
        #region Properties

        private new ResizeBitmapViewModel ViewModel => (ResizeBitmapViewModel)base.ViewModel!;

        #endregion

        #region Constructors

        #region Internal Constructors

        internal ResizeBitmapControl(ResizeBitmapViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }

        #endregion

        #region Private Constructors

        private ResizeBitmapControl() : this(null!)
        {
            // this ctor is just for the designer
        }

        #endregion

        #endregion

        #region Methods

        #region Protected Methods

        protected override void InitParentProperties(ParentViewProperties properties)
        {
            base.InitParentProperties(properties);
            properties.MinimumSize = new Size(350, 330);
            properties.Icon = Properties.Resources.Resize;
        }

        protected override void OnLoad(EventArgs e)
        {
            // Fixing high DPI appearance on Mono
            PointF scale;
            if (OSUtils.IsMono && (scale = this.GetScale()) != new PointF(1f, 1f))
                tblNewSize.ColumnStyles[0].Width = (int)(100 * scale.X);

            if (!IsLoaded)
            {
                ValidationMapping[nameof(ViewModel.Width)] = lblWidthPercent;
                ValidationMapping[nameof(ViewModel.Height)] = lblHeightPercent;
            }

            base.OnLoad(e);
        }

        protected override void ApplyViewModel()
        {
            InitPropertyBindings();
            base.ApplyViewModel();
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
                components?.Dispose();
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void InitPropertyBindings()
        {
            #region Local Methods

            static object FormatPercentage(object value) => ((float)value * 100f).ToString("F0", LanguageSettings.FormattingLanguage);
            static object ParsePercentage(object value) => Single.Parse((string)value, NumberStyles.Number, LanguageSettings.FormattingLanguage) / 100f;
            static object FormatInteger(object value) => ((int)value).ToString("F0", LanguageSettings.FormattingLanguage);
            static object ParseInteger(object value) => Int32.Parse((string)value, NumberStyles.Integer, LanguageSettings.FormattingLanguage);

            void AddTwoWayValidatedBinding(object source, string sourcePropertyName, object target, string targetPropertyName,
                Func<object?, object?> format, Func<object?, object?> parse)
            {
                // Creating the regular two-way binding and adding error handling to the View -> VM (back) direction, which is the 2nd item.
                ICommandBinding[] bindings = CommandBindings.AddTwoWayPropertyBinding(source, sourcePropertyName, target, targetPropertyName, format, parse);
                bindings[1].Executing += (_, _) => ViewModel.SetBindingError(sourcePropertyName, null);
                bindings[1].Error += (_, e) =>
                {
                    ViewModel.SetBindingError(sourcePropertyName, PublicResources.ArgumentInvalidString);
                    e.Handled = true;
                };
            }

            #endregion

            // Simple initialization rather than bindings because this will not change:
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

            // The validations can be achieved also by WinForms bindings by handling the BindingComplete event,
            // but on Mono it fails to sync the other properties properly whereas KGy SOFT binding works on every target.

            // ViewModel.WidthRatio <-> txtWidthPercent.Text
            AddTwoWayValidatedBinding(ViewModel, nameof(ViewModel.WidthRatio), txtWidthPercent, nameof(txtWidthPercent.Text), FormatPercentage!, ParsePercentage!);

            // ViewModel.HeightRatio <-> txtHeightPercent.Text
            AddTwoWayValidatedBinding(ViewModel, nameof(ViewModel.HeightRatio), txtHeightPercent, nameof(txtHeightPercent.Text), FormatPercentage!, ParsePercentage!);

            // ViewModel.Width <-> txtWidthPx.Text
            AddTwoWayValidatedBinding(ViewModel, nameof(ViewModel.Width), txtWidthPx, nameof(txtWidthPx.Text), FormatInteger!, ParseInteger!);

            // ViewModel.Height <-> txtHeightPx.Text
            AddTwoWayValidatedBinding(ViewModel, nameof(ViewModel.Height), txtHeightPx, nameof(txtHeightPx.Text), FormatInteger!, ParseInteger!);
        }

        #endregion

        #endregion
    }
}
