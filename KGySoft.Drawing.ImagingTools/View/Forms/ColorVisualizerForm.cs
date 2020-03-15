#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorVisualizerForm.cs
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

using System.Drawing;
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
        }

        #endregion

        #region Private Constructors

        private ColorVisualizerForm() : this(null)
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
            // VM.ReadOnly -> ucColorVisualizer.ReadOnly
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.ReadOnly), nameof(ucColorVisualizer.ReadOnly), ucColorVisualizer);

            // VM.Color -> ucColorVisualizer.Color, Text
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.Color), nameof(ucColorVisualizer.Color), ucColorVisualizer);
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.Color), nameof(Text), c => Res.TitleColor((Color)c), this);
        }

        private void InitCommandBindings()
        {
            CommandBindings.Add(OnColorEditedCommand)
                .AddSource(ucColorVisualizer, nameof(ucColorVisualizer.ColorEdited));
        }

        #endregion

        #region Command Handlers

        private void OnColorEditedCommand()
        {
            if (ViewModel.ReadOnly)
                return;
            ViewModel.Color = ucColorVisualizer.Color;
        }

        #endregion

        #endregion
    }
}
