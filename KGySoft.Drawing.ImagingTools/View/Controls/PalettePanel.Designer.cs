using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    partial class PalettePanel
    {
        private IContainer components;

        private void InitializeComponent()
        {
            components = new Container();
            sbPalette = new VScrollBar();
            timerSelection = new Timer(components);
            SuspendLayout();
            // 
            // sbPalette
            // 
            sbPalette.Dock = DockStyle.Right;
            sbPalette.Location = new Point(183, 0);
            sbPalette.Name = "sbPalette";
            sbPalette.Size = new Size(17, 100);
            sbPalette.TabIndex = 0;
            sbPalette.Visible = false;
            sbPalette.ValueChanged += new EventHandler(sbPalette_ValueChanged);
            // 
            // timerSelection
            // 
            timerSelection.Interval = 20;
            timerSelection.Tick += new EventHandler(timerSelection_Tick);
            // 
            // PalettePanel
            // 
            Controls.Add(sbPalette);
            ResumeLayout(false);
        }

        private VScrollBar sbPalette;
        private Timer timerSelection;
    }
}
