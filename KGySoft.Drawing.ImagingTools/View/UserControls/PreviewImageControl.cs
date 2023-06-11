#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: PreviewImageControl.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2023 - All Rights Reserved
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

using KGySoft.Drawing.ImagingTools.View.Controls;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal partial class PreviewImageControl : MvvmBaseUserControl
    {
        #region Properties

        internal new PreviewImageViewModel? ViewModel
        {
            get => (PreviewImageViewModel?)base.ViewModel;
            set => base.ViewModel = value;
        }

        internal Image? Image
        {
            get => ViewModel?.PreviewImage;
            set
            {
                if (ViewModel is PreviewImageViewModel vm)
                    vm.PreviewImage = value;
            }
        }

        internal bool AutoZoom
        {
            get => ViewModel?.AutoZoom ?? false;
            set
            {
                if (ViewModel is PreviewImageViewModel vm)
                    vm.AutoZoom = value;
            }
        }

        internal bool SmoothZooming
        {
            get => ViewModel?.SmoothZooming ?? false;
            set
            {
                if (ViewModel is PreviewImageViewModel vm)
                    vm.SmoothZooming = value;
            }
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

        protected override void OnLoad(EventArgs e)
        {
            tsMenu.FixAppearance();
            base.OnLoad(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();

            base.Dispose(disposing);
        }

        protected override void ApplyResources()
        {
            btnAntiAlias.Image = Images.SmoothZoom;
            btnShowOriginal.Image = Images.Compare;
            base.ApplyResources();
        }

        protected override void ApplyViewModel()
        {
            InitCommandBindings();
            InitPropertyBindings();
            base.ApplyViewModel();
        }

        #endregion

        #region Private Methods

        private void InitCommandBindings()
        {
            CommandBindings.Add(ivPreview.IncreaseZoom)
                .AddSource(btnZoom.IncreaseZoomMenuItem, nameof(btnZoom.IncreaseZoomMenuItem.Click));
            CommandBindings.Add(ivPreview.DecreaseZoom)
                .AddSource(btnZoom.DecreaseZoomMenuItem, nameof(btnZoom.DecreaseZoomMenuItem.Click));
            CommandBindings.Add(ivPreview.ResetZoom)
                .AddSource(btnZoom.ResetZoomMenuItem, nameof(btnZoom.ResetZoomMenuItem.Click));
            CommandBindings.Add(() => ViewModel!.ShowOriginal = true)
                .AddSource(btnShowOriginal, nameof(btnShowOriginal.MouseDown));
            CommandBindings.Add(() => ViewModel!.ShowOriginal = false)
                .AddSource(btnShowOriginal, nameof(btnShowOriginal.MouseUp));
        }

        private void InitPropertyBindings()
        {
            // VM.DisplayImage -> ivPreview.Image
            CommandBindings.AddPropertyBinding(ViewModel!, nameof(ViewModel.DisplayImage), nameof(ivPreview.Image), ivPreview);

            // VM.ShowOriginalEnabled -> btnShowOriginal.Enabled
            CommandBindings.AddPropertyBinding(ViewModel!, nameof(ViewModel.ShowOriginalEnabled), nameof(ToolStripItem.Enabled), btnShowOriginal);

            // btnAutoZoom.Checked <-> VM.AutoZoom <-> ivPreview.AutoZoom
            CommandBindings.AddTwoWayPropertyBinding(ViewModel!, nameof(ViewModel.AutoZoom), btnZoom, nameof(btnZoom.Checked));
            CommandBindings.AddTwoWayPropertyBinding(ViewModel!, nameof(ViewModel.AutoZoom), ivPreview, nameof(ivPreview.AutoZoom));

            // btnAntiAlias.Checked <-> VM.SmoothZooming -> ivPreview.SmoothZooming
            CommandBindings.AddTwoWayPropertyBinding(ViewModel!, nameof(ViewModel.SmoothZooming), btnAntiAlias, nameof(btnAntiAlias.Checked));
            CommandBindings.AddPropertyBinding(ViewModel!, nameof(ViewModel.SmoothZooming), nameof(ivPreview.SmoothZooming), ivPreview);
        }

        #endregion

        #endregion
    }
}
