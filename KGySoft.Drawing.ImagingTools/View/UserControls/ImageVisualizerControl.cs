#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageVisualizerControl.cs
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

using KGySoft.ComponentModel;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Drawing.ImagingTools.View.Forms;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

#region Suppressions

#if NETCOREAPP3_0
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type. - DropDownItems items are never null
#pragma warning disable CS8602 // Dereference of a possibly null reference. - DropDownItems items are never null
#endif

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal partial class ImageVisualizerControl : MvvmBaseUserControl
    {
        #region Fields

        private ParentViewProperties? parentProperties;

        #endregion

        #region Properties

        #region Internal Properties

        internal override ParentViewProperties ParentViewProperties => parentProperties ??= new ParentViewProperties
        {
            BorderStyle = FormBorderStyle.SizableToolWindow,
            Icon = Properties.Resources.ImagingTools,
            MinimumSize = new Size(200, 200),
            AcceptButton = buttons.OKButton,
            CancelButton = buttons.CancelButton,
            ProcessKeyCallback = (parent, key) =>
            {
                if (key == Keys.Escape && ViewModel.ReadOnly)
                {
                    parent.DialogResult = DialogResult.Cancel;
                    return true;
                }

                return false;
            },
            ClosingCallback = (sender, _) =>
            {
                if (((MvvmParentForm)sender).DialogResult == DialogResult.Cancel)
                    ViewModel.SetModified(false);
            }
        };

        internal override Action<MvvmParentForm> ParentViewPropertyBindingsInitializer => InitParentViewPropertyBindings;

        #endregion

        #region Private Properties

        private new ImageVisualizerViewModel ViewModel => (ImageVisualizerViewModel)base.ViewModel!;

        #endregion

        #endregion

        #region Constructors

        #region Internal Constructors

        internal ImageVisualizerControl(ImageVisualizerViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }

        #endregion

        #region Private Constructors

        private ImageVisualizerControl() : this(null!)
        {
            // this ctor is just for the designer
        }

        #endregion

        #endregion

        #region Methods

        #region Static Methods

        private static Image GetCompoundViewIcon(ImageInfoType type)
        {
            switch (type)
            {
                case ImageInfoType.Pages:
                    return Images.MultiPage;
                case ImageInfoType.Animation:
                    return Images.Animation;
                case ImageInfoType.MultiRes:
                case ImageInfoType.Icon:
                    return Images.MultiSize;
                default:
                    throw new InvalidOperationException(Res.InternalError($"Unexpected compound type: {type}"));
            }
        }

        #endregion

        #region Instance Methods

        #region Protected Methods

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            tsMenu.FixAppearance();
        }

        protected override void ApplyResources()
        {
            base.ApplyResources();

            btnAntiAlias.Image = Images.SmoothZoom;
            btnOpen.Image = Images.Open;
            btnSave.Image = Images.Save;
            btnClear.Image = Images.Clear;

            btnColorSettings.Image = Images.Palette;
            miBackColorDefault.Image = Images.Check;
            miShowPalette.Image = Images.Palette;

            btnEdit.Image = Images.Edit;
            miRotateLeft.Image = Images.RotateLeft;
            miRotateRight.Image = Images.RotateRight;
            miResizeBitmap.Image = Images.Resize;
            miColorSpace.Image = Images.Quantize;
            miAdjustColors.Image = Images.Colors;

            btnPrev.Image = Images.Prev;
            btnNext.Image = Images.Next;

            miManageInstallations.Image = Images.Settings;
            miLanguageSettings.Image = Images.Language;
            btnConfiguration.SetDefaultItem(miManageInstallations);

            miEasterEgg.Image = Images.ImagingTools;
            btnAbout.Image = miAbout.Image = Icons.SystemInformation.ToScaledBitmap();
        }

        protected override void ApplyStringResources()
        {
            base.ApplyStringResources();
            dlgOpen.Title = Res.TitleOpenFileDialog;
            dlgSave.Title = Res.TitleSaveFileDialog;
        }

        protected override void ApplyViewModel()
        {
            InitViewModelDependencies();
            InitPropertyBindings();
            InitCommandBindings();
            base.ApplyViewModel(); //Apply is Enabled, OK is Disabled. Why?
            imageViewer.Focus();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Control | Keys.O:
                    btnOpen.PerformClick();
                    return true;
                case Keys.Control | Keys.S:
                    btnSave.PerformClick();
                    return true;
                case Keys.Control | Keys.Delete:
                    btnClear.PerformClick();
                    return true;
                case Keys.Alt | Keys.S:
                    btnAntiAlias.PerformClick();
                    return true;
                case Keys.Shift | Keys.Right:
                    (RightToLeft == RightToLeft.Yes ? btnPrev : btnNext).PerformClick();
                    return true;
                case Keys.Shift | Keys.Left:
                    (RightToLeft == RightToLeft.Yes ? btnNext : btnPrev).PerformClick();
                    return true;

                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
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

        private void InitViewModelDependencies()
        {
            ViewModel.GetScreenRectangleCallback = GetScreenRectangle;
            ViewModel.GetViewSizeCallback = () => ParentForm?.Size ?? Size;
            ViewModel.GetImagePreviewSizeCallback = () => imageViewer.ClientSize;
            ViewModel.SelectFileToOpenCallback = SelectFileToOpen;
            ViewModel.SelectFileToSaveCallback = SelectFileToSave;
            ViewModel.ApplyViewSizeCallback = ApplySize;
            ViewModel.UpdatePreviewImageCallback = () => imageViewer.UpdateImage();
            ViewModel.GetCompoundViewIconCallback = GetCompoundViewIcon;
        }

        private void InitPropertyBindings()
        {
            // not as binding because will not change, and we don't need the buttons for main form
            if (ViewModel.ReadOnly)
                buttons.Visible = false;

            // VM.Notification -> lblNotification.Text
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.Notification), nameof(Label.Text), lblNotification);

            // VM.PreviewImage != null -> btnSave.Enabled
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.PreviewImage), nameof(Button.Enabled), img => img != null, btnSave);

            // VM.InfoText -> txtInfo.Text
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.InfoText), nameof(TextBox.Text), txtInfo);

            // imageViewer.AutoZoom <-> VM.AutoZoom -> btnZoom.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.AutoZoom), imageViewer, nameof(imageViewer.AutoZoom));
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.AutoZoom), nameof(btnZoom.Checked), btnZoom);

            // VM.Zoom <-> imageViewer.Zoom
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.Zoom), imageViewer, nameof(imageViewer.Zoom));

            // VM.SmoothZooming -> btnAntiAlias.Checked
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.SmoothZooming), nameof(btnAntiAlias.Checked), btnAntiAlias);
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.SmoothZooming), nameof(imageViewer.SmoothZooming), imageViewer);

            // VM.IsCompoundView -> btnCompound.Checked
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.IsCompoundView), nameof(btnCompound.Checked), btnCompound);

            // VM.IsAutoPlaying -> timerPlayer.Enabled
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.IsAutoPlaying), nameof(timerPlayer.Enabled), timerPlayer);

            // VM.PreviewImage -> imageViewer.Image
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.PreviewImage), nameof(imageViewer.Image), imageViewer);

            // VM.SetCompoundViewCommandState.Visible -> sepFrames.Visible, btnCompound.Visible, btnPrev.Visible, btnNext.Visible
            CommandBindings.AddPropertyBinding(ViewModel.SetCompoundViewCommandState, nameof(Visible), nameof(Visible),
                sepFrames, btnCompound, btnPrev, btnNext);

            // VM.OpenFileFilter -> dlgOpen.Filter
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.OpenFileFilter), nameof(dlgOpen.Filter), dlgOpen);

            // VM.SaveFileFilter/Index/DefaultExtension -> dlgSave.Filter/Index/DefaultExt
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.SaveFileFilter), nameof(dlgSave.Filter), dlgSave);
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.SaveFileFilterIndex), nameof(dlgSave.FilterIndex), dlgSave);
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.SaveFileDefaultExtension), nameof(dlgSave.DefaultExt), dlgSave);

            // VM.IsModified -> OKButton.Enabled
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.IsModified), nameof(Enabled), buttons.OKButton);

            bool isInForm = ParentForm != null;
            buttons.DefaultButtonsVisible = isInForm;
            buttons.ApplyButtonVisible = !isInForm;
        }

        private void InitParentViewPropertyBindings(MvvmParentForm parent)
        {
            // VM.TitleCaption -> Text
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.TitleCaption), nameof(parent.Text), parent);
        }

        private void InitCommandBindings()
        {
            // View
            CommandBindings.Add(ViewModel.SetAutoZoomCommand, ViewModel.SetAutoZoomCommandState)
                .WithParameter(() => btnZoom.Checked)
                .AddSource(btnZoom, nameof(btnZoom.CheckedChanged));
            CommandBindings.Add(imageViewer.IncreaseZoom)
                .AddSource(btnZoom.IncreaseZoomMenuItem, nameof(btnZoom.IncreaseZoomMenuItem.Click));
            CommandBindings.Add(imageViewer.DecreaseZoom)
                .AddSource(btnZoom.DecreaseZoomMenuItem, nameof(btnZoom.DecreaseZoomMenuItem.Click));
            CommandBindings.Add(imageViewer.ResetZoom)
                .AddSource(btnZoom.ResetZoomMenuItem, nameof(btnZoom.ResetZoomMenuItem.Click));
            CommandBindings.Add(ViewModel.SetSmoothZoomingCommand, ViewModel.SetSmoothZoomingCommandState)
                .WithParameter(() => btnAntiAlias.Checked)
                .AddSource(btnAntiAlias, nameof(btnAntiAlias.CheckedChanged));

            // File
            CommandBindings.Add(ViewModel.OpenFileCommand, ViewModel.OpenFileCommandState)
                .AddSource(btnOpen, nameof(btnOpen.Click));
            CommandBindings.Add(ViewModel.SaveFileCommand, ViewModel.SaveFileCommandState)
                .AddSource(btnSave, nameof(btnSave.Click));
            CommandBindings.Add(ViewModel.ClearCommand, ViewModel.ClearCommandState)
                .AddSource(btnClear, nameof(btnClear.Click));

            // Color Settings
            CommandBindings.Add<EventArgs>(OnSetBackColorCommand)
                .AddSource(miBackColorDefault, nameof(miBackColorDefault.Click))
                .AddSource(miBackColorBlack, nameof(miBackColorBlack.Click))
                .AddSource(miBackColorWhite, nameof(miBackColorWhite.Click));
            CommandBindings.Add(ViewModel.ShowPaletteCommand, ViewModel.ShowPaletteCommandState)
                .AddSource(miShowPalette, nameof(miShowPalette.Click));
            CommandBindings.Add(ViewModel.CountColorsCommand, ViewModel.CountColorsCommandState)
                .AddSource(miCountColors, nameof(miCountColors.Click));

            // Edit
            CommandBindings.Add(ViewModel.RotateLeftCommand, ViewModel.EditBitmapCommandState)
                .AddSource(miRotateLeft, nameof(miRotateLeft.Click));
            CommandBindings.Add(ViewModel.RotateRightCommand, ViewModel.EditBitmapCommandState)
                .AddSource(miRotateRight, nameof(miRotateRight.Click));
            CommandBindings.Add(ViewModel.ResizeBitmapCommand, ViewModel.EditBitmapCommandState)
                .AddSource(miResizeBitmap, nameof(miResizeBitmap.Click));
            CommandBindings.Add(ViewModel.AdjustColorSpaceCommand, ViewModel.EditBitmapCommandState)
                .AddSource(miColorSpace, nameof(miColorSpace.Click));
            CommandBindings.Add(ViewModel.AdjustBrightnessCommand, ViewModel.EditBitmapCommandState)
                .AddSource(miBrightness, nameof(miBrightness.Click));
            CommandBindings.Add(ViewModel.AdjustContrastCommand, ViewModel.EditBitmapCommandState)
                .AddSource(miContrast, nameof(miContrast.Click));
            CommandBindings.Add(ViewModel.AdjustGammaCommand, ViewModel.EditBitmapCommandState)
                .AddSource(miGamma, nameof(miGamma.Click));

            // Compound images
            CommandBindings.Add(ViewModel.SetCompoundViewCommand, ViewModel.SetCompoundViewCommandState)
                .WithParameter(() => btnCompound.Checked)
                .AddSource(btnCompound, nameof(btnCompound.CheckedChanged));
            CommandBindings.Add(ViewModel.PrevImageCommand, ViewModel.PrevImageCommandState)
                .AddSource(btnPrev, nameof(btnPrev.Click));
            CommandBindings.Add(ViewModel.NextImageCommand, ViewModel.NextImageCommandState)
                .AddSource(btnNext, nameof(btnNext.Click));
            CommandBindings.Add(ViewModel.AdvanceAnimationCommand, ViewModel.AdvanceAnimationCommandState)
                .AddSource(timerPlayer, nameof(timerPlayer.Tick));
            CommandBindings.Add(ViewModel.ViewImagePreviewSizeChangedCommand)
                .AddSource(imageViewer, nameof(imageViewer.SizeChanged))
                .AddSource(imageViewer, nameof(imageViewer.ZoomChanged));

            // Configuration
            CommandBindings.Add(ViewModel.ManageInstallationsCommand)
                .AddSource(miManageInstallations, nameof(miManageInstallations.Click));
            CommandBindings.Add(ViewModel.SetLanguageCommand)
                .AddSource(miLanguageSettings, nameof(miLanguageSettings.Click));

            // About
            CommandBindings.Add(ViewModel.ShowAboutCommand)
                .AddSource(btnAbout, nameof(btnAbout.ButtonClick));
            CommandBindings.Add(ViewModel.ShowAboutCommand)
                .AddSource(miAbout, nameof(miAbout.Click));
            CommandBindings.Add(ViewModel.VisitWebSiteCommand)
                .AddSource(miWebSite, nameof(miWebSite.Click));
            CommandBindings.Add(ViewModel.VisitGitHubCommand)
                .AddSource(miGitHub, nameof(miGitHub.Click));
            CommandBindings.Add(ViewModel.VisitMarketplaceCommand)
                .AddSource(miMarketplace, nameof(miMarketplace.Click));
            CommandBindings.Add(ViewModel.SubmitResourcesCommand)
                .AddSource(miSubmitResources, nameof(miSubmitResources.Click));
            CommandBindings.Add(ViewModel.ShowEasterEggCommand)
                .AddSource(miEasterEgg, nameof(miEasterEgg.Click));

            // View commands
            CommandBindings.Add(OnResizeCommand)
                .AddSource(this, nameof(Resize));
            CommandBindings.Add(OnPreviewImageResizedCommand)
                .AddSource(imageViewer, nameof(imageViewer.SizeChanged));
            CommandBindings.Add(() => miEasterEgg.Visible |= ModifierKeys == (Keys.Shift | Keys.Control))
                .AddSource(miAbout, nameof(miAbout.MouseDown));

            // ApplyButton.Click -> VM.ApplyChangesCommand
            CommandBindings.Add(ViewModel.ApplyChangesCommand, ViewModel.ApplyChangesCommandCommandState)
                .AddSource(buttons.ApplyButton, nameof(buttons.ApplyButton.Click));
        }

        private Rectangle GetScreenRectangle() => Screen.FromHandle(Handle).WorkingArea;

        private string? SelectFileToOpen()
        {
            if (dlgOpen.ShowDialog(this) != DialogResult.OK)
                return null;
            return dlgOpen.FileName;
        }

        private string? SelectFileToSave()
        {
            if (dlgSave.ShowDialog(this) != DialogResult.OK)
                return null;
            ViewModel.SaveFileFilterIndex = dlgSave.FilterIndex;
            return dlgSave.FileName;
        }

        private void AdjustSize()
        {
            int minHeight = new Size(16, 16).Scale(this.GetScale()).Height + SystemInformation.HorizontalScrollBarHeight;
            if (imageViewer.Height >= minHeight)
                return;
            int buttonsHeight = buttons.Visible ? buttons.Height : 0;
            int notificationHeight = lblNotification.Visible ? lblNotification.Height : 0;
            txtInfo.Height = ClientSize.Height - Padding.Vertical - tsMenu.Height - splitter.Height - buttonsHeight - notificationHeight - minHeight;
            PerformLayout();
        }

        private void ApplySize(Size size)
        {
            Form? parent = ParentForm;
            if (parent == null)
                return;

            parent.Size = size;
            Rectangle workingArea = GetScreenRectangle();
            if (parent.Top < workingArea.Top)
                parent.Top = workingArea.Top;
            if (parent.Left < workingArea.Left)
                parent.Left = workingArea.Left;
            if (parent.Bottom > workingArea.Bottom)
                parent.Top = workingArea.Bottom - Height;
            if (parent.Right > workingArea.Right)
                parent.Left = workingArea.Right - Width;
        }

        #endregion

        #region Command Handlers

        private void OnSetBackColorCommand(ICommandSource source)
        {
            var sender = source.Source;
            foreach (ToolStripMenuItem item in miBackColor.DropDownItems)
                item.Image = item == sender ? Images.Check : null;

            if (sender == miBackColorDefault)
                imageViewer.BackColor = SystemColors.Control;
            else if (sender == miBackColorWhite)
                imageViewer.BackColor = Color.White;
            else
                imageViewer.BackColor = Color.Black;
        }

        private void OnPreviewImageResizedCommand() => AdjustSize();

        private void OnResizeCommand() => AdjustSize();

        #endregion

        #endregion

        #endregion
    }
}
