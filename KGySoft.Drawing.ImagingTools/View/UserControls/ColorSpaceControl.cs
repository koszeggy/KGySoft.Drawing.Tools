#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorSpaceControl.cs
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
using System.Linq;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal sealed partial class ColorSpaceControl : TransformBitmapControlBase
    {
        #region Properties

        private new ColorSpaceViewModel ViewModel => (ColorSpaceViewModel)base.ViewModel!;

        #endregion

        #region Constructors

        #region Internal Constructors

        internal ColorSpaceControl(ColorSpaceViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
            BackColor = Color.Transparent; // to make the resize grip in the parent form visible
        }

        #endregion

        #region Private Constructors

        private ColorSpaceControl() : this(null!)
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
            properties.MinimumSize = new Size(400, 460);
            properties.Icon = Properties.Resources.Quantize;
        }

        protected override void OnLoad(EventArgs e)
        {
            // Mono/Windows: ignoring because ToolTips throw an exception if set for an embedded control and
            // since they don't appear for negative padding there is simply no place for them.
            if (!IsLoaded && !(OSUtils.IsMono && OSUtils.IsWindows))
            {
                ValidationMapping[nameof(ViewModel.PixelFormat)] = gbPixelFormat.CheckBox;
                ValidationMapping[nameof(ViewModel.QuantizerSelectorViewModel.Quantizer)] = gbQuantizer.CheckBox;
                ValidationMapping[nameof(ViewModel.DithererSelectorViewModel.Ditherer)] = gbDitherer.CheckBox;
                foreach (Control control in ValidationMapping.Values.Where(c => c is CheckBox))
                {
                    ErrorProvider.SetIconAlignment(control, ErrorIconAlignment.TopRight);
                    WarningProvider.SetIconAlignment(control, ErrorIconAlignment.TopRight);
                    InfoProvider.SetIconAlignment(control, ErrorIconAlignment.TopRight);
                }
            }

            base.OnLoad(e);
        }

        protected override void ApplyViewModel()
        {
            InitCommandBindings();
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

        private void InitCommandBindings()
        {
            CommandBindings.Add(OnAdjustParametersHeightCommand)
                .AddSource(gbQuantizer, nameof(gbQuantizer.SizeChanged))
                .AddSource(gbDitherer, nameof(gbDitherer.SizeChanged));
        }

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

        #region Command Handlers

        private void OnAdjustParametersHeightCommand() => pnlSettings.Height = pnlSettings.Controls.Cast<Control>().Sum(c => c.Height);

        #endregion

        #endregion
    }
}
