using System.Windows.Forms;

namespace KGySoft.Drawing.DebuggerVisualizers.Test.Forms
{
    partial class DebuggerTestForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rbBitmap4 = new System.Windows.Forms.RadioButton();
            this.rbBitmap16 = new System.Windows.Forms.RadioButton();
            this.btnViewDirect = new System.Windows.Forms.Button();
            this.btnViewByDebugger = new System.Windows.Forms.Button();
            this.tbFile = new System.Windows.Forms.TextBox();
            this.rbFromFile = new System.Windows.Forms.RadioButton();
            this.rbManagedIcon = new System.Windows.Forms.RadioButton();
            this.rbHIcon = new System.Windows.Forms.RadioButton();
            this.rbMetafile = new System.Windows.Forms.RadioButton();
            this.rbBitmap32 = new System.Windows.Forms.RadioButton();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.panelControls = new System.Windows.Forms.Panel();
            this.gbFile = new System.Windows.Forms.GroupBox();
            this.rbAsIcon = new System.Windows.Forms.RadioButton();
            this.rbAsMetafile = new System.Windows.Forms.RadioButton();
            this.rbAsBitmap = new System.Windows.Forms.RadioButton();
            this.rbAsImage = new System.Windows.Forms.RadioButton();
            this.rbColor = new System.Windows.Forms.RadioButton();
            this.rbPalette2 = new System.Windows.Forms.RadioButton();
            this.rbPalette256 = new System.Windows.Forms.RadioButton();
            this.rbBitmapData8 = new System.Windows.Forms.RadioButton();
            this.rbBitmapData32 = new System.Windows.Forms.RadioButton();
            this.rbGraphicsHwnd = new System.Windows.Forms.RadioButton();
            this.rbGraphicsBitmap = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.panelControls.SuspendLayout();
            this.gbFile.SuspendLayout();
            this.SuspendLayout();
            // 
            // rbBitmap4
            // 
            this.rbBitmap4.AutoSize = true;
            this.rbBitmap4.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbBitmap4.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbBitmap4.Location = new System.Drawing.Point(0, 36);
            this.rbBitmap4.Name = "rbBitmap4";
            this.rbBitmap4.Size = new System.Drawing.Size(209, 18);
            this.rbBitmap4.TabIndex = 10;
            this.rbBitmap4.TabStop = true;
            this.rbBitmap4.Text = "Memory BMP 4 bpp";
            this.rbBitmap4.UseVisualStyleBackColor = true;
            // 
            // rbBitmap16
            // 
            this.rbBitmap16.AutoSize = true;
            this.rbBitmap16.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbBitmap16.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbBitmap16.Location = new System.Drawing.Point(0, 18);
            this.rbBitmap16.Name = "rbBitmap16";
            this.rbBitmap16.Size = new System.Drawing.Size(209, 18);
            this.rbBitmap16.TabIndex = 9;
            this.rbBitmap16.TabStop = true;
            this.rbBitmap16.Text = "Memory BMP 16 bpp";
            this.rbBitmap16.UseVisualStyleBackColor = true;
            // 
            // btnViewDirect
            // 
            this.btnViewDirect.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnViewDirect.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnViewDirect.Location = new System.Drawing.Point(0, 363);
            this.btnViewDirect.Name = "btnViewDirect";
            this.btnViewDirect.Size = new System.Drawing.Size(209, 24);
            this.btnViewDirect.TabIndex = 8;
            this.btnViewDirect.Text = "View Directly";
            this.btnViewDirect.UseVisualStyleBackColor = true;
            // 
            // btnViewByDebugger
            // 
            this.btnViewByDebugger.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnViewByDebugger.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnViewByDebugger.Location = new System.Drawing.Point(0, 387);
            this.btnViewByDebugger.Name = "btnViewByDebugger";
            this.btnViewByDebugger.Size = new System.Drawing.Size(209, 24);
            this.btnViewByDebugger.TabIndex = 7;
            this.btnViewByDebugger.Text = "View by Debugger Mock";
            this.btnViewByDebugger.UseVisualStyleBackColor = true;
            // 
            // tbFile
            // 
            this.tbFile.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.tbFile.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.tbFile.Dock = System.Windows.Forms.DockStyle.Top;
            this.tbFile.Location = new System.Drawing.Point(3, 16);
            this.tbFile.Name = "tbFile";
            this.tbFile.Size = new System.Drawing.Size(203, 20);
            this.tbFile.TabIndex = 5;
            // 
            // rbFromFile
            // 
            this.rbFromFile.AutoSize = true;
            this.rbFromFile.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbFromFile.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbFromFile.Location = new System.Drawing.Point(0, 234);
            this.rbFromFile.Name = "rbFromFile";
            this.rbFromFile.Size = new System.Drawing.Size(209, 18);
            this.rbFromFile.TabIndex = 4;
            this.rbFromFile.TabStop = true;
            this.rbFromFile.Text = "Image From File";
            this.rbFromFile.UseVisualStyleBackColor = true;
            // 
            // rbManagedIcon
            // 
            this.rbManagedIcon.AutoSize = true;
            this.rbManagedIcon.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbManagedIcon.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbManagedIcon.Location = new System.Drawing.Point(0, 90);
            this.rbManagedIcon.Name = "rbManagedIcon";
            this.rbManagedIcon.Size = new System.Drawing.Size(209, 18);
            this.rbManagedIcon.TabIndex = 3;
            this.rbManagedIcon.TabStop = true;
            this.rbManagedIcon.Text = "Managed Icon";
            this.rbManagedIcon.UseVisualStyleBackColor = true;
            // 
            // rbHIcon
            // 
            this.rbHIcon.AutoSize = true;
            this.rbHIcon.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbHIcon.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbHIcon.Location = new System.Drawing.Point(0, 72);
            this.rbHIcon.Name = "rbHIcon";
            this.rbHIcon.Size = new System.Drawing.Size(209, 18);
            this.rbHIcon.TabIndex = 2;
            this.rbHIcon.Text = "Icon from Handle";
            this.rbHIcon.UseVisualStyleBackColor = true;
            // 
            // rbMetafile
            // 
            this.rbMetafile.AutoSize = true;
            this.rbMetafile.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbMetafile.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbMetafile.Location = new System.Drawing.Point(0, 54);
            this.rbMetafile.Name = "rbMetafile";
            this.rbMetafile.Size = new System.Drawing.Size(209, 18);
            this.rbMetafile.TabIndex = 1;
            this.rbMetafile.Text = "Metafile";
            this.rbMetafile.UseVisualStyleBackColor = true;
            // 
            // rbBitmap32
            // 
            this.rbBitmap32.AutoSize = true;
            this.rbBitmap32.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbBitmap32.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbBitmap32.Location = new System.Drawing.Point(0, 0);
            this.rbBitmap32.Name = "rbBitmap32";
            this.rbBitmap32.Size = new System.Drawing.Size(209, 18);
            this.rbBitmap32.TabIndex = 0;
            this.rbBitmap32.Text = "Memory BMP 32 bpp";
            this.rbBitmap32.UseVisualStyleBackColor = true;
            // 
            // pictureBox
            // 
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(224, 15);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(195, 411);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 14;
            this.pictureBox.TabStop = false;
            // 
            // panelControls
            // 
            this.panelControls.Controls.Add(this.btnViewByDebugger);
            this.panelControls.Controls.Add(this.btnViewDirect);
            this.panelControls.Controls.Add(this.gbFile);
            this.panelControls.Controls.Add(this.rbFromFile);
            this.panelControls.Controls.Add(this.rbColor);
            this.panelControls.Controls.Add(this.rbPalette2);
            this.panelControls.Controls.Add(this.rbPalette256);
            this.panelControls.Controls.Add(this.rbBitmapData8);
            this.panelControls.Controls.Add(this.rbBitmapData32);
            this.panelControls.Controls.Add(this.rbGraphicsHwnd);
            this.panelControls.Controls.Add(this.rbGraphicsBitmap);
            this.panelControls.Controls.Add(this.rbManagedIcon);
            this.panelControls.Controls.Add(this.rbHIcon);
            this.panelControls.Controls.Add(this.rbMetafile);
            this.panelControls.Controls.Add(this.rbBitmap4);
            this.panelControls.Controls.Add(this.rbBitmap16);
            this.panelControls.Controls.Add(this.rbBitmap32);
            this.panelControls.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelControls.Location = new System.Drawing.Point(15, 15);
            this.panelControls.Name = "panelControls";
            this.panelControls.Size = new System.Drawing.Size(209, 411);
            this.panelControls.TabIndex = 15;
            // 
            // gbFile
            // 
            this.gbFile.AutoSize = true;
            this.gbFile.Controls.Add(this.rbAsIcon);
            this.gbFile.Controls.Add(this.rbAsMetafile);
            this.gbFile.Controls.Add(this.rbAsBitmap);
            this.gbFile.Controls.Add(this.rbAsImage);
            this.gbFile.Controls.Add(this.tbFile);
            this.gbFile.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbFile.Enabled = false;
            this.gbFile.Location = new System.Drawing.Point(0, 252);
            this.gbFile.Name = "gbFile";
            this.gbFile.Size = new System.Drawing.Size(209, 111);
            this.gbFile.TabIndex = 17;
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
            this.rbAsIcon.Size = new System.Drawing.Size(203, 18);
            this.rbAsIcon.TabIndex = 9;
            this.rbAsIcon.TabStop = true;
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
            this.rbAsMetafile.Size = new System.Drawing.Size(203, 18);
            this.rbAsMetafile.TabIndex = 8;
            this.rbAsMetafile.TabStop = true;
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
            this.rbAsBitmap.Size = new System.Drawing.Size(203, 18);
            this.rbAsBitmap.TabIndex = 7;
            this.rbAsBitmap.TabStop = true;
            this.rbAsBitmap.Text = "As Bitmap";
            this.rbAsBitmap.UseVisualStyleBackColor = true;
            // 
            // rbAsImage
            // 
            this.rbAsImage.AutoSize = true;
            this.rbAsImage.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbAsImage.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbAsImage.Location = new System.Drawing.Point(3, 36);
            this.rbAsImage.Name = "rbAsImage";
            this.rbAsImage.Size = new System.Drawing.Size(203, 18);
            this.rbAsImage.TabIndex = 6;
            this.rbAsImage.TabStop = true;
            this.rbAsImage.Text = "As Image";
            this.rbAsImage.UseVisualStyleBackColor = true;
            // 
            // rbColor
            // 
            this.rbColor.AutoSize = true;
            this.rbColor.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbColor.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbColor.Location = new System.Drawing.Point(0, 216);
            this.rbColor.Name = "rbColor";
            this.rbColor.Size = new System.Drawing.Size(209, 18);
            this.rbColor.TabIndex = 30;
            this.rbColor.TabStop = true;
            this.rbColor.Text = "Single Color";
            this.rbColor.UseVisualStyleBackColor = true;
            // 
            // rbPalette2
            // 
            this.rbPalette2.AutoSize = true;
            this.rbPalette2.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbPalette2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbPalette2.Location = new System.Drawing.Point(0, 198);
            this.rbPalette2.Name = "rbPalette2";
            this.rbPalette2.Size = new System.Drawing.Size(209, 18);
            this.rbPalette2.TabIndex = 25;
            this.rbPalette2.TabStop = true;
            this.rbPalette2.Text = "Palette 2 Colors";
            this.rbPalette2.UseVisualStyleBackColor = true;
            // 
            // rbPalette256
            // 
            this.rbPalette256.AutoSize = true;
            this.rbPalette256.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbPalette256.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbPalette256.Location = new System.Drawing.Point(0, 180);
            this.rbPalette256.Name = "rbPalette256";
            this.rbPalette256.Size = new System.Drawing.Size(209, 18);
            this.rbPalette256.TabIndex = 20;
            this.rbPalette256.TabStop = true;
            this.rbPalette256.Text = "Palette 256 Colors";
            this.rbPalette256.UseVisualStyleBackColor = true;
            // 
            // rbBitmapData8
            // 
            this.rbBitmapData8.AutoSize = true;
            this.rbBitmapData8.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbBitmapData8.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbBitmapData8.Location = new System.Drawing.Point(0, 162);
            this.rbBitmapData8.Name = "rbBitmapData8";
            this.rbBitmapData8.Size = new System.Drawing.Size(209, 18);
            this.rbBitmapData8.TabIndex = 19;
            this.rbBitmapData8.TabStop = true;
            this.rbBitmapData8.Text = "BitmapData 8 bpp";
            this.rbBitmapData8.UseVisualStyleBackColor = true;
            // 
            // rbBitmapData32
            // 
            this.rbBitmapData32.AutoSize = true;
            this.rbBitmapData32.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbBitmapData32.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbBitmapData32.Location = new System.Drawing.Point(0, 144);
            this.rbBitmapData32.Name = "rbBitmapData32";
            this.rbBitmapData32.Size = new System.Drawing.Size(209, 18);
            this.rbBitmapData32.TabIndex = 18;
            this.rbBitmapData32.TabStop = true;
            this.rbBitmapData32.Text = "BitmapData 32 bpp";
            this.rbBitmapData32.UseVisualStyleBackColor = true;
            // 
            // rbGraphicsHwnd
            // 
            this.rbGraphicsHwnd.AutoSize = true;
            this.rbGraphicsHwnd.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbGraphicsHwnd.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbGraphicsHwnd.Location = new System.Drawing.Point(0, 126);
            this.rbGraphicsHwnd.Name = "rbGraphicsHwnd";
            this.rbGraphicsHwnd.Size = new System.Drawing.Size(209, 18);
            this.rbGraphicsHwnd.TabIndex = 15;
            this.rbGraphicsHwnd.TabStop = true;
            this.rbGraphicsHwnd.Text = "Graphics from Control";
            this.rbGraphicsHwnd.UseVisualStyleBackColor = true;
            // 
            // rbGraphicsBitmap
            // 
            this.rbGraphicsBitmap.AutoSize = true;
            this.rbGraphicsBitmap.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbGraphicsBitmap.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbGraphicsBitmap.Location = new System.Drawing.Point(0, 108);
            this.rbGraphicsBitmap.Name = "rbGraphicsBitmap";
            this.rbGraphicsBitmap.Size = new System.Drawing.Size(209, 18);
            this.rbGraphicsBitmap.TabIndex = 14;
            this.rbGraphicsBitmap.TabStop = true;
            this.rbGraphicsBitmap.Text = "Graphics from Bitmap";
            this.rbGraphicsBitmap.UseVisualStyleBackColor = true;
            this.rbGraphicsBitmap.CheckedChanged += new System.EventHandler(this.rbGraphicsBitmap_CheckedChanged);
            // 
            // DebuggerTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 441);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.panelControls);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(250, 480);
            this.Name = "DebuggerTestForm";
            this.Padding = new System.Windows.Forms.Padding(15);
            this.Text = "Debugger Visualizer Test";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.panelControls.ResumeLayout(false);
            this.panelControls.PerformLayout();
            this.gbFile.ResumeLayout(false);
            this.gbFile.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private RadioButton rbBitmap32;
        private RadioButton rbMetafile;
        private RadioButton rbHIcon;
        private RadioButton rbManagedIcon;
        private RadioButton rbFromFile;
        private TextBox tbFile;
        private Button btnViewByDebugger;
        private Button btnViewDirect;
        private RadioButton rbBitmap16;
        private RadioButton rbBitmap4;
        private PictureBox pictureBox;
        private Panel panelControls;
        private RadioButton rbGraphicsBitmap;
        private RadioButton rbGraphicsHwnd;
        private GroupBox gbFile;
        private RadioButton rbBitmapData32;
        private RadioButton rbPalette256;
        private RadioButton rbBitmapData8;
        private RadioButton rbColor;
        private RadioButton rbPalette2;
        private RadioButton rbAsIcon;
        private RadioButton rbAsMetafile;
        private RadioButton rbAsBitmap;
        private RadioButton rbAsImage;
    }
}