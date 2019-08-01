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
            this.advancedCheckBox1 = new System.Windows.Forms.CheckBox();
            this.advancedRadioButton7 = new System.Windows.Forms.RadioButton();
            this.advancedRadioButton6 = new System.Windows.Forms.RadioButton();
            this.advancedButton2 = new System.Windows.Forms.Button();
            this.advancedButton1 = new System.Windows.Forms.Button();
            this.ucCustomSelector1 = new System.Windows.Forms.TextBox();
            this.advancedRadioButton5 = new System.Windows.Forms.RadioButton();
            this.advancedRadioButton4 = new System.Windows.Forms.RadioButton();
            this.advancedRadioButton3 = new System.Windows.Forms.RadioButton();
            this.advancedRadioButton2 = new System.Windows.Forms.RadioButton();
            this.rbBitmap32 = new System.Windows.Forms.RadioButton();
            this.advancedButton3 = new System.Windows.Forms.Button();
            this.advancedButton4 = new System.Windows.Forms.Button();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.panelControls = new System.Windows.Forms.Panel();
            this.splitter = new System.Windows.Forms.Splitter();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.panelControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // advancedCheckBox1
            // 
            this.advancedCheckBox1.AutoSize = true;
            this.advancedCheckBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.advancedCheckBox1.Location = new System.Drawing.Point(161, 162);
            this.advancedCheckBox1.Name = "advancedCheckBox1";
            this.advancedCheckBox1.Size = new System.Drawing.Size(68, 18);
            this.advancedCheckBox1.TabIndex = 11;
            this.advancedCheckBox1.Text = "As Icon";
            this.advancedCheckBox1.UseVisualStyleBackColor = true;
            // 
            // advancedRadioButton7
            // 
            this.advancedRadioButton7.AutoSize = true;
            this.advancedRadioButton7.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.advancedRadioButton7.Location = new System.Drawing.Point(31, 139);
            this.advancedRadioButton7.Name = "advancedRadioButton7";
            this.advancedRadioButton7.Size = new System.Drawing.Size(124, 18);
            this.advancedRadioButton7.TabIndex = 10;
            this.advancedRadioButton7.TabStop = true;
            this.advancedRadioButton7.Text = "Memory BMP 4 bpp";
            this.advancedRadioButton7.UseVisualStyleBackColor = true;
            this.advancedRadioButton7.CheckedChanged += new System.EventHandler(this.advancedRadioButton7_CheckedChanged);
            // 
            // advancedRadioButton6
            // 
            this.advancedRadioButton6.AutoSize = true;
            this.advancedRadioButton6.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.advancedRadioButton6.Location = new System.Drawing.Point(31, 116);
            this.advancedRadioButton6.Name = "advancedRadioButton6";
            this.advancedRadioButton6.Size = new System.Drawing.Size(130, 18);
            this.advancedRadioButton6.TabIndex = 9;
            this.advancedRadioButton6.TabStop = true;
            this.advancedRadioButton6.Text = "Memory BMP 16 bpp";
            this.advancedRadioButton6.UseVisualStyleBackColor = true;
            this.advancedRadioButton6.CheckedChanged += new System.EventHandler(this.advancedRadioButton6_CheckedChanged);
            // 
            // advancedButton2
            // 
            this.advancedButton2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.advancedButton2.Location = new System.Drawing.Point(145, 214);
            this.advancedButton2.Name = "advancedButton2";
            this.advancedButton2.Size = new System.Drawing.Size(78, 23);
            this.advancedButton2.TabIndex = 8;
            this.advancedButton2.Text = "View direct";
            this.advancedButton2.UseVisualStyleBackColor = true;
            this.advancedButton2.Click += new System.EventHandler(this.advancedButton2_Click);
            // 
            // advancedButton1
            // 
            this.advancedButton1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.advancedButton1.Location = new System.Drawing.Point(31, 214);
            this.advancedButton1.Name = "advancedButton1";
            this.advancedButton1.Size = new System.Drawing.Size(108, 23);
            this.advancedButton1.TabIndex = 7;
            this.advancedButton1.Text = "View with debugger";
            this.advancedButton1.UseVisualStyleBackColor = true;
            this.advancedButton1.Click += new System.EventHandler(this.advancedButton1_Click);
            // 
            // ucCustomSelector1
            // 
            this.ucCustomSelector1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.ucCustomSelector1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.ucCustomSelector1.Location = new System.Drawing.Point(31, 185);
            this.ucCustomSelector1.Name = "ucCustomSelector1";
            this.ucCustomSelector1.Size = new System.Drawing.Size(192, 20);
            this.ucCustomSelector1.TabIndex = 5;
            // 
            // advancedRadioButton5
            // 
            this.advancedRadioButton5.AutoSize = true;
            this.advancedRadioButton5.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.advancedRadioButton5.Location = new System.Drawing.Point(31, 162);
            this.advancedRadioButton5.Name = "advancedRadioButton5";
            this.advancedRadioButton5.Size = new System.Drawing.Size(114, 18);
            this.advancedRadioButton5.TabIndex = 4;
            this.advancedRadioButton5.TabStop = true;
            this.advancedRadioButton5.Text = "As Image from file";
            this.advancedRadioButton5.UseVisualStyleBackColor = true;
            this.advancedRadioButton5.CheckedChanged += new System.EventHandler(this.advancedRadioButton5_CheckedChanged);
            // 
            // advancedRadioButton4
            // 
            this.advancedRadioButton4.AutoSize = true;
            this.advancedRadioButton4.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.advancedRadioButton4.Location = new System.Drawing.Point(31, 93);
            this.advancedRadioButton4.Name = "advancedRadioButton4";
            this.advancedRadioButton4.Size = new System.Drawing.Size(114, 18);
            this.advancedRadioButton4.TabIndex = 3;
            this.advancedRadioButton4.TabStop = true;
            this.advancedRadioButton4.Text = "As managed Icon";
            this.advancedRadioButton4.UseVisualStyleBackColor = true;
            this.advancedRadioButton4.CheckedChanged += new System.EventHandler(this.advancedRadioButton4_CheckedChanged);
            // 
            // advancedRadioButton3
            // 
            this.advancedRadioButton3.AutoSize = true;
            this.advancedRadioButton3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.advancedRadioButton3.Location = new System.Drawing.Point(31, 70);
            this.advancedRadioButton3.Name = "advancedRadioButton3";
            this.advancedRadioButton3.Size = new System.Drawing.Size(99, 18);
            this.advancedRadioButton3.TabIndex = 2;
            this.advancedRadioButton3.Text = "As native Icon";
            this.advancedRadioButton3.UseVisualStyleBackColor = true;
            this.advancedRadioButton3.CheckedChanged += new System.EventHandler(this.advancedRadioButton3_CheckedChanged);
            // 
            // advancedRadioButton2
            // 
            this.advancedRadioButton2.AutoSize = true;
            this.advancedRadioButton2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.advancedRadioButton2.Location = new System.Drawing.Point(31, 47);
            this.advancedRadioButton2.Name = "advancedRadioButton2";
            this.advancedRadioButton2.Size = new System.Drawing.Size(83, 18);
            this.advancedRadioButton2.TabIndex = 1;
            this.advancedRadioButton2.Text = "As Metafile";
            this.advancedRadioButton2.UseVisualStyleBackColor = true;
            this.advancedRadioButton2.CheckedChanged += new System.EventHandler(this.advancedRadioButton2_CheckedChanged);
            // 
            // rbBitmap32
            // 
            this.rbBitmap32.AutoSize = true;
            this.rbBitmap32.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbBitmap32.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbBitmap32.Location = new System.Drawing.Point(0, 0);
            this.rbBitmap32.Name = "rbBitmap32";
            this.rbBitmap32.Size = new System.Drawing.Size(268, 18);
            this.rbBitmap32.TabIndex = 0;
            this.rbBitmap32.Text = "Memory BMP 32 bpp";
            this.rbBitmap32.UseVisualStyleBackColor = true;
            this.rbBitmap32.CheckedChanged += new System.EventHandler(this.rbBitmap32_CheckedChanged);
            // 
            // advancedButton3
            // 
            this.advancedButton3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.advancedButton3.Location = new System.Drawing.Point(31, 246);
            this.advancedButton3.Name = "advancedButton3";
            this.advancedButton3.Size = new System.Drawing.Size(93, 23);
            this.advancedButton3.TabIndex = 12;
            this.advancedButton3.Text = "Form Graphics";
            this.advancedButton3.UseVisualStyleBackColor = true;
            this.advancedButton3.Click += new System.EventHandler(this.advancedButton3_Click);
            // 
            // advancedButton4
            // 
            this.advancedButton4.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.advancedButton4.Location = new System.Drawing.Point(130, 246);
            this.advancedButton4.Name = "advancedButton4";
            this.advancedButton4.Size = new System.Drawing.Size(93, 23);
            this.advancedButton4.TabIndex = 13;
            this.advancedButton4.Text = "Bitmap graphics";
            this.advancedButton4.UseVisualStyleBackColor = true;
            this.advancedButton4.Click += new System.EventHandler(this.advancedButton4_Click);
            // 
            // pictureBox
            // 
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.pictureBox.Location = new System.Drawing.Point(283, 15);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(173, 287);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 14;
            this.pictureBox.TabStop = false;
            // 
            // panelControls
            // 
            this.panelControls.Controls.Add(this.rbBitmap32);
            this.panelControls.Controls.Add(this.advancedRadioButton2);
            this.panelControls.Controls.Add(this.advancedButton4);
            this.panelControls.Controls.Add(this.advancedRadioButton3);
            this.panelControls.Controls.Add(this.advancedButton3);
            this.panelControls.Controls.Add(this.advancedRadioButton4);
            this.panelControls.Controls.Add(this.advancedCheckBox1);
            this.panelControls.Controls.Add(this.advancedRadioButton5);
            this.panelControls.Controls.Add(this.advancedRadioButton7);
            this.panelControls.Controls.Add(this.ucCustomSelector1);
            this.panelControls.Controls.Add(this.advancedRadioButton6);
            this.panelControls.Controls.Add(this.advancedButton1);
            this.panelControls.Controls.Add(this.advancedButton2);
            this.panelControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControls.Location = new System.Drawing.Point(15, 15);
            this.panelControls.Name = "panelControls";
            this.panelControls.Size = new System.Drawing.Size(268, 287);
            this.panelControls.TabIndex = 15;
            // 
            // splitter
            // 
            this.splitter.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter.Location = new System.Drawing.Point(280, 15);
            this.splitter.Name = "splitter";
            this.splitter.Size = new System.Drawing.Size(3, 287);
            this.splitter.TabIndex = 16;
            this.splitter.TabStop = false;
            // 
            // DebuggerTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(471, 317);
            this.Controls.Add(this.splitter);
            this.Controls.Add(this.panelControls);
            this.Controls.Add(this.pictureBox);
            this.Name = "DebuggerTestForm";
            this.Padding = new System.Windows.Forms.Padding(15);
            this.Text = "Debugger Visualizer Test";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.panelControls.ResumeLayout(false);
            this.panelControls.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private RadioButton rbBitmap32;
        private RadioButton advancedRadioButton2;
        private RadioButton advancedRadioButton3;
        private RadioButton advancedRadioButton4;
        private RadioButton advancedRadioButton5;
        private TextBox ucCustomSelector1;
        private Button advancedButton1;
        private Button advancedButton2;
        private RadioButton advancedRadioButton6;
        private RadioButton advancedRadioButton7;
        private CheckBox advancedCheckBox1;
        private Button advancedButton3;
        private Button advancedButton4;
        private PictureBox pictureBox;
        private Panel panelControls;
        private Splitter splitter;
    }
}