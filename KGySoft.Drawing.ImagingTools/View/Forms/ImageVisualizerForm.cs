#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageVisualizerForm.cs
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

using KGySoft.ComponentModel;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    internal partial class ImageVisualizerForm : MvvmBaseForm<ImageVisualizerViewModel>
    {
        #region Constructors

        #region Internal Constructors

        internal ImageVisualizerForm(ImageVisualizerViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();

            if (OSUtils.IsWindows || SystemInformation.HighContrast)
                return;

            // fixing "dark on dark" menu issue on Linux
            var menuItemBackColor = Color.FromArgb(ProfessionalColors.MenuStripGradientBegin.ToArgb());
            miBackColor.BackColor = miShowPalette.BackColor = miBackColorDefault.BackColor = menuItemBackColor;
        }

        #endregion

        #region Private Constructors

        private ImageVisualizerForm() : this(null)
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

        protected override void ApplyResources()
        {
            // applying static resources
            base.ApplyResources();
            Icon = Properties.Resources.ImagingTools;
            btnAutoZoom.Image = Images.Magnifier;
            btnOpen.Image = Images.Open;
            btnSave.Image = Images.Save;
            btnClear.Image = Images.Clear;
            btnColorSettings.Image = Images.Palette;
            miShowPalette.Image = Images.Palette;
            miBackColorDefault.Image = Images.Check;
            btnPrev.Image = Images.Prev;
            btnNext.Image = Images.Next;
            btnConfiguration.Image = Images.Settings;
            btnAntiAlias.Image = Images.SmoothZoom;
            toolTip.SetToolTip(lblNotification, Res.Get($"{nameof(lblNotification)}.ToolTip"));

            // base cannot handle these because components do not have names and dialogs are not even added to components field
            dlgOpen.Title = Res.Get($"{nameof(dlgOpen)}.{nameof(dlgOpen.Title)}");
            dlgSave.Title = Res.Get($"{nameof(dlgSave)}.{nameof(dlgSave.Title)}");
        }

        protected override void ApplyViewModel()
        {
            InitViewModelDependencies();
            InitPropertyBindings();
            InitCommandBindings();
            base.ApplyViewModel();
            imageViewer.Focus();
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
            ViewModel.GetScreenRectangleCallback = GetScreenRectangle;
            ViewModel.GetViewSizeCallback = () => Size;
            ViewModel.GetImagePreviewSizeCallback = () => imageViewer.ClientSize;
            ViewModel.SelectFileToOpenCallback = SelectFileToOpen;
            ViewModel.SelectFileToSaveCallback = SelectFileToSave;
            ViewModel.ApplyViewSizeCallback = ApplySize;
            ViewModel.UpdatePreviewImageCallback = () => imageViewer.Invalidate();
            ViewModel.GetCompoundViewIconCallback = GetCompoundViewIcon;
        }

        private void InitPropertyBindings()
        {
            // VM.Notification -> lblNotification.Text
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.Notification), nameof(Label.Text), lblNotification);

            // VM.PreviewImage != null -> btnSave.Enabled
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.PreviewImage), nameof(Button.Enabled), img => img != null, btnSave);

            // VM.TitleCaption -> Text
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.TitleCaption), nameof(Text), this);

            // VM.InfoText -> txtInfo.Text
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.InfoText), nameof(TextBox.Text), txtInfo);

            // VM.AutoZoom -> btnAutoZoom.Checked, imageViewer.AutoZoom
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.AutoZoom), nameof(btnAutoZoom.Checked), btnAutoZoom);
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.AutoZoom), nameof(imageViewer.AutoZoom), imageViewer);

            // VM.Zoom <-> imageViewer.Zoom
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.Zoom), nameof(imageViewer.Zoom), imageViewer);
            CommandBindings.AddPropertyBinding(imageViewer, nameof(imageViewer.Zoom), nameof(ViewModel.Zoom), ViewModel);

            // VM.SmoothZooming -> btnAntiAlias.Checked, imageViewer.SmoothZooming
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
        }
        private void InitCommandBindings()
        {
            // ViewModel commands
            CommandBindings.Add(ViewModel.SetAutoZoomCommand, ViewModel.SetAutoZoomCommandState)
                .WithParameter(() => btnAutoZoom.Checked)
                .AddSource(btnAutoZoom, nameof(btnAutoZoom.CheckedChanged));
            CommandBindings.Add(ViewModel.SetSmoothZoomingCommand, ViewModel.SetSmoothZoomingCommandState)
                .WithParameter(() => btnAntiAlias.Checked)
                .AddSource(btnAntiAlias, nameof(btnAntiAlias.CheckedChanged));
            CommandBindings.Add(ViewModel.OpenFileCommand, ViewModel.OpenFileCommandState)
                .AddSource(btnOpen, nameof(btnOpen.Click));
            CommandBindings.Add(ViewModel.SaveFileCommand, ViewModel.SaveFileCommandState)
                .AddSource(btnSave, nameof(btnSave.Click));
            CommandBindings.Add(ViewModel.ClearCommand, ViewModel.ClearCommandState)
                .AddSource(btnClear, nameof(btnClear.Click));
            CommandBindings.Add(ViewModel.SetCompoundViewCommand, ViewModel.SetCompoundViewCommandState)
                .WithParameter(() => btnCompound.Checked)
                .AddSource(btnCompound, nameof(btnCompound.CheckedChanged));
            CommandBindings.Add(ViewModel.AdvanceAnimationCommand, ViewModel.AdvanceAnimationCommandState)
                .AddSource(timerPlayer, nameof(timerPlayer.Tick));
            CommandBindings.Add(ViewModel.PrevImageCommand, ViewModel.PrevImageCommandState)
                .AddSource(btnPrev, nameof(btnPrev.Click));
            CommandBindings.Add(ViewModel.NextImageCommand, ViewModel.NextImageCommandState)
                .AddSource(btnNext, nameof(btnNext.Click));
            CommandBindings.Add(ViewModel.ShowPaletteCommand, ViewModel.ShowPaletteCommandState)
                .AddSource(miShowPalette, nameof(miShowPalette.Click));
            CommandBindings.Add(ViewModel.ManageInstallationsCommand)
                .AddSource(btnConfiguration, nameof(btnConfiguration.Click));
            CommandBindings.Add(ViewModel.ViewImagePreviewSizeChangedCommand)
                .AddSource(imageViewer, nameof(imageViewer.SizeChanged))
                .AddSource(imageViewer, nameof(imageViewer.ZoomChanged));

            // View commands
            CommandBindings.Add(OnResizeCommand)
                .AddSource(this, nameof(Resize));
            CommandBindings.Add(OnPreviewImageResizedCommand)
                .AddSource(imageViewer, nameof(imageViewer.SizeChanged));
            CommandBindings.Add<EventArgs>(OnSetBackColorCommand)
                .AddSource(miBackColorDefault, nameof(miBackColorDefault.Click))
                .AddSource(miBackColorBlack, nameof(miBackColorBlack.Click))
                .AddSource(miBackColorWhite, nameof(miBackColorWhite.Click));
        }

        private Rectangle GetScreenRectangle() => Screen.FromHandle(Handle).WorkingArea;

        private string SelectFileToOpen()
        {
            if (dlgOpen.ShowDialog(this) != DialogResult.OK)
                return null;
            return dlgOpen.FileName;
        }

        private string SelectFileToSave()
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
            txtInfo.Height = ClientSize.Height - tsMenu.Height - splitter.Height - minHeight;
            PerformLayout();
        }

        private void ApplySize(Size size)
        {
            Size = size;
            Rectangle workingArea = GetScreenRectangle();
            if (Top < workingArea.Top)
                Top = workingArea.Top;
            if (Left < workingArea.Left)
                Left = workingArea.Left;
            if (Bottom > workingArea.Bottom)
                Top = workingArea.Bottom - Height;
            if (Right > workingArea.Right)
                Left = workingArea.Right - Width;
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
