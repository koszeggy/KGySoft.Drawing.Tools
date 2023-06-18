#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorSpaceForm.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2023 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System.Linq;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    internal sealed partial class ColorSpaceForm : TransformBitmapFormBase
    {
        #region Properties

        private new ColorSpaceViewModel ViewModel => (ColorSpaceViewModel)base.ViewModel;

        #endregion

        #region Constructors

        #region Internal Constructors

        internal ColorSpaceForm(ColorSpaceViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();

            // Mono/Windows: exiting because ToolTips throw an exception if set for an embedded control and
            // since they don't appear for negative padding there is simply no place for them.
            if (OSUtils.IsMono && OSUtils.IsWindows)
                return;

            ValidationMapping[nameof(viewModel.PixelFormat)] = gbPixelFormat.CheckBox;
            ValidationMapping[nameof(viewModel.QuantizerSelectorViewModel.Quantizer)] = gbQuantizer.CheckBox;
            ValidationMapping[nameof(viewModel.DithererSelectorViewModel.Ditherer)] = gbDitherer.CheckBox;
            foreach (Control control in ValidationMapping.Values.Where(c => c is CheckBox))
            {
                ErrorProvider.SetIconAlignment(control, ErrorIconAlignment.TopRight);
                WarningProvider.SetIconAlignment(control, ErrorIconAlignment.TopRight);
                InfoProvider.SetIconAlignment(control, ErrorIconAlignment.TopRight);
            }
        }

        #endregion

        #region Private Constructors

        private ColorSpaceForm() : this(null!)
        {
            // this ctor is just for the designer
        }

        #endregion

        #endregion

        #region Methods

        #region Protected Methods

        protected override void ApplyResources()
        {
            Icon = Properties.Resources.Quantize;
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
            cmbPixelFormat.DataSource = ViewModel.PixelFormats;
            quantizerSelector.ViewModel = ViewModel.QuantizerSelectorViewModel;
            dithererSelector.ViewModel = ViewModel.DithererSelectorViewModel;

            // ViewModel.ChangePixelFormat <-> gbPixelFormat.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.ChangePixelFormat), gbPixelFormat, nameof(gbPixelFormat.Checked));

            // ViewModel.SelectedPixelFormat -> cmbPixelFormat.SelectedItem (cannot use two-way for SelectedItem because there is no SelectedItemChanged event)
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.PixelFormat), nameof(cmbPixelFormat.SelectedItem), cmbPixelFormat);

            // cmbPixelFormat.SelectedValue -> ViewModel.SelectedPixelFormat (cannot use two-way for SelectedValue because ValueMember is not set)
            CommandBindings.AddPropertyBinding(cmbPixelFormat, nameof(cmbPixelFormat.SelectedValue), nameof(ViewModel.PixelFormat), ViewModel);

            // ViewModel.UseQuantizer <-> gbQuantizer.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.UseQuantizer), gbQuantizer, nameof(gbQuantizer.Checked));

            // ViewModel.UseDitherer <-> gbDitherer.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.UseDitherer), gbDitherer, nameof(gbDitherer.Checked));
        }

        #endregion

        #endregion
    }
}
