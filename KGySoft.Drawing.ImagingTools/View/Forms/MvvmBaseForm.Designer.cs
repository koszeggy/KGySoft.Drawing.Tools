namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    partial class MvvmBaseForm<TViewModel>
    {
        private System.Windows.Forms.ToolTip toolTip;
        private System.ComponentModel.IContainer components;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 10000;
            // 
            // MvvmBaseForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "MvvmBaseForm";
            this.ResumeLayout(false);
        }
    }
}