#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: PreviewImageControl.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2026 - All Rights Reserved
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
using KGySoft.WinForms.Controls;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal partial class PreviewImageControl : MvvmBaseUserControl
    {
        #region Fields

        private bool isProcessingCmdKey;

        #endregion

        #region Properties

        internal new PreviewImageViewModel? ViewModel
        {
            get => (PreviewImageViewModel?)base.ViewModel;
            set => base.ViewModel = value;
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

        #region Internal Methods

        internal bool ProcessCmdKeyInternal(ref Message m, Keys keyData)
        {
            if (isProcessingCmdKey)
                return false;
            isProcessingCmdKey = true;
            try
            {
                return ProcessCmdKey(ref m, keyData);
            }
            finally
            {
                isProcessingCmdKey = false;
            }
        }

        #endregion

        #region Protected Methods

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

        protected override void ApplyTheme()
        {
            base.ApplyTheme();
            ivPreview.BackColor = ThemeColors.Control;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Alt | Keys.S:
                    ViewModel?.SmoothZooming = !ViewModel.SmoothZooming;
                    return true;

                default:
                    bool result = base.ProcessCmdKey(ref msg, keyData);

                    // Workaround: ToolStrip hotkeys may stop working when their parent is moved to the overflow area, and the overflow button was dropped down since then.
                    if (!result && ToolStripManager.IsShortcutDefined(keyData))
                        result = tsMenu.ProcessCmdKeyInternal(ref msg, keyData);
                    return result;
            }
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

            // btnAntiAlias.Checked <-> VM.SmoothZooming -> ivPreview.SmoothingEnabled
            CommandBindings.AddTwoWayPropertyBinding(ViewModel!, nameof(ViewModel.SmoothZooming), btnAntiAlias, nameof(btnAntiAlias.Checked));
            CommandBindings.AddPropertyBinding(ViewModel!, nameof(ViewModel.SmoothZooming), nameof(ivPreview.SmoothingEnabled), ivPreview);
        }

        #endregion

        #endregion
    }
}
