namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal partial class AdjustColorsControlBase
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
            this.trackBar = new System.Windows.Forms.TrackBar();
            this.btnReset = new KGySoft.WinForms.Controls.AdvancedButton();
            this.lblValue = new KGySoft.WinForms.Controls.AdvancedLabel();
            this.pnlCheckBoxes = new System.Windows.Forms.TableLayoutPanel();
            this.chbBlue = new KGySoft.WinForms.Controls.AdvancedCheckBox();
            this.chbGreen = new KGySoft.WinForms.Controls.AdvancedCheckBox();
            this.chbRed = new KGySoft.WinForms.Controls.AdvancedCheckBox();
            this.pnlSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
            this.pnlCheckBoxes.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSettings
            // 
            this.pnlSettings.Controls.Add(this.trackBar);
            this.pnlSettings.Controls.Add(this.btnReset);
            this.pnlSettings.Controls.Add(this.lblValue);
            this.pnlSettings.Controls.Add(this.pnlCheckBoxes);
            // 
            // trackBar
            // 
            this.trackBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBar.Location = new System.Drawing.Point(35, 25);
            this.trackBar.Name = "trackBar";
            this.trackBar.RightToLeftLayout = true;
            this.trackBar.Size = new System.Drawing.Size(149, 31);
            this.trackBar.TabIndex = 2;
            // 
            // btnReset
            // 
            this.btnReset.AutoSize = true;
            this.btnReset.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnReset.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnReset.Location = new System.Drawing.Point(184, 25);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(58, 31);
            this.btnReset.TabIndex = 3;
            this.btnReset.Text = "btnReset";
            this.btnReset.UseVisualStyleBackColor = true;
            // 
            // lblValue
            // 
            this.lblValue.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblValue.Location = new System.Drawing.Point(0, 25);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(35, 31);
            this.lblValue.TabIndex = 1;
            this.lblValue.Text = "lblValue";
            this.lblValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlCheckBoxes
            // 
            this.pnlCheckBoxes.ColumnCount = 3;
            this.pnlCheckBoxes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.pnlCheckBoxes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.pnlCheckBoxes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.pnlCheckBoxes.Controls.Add(this.chbBlue, 2, 0);
            this.pnlCheckBoxes.Controls.Add(this.chbGreen, 1, 0);
            this.pnlCheckBoxes.Controls.Add(this.chbRed, 0, 0);
            this.pnlCheckBoxes.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlCheckBoxes.Location = new System.Drawing.Point(0, 0);
            this.pnlCheckBoxes.Name = "pnlCheckBoxes";
            this.pnlCheckBoxes.RowCount = 1;
            this.pnlCheckBoxes.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlCheckBoxes.Size = new System.Drawing.Size(242, 25);
            this.pnlCheckBoxes.TabIndex = 0;
            // 
            // chbBlue
            // 
            this.chbBlue.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chbBlue.AutoSize = true;
            this.chbBlue.Location = new System.Drawing.Point(168, 4);
            this.chbBlue.Name = "chbBlue";
            this.chbBlue.Size = new System.Drawing.Size(65, 17);
            this.chbBlue.TabIndex = 2;
            this.chbBlue.Text = "chbBlue";
            this.chbBlue.UseVisualStyleBackColor = true;
            // 
            // chbGreen
            // 
            this.chbGreen.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chbGreen.AutoSize = true;
            this.chbGreen.Location = new System.Drawing.Point(83, 4);
            this.chbGreen.Name = "chbGreen";
            this.chbGreen.Size = new System.Drawing.Size(73, 17);
            this.chbGreen.TabIndex = 1;
            this.chbGreen.Text = "chbGreen";
            this.chbGreen.UseVisualStyleBackColor = true;
            // 
            // chbRed
            // 
            this.chbRed.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chbRed.AutoSize = true;
            this.chbRed.Location = new System.Drawing.Point(8, 4);
            this.chbRed.Name = "chbRed";
            this.chbRed.Size = new System.Drawing.Size(64, 17);
            this.chbRed.TabIndex = 0;
            this.chbRed.Text = "chbRed";
            this.chbRed.UseVisualStyleBackColor = true;
            // 
            // AdjustColorsControlBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "AdjustColorsControlBase";
            this.pnlSettings.ResumeLayout(false);
            this.pnlSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
            this.pnlCheckBoxes.ResumeLayout(false);
            this.pnlCheckBoxes.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TrackBar trackBar;
        private KGySoft.WinForms.Controls.AdvancedButton btnReset;
        private KGySoft.WinForms.Controls.AdvancedLabel lblValue;
        private System.Windows.Forms.TableLayoutPanel pnlCheckBoxes;
        private KGySoft.WinForms.Controls.AdvancedCheckBox chbBlue;
        private KGySoft.WinForms.Controls.AdvancedCheckBox chbGreen;
        private KGySoft.WinForms.Controls.AdvancedCheckBox chbRed;
    }
}