#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: PaletteVisualizerForm.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2022 - All Rights Reserved
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
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    internal partial class PaletteVisualizerForm : MvvmBaseForm
    {
        #region Properties

        private new PaletteVisualizerViewModel ViewModel => (PaletteVisualizerViewModel)base.ViewModel;

        #endregion

        #region Constructors

        #region Internal Constructors

        internal PaletteVisualizerForm(PaletteVisualizerViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
            AcceptButton = okCancelButtons.OKButton;
            CancelButton = okCancelButtons.CancelButton;
        }

        #endregion

        #region Private Constructors

        private PaletteVisualizerForm() : this(null!)
        {
            // this ctor is just for the designer
        }

        #endregion

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnLoad(EventArgs e)
        {
            // Fixing high DPI appearance on Mono
            PointF scale;
            if (OSUtils.IsMono && (scale = this.GetScale()) != new PointF(1f, 1f))
            {
                MinimumSize = new Size(255, 335).Scale(scale);
                MaximumSize = new Size((int)(280 * scale.X), Int16.MaxValue);
            }

            base.OnLoad(e);
        }

        protected override void ApplyResources()
        {
            base.ApplyResources();
            Icon = Properties.Resources.Palette;
        }

        protected override void ApplyViewModel()
        {
            InitPropertyBindings();
            InitCommandBindings();
            base.ApplyViewModel();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
                ViewModel.SetModified(false);
            base.OnFormClosing(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Escape when ViewModel.ReadOnly: // if not ReadOnly, use the Cancel button
                    DialogResult = DialogResult.Cancel;
                    return true;

                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
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
            // !VM.ReadOnly -> okCancelButtons.Visible
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.ReadOnly), nameof(okCancelButtons.Visible), ro => ro is false, okCancelButtons);

            // VM.Palette -> pnlPalette.Palette
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.Palette), nameof(pnlPalette.Palette), pnlPalette);

            // VM.Count -> Text (formatted)
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.Count), nameof(Text), c => Res.TitlePaletteCount((int)c!), this);

            // VM.SelectedColorViewModel -> colorVisualizerControl.ViewModel
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.SelectedColorViewModel), nameof(colorVisualizerControl.ViewModel), colorVisualizerControl);

            // pnlPalette.SelectedColorIndex -> VM.SelectedColorIndex
            CommandBindings.AddPropertyBinding(pnlPalette, nameof(pnlPalette.SelectedColorIndex), nameof(ViewModel.SelectedColorIndex), ViewModel);

            // VM.IsModified -> OKButton.Enabled
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.IsModified), nameof(okCancelButtons.OKButton.Enabled), okCancelButtons.OKButton);
        }

        private void InitCommandBindings()
        {
            CommandBindings.Add(OnCancelCommand)
                .AddSource(okCancelButtons.CancelButton, nameof(okCancelButtons.CancelButton.Click));
        }

        #endregion

        #region Command Handlers

        private void OnCancelCommand() => ViewModel.SetModified(false);

        #endregion

        #endregion
    }
}
