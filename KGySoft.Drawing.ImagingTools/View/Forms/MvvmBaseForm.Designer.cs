namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    partial class MvvmBaseForm
    {
        private KGySoft.Drawing.ImagingTools.View.Components.AdvancedToolTip toolTip;
        private System.ComponentModel.IContainer components;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolTip = new KGySoft.Drawing.ImagingTools.View.Components.AdvancedToolTip(this.components);
            this.SuspendLayout();
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 10000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.ReshowDelay = 100;
            // 
            // MvvmBaseForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "MvvmBaseForm";
            this.RightToLeftLayout = true;
            this.ResumeLayout(false);
        }
    }
}