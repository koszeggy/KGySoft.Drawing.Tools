namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    partial class AdjustBrightnessForm
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
            this.components = new System.ComponentModel.Container();
            this.progress = new KGySoft.Drawing.ImagingTools.View.Controls.DrawingProgressStatusStrip();
            this.okCancelButtons = new KGySoft.Drawing.ImagingTools.View.UserControls.OkCancelButtons();
            this.previewImage = new KGySoft.Drawing.ImagingTools.View.UserControls.PreviewImageControl();
            this.pnlSettings = new System.Windows.Forms.Panel();
            this.trackBar = new System.Windows.Forms.TrackBar();
            this.btnReset = new System.Windows.Forms.Button();
            this.lblValue = new System.Windows.Forms.Label();
            this.pnlCheckBoxes = new System.Windows.Forms.TableLayoutPanel();
            this.chbBlue = new System.Windows.Forms.CheckBox();
            this.chbGreen = new System.Windows.Forms.CheckBox();
            this.chbRed = new System.Windows.Forms.CheckBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.pnlSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
            this.pnlCheckBoxes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // progress
            // 
            this.progress.BackColor = System.Drawing.Color.Transparent;
            this.progress.Location = new System.Drawing.Point(0, 189);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(248, 22);
            this.progress.SizingGrip = false;
            this.progress.TabIndex = 3;
            this.progress.Text = "drawingProgressStatusStrip1";
            // 
            // okCancelButtons
            // 
            this.okCancelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.okCancelButtons.Location = new System.Drawing.Point(0, 149);
            this.okCancelButtons.Name = "okCancelButtons";
            this.okCancelButtons.Size = new System.Drawing.Size(248, 40);
            this.okCancelButtons.TabIndex = 2;
            // 
            // previewImage
            // 
            this.previewImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewImage.Location = new System.Drawing.Point(0, 56);
            this.previewImage.Name = "previewImage";
            this.previewImage.Size = new System.Drawing.Size(248, 93);
            this.previewImage.TabIndex = 1;
            // 
            // pnlSettings
            // 
            this.pnlSettings.Controls.Add(this.trackBar);
            this.pnlSettings.Controls.Add(this.btnReset);
            this.pnlSettings.Controls.Add(this.lblValue);
            this.pnlSettings.Controls.Add(this.pnlCheckBoxes);
            this.pnlSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSettings.Location = new System.Drawing.Point(0, 0);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Size = new System.Drawing.Size(248, 56);
            this.pnlSettings.TabIndex = 0;
            // 
            // trackBar
            // 
            this.trackBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBar.LargeChange = 10;
            this.trackBar.Location = new System.Drawing.Point(35, 25);
            this.trackBar.Maximum = 100;
            this.trackBar.Minimum = -100;
            this.trackBar.Name = "trackBar";
            this.trackBar.Size = new System.Drawing.Size(161, 31);
            this.trackBar.TabIndex = 2;
            this.trackBar.TickFrequency = 10;
            // 
            // btnReset
            // 
            this.btnReset.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnReset.Location = new System.Drawing.Point(196, 25);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(52, 31);
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
            this.pnlCheckBoxes.Size = new System.Drawing.Size(248, 25);
            this.pnlCheckBoxes.TabIndex = 0;
            // 
            // chbBlue
            // 
            this.chbBlue.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chbBlue.AutoSize = true;
            this.chbBlue.Location = new System.Drawing.Point(173, 4);
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
            this.chbGreen.Location = new System.Drawing.Point(86, 4);
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
            this.chbRed.Location = new System.Drawing.Point(9, 4);
            this.chbRed.Name = "chbRed";
            this.chbRed.Size = new System.Drawing.Size(64, 17);
            this.chbRed.TabIndex = 0;
            this.chbRed.Text = "chbRed";
            this.chbRed.UseVisualStyleBackColor = true;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // AdjustBrightnessForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(248, 211);
            this.Controls.Add(this.previewImage);
            this.Controls.Add(this.pnlSettings);
            this.Controls.Add(this.okCancelButtons);
            this.Controls.Add(this.progress);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(250, 250);
            this.Name = "AdjustBrightnessForm";
            this.Text = "AdjustBrightnessForm";
            this.pnlSettings.ResumeLayout(false);
            this.pnlSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
            this.pnlCheckBoxes.ResumeLayout(false);
            this.pnlCheckBoxes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.DrawingProgressStatusStrip progress;
        private UserControls.OkCancelButtons okCancelButtons;
        private UserControls.PreviewImageControl previewImage;
        private System.Windows.Forms.Panel pnlSettings;
        private System.Windows.Forms.TableLayoutPanel pnlCheckBoxes;
        private System.Windows.Forms.CheckBox chbBlue;
        private System.Windows.Forms.CheckBox chbGreen;
        private System.Windows.Forms.CheckBox chbRed;
        private System.Windows.Forms.TrackBar trackBar;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}