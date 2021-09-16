#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorVisualizerForm.cs
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

using System.Drawing;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    internal partial class ColorVisualizerForm : MvvmBaseForm<ColorVisualizerViewModel>
    {
        #region Constructors

        #region Internal Constructors

        internal ColorVisualizerForm(ColorVisualizerViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
            AcceptButton = okCancelButtons.OKButton;
            CancelButton = okCancelButtons.CancelButton;
        }

        #endregion

        #region Private Constructors

        private ColorVisualizerForm() : this(null!)
        {
            // this ctor is just for the designer
        }

        #endregion

        #endregion

        #region Methods

        #region Protected Methods

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

            // VM.ReadOnly -> ucColorVisualizer.ReadOnly
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.ReadOnly), nameof(ucColorVisualizer.ReadOnly), ucColorVisualizer);

            // VM.Color -> ucColorVisualizer.Color, Text
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.Color), nameof(ucColorVisualizer.Color), ucColorVisualizer);
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.Color), nameof(Text), c => Res.TitleColor((Color)c!), this);

            // VM.IsModified -> OKButton.Enabled
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.IsModified), nameof(okCancelButtons.OKButton.Enabled), okCancelButtons.OKButton);
        }

        private void InitCommandBindings()
        {
            CommandBindings.Add(OnColorEditedCommand)
                .AddSource(ucColorVisualizer, nameof(ucColorVisualizer.ColorEdited));
            CommandBindings.Add(OnCancelCommand)
                .AddSource(okCancelButtons.CancelButton, nameof(okCancelButtons.CancelButton.Click));
        }

        #endregion

        #region Command Handlers

        private void OnColorEditedCommand()
        {
            if (ViewModel.ReadOnly)
                return;
            ViewModel.Color = ucColorVisualizer.Color;
        }

        private void OnCancelCommand() => ViewModel.SetModified(false);

        #endregion

        #endregion
    }
}
