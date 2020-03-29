﻿using System.Windows.Forms;
using KGySoft.Drawing.ImagingTools.View.Controls;

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    partial class ImageVisualizerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.tsMenu = new KGySoft.Drawing.ImagingTools.View.Controls.ScalingToolStrip();
            this.btnAutoZoom = new System.Windows.Forms.ToolStripButton();
            this.btnColorSettings = new KGySoft.Drawing.ImagingTools.View.Controls.ScalingToolStripDropDownButton();
            this.miBackColor = new System.Windows.Forms.ToolStripMenuItem();
            this.miBackColorDefault = new System.Windows.Forms.ToolStripMenuItem();
            this.miBackColorWhite = new System.Windows.Forms.ToolStripMenuItem();
            this.miBackColorBlack = new System.Windows.Forms.ToolStripMenuItem();
            this.miShowPalette = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnOpen = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnClear = new System.Windows.Forms.ToolStripButton();
            this.sepFrames = new System.Windows.Forms.ToolStripSeparator();
            this.btnCompound = new System.Windows.Forms.ToolStripButton();
            this.btnPrev = new System.Windows.Forms.ToolStripButton();
            this.btnNext = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnConfiguration = new System.Windows.Forms.ToolStripButton();
            this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
            this.dlgSave = new System.Windows.Forms.SaveFileDialog();
            this.splitter = new System.Windows.Forms.Splitter();
            this.timerPlayer = new System.Windows.Forms.Timer(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.lblNotification = new KGySoft.Drawing.ImagingTools.View.Controls.NotificationLabel();
            this.pbImage = new System.Windows.Forms.PictureBox();
            this.tsMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).BeginInit();
            this.SuspendLayout();
            // 
            // txtInfo
            // 
            this.txtInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtInfo.Location = new System.Drawing.Point(0, 193);
            this.txtInfo.Multiline = true;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ReadOnly = true;
            this.txtInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtInfo.Size = new System.Drawing.Size(334, 123);
            this.txtInfo.TabIndex = 0;
            this.txtInfo.TabStop = false;
            this.txtInfo.WordWrap = false;
            // 
            // tsMenu
            // 
            this.tsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAutoZoom,
            this.btnColorSettings,
            this.toolStripSeparator1,
            this.btnOpen,
            this.btnSave,
            this.btnClear,
            this.sepFrames,
            this.btnCompound,
            this.btnPrev,
            this.btnNext,
            this.toolStripSeparator2,
            this.btnConfiguration});
            this.tsMenu.Location = new System.Drawing.Point(0, 0);
            this.tsMenu.Name = "tsMenu";
            this.tsMenu.Size = new System.Drawing.Size(334, 25);
            this.tsMenu.TabIndex = 2;
            // 
            // btnAutoZoom
            // 
            this.btnAutoZoom.CheckOnClick = true;
            this.btnAutoZoom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAutoZoom.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAutoZoom.Name = "btnAutoZoom";
            this.btnAutoZoom.Size = new System.Drawing.Size(23, 22);
            // 
            // btnColorSettings
            // 
            this.btnColorSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnColorSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miBackColor,
            this.miShowPalette});
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
            this.miBackColor.Size = new System.Drawing.Size(67, 22);
            // 
            // miDefault
            // 
            this.miBackColorDefault.Name = "miBackColorDefault";
            this.miBackColorDefault.Size = new System.Drawing.Size(67, 22);
            this.miBackColorDefault.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            // 
            // miWhite
            // 
            this.miBackColorWhite.BackColor = System.Drawing.Color.White;
            this.miBackColorWhite.ForeColor = System.Drawing.Color.Black;
            this.miBackColorWhite.Name = "miBackColorWhite";
            this.miBackColorWhite.Size = new System.Drawing.Size(67, 22);
            // 
            // miBlack
            // 
            this.miBackColorBlack.BackColor = System.Drawing.Color.Black;
            this.miBackColorBlack.ForeColor = System.Drawing.Color.White;
            this.miBackColorBlack.Name = "miBackColorBlack";
            this.miBackColorBlack.Size = new System.Drawing.Size(67, 22);
            // 
            // miShowPalette
            // 
            this.miShowPalette.Name = "miShowPalette";
            this.miShowPalette.Size = new System.Drawing.Size(67, 22);
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
            // sepFrames
            // 
            this.sepFrames.Name = "sepFrames";
            this.sepFrames.Size = new System.Drawing.Size(6, 25);
            // 
            // btnCompound
            // 
            this.btnCompound.Checked = true;
            this.btnCompound.CheckOnClick = true;
            this.btnCompound.CheckState = System.Windows.Forms.CheckState.Checked;
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
            this.btnPrev.Size = new System.Drawing.Size(23, 22);
            // 
            // btnNext
            // 
            this.btnNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(23, 22);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnConfiguration
            // 
            this.btnConfiguration.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnConfiguration.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnConfiguration.Name = "btnConfiguration";
            this.btnConfiguration.Size = new System.Drawing.Size(23, 22);
            // 
            // splitter
            // 
            this.splitter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter.Location = new System.Drawing.Point(0, 190);
            this.splitter.MinExtra = 16;
            this.splitter.Name = "splitter";
            this.splitter.Size = new System.Drawing.Size(334, 3);
            this.splitter.TabIndex = 3;
            this.splitter.TabStop = false;
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
            this.lblNotification.Size = new System.Drawing.Size(334, 24);
            this.lblNotification.TabIndex = 4;
            this.lblNotification.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pbImage
            // 
            this.pbImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbImage.Location = new System.Drawing.Point(0, 49);
            this.pbImage.Name = "pbImage";
            this.pbImage.Size = new System.Drawing.Size(334, 141);
            this.pbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbImage.TabIndex = 1;
            this.pbImage.TabStop = false;
            // 
            // ImageVisualizerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 316);
            this.Controls.Add(this.pbImage);
            this.Controls.Add(this.lblNotification);
            this.Controls.Add(this.splitter);
            this.Controls.Add(this.tsMenu);
            this.Controls.Add(this.txtInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(200, 200);
            this.Name = "ImageVisualizerForm";
            this.tsMenu.ResumeLayout(false);
            this.tsMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbImage;
        private System.Windows.Forms.ToolStripButton btnAutoZoom;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripButton btnOpen;
        private System.Windows.Forms.ToolStripButton btnClear;
        private System.Windows.Forms.ToolStripSeparator sepFrames;
        private System.Windows.Forms.ToolStripButton btnPrev;
        private System.Windows.Forms.ToolStripButton btnNext;
        private System.Windows.Forms.ToolStripButton btnCompound;
        private System.Windows.Forms.OpenFileDialog dlgOpen;
        private System.Windows.Forms.SaveFileDialog dlgSave;
        private System.Windows.Forms.Splitter splitter;
        private System.Windows.Forms.Timer timerPlayer;
        private ScalingToolStripDropDownButton btnColorSettings;
        private System.Windows.Forms.ToolStripMenuItem miBackColor;
        private System.Windows.Forms.ToolStripMenuItem miBackColorDefault;
        private System.Windows.Forms.ToolStripMenuItem miBackColorWhite;
        private System.Windows.Forms.ToolStripMenuItem miBackColorBlack;
        private System.Windows.Forms.ToolStripMenuItem miShowPalette;
        protected ScalingToolStrip tsMenu;
        protected System.Windows.Forms.TextBox txtInfo;
        private NotificationLabel lblNotification;
        private ToolTip toolTip;
        private ToolStripSeparator toolStripSeparator2;
        protected ToolStripButton btnConfiguration;
    }
}