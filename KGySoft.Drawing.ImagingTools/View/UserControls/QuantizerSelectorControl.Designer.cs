namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    partial class QuantizerSelectorControl
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
            this.cmbQuantizer = new System.Windows.Forms.ComboBox();
            this.gbParameters = new System.Windows.Forms.GroupBox();
            this.pgParameters = new System.Windows.Forms.PropertyGrid();
            this.gbParameters.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbQuantizer
            // 
            this.cmbQuantizer.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmbQuantizer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbQuantizer.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbQuantizer.FormattingEnabled = true;
            this.cmbQuantizer.Location = new System.Drawing.Point(0, 0);
            this.cmbQuantizer.Name = "cmbQuantizer";
            this.cmbQuantizer.Size = new System.Drawing.Size(150, 21);
            this.cmbQuantizer.TabIndex = 1;
            // 
            // gbParameters
            // 
            this.gbParameters.Controls.Add(this.pgParameters);
            this.gbParameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbParameters.Location = new System.Drawing.Point(0, 21);
            this.gbParameters.Name = "gbParameters";
            this.gbParameters.Size = new System.Drawing.Size(150, 129);
            this.gbParameters.TabIndex = 2;
            this.gbParameters.TabStop = false;
            this.gbParameters.Text = "gbParameters";
            // 
            // pgParameters
            // 
            this.pgParameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgParameters.HelpVisible = false;
            this.pgParameters.Location = new System.Drawing.Point(3, 16);
            this.pgParameters.Name = "pgParameters";
            this.pgParameters.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.pgParameters.Size = new System.Drawing.Size(144, 110);
            this.pgParameters.TabIndex = 0;
            this.pgParameters.ToolbarVisible = false;
            // 
            // QuantizerSelectorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbParameters);
            this.Controls.Add(this.cmbQuantizer);
            this.Name = "QuantizerSelectorControl";
            this.gbParameters.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ComboBox cmbQuantizer;
        private System.Windows.Forms.GroupBox gbParameters;
        private System.Windows.Forms.PropertyGrid pgParameters;
    }
}
