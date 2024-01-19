using System.Windows.Forms;

namespace KGySoft.Drawing.DebuggerVisualizers.Core.Test.View
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
            this.rbManagedBitmapData = new System.Windows.Forms.RadioButton();
            this.pbPreview = new System.Windows.Forms.PictureBox();
            this.panelControls = new System.Windows.Forms.Panel();
            this.gbFile = new System.Windows.Forms.GroupBox();
            this.rbAsManaged = new System.Windows.Forms.RadioButton();
            this.rbAsNative = new System.Windows.Forms.RadioButton();
            this.rbPColorF = new System.Windows.Forms.RadioButton();
            this.rbColorF = new System.Windows.Forms.RadioButton();
            this.rbPColor64 = new System.Windows.Forms.RadioButton();
            this.rbColor64 = new System.Windows.Forms.RadioButton();
            this.rbPColor32 = new System.Windows.Forms.RadioButton();
            this.rbColor32 = new System.Windows.Forms.RadioButton();
            this.rbPalette = new System.Windows.Forms.RadioButton();
            this.cmbPixelFormat = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).BeginInit();
            this.panelControls.SuspendLayout();
            this.gbFile.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnViewDirect
            // 
            this.btnViewDirect.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnViewDirect.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnViewDirect.Location = new System.Drawing.Point(0, 259);
            this.btnViewDirect.Name = "btnViewDirect";
            this.btnViewDirect.Size = new System.Drawing.Size(196, 24);
            this.btnViewDirect.TabIndex = 11;
            this.btnViewDirect.Text = "View Directly";
            this.btnViewDirect.UseVisualStyleBackColor = true;
            // 
            // btnViewByDebugger
            // 
            this.btnViewByDebugger.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnViewByDebugger.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnViewByDebugger.Location = new System.Drawing.Point(0, 283);
            this.btnViewByDebugger.Name = "btnViewByDebugger";
            this.btnViewByDebugger.Size = new System.Drawing.Size(196, 24);
            this.btnViewByDebugger.TabIndex = 12;
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
            this.rbFromFile.Location = new System.Drawing.Point(0, 165);
            this.rbFromFile.Name = "rbFromFile";
            this.rbFromFile.Size = new System.Drawing.Size(196, 18);
            this.rbFromFile.TabIndex = 9;
            this.rbFromFile.Text = "BitmapData From File";
            this.rbFromFile.UseVisualStyleBackColor = true;
            // 
            // rbManagedBitmapData
            // 
            this.rbManagedBitmapData.AutoSize = true;
            this.rbManagedBitmapData.Checked = true;
            this.rbManagedBitmapData.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbManagedBitmapData.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbManagedBitmapData.Location = new System.Drawing.Point(0, 21);
            this.rbManagedBitmapData.Name = "rbManagedBitmapData";
            this.rbManagedBitmapData.Size = new System.Drawing.Size(196, 18);
            this.rbManagedBitmapData.TabIndex = 1;
            this.rbManagedBitmapData.TabStop = true;
            this.rbManagedBitmapData.Text = "Managed BitmapData";
            this.rbManagedBitmapData.UseVisualStyleBackColor = true;
            // 
            // pbPreview
            // 
            this.pbPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pbPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbPreview.Location = new System.Drawing.Point(224, 10);
            this.pbPreview.Name = "pbPreview";
            this.pbPreview.Size = new System.Drawing.Size(159, 306);
            this.pbPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbPreview.TabIndex = 14;
            this.pbPreview.TabStop = false;
            // 
            // panelControls
            // 
            this.panelControls.Controls.Add(this.btnViewByDebugger);
            this.panelControls.Controls.Add(this.btnViewDirect);
            this.panelControls.Controls.Add(this.gbFile);
            this.panelControls.Controls.Add(this.rbFromFile);
            this.panelControls.Controls.Add(this.rbPColorF);
            this.panelControls.Controls.Add(this.rbColorF);
            this.panelControls.Controls.Add(this.rbPColor64);
            this.panelControls.Controls.Add(this.rbColor64);
            this.panelControls.Controls.Add(this.rbPColor32);
            this.panelControls.Controls.Add(this.rbColor32);
            this.panelControls.Controls.Add(this.rbPalette);
            this.panelControls.Controls.Add(this.rbManagedBitmapData);
            this.panelControls.Controls.Add(this.cmbPixelFormat);
            this.panelControls.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelControls.Location = new System.Drawing.Point(15, 10);
            this.panelControls.Name = "panelControls";
            this.panelControls.Padding = new System.Windows.Forms.Padding(0, 0, 13, 0);
            this.panelControls.Size = new System.Drawing.Size(209, 306);
            this.panelControls.TabIndex = 0;
            // 
            // gbFile
            // 
            this.gbFile.Controls.Add(this.rbAsManaged);
            this.gbFile.Controls.Add(this.rbAsNative);
            this.gbFile.Controls.Add(this.txtFile);
            this.gbFile.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbFile.Enabled = false;
            this.gbFile.Location = new System.Drawing.Point(0, 183);
            this.gbFile.Name = "gbFile";
            this.gbFile.Size = new System.Drawing.Size(196, 76);
            this.gbFile.TabIndex = 10;
            this.gbFile.TabStop = false;
            this.gbFile.Text = "File Details";
            // 
            // rbAsManaged
            // 
            this.rbAsManaged.AutoSize = true;
            this.rbAsManaged.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbAsManaged.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbAsManaged.Location = new System.Drawing.Point(3, 54);
            this.rbAsManaged.Name = "rbAsManaged";
            this.rbAsManaged.Size = new System.Drawing.Size(190, 18);
            this.rbAsManaged.TabIndex = 2;
            this.rbAsManaged.Text = "As Managed";
            this.rbAsManaged.UseVisualStyleBackColor = true;
            // 
            // rbAsNative
            // 
            this.rbAsNative.AutoSize = true;
            this.rbAsNative.Checked = true;
            this.rbAsNative.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbAsNative.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbAsNative.Location = new System.Drawing.Point(3, 36);
            this.rbAsNative.Name = "rbAsNative";
            this.rbAsNative.Size = new System.Drawing.Size(190, 18);
            this.rbAsNative.TabIndex = 1;
            this.rbAsNative.TabStop = true;
            this.rbAsNative.Text = "As Native";
            this.rbAsNative.UseVisualStyleBackColor = true;
            // 
            // rbPColorF
            // 
            this.rbPColorF.AutoSize = true;
            this.rbPColorF.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbPColorF.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbPColorF.Location = new System.Drawing.Point(0, 147);
            this.rbPColorF.Name = "rbPColorF";
            this.rbPColorF.Size = new System.Drawing.Size(196, 18);
            this.rbPColorF.TabIndex = 8;
            this.rbPColorF.Text = "PColorF";
            this.rbPColorF.UseVisualStyleBackColor = true;
            // 
            // rbColorF
            // 
            this.rbColorF.AutoSize = true;
            this.rbColorF.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbColorF.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbColorF.Location = new System.Drawing.Point(0, 129);
            this.rbColorF.Name = "rbColorF";
            this.rbColorF.Size = new System.Drawing.Size(196, 18);
            this.rbColorF.TabIndex = 7;
            this.rbColorF.Text = "ColorF";
            this.rbColorF.UseVisualStyleBackColor = true;
            // 
            // rbPColor64
            // 
            this.rbPColor64.AutoSize = true;
            this.rbPColor64.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbPColor64.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbPColor64.Location = new System.Drawing.Point(0, 111);
            this.rbPColor64.Name = "rbPColor64";
            this.rbPColor64.Size = new System.Drawing.Size(196, 18);
            this.rbPColor64.TabIndex = 6;
            this.rbPColor64.Text = "PColor64";
            this.rbPColor64.UseVisualStyleBackColor = true;
            // 
            // rbColor64
            // 
            this.rbColor64.AutoSize = true;
            this.rbColor64.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbColor64.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbColor64.Location = new System.Drawing.Point(0, 93);
            this.rbColor64.Name = "rbColor64";
            this.rbColor64.Size = new System.Drawing.Size(196, 18);
            this.rbColor64.TabIndex = 5;
            this.rbColor64.Text = "Color64";
            this.rbColor64.UseVisualStyleBackColor = true;
            // 
            // rbPColor32
            // 
            this.rbPColor32.AutoSize = true;
            this.rbPColor32.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbPColor32.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbPColor32.Location = new System.Drawing.Point(0, 75);
            this.rbPColor32.Name = "rbPColor32";
            this.rbPColor32.Size = new System.Drawing.Size(196, 18);
            this.rbPColor32.TabIndex = 4;
            this.rbPColor32.Text = "PColor32";
            this.rbPColor32.UseVisualStyleBackColor = true;
            // 
            // rbColor32
            // 
            this.rbColor32.AutoSize = true;
            this.rbColor32.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbColor32.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbColor32.Location = new System.Drawing.Point(0, 57);
            this.rbColor32.Name = "rbColor32";
            this.rbColor32.Size = new System.Drawing.Size(196, 18);
            this.rbColor32.TabIndex = 3;
            this.rbColor32.Text = "Color32";
            this.rbColor32.UseVisualStyleBackColor = true;
            // 
            // rbPalette
            // 
            this.rbPalette.AutoSize = true;
            this.rbPalette.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbPalette.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbPalette.Location = new System.Drawing.Point(0, 39);
            this.rbPalette.Name = "rbPalette";
            this.rbPalette.Size = new System.Drawing.Size(196, 18);
            this.rbPalette.TabIndex = 2;
            this.rbPalette.Text = "Palette";
            this.rbPalette.UseVisualStyleBackColor = true;
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
            this.ClientSize = new System.Drawing.Size(398, 321);
            this.Controls.Add(this.pbPreview);
            this.Controls.Add(this.panelControls);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(250, 360);
            this.Name = "DebuggerTestForm";
            this.Padding = new System.Windows.Forms.Padding(15, 10, 15, 5);
            this.Text = "Debugger Visualizer Test for Drawing.Core";
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).EndInit();
            this.panelControls.ResumeLayout(false);
            this.panelControls.PerformLayout();
            this.gbFile.ResumeLayout(false);
            this.gbFile.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private RadioButton rbManagedBitmapData;
        private RadioButton rbFromFile;
        private TextBox txtFile;
        private Button btnViewByDebugger;
        private Button btnViewDirect;
        private PictureBox pbPreview;
        private Panel panelControls;
        private GroupBox gbFile;
        private RadioButton rbPalette;
        private RadioButton rbColor32;
        private RadioButton rbAsManaged;
        private RadioButton rbAsNative;
        private ComboBox cmbPixelFormat;
        private RadioButton rbPColorF;
        private RadioButton rbColorF;
        private RadioButton rbPColor64;
        private RadioButton rbColor64;
        private RadioButton rbPColor32;
    }
}