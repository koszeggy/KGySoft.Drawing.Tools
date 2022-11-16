using System.Windows.Forms;

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
            this.btnViewDirect = new System.Windows.Forms.Button();
            this.btnViewByDebugger = new System.Windows.Forms.Button();
            this.txtFile = new System.Windows.Forms.TextBox();
            this.rbFromFile = new System.Windows.Forms.RadioButton();
            this.rbManagedIcon = new System.Windows.Forms.RadioButton();
            this.rbHIcon = new System.Windows.Forms.RadioButton();
            this.rbMetafile = new System.Windows.Forms.RadioButton();
            this.rbBitmap = new System.Windows.Forms.RadioButton();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.panelControls = new System.Windows.Forms.Panel();
            this.chbAsReadOnly = new System.Windows.Forms.CheckBox();
            this.gbFile = new System.Windows.Forms.GroupBox();
            this.rbAsIcon = new System.Windows.Forms.RadioButton();
            this.rbAsMetafile = new System.Windows.Forms.RadioButton();
            this.rbAsBitmap = new System.Windows.Forms.RadioButton();
            this.rbAsImage = new System.Windows.Forms.RadioButton();
            this.rbColor = new System.Windows.Forms.RadioButton();
            this.rbPalette = new System.Windows.Forms.RadioButton();
            this.rbBitmapData = new System.Windows.Forms.RadioButton();
            this.rbGraphicsHwnd = new System.Windows.Forms.RadioButton();
            this.rbGraphicsBitmap = new System.Windows.Forms.RadioButton();
            this.chbAsImage = new System.Windows.Forms.CheckBox();
            this.cmbPixelFormat = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.panelControls.SuspendLayout();
            this.gbFile.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnViewDirect
            // 
            this.btnViewDirect.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnViewDirect.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnViewDirect.Location = new System.Drawing.Point(0, 347);
            this.btnViewDirect.Name = "btnViewDirect";
            this.btnViewDirect.Size = new System.Drawing.Size(196, 24);
            this.btnViewDirect.TabIndex = 14;
            this.btnViewDirect.Text = "View Directly";
            this.btnViewDirect.UseVisualStyleBackColor = true;
            // 
            // btnViewByDebugger
            // 
            this.btnViewByDebugger.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnViewByDebugger.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnViewByDebugger.Location = new System.Drawing.Point(0, 371);
            this.btnViewByDebugger.Name = "btnViewByDebugger";
            this.btnViewByDebugger.Size = new System.Drawing.Size(196, 24);
            this.btnViewByDebugger.TabIndex = 15;
            this.btnViewByDebugger.Text = "View by Debugger";
            this.btnViewByDebugger.UseVisualStyleBackColor = true;
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
            this.rbFromFile.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbFromFile.Location = new System.Drawing.Point(0, 201);
            this.rbFromFile.Name = "rbFromFile";
            this.rbFromFile.Size = new System.Drawing.Size(196, 18);
            this.rbFromFile.TabIndex = 11;
            this.rbFromFile.Text = "Image From File";
            this.rbFromFile.UseVisualStyleBackColor = true;
            // 
            // rbManagedIcon
            // 
            this.rbManagedIcon.AutoSize = true;
            this.rbManagedIcon.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbManagedIcon.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbManagedIcon.Location = new System.Drawing.Point(0, 93);
            this.rbManagedIcon.Name = "rbManagedIcon";
            this.rbManagedIcon.Size = new System.Drawing.Size(196, 18);
            this.rbManagedIcon.TabIndex = 5;
            this.rbManagedIcon.Text = "Managed Icon";
            this.rbManagedIcon.UseVisualStyleBackColor = true;
            // 
            // rbHIcon
            // 
            this.rbHIcon.AutoSize = true;
            this.rbHIcon.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbHIcon.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbHIcon.Location = new System.Drawing.Point(0, 75);
            this.rbHIcon.Name = "rbHIcon";
            this.rbHIcon.Size = new System.Drawing.Size(196, 18);
            this.rbHIcon.TabIndex = 4;
            this.rbHIcon.Text = "Icon from Handle";
            this.rbHIcon.UseVisualStyleBackColor = true;
            // 
            // rbMetafile
            // 
            this.rbMetafile.AutoSize = true;
            this.rbMetafile.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbMetafile.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbMetafile.Location = new System.Drawing.Point(0, 57);
            this.rbMetafile.Name = "rbMetafile";
            this.rbMetafile.Size = new System.Drawing.Size(196, 18);
            this.rbMetafile.TabIndex = 3;
            this.rbMetafile.Text = "Metafile";
            this.rbMetafile.UseVisualStyleBackColor = true;
            // 
            // rbBitmap
            // 
            this.rbBitmap.AutoSize = true;
            this.rbBitmap.Checked = true;
            this.rbBitmap.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbBitmap.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbBitmap.Location = new System.Drawing.Point(0, 39);
            this.rbBitmap.Name = "rbBitmap";
            this.rbBitmap.Size = new System.Drawing.Size(196, 18);
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
            this.pictureBox.Size = new System.Drawing.Size(195, 396);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 14;
            this.pictureBox.TabStop = false;
            // 
            // panelControls
            // 
            this.panelControls.Controls.Add(this.btnViewByDebugger);
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
            this.panelControls.Size = new System.Drawing.Size(209, 396);
            this.panelControls.TabIndex = 0;
            // 
            // chbAsReadOnly
            // 
            this.chbAsReadOnly.AutoSize = true;
            this.chbAsReadOnly.Dock = System.Windows.Forms.DockStyle.Top;
            this.chbAsReadOnly.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chbAsReadOnly.Location = new System.Drawing.Point(0, 329);
            this.chbAsReadOnly.Name = "chbAsReadOnly";
            this.chbAsReadOnly.Size = new System.Drawing.Size(196, 18);
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
            this.gbFile.Location = new System.Drawing.Point(0, 219);
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
            this.rbAsIcon.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbAsIcon.Location = new System.Drawing.Point(3, 90);
            this.rbAsIcon.Name = "rbAsIcon";
            this.rbAsIcon.Size = new System.Drawing.Size(190, 18);
            this.rbAsIcon.TabIndex = 4;
            this.rbAsIcon.Text = "As Icon";
            this.rbAsIcon.UseVisualStyleBackColor = true;
            // 
            // rbAsMetafile
            // 
            this.rbAsMetafile.AutoSize = true;
            this.rbAsMetafile.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbAsMetafile.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbAsMetafile.Location = new System.Drawing.Point(3, 72);
            this.rbAsMetafile.Name = "rbAsMetafile";
            this.rbAsMetafile.Size = new System.Drawing.Size(190, 18);
            this.rbAsMetafile.TabIndex = 3;
            this.rbAsMetafile.Text = "As Metafile";
            this.rbAsMetafile.UseVisualStyleBackColor = true;
            // 
            // rbAsBitmap
            // 
            this.rbAsBitmap.AutoSize = true;
            this.rbAsBitmap.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbAsBitmap.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbAsBitmap.Location = new System.Drawing.Point(3, 54);
            this.rbAsBitmap.Name = "rbAsBitmap";
            this.rbAsBitmap.Size = new System.Drawing.Size(190, 18);
            this.rbAsBitmap.TabIndex = 2;
            this.rbAsBitmap.Text = "As Bitmap";
            this.rbAsBitmap.UseVisualStyleBackColor = true;
            // 
            // rbAsImage
            // 
            this.rbAsImage.AutoSize = true;
            this.rbAsImage.Checked = true;
            this.rbAsImage.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbAsImage.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbAsImage.Location = new System.Drawing.Point(3, 36);
            this.rbAsImage.Name = "rbAsImage";
            this.rbAsImage.Size = new System.Drawing.Size(190, 18);
            this.rbAsImage.TabIndex = 1;
            this.rbAsImage.TabStop = true;
            this.rbAsImage.Text = "As Image";
            this.rbAsImage.UseVisualStyleBackColor = true;
            // 
            // rbColor
            // 
            this.rbColor.AutoSize = true;
            this.rbColor.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbColor.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbColor.Location = new System.Drawing.Point(0, 183);
            this.rbColor.Name = "rbColor";
            this.rbColor.Size = new System.Drawing.Size(196, 18);
            this.rbColor.TabIndex = 10;
            this.rbColor.Text = "Single Color";
            this.rbColor.UseVisualStyleBackColor = true;
            // 
            // rbPalette
            // 
            this.rbPalette.AutoSize = true;
            this.rbPalette.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbPalette.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbPalette.Location = new System.Drawing.Point(0, 165);
            this.rbPalette.Name = "rbPalette";
            this.rbPalette.Size = new System.Drawing.Size(196, 18);
            this.rbPalette.TabIndex = 9;
            this.rbPalette.Text = "Palette";
            this.rbPalette.UseVisualStyleBackColor = true;
            // 
            // rbBitmapData
            // 
            this.rbBitmapData.AutoSize = true;
            this.rbBitmapData.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbBitmapData.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbBitmapData.Location = new System.Drawing.Point(0, 147);
            this.rbBitmapData.Name = "rbBitmapData";
            this.rbBitmapData.Size = new System.Drawing.Size(196, 18);
            this.rbBitmapData.TabIndex = 8;
            this.rbBitmapData.Text = "BitmapData";
            this.rbBitmapData.UseVisualStyleBackColor = true;
            // 
            // rbGraphicsHwnd
            // 
            this.rbGraphicsHwnd.AutoSize = true;
            this.rbGraphicsHwnd.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbGraphicsHwnd.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbGraphicsHwnd.Location = new System.Drawing.Point(0, 129);
            this.rbGraphicsHwnd.Name = "rbGraphicsHwnd";
            this.rbGraphicsHwnd.Size = new System.Drawing.Size(196, 18);
            this.rbGraphicsHwnd.TabIndex = 7;
            this.rbGraphicsHwnd.Text = "Graphics from Handle";
            this.rbGraphicsHwnd.UseVisualStyleBackColor = true;
            // 
            // rbGraphicsBitmap
            // 
            this.rbGraphicsBitmap.AutoSize = true;
            this.rbGraphicsBitmap.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbGraphicsBitmap.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbGraphicsBitmap.Location = new System.Drawing.Point(0, 111);
            this.rbGraphicsBitmap.Name = "rbGraphicsBitmap";
            this.rbGraphicsBitmap.Size = new System.Drawing.Size(196, 18);
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
            this.chbAsImage.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chbAsImage.Location = new System.Drawing.Point(0, 21);
            this.chbAsImage.Name = "chbAsImage";
            this.chbAsImage.Size = new System.Drawing.Size(196, 18);
            this.chbAsImage.TabIndex = 1;
            this.chbAsImage.Text = "As Image";
            this.chbAsImage.UseVisualStyleBackColor = true;
            // 
            // cmbPixelFormat
            // 
            this.cmbPixelFormat.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmbPixelFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPixelFormat.FlatStyle = System.Windows.Forms.FlatStyle.System;
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
            this.ClientSize = new System.Drawing.Size(434, 411);
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

        private RadioButton rbBitmap;
        private RadioButton rbMetafile;
        private RadioButton rbHIcon;
        private RadioButton rbManagedIcon;
        private RadioButton rbFromFile;
        private TextBox txtFile;
        private Button btnViewByDebugger;
        private Button btnViewDirect;
        private PictureBox pictureBox;
        private Panel panelControls;
        private RadioButton rbGraphicsBitmap;
        private RadioButton rbGraphicsHwnd;
        private GroupBox gbFile;
        private RadioButton rbBitmapData;
        private RadioButton rbPalette;
        private RadioButton rbColor;
        private RadioButton rbAsIcon;
        private RadioButton rbAsMetafile;
        private RadioButton rbAsBitmap;
        private RadioButton rbAsImage;
        private ComboBox cmbPixelFormat;
        private CheckBox chbAsImage;
        private CheckBox chbAsReadOnly;
    }
}