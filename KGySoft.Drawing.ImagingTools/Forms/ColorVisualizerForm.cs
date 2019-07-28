#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorVisualizerForm.cs
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
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace KGySoft.Drawing.ImagingTools.Forms
{
    internal partial class ColorVisualizerForm : BaseForm
    {
        #region Fields

        private bool changed;

        #endregion

        #region Properties

        internal bool ReadOnly
        {
            get => ucColorVisualizer.ReadOnly;
            set => ucColorVisualizer.ReadOnly = value;
        }

        internal Color Color
        {
            get => ucColorVisualizer.Color;
            set
            {
                ucColorVisualizer.Color = value;
                UpdateInfo();
            }
        }

        internal bool ColorChanged => changed;

        #endregion

        #region Constructors

        internal ColorVisualizerForm()
        {
            InitializeComponent();
            ucColorVisualizer.ColorEdited += new EventHandler(ucColorVisualizer_ColorEdited);
        }

        #endregion

        #region Methods

        #region Protected Methods

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();

            ucColorVisualizer.ColorEdited -= ucColorVisualizer_ColorEdited;
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void UpdateInfo() => Text = "Color: " + Color.Name;

        #endregion

        #region Event handlers

        void ucColorVisualizer_ColorEdited(object sender, EventArgs e)
        {
            if (ReadOnly)
                return;

            UpdateInfo();
            changed = true;
        }

        #endregion

        #endregion
    }
}
