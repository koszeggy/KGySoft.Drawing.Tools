using System.ComponentModel;
using System.Windows.Forms;

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    partial class ImageViewer
    {
        private void InitializeComponent()
        {
            this.sbHorizontal = new System.Windows.Forms.HScrollBar();
            this.sbVertical = new System.Windows.Forms.VScrollBar();
            this.SuspendLayout();
            // 
            // sbHorizontal
            // 
            this.sbHorizontal.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.sbHorizontal.Location = new System.Drawing.Point(0, 0);
            this.sbHorizontal.Name = "sbHorizontal";
            this.sbHorizontal.Size = new System.Drawing.Size(80, 17);
            this.sbHorizontal.TabIndex = 0;
            this.sbHorizontal.Visible = false;
            // 
            // sbVertical
            // 
            this.sbVertical.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.sbVertical.Location = new System.Drawing.Point(0, 0);
            this.sbVertical.Name = "sbVertical";
            this.sbVertical.Size = new System.Drawing.Size(17, 80);
            this.sbVertical.TabIndex = 0;
            this.sbVertical.Visible = false;
            // 
            // ImageViewer
            // 
            this.Controls.Add(this.sbHorizontal);
            this.Controls.Add(this.sbVertical);
            this.ResumeLayout(false);

        }

        private HScrollBar sbHorizontal;
        private VScrollBar sbVertical;
    }
}
