#region Used namespaces

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace KGySoft.Drawing.ImagingTools.Forms
{
    internal partial class PaletteVisualizerForm : Form
    {
        #region Fields

        private IList<Color> palette;
        private bool readOnly;
        private bool changed;

        #endregion

        #region Properties

        internal IList<Color> Palette
        {
            get { return palette; }
            set
            {
                palette = value;
                readOnly = palette == null || !(palette is Array) && palette.IsReadOnly;
                pnlPalette.Palette = value;
                Text = String.Format("Palette Count: {0}", palette == null ? 0 : palette.Count);
                ucColorVisualizer.ReadOnly = readOnly || palette == null || palette.Count == 0;

                if (palette == null || palette.Count == 0)
                    return;

                pnlPalette.SelectedColorIndex = 0;
            }
        }

        internal bool PaletteChanged
        {
            get { return changed; }
        }

        #endregion

        #region Construction and Destruction

        #region Constructors

        public PaletteVisualizerForm()
        {
            InitializeComponent();
            ucColorVisualizer.ColorEdited += new EventHandler(ucColorVisualizer_ColorEdited);
            pnlPalette.SelectedColorChanged += new System.EventHandler(this.pnlPalette_SelectedColorChanged);
        }

        #endregion

        #region Explicit Disposing

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            ucColorVisualizer.ColorEdited -= ucColorVisualizer_ColorEdited;
            pnlPalette.SelectedColorChanged -= pnlPalette_SelectedColorChanged;

            base.Dispose(disposing);
        }

        #endregion

        #endregion

        #region Event Handlers
        //ReSharper disable InconsistentNaming

        void ucColorVisualizer_ColorEdited(object sender, EventArgs e)
        {
            if (readOnly)
                return;

            // if there is no cloning, both of the following lines set the same instance in the collection but it is clear to set twice
            palette[pnlPalette.SelectedColorIndex] = ucColorVisualizer.Color;
            pnlPalette.SelectedColor = ucColorVisualizer.Color;
            changed = true;
        }

        private void pnlPalette_SelectedColorChanged(object sender, EventArgs e)
        {
            ucColorVisualizer.SpecialInfo = String.Format("Selected index: {0}", pnlPalette.SelectedColorIndex);
            ucColorVisualizer.Color = pnlPalette.SelectedColor;
        }

        //ReSharper restore InconsistentNaming
        #endregion
    }
}
