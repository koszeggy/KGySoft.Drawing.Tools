
using KGySoft.Drawing.ImagingTools.View.UserControls;

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    partial class ColorVisualizerForm
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
            this.ucColorVisualizer = new KGySoft.Drawing.ImagingTools.View.UserControls.ColorVisualizerControl();
            this.okCancelButtons = new KGySoft.Drawing.ImagingTools.View.UserControls.OkCancelButtons();
            this.SuspendLayout();
            // 
            // ucColorVisualizer
            // 
            this.ucColorVisualizer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucColorVisualizer.Location = new System.Drawing.Point(0, 0);
            this.ucColorVisualizer.Name = "ucColorVisualizer";
            this.ucColorVisualizer.Size = new System.Drawing.Size(244, 201);
            this.ucColorVisualizer.TabIndex = 1;
            // 
            // okCancelButtons
            // 
            this.okCancelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.okCancelButtons.Location = new System.Drawing.Point(0, 201);
            this.okCancelButtons.Name = "okCancelButtons";
            this.okCancelButtons.Size = new System.Drawing.Size(244, 35);
            this.okCancelButtons.TabIndex = 2;
            // 
            // ColorVisualizerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 241);
            this.Controls.Add(this.ucColorVisualizer);
            this.Controls.Add(this.okCancelButtons);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(260, 234);
            this.Name = "ColorVisualizerForm";
            this.ResumeLayout(false);

        }

        #endregion

        private ColorVisualizerControl ucColorVisualizer;
        private OkCancelButtons okCancelButtons;
    }
}