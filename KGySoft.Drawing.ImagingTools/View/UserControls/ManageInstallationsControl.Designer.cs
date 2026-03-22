namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    partial class ManageInstallationsControl
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
            this.gbInstallation = new System.Windows.Forms.GroupBox();
            this.pnlStatus = new KGySoft.Drawing.ImagingTools.View.Controls.AutoMirrorPanel();
            this.lblStatusText = new KGySoft.WinForms.Controls.AdvancedLabel();
            this.lblStatus = new KGySoft.WinForms.Controls.AdvancedLabel();
            this.pnlButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnRemove = new KGySoft.WinForms.Controls.AdvancedButton();
            this.btnInstall = new KGySoft.WinForms.Controls.AdvancedButton();
            this.tbPath = new KGySoft.Drawing.ImagingTools.View.Controls.AdvancedTextBox();
            this.lblPath = new KGySoft.WinForms.Controls.AdvancedLabel();
            this.gbVisualStudioVersions = new System.Windows.Forms.GroupBox();
            this.cmbInstallations = new KGySoft.WinForms.Controls.AdvancedComboBox();
            this.gbAvailableVersion = new System.Windows.Forms.GroupBox();
            this.lblAvailableVersion = new KGySoft.WinForms.Controls.AdvancedLabel();
            this.gbInstallation.SuspendLayout();
            this.pnlStatus.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.gbVisualStudioVersions.SuspendLayout();
            this.gbAvailableVersion.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbInstallation
            // 
            this.gbInstallation.Controls.Add(this.pnlStatus);
            this.gbInstallation.Controls.Add(this.pnlButtons);
            this.gbInstallation.Controls.Add(this.tbPath);
            this.gbInstallation.Controls.Add(this.lblPath);
            this.gbInstallation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbInstallation.Location = new System.Drawing.Point(3, 89);
            this.gbInstallation.Name = "gbInstallation";
            this.gbInstallation.Size = new System.Drawing.Size(398, 119);
            this.gbInstallation.TabIndex = 2;
            this.gbInstallation.TabStop = false;
            this.gbInstallation.Text = "gbInstallation";
            // 
            // pnlStatus
            // 
            this.pnlStatus.Controls.Add(this.lblStatusText);
            this.pnlStatus.Controls.Add(this.lblStatus);
            this.pnlStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlStatus.Location = new System.Drawing.Point(3, 49);
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.pnlStatus.Size = new System.Drawing.Size(392, 32);
            this.pnlStatus.TabIndex = 2;
            // 
            // lblStatusText
            // 
            this.lblStatusText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStatusText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblStatusText.Location = new System.Drawing.Point(47, 3);
            this.lblStatusText.Name = "lblStatusText";
            this.lblStatusText.Size = new System.Drawing.Size(345, 29);
            this.lblStatusText.TabIndex = 1;
            this.lblStatusText.Text = "lblStatusText";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblStatus.Location = new System.Drawing.Point(0, 3);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(47, 13);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "lblStatus";
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.btnRemove);
            this.pnlButtons.Controls.Add(this.btnInstall);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.pnlButtons.Location = new System.Drawing.Point(3, 81);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Padding = new System.Windows.Forms.Padding(3);
            this.pnlButtons.Size = new System.Drawing.Size(392, 35);
            this.pnlButtons.TabIndex = 3;
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnRemove.AutoSize = true;
            this.btnRemove.Location = new System.Drawing.Point(307, 6);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(76, 23);
            this.btnRemove.TabIndex = 1;
            this.btnRemove.Text = "btnRemove";
            this.btnRemove.UseVisualStyleBackColor = true;
            // 
            // btnInstall
            // 
            this.btnInstall.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnInstall.AutoSize = true;
            this.btnInstall.Location = new System.Drawing.Point(226, 6);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(75, 23);
            this.btnInstall.TabIndex = 0;
            this.btnInstall.Text = "btnInstall";
            this.btnInstall.UseVisualStyleBackColor = true;
            // 
            // tbPath
            // 
            this.tbPath.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.tbPath.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.tbPath.Dock = System.Windows.Forms.DockStyle.Top;
            this.tbPath.Location = new System.Drawing.Point(3, 29);
            this.tbPath.Name = "tbPath";
            this.tbPath.Size = new System.Drawing.Size(392, 20);
            this.tbPath.TabIndex = 1;
            this.tbPath.Text = "tbPath";
            // 
            // lblPath
            // 
            this.lblPath.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPath.Location = new System.Drawing.Point(3, 16);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(392, 13);
            this.lblPath.TabIndex = 0;
            this.lblPath.Text = "lblPath";
            // 
            // gbVisualStudioVersions
            // 
            this.gbVisualStudioVersions.Controls.Add(this.cmbInstallations);
            this.gbVisualStudioVersions.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbVisualStudioVersions.Location = new System.Drawing.Point(3, 43);
            this.gbVisualStudioVersions.Name = "gbVisualStudioVersions";
            this.gbVisualStudioVersions.Size = new System.Drawing.Size(398, 46);
            this.gbVisualStudioVersions.TabIndex = 1;
            this.gbVisualStudioVersions.TabStop = false;
            this.gbVisualStudioVersions.Text = "gbVisualStudioVersions";
            // 
            // cmbInstallations
            // 
            this.cmbInstallations.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmbInstallations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInstallations.FormattingEnabled = true;
            this.cmbInstallations.Location = new System.Drawing.Point(3, 16);
            this.cmbInstallations.Name = "cmbInstallations";
            this.cmbInstallations.Size = new System.Drawing.Size(392, 21);
            this.cmbInstallations.TabIndex = 0;
            // 
            // gbAvailableVersion
            // 
            this.gbAvailableVersion.Controls.Add(this.lblAvailableVersion);
            this.gbAvailableVersion.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbAvailableVersion.Location = new System.Drawing.Point(3, 3);
            this.gbAvailableVersion.Name = "gbAvailableVersion";
            this.gbAvailableVersion.Size = new System.Drawing.Size(398, 40);
            this.gbAvailableVersion.TabIndex = 0;
            this.gbAvailableVersion.TabStop = false;
            this.gbAvailableVersion.Text = "gbAvailableVersion";
            // 
            // lblAvailableVersion
            // 
            this.lblAvailableVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAvailableVersion.Location = new System.Drawing.Point(3, 16);
            this.lblAvailableVersion.Name = "lblAvailableVersion";
            this.lblAvailableVersion.Size = new System.Drawing.Size(392, 21);
            this.lblAvailableVersion.TabIndex = 0;
            this.lblAvailableVersion.Text = "lblAvailableVersion";
            // 
            // ManageInstallationsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Size = new System.Drawing.Size(404, 211);
            this.Controls.Add(this.gbInstallation);
            this.Controls.Add(this.gbVisualStudioVersions);
            this.Controls.Add(this.gbAvailableVersion);
            this.Name = "ManageInstallationsControl";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.gbInstallation.ResumeLayout(false);
            this.gbInstallation.PerformLayout();
            this.pnlStatus.ResumeLayout(false);
            this.pnlStatus.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.gbVisualStudioVersions.ResumeLayout(false);
            this.gbAvailableVersion.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.GroupBox gbInstallation;
        private System.Windows.Forms.FlowLayoutPanel pnlButtons;
        private KGySoft.WinForms.Controls.AdvancedButton btnRemove;
        private KGySoft.WinForms.Controls.AdvancedButton btnInstall;
        private Controls.AutoMirrorPanel pnlStatus;
        private KGySoft.WinForms.Controls.AdvancedLabel lblStatusText;
        private KGySoft.WinForms.Controls.AdvancedLabel lblStatus;
        private Controls.AdvancedTextBox tbPath;
        private KGySoft.WinForms.Controls.AdvancedLabel lblPath;
        private System.Windows.Forms.GroupBox gbVisualStudioVersions;
        private KGySoft.WinForms.Controls.AdvancedComboBox cmbInstallations;
        private System.Windows.Forms.GroupBox gbAvailableVersion;
        private KGySoft.WinForms.Controls.AdvancedLabel lblAvailableVersion;
    }
}