namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal partial class DownloadResourcesControl
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
            this.bindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.okCancelButtons = new KGySoft.Drawing.ImagingTools.View.UserControls.OkCancelButtons();
            this.gridDownloadableResources = new KGySoft.Drawing.ImagingTools.View.Controls.AdvancedDataGridView();
            this.colSelected = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colLanguage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAuthor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colImagingToolsVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.progress = new KGySoft.Drawing.ImagingTools.View.Controls.DownloadProgressFooter();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDownloadableResources)).BeginInit();
            this.SuspendLayout();
            // 
            // bindingSource
            // 
            this.bindingSource.DataSource = typeof(KGySoft.Drawing.ImagingTools.ViewModel.DownloadableResourceItem);
            // 
            // okCancelButtons
            // 
            this.okCancelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.okCancelButtons.Location = new System.Drawing.Point(3, 166);
            this.okCancelButtons.Name = "okCancelButtons";
            this.okCancelButtons.Size = new System.Drawing.Size(358, 35);
            this.okCancelButtons.TabIndex = 1;
            // 
            // gridDownloadableResources
            // 
            this.gridDownloadableResources.AllowUserToAddRows = false;
            this.gridDownloadableResources.AllowUserToDeleteRows = false;
            this.gridDownloadableResources.AutoGenerateColumns = false;
            this.gridDownloadableResources.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSelected,
            this.colLanguage,
            this.colAuthor,
            this.colImagingToolsVersion,
            this.colDescription});
            this.gridDownloadableResources.DataSource = this.bindingSource;
            this.gridDownloadableResources.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridDownloadableResources.Location = new System.Drawing.Point(3, 3);
            this.gridDownloadableResources.MultiSelect = false;
            this.gridDownloadableResources.Name = "gridDownloadableResources";
            this.gridDownloadableResources.Size = new System.Drawing.Size(358, 163);
            this.gridDownloadableResources.TabIndex = 0;
            // 
            // colSelected
            // 
            this.colSelected.DataPropertyName = "Selected";
            this.colSelected.HeaderText = "colSelected";
            this.colSelected.Name = "colSelected";
            this.colSelected.Width = 50;
            // 
            // colLanguage
            // 
            this.colLanguage.DataPropertyName = "Language";
            this.colLanguage.HeaderText = "colLanguage";
            this.colLanguage.Name = "colLanguage";
            this.colLanguage.ReadOnly = true;
            // 
            // colAuthor
            // 
            this.colAuthor.DataPropertyName = "Author";
            this.colAuthor.HeaderText = "colAuthor";
            this.colAuthor.Name = "colAuthor";
            this.colAuthor.ReadOnly = true;
            // 
            // colImagingToolsVersion
            // 
            this.colImagingToolsVersion.DataPropertyName = "ImagingToolsVersion";
            this.colImagingToolsVersion.HeaderText = "colImagingToolsVersion";
            this.colImagingToolsVersion.Name = "colImagingToolsVersion";
            this.colImagingToolsVersion.ReadOnly = true;
            this.colImagingToolsVersion.Width = 60;
            // 
            // colDescription
            // 
            this.colDescription.DataPropertyName = "Description";
            this.colDescription.HeaderText = "colDescription";
            this.colDescription.Name = "colDescription";
            this.colDescription.ReadOnly = true;
            this.colDescription.Width = 120;
            // 
            // progress
            // 
            this.progress.BackColor = System.Drawing.Color.Transparent;
            this.progress.Location = new System.Drawing.Point(3, 201);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(358, 22);
            this.progress.TabIndex = 2;
            // 
            // DownloadResourcesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Size = new System.Drawing.Size(364, 226);
            this.Controls.Add(this.gridDownloadableResources);
            this.Controls.Add(this.okCancelButtons);
            this.Controls.Add(this.progress);
            this.Name = "DownloadResourcesControl";
            this.Padding = new System.Windows.Forms.Padding(3);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDownloadableResources)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.DownloadProgressFooter progress;
        private Controls.AdvancedDataGridView gridDownloadableResources;
        private System.Windows.Forms.BindingSource bindingSource;
        private UserControls.OkCancelButtons okCancelButtons;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colSelected;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLanguage;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAuthor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colImagingToolsVersion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescription;
    }
}