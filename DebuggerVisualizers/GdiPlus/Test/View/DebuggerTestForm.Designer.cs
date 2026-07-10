namespace KGySoft.Drawing.DebuggerVisualizers.GdiPlus.Test.View
{
    partial class DebuggerTestForm
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
            this.btnViewDirect = new KGySoft.WinForms.Controls.AdvancedButton();
            this.btnViewByClassicDebugger = new KGySoft.WinForms.Controls.AdvancedButton();
            this.txtFile = new KGySoft.WinForms.Controls.AdvancedTextBox();
            this.rbFromFile = new KGySoft.WinForms.Controls.AdvancedRadioButton();
            this.rbManagedIcon = new KGySoft.WinForms.Controls.AdvancedRadioButton();
            this.rbHIcon = new KGySoft.WinForms.Controls.AdvancedRadioButton();
            this.rbMetafile = new KGySoft.WinForms.Controls.AdvancedRadioButton();
            this.rbBitmap = new KGySoft.WinForms.Controls.AdvancedRadioButton();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.panelControls = new System.Windows.Forms.Panel();
            this.btnViewByExtensionDebugger = new KGySoft.WinForms.Controls.AdvancedButton();
            this.chbAsReadOnly = new KGySoft.WinForms.Controls.AdvancedCheckBox();
            this.gbFile = new System.Windows.Forms.GroupBox();
            this.rbAsIcon = new KGySoft.WinForms.Controls.AdvancedRadioButton();
            this.rbAsMetafile = new KGySoft.WinForms.Controls.AdvancedRadioButton();
            this.rbAsBitmap = new KGySoft.WinForms.Controls.AdvancedRadioButton();
            this.rbAsImage = new KGySoft.WinForms.Controls.AdvancedRadioButton();
            this.rbColor = new KGySoft.WinForms.Controls.AdvancedRadioButton();
            this.rbPalette = new KGySoft.WinForms.Controls.AdvancedRadioButton();
            this.rbBitmapData = new KGySoft.WinForms.Controls.AdvancedRadioButton();
            this.rbGraphicsHwnd = new KGySoft.WinForms.Controls.AdvancedRadioButton();
            this.rbGraphicsBitmap = new KGySoft.WinForms.Controls.AdvancedRadioButton();
            this.chbAsImage = new KGySoft.WinForms.Controls.AdvancedCheckBox();
            this.cmbPixelFormat = new KGySoft.WinForms.Controls.AdvancedComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.panelControls.SuspendLayout();
            this.gbFile.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnViewDirect
            // 
            this.btnViewDirect.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnViewDirect.Location = new System.Drawing.Point(0, 335);
            this.btnViewDirect.Name = "btnViewDirect";
            this.btnViewDirect.Size = new System.Drawing.Size(196, 24);
            this.btnViewDirect.TabIndex = 14;
            this.btnViewDirect.Text = "View Directly";
            this.btnViewDirect.UseVisualStyleBackColor = true;
            // 
            // btnViewByClassicDebugger
            // 
            this.btnViewByClassicDebugger.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnViewByClassicDebugger.Location = new System.Drawing.Point(0, 359);
            this.btnViewByClassicDebugger.Name = "btnViewByClassicDebugger";
            this.btnViewByClassicDebugger.Size = new System.Drawing.Size(196, 24);
            this.btnViewByClassicDebugger.TabIndex = 15;
            this.btnViewByClassicDebugger.Text = "View by Classic Debugger";
            this.btnViewByClassicDebugger.UseVisualStyleBackColor = true;
            // 
            // txtFile
            // 
            this.txtFile.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtFile.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.txtFile.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtFile.Location = new System.Drawing.Point(3, 16);
            this.txtFile.Name = "txtFile";
            this.txtFile.Size = new System.Drawing.Size(190, 20);
            this.txtFile.TabIndex = 0;
            // 
            // rbFromFile
            // 
            this.rbFromFile.AutoSize = true;
            this.rbFromFile.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbFromFile.Location = new System.Drawing.Point(0, 191);
            this.rbFromFile.Name = "rbFromFile";
            this.rbFromFile.Size = new System.Drawing.Size(196, 17);
            this.rbFromFile.TabIndex = 11;
            this.rbFromFile.Text = "Image From File";
            this.rbFromFile.UseVisualStyleBackColor = true;
            // 
            // rbManagedIcon
            // 
            this.rbManagedIcon.AutoSize = true;
            this.rbManagedIcon.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbManagedIcon.Location = new System.Drawing.Point(0, 89);
            this.rbManagedIcon.Name = "rbManagedIcon";
            this.rbManagedIcon.Size = new System.Drawing.Size(196, 17);
            this.rbManagedIcon.TabIndex = 5;
            this.rbManagedIcon.Text = "Managed Icon";
            this.rbManagedIcon.UseVisualStyleBackColor = true;
            // 
            // rbHIcon
            // 
            this.rbHIcon.AutoSize = true;
            this.rbHIcon.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbHIcon.Location = new System.Drawing.Point(0, 72);
            this.rbHIcon.Name = "rbHIcon";
            this.rbHIcon.Size = new System.Drawing.Size(196, 17);
            this.rbHIcon.TabIndex = 4;
            this.rbHIcon.Text = "Icon from Handle";
            this.rbHIcon.UseVisualStyleBackColor = true;
            // 
            // rbMetafile
            // 
            this.rbMetafile.AutoSize = true;
            this.rbMetafile.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbMetafile.Location = new System.Drawing.Point(0, 55);
            this.rbMetafile.Name = "rbMetafile";
            this.rbMetafile.Size = new System.Drawing.Size(196, 17);
            this.rbMetafile.TabIndex = 3;
            this.rbMetafile.Text = "Metafile";
            this.rbMetafile.UseVisualStyleBackColor = true;
            // 
            // rbBitmap
            // 
            this.rbBitmap.AutoSize = true;
            this.rbBitmap.Checked = true;
            this.rbBitmap.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbBitmap.Location = new System.Drawing.Point(0, 38);
            this.rbBitmap.Name = "rbBitmap";
            this.rbBitmap.Size = new System.Drawing.Size(196, 17);
            this.rbBitmap.TabIndex = 2;
            this.rbBitmap.TabStop = true;
            this.rbBitmap.Text = "Bitmap";
            this.rbBitmap.UseVisualStyleBackColor = true;
            // 
            // pictureBox
            // 
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(224, 10);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(195, 420);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 14;
            this.pictureBox.TabStop = false;
            // 
            // panelControls
            // 
            this.panelControls.Controls.Add(this.btnViewByExtensionDebugger);
            this.panelControls.Controls.Add(this.btnViewByClassicDebugger);
            this.panelControls.Controls.Add(this.btnViewDirect);
            this.panelControls.Controls.Add(this.chbAsReadOnly);
            this.panelControls.Controls.Add(this.gbFile);
            this.panelControls.Controls.Add(this.rbFromFile);
            this.panelControls.Controls.Add(this.rbColor);
            this.panelControls.Controls.Add(this.rbPalette);
            this.panelControls.Controls.Add(this.rbBitmapData);
            this.panelControls.Controls.Add(this.rbGraphicsHwnd);
            this.panelControls.Controls.Add(this.rbGraphicsBitmap);
            this.panelControls.Controls.Add(this.rbManagedIcon);
            this.panelControls.Controls.Add(this.rbHIcon);
            this.panelControls.Controls.Add(this.rbMetafile);
            this.panelControls.Controls.Add(this.rbBitmap);
            this.panelControls.Controls.Add(this.chbAsImage);
            this.panelControls.Controls.Add(this.cmbPixelFormat);
            this.panelControls.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelControls.Location = new System.Drawing.Point(15, 10);
            this.panelControls.Name = "panelControls";
            this.panelControls.Padding = new System.Windows.Forms.Padding(0, 0, 13, 0);
            this.panelControls.Size = new System.Drawing.Size(209, 420);
            this.panelControls.TabIndex = 0;
            // 
            // btnViewByExtensionDebugger
            // 
            this.btnViewByExtensionDebugger.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnViewByExtensionDebugger.Location = new System.Drawing.Point(0, 383);
            this.btnViewByExtensionDebugger.Name = "btnViewByExtensionDebugger";
            this.btnViewByExtensionDebugger.Size = new System.Drawing.Size(196, 24);
            this.btnViewByExtensionDebugger.TabIndex = 16;
            this.btnViewByExtensionDebugger.Text = "View by Extension Debugger";
            this.btnViewByExtensionDebugger.UseVisualStyleBackColor = true;
            // 
            // chbAsReadOnly
            // 
            this.chbAsReadOnly.AutoSize = true;
            this.chbAsReadOnly.Dock = System.Windows.Forms.DockStyle.Top;
            this.chbAsReadOnly.Location = new System.Drawing.Point(0, 318);
            this.chbAsReadOnly.Name = "chbAsReadOnly";
            this.chbAsReadOnly.Size = new System.Drawing.Size(196, 17);
            this.chbAsReadOnly.TabIndex = 13;
            this.chbAsReadOnly.Text = "As Read-Only";
            this.chbAsReadOnly.UseVisualStyleBackColor = true;
            // 
            // gbFile
            // 
            this.gbFile.Controls.Add(this.rbAsIcon);
            this.gbFile.Controls.Add(this.rbAsMetafile);
            this.gbFile.Controls.Add(this.rbAsBitmap);
            this.gbFile.Controls.Add(this.rbAsImage);
            this.gbFile.Controls.Add(this.txtFile);
            this.gbFile.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbFile.Enabled = false;
            this.gbFile.Location = new System.Drawing.Point(0, 208);
            this.gbFile.Name = "gbFile";
            this.gbFile.Size = new System.Drawing.Size(196, 110);
            this.gbFile.TabIndex = 12;
            this.gbFile.TabStop = false;
            this.gbFile.Text = "File Details";
            // 
            // rbAsIcon
            // 
            this.rbAsIcon.AutoSize = true;
            this.rbAsIcon.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbAsIcon.Location = new System.Drawing.Point(3, 87);
            this.rbAsIcon.Name = "rbAsIcon";
            this.rbAsIcon.Size = new System.Drawing.Size(190, 17);
            this.rbAsIcon.TabIndex = 4;
            this.rbAsIcon.Text = "As Icon";
            this.rbAsIcon.UseVisualStyleBackColor = true;
            // 
            // rbAsMetafile
            // 
            this.rbAsMetafile.AutoSize = true;
            this.rbAsMetafile.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbAsMetafile.Location = new System.Drawing.Point(3, 70);
            this.rbAsMetafile.Name = "rbAsMetafile";
            this.rbAsMetafile.Size = new System.Drawing.Size(190, 17);
            this.rbAsMetafile.TabIndex = 3;
            this.rbAsMetafile.Text = "As Metafile";
            this.rbAsMetafile.UseVisualStyleBackColor = true;
            // 
            // rbAsBitmap
            // 
            this.rbAsBitmap.AutoSize = true;
            this.rbAsBitmap.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbAsBitmap.Location = new System.Drawing.Point(3, 53);
            this.rbAsBitmap.Name = "rbAsBitmap";
            this.rbAsBitmap.Size = new System.Drawing.Size(190, 17);
            this.rbAsBitmap.TabIndex = 2;
            this.rbAsBitmap.Text = "As Bitmap";
            this.rbAsBitmap.UseVisualStyleBackColor = true;
            // 
            // rbAsImage
            // 
            this.rbAsImage.AutoSize = true;
            this.rbAsImage.Checked = true;
            this.rbAsImage.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbAsImage.Location = new System.Drawing.Point(3, 36);
            this.rbAsImage.Name = "rbAsImage";
            this.rbAsImage.Size = new System.Drawing.Size(190, 17);
            this.rbAsImage.TabIndex = 1;
            this.rbAsImage.TabStop = true;
            this.rbAsImage.Text = "As Image";
            this.rbAsImage.UseVisualStyleBackColor = true;
            // 
            // rbColor
            // 
            this.rbColor.AutoSize = true;
            this.rbColor.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbColor.Location = new System.Drawing.Point(0, 174);
            this.rbColor.Name = "rbColor";
            this.rbColor.Size = new System.Drawing.Size(196, 17);
            this.rbColor.TabIndex = 10;
            this.rbColor.Text = "Single Color";
            this.rbColor.UseVisualStyleBackColor = true;
            // 
            // rbPalette
            // 
            this.rbPalette.AutoSize = true;
            this.rbPalette.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbPalette.Location = new System.Drawing.Point(0, 157);
            this.rbPalette.Name = "rbPalette";
            this.rbPalette.Size = new System.Drawing.Size(196, 17);
            this.rbPalette.TabIndex = 9;
            this.rbPalette.Text = "Palette";
            this.rbPalette.UseVisualStyleBackColor = true;
            // 
            // rbBitmapData
            // 
            this.rbBitmapData.AutoSize = true;
            this.rbBitmapData.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbBitmapData.Location = new System.Drawing.Point(0, 140);
            this.rbBitmapData.Name = "rbBitmapData";
            this.rbBitmapData.Size = new System.Drawing.Size(196, 17);
            this.rbBitmapData.TabIndex = 8;
            this.rbBitmapData.Text = "BitmapData";
            this.rbBitmapData.UseVisualStyleBackColor = true;
            // 
            // rbGraphicsHwnd
            // 
            this.rbGraphicsHwnd.AutoSize = true;
            this.rbGraphicsHwnd.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbGraphicsHwnd.Location = new System.Drawing.Point(0, 123);
            this.rbGraphicsHwnd.Name = "rbGraphicsHwnd";
            this.rbGraphicsHwnd.Size = new System.Drawing.Size(196, 17);
            this.rbGraphicsHwnd.TabIndex = 7;
            this.rbGraphicsHwnd.Text = "Graphics from Handle";
            this.rbGraphicsHwnd.UseVisualStyleBackColor = true;
            // 
            // rbGraphicsBitmap
            // 
            this.rbGraphicsBitmap.AutoSize = true;
            this.rbGraphicsBitmap.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbGraphicsBitmap.Location = new System.Drawing.Point(0, 106);
            this.rbGraphicsBitmap.Name = "rbGraphicsBitmap";
            this.rbGraphicsBitmap.Size = new System.Drawing.Size(196, 17);
            this.rbGraphicsBitmap.TabIndex = 6;
            this.rbGraphicsBitmap.Text = "Graphics from Bitmap";
            this.rbGraphicsBitmap.UseVisualStyleBackColor = true;
            // 
            // chbAsImage
            // 
            this.chbAsImage.AutoSize = true;
            this.chbAsImage.Checked = true;
            this.chbAsImage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbAsImage.Dock = System.Windows.Forms.DockStyle.Top;
            this.chbAsImage.Location = new System.Drawing.Point(0, 21);
            this.chbAsImage.Name = "chbAsImage";
            this.chbAsImage.Size = new System.Drawing.Size(196, 17);
            this.chbAsImage.TabIndex = 1;
            this.chbAsImage.Text = "As Image";
            this.chbAsImage.UseVisualStyleBackColor = true;
            // 
            // cmbPixelFormat
            // 
            this.cmbPixelFormat.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmbPixelFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPixelFormat.FormattingEnabled = true;
            this.cmbPixelFormat.Location = new System.Drawing.Point(0, 0);
            this.cmbPixelFormat.Name = "cmbPixelFormat";
            this.cmbPixelFormat.Size = new System.Drawing.Size(196, 21);
            this.cmbPixelFormat.TabIndex = 0;
            // 
            // DebuggerTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 435);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.panelControls);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(250, 450);
            this.Name = "DebuggerTestForm";
            this.Padding = new System.Windows.Forms.Padding(15, 10, 15, 5);
            this.Text = "Debugger Visualizer Test";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.panelControls.ResumeLayout(false);
            this.panelControls.PerformLayout();
            this.gbFile.ResumeLayout(false);
            this.gbFile.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private KGySoft.WinForms.Controls.AdvancedRadioButton rbBitmap;
        private KGySoft.WinForms.Controls.AdvancedRadioButton rbMetafile;
        private KGySoft.WinForms.Controls.AdvancedRadioButton rbHIcon;
        private KGySoft.WinForms.Controls.AdvancedRadioButton rbManagedIcon;
        private KGySoft.WinForms.Controls.AdvancedRadioButton rbFromFile;
        private KGySoft.WinForms.Controls.AdvancedTextBox txtFile;
        private KGySoft.WinForms.Controls.AdvancedButton btnViewByClassicDebugger;
        private KGySoft.WinForms.Controls.AdvancedButton btnViewDirect;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Panel panelControls;
        private KGySoft.WinForms.Controls.AdvancedRadioButton rbGraphicsBitmap;
        private KGySoft.WinForms.Controls.AdvancedRadioButton rbGraphicsHwnd;
        private System.Windows.Forms.GroupBox gbFile;
        private KGySoft.WinForms.Controls.AdvancedRadioButton rbBitmapData;
        private KGySoft.WinForms.Controls.AdvancedRadioButton rbPalette;
        private KGySoft.WinForms.Controls.AdvancedRadioButton rbColor;
        private KGySoft.WinForms.Controls.AdvancedRadioButton rbAsIcon;
        private KGySoft.WinForms.Controls.AdvancedRadioButton rbAsMetafile;
        private KGySoft.WinForms.Controls.AdvancedRadioButton rbAsBitmap;
        private KGySoft.WinForms.Controls.AdvancedRadioButton rbAsImage;
        private KGySoft.WinForms.Controls.AdvancedComboBox cmbPixelFormat;
        private KGySoft.WinForms.Controls.AdvancedCheckBox chbAsImage;
        private KGySoft.WinForms.Controls.AdvancedCheckBox chbAsReadOnly;
        private KGySoft.WinForms.Controls.AdvancedButton btnViewByExtensionDebugger;
    }
}