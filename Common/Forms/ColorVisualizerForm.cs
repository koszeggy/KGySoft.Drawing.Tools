#region Used namespaces

using System;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace KGySoft.DebuggerVisualizers.Common
{
    internal partial class ColorVisualizerForm : Form
    {
        #region Fields

        private bool changed;

        #endregion

        #region Properties

        internal bool ReadOnly
        {
            get { return ucColorVisualizer.ReadOnly; }
            set { ucColorVisualizer.ReadOnly = value; }
        }

        internal Color Color
        {
            get { return ucColorVisualizer.Color; }
            set
            {
                ucColorVisualizer.Color = value;
                UpdateInfo();
            }
        }

        internal bool ColorChanged
        {
            get { return changed; }
        }

        #endregion

        #region Construction and Destruction

        #region Constructors

        internal ColorVisualizerForm()
        {
            InitializeComponent();
            ucColorVisualizer.ColorEdited += new EventHandler(ucColorVisualizer_ColorEdited);
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
            base.Dispose(disposing);
        }

        #endregion

        #endregion

        #region Methods

        private void UpdateInfo()
        {
            Text = "Color: " + Color.Name;
        }

        #endregion

        #region Event Handlers
        //ReSharper disable InconsistentNaming

        void ucColorVisualizer_ColorEdited(object sender, EventArgs e)
        {
            if (ReadOnly)
                return;

            UpdateInfo();
            changed = true;
        }

        //ReSharper restore InconsistentNaming
        #endregion
    }
}
