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
            this.pnlPalette = new KGySoft.Drawing.ImagingTools.View.Controls.PalettePanel();
            this.gbSelectedColor = new System.Windows.Forms.GroupBox();
            this.colorVisualizerControl = new KGySoft.Drawing.ImagingTools.View.UserControls.ColorVisualizerControl();
            this.okCancelButtons = new KGySoft.Drawing.ImagingTools.View.UserControls.OkCancelButtons();
            this.gbPalette.SuspendLayout();
            this.gbSelectedColor.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbPalette
            // 
            this.gbPalette.Controls.Add(this.pnlPalette);
            this.gbPalette.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbPalette.Location = new System.Drawing.Point(3, 3);
            this.gbPalette.Name = "gbPalette";
            this.gbPalette.Size = new System.Drawing.Size(241, 233);
            this.gbPalette.TabIndex = 0;
            this.gbPalette.TabStop = false;
            // 
            // pnlPalette
            // 
            this.pnlPalette.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPalette.Location = new System.Drawing.Point(3, 16);
            this.pnlPalette.Name = "pnlPalette";
            this.pnlPalette.Size = new System.Drawing.Size(235, 214);
            this.pnlPalette.TabIndex = 0;
            // 
            // gbSelectedColor
            // 
            this.gbSelectedColor.Controls.Add(this.colorVisualizerControl);
            this.gbSelectedColor.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gbSelectedColor.Location = new System.Drawing.Point(3, 236);
            this.gbSelectedColor.Name = "gbSelectedColor";
            this.gbSelectedColor.Size = new System.Drawing.Size(241, 216);
            this.gbSelectedColor.TabIndex = 1;
            this.gbSelectedColor.TabStop = false;
            // 
            // ucColorVisualizer
            // 
            this.colorVisualizerControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.colorVisualizerControl.Location = new System.Drawing.Point(3, 16);
            this.colorVisualizerControl.Name = "colorVisualizerControl";
            this.colorVisualizerControl.Size = new System.Drawing.Size(235, 197);
            this.colorVisualizerControl.TabIndex = 0;
            // 
            // okCancelButtons
            // 
            this.okCancelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.okCancelButtons.Location = new System.Drawing.Point(3, 452);
            this.okCancelButtons.Name = "okCancelButtons";
            this.okCancelButtons.Size = new System.Drawing.Size(241, 35);
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
            this.MaximumSize = new System.Drawing.Size(280, 32767);
            this.MinimumSize = new System.Drawing.Size(255, 335);
            this.Name = "PaletteVisualizerForm";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.gbPalette.ResumeLayout(false);
            this.gbSelectedColor.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbPalette;
        private PalettePanel pnlPalette;
        private System.Windows.Forms.GroupBox gbSelectedColor;
        private ColorVisualizerControl colorVisualizerControl;
        private OkCancelButtons okCancelButtons;
    }
}