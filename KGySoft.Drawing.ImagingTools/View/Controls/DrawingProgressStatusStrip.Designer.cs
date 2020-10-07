using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    partial class DrawingProgressStatusStrip
    {
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblProgress = new System.Windows.Forms.ToolStripStatusLabel();
            this.pbProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // lblProgress
            // 
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(0, 17);
            // 
            // pbProgress
            // 
            this.pbProgress.AutoSize = false;
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(100, 16);
            // 
            // timer
            // 
            this.timer.Interval = 30;
            // 
            // DrawingProgressStatusStrip
            // 
            this.BackColor = System.Drawing.Color.Transparent;
            this.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblProgress,
            this.pbProgress});
            this.SizingGrip = false;
            this.ResumeLayout(false);

        }

        private ToolStripStatusLabel lblProgress;
        private ToolStripProgressBar pbProgress;
        private Timer timer;
        private System.ComponentModel.IContainer components;
    }
}
