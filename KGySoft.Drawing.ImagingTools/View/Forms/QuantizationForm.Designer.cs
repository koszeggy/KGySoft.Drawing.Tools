namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    partial class QuantizationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.quantizerSelectorControl1 = new KGySoft.Drawing.ImagingTools.View.UserControls.QuantizerSelectorControl();
            this.dithererSelectorControl1 = new KGySoft.Drawing.ImagingTools.View.UserControls.DithererSelectorControl();
            this.SuspendLayout();
            // 
            // quantizerSelectorControl1
            // 
            this.quantizerSelectorControl1.Location = new System.Drawing.Point(36, 19);
            this.quantizerSelectorControl1.Name = "quantizerSelectorControl1";
            this.quantizerSelectorControl1.Size = new System.Drawing.Size(246, 220);
            this.quantizerSelectorControl1.TabIndex = 0;
            // 
            // dithererSelectorControl1
            // 
            this.dithererSelectorControl1.Location = new System.Drawing.Point(344, 24);
            this.dithererSelectorControl1.Name = "dithererSelectorControl1";
            this.dithererSelectorControl1.Size = new System.Drawing.Size(314, 215);
            this.dithererSelectorControl1.TabIndex = 1;
            // 
            // QuantizationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 390);
            this.Controls.Add(this.dithererSelectorControl1);
            this.Controls.Add(this.quantizerSelectorControl1);
            this.Name = "QuantizationForm";
            this.Text = "QuantizationForm";
            this.ResumeLayout(false);

        }

        #endregion

        private UserControls.QuantizerSelectorControl quantizerSelectorControl1;
        private UserControls.DithererSelectorControl dithererSelectorControl1;
    }
}