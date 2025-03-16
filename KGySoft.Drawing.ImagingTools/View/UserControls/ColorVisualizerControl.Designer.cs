using KGySoft.Drawing.ImagingTools.View.Controls;

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    partial class ColorVisualizerControl
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
            this.pnlControls = new System.Windows.Forms.Panel();
            this.tblColor = new System.Windows.Forms.TableLayoutPanel();
            this.lblRed = new System.Windows.Forms.Label();
            this.pnlRed = new System.Windows.Forms.Panel();
            this.tbRed = new System.Windows.Forms.TrackBar();
            this.lblGreen = new System.Windows.Forms.Label();
            this.pnlGreen = new System.Windows.Forms.Panel();
            this.tbGreen = new System.Windows.Forms.TrackBar();
            this.lblBlue = new System.Windows.Forms.Label();
            this.pnlBlue = new System.Windows.Forms.Panel();
            this.tbBlue = new System.Windows.Forms.TrackBar();
            this.pnlColor = new System.Windows.Forms.Panel();
            this.lblAlpha = new System.Windows.Forms.Label();
            this.pnlAlpha = new System.Windows.Forms.Panel();
            this.tbAlpha = new System.Windows.Forms.TrackBar();
            this.tsMenu = new KGySoft.Drawing.ImagingTools.View.Controls.AdvancedToolStrip();
            this.btnSelectColor = new System.Windows.Forms.ToolStripButton();
            this.txtColor = new System.Windows.Forms.TextBox();
            this.buttons = new KGySoft.Drawing.ImagingTools.View.UserControls.OkCancelButtons();
            this.pnlControls.SuspendLayout();
            this.tblColor.SuspendLayout();
            this.pnlRed.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbRed)).BeginInit();
            this.pnlGreen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbGreen)).BeginInit();
            this.pnlBlue.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbBlue)).BeginInit();
            this.pnlAlpha.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbAlpha)).BeginInit();
            this.tsMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlControls
            // 
            this.pnlControls.Controls.Add(this.tblColor);
            this.pnlControls.Controls.Add(this.tsMenu);
            this.pnlControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlControls.Location = new System.Drawing.Point(0, 0);
            this.pnlControls.Name = "pnlControls";
            this.pnlControls.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.pnlControls.Size = new System.Drawing.Size(247, 83);
            this.pnlControls.TabIndex = 0;
            // 
            // tblColor
            // 
            this.tblColor.ColumnCount = 3;
            this.tblColor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tblColor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tblColor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblColor.Controls.Add(this.lblRed, 0, 1);
            this.tblColor.Controls.Add(this.pnlRed, 1, 1);
            this.tblColor.Controls.Add(this.lblGreen, 0, 2);
            this.tblColor.Controls.Add(this.pnlGreen, 1, 2);
            this.tblColor.Controls.Add(this.lblBlue, 0, 3);
            this.tblColor.Controls.Add(this.pnlBlue, 1, 3);
            this.tblColor.Controls.Add(this.pnlColor, 2, 0);
            this.tblColor.Controls.Add(this.lblAlpha, 0, 0);
            this.tblColor.Controls.Add(this.pnlAlpha, 1, 0);
            this.tblColor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblColor.Location = new System.Drawing.Point(0, 0);
            this.tblColor.Name = "tblColor";
            this.tblColor.RowCount = 4;
            this.tblColor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tblColor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tblColor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tblColor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tblColor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblColor.Size = new System.Drawing.Size(223, 80);
            this.tblColor.TabIndex = 1;
            // 
            // lblRed
            // 
            this.lblRed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRed.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblRed.Location = new System.Drawing.Point(3, 20);
            this.lblRed.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.lblRed.Name = "lblRed";
            this.lblRed.Size = new System.Drawing.Size(47, 20);
            this.lblRed.TabIndex = 2;
            this.lblRed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlRed
            // 
            this.pnlRed.Controls.Add(this.tbRed);
            this.pnlRed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRed.Location = new System.Drawing.Point(51, 21);
            this.pnlRed.Margin = new System.Windows.Forms.Padding(1);
            this.pnlRed.Name = "pnlRed";
            this.pnlRed.Size = new System.Drawing.Size(78, 18);
            this.pnlRed.TabIndex = 3;
            // 
            // tbRed
            // 
            this.tbRed.AutoSize = false;
            this.tbRed.Dock = System.Windows.Forms.DockStyle.Top;
            this.tbRed.LargeChange = 64;
            this.tbRed.Location = new System.Drawing.Point(0, 0);
            this.tbRed.Maximum = 255;
            this.tbRed.Name = "tbRed";
            this.tbRed.RightToLeftLayout = true;
            this.tbRed.Size = new System.Drawing.Size(78, 18);
            this.tbRed.TabIndex = 0;
            this.tbRed.TickFrequency = 64;
            // 
            // lblGreen
            // 
            this.lblGreen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblGreen.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblGreen.Location = new System.Drawing.Point(3, 40);
            this.lblGreen.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.lblGreen.Name = "lblGreen";
            this.lblGreen.Size = new System.Drawing.Size(47, 20);
            this.lblGreen.TabIndex = 4;
            this.lblGreen.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlGreen
            // 
            this.pnlGreen.Controls.Add(this.tbGreen);
            this.pnlGreen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGreen.Location = new System.Drawing.Point(51, 41);
            this.pnlGreen.Margin = new System.Windows.Forms.Padding(1);
            this.pnlGreen.Name = "pnlGreen";
            this.pnlGreen.Size = new System.Drawing.Size(78, 18);
            this.pnlGreen.TabIndex = 5;
            // 
            // tbGreen
            // 
            this.tbGreen.AutoSize = false;
            this.tbGreen.Dock = System.Windows.Forms.DockStyle.Top;
            this.tbGreen.LargeChange = 64;
            this.tbGreen.Location = new System.Drawing.Point(0, 0);
            this.tbGreen.Maximum = 255;
            this.tbGreen.Name = "tbGreen";
            this.tbGreen.RightToLeftLayout = true;
            this.tbGreen.Size = new System.Drawing.Size(78, 18);
            this.tbGreen.TabIndex = 0;
            this.tbGreen.TickFrequency = 64;
            // 
            // lblBlue
            // 
            this.lblBlue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblBlue.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblBlue.Location = new System.Drawing.Point(3, 60);
            this.lblBlue.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.lblBlue.Name = "lblBlue";
            this.lblBlue.Size = new System.Drawing.Size(47, 20);
            this.lblBlue.TabIndex = 6;
            this.lblBlue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlBlue
            // 
            this.pnlBlue.Controls.Add(this.tbBlue);
            this.pnlBlue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBlue.Location = new System.Drawing.Point(51, 61);
            this.pnlBlue.Margin = new System.Windows.Forms.Padding(1);
            this.pnlBlue.Name = "pnlBlue";
            this.pnlBlue.Size = new System.Drawing.Size(78, 18);
            this.pnlBlue.TabIndex = 7;
            // 
            // tbBlue
            // 
            this.tbBlue.AutoSize = false;
            this.tbBlue.Dock = System.Windows.Forms.DockStyle.Top;
            this.tbBlue.LargeChange = 64;
            this.tbBlue.Location = new System.Drawing.Point(0, 0);
            this.tbBlue.Maximum = 255;
            this.tbBlue.Name = "tbBlue";
            this.tbBlue.RightToLeftLayout = true;
            this.tbBlue.Size = new System.Drawing.Size(78, 18);
            this.tbBlue.TabIndex = 0;
            this.tbBlue.TickFrequency = 64;
            // 
            // pnlColor
            // 
            this.pnlColor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlColor.Location = new System.Drawing.Point(133, 1);
            this.pnlColor.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.pnlColor.Name = "pnlColor";
            this.tblColor.SetRowSpan(this.pnlColor, 4);
            this.pnlColor.Size = new System.Drawing.Size(87, 78);
            this.pnlColor.TabIndex = 8;
            // 
            // lblAlpha
            // 
            this.lblAlpha.AutoSize = true;
            this.lblAlpha.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAlpha.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblAlpha.Location = new System.Drawing.Point(3, 0);
            this.lblAlpha.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.lblAlpha.Name = "lblAlpha";
            this.lblAlpha.Size = new System.Drawing.Size(47, 20);
            this.lblAlpha.TabIndex = 0;
            this.lblAlpha.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlAlpha
            // 
            this.pnlAlpha.Controls.Add(this.tbAlpha);
            this.pnlAlpha.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAlpha.Location = new System.Drawing.Point(51, 1);
            this.pnlAlpha.Margin = new System.Windows.Forms.Padding(1);
            this.pnlAlpha.Name = "pnlAlpha";
            this.pnlAlpha.Size = new System.Drawing.Size(78, 18);
            this.pnlAlpha.TabIndex = 1;
            // 
            // tbAlpha
            // 
            this.tbAlpha.AutoSize = false;
            this.tbAlpha.Dock = System.Windows.Forms.DockStyle.Top;
            this.tbAlpha.LargeChange = 64;
            this.tbAlpha.Location = new System.Drawing.Point(0, 0);
            this.tbAlpha.Maximum = 255;
            this.tbAlpha.Name = "tbAlpha";
            this.tbAlpha.RightToLeftLayout = true;
            this.tbAlpha.Size = new System.Drawing.Size(78, 18);
            this.tbAlpha.TabIndex = 0;
            this.tbAlpha.TickFrequency = 64;
            // 
            // tsMenu
            // 
            this.tsMenu.Dock = System.Windows.Forms.DockStyle.Right;
            this.tsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSelectColor});
            this.tsMenu.Location = new System.Drawing.Point(223, 0);
            this.tsMenu.Name = "tsMenu";
            this.tsMenu.Size = new System.Drawing.Size(24, 80);
            this.tsMenu.TabIndex = 1;
            // 
            // btnSelectColor
            // 
            this.btnSelectColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSelectColor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSelectColor.Name = "btnSelectColor";
            this.btnSelectColor.Size = new System.Drawing.Size(21, 4);
            // 
            // txtColor
            // 
            this.txtColor.BackColor = System.Drawing.SystemColors.Control;
            this.txtColor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtColor.ForeColor = System.Drawing.SystemColors.ControlText;
            this.txtColor.Location = new System.Drawing.Point(0, 83);
            this.txtColor.Multiline = true;
            this.txtColor.Name = "txtColor";
            this.txtColor.ReadOnly = true;
            this.txtColor.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtColor.Size = new System.Drawing.Size(247, 95);
            this.txtColor.TabIndex = 1;
            this.txtColor.TabStop = false;
            this.txtColor.WordWrap = false;
            // 
            // buttons
            // 
            this.buttons.BackColor = System.Drawing.Color.Transparent;
            this.buttons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttons.Location = new System.Drawing.Point(0, 178);
            this.buttons.Name = "buttons";
            this.buttons.Size = new System.Drawing.Size(247, 35);
            this.buttons.TabIndex = 2;
            this.buttons.Visible = false;
            // 
            // ColorVisualizerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtColor);
            this.Controls.Add(this.pnlControls);
            this.Controls.Add(this.buttons);
            this.Name = "ColorVisualizerControl";
            this.Size = new System.Drawing.Size(244, 221);
            this.pnlControls.ResumeLayout(false);
            this.pnlControls.PerformLayout();
            this.tblColor.ResumeLayout(false);
            this.tblColor.PerformLayout();
            this.pnlRed.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tbRed)).EndInit();
            this.pnlGreen.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tbGreen)).EndInit();
            this.pnlBlue.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tbBlue)).EndInit();
            this.pnlAlpha.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tbAlpha)).EndInit();
            this.tsMenu.ResumeLayout(false);
            this.tsMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlControls;
        private AdvancedToolStrip tsMenu;
        private System.Windows.Forms.TableLayoutPanel tblColor;
        private System.Windows.Forms.Label lblRed;
        private System.Windows.Forms.Panel pnlRed;
        private System.Windows.Forms.Label lblGreen;
        private System.Windows.Forms.Panel pnlGreen;
        private System.Windows.Forms.Label lblBlue;
        private System.Windows.Forms.Panel pnlBlue;
        private System.Windows.Forms.Panel pnlColor;
        private System.Windows.Forms.Label lblAlpha;
        private System.Windows.Forms.Panel pnlAlpha;
        private System.Windows.Forms.ToolStripButton btnSelectColor;
        private System.Windows.Forms.TextBox txtColor;
        private System.Windows.Forms.TrackBar tbAlpha;
        private System.Windows.Forms.TrackBar tbRed;
        private System.Windows.Forms.TrackBar tbGreen;
        private System.Windows.Forms.TrackBar tbBlue;
        private OkCancelButtons buttons;
    }
}
