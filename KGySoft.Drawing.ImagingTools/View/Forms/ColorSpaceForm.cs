#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorSpaceForm.cs
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

using System.Linq;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    internal sealed partial class ColorSpaceForm : TransformBitmapFormBase
    {
        #region Properties

        // this would not be needed if designer had better generics support
        private ColorSpaceViewModel VM => (ColorSpaceViewModel)ViewModel;

        #endregion

        #region Constructors

        #region Internal Constructors

        internal ColorSpaceForm(ColorSpaceViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();

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

        private ColorSpaceForm() : this(null)
        {
            // this ctor is just for the designer
        }

        #endregion

        #endregion

        #region Methods

        #region Protected Methods

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
            cmbPixelFormat.DataSource = VM.PixelFormats;
            quantizerSelector.ViewModel = VM.QuantizerSelectorViewModel;
            dithererSelector.ViewModel = VM.DithererSelectorViewModel;

            // VM.ChangePixelFormat <-> gbPixelFormat.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(VM.ChangePixelFormat), gbPixelFormat, nameof(gbPixelFormat.Checked));

            // VM.SelectedPixelFormat -> cmbPixelFormat.SelectedItem (cannot use two-way for SelectedItem because there is no SelectedItemChanged event)
            CommandBindings.AddPropertyBinding(ViewModel, nameof(VM.PixelFormat), nameof(cmbPixelFormat.SelectedItem), cmbPixelFormat);

            // cmbPixelFormat.SelectedItem -> VM.SelectedPixelFormat (cannot use two-way for SelectedValue because ValueMember is not set)
            CommandBindings.AddPropertyBinding(cmbPixelFormat, nameof(cmbPixelFormat.SelectedValue), nameof(VM.PixelFormat), ViewModel);

            // VM.UseQuantizer <-> gbQuantizer.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(VM.UseQuantizer), gbQuantizer, nameof(gbQuantizer.Checked));

            // VM.UseDitherer <-> gbDitherer.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(VM.UseDitherer), gbDitherer, nameof(gbDitherer.Checked));
        }

        #endregion

        #endregion
    }
}
