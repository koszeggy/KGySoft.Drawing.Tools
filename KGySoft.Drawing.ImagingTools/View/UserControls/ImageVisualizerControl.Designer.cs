﻿namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    partial class ImageVisualizerControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.txtInfo = new KGySoft.Drawing.ImagingTools.View.Controls.AdvancedTextBox();
            this.miColorSpace = new System.Windows.Forms.ToolStripMenuItem();
            this.miAdjustColors = new System.Windows.Forms.ToolStripMenuItem();
            this.miBrightness = new System.Windows.Forms.ToolStripMenuItem();
            this.miContrast = new System.Windows.Forms.ToolStripMenuItem();
            this.miGamma = new System.Windows.Forms.ToolStripMenuItem();
            this.sepFrames = new System.Windows.Forms.ToolStripSeparator();
            this.btnCompound = new System.Windows.Forms.ToolStripButton();
            this.btnPrev = new System.Windows.Forms.ToolStripButton();
            this.btnNext = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.btnAbout = new System.Windows.Forms.ToolStripSplitButton();
            this.miWebSite = new System.Windows.Forms.ToolStripMenuItem();
            this.miGitHub = new System.Windows.Forms.ToolStripMenuItem();
            this.miMarketplace = new System.Windows.Forms.ToolStripMenuItem();
            this.miSubmitResources = new System.Windows.Forms.ToolStripMenuItem();
            this.miSeparatorAbout = new System.Windows.Forms.ToolStripSeparator();
            this.miAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.miEasterEgg = new System.Windows.Forms.ToolStripMenuItem();
            this.btnConfiguration = new KGySoft.Drawing.ImagingTools.View.Components.AdvancedToolStripSplitButton();
            this.miManageInstallations = new System.Windows.Forms.ToolStripMenuItem();
            this.miLanguageSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.buttons = new KGySoft.Drawing.ImagingTools.View.UserControls.OkCancelButtons();
            this.miResizeBitmap = new System.Windows.Forms.ToolStripMenuItem();
            this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
            this.dlgSave = new System.Windows.Forms.SaveFileDialog();
            this.timerPlayer = new System.Windows.Forms.Timer(this.components);
            this.imageViewer = new KGySoft.Drawing.ImagingTools.View.Controls.ImageViewer();
            this.lblNotification = new KGySoft.Drawing.ImagingTools.View.Controls.NotificationLabel();
            this.splitter = new System.Windows.Forms.Splitter();
            this.tsMenu = new KGySoft.Drawing.ImagingTools.View.Controls.AdvancedToolStrip();
            this.btnZoom = new KGySoft.Drawing.ImagingTools.View.Components.ZoomSplitButton();
            this.btnAntiAlias = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnOpen = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnClear = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnColorSettings = new KGySoft.Drawing.ImagingTools.View.Components.ScalingToolStripDropDownButton();
            this.miBackColor = new System.Windows.Forms.ToolStripMenuItem();
            this.miBackColorDefault = new System.Windows.Forms.ToolStripMenuItem();
            this.miBackColorWhite = new System.Windows.Forms.ToolStripMenuItem();
            this.miBackColorBlack = new System.Windows.Forms.ToolStripMenuItem();
            this.miShowPalette = new System.Windows.Forms.ToolStripMenuItem();
            this.miCountColors = new System.Windows.Forms.ToolStripMenuItem();
            this.btnEdit = new KGySoft.Drawing.ImagingTools.View.Components.ScalingToolStripDropDownButton();
            this.miRotateLeft = new System.Windows.Forms.ToolStripMenuItem();
            this.miRotateRight = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtInfo
            // 
            this.txtInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtInfo.Location = new System.Drawing.Point(0, 163);
            this.txtInfo.Multiline = true;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ReadOnly = true;
            this.txtInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtInfo.Size = new System.Drawing.Size(364, 118);
            this.txtInfo.TabIndex = 6;
            this.txtInfo.TabStop = false;
            this.txtInfo.WordWrap = false;
            // 
            // miColorSpace
            // 
            this.miColorSpace.Name = "miColorSpace";
            this.miColorSpace.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.miColorSpace.Size = new System.Drawing.Size(208, 22);
            this.miColorSpace.Text = "miColorSpace";
            // 
            // miAdjustColors
            // 
            this.miAdjustColors.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miBrightness,
            this.miContrast,
            this.miGamma});
            this.miAdjustColors.Name = "miAdjustColors";
            this.miAdjustColors.Size = new System.Drawing.Size(208, 22);
            this.miAdjustColors.Text = "miAdjustColors";
            // 
            // miBrightness
            // 
            this.miBrightness.Name = "miBrightness";
            this.miBrightness.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.B)));
            this.miBrightness.Size = new System.Drawing.Size(216, 22);
            this.miBrightness.Text = "miBrightness";
            // 
            // miContrast
            // 
            this.miContrast.Name = "miContrast";
            this.miContrast.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.C)));
            this.miContrast.Size = new System.Drawing.Size(216, 22);
            this.miContrast.Text = "miContrast";
            // 
            // miGamma
            // 
            this.miGamma.Name = "miGamma";
            this.miGamma.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.G)));
            this.miGamma.Size = new System.Drawing.Size(216, 22);
            this.miGamma.Text = "miGamma";
            // 
            // sepFrames
            // 
            this.sepFrames.Name = "sepFrames";
            this.sepFrames.Size = new System.Drawing.Size(6, 25);
            // 
            // btnCompound
            // 
            this.btnCompound.CheckOnClick = true;
            this.btnCompound.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCompound.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCompound.Name = "btnCompound";
            this.btnCompound.Size = new System.Drawing.Size(23, 22);
            // 
            // btnPrev
            // 
            this.btnPrev.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPrev.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.RightToLeftAutoMirrorImage = true;
            this.btnPrev.Size = new System.Drawing.Size(23, 22);
            // 
            // btnNext
            // 
            this.btnNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNext.Name = "btnNext";
            this.btnNext.RightToLeftAutoMirrorImage = true;
            this.btnNext.Size = new System.Drawing.Size(23, 22);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // btnAbout
            // 
            this.btnAbout.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnAbout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAbout.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miWebSite,
            this.miGitHub,
            this.miMarketplace,
            this.miSubmitResources,
            this.miSeparatorAbout,
            this.miAbout,
            this.miEasterEgg});
            this.btnAbout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(16, 22);
            this.btnAbout.Text = "btnAbout";
            // 
            // miWebSite
            // 
            this.miWebSite.Name = "miWebSite";
            this.miWebSite.Size = new System.Drawing.Size(179, 22);
            this.miWebSite.Text = "miWebSite";
            // 
            // miGitHub
            // 
            this.miGitHub.Name = "miGitHub";
            this.miGitHub.Size = new System.Drawing.Size(179, 22);
            this.miGitHub.Text = "miGitHub";
            // 
            // miMarketplace
            // 
            this.miMarketplace.Name = "miMarketplace";
            this.miMarketplace.Size = new System.Drawing.Size(179, 22);
            this.miMarketplace.Text = "miMarketplace";
            // 
            // miSubmitResources
            // 
            this.miSubmitResources.Name = "miSubmitResources";
            this.miSubmitResources.Size = new System.Drawing.Size(179, 22);
            this.miSubmitResources.Text = "miSubmitResources";
            // 
            // miSeparatorAbout
            // 
            this.miSeparatorAbout.Name = "miSeparatorAbout";
            this.miSeparatorAbout.Size = new System.Drawing.Size(176, 6);
            // 
            // miAbout
            // 
            this.miAbout.Name = "miAbout";
            this.miAbout.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.miAbout.Size = new System.Drawing.Size(179, 22);
            this.miAbout.Text = "miAbout";
            // 
            // miEasterEgg
            // 
            this.miEasterEgg.Name = "miEasterEgg";
            this.miEasterEgg.Size = new System.Drawing.Size(179, 22);
            this.miEasterEgg.Visible = false;
            // 
            // btnConfiguration
            // 
            this.btnConfiguration.AutoChangeDefaultItem = true;
            this.btnConfiguration.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnConfiguration.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miManageInstallations,
            this.miLanguageSettings});
            this.btnConfiguration.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnConfiguration.Name = "btnConfiguration";
            this.btnConfiguration.Size = new System.Drawing.Size(16, 22);
            // 
            // miManageInstallations
            // 
            this.miManageInstallations.Name = "miManageInstallations";
            this.miManageInstallations.Size = new System.Drawing.Size(194, 22);
            this.miManageInstallations.Text = "miManageInstallations";
            // 
            // miLanguageSettings
            // 
            this.miLanguageSettings.Name = "miLanguageSettings";
            this.miLanguageSettings.Size = new System.Drawing.Size(194, 22);
            this.miLanguageSettings.Text = "miLanguageSettings";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(205, 6);
            // 
            // buttons
            // 
            this.buttons.BackColor = System.Drawing.Color.Transparent;
            this.buttons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttons.Location = new System.Drawing.Point(0, 281);
            this.buttons.Name = "buttons";
            this.buttons.Size = new System.Drawing.Size(364, 35);
            this.buttons.TabIndex = 11;
            // 
            // miResizeBitmap
            // 
            this.miResizeBitmap.Name = "miResizeBitmap";
            this.miResizeBitmap.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.miResizeBitmap.Size = new System.Drawing.Size(208, 22);
            this.miResizeBitmap.Text = "miResizeBitmap";
            // 
            // imageViewer
            // 
            this.imageViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageViewer.Location = new System.Drawing.Point(0, 49);
            this.imageViewer.Name = "imageViewer";
            this.imageViewer.Size = new System.Drawing.Size(364, 111);
            this.imageViewer.TabIndex = 7;
            this.imageViewer.TabStop = false;
            // 
            // lblNotification
            // 
            this.lblNotification.AutoSize = true;
            this.lblNotification.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.lblNotification.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNotification.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblNotification.ForeColor = System.Drawing.Color.Black;
            this.lblNotification.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblNotification.Location = new System.Drawing.Point(0, 25);
            this.lblNotification.Name = "lblNotification";
            this.lblNotification.Padding = new System.Windows.Forms.Padding(3, 3, 20, 3);
            this.lblNotification.Size = new System.Drawing.Size(364, 24);
            this.lblNotification.TabIndex = 10;
            this.lblNotification.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // splitter
            // 
            this.splitter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter.Location = new System.Drawing.Point(0, 160);
            this.splitter.MinExtra = 16;
            this.splitter.MinSize = 50;
            this.splitter.Name = "splitter";
            this.splitter.Size = new System.Drawing.Size(364, 3);
            this.splitter.TabIndex = 9;
            this.splitter.TabStop = false;
            // 
            // tsMenu
            // 
            this.tsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnZoom,
            this.btnAntiAlias,
            this.toolStripSeparator1,
            this.btnOpen,
            this.btnSave,
            this.btnClear,
            this.toolStripSeparator2,
            this.btnColorSettings,
            this.btnEdit,
            this.sepFrames,
            this.btnCompound,
            this.btnPrev,
            this.btnNext,
            this.toolStripSeparator4,
            this.btnAbout,
            this.btnConfiguration});
            this.tsMenu.Location = new System.Drawing.Point(0, 0);
            this.tsMenu.Name = "tsMenu";
            this.tsMenu.Size = new System.Drawing.Size(364, 25);
            this.tsMenu.TabIndex = 8;
            // 
            // btnZoom
            // 
            this.btnZoom.CheckOnClick = true;
            this.btnZoom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnZoom.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoom.Name = "btnZoom";
            this.btnZoom.Size = new System.Drawing.Size(32, 22);
            // 
            // btnAntiAlias
            // 
            this.btnAntiAlias.CheckOnClick = true;
            this.btnAntiAlias.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAntiAlias.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAntiAlias.Name = "btnAntiAlias";
            this.btnAntiAlias.Size = new System.Drawing.Size(23, 22);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnOpen
            // 
            this.btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(23, 22);
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(23, 22);
            // 
            // btnClear
            // 
            this.btnClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(23, 22);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnColorSettings
            // 
            this.btnColorSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnColorSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miBackColor,
            this.miShowPalette,
            this.miCountColors});
            this.btnColorSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnColorSettings.Name = "btnColorSettings";
            this.btnColorSettings.Size = new System.Drawing.Size(13, 22);
            // 
            // miBackColor
            // 
            this.miBackColor.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miBackColorDefault,
            this.miBackColorWhite,
            this.miBackColorBlack});
            this.miBackColor.Name = "miBackColor";
            this.miBackColor.Size = new System.Drawing.Size(193, 22);
            this.miBackColor.Text = "miBackColor";
            // 
            // miBackColorDefault
            // 
            this.miBackColorDefault.Name = "miBackColorDefault";
            this.miBackColorDefault.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D1)));
            this.miBackColorDefault.Size = new System.Drawing.Size(216, 22);
            this.miBackColorDefault.Text = "miBackColorDefault";
            this.miBackColorDefault.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            // 
            // miBackColorWhite
            // 
            this.miBackColorWhite.BackColor = System.Drawing.Color.White;
            this.miBackColorWhite.ForeColor = System.Drawing.Color.Black;
            this.miBackColorWhite.Name = "miBackColorWhite";
            this.miBackColorWhite.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D2)));
            this.miBackColorWhite.Size = new System.Drawing.Size(216, 22);
            this.miBackColorWhite.Text = "miBackColorWhite";
            // 
            // miBackColorBlack
            // 
            this.miBackColorBlack.BackColor = System.Drawing.Color.Black;
            this.miBackColorBlack.ForeColor = System.Drawing.Color.White;
            this.miBackColorBlack.Name = "miBackColorBlack";
            this.miBackColorBlack.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D3)));
            this.miBackColorBlack.Size = new System.Drawing.Size(216, 22);
            this.miBackColorBlack.Text = "miBackColorBlack";
            // 
            // miShowPalette
            // 
            this.miShowPalette.Name = "miShowPalette";
            this.miShowPalette.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.P)));
            this.miShowPalette.Size = new System.Drawing.Size(193, 22);
            this.miShowPalette.Text = "miShowPalette";
            // 
            // miCountColors
            // 
            this.miCountColors.Name = "miCountColors";
            this.miCountColors.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.C)));
            this.miCountColors.Size = new System.Drawing.Size(193, 22);
            this.miCountColors.Text = "miCountColors";
            // 
            // btnEdit
            // 
            this.btnEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miRotateLeft,
            this.miRotateRight,
            this.miResizeBitmap,
            this.toolStripSeparator3,
            this.miColorSpace,
            this.miAdjustColors});
            this.btnEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(13, 22);
            // 
            // miRotateLeft
            // 
            this.miRotateLeft.Name = "miRotateLeft";
            this.miRotateLeft.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Left)));
            this.miRotateLeft.Size = new System.Drawing.Size(208, 22);
            this.miRotateLeft.Text = "miRotateLeft";
            // 
            // miRotateRight
            // 
            this.miRotateRight.Name = "miRotateRight";
            this.miRotateRight.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Right)));
            this.miRotateRight.Size = new System.Drawing.Size(208, 22);
            this.miRotateRight.Text = "miRotateRight";
            // 
            // ImageVisualizerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.imageViewer);
            this.Controls.Add(this.lblNotification);
            this.Controls.Add(this.splitter);
            this.Controls.Add(this.tsMenu);
            this.Controls.Add(this.txtInfo);
            this.Controls.Add(this.buttons);
            this.Name = "ImageVisualizerControl";
            this.Size = new System.Drawing.Size(364, 316);
            this.tsMenu.ResumeLayout(false);
            this.tsMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        protected KGySoft.Drawing.ImagingTools.View.Controls.AdvancedTextBox txtInfo;
        private System.Windows.Forms.ToolStripMenuItem miColorSpace;
        private System.Windows.Forms.ToolStripMenuItem miAdjustColors;
        private System.Windows.Forms.ToolStripMenuItem miBrightness;
        private System.Windows.Forms.ToolStripMenuItem miContrast;
        private System.Windows.Forms.ToolStripMenuItem miGamma;
        private System.Windows.Forms.ToolStripSeparator sepFrames;
        private System.Windows.Forms.ToolStripButton btnCompound;
        private System.Windows.Forms.ToolStripButton btnPrev;
        private System.Windows.Forms.ToolStripButton btnNext;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSplitButton btnAbout;
        private System.Windows.Forms.ToolStripMenuItem miWebSite;
        private System.Windows.Forms.ToolStripMenuItem miGitHub;
        private System.Windows.Forms.ToolStripMenuItem miMarketplace;
        private System.Windows.Forms.ToolStripMenuItem miSubmitResources;
        private System.Windows.Forms.ToolStripSeparator miSeparatorAbout;
        private System.Windows.Forms.ToolStripMenuItem miAbout;
        private System.Windows.Forms.ToolStripMenuItem miEasterEgg;
        protected Components.AdvancedToolStripSplitButton btnConfiguration;
        private System.Windows.Forms.ToolStripMenuItem miManageInstallations;
        private System.Windows.Forms.ToolStripMenuItem miLanguageSettings;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        protected OkCancelButtons buttons;
        private System.Windows.Forms.ToolStripMenuItem miResizeBitmap;
        private System.Windows.Forms.OpenFileDialog dlgOpen;
        private System.Windows.Forms.SaveFileDialog dlgSave;
        private System.Windows.Forms.Timer timerPlayer;
        private Controls.ImageViewer imageViewer;
        private Controls.NotificationLabel lblNotification;
        private System.Windows.Forms.Splitter splitter;
        protected Controls.AdvancedToolStrip tsMenu;
        private Components.ZoomSplitButton btnZoom;
        private System.Windows.Forms.ToolStripButton btnAntiAlias;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnOpen;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripButton btnClear;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private Components.ScalingToolStripDropDownButton btnColorSettings;
        private System.Windows.Forms.ToolStripMenuItem miBackColor;
        private System.Windows.Forms.ToolStripMenuItem miBackColorDefault;
        private System.Windows.Forms.ToolStripMenuItem miBackColorWhite;
        private System.Windows.Forms.ToolStripMenuItem miBackColorBlack;
        private System.Windows.Forms.ToolStripMenuItem miShowPalette;
        private System.Windows.Forms.ToolStripMenuItem miCountColors;
        private Components.ScalingToolStripDropDownButton btnEdit;
        private System.Windows.Forms.ToolStripMenuItem miRotateLeft;
        private System.Windows.Forms.ToolStripMenuItem miRotateRight;
    }
}
