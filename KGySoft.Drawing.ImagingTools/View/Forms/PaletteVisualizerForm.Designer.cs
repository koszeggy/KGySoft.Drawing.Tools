using KGySoft.Drawing.ImagingTools.View.Controls;
using KGySoft.Drawing.ImagingTools.View.UserControls;

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    partial class PaletteVisualizerForm
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
            this.gbPalette = new System.Windows.Forms.GroupBox();
            this.pnlPalette = new PalettePanel();
            this.gbSelectedColor = new System.Windows.Forms.GroupBox();
            this.ucColorVisualizer = new KGySoft.Drawing.ImagingTools.View.UserControls.ColorVisualizerControl();
            this.okCancelButtons = new KGySoft.Drawing.ImagingTools.View.UserControls.OkCancelButtons();
            this.gbPalette.SuspendLayout();
            this.gbSelectedColor.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbPalette
            // 
            this.gbPalette.Controls.Add(this.pnlPalette);
            this.gbPalette.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbPalette.Location = new System.Drawing.Point(0, 0);
            this.gbPalette.Name = "gbPalette";
            this.gbPalette.Size = new System.Drawing.Size(247, 234);
            this.gbPalette.TabIndex = 0;
            this.gbPalette.TabStop = false;
            // 
            // pnlPalette
            // 
            this.pnlPalette.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPalette.Location = new System.Drawing.Point(3, 16);
            this.pnlPalette.Name = "pnlPalette";
            this.pnlPalette.Size = new System.Drawing.Size(241, 215);
            this.pnlPalette.TabIndex = 0;
            this.pnlPalette.TabStop = true;
            // 
            // gbSelectedColor
            // 
            this.gbSelectedColor.Controls.Add(this.ucColorVisualizer);
            this.gbSelectedColor.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gbSelectedColor.Location = new System.Drawing.Point(0, 234);
            this.gbSelectedColor.Name = "gbSelectedColor";
            this.gbSelectedColor.Size = new System.Drawing.Size(247, 216);
            this.gbSelectedColor.TabIndex = 1;
            this.gbSelectedColor.TabStop = false;
            // 
            // ucColorVisualizer
            // 
            this.ucColorVisualizer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucColorVisualizer.Location = new System.Drawing.Point(3, 16);
            this.ucColorVisualizer.Name = "ucColorVisualizer";
            this.ucColorVisualizer.Size = new System.Drawing.Size(241, 197);
            this.ucColorVisualizer.TabIndex = 0;
            // 
            // okCancelButtons
            // 
            this.okCancelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.okCancelButtons.Location = new System.Drawing.Point(0, 450);
            this.okCancelButtons.Name = "okCancelButtons";
            this.okCancelButtons.Size = new System.Drawing.Size(247, 40);
            this.okCancelButtons.TabIndex = 2;
            // 
            // PaletteVisualizerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(247, 490);
            this.Controls.Add(this.gbPalette);
            this.Controls.Add(this.gbSelectedColor);
            this.Controls.Add(this.okCancelButtons);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximumSize = new System.Drawing.Size(280, 32867);
            this.MinimumSize = new System.Drawing.Size(255, 300);
            this.Name = "PaletteVisualizerForm";
            this.gbPalette.ResumeLayout(false);
            this.gbSelectedColor.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbPalette;
        private PalettePanel pnlPalette;
        private System.Windows.Forms.GroupBox gbSelectedColor;
        private ColorVisualizerControl ucColorVisualizer;
        private OkCancelButtons okCancelButtons;
    }
}