namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    partial class ResizeBitmapForm
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
            this.chbMaintainAspectRatio = new System.Windows.Forms.CheckBox();
            this.tblNewSize = new System.Windows.Forms.TableLayoutPanel();
            this.lblScalingMode = new System.Windows.Forms.Label();
            this.pnlHeightPx = new System.Windows.Forms.Panel();
            this.lblHeightPx = new System.Windows.Forms.Label();
            this.txtHeightPx = new System.Windows.Forms.TextBox();
            this.pnlHeightPercent = new System.Windows.Forms.Panel();
            this.lblHeightPercent = new System.Windows.Forms.Label();
            this.txtHeightPercent = new System.Windows.Forms.TextBox();
            this.pnlWidthPx = new System.Windows.Forms.Panel();
            this.lblWidthPx = new System.Windows.Forms.Label();
            this.txtWidthPx = new System.Windows.Forms.TextBox();
            this.rbByPixels = new System.Windows.Forms.RadioButton();
            this.rbByPercentage = new System.Windows.Forms.RadioButton();
            this.pnlWidthPercent = new System.Windows.Forms.Panel();
            this.lblWidthPercent = new System.Windows.Forms.Label();
            this.txtWidthPercent = new System.Windows.Forms.TextBox();
            this.lblWidth = new System.Windows.Forms.Label();
            this.lblHeight = new System.Windows.Forms.Label();
            this.cmbScalingMode = new System.Windows.Forms.ComboBox();
            this.pnlSettings.SuspendLayout();
            this.tblNewSize.SuspendLayout();
            this.pnlHeightPx.SuspendLayout();
            this.pnlHeightPercent.SuspendLayout();
            this.pnlWidthPx.SuspendLayout();
            this.pnlWidthPercent.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSettings
            // 
            this.pnlSettings.Controls.Add(this.tblNewSize);
            this.pnlSettings.Controls.Add(this.chbMaintainAspectRatio);
            this.pnlSettings.Padding = new System.Windows.Forms.Padding(5);
            this.pnlSettings.Size = new System.Drawing.Size(304, 139);
            // 
            // chbMaintainAspectRatio
            // 
            this.chbMaintainAspectRatio.AutoSize = true;
            this.chbMaintainAspectRatio.Dock = System.Windows.Forms.DockStyle.Top;
            this.chbMaintainAspectRatio.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chbMaintainAspectRatio.Location = new System.Drawing.Point(5, 5);
            this.chbMaintainAspectRatio.Name = "chbMaintainAspectRatio";
            this.chbMaintainAspectRatio.Size = new System.Drawing.Size(294, 18);
            this.chbMaintainAspectRatio.TabIndex = 0;
            this.chbMaintainAspectRatio.Text = "chbMaintainAspectRatio";
            this.chbMaintainAspectRatio.UseVisualStyleBackColor = true;
            // 
            // tblNewSize
            // 
            this.tblNewSize.ColumnCount = 3;
            this.tblNewSize.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tblNewSize.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblNewSize.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblNewSize.Controls.Add(this.lblScalingMode, 0, 3);
            this.tblNewSize.Controls.Add(this.pnlHeightPx, 2, 2);
            this.tblNewSize.Controls.Add(this.pnlHeightPercent, 1, 2);
            this.tblNewSize.Controls.Add(this.pnlWidthPx, 2, 1);
            this.tblNewSize.Controls.Add(this.rbByPixels, 2, 0);
            this.tblNewSize.Controls.Add(this.rbByPercentage, 1, 0);
            this.tblNewSize.Controls.Add(this.pnlWidthPercent, 1, 1);
            this.tblNewSize.Controls.Add(this.lblWidth, 0, 1);
            this.tblNewSize.Controls.Add(this.lblHeight, 0, 2);
            this.tblNewSize.Controls.Add(this.cmbScalingMode, 1, 3);
            this.tblNewSize.Dock = System.Windows.Forms.DockStyle.Top;
            this.tblNewSize.Location = new System.Drawing.Point(5, 23);
            this.tblNewSize.Name = "tblNewSize";
            this.tblNewSize.RowCount = 4;
            this.tblNewSize.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tblNewSize.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tblNewSize.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tblNewSize.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tblNewSize.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblNewSize.Size = new System.Drawing.Size(294, 108);
            this.tblNewSize.TabIndex = 1;
            // 
            // lblScalingMode
            // 
            this.lblScalingMode.AutoSize = true;
            this.lblScalingMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblScalingMode.Location = new System.Drawing.Point(3, 81);
            this.lblScalingMode.Name = "lblScalingMode";
            this.lblScalingMode.Size = new System.Drawing.Size(74, 27);
            this.lblScalingMode.TabIndex = 8;
            this.lblScalingMode.Text = "lblScalingMode";
            this.lblScalingMode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlHeightPx
            // 
            this.pnlHeightPx.Controls.Add(this.lblHeightPx);
            this.pnlHeightPx.Controls.Add(this.txtHeightPx);
            this.pnlHeightPx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlHeightPx.Location = new System.Drawing.Point(190, 57);
            this.pnlHeightPx.Name = "pnlHeightPx";
            this.pnlHeightPx.Size = new System.Drawing.Size(101, 21);
            this.pnlHeightPx.TabIndex = 7;
            // 
            // lblHeightPx
            // 
            this.lblHeightPx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHeightPx.Location = new System.Drawing.Point(62, 0);
            this.lblHeightPx.Name = "lblHeightPx";
            this.lblHeightPx.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.lblHeightPx.Size = new System.Drawing.Size(39, 21);
            this.lblHeightPx.TabIndex = 1;
            this.lblHeightPx.Text = "lblHeightPx";
            this.lblHeightPx.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtHeightPx
            // 
            this.txtHeightPx.Dock = System.Windows.Forms.DockStyle.Left;
            this.txtHeightPx.Location = new System.Drawing.Point(0, 0);
            this.txtHeightPx.Name = "txtHeightPx";
            this.txtHeightPx.Size = new System.Drawing.Size(62, 20);
            this.txtHeightPx.TabIndex = 0;
            this.txtHeightPx.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // pnlHeightPercent
            // 
            this.pnlHeightPercent.Controls.Add(this.lblHeightPercent);
            this.pnlHeightPercent.Controls.Add(this.txtHeightPercent);
            this.pnlHeightPercent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlHeightPercent.Location = new System.Drawing.Point(83, 57);
            this.pnlHeightPercent.Name = "pnlHeightPercent";
            this.pnlHeightPercent.Size = new System.Drawing.Size(101, 21);
            this.pnlHeightPercent.TabIndex = 6;
            // 
            // lblHeightPercent
            // 
            this.lblHeightPercent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHeightPercent.Location = new System.Drawing.Point(62, 0);
            this.lblHeightPercent.Name = "lblHeightPercent";
            this.lblHeightPercent.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.lblHeightPercent.Size = new System.Drawing.Size(39, 21);
            this.lblHeightPercent.TabIndex = 1;
            this.lblHeightPercent.Text = "lblHeightPercent";
            this.lblHeightPercent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtHeightPercent
            // 
            this.txtHeightPercent.Dock = System.Windows.Forms.DockStyle.Left;
            this.txtHeightPercent.Location = new System.Drawing.Point(0, 0);
            this.txtHeightPercent.Name = "txtHeightPercent";
            this.txtHeightPercent.Size = new System.Drawing.Size(62, 20);
            this.txtHeightPercent.TabIndex = 0;
            this.txtHeightPercent.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // pnlWidthPx
            // 
            this.pnlWidthPx.Controls.Add(this.lblWidthPx);
            this.pnlWidthPx.Controls.Add(this.txtWidthPx);
            this.pnlWidthPx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlWidthPx.Location = new System.Drawing.Point(190, 30);
            this.pnlWidthPx.Name = "pnlWidthPx";
            this.pnlWidthPx.Size = new System.Drawing.Size(101, 21);
            this.pnlWidthPx.TabIndex = 4;
            // 
            // lblWidthPx
            // 
            this.lblWidthPx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblWidthPx.Location = new System.Drawing.Point(62, 0);
            this.lblWidthPx.Name = "lblWidthPx";
            this.lblWidthPx.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.lblWidthPx.Size = new System.Drawing.Size(39, 21);
            this.lblWidthPx.TabIndex = 1;
            this.lblWidthPx.Text = "lblWidthPx";
            this.lblWidthPx.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtWidthPx
            // 
            this.txtWidthPx.Dock = System.Windows.Forms.DockStyle.Left;
            this.txtWidthPx.Location = new System.Drawing.Point(0, 0);
            this.txtWidthPx.Name = "txtWidthPx";
            this.txtWidthPx.Size = new System.Drawing.Size(62, 20);
            this.txtWidthPx.TabIndex = 0;
            this.txtWidthPx.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // rbByPixels
            // 
            this.rbByPixels.AutoSize = true;
            this.rbByPixels.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbByPixels.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbByPixels.Location = new System.Drawing.Point(190, 3);
            this.rbByPixels.Name = "rbByPixels";
            this.rbByPixels.Size = new System.Drawing.Size(101, 18);
            this.rbByPixels.TabIndex = 1;
            this.rbByPixels.TabStop = true;
            this.rbByPixels.Text = "rbByPixels";
            this.rbByPixels.UseVisualStyleBackColor = true;
            // 
            // rbByPercentage
            // 
            this.rbByPercentage.AutoSize = true;
            this.rbByPercentage.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbByPercentage.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbByPercentage.Location = new System.Drawing.Point(83, 3);
            this.rbByPercentage.Name = "rbByPercentage";
            this.rbByPercentage.Size = new System.Drawing.Size(101, 18);
            this.rbByPercentage.TabIndex = 0;
            this.rbByPercentage.TabStop = true;
            this.rbByPercentage.Text = "rbByPercentage";
            this.rbByPercentage.UseVisualStyleBackColor = true;
            // 
            // pnlWidthPercent
            // 
            this.pnlWidthPercent.Controls.Add(this.lblWidthPercent);
            this.pnlWidthPercent.Controls.Add(this.txtWidthPercent);
            this.pnlWidthPercent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlWidthPercent.Location = new System.Drawing.Point(83, 30);
            this.pnlWidthPercent.Name = "pnlWidthPercent";
            this.pnlWidthPercent.Size = new System.Drawing.Size(101, 21);
            this.pnlWidthPercent.TabIndex = 3;
            // 
            // lblWidthPercent
            // 
            this.lblWidthPercent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblWidthPercent.Location = new System.Drawing.Point(62, 0);
            this.lblWidthPercent.Name = "lblWidthPercent";
            this.lblWidthPercent.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.lblWidthPercent.Size = new System.Drawing.Size(39, 21);
            this.lblWidthPercent.TabIndex = 1;
            this.lblWidthPercent.Text = "lblWidthPercent";
            this.lblWidthPercent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtWidthPercent
            // 
            this.txtWidthPercent.Dock = System.Windows.Forms.DockStyle.Left;
            this.txtWidthPercent.Location = new System.Drawing.Point(0, 0);
            this.txtWidthPercent.Name = "txtWidthPercent";
            this.txtWidthPercent.Size = new System.Drawing.Size(62, 20);
            this.txtWidthPercent.TabIndex = 0;
            this.txtWidthPercent.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblWidth
            // 
            this.lblWidth.AutoSize = true;
            this.lblWidth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblWidth.Location = new System.Drawing.Point(3, 27);
            this.lblWidth.Name = "lblWidth";
            this.lblWidth.Size = new System.Drawing.Size(74, 27);
            this.lblWidth.TabIndex = 2;
            this.lblWidth.Text = "lblWidth";
            this.lblWidth.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblHeight
            // 
            this.lblHeight.AutoSize = true;
            this.lblHeight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHeight.Location = new System.Drawing.Point(3, 54);
            this.lblHeight.Name = "lblHeight";
            this.lblHeight.Size = new System.Drawing.Size(74, 27);
            this.lblHeight.TabIndex = 5;
            this.lblHeight.Text = "lblHeight";
            this.lblHeight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbScalingMode
            // 
            this.tblNewSize.SetColumnSpan(this.cmbScalingMode, 2);
            this.cmbScalingMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbScalingMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbScalingMode.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbScalingMode.FormattingEnabled = true;
            this.cmbScalingMode.Location = new System.Drawing.Point(83, 84);
            this.cmbScalingMode.Name = "cmbScalingMode";
            this.cmbScalingMode.Size = new System.Drawing.Size(208, 21);
            this.cmbScalingMode.TabIndex = 9;
            // 
            // ResizeBitmapForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 281);
            this.MinimumSize = new System.Drawing.Size(320, 320);
            this.Name = "ResizeBitmapForm";
            this.Text = "ResizeBitmapForm";
            this.pnlSettings.ResumeLayout(false);
            this.pnlSettings.PerformLayout();
            this.tblNewSize.ResumeLayout(false);
            this.tblNewSize.PerformLayout();
            this.pnlHeightPx.ResumeLayout(false);
            this.pnlHeightPx.PerformLayout();
            this.pnlHeightPercent.ResumeLayout(false);
            this.pnlHeightPercent.PerformLayout();
            this.pnlWidthPx.ResumeLayout(false);
            this.pnlWidthPx.PerformLayout();
            this.pnlWidthPercent.ResumeLayout(false);
            this.pnlWidthPercent.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblNewSize;
        private System.Windows.Forms.RadioButton rbByPixels;
        private System.Windows.Forms.RadioButton rbByPercentage;
        private System.Windows.Forms.Panel pnlWidthPercent;
        private System.Windows.Forms.Label lblWidthPercent;
        private System.Windows.Forms.TextBox txtWidthPercent;
        private System.Windows.Forms.CheckBox chbMaintainAspectRatio;
        private System.Windows.Forms.Label lblWidth;
        private System.Windows.Forms.Label lblHeight;
        private System.Windows.Forms.Panel pnlHeightPx;
        private System.Windows.Forms.Label lblHeightPx;
        private System.Windows.Forms.TextBox txtHeightPx;
        private System.Windows.Forms.Panel pnlHeightPercent;
        private System.Windows.Forms.Label lblHeightPercent;
        private System.Windows.Forms.TextBox txtHeightPercent;
        private System.Windows.Forms.Panel pnlWidthPx;
        private System.Windows.Forms.Label lblWidthPx;
        private System.Windows.Forms.TextBox txtWidthPx;
        private System.Windows.Forms.Label lblScalingMode;
        private System.Windows.Forms.ComboBox cmbScalingMode;
    }
}