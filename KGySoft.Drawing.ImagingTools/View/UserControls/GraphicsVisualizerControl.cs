#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: GraphicsVisualizerControl.cs
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
    internal partial class GraphicsVisualizerControl : ImageVisualizerControl
    {
        #region Fields

        private readonly ToolStripButton btnCrop;
        private readonly ToolStripButton btnHighlightClip;

        #endregion

        #region Properties

        private new GraphicsVisualizerViewModel ViewModel => (GraphicsVisualizerViewModel)base.ViewModel!;

        #endregion

        #region Constructors

        #region Internal Constructors

        internal GraphicsVisualizerControl(GraphicsVisualizerViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();

            ToolStripItem separator = new ToolStripSeparator();
            btnCrop = new ToolStripButton
            {
                Name = nameof(btnCrop),
                CheckOnClick = true,
                DisplayStyle = ToolStripItemDisplayStyle.Image
            };

            btnHighlightClip = new ToolStripButton
            {
                Name = nameof(btnHighlightClip),
                CheckOnClick = true,
                DisplayStyle = ToolStripItemDisplayStyle.Image
            };

            tsMenu.Items.AddRange(new ToolStripItem[] { separator, btnCrop, btnHighlightClip });
        }

        #endregion

        #region Private Constructors

        private GraphicsVisualizerControl() : this(null!)
        {
            // this ctor is just for the designer
        }

        #endregion

        #endregion

        #region Methods

        #region Protected Methods

        protected override void ApplyResources()
        {
            btnCrop.Image = Images.Crop;
            btnHighlightClip.Image = Images.HighlightVisibleClip;
            base.ApplyResources();
        }

        protected override void ApplyViewModel()
        {
            InitViewModelDependencies();
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

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void InitViewModelDependencies()
        {
            ViewModel.DrawFocusRectangleCallback = (g, visibleRect) => ControlPaint.DrawFocusRectangle(g, visibleRect, Color.White, Color.Black);
        }

        private void InitPropertyBindings()
        {
            // ViewModel.Crop -> btnCrop.Checked
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.Crop), nameof(btnCrop.Checked), btnCrop);

            // ViewModel.HighlightVisibleClip -> btnHighlightClip.Checked
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.HighlightVisibleClip), nameof(btnHighlightClip.Checked), btnHighlightClip);
        }

        private void InitCommandBindings()
        {
            CommandBindings.Add(ViewModel.CropCommand, ViewModel.CropCommandState)
                .WithParameter(() => btnCrop.Checked)
                .AddSource(btnCrop, nameof(btnCrop.CheckedChanged));
            CommandBindings.Add(ViewModel.HighlightVisibleClipCommand, ViewModel.HighlightVisibleClipCommandState)
                .WithParameter(() => btnHighlightClip.Checked)
                .AddSource(btnHighlightClip, nameof(btnHighlightClip.CheckedChanged));
        }

        #endregion

        #endregion
    }
}
