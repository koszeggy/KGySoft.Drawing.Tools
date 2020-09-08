#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: PreviewImageControl.cs
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

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using KGySoft.Drawing.ImagingTools.View.Controls;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal partial class PreviewImageControl : MvvmBaseUserControl<PreviewImageViewModel>
    {
        #region Properties

        internal Image Image
        {
            get => ViewModel.Image;
            set => ViewModel.Image = value;
        }

        internal bool AutoZoom
        {
            get => ViewModel.AutoZoom;
            set => ViewModel.AutoZoom = value;
        }

        internal ImageViewer ImageViewer => ivPreview;

        #endregion

        #region Constructors

        public PreviewImageControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();

            base.Dispose(disposing);
        }

        protected override void ApplyResources()
        {
            btnAutoZoom.Image = Images.Magnifier;
            btnAntiAlias.Image = Images.SmoothZoom;
            base.ApplyResources();
        }

        protected override void ApplyViewModel()
        {
            InitPropertyBindings();
            base.ApplyViewModel();
        }

        #endregion

        #region Private Methods

        private void InitPropertyBindings()
        {
            // VM.Image -> ivPreview.Image
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.Image), nameof(ivPreview.Image), ivPreview);

            // VM.ButtonsEnabled -> btnAutoZoom/btnAntiAlias.Enabled
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.ButtonsEnabled), nameof(ToolStripItem.Enabled), btnAutoZoom, btnAntiAlias);

            // btnAutoZoom.Checked <-> VM.AutoZoom -> ivPreview.AutoZoom
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.AutoZoom), btnAutoZoom, nameof(btnAutoZoom.Checked));
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.AutoZoom), nameof(ivPreview.AutoZoom), ivPreview);

            // btnAntiAlias.Checked -> VM.SmoothZooming -> ivPreview.SmoothZooming
            CommandBindings.AddPropertyBinding(btnAntiAlias, nameof(btnAntiAlias.Checked), nameof(ViewModel.SmoothZooming), ViewModel);
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.SmoothZooming), nameof(ivPreview.SmoothZooming), ivPreview);
        }

        #endregion

        #endregion
    }
}
