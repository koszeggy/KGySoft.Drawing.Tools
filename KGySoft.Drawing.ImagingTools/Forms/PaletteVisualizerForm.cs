#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: PaletteVisualizerForm.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2019 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution. If not, then this file is considered as
//  an illegal copy.
//
//  Unauthorized copying of this file, via any medium is strictly prohibited.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace KGySoft.Drawing.ImagingTools.Forms
{
    internal partial class PaletteVisualizerForm : BaseForm
    {
        #region Fields

        private IList<Color> palette;
        private bool readOnly;
        private bool changed;

        #endregion

        #region Properties

        internal IList<Color> Palette
        {
            get => palette;
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

        internal bool PaletteChanged => changed;

        #endregion

        #region Constructors

        public PaletteVisualizerForm()
        {
            InitializeComponent();
            ucColorVisualizer.ColorEdited += new EventHandler(ucColorVisualizer_ColorEdited);
            pnlPalette.SelectedColorChanged += new System.EventHandler(this.pnlPalette_SelectedColorChanged);
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();

            ucColorVisualizer.ColorEdited -= ucColorVisualizer_ColorEdited;
            pnlPalette.SelectedColorChanged -= pnlPalette_SelectedColorChanged;

            base.Dispose(disposing);
        }

        #endregion

        #region Event handlers
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

        #endregion
    }
}
