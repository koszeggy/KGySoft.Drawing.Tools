
namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    partial class LanguageSettingsForm
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
            this.components = new System.ComponentModel.Container();
            this.okCancelApplyButtons = new KGySoft.Drawing.ImagingTools.View.UserControls.OkCancelButtons();
            this.gbAllowResxResources = new KGySoft.Drawing.ImagingTools.View.Controls.CheckGroupBox();
            this.gbDisplayLanguage = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnDownloadResources = new System.Windows.Forms.Button();
            this.btnEditResources = new System.Windows.Forms.Button();
            this.cmbLanguages = new System.Windows.Forms.ComboBox();
            this.chbExistingResourcesOnly = new System.Windows.Forms.CheckBox();
            this.chbUseOSLanguage = new System.Windows.Forms.CheckBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.gbAllowResxResources.SuspendLayout();
            this.gbDisplayLanguage.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // okCancelApplyButtons
            // 
            this.okCancelApplyButtons.ApplyButtonVisible = true;
            this.okCancelApplyButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.okCancelApplyButtons.Location = new System.Drawing.Point(3, 137);
            this.okCancelApplyButtons.Name = "okCancelApplyButtons";
            this.okCancelApplyButtons.Size = new System.Drawing.Size(328, 35);
            this.okCancelApplyButtons.TabIndex = 1;
            // 
            // gbAllowResxResources
            // 
            this.gbAllowResxResources.Controls.Add(this.gbDisplayLanguage);
            this.gbAllowResxResources.Controls.Add(this.chbExistingResourcesOnly);
            this.gbAllowResxResources.Controls.Add(this.chbUseOSLanguage);
            this.gbAllowResxResources.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbAllowResxResources.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.gbAllowResxResources.Location = new System.Drawing.Point(3, 3);
            this.gbAllowResxResources.Name = "gbAllowResxResources";
            this.gbAllowResxResources.Padding = new System.Windows.Forms.Padding(5);
            this.gbAllowResxResources.Size = new System.Drawing.Size(328, 134);
            this.gbAllowResxResources.TabIndex = 0;
            this.gbAllowResxResources.TabStop = false;
            this.gbAllowResxResources.Text = "gbAllowResxResources";
            // 
            // gbDisplayLanguage
            // 
            this.gbDisplayLanguage.Controls.Add(this.tableLayoutPanel1);
            this.gbDisplayLanguage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbDisplayLanguage.Location = new System.Drawing.Point(5, 54);
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
            this.btnDownloadResources.Size = new System.Drawing.Size(307, 23);
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
            this.btnEditResources.Size = new System.Drawing.Size(105, 23);
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
            // LanguageSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 175);
            this.Controls.Add(this.gbAllowResxResources);
            this.Controls.Add(this.okCancelApplyButtons);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LanguageSettingsForm";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "LanguageSettingsForm";
            this.gbAllowResxResources.ResumeLayout(false);
            this.gbAllowResxResources.PerformLayout();
            this.gbDisplayLanguage.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private UserControls.OkCancelButtons okCancelApplyButtons;
        private KGySoft.Drawing.ImagingTools.View.Controls.CheckGroupBox gbAllowResxResources;
        private System.Windows.Forms.CheckBox chbExistingResourcesOnly;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.GroupBox gbDisplayLanguage;
        private System.Windows.Forms.CheckBox chbUseOSLanguage;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnEditResources;
        private System.Windows.Forms.ComboBox cmbLanguages;
        private System.Windows.Forms.Button btnDownloadResources;
    }
}