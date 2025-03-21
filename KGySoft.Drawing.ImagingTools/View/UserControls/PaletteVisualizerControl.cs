#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: PaletteVisualizerControl.cs
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
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.View.Forms;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal partial class PaletteVisualizerControl : MvvmBaseUserControl
    {
        #region Fields

        private ParentViewProperties? parentProperties;

        #endregion

        #region Properties

        #region Internal Properties

        internal override ParentViewProperties ParentViewProperties => parentProperties ??= new ParentViewProperties
        {
            BorderStyle = FormBorderStyle.SizableToolWindow,
            Icon = Properties.Resources.Palette,
            MinimumSize = new Size(255, 335),
            MaximumSize = new Size(280, Int16.MaxValue),
            AcceptButton = okCancelButtons.OKButton,
            CancelButton = okCancelButtons.CancelButton,
            ClosingCallback = (sender, _) =>
            {
                if (((Form)sender).DialogResult != DialogResult.OK)
                    ViewModel.SetModified(false);
            },
            ProcessKeyCallback = (parent, key) =>
            {
                if (key == Keys.Escape && ViewModel.ReadOnly) // if not ReadOnly, use the Cancel button
                {
                    parent.DialogResult = DialogResult.Cancel;
                    return true;
                }

                return false;
            }
        };

        internal override Action<MvvmParentForm> ParentViewPropertyBindingsInitializer => InitParentViewPropertyBindings;

        #endregion

        #region Private Properties

        private new PaletteVisualizerViewModel ViewModel => (PaletteVisualizerViewModel)base.ViewModel!;

        #endregion

        #endregion

        #region Constructors

        #region Internal Constructors

        internal PaletteVisualizerControl(PaletteVisualizerViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }

        #endregion

        #region Private Constructors

        private PaletteVisualizerControl() : this(null!)
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
            if (OSUtils.IsMono && (scale = this.GetScale()) != new PointF(1f, 1f) && ParentForm is MvvmParentForm parent)
            {
                parent.MinimumSize = new Size(255, 335).Scale(scale);
                parent.MaximumSize = new Size((int)(280 * scale.X), Int16.MaxValue);
            }

            base.OnLoad(e);
        }

        protected override void ApplyViewModel()
        {
            InitPropertyBindings();
            InitCommandBindings();
            base.ApplyViewModel();
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
                components?.Dispose();
            parentProperties = null;
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

            // VM.SelectedColorViewModel -> colorVisualizerControl.ViewModel
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.SelectedColorViewModel), nameof(colorVisualizerControl.ViewModel), colorVisualizerControl);

            // pnlPalette.SelectedColorIndex -> VM.SelectedColorIndex
            CommandBindings.AddPropertyBinding(pnlPalette, nameof(pnlPalette.SelectedColorIndex), nameof(ViewModel.SelectedColorIndex), ViewModel);

            // VM.IsModified -> OKButton.Enabled
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.IsModified), nameof(okCancelButtons.OKButton.Enabled), okCancelButtons.OKButton);

            bool isInForm = ParentForm != null;
            okCancelButtons.DefaultButtonsVisible = isInForm;
            okCancelButtons.ApplyButtonVisible = !isInForm;
        }

        private void InitParentViewPropertyBindings(MvvmParentForm parent)
        {
            // VM.TitleCaption -> Text
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.TitleCaption), nameof(parent.Text), parent);
        }

        private void InitCommandBindings()
        {
            // CancelButton.Click -> OnCancelCommand
            CommandBindings.Add(OnCancelCommand)
                .AddSource(okCancelButtons.CancelButton, nameof(okCancelButtons.CancelButton.Click));

            // ApplyButton.Click -> VM.ApplyChangesCommand
            CommandBindings.Add(ViewModel.ApplyChangesCommand, ViewModel.ApplyChangesCommandCommandState)
                .AddSource(okCancelButtons.ApplyButton, nameof(okCancelButtons.ApplyButton.Click));
        }

        #endregion

        #region Command Handlers

        private void OnCancelCommand() => ViewModel.SetModified(false);

        #endregion

        #endregion
    }
}
