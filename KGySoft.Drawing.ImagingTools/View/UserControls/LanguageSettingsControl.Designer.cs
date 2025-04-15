namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    partial class LanguageSettingsControl
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
            this.components = new System.ComponentModel.Container();
            this.okCancelApplyButtons = new KGySoft.Drawing.ImagingTools.View.UserControls.OkCancelButtons();
            this.gbAllowResxResources = new KGySoft.Drawing.ImagingTools.View.Controls.CheckGroupBox();
            this.gbDisplayLanguage = new KGySoft.Drawing.ImagingTools.View.Controls.AdvancedGroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnDownloadResources = new System.Windows.Forms.Button();
            this.btnEditResources = new System.Windows.Forms.Button();
            this.cmbLanguages = new System.Windows.Forms.ComboBox();
            this.gbResxResourcesPath = new KGySoft.Drawing.ImagingTools.View.Controls.CheckGroupBox();
            this.txtResxResourcesPath = new Controls.AdvancedTextBox();
            this.chbExistingResourcesOnly = new KGySoft.Drawing.ImagingTools.View.Controls.AdvancedCheckBox();
            this.chbUseOSLanguage = new KGySoft.Drawing.ImagingTools.View.Controls.AdvancedCheckBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.gbAllowResxResources.SuspendLayout();
            this.gbDisplayLanguage.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.gbResxResourcesPath.SuspendLayout();
            this.SuspendLayout();
            // 
            // okCancelApplyButtons
            // 
            this.okCancelApplyButtons.ApplyButtonVisible = true;
            this.okCancelApplyButtons.BackColor = System.Drawing.Color.Transparent;
            this.okCancelApplyButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.okCancelApplyButtons.Location = new System.Drawing.Point(3, 182);
            this.okCancelApplyButtons.Name = "okCancelApplyButtons";
            this.okCancelApplyButtons.Size = new System.Drawing.Size(328, 35);
            this.okCancelApplyButtons.TabIndex = 1;
            // 
            // gbAllowResxResources
            // 
            this.gbAllowResxResources.Controls.Add(this.gbDisplayLanguage);
            this.gbAllowResxResources.Controls.Add(this.gbResxResourcesPath);
            this.gbAllowResxResources.Controls.Add(this.chbExistingResourcesOnly);
            this.gbAllowResxResources.Controls.Add(this.chbUseOSLanguage);
            this.gbAllowResxResources.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbAllowResxResources.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.gbAllowResxResources.Location = new System.Drawing.Point(3, 3);
            this.gbAllowResxResources.Name = "gbAllowResxResources";
            this.gbAllowResxResources.Padding = new System.Windows.Forms.Padding(5);
            this.gbAllowResxResources.Size = new System.Drawing.Size(328, 179);
            this.gbAllowResxResources.TabIndex = 0;
            this.gbAllowResxResources.TabStop = false;
            this.gbAllowResxResources.Text = "gbAllowResxResources";
            // 
            // gbDisplayLanguage
            // 
            this.gbDisplayLanguage.Controls.Add(this.tableLayoutPanel1);
            this.gbDisplayLanguage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbDisplayLanguage.Location = new System.Drawing.Point(5, 99);
            this.gbDisplayLanguage.Name = "gbDisplayLanguage";
            this.gbDisplayLanguage.Size = new System.Drawing.Size(318, 75);
            this.gbDisplayLanguage.TabIndex = 2;
            this.gbDisplayLanguage.TabStop = false;
            this.gbDisplayLanguage.Text = "gbDisplayLanguage";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.btnDownloadResources, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnEditResources, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.cmbLanguages, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(312, 56);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // btnDownloadResources
            // 
            this.btnDownloadResources.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.btnDownloadResources, 2);
            this.btnDownloadResources.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnDownloadResources.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnDownloadResources.Location = new System.Drawing.Point(2, 30);
            this.btnDownloadResources.Margin = new System.Windows.Forms.Padding(2, 2, 3, 3);
            this.btnDownloadResources.Name = "btnDownloadResources";
            this.btnDownloadResources.Size = new System.Drawing.Size(307, 22);
            this.btnDownloadResources.TabIndex = 2;
            this.btnDownloadResources.Text = "btnDownloadResources";
            this.btnDownloadResources.UseVisualStyleBackColor = true;
            // 
            // btnEditResources
            // 
            this.btnEditResources.AutoSize = true;
            this.btnEditResources.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnEditResources.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnEditResources.Location = new System.Drawing.Point(204, 2);
            this.btnEditResources.Margin = new System.Windows.Forms.Padding(3, 2, 3, 3);
            this.btnEditResources.Name = "btnEditResources";
            this.btnEditResources.Size = new System.Drawing.Size(105, 22);
            this.btnEditResources.TabIndex = 1;
            this.btnEditResources.Text = "btnEditResources";
            this.btnEditResources.UseVisualStyleBackColor = true;
            // 
            // cmbLanguages
            // 
            this.cmbLanguages.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmbLanguages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLanguages.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbLanguages.FormattingEnabled = true;
            this.cmbLanguages.Location = new System.Drawing.Point(3, 3);
            this.cmbLanguages.Name = "cmbLanguages";
            this.cmbLanguages.Size = new System.Drawing.Size(195, 21);
            this.cmbLanguages.TabIndex = 0;
            // 
            // gbResxResourcesPath
            // 
            this.gbResxResourcesPath.Controls.Add(this.txtResxResourcesPath);
            this.gbResxResourcesPath.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbResxResourcesPath.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.gbResxResourcesPath.Location = new System.Drawing.Point(5, 54);
            this.gbResxResourcesPath.Name = "gbResxResourcesPath";
            this.gbResxResourcesPath.Size = new System.Drawing.Size(318, 45);
            this.gbResxResourcesPath.TabIndex = 3;
            this.gbResxResourcesPath.TabStop = false;
            this.gbResxResourcesPath.Text = "gbResxResourcesPath";
            // 
            // txtResxResourcesPath
            // 
            this.txtResxResourcesPath.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtResxResourcesPath.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.txtResxResourcesPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResxResourcesPath.Location = new System.Drawing.Point(3, 18);
            this.txtResxResourcesPath.Name = "txtResxResourcesPath";
            this.txtResxResourcesPath.Size = new System.Drawing.Size(312, 20);
            this.txtResxResourcesPath.TabIndex = 1;
            // 
            // chbExistingResourcesOnly
            // 
            this.chbExistingResourcesOnly.AutoSize = true;
            this.chbExistingResourcesOnly.Dock = System.Windows.Forms.DockStyle.Top;
            this.chbExistingResourcesOnly.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chbExistingResourcesOnly.Location = new System.Drawing.Point(5, 36);
            this.chbExistingResourcesOnly.Name = "chbExistingResourcesOnly";
            this.chbExistingResourcesOnly.Size = new System.Drawing.Size(318, 18);
            this.chbExistingResourcesOnly.TabIndex = 1;
            this.chbExistingResourcesOnly.Text = "chbExistingResourcesOnly";
            this.chbExistingResourcesOnly.UseVisualStyleBackColor = true;
            // 
            // chbUseOSLanguage
            // 
            this.chbUseOSLanguage.AutoSize = true;
            this.chbUseOSLanguage.Dock = System.Windows.Forms.DockStyle.Top;
            this.chbUseOSLanguage.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chbUseOSLanguage.Location = new System.Drawing.Point(5, 18);
            this.chbUseOSLanguage.Name = "chbUseOSLanguage";
            this.chbUseOSLanguage.Size = new System.Drawing.Size(318, 18);
            this.chbUseOSLanguage.TabIndex = 0;
            this.chbUseOSLanguage.Text = "chbUseOSLanguage";
            this.chbUseOSLanguage.UseVisualStyleBackColor = true;
            // 
            // LanguageSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Size = new System.Drawing.Size(334, 220);
            this.Controls.Add(this.gbAllowResxResources);
            this.Controls.Add(this.okCancelApplyButtons);
            this.Name = "LanguageSettingsControl";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.gbAllowResxResources.ResumeLayout(false);
            this.gbAllowResxResources.PerformLayout();
            this.gbDisplayLanguage.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.gbResxResourcesPath.ResumeLayout(false);
            this.gbResxResourcesPath.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private UserControls.OkCancelButtons okCancelApplyButtons;
        private KGySoft.Drawing.ImagingTools.View.Controls.CheckGroupBox gbAllowResxResources;
        private KGySoft.Drawing.ImagingTools.View.Controls.AdvancedCheckBox chbExistingResourcesOnly;
        private System.Windows.Forms.ToolTip toolTip;
        private KGySoft.Drawing.ImagingTools.View.Controls.AdvancedGroupBox gbDisplayLanguage;
        private KGySoft.Drawing.ImagingTools.View.Controls.AdvancedCheckBox chbUseOSLanguage;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnEditResources;
        private System.Windows.Forms.ComboBox cmbLanguages;
        private System.Windows.Forms.Button btnDownloadResources;
        private Controls.CheckGroupBox gbResxResourcesPath;
        private Controls.AdvancedTextBox txtResxResourcesPath;
    }
}