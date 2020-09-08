namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    partial class ColorSpaceForm
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
            this.previewImage = new KGySoft.Drawing.ImagingTools.View.UserControls.PreviewImageControl();
            this.okCancelButtons = new KGySoft.Drawing.ImagingTools.View.UserControls.OkCancelButtons();
            this.gbDitherer = new KGySoft.Drawing.ImagingTools.View.Controls.CheckGroupBox();
            this.dithererSelector = new KGySoft.Drawing.ImagingTools.View.UserControls.DithererSelectorControl();
            this.gbQuantizer = new KGySoft.Drawing.ImagingTools.View.Controls.CheckGroupBox();
            this.quantizerSelector = new KGySoft.Drawing.ImagingTools.View.UserControls.QuantizerSelectorControl();
            this.gbPixelFormat = new KGySoft.Drawing.ImagingTools.View.Controls.CheckGroupBox();
            this.cmbPixelFormat = new System.Windows.Forms.ComboBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.warningProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.infoProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.progress = new KGySoft.Drawing.ImagingTools.View.Controls.DrawingProgressStatusStrip();
            this.gbDitherer.SuspendLayout();
            this.gbQuantizer.SuspendLayout();
            this.gbPixelFormat.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.warningProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.infoProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // previewImage
            // 
            this.previewImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewImage.Location = new System.Drawing.Point(3, 270);
            this.previewImage.Name = "previewImage";
            this.previewImage.Size = new System.Drawing.Size(428, 88);
            this.previewImage.TabIndex = 7;
            // 
            // okCancelButtons
            // 
            this.okCancelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.okCancelButtons.Location = new System.Drawing.Point(3, 358);
            this.okCancelButtons.Name = "okCancelButtons";
            this.okCancelButtons.Size = new System.Drawing.Size(428, 31);
            this.okCancelButtons.TabIndex = 6;
            // 
            // gbDitherer
            // 
            this.gbDitherer.Controls.Add(this.dithererSelector);
            this.gbDitherer.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbDitherer.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.gbDitherer.Location = new System.Drawing.Point(3, 158);
            this.gbDitherer.Name = "gbDitherer";
            this.gbDitherer.Size = new System.Drawing.Size(428, 112);
            this.gbDitherer.TabIndex = 5;
            this.gbDitherer.TabStop = false;
            this.gbDitherer.Text = "gbDitherer";
            // 
            // dithererSelector
            // 
            this.dithererSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dithererSelector.Location = new System.Drawing.Point(3, 18);
            this.dithererSelector.Name = "dithererSelector";
            this.dithererSelector.Size = new System.Drawing.Size(422, 91);
            this.dithererSelector.TabIndex = 1;
            // 
            // gbQuantizer
            // 
            this.gbQuantizer.Controls.Add(this.quantizerSelector);
            this.gbQuantizer.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbQuantizer.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.gbQuantizer.Location = new System.Drawing.Point(3, 46);
            this.gbQuantizer.Name = "gbQuantizer";
            this.gbQuantizer.Size = new System.Drawing.Size(428, 112);
            this.gbQuantizer.TabIndex = 4;
            this.gbQuantizer.TabStop = false;
            this.gbQuantizer.Text = "gbQuantizer";
            // 
            // quantizerSelector
            // 
            this.quantizerSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.quantizerSelector.Location = new System.Drawing.Point(3, 18);
            this.quantizerSelector.Name = "quantizerSelector";
            this.quantizerSelector.Size = new System.Drawing.Size(422, 91);
            this.quantizerSelector.TabIndex = 0;
            // 
            // gbPixelFormat
            // 
            this.gbPixelFormat.Controls.Add(this.cmbPixelFormat);
            this.gbPixelFormat.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbPixelFormat.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.gbPixelFormat.Location = new System.Drawing.Point(3, 3);
            this.gbPixelFormat.Name = "gbPixelFormat";
            this.gbPixelFormat.Size = new System.Drawing.Size(428, 43);
            this.gbPixelFormat.TabIndex = 3;
            this.gbPixelFormat.TabStop = false;
            this.gbPixelFormat.Text = "gbPixelFormat";
            // 
            // cmbPixelFormat
            // 
            this.cmbPixelFormat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbPixelFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPixelFormat.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbPixelFormat.FormattingEnabled = true;
            this.cmbPixelFormat.Location = new System.Drawing.Point(3, 18);
            this.cmbPixelFormat.Name = "cmbPixelFormat";
            this.cmbPixelFormat.Size = new System.Drawing.Size(422, 21);
            this.cmbPixelFormat.TabIndex = 0;
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
            // progress
            // 
            this.progress.BackColor = System.Drawing.Color.Transparent;
            this.progress.Location = new System.Drawing.Point(3, 389);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(428, 22);
            this.progress.SizingGrip = false;
            this.progress.TabIndex = 8;
            this.progress.Text = "drawingProgressStatusStrip1";
            // 
            // ColorSpaceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 411);
            this.Controls.Add(this.previewImage);
            this.Controls.Add(this.okCancelButtons);
            this.Controls.Add(this.gbDitherer);
            this.Controls.Add(this.gbQuantizer);
            this.Controls.Add(this.gbPixelFormat);
            this.Controls.Add(this.progress);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(450, 450);
            this.Name = "ColorSpaceForm";
            this.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.Text = "ColorSpaceForm";
            this.gbDitherer.ResumeLayout(false);
            this.gbDitherer.PerformLayout();
            this.gbQuantizer.ResumeLayout(false);
            this.gbQuantizer.PerformLayout();
            this.gbPixelFormat.ResumeLayout(false);
            this.gbPixelFormat.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.warningProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.infoProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UserControls.QuantizerSelectorControl quantizerSelector;
        private UserControls.DithererSelectorControl dithererSelector;
        private Controls.CheckGroupBox gbPixelFormat;
        private System.Windows.Forms.ComboBox cmbPixelFormat;
        private Controls.CheckGroupBox gbQuantizer;
        private Controls.CheckGroupBox gbDitherer;
        private UserControls.OkCancelButtons okCancelButtons;
        private UserControls.PreviewImageControl previewImage;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.ErrorProvider infoProvider;
        private System.Windows.Forms.ErrorProvider warningProvider;
        private Controls.DrawingProgressStatusStrip progress;
    }
}