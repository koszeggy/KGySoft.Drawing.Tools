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
            this.btnZoom = new KGySoft.Drawing.ImagingTools.View.Controls.ZoomSplitButton();
            this.btnAntiAlias = new System.Windows.Forms.ToolStripButton();
            this.btnShowOriginal = new System.Windows.Forms.ToolStripButton();
            this.ivPreview = new KGySoft.Drawing.ImagingTools.View.Controls.ImageViewer();
            this.scalingToolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // scalingToolStrip1
            // 
            this.scalingToolStrip1.Dock = System.Windows.Forms.DockStyle.Left;
            this.scalingToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnZoom,
            this.btnAntiAlias,
            this.btnShowOriginal});
            this.scalingToolStrip1.Location = new System.Drawing.Point(0, 0);
            this.scalingToolStrip1.Name = "scalingToolStrip1";
            this.scalingToolStrip1.Size = new System.Drawing.Size(32, 150);
            this.scalingToolStrip1.TabIndex = 0;
            this.scalingToolStrip1.Text = "tsMenu";
            // 
            // btnAutoZoom
            // 
            this.btnZoom.CheckOnClick = true;
            this.btnZoom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnZoom.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoom.Name = "btnZoom";
            this.btnZoom.Size = new System.Drawing.Size(29, 4);
            // 
            // btnAntiAlias
            // 
            this.btnAntiAlias.CheckOnClick = true;
            this.btnAntiAlias.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAntiAlias.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAntiAlias.Name = "btnAntiAlias";
            this.btnAntiAlias.Size = new System.Drawing.Size(29, 4);
            // 
            // btnShowOriginal
            // 
            this.btnShowOriginal.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnShowOriginal.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnShowOriginal.Name = "btnShowOriginal";
            this.btnShowOriginal.Size = new System.Drawing.Size(29, 4);
            // 
            // ivPreview
            // 
            this.ivPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ivPreview.Location = new System.Drawing.Point(32, 0);
            this.ivPreview.Name = "ivPreview";
            this.ivPreview.Size = new System.Drawing.Size(118, 150);
            this.ivPreview.TabIndex = 1;
            // 
            // PreviewImageControl
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
        private Controls.ZoomSplitButton btnZoom;
        private System.Windows.Forms.ToolStripButton btnAntiAlias;
        private Controls.ImageViewer ivPreview;
        private System.Windows.Forms.ToolStripButton btnShowOriginal;
    }
}
