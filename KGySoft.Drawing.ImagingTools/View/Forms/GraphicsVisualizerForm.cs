#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: GraphicsVisualizerForm.cs
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

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    internal partial class GraphicsVisualizerForm : ImageVisualizerForm
    {
        #region Fields

        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed",
            Justification = "False alarm, added to tsMenu, which is disposed by base")]
        private readonly ToolStripButton btnCrop;

        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed",
            Justification = "False alarm, added to tsMenu, which is disposed by base")]
        private readonly ToolStripButton btnHighlightClip;

        #endregion

        #region Properties

        private GraphicsVisualizerViewModel VM => (GraphicsVisualizerViewModel)ViewModel;

        #endregion

        #region Constructors

        #region Internal Constructors

        internal GraphicsVisualizerForm(GraphicsVisualizerViewModel viewModel)
            : base(viewModel)
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

        private GraphicsVisualizerForm() : this(null)
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
            if (disposing)
                components?.Dispose();

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void InitViewModelDependencies()
        {
            VM.DrawFocusRectangleCallback = (g, visibleRect) => ControlPaint.DrawFocusRectangle(g, visibleRect, Color.White, Color.Black);
        }

        private void InitPropertyBindings()
        {
            // VM.Crop -> btnCrop.Checked
            CommandBindings.AddPropertyBinding(VM, nameof(VM.Crop), nameof(btnCrop.Checked), btnCrop);

            // VM.HighlightVisibleClip -> btnHighlightClip.Checked
            CommandBindings.AddPropertyBinding(VM, nameof(VM.HighlightVisibleClip), nameof(btnHighlightClip.Checked), btnHighlightClip);
        }

        private void InitCommandBindings()
        {
            CommandBindings.Add(VM.CropCommand, VM.CropCommandState)
                .WithParameter(() => btnCrop.Checked)
                .AddSource(btnCrop, nameof(btnCrop.CheckedChanged));
            CommandBindings.Add(VM.HighlightVisibleClipCommand, VM.HighlightVisibleClipCommandState)
                .WithParameter(() => btnHighlightClip.Checked)
                .AddSource(btnHighlightClip, nameof(btnHighlightClip.CheckedChanged));
        }

        #endregion

        #endregion
    }
}
