using KGySoft.Drawing.ImagingTools.View.Components;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreviewImageControl));
            this.ivPreview = new KGySoft.Drawing.ImagingTools.View.Controls.ImageViewer();
            this.tsMenu = new KGySoft.Drawing.ImagingTools.View.Controls.ScalingToolStrip();
            this.btnZoom = new ZoomSplitButton();
            this.btnAntiAlias = new System.Windows.Forms.ToolStripButton();
            this.btnShowOriginal = new System.Windows.Forms.ToolStripButton();
            this.tsMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // ivPreview
            // 
            this.ivPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ivPreview.Location = new System.Drawing.Point(33, 0);
            this.ivPreview.Name = "ivPreview";
            this.ivPreview.Size = new System.Drawing.Size(117, 150);
            this.ivPreview.TabIndex = 1;
            // 
            // tsMenu
            // 
            this.tsMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.tsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnZoom,
            this.btnAntiAlias,
            this.btnShowOriginal});
            this.tsMenu.Location = new System.Drawing.Point(0, 0);
            this.tsMenu.Name = "tsMenu";
            this.tsMenu.Size = new System.Drawing.Size(33, 150);
            this.tsMenu.TabIndex = 0;
            this.tsMenu.Text = "tsMenu";
            // 
            // btnZoom
            // 
            this.btnZoom.CheckOnClick = true;
            this.btnZoom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnZoom.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoom.Name = "btnZoom";
            this.btnZoom.Size = new System.Drawing.Size(30, 20);
            // 
            // btnAntiAlias
            // 
            this.btnAntiAlias.CheckOnClick = true;
            this.btnAntiAlias.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAntiAlias.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAntiAlias.Name = "btnAntiAlias";
            this.btnAntiAlias.Size = new System.Drawing.Size(30, 4);
            // 
            // btnShowOriginal
            // 
            this.btnShowOriginal.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnShowOriginal.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnShowOriginal.Name = "btnShowOriginal";
            this.btnShowOriginal.Size = new System.Drawing.Size(30, 4);
            // 
            // PreviewImageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ivPreview);
            this.Controls.Add(this.tsMenu);
            this.Name = "PreviewImageControl";
            this.tsMenu.ResumeLayout(false);
            this.tsMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.ScalingToolStrip tsMenu;
        private ZoomSplitButton btnZoom;
        private System.Windows.Forms.ToolStripButton btnAntiAlias;
        private Controls.ImageViewer ivPreview;
        private System.Windows.Forms.ToolStripButton btnShowOriginal;
    }
}
