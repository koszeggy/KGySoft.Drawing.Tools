namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    partial class DithererSelectorControl
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
            this.gbParameters = new System.Windows.Forms.GroupBox();
            this.pgParameters = new System.Windows.Forms.PropertyGrid();
            this.cmbDitherer = new System.Windows.Forms.ComboBox();
            this.lblDitherer = new System.Windows.Forms.Label();
            this.gbParameters.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbParameters
            // 
            this.gbParameters.Controls.Add(this.pgParameters);
            this.gbParameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbParameters.Location = new System.Drawing.Point(0, 34);
            this.gbParameters.Name = "gbParameters";
            this.gbParameters.Size = new System.Drawing.Size(150, 116);
            this.gbParameters.TabIndex = 5;
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
            this.pgParameters.Size = new System.Drawing.Size(144, 97);
            this.pgParameters.TabIndex = 0;
            this.pgParameters.ToolbarVisible = false;
            // 
            // cmbDitherer
            // 
            this.cmbDitherer.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmbDitherer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDitherer.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbDitherer.FormattingEnabled = true;
            this.cmbDitherer.Location = new System.Drawing.Point(0, 13);
            this.cmbDitherer.Name = "cmbDitherer";
            this.cmbDitherer.Size = new System.Drawing.Size(150, 21);
            this.cmbDitherer.TabIndex = 4;
            // 
            // lblDitherer
            // 
            this.lblDitherer.AutoSize = true;
            this.lblDitherer.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDitherer.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblDitherer.Location = new System.Drawing.Point(0, 0);
            this.lblDitherer.Name = "lblDitherer";
            this.lblDitherer.Size = new System.Drawing.Size(54, 13);
            this.lblDitherer.TabIndex = 3;
            this.lblDitherer.Text = "lblDitherer";
            // 
            // DithererSelectorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbParameters);
            this.Controls.Add(this.cmbDitherer);
            this.Controls.Add(this.lblDitherer);
            this.Name = "DithererSelectorControl";
            this.gbParameters.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbParameters;
        private System.Windows.Forms.PropertyGrid pgParameters;
        private System.Windows.Forms.ComboBox cmbDitherer;
        private System.Windows.Forms.Label lblDitherer;
    }
}
