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
            this.gbInstallation = new System.Windows.Forms.GroupBox();
            this.tblButtons = new System.Windows.Forms.TableLayoutPanel();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnInstall = new System.Windows.Forms.Button();
            this.pnlStatus = new KGySoft.Drawing.ImagingTools.View.Controls.AutoMirrorPanel();
            this.lblStatusText = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.tbPath = new System.Windows.Forms.TextBox();
            this.lblPath = new System.Windows.Forms.Label();
            this.gbVisualStudioVersions = new System.Windows.Forms.GroupBox();
            this.cmbInstallations = new System.Windows.Forms.ComboBox();
            this.gbAvailableVersion = new System.Windows.Forms.GroupBox();
            this.lblAvailableVersion = new System.Windows.Forms.Label();
            this.gbInstallation.SuspendLayout();
            this.tblButtons.SuspendLayout();
            this.pnlStatus.SuspendLayout();
            this.gbVisualStudioVersions.SuspendLayout();
            this.gbAvailableVersion.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbInstallation
            // 
            this.gbInstallation.Controls.Add(this.tblButtons);
            this.gbInstallation.Controls.Add(this.pnlStatus);
            this.gbInstallation.Controls.Add(this.tbPath);
            this.gbInstallation.Controls.Add(this.lblPath);
            this.gbInstallation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbInstallation.Location = new System.Drawing.Point(3, 89);
            this.gbInstallation.Name = "gbInstallation";
            this.gbInstallation.Size = new System.Drawing.Size(378, 105);
            this.gbInstallation.TabIndex = 2;
            this.gbInstallation.TabStop = false;
            this.gbInstallation.Text = "gbInstallation";
            // 
            // tblButtons
            // 
            this.tblButtons.ColumnCount = 2;
            this.tblButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblButtons.Controls.Add(this.btnRemove, 1, 0);
            this.tblButtons.Controls.Add(this.btnInstall, 0, 0);
            this.tblButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.tblButtons.Location = new System.Drawing.Point(3, 66);
            this.tblButtons.Name = "tblButtons";
            this.tblButtons.RowCount = 1;
            this.tblButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.tblButtons.Size = new System.Drawing.Size(372, 37);
            this.tblButtons.TabIndex = 3;
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnRemove.Location = new System.Drawing.Point(241, 7);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 1;
            this.btnRemove.Text = "btnRemove";
            this.btnRemove.UseVisualStyleBackColor = true;
            // 
            // btnInstall
            // 
            this.btnInstall.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnInstall.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnInstall.Location = new System.Drawing.Point(55, 7);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(75, 23);
            this.btnInstall.TabIndex = 0;
            this.btnInstall.Text = "btnInstall";
            this.btnInstall.UseVisualStyleBackColor = true;
            // 
            // pnlStatus
            // 
            this.pnlStatus.Controls.Add(this.lblStatusText);
            this.pnlStatus.Controls.Add(this.lblStatus);
            this.pnlStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlStatus.Location = new System.Drawing.Point(3, 49);
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Size = new System.Drawing.Size(372, 17);
            this.pnlStatus.TabIndex = 2;
            // 
            // lblStatusText
            // 
            this.lblStatusText.AutoSize = true;
            this.lblStatusText.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblStatusText.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblStatusText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblStatusText.Location = new System.Drawing.Point(47, 0);
            this.lblStatusText.Name = "lblStatusText";
            this.lblStatusText.Size = new System.Drawing.Size(81, 13);
            this.lblStatusText.TabIndex = 1;
            this.lblStatusText.Text = "lblStatusText";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblStatus.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblStatus.Location = new System.Drawing.Point(0, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(47, 13);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "lblStatus";
            // 
            // tbPath
            // 
            this.tbPath.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.tbPath.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.tbPath.Dock = System.Windows.Forms.DockStyle.Top;
            this.tbPath.Location = new System.Drawing.Point(3, 29);
            this.tbPath.Name = "tbPath";
            this.tbPath.Size = new System.Drawing.Size(372, 20);
            this.tbPath.TabIndex = 1;
            this.tbPath.Text = "tbPath";
            // 
            // lblPath
            // 
            this.lblPath.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPath.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblPath.Location = new System.Drawing.Point(3, 16);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(372, 13);
            this.lblPath.TabIndex = 0;
            this.lblPath.Text = "lblPath";
            // 
            // gbVisualStudioVersions
            // 
            this.gbVisualStudioVersions.Controls.Add(this.cmbInstallations);
            this.gbVisualStudioVersions.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbVisualStudioVersions.Location = new System.Drawing.Point(3, 43);
            this.gbVisualStudioVersions.Name = "gbVisualStudioVersions";
            this.gbVisualStudioVersions.Size = new System.Drawing.Size(378, 46);
            this.gbVisualStudioVersions.TabIndex = 1;
            this.gbVisualStudioVersions.TabStop = false;
            this.gbVisualStudioVersions.Text = "gbVisualStudioVersions";
            // 
            // cmbInstallations
            // 
            this.cmbInstallations.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmbInstallations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInstallations.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbInstallations.FormattingEnabled = true;
            this.cmbInstallations.Location = new System.Drawing.Point(3, 16);
            this.cmbInstallations.Name = "cmbInstallations";
            this.cmbInstallations.Size = new System.Drawing.Size(372, 21);
            this.cmbInstallations.TabIndex = 0;
            // 
            // gbAvailableVersion
            // 
            this.gbAvailableVersion.Controls.Add(this.lblAvailableVersion);
            this.gbAvailableVersion.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbAvailableVersion.Location = new System.Drawing.Point(3, 3);
            this.gbAvailableVersion.Name = "gbAvailableVersion";
            this.gbAvailableVersion.Size = new System.Drawing.Size(378, 40);
            this.gbAvailableVersion.TabIndex = 0;
            this.gbAvailableVersion.TabStop = false;
            this.gbAvailableVersion.Text = "gbAvailableVersion";
            // 
            // lblAvailableVersion
            // 
            this.lblAvailableVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAvailableVersion.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblAvailableVersion.Location = new System.Drawing.Point(3, 16);
            this.lblAvailableVersion.Name = "lblAvailableVersion";
            this.lblAvailableVersion.Size = new System.Drawing.Size(372, 21);
            this.lblAvailableVersion.TabIndex = 0;
            this.lblAvailableVersion.Text = "lblAvailableVersion";
            // 
            // ManageInstallationsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 197);
            this.Controls.Add(this.gbInstallation);
            this.Controls.Add(this.gbVisualStudioVersions);
            this.Controls.Add(this.gbAvailableVersion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ManageInstallationsForm";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "ManageInstallationsForm";
            this.gbInstallation.ResumeLayout(false);
            this.gbInstallation.PerformLayout();
            this.tblButtons.ResumeLayout(false);
            this.pnlStatus.ResumeLayout(false);
            this.pnlStatus.PerformLayout();
            this.gbVisualStudioVersions.ResumeLayout(false);
            this.gbAvailableVersion.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox gbInstallation;
        private System.Windows.Forms.TableLayoutPanel tblButtons;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnInstall;
        private Controls.AutoMirrorPanel pnlStatus;
        private System.Windows.Forms.Label lblStatusText;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TextBox tbPath;
        private System.Windows.Forms.Label lblPath;
        private GroupBox gbVisualStudioVersions;
        private ComboBox cmbInstallations;
        private GroupBox gbAvailableVersion;
        private Label lblAvailableVersion;
    }
}