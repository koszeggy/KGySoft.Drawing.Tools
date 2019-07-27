using KGySoft.Drawing.ImagingTools.UserControls;

namespace KGySoft.Drawing.ImagingTools.Forms
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
            this.ucColorVisualizer = new ColorVisualizerControl();
            this.SuspendLayout();
            // 
            // ucColorVisualizer
            // 
            this.ucColorVisualizer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucColorVisualizer.Location = new System.Drawing.Point(0, 0);
            this.ucColorVisualizer.Name = "ucColorVisualizer";
            this.ucColorVisualizer.Size = new System.Drawing.Size(244, 200);
            this.ucColorVisualizer.TabIndex = 1;
            // 
            // ColorVisualizerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 200);
            this.Controls.Add(this.ucColorVisualizer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(260, 234);
            this.Name = "ColorVisualizerForm";
            this.Text = "ColorVisualizerForm";
            this.ResumeLayout(false);

        }

        #endregion

        private ColorVisualizerControl ucColorVisualizer;
    }
}