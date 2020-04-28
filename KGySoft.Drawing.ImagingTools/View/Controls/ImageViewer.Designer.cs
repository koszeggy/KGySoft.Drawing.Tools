using System.ComponentModel;
using System.Windows.Forms;

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    partial class ImageViewer
    {
        private IContainer components;

        private void InitializeComponent()
        {
            this.hScrollBar = new System.Windows.Forms.HScrollBar();
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.SuspendLayout();
            // 
            // hScrollBar
            // 
            this.hScrollBar.Location = new System.Drawing.Point(0, 0);
            this.hScrollBar.Name = "hScrollBar";
            this.hScrollBar.Size = new System.Drawing.Size(80, 17);
            this.hScrollBar.TabIndex = 0;
            this.hScrollBar.Visible = false;
            // 
            // vScrollBar
            // 
            this.vScrollBar.Location = new System.Drawing.Point(0, 0);
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size(17, 80);
            this.vScrollBar.TabIndex = 0;
            this.vScrollBar.Visible = false;
            // 
            // ImageViewer
            // 
            this.Controls.Add(this.hScrollBar);
            this.Controls.Add(this.vScrollBar);
            this.ResumeLayout(false);

        }

        private HScrollBar hScrollBar;
        private VScrollBar vScrollBar;
    }
}
