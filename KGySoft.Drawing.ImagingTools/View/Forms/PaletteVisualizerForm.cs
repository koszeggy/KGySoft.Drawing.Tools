#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: PaletteVisualizerForm.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2019 - All Rights Reserved
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
using System.Drawing;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    internal partial class PaletteVisualizerForm : MvvmBaseForm<PaletteVisualizerViewModel>
    {
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
            UpdateInfo();
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

            // VM.ReadOnly -> ucColorVisualizer.ReadOnly
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.ReadOnly), nameof(ucColorVisualizer.ReadOnly), ucColorVisualizer);

            // pnlPalette.SelectedColor -> ucColorVisualizer.Color
            CommandBindings.AddPropertyBinding(pnlPalette, nameof(pnlPalette.SelectedColor), nameof(ucColorVisualizer.Color), ucColorVisualizer);

            // VM.IsModified -> OKButton.Enabled
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.IsModified), nameof(okCancelButtons.OKButton.Enabled), okCancelButtons.OKButton);
        }

        private void InitCommandBindings()
        {
            CommandBindings.Add(OnColorEditedCommand)
                .AddSource(ucColorVisualizer, nameof(ucColorVisualizer.ColorEdited));
            CommandBindings.Add(OnSelectedColorChangedCommand)
                .AddSource(pnlPalette, nameof(pnlPalette.SelectedColorChanged));
            CommandBindings.Add(OnCancelCommand)
                .AddSource(okCancelButtons.CancelButton, nameof(okCancelButtons.CancelButton.Click));
        }

        private void UpdateInfo() => ucColorVisualizer.SpecialInfo = Res.InfoSelectedIndex(pnlPalette.SelectedColorIndex);

        #endregion

        #region Command Handlers

        private void OnColorEditedCommand()
        {
            if (ViewModel.ReadOnly)
                return;

            // if there is no cloning, both of the following lines set the same instance in the collection but it is cleaner to set twice
            ViewModel.Palette[pnlPalette.SelectedColorIndex] = ucColorVisualizer.Color;
            pnlPalette.SelectedColor = ucColorVisualizer.Color;
            ViewModel.SetModified(true);
        }

        private void OnSelectedColorChangedCommand() => UpdateInfo();

        private void OnCancelCommand() => ViewModel.SetModified(false);

        #endregion

        #endregion
    }
}
