namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    partial class DithererStrengthEditorControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.trackBar = new System.Windows.Forms.TrackBar();
            this.okCancelButtons = new KGySoft.Drawing.ImagingTools.View.UserControls.OkCancelButtons();
            this.lblValue = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBar
            // 
            this.trackBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBar.Location = new System.Drawing.Point(35, 0);
            this.trackBar.Maximum = 100;
            this.trackBar.Name = "trackBar";
            this.trackBar.RightToLeftLayout = true;
            this.trackBar.Size = new System.Drawing.Size(147, 27);
            this.trackBar.TabIndex = 0;
            this.trackBar.TickFrequency = 10;
            // 
            // okCancelButtons
            // 
            this.okCancelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.okCancelButtons.Location = new System.Drawing.Point(0, 27);
            this.okCancelButtons.Name = "okCancelButtons";
            this.okCancelButtons.Size = new System.Drawing.Size(182, 35);
            this.okCancelButtons.TabIndex = 1;
            // 
            // lblValue
            // 
            this.lblValue.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblValue.Location = new System.Drawing.Point(0, 0);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(35, 27);
            this.lblValue.TabIndex = 2;
            this.lblValue.Text = "lblValue";
            this.lblValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // DithererStrengthEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.trackBar);
            this.Controls.Add(this.lblValue);
            this.Controls.Add(this.okCancelButtons);
            this.Name = "DithererStrengthEditorControl";
            this.Size = new System.Drawing.Size(182, 67);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar trackBar;
        private OkCancelButtons okCancelButtons;
        private System.Windows.Forms.Label lblValue;
    }
}
