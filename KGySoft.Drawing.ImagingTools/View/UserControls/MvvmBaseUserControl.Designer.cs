﻿namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    partial class MvvmBaseUserControl
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
            this.toolTip.InitialDelay = 500;
            this.toolTip.ReshowDelay = 100;
            // 
            // MvvmBaseUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "MvvmBaseUserControl";
            this.Size = new System.Drawing.Size(284, 261);
            this.ResumeLayout(false);

        }
    }
}