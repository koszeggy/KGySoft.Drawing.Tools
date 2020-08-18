namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    partial class PreviewImageControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.scalingToolStrip1 = new KGySoft.Drawing.ImagingTools.View.Controls.ScalingToolStrip();
            this.btnAutoZoom = new System.Windows.Forms.ToolStripButton();
            this.btnAntiAlias = new System.Windows.Forms.ToolStripButton();
            this.ivPreview = new KGySoft.Drawing.ImagingTools.View.Controls.ImageViewer();
            this.scalingToolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // scalingToolStrip1
            // 
            this.scalingToolStrip1.Dock = System.Windows.Forms.DockStyle.Left;
            this.scalingToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAutoZoom,
            this.btnAntiAlias});
            this.scalingToolStrip1.Location = new System.Drawing.Point(0, 0);
            this.scalingToolStrip1.Name = "scalingToolStrip1";
            this.scalingToolStrip1.Size = new System.Drawing.Size(24, 150);
            this.scalingToolStrip1.TabIndex = 0;
            this.scalingToolStrip1.Text = "tsMenu";
            // 
            // btnAutoZoom
            // 
            this.btnAutoZoom.CheckOnClick = true;
            this.btnAutoZoom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAutoZoom.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAutoZoom.Name = "btnAutoZoom";
            this.btnAutoZoom.Size = new System.Drawing.Size(21, 4);
            // 
            // btnAntiAlias
            // 
            this.btnAntiAlias.CheckOnClick = true;
            this.btnAntiAlias.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAntiAlias.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAntiAlias.Name = "btnAntiAlias";
            this.btnAntiAlias.Size = new System.Drawing.Size(21, 4);
            // 
            // ivPreview
            // 
            this.ivPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ivPreview.Location = new System.Drawing.Point(24, 0);
            this.ivPreview.Name = "ivPreview";
            this.ivPreview.Size = new System.Drawing.Size(126, 150);
            this.ivPreview.TabIndex = 1;
            // 
            // PreviewBitmapControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ivPreview);
            this.Controls.Add(this.scalingToolStrip1);
            this.Name = "PreviewImageControl";
            this.scalingToolStrip1.ResumeLayout(false);
            this.scalingToolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.ScalingToolStrip scalingToolStrip1;
        private System.Windows.Forms.ToolStripButton btnAutoZoom;
        private System.Windows.Forms.ToolStripButton btnAntiAlias;
        private Controls.ImageViewer ivPreview;
    }
}
