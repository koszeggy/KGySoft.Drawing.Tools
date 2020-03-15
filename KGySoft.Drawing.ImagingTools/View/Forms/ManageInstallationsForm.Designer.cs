using System.Windows.Forms;

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    partial class ManageInstallationsForm
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
            this.cbInstallations = new System.Windows.Forms.ComboBox();
            this.lblVersions = new System.Windows.Forms.Label();
            this.gbInstallation = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnInstall = new System.Windows.Forms.Button();
            this.pnlStatus = new System.Windows.Forms.Panel();
            this.lblStatusText = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.tbPath = new System.Windows.Forms.TextBox();
            this.lblPath = new System.Windows.Forms.Label();
            this.gbInstallation.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.pnlStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbInstallations
            // 
            this.cbInstallations.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbInstallations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbInstallations.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbInstallations.FormattingEnabled = true;
            this.cbInstallations.Location = new System.Drawing.Point(3, 16);
            this.cbInstallations.Name = "cbInstallations";
            this.cbInstallations.Size = new System.Drawing.Size(308, 21);
            this.cbInstallations.TabIndex = 1;
            // 
            // lblVersions
            // 
            this.lblVersions.AutoSize = true;
            this.lblVersions.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblVersions.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblVersions.Location = new System.Drawing.Point(3, 3);
            this.lblVersions.Name = "lblVersions";
            this.lblVersions.Size = new System.Drawing.Size(160, 13);
            this.lblVersions.TabIndex = 0;
            // 
            // gbInstallation
            // 
            this.gbInstallation.Controls.Add(this.tableLayoutPanel1);
            this.gbInstallation.Controls.Add(this.pnlStatus);
            this.gbInstallation.Controls.Add(this.tbPath);
            this.gbInstallation.Controls.Add(this.lblPath);
            this.gbInstallation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbInstallation.Location = new System.Drawing.Point(3, 37);
            this.gbInstallation.Name = "gbInstallation";
            this.gbInstallation.Size = new System.Drawing.Size(308, 109);
            this.gbInstallation.TabIndex = 2;
            this.gbInstallation.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.btnRemove, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnInstall, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 66);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(302, 37);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnRemove.Location = new System.Drawing.Point(189, 7);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 1;
            this.btnRemove.UseVisualStyleBackColor = true;
            // 
            // btnInstall
            // 
            this.btnInstall.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnInstall.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnInstall.Location = new System.Drawing.Point(38, 7);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(75, 23);
            this.btnInstall.TabIndex = 0;
            this.btnInstall.UseVisualStyleBackColor = true;
            // 
            // pnlStatus
            // 
            this.pnlStatus.Controls.Add(this.lblStatusText);
            this.pnlStatus.Controls.Add(this.lblStatus);
            this.pnlStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlStatus.Location = new System.Drawing.Point(3, 49);
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Size = new System.Drawing.Size(302, 17);
            this.pnlStatus.TabIndex = 2;
            // 
            // lblStatusText
            // 
            this.lblStatusText.AutoSize = true;
            this.lblStatusText.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblStatusText.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblStatusText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblStatusText.Location = new System.Drawing.Point(40, 0);
            this.lblStatusText.Name = "lblStatusText";
            this.lblStatusText.Size = new System.Drawing.Size(11, 13);
            this.lblStatusText.TabIndex = 1;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblStatus.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblStatus.Location = new System.Drawing.Point(0, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(40, 13);
            this.lblStatus.TabIndex = 0;
            // 
            // tbPath
            // 
            this.tbPath.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.tbPath.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.tbPath.Dock = System.Windows.Forms.DockStyle.Top;
            this.tbPath.Location = new System.Drawing.Point(3, 29);
            this.tbPath.Name = "tbPath";
            this.tbPath.Size = new System.Drawing.Size(302, 20);
            this.tbPath.TabIndex = 1;
            // 
            // lblPath
            // 
            this.lblPath.AutoSize = true;
            this.lblPath.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPath.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblPath.Location = new System.Drawing.Point(3, 16);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(32, 13);
            this.lblPath.TabIndex = 0;
            // 
            // ManageInstallationsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 149);
            this.Controls.Add(this.gbInstallation);
            this.Controls.Add(this.cbInstallations);
            this.Controls.Add(this.lblVersions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ManageInstallationsForm";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.gbInstallation.ResumeLayout(false);
            this.gbInstallation.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.pnlStatus.ResumeLayout(false);
            this.pnlStatus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbInstallations;
        private System.Windows.Forms.Label lblVersions;
        private System.Windows.Forms.GroupBox gbInstallation;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnInstall;
        private System.Windows.Forms.Panel pnlStatus;
        private System.Windows.Forms.Label lblStatusText;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TextBox tbPath;
        private System.Windows.Forms.Label lblPath;
    }
}