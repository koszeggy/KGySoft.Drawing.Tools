namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    partial class TransformBitmapFormBase
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
            this.progress = new KGySoft.Drawing.ImagingTools.View.Controls.DrawingProgressStatusStrip();
            this.okCancelButtons = new KGySoft.Drawing.ImagingTools.View.UserControls.OkCancelButtons();
            this.previewImage = new KGySoft.Drawing.ImagingTools.View.UserControls.PreviewImageControl();
            this.pnlSettings = new System.Windows.Forms.Panel();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.warningProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.infoProvider = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.warningProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.infoProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // progress
            // 
            this.progress.BackColor = System.Drawing.Color.Transparent;
            this.progress.Location = new System.Drawing.Point(0, 189);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(248, 22);
            this.progress.SizingGrip = false;
            this.progress.TabIndex = 3;
            this.progress.Text = "drawingProgressStatusStrip1";
            // 
            // okCancelButtons
            // 
            this.okCancelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.okCancelButtons.Location = new System.Drawing.Point(0, 149);
            this.okCancelButtons.Name = "okCancelButtons";
            this.okCancelButtons.Size = new System.Drawing.Size(248, 40);
            this.okCancelButtons.TabIndex = 2;
            // 
            // previewImage
            // 
            this.previewImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewImage.Location = new System.Drawing.Point(0, 56);
            this.previewImage.Name = "previewImage";
            this.previewImage.Size = new System.Drawing.Size(248, 93);
            this.previewImage.TabIndex = 1;
            // 
            // pnlSettings
            // 
            this.pnlSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSettings.Location = new System.Drawing.Point(0, 0);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Size = new System.Drawing.Size(248, 56);
            this.pnlSettings.TabIndex = 0;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // warningProvider
            // 
            this.warningProvider.ContainerControl = this;
            // 
            // infoProvider
            // 
            this.infoProvider.ContainerControl = this;
            // 
            // TransformBitmapFormBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(248, 211);
            this.Controls.Add(this.previewImage);
            this.Controls.Add(this.pnlSettings);
            this.Controls.Add(this.okCancelButtons);
            this.Controls.Add(this.progress);
            this.MinimizeBox = false;
            this.Name = "TransformBitmapFormBase";
            this.Text = "TransformBitmapFormBase";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.warningProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.infoProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.DrawingProgressStatusStrip progress;
        private UserControls.OkCancelButtons okCancelButtons;
        private UserControls.PreviewImageControl previewImage;
        protected System.Windows.Forms.Panel pnlSettings;
        private System.Windows.Forms.ErrorProvider warningProvider;
        private System.Windows.Forms.ErrorProvider infoProvider;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}