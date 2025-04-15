using System.Drawing;

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal partial class ResizeBitmapControl
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
            this.tblNewSize = new System.Windows.Forms.TableLayoutPanel();
            this.chbMaintainAspectRatio = new KGySoft.Drawing.ImagingTools.View.Controls.AdvancedCheckBox();
            this.lblScalingMode = new System.Windows.Forms.Label();
            this.pnlHeightPx = new KGySoft.Drawing.ImagingTools.View.Controls.AutoMirrorPanel();
            this.lblHeightPx = new System.Windows.Forms.Label();
            this.txtHeightPx = new System.Windows.Forms.TextBox();
            this.pnlHeightPercent = new KGySoft.Drawing.ImagingTools.View.Controls.AutoMirrorPanel();
            this.lblHeightPercent = new System.Windows.Forms.Label();
            this.txtHeightPercent = new System.Windows.Forms.TextBox();
            this.pnlWidthPx = new KGySoft.Drawing.ImagingTools.View.Controls.AutoMirrorPanel();
            this.lblWidthPx = new System.Windows.Forms.Label();
            this.txtWidthPx = new System.Windows.Forms.TextBox();
            this.rbByPixels = new System.Windows.Forms.RadioButton();
            this.rbByPercentage = new System.Windows.Forms.RadioButton();
            this.pnlWidthPercent = new KGySoft.Drawing.ImagingTools.View.Controls.AutoMirrorPanel();
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
            this.pnlSettings.Padding = new System.Windows.Forms.Padding(5);
            this.pnlSettings.Size = new System.Drawing.Size(328, 143);
            // 
            // tblNewSize
            // 
            this.tblNewSize.ColumnCount = 3;
            this.tblNewSize.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tblNewSize.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblNewSize.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblNewSize.Controls.Add(this.chbMaintainAspectRatio, 1, 0);
            this.tblNewSize.Controls.Add(this.lblScalingMode, 0, 4);
            this.tblNewSize.Controls.Add(this.pnlHeightPx, 2, 3);
            this.tblNewSize.Controls.Add(this.pnlHeightPercent, 1, 3);
            this.tblNewSize.Controls.Add(this.pnlWidthPx, 2, 2);
            this.tblNewSize.Controls.Add(this.rbByPixels, 2, 1);
            this.tblNewSize.Controls.Add(this.rbByPercentage, 1, 1);
            this.tblNewSize.Controls.Add(this.pnlWidthPercent, 1, 2);
            this.tblNewSize.Controls.Add(this.lblWidth, 0, 2);
            this.tblNewSize.Controls.Add(this.lblHeight, 0, 3);
            this.tblNewSize.Controls.Add(this.cmbScalingMode, 1, 4);
            this.tblNewSize.Dock = System.Windows.Forms.DockStyle.Top;
            this.tblNewSize.Location = new System.Drawing.Point(5, 5);
            this.tblNewSize.Name = "tblNewSize";
            this.tblNewSize.RowCount = 5;
            this.tblNewSize.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tblNewSize.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tblNewSize.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tblNewSize.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tblNewSize.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tblNewSize.Size = new System.Drawing.Size(318, 137);
            this.tblNewSize.TabIndex = 0;
            // 
            // chbMaintainAspectRatio
            // 
            this.chbMaintainAspectRatio.AutoSize = true;
            this.tblNewSize.SetColumnSpan(this.chbMaintainAspectRatio, 2);
            this.chbMaintainAspectRatio.Dock = System.Windows.Forms.DockStyle.Top;
            this.chbMaintainAspectRatio.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chbMaintainAspectRatio.Location = new System.Drawing.Point(103, 3);
            this.chbMaintainAspectRatio.Name = "chbMaintainAspectRatio";
            this.chbMaintainAspectRatio.Size = new System.Drawing.Size(212, 18);
            this.chbMaintainAspectRatio.TabIndex = 0;
            this.chbMaintainAspectRatio.Text = "chbMaintainAspectRatio";
            this.chbMaintainAspectRatio.UseVisualStyleBackColor = true;
            // 
            // lblScalingMode
            // 
            this.lblScalingMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblScalingMode.Location = new System.Drawing.Point(3, 108);
            this.lblScalingMode.Name = "lblScalingMode";
            this.lblScalingMode.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.lblScalingMode.Size = new System.Drawing.Size(94, 29);
            this.lblScalingMode.TabIndex = 9;
            this.lblScalingMode.Text = "lblScalingMode";
            this.lblScalingMode.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // pnlHeightPx
            // 
            this.pnlHeightPx.Controls.Add(this.lblHeightPx);
            this.pnlHeightPx.Controls.Add(this.txtHeightPx);
            this.pnlHeightPx.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeightPx.Location = new System.Drawing.Point(209, 81);
            this.pnlHeightPx.Margin = new System.Windows.Forms.Padding(0);
            this.pnlHeightPx.Name = "pnlHeightPx";
            this.pnlHeightPx.Size = new System.Drawing.Size(109, 21);
            this.pnlHeightPx.TabIndex = 8;
            // 
            // lblHeightPx
            // 
            this.lblHeightPx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHeightPx.Location = new System.Drawing.Point(62, 0);
            this.lblHeightPx.Name = "lblHeightPx";
            this.lblHeightPx.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.lblHeightPx.Size = new System.Drawing.Size(47, 21);
            this.lblHeightPx.TabIndex = 1;
            this.lblHeightPx.Text = "lblHeightPx";
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
            this.pnlHeightPercent.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeightPercent.Location = new System.Drawing.Point(100, 81);
            this.pnlHeightPercent.Margin = new System.Windows.Forms.Padding(0);
            this.pnlHeightPercent.Name = "pnlHeightPercent";
            this.pnlHeightPercent.Size = new System.Drawing.Size(109, 21);
            this.pnlHeightPercent.TabIndex = 7;
            // 
            // lblHeightPercent
            // 
            this.lblHeightPercent.AutoSize = true;
            this.lblHeightPercent.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblHeightPercent.Location = new System.Drawing.Point(62, 0);
            this.lblHeightPercent.Name = "lblHeightPercent";
            this.lblHeightPercent.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.lblHeightPercent.Size = new System.Drawing.Size(85, 15);
            this.lblHeightPercent.TabIndex = 1;
            this.lblHeightPercent.Text = "lblHeightPercent";
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
            this.pnlWidthPx.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlWidthPx.Location = new System.Drawing.Point(209, 54);
            this.pnlWidthPx.Margin = new System.Windows.Forms.Padding(0);
            this.pnlWidthPx.Name = "pnlWidthPx";
            this.pnlWidthPx.Size = new System.Drawing.Size(109, 21);
            this.pnlWidthPx.TabIndex = 5;
            // 
            // lblWidthPx
            // 
            this.lblWidthPx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblWidthPx.Location = new System.Drawing.Point(62, 0);
            this.lblWidthPx.Name = "lblWidthPx";
            this.lblWidthPx.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.lblWidthPx.Size = new System.Drawing.Size(47, 21);
            this.lblWidthPx.TabIndex = 1;
            this.lblWidthPx.Text = "lblWidthPx";
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
            this.rbByPixels.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbByPixels.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbByPixels.Location = new System.Drawing.Point(212, 30);
            this.rbByPixels.Name = "rbByPixels";
            this.rbByPixels.Size = new System.Drawing.Size(103, 21);
            this.rbByPixels.TabIndex = 2;
            this.rbByPixels.TabStop = true;
            this.rbByPixels.Text = "rbByPixels";
            this.rbByPixels.UseVisualStyleBackColor = true;
            // 
            // rbByPercentage
            // 
            this.rbByPercentage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbByPercentage.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbByPercentage.Location = new System.Drawing.Point(103, 30);
            this.rbByPercentage.Name = "rbByPercentage";
            this.rbByPercentage.Size = new System.Drawing.Size(103, 21);
            this.rbByPercentage.TabIndex = 1;
            this.rbByPercentage.TabStop = true;
            this.rbByPercentage.Text = "rbByPercentage";
            this.rbByPercentage.UseVisualStyleBackColor = true;
            // 
            // pnlWidthPercent
            // 
            this.pnlWidthPercent.Controls.Add(this.lblWidthPercent);
            this.pnlWidthPercent.Controls.Add(this.txtWidthPercent);
            this.pnlWidthPercent.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlWidthPercent.Location = new System.Drawing.Point(100, 54);
            this.pnlWidthPercent.Margin = new System.Windows.Forms.Padding(0);
            this.pnlWidthPercent.Name = "pnlWidthPercent";
            this.pnlWidthPercent.Size = new System.Drawing.Size(109, 21);
            this.pnlWidthPercent.TabIndex = 4;
            // 
            // lblWidthPercent
            // 
            this.lblWidthPercent.AutoSize = true;
            this.lblWidthPercent.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblWidthPercent.Location = new System.Drawing.Point(62, 0);
            this.lblWidthPercent.Name = "lblWidthPercent";
            this.lblWidthPercent.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.lblWidthPercent.Size = new System.Drawing.Size(82, 15);
            this.lblWidthPercent.TabIndex = 1;
            this.lblWidthPercent.Text = "lblWidthPercent";
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
            this.lblWidth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblWidth.Location = new System.Drawing.Point(3, 54);
            this.lblWidth.Name = "lblWidth";
            this.lblWidth.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.lblWidth.Size = new System.Drawing.Size(94, 27);
            this.lblWidth.TabIndex = 3;
            this.lblWidth.Text = "lblWidth";
            this.lblWidth.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblHeight
            // 
            this.lblHeight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHeight.Location = new System.Drawing.Point(3, 81);
            this.lblHeight.Name = "lblHeight";
            this.lblHeight.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.lblHeight.Size = new System.Drawing.Size(94, 27);
            this.lblHeight.TabIndex = 6;
            this.lblHeight.Text = "lblHeight";
            this.lblHeight.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // cmbScalingMode
            // 
            this.tblNewSize.SetColumnSpan(this.cmbScalingMode, 2);
            this.cmbScalingMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbScalingMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbScalingMode.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbScalingMode.FormattingEnabled = true;
            this.cmbScalingMode.Location = new System.Drawing.Point(100, 108);
            this.cmbScalingMode.Margin = new System.Windows.Forms.Padding(0);
            this.cmbScalingMode.Name = "cmbScalingMode";
            this.cmbScalingMode.Size = new System.Drawing.Size(218, 21);
            this.cmbScalingMode.TabIndex = 10;
            // 
            // ResizeBitmapControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "ResizeBitmapControl";
            this.Size = new System.Drawing.Size(334, 291);
            this.pnlSettings.ResumeLayout(false);
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

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblNewSize;
        private System.Windows.Forms.RadioButton rbByPixels;
        private System.Windows.Forms.RadioButton rbByPercentage;
        private Controls.AutoMirrorPanel pnlWidthPercent;
        private System.Windows.Forms.Label lblWidthPercent;
        private System.Windows.Forms.TextBox txtWidthPercent;
        private System.Windows.Forms.Label lblWidth;
        private System.Windows.Forms.Label lblHeight;
        private Controls.AutoMirrorPanel pnlHeightPx;
        private System.Windows.Forms.Label lblHeightPx;
        private System.Windows.Forms.TextBox txtHeightPx;
        private Controls.AutoMirrorPanel pnlHeightPercent;
        private System.Windows.Forms.Label lblHeightPercent;
        private System.Windows.Forms.TextBox txtHeightPercent;
        private Controls.AutoMirrorPanel pnlWidthPx;
        private System.Windows.Forms.Label lblWidthPx;
        private System.Windows.Forms.TextBox txtWidthPx;
        private System.Windows.Forms.Label lblScalingMode;
        private System.Windows.Forms.ComboBox cmbScalingMode;
        private KGySoft.Drawing.ImagingTools.View.Controls.AdvancedCheckBox chbMaintainAspectRatio;
    }
}