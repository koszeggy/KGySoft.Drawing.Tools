#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: QuantizerSelectorControl.cs
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

using System.Drawing;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal sealed partial class QuantizerSelectorControl : MvvmBaseUserControl
    {
        #region Properties

        internal new QuantizerSelectorViewModel? ViewModel
        {
            get => (QuantizerSelectorViewModel?)base.ViewModel;
            set => base.ViewModel = value;
        }

        #endregion

        #region Constructors

        public QuantizerSelectorControl()
        {
            InitializeComponent();
        }

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
            if (disposing)
                components?.Dispose();

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void InitCommandBindings()
        {
            CommandBindings.Add(OnSelectBackColorCommand)
                .AddSource(btnBackColor, nameof(btnBackColor.Click))
                .AddSource(pnlBackColor, nameof(btnBackColor.DoubleClick));

            CommandBindings.AddPropertyChangedHandlerBinding(ViewModel!, OnResetParentSizeCommand, nameof(ViewModel.SelectedQuantizer));
        }

        private void InitPropertyBindings()
        {
            #region Local Methods

            static object FormatInt(object? value) => ((int)value!).ToString(LanguageSettings.FormattingLanguage);
            
            static object FormatBitLevel(object? value)
            {
                int v = (int)value!;
                return v switch
                {
                    0 => Res.TextAuto,
                    _ => v.ToString(LanguageSettings.FormattingLanguage)
                };
            }

            #endregion

            // will not change so not as an actual binding
            cmbQuantizer.DataSource = ViewModel!.Quantizers;

            // Color Space
            CommandBindings.AddPropertyBinding(chbLinearColorSpace, nameof(chbLinearColorSpace.Checked), nameof(ViewModel.UseLinearColorSpace), ViewModel);

            // BackColor
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.BackColorEnabled), nameof(tblBackColor.Enabled), tblBackColor);
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.BackColor), nameof(pnlBackColor.BackColor), pnlBackColor);

            // AlphaThreshold
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.AlphaThresholdVisible), nameof(tblAlphaThreshold.Visible), tblAlphaThreshold);
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.AlphaThresholdEnabled), nameof(tblAlphaThreshold.Enabled), tblAlphaThreshold);
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.AlphaThreshold), tbAlphaThreshold, nameof(tbAlphaThreshold.Value));
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.AlphaThreshold), nameof(lblAlphaThresholdValue.Text), FormatInt, lblAlphaThresholdValue);

            // WhiteThreshold
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.WhiteThresholdVisible), nameof(tblWhiteThreshold.Visible), tblWhiteThreshold);
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.WhiteThreshold), tbWhiteThreshold, nameof(tbWhiteThreshold.Value));
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.WhiteThreshold), nameof(lblWhiteThresholdValue.Text), FormatInt, lblWhiteThresholdValue);

            // Number of colors
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.NumColorsVisible), nameof(tblNumColors.Visible), tblNumColors);
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.NumColors), tbNumColors, nameof(tbNumColors.Value));
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.NumColors), nameof(lblNumColorsValue.Text), FormatInt, lblNumColorsValue);
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.MaxColors), nameof(tbNumColors.Maximum), tbNumColors);

            // DirectMapping
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.DirectMappingVisible), nameof(chbDirectMapping.Visible), chbDirectMapping);
            CommandBindings.AddPropertyBinding(chbDirectMapping, nameof(chbDirectMapping.Checked), nameof(ViewModel.DirectMapping), ViewModel);

            // BitLevel
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.BitLevelVisible), nameof(tblBitLevel.Visible), tblBitLevel);
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.BitLevel), tbBitLevel, nameof(tbBitLevel.Value));
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.BitLevel), nameof(lblBitLevelValue.Text), FormatBitLevel, lblBitLevelValue);

            // cmbQuantizer.SelectedValue -> VM.SelectedQuantizer (intentionally last so visibilities are already bound)
            CommandBindings.AddPropertyBinding(cmbQuantizer, nameof(cmbQuantizer.SelectedValue), nameof(ViewModel.SelectedQuantizer), ViewModel);
        }

        #endregion

        #region Command Handlers

        private void OnSelectBackColorCommand()
        {
            Color? result = Dialogs.PickColor(ViewModel!.BackColor);
            if (result.HasValue)
                ViewModel.BackColor = result.Value;
        }

        private void OnResetParentSizeCommand()
        {
            Control? parent = Parent?.Parent;
            if (parent == null)
                return;

            int height = 0;
            foreach (Control control in Controls)
            {
                if (!control.Visible)
                    continue;

                height += control.Height;
            }

            parent.Height = height + (parent.Height - parent.DisplayRectangle.Height);
        }

        #endregion

        #endregion
    }
}
