namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    partial class CountColorsForm
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
            this.lblCountColorsStatus = new System.Windows.Forms.Label();
            this.pnlButton = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.progress = new KGySoft.Drawing.ImagingTools.View.Controls.DrawingProgressStatusStrip();
            this.pnlButton.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblCountColorsStatus
            // 
            this.lblCountColorsStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCountColorsStatus.Location = new System.Drawing.Point(0, 0);
            this.lblCountColorsStatus.Name = "lblCountColorsStatus";
            this.lblCountColorsStatus.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.lblCountColorsStatus.Size = new System.Drawing.Size(274, 39);
            this.lblCountColorsStatus.TabIndex = 0;
            this.lblCountColorsStatus.Text = "lblCountColorsStatus";
            this.lblCountColorsStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlButton
            // 
            this.pnlButton.Controls.Add(this.btnClose);
            this.pnlButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButton.Location = new System.Drawing.Point(0, 39);
            this.pnlButton.Name = "pnlButton";
            this.pnlButton.Size = new System.Drawing.Size(274, 29);
            this.pnlButton.TabIndex = 1;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnClose.Location = new System.Drawing.Point(100, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "btnClose";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // progress
            // 
            this.progress.BackColor = System.Drawing.Color.Transparent;
            this.progress.Location = new System.Drawing.Point(0, 68);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(274, 22);
            this.progress.TabIndex = 2;
            this.progress.Text = "drawingProgressStatusStrip1";
            // 
            // CountColorsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(274, 90);
            this.Controls.Add(this.lblCountColorsStatus);
            this.Controls.Add(this.pnlButton);
            this.Controls.Add(this.progress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CountColorsForm";
            this.Text = "CountColorsForm";
            this.pnlButton.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.DrawingProgressStatusStrip progress;
        private System.Windows.Forms.Label lblCountColorsStatus;
        private System.Windows.Forms.Panel pnlButton;
        private System.Windows.Forms.Button btnClose;
    }
}