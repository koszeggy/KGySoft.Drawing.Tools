﻿namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    partial class TransformBitmapControlBase
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
            this.progress = new KGySoft.Drawing.ImagingTools.View.Controls.DrawingProgressFooter();
            this.okCancelButtons = new KGySoft.Drawing.ImagingTools.View.UserControls.OkCancelButtons();
            this.previewImage = new KGySoft.Drawing.ImagingTools.View.UserControls.PreviewImageControl();
            this.pnlSettings = new KGySoft.Drawing.ImagingTools.View.Controls.AutoMirrorPanel();
            this.SuspendLayout();
            // 
            // progress
            // 
            this.progress.BackColor = System.Drawing.Color.Transparent;
            this.progress.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progress.Location = new System.Drawing.Point(3, 189);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(242, 22);
            this.progress.TabIndex = 3;
            // 
            // okCancelButtons
            // 
            this.okCancelButtons.BackColor = System.Drawing.Color.Transparent;
            this.okCancelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.okCancelButtons.Location = new System.Drawing.Point(3, 154);
            this.okCancelButtons.Name = "okCancelButtons";
            this.okCancelButtons.Size = new System.Drawing.Size(242, 35);
            this.okCancelButtons.TabIndex = 2;
            // 
            // previewImage
            // 
            this.previewImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewImage.Location = new System.Drawing.Point(3, 56);
            this.previewImage.Name = "previewImage";
            this.previewImage.Size = new System.Drawing.Size(242, 98);
            this.previewImage.TabIndex = 1;
            // 
            // pnlSettings
            // 
            this.pnlSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSettings.Location = new System.Drawing.Point(3, 0);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Size = new System.Drawing.Size(242, 56);
            this.pnlSettings.TabIndex = 0;
            // 
            // TransformBitmapControlBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Size = new System.Drawing.Size(248, 211);
            this.Controls.Add(this.previewImage);
            this.Controls.Add(this.pnlSettings);
            this.Controls.Add(this.okCancelButtons);
            this.Controls.Add(this.progress);
            this.Name = "TransformBitmapControlBase";
            this.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.ResumeLayout(false);
        }

        #endregion

        private Controls.DrawingProgressFooter progress;
        private UserControls.OkCancelButtons okCancelButtons;
        private UserControls.PreviewImageControl previewImage;
        protected Controls.AutoMirrorPanel pnlSettings;
    }
}