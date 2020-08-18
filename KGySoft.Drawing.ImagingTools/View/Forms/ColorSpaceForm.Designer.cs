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
            this.quantizerSelector = new KGySoft.Drawing.ImagingTools.View.UserControls.QuantizerSelectorControl();
            this.dithererSelector = new KGySoft.Drawing.ImagingTools.View.UserControls.DithererSelectorControl();
            this.gbPixelFormat = new KGySoft.Drawing.ImagingTools.View.UserControls.CheckGroupBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.gbQuantizer = new KGySoft.Drawing.ImagingTools.View.UserControls.CheckGroupBox();
            this.gbDitherer = new KGySoft.Drawing.ImagingTools.View.UserControls.CheckGroupBox();
            this.okCancelButtons = new KGySoft.Drawing.ImagingTools.View.UserControls.OkCancelButtons();
            this.previewImage = new KGySoft.Drawing.ImagingTools.View.UserControls.PreviewImageControl();
            this.gbPixelFormat.GroupBox.SuspendLayout();
            this.gbPixelFormat.SuspendLayout();
            this.gbQuantizer.GroupBox.SuspendLayout();
            this.gbQuantizer.SuspendLayout();
            this.gbDitherer.GroupBox.SuspendLayout();
            this.gbDitherer.SuspendLayout();
            this.SuspendLayout();
            // 
            // quantizerSelectorControl1
            // 
            this.quantizerSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.quantizerSelector.Location = new System.Drawing.Point(3, 17);
            this.quantizerSelector.Name = "quantizerSelector";
            this.quantizerSelector.Size = new System.Drawing.Size(460, 92);
            this.quantizerSelector.TabIndex = 0;
            // 
            // dithererSelectorControl1
            // 
            this.dithererSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dithererSelector.Location = new System.Drawing.Point(3, 17);
            this.dithererSelector.Name = "dithererSelector";
            this.dithererSelector.Size = new System.Drawing.Size(460, 92);
            this.dithererSelector.TabIndex = 1;
            // 
            // gbPixelFormat
            // 
            this.gbPixelFormat.Dock = System.Windows.Forms.DockStyle.Top;
            // 
            // gbPixelFormat.GroupBox
            // 
            this.gbPixelFormat.GroupBox.Controls.Add(this.comboBox1);
            this.gbPixelFormat.GroupBox.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.gbPixelFormat.Location = new System.Drawing.Point(3, 3);
            this.gbPixelFormat.Name = "gbPixelFormat";
            this.gbPixelFormat.Size = new System.Drawing.Size(466, 43);
            this.gbPixelFormat.TabIndex = 3;
            this.gbPixelFormat.Text = "gbPixelFormat";
            // 
            // comboBox1
            // 
            this.comboBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(3, 17);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(460, 21);
            this.comboBox1.TabIndex = 0;
            // 
            // gbQuantizer
            // 
            this.gbQuantizer.Dock = System.Windows.Forms.DockStyle.Top;
            // 
            // gbQuantizer.GroupBox
            // 
            this.gbQuantizer.GroupBox.Controls.Add(this.quantizerSelector);
            this.gbQuantizer.GroupBox.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.gbQuantizer.Location = new System.Drawing.Point(3, 46);
            this.gbQuantizer.Name = "gbQuantizer";
            this.gbQuantizer.Size = new System.Drawing.Size(466, 112);
            this.gbQuantizer.TabIndex = 4;
            this.gbQuantizer.Text = "gbQuantizer";
            // 
            // gbDitherer
            // 
            this.gbDitherer.Dock = System.Windows.Forms.DockStyle.Top;
            // 
            // gbDitherer.GroupBox
            // 
            this.gbDitherer.GroupBox.Controls.Add(this.dithererSelector);
            this.gbDitherer.GroupBox.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.gbDitherer.Location = new System.Drawing.Point(3, 158);
            this.gbDitherer.Name = "gbDitherer";
            this.gbDitherer.Size = new System.Drawing.Size(466, 112);
            this.gbDitherer.TabIndex = 5;
            this.gbDitherer.Text = "gbDitherer";
            // 
            // okCancelButtons
            // 
            this.okCancelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.okCancelButtons.Location = new System.Drawing.Point(3, 380);
            this.okCancelButtons.Name = "okCancelButtons";
            this.okCancelButtons.Size = new System.Drawing.Size(466, 31);
            this.okCancelButtons.TabIndex = 6;
            // 
            // previewBitmapControl1
            // 
            this.previewImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewImage.Location = new System.Drawing.Point(3, 270);
            this.previewImage.Name = "previewImage";
            this.previewImage.Size = new System.Drawing.Size(466, 110);
            this.previewImage.TabIndex = 7;
            // 
            // ColorSpaceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 411);
            this.Controls.Add(this.previewImage);
            this.Controls.Add(this.okCancelButtons);
            this.Controls.Add(this.gbDitherer);
            this.Controls.Add(this.gbQuantizer);
            this.Controls.Add(this.gbPixelFormat);
            this.MinimizeBox = false;
            this.Name = "ColorSpaceForm";
            this.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.Text = "ColorSpaceForm";
            this.gbPixelFormat.GroupBox.ResumeLayout(false);
            this.gbPixelFormat.ResumeLayout(false);
            this.gbPixelFormat.PerformLayout();
            this.gbQuantizer.GroupBox.ResumeLayout(false);
            this.gbQuantizer.ResumeLayout(false);
            this.gbQuantizer.PerformLayout();
            this.gbDitherer.GroupBox.ResumeLayout(false);
            this.gbDitherer.ResumeLayout(false);
            this.gbDitherer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private UserControls.QuantizerSelectorControl quantizerSelector;
        private UserControls.DithererSelectorControl dithererSelector;
        private UserControls.CheckGroupBox gbPixelFormat;
        private System.Windows.Forms.ComboBox comboBox1;
        private UserControls.CheckGroupBox gbQuantizer;
        private UserControls.CheckGroupBox gbDitherer;
        private UserControls.OkCancelButtons okCancelButtons;
        private UserControls.PreviewImageControl previewImage;
    }
}