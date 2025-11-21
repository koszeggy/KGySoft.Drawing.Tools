namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    partial class ColorSpaceControl
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
            this.cmbPixelFormat = new System.Windows.Forms.ComboBox();
            this.gbPixelFormat = new KGySoft.Drawing.ImagingTools.View.Controls.CheckGroupBox();
            this.quantizerSelector = new KGySoft.Drawing.ImagingTools.View.UserControls.QuantizerSelectorControl();
            this.gbQuantizer = new KGySoft.Drawing.ImagingTools.View.Controls.CheckGroupBox();
            this.dithererSelector = new KGySoft.Drawing.ImagingTools.View.UserControls.DithererSelectorControl();
            this.gbDitherer = new KGySoft.Drawing.ImagingTools.View.Controls.CheckGroupBox();
            this.pnlSettings.SuspendLayout();
            this.gbPixelFormat.SuspendLayout();
            this.gbQuantizer.SuspendLayout();
            this.gbDitherer.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSettings
            // 
            this.pnlSettings.Controls.Add(this.gbDitherer);
            this.pnlSettings.Controls.Add(this.gbQuantizer);
            this.pnlSettings.Controls.Add(this.gbPixelFormat);
            this.pnlSettings.Size = new System.Drawing.Size(378, 267);
            // 
            // cmbPixelFormat
            // 
            this.cmbPixelFormat.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmbPixelFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPixelFormat.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbPixelFormat.FormattingEnabled = true;
            this.cmbPixelFormat.Location = new System.Drawing.Point(3, 18);
            this.cmbPixelFormat.Name = "cmbPixelFormat";
            this.cmbPixelFormat.Size = new System.Drawing.Size(372, 21);
            this.cmbPixelFormat.TabIndex = 0;
            // 
            // gbPixelFormat
            // 
            this.gbPixelFormat.Controls.Add(this.cmbPixelFormat);
            this.gbPixelFormat.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbPixelFormat.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.gbPixelFormat.Location = new System.Drawing.Point(0, 0);
            this.gbPixelFormat.Name = "gbPixelFormat";
            this.gbPixelFormat.Size = new System.Drawing.Size(378, 43);
            this.gbPixelFormat.TabIndex = 0;
            this.gbPixelFormat.TabStop = false;
            this.gbPixelFormat.Text = "gbPixelFormat";
            // 
            // quantizerSelector
            // 
            this.quantizerSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.quantizerSelector.Location = new System.Drawing.Point(3, 18);
            this.quantizerSelector.Name = "quantizerSelector";
            this.quantizerSelector.Size = new System.Drawing.Size(372, 91);
            this.quantizerSelector.TabIndex = 0;
            // 
            // gbQuantizer
            // 
            this.gbQuantizer.Controls.Add(this.quantizerSelector);
            this.gbQuantizer.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbQuantizer.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.gbQuantizer.Location = new System.Drawing.Point(0, 43);
            this.gbQuantizer.Name = "gbQuantizer";
            this.gbQuantizer.Size = new System.Drawing.Size(378, 112);
            this.gbQuantizer.TabIndex = 1;
            this.gbQuantizer.TabStop = false;
            this.gbQuantizer.Text = "gbQuantizer";
            // 
            // dithererSelector
            // 
            this.dithererSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dithererSelector.Location = new System.Drawing.Point(3, 18);
            this.dithererSelector.Name = "dithererSelector";
            this.dithererSelector.Size = new System.Drawing.Size(372, 91);
            this.dithererSelector.TabIndex = 0;
            // 
            // gbDitherer
            // 
            this.gbDitherer.Controls.Add(this.dithererSelector);
            this.gbDitherer.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbDitherer.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.gbDitherer.Location = new System.Drawing.Point(0, 155);
            this.gbDitherer.Name = "gbDitherer";
            this.gbDitherer.Size = new System.Drawing.Size(378, 112);
            this.gbDitherer.TabIndex = 2;
            this.gbDitherer.TabStop = false;
            this.gbDitherer.Text = "gbDitherer";
            // 
            // ColorSpaceControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Size = new System.Drawing.Size(384, 421);
            this.Name = "ColorSpaceControl";
            this.pnlSettings.ResumeLayout(false);
            this.gbPixelFormat.ResumeLayout(false);
            this.gbPixelFormat.PerformLayout();
            this.gbQuantizer.ResumeLayout(false);
            this.gbQuantizer.PerformLayout();
            this.gbDitherer.ResumeLayout(false);
            this.gbDitherer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.CheckGroupBox gbDitherer;
        private UserControls.DithererSelectorControl dithererSelector;
        private Controls.CheckGroupBox gbQuantizer;
        private UserControls.QuantizerSelectorControl quantizerSelector;
        private Controls.CheckGroupBox gbPixelFormat;
        private System.Windows.Forms.ComboBox cmbPixelFormat;
    }
}