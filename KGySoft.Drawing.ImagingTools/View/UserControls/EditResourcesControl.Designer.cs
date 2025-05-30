﻿namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal partial class EditResourcesControl
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
            this.gbResourceEntries = new System.Windows.Forms.GroupBox();
            this.gridResources = new KGySoft.Drawing.ImagingTools.View.Controls.AdvancedDataGridView();
            this.colResourceKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOriginalText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTranslatedText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.pnlFilter = new KGySoft.Drawing.ImagingTools.View.Controls.AutoMirrorPanel();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.lblFilter = new System.Windows.Forms.Label();
            this.gbResourceFile = new System.Windows.Forms.GroupBox();
            this.cmbResourceFiles = new System.Windows.Forms.ComboBox();
            this.chbHideDependencies = new KGySoft.Drawing.ImagingTools.View.Controls.AdvancedCheckBox();
            this.splitterEditResources = new System.Windows.Forms.Splitter();
            this.pnlEditResourceEntry = new System.Windows.Forms.TableLayoutPanel();
            this.gbOriginalText = new System.Windows.Forms.GroupBox();
            this.txtOriginalText = new KGySoft.Drawing.ImagingTools.View.Controls.AdvancedTextBox();
            this.gbTranslatedText = new System.Windows.Forms.GroupBox();
            this.txtTranslatedText = new KGySoft.Drawing.ImagingTools.View.Controls.AdvancedTextBox();
            this.okCancelApplyButtons = new KGySoft.Drawing.ImagingTools.View.UserControls.OkCancelButtons();
            this.gbResourceEntries.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridResources)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).BeginInit();
            this.pnlFilter.SuspendLayout();
            this.gbResourceFile.SuspendLayout();
            this.pnlEditResourceEntry.SuspendLayout();
            this.gbOriginalText.SuspendLayout();
            this.gbTranslatedText.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbResourceEntries
            // 
            this.gbResourceEntries.Controls.Add(this.gridResources);
            this.gbResourceEntries.Controls.Add(this.pnlFilter);
            this.gbResourceEntries.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbResourceEntries.Location = new System.Drawing.Point(3, 67);
            this.gbResourceEntries.Name = "gbResourceEntries";
            this.gbResourceEntries.Size = new System.Drawing.Size(578, 99);
            this.gbResourceEntries.TabIndex = 1;
            this.gbResourceEntries.TabStop = false;
            this.gbResourceEntries.Text = "gbResourceEntries";
            // 
            // gridResources
            // 
            this.gridResources.AllowUserToAddRows = false;
            this.gridResources.AllowUserToDeleteRows = false;
            this.gridResources.AutoGenerateColumns = false;
            this.gridResources.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridResources.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colResourceKey,
            this.colOriginalText,
            this.colTranslatedText});
            this.gridResources.DataSource = this.bindingSource;
            this.gridResources.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridResources.Location = new System.Drawing.Point(3, 40);
            this.gridResources.MultiSelect = false;
            this.gridResources.Name = "gridResources";
            this.gridResources.Size = new System.Drawing.Size(572, 56);
            this.gridResources.TabIndex = 3;
            // 
            // colResourceKey
            // 
            this.colResourceKey.DataPropertyName = "Key";
            this.colResourceKey.HeaderText = "colResourceKey";
            this.colResourceKey.Name = "colResourceKey";
            this.colResourceKey.ReadOnly = true;
            // 
            // colOriginalText
            // 
            this.colOriginalText.DataPropertyName = "OriginalText";
            this.colOriginalText.HeaderText = "colOriginalText";
            this.colOriginalText.Name = "colOriginalText";
            this.colOriginalText.ReadOnly = true;
            this.colOriginalText.Width = 200;
            // 
            // colTranslatedText
            // 
            this.colTranslatedText.DataPropertyName = "TranslatedText";
            this.colTranslatedText.HeaderText = "colTranslatedText";
            this.colTranslatedText.Name = "colTranslatedText";
            this.colTranslatedText.Width = 200;
            // 
            // bindingSource
            // 
            this.bindingSource.DataSource = typeof(KGySoft.Drawing.ImagingTools.Model.ResourceEntry);
            // 
            // pnlFilter
            // 
            this.pnlFilter.Controls.Add(this.txtFilter);
            this.pnlFilter.Controls.Add(this.lblFilter);
            this.pnlFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFilter.Location = new System.Drawing.Point(3, 16);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.pnlFilter.Size = new System.Drawing.Size(572, 24);
            this.pnlFilter.TabIndex = 4;
            // 
            // txtFilter
            // 
            this.txtFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFilter.Location = new System.Drawing.Point(39, 2);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(533, 20);
            this.txtFilter.TabIndex = 1;
            // 
            // lblFilter
            // 
            this.lblFilter.AutoSize = true;
            this.lblFilter.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblFilter.Location = new System.Drawing.Point(0, 2);
            this.lblFilter.Name = "lblFilter";
            this.lblFilter.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.lblFilter.Size = new System.Drawing.Size(39, 15);
            this.lblFilter.TabIndex = 0;
            this.lblFilter.Text = "lblFilter";
            // 
            // gbResourceFile
            // 
            this.gbResourceFile.Controls.Add(this.cmbResourceFiles);
            this.gbResourceFile.Controls.Add(this.chbHideDependencies);
            this.gbResourceFile.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbResourceFile.Location = new System.Drawing.Point(3, 3);
            this.gbResourceFile.Name = "gbResourceFile";
            this.gbResourceFile.Size = new System.Drawing.Size(578, 64);
            this.gbResourceFile.TabIndex = 0;
            this.gbResourceFile.TabStop = false;
            this.gbResourceFile.Text = "gbResourceFile";
            // 
            // cmbResourceFiles
            // 
            this.cmbResourceFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbResourceFiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbResourceFiles.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbResourceFiles.FormattingEnabled = true;
            this.cmbResourceFiles.Location = new System.Drawing.Point(3, 33);
            this.cmbResourceFiles.Name = "cmbResourceFiles";
            this.cmbResourceFiles.Size = new System.Drawing.Size(572, 21);
            this.cmbResourceFiles.TabIndex = 1;
            // 
            // chbHideDependencies
            // 
            this.chbHideDependencies.AutoSize = true;
            this.chbHideDependencies.Checked = true;
            this.chbHideDependencies.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbHideDependencies.Dock = System.Windows.Forms.DockStyle.Top;
            this.chbHideDependencies.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chbHideDependencies.Location = new System.Drawing.Point(3, 16);
            this.chbHideDependencies.Name = "chbHideDependencies";
            this.chbHideDependencies.Size = new System.Drawing.Size(572, 17);
            this.chbHideDependencies.TabIndex = 0;
            this.chbHideDependencies.Text = "chbHideDependencies";
            this.chbHideDependencies.UseVisualStyleBackColor = true;
            // 
            // splitterEditResources
            // 
            this.splitterEditResources.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitterEditResources.Location = new System.Drawing.Point(3, 166);
            this.splitterEditResources.MinExtra = 50;
            this.splitterEditResources.MinSize = 50;
            this.splitterEditResources.Name = "splitterEditResources";
            this.splitterEditResources.Size = new System.Drawing.Size(578, 3);
            this.splitterEditResources.TabIndex = 2;
            this.splitterEditResources.TabStop = false;
            // 
            // pnlEditResourceEntry
            // 
            this.pnlEditResourceEntry.ColumnCount = 2;
            this.pnlEditResourceEntry.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pnlEditResourceEntry.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pnlEditResourceEntry.Controls.Add(this.gbOriginalText, 0, 0);
            this.pnlEditResourceEntry.Controls.Add(this.gbTranslatedText, 1, 0);
            this.pnlEditResourceEntry.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlEditResourceEntry.Location = new System.Drawing.Point(3, 169);
            this.pnlEditResourceEntry.Name = "pnlEditResourceEntry";
            this.pnlEditResourceEntry.RowCount = 1;
            this.pnlEditResourceEntry.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pnlEditResourceEntry.Size = new System.Drawing.Size(578, 104);
            this.pnlEditResourceEntry.TabIndex = 3;
            // 
            // gbOriginalText
            // 
            this.gbOriginalText.Controls.Add(this.txtOriginalText);
            this.gbOriginalText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbOriginalText.Location = new System.Drawing.Point(3, 3);
            this.gbOriginalText.Name = "gbOriginalText";
            this.gbOriginalText.Size = new System.Drawing.Size(283, 98);
            this.gbOriginalText.TabIndex = 0;
            this.gbOriginalText.TabStop = false;
            this.gbOriginalText.Text = "gbOriginalText";
            // 
            // txtOriginalText
            // 
            this.txtOriginalText.BackColor = System.Drawing.SystemColors.Control;
            this.txtOriginalText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOriginalText.ForeColor = System.Drawing.SystemColors.ControlText;
            this.txtOriginalText.Location = new System.Drawing.Point(3, 16);
            this.txtOriginalText.Multiline = true;
            this.txtOriginalText.Name = "txtOriginalText";
            this.txtOriginalText.ReadOnly = true;
            this.txtOriginalText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtOriginalText.Size = new System.Drawing.Size(277, 79);
            this.txtOriginalText.TabIndex = 0;
            this.txtOriginalText.WordWrap = false;
            // 
            // gbTranslatedText
            // 
            this.gbTranslatedText.Controls.Add(this.txtTranslatedText);
            this.gbTranslatedText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbTranslatedText.Location = new System.Drawing.Point(292, 3);
            this.gbTranslatedText.Name = "gbTranslatedText";
            this.gbTranslatedText.Size = new System.Drawing.Size(283, 98);
            this.gbTranslatedText.TabIndex = 1;
            this.gbTranslatedText.TabStop = false;
            this.gbTranslatedText.Text = "gbTranslatedText";
            // 
            // txtTranslatedText
            // 
            this.txtTranslatedText.AcceptsTab = true;
            this.txtTranslatedText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTranslatedText.Location = new System.Drawing.Point(3, 16);
            this.txtTranslatedText.Multiline = true;
            this.txtTranslatedText.Name = "txtTranslatedText";
            this.txtTranslatedText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtTranslatedText.Size = new System.Drawing.Size(277, 79);
            this.txtTranslatedText.TabIndex = 1;
            this.txtTranslatedText.WordWrap = false;
            // 
            // okCancelApplyButtons
            // 
            this.okCancelApplyButtons.ApplyButtonVisible = true;
            this.okCancelApplyButtons.BackColor = System.Drawing.Color.Transparent;
            this.okCancelApplyButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.okCancelApplyButtons.Location = new System.Drawing.Point(3, 273);
            this.okCancelApplyButtons.Name = "okCancelApplyButtons";
            this.okCancelApplyButtons.Size = new System.Drawing.Size(578, 35);
            this.okCancelApplyButtons.TabIndex = 4;
            // 
            // EditResourcesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbResourceEntries);
            this.Controls.Add(this.gbResourceFile);
            this.Controls.Add(this.splitterEditResources);
            this.Controls.Add(this.pnlEditResourceEntry);
            this.Controls.Add(this.okCancelApplyButtons);
            this.Name = "EditResourcesControl";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(585, 350);
            this.gbResourceEntries.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridResources)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).EndInit();
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.PerformLayout();
            this.gbResourceFile.ResumeLayout(false);
            this.gbResourceFile.PerformLayout();
            this.pnlEditResourceEntry.ResumeLayout(false);
            this.gbOriginalText.ResumeLayout(false);
            this.gbOriginalText.PerformLayout();
            this.gbTranslatedText.ResumeLayout(false);
            this.gbTranslatedText.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private UserControls.OkCancelButtons okCancelApplyButtons;
        private System.Windows.Forms.GroupBox gbResourceEntries;
        private Controls.AdvancedDataGridView gridResources;
        private System.Windows.Forms.TableLayoutPanel pnlEditResourceEntry;
        private System.Windows.Forms.GroupBox gbOriginalText;
        private System.Windows.Forms.GroupBox gbTranslatedText;
        private System.Windows.Forms.Splitter splitterEditResources;
        private KGySoft.Drawing.ImagingTools.View.Controls.AdvancedTextBox txtOriginalText;
        private KGySoft.Drawing.ImagingTools.View.Controls.AdvancedTextBox txtTranslatedText;
        private System.Windows.Forms.BindingSource bindingSource;
        private System.Windows.Forms.GroupBox gbResourceFile;
        private System.Windows.Forms.ComboBox cmbResourceFiles;
        private System.Windows.Forms.DataGridViewTextBoxColumn colResourceKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOriginalText;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTranslatedText;
        private Controls.AutoMirrorPanel pnlFilter;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Label lblFilter;
        private Controls.AdvancedCheckBox chbHideDependencies;
    }
}