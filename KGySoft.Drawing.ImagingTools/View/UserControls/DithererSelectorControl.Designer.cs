namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    partial class DithererSelectorControl
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
            this.cmbDitherer = new System.Windows.Forms.ComboBox();
            this.tblStrength = new System.Windows.Forms.TableLayoutPanel();
            this.lblStrength = new System.Windows.Forms.Label();
            this.lblStrengthValue = new System.Windows.Forms.Label();
            this.tbStrength = new System.Windows.Forms.TrackBar();
            this.chbSerpentineProcessing = new System.Windows.Forms.CheckBox();
            this.chbByBrightness = new System.Windows.Forms.CheckBox();
            this.tblSeed = new System.Windows.Forms.TableLayoutPanel();
            this.lblSeed = new System.Windows.Forms.Label();
            this.txtSeed = new System.Windows.Forms.TextBox();
            this.tblStrength.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbStrength)).BeginInit();
            this.tblSeed.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbDitherer
            // 
            this.cmbDitherer.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmbDitherer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDitherer.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbDitherer.FormattingEnabled = true;
            this.cmbDitherer.Location = new System.Drawing.Point(0, 0);
            this.cmbDitherer.Name = "cmbDitherer";
            this.cmbDitherer.Size = new System.Drawing.Size(311, 21);
            this.cmbDitherer.TabIndex = 4;
            // 
            // tblStrength
            // 
            this.tblStrength.ColumnCount = 3;
            this.tblStrength.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tblStrength.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tblStrength.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblStrength.Controls.Add(this.lblStrength, 0, 0);
            this.tblStrength.Controls.Add(this.lblStrengthValue, 2, 0);
            this.tblStrength.Controls.Add(this.tbStrength, 1, 0);
            this.tblStrength.Dock = System.Windows.Forms.DockStyle.Top;
            this.tblStrength.Location = new System.Drawing.Point(0, 21);
            this.tblStrength.Name = "tblStrength";
            this.tblStrength.RowCount = 1;
            this.tblStrength.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblStrength.Size = new System.Drawing.Size(311, 22);
            this.tblStrength.TabIndex = 6;
            // 
            // lblStrength
            // 
            this.lblStrength.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblStrength.AutoSize = true;
            this.lblStrength.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblStrength.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStrength.Location = new System.Drawing.Point(3, 4);
            this.lblStrength.Name = "lblStrength";
            this.lblStrength.Size = new System.Drawing.Size(57, 13);
            this.lblStrength.TabIndex = 0;
            this.lblStrength.Text = "lblStrength";
            // 
            // lblStrengthValue
            // 
            this.lblStrengthValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblStrengthValue.AutoSize = true;
            this.lblStrengthValue.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblStrengthValue.Location = new System.Drawing.Point(243, 4);
            this.lblStrengthValue.Name = "lblStrengthValue";
            this.lblStrengthValue.Size = new System.Drawing.Size(13, 13);
            this.lblStrengthValue.TabIndex = 1;
            this.lblStrengthValue.Text = "0";
            // 
            // tbStrength
            // 
            this.tbStrength.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbStrength.LargeChange = 64;
            this.tbStrength.Location = new System.Drawing.Point(120, 0);
            this.tbStrength.Margin = new System.Windows.Forms.Padding(0);
            this.tbStrength.Maximum = 255;
            this.tbStrength.Name = "tbStrength";
            this.tbStrength.Size = new System.Drawing.Size(120, 22);
            this.tbStrength.TabIndex = 2;
            this.tbStrength.TickFrequency = 16;
            // 
            // chbSerpentineProcessing
            // 
            this.chbSerpentineProcessing.Dock = System.Windows.Forms.DockStyle.Top;
            this.chbSerpentineProcessing.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chbSerpentineProcessing.Location = new System.Drawing.Point(0, 43);
            this.chbSerpentineProcessing.Name = "chbSerpentineProcessing";
            this.chbSerpentineProcessing.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.chbSerpentineProcessing.Size = new System.Drawing.Size(311, 22);
            this.chbSerpentineProcessing.TabIndex = 9;
            this.chbSerpentineProcessing.Text = "chbSerpentineProcessing";
            this.chbSerpentineProcessing.UseVisualStyleBackColor = true;
            // 
            // chbByBrightness
            // 
            this.chbByBrightness.Checked = true;
            this.chbByBrightness.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.chbByBrightness.Dock = System.Windows.Forms.DockStyle.Top;
            this.chbByBrightness.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chbByBrightness.Location = new System.Drawing.Point(0, 65);
            this.chbByBrightness.Name = "chbByBrightness";
            this.chbByBrightness.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.chbByBrightness.Size = new System.Drawing.Size(311, 22);
            this.chbByBrightness.TabIndex = 10;
            this.chbByBrightness.Text = "chbByBrightness";
            this.chbByBrightness.ThreeState = true;
            this.chbByBrightness.UseVisualStyleBackColor = true;
            // 
            // tblSeed
            // 
            this.tblSeed.ColumnCount = 3;
            this.tblSeed.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tblSeed.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tblSeed.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblSeed.Controls.Add(this.lblSeed, 0, 0);
            this.tblSeed.Controls.Add(this.txtSeed, 1, 0);
            this.tblSeed.Dock = System.Windows.Forms.DockStyle.Top;
            this.tblSeed.Location = new System.Drawing.Point(0, 87);
            this.tblSeed.Name = "tblSeed";
            this.tblSeed.RowCount = 1;
            this.tblSeed.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblSeed.Size = new System.Drawing.Size(311, 22);
            this.tblSeed.TabIndex = 11;
            // 
            // lblSeed
            // 
            this.lblSeed.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblSeed.AutoSize = true;
            this.lblSeed.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblSeed.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblSeed.Location = new System.Drawing.Point(3, 4);
            this.lblSeed.Name = "lblSeed";
            this.lblSeed.Size = new System.Drawing.Size(42, 13);
            this.lblSeed.TabIndex = 0;
            this.lblSeed.Text = "lblSeed";
            // 
            // txtSeed
            // 
            this.txtSeed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSeed.Location = new System.Drawing.Point(123, 0);
            this.txtSeed.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.txtSeed.Name = "txtSeed";
            this.txtSeed.Size = new System.Drawing.Size(114, 20);
            this.txtSeed.TabIndex = 1;
            this.txtSeed.Text = "0";
            // 
            // DithererSelectorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tblSeed);
            this.Controls.Add(this.chbByBrightness);
            this.Controls.Add(this.chbSerpentineProcessing);
            this.Controls.Add(this.tblStrength);
            this.Controls.Add(this.cmbDitherer);
            this.Name = "DithererSelectorControl";
            this.Size = new System.Drawing.Size(311, 114);
            this.tblStrength.ResumeLayout(false);
            this.tblStrength.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbStrength)).EndInit();
            this.tblSeed.ResumeLayout(false);
            this.tblSeed.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ComboBox cmbDitherer;
        private System.Windows.Forms.TableLayoutPanel tblStrength;
        private System.Windows.Forms.Label lblStrength;
        private System.Windows.Forms.Label lblStrengthValue;
        private System.Windows.Forms.TrackBar tbStrength;
        private System.Windows.Forms.CheckBox chbSerpentineProcessing;
        private System.Windows.Forms.CheckBox chbByBrightness;
        private System.Windows.Forms.TableLayoutPanel tblSeed;
        private System.Windows.Forms.Label lblSeed;
        private System.Windows.Forms.TextBox txtSeed;
    }
}
