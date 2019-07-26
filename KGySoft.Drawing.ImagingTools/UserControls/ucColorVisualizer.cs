#region Used namespaces

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using KGySoft.CoreLibraries;

#endregion

namespace KGySoft.Drawing.ImagingTools.UserControls
{
    internal partial class ucColorVisualizer : UserControl
    {
        #region Fields

        #region Static Fields

        private static Dictionary<int, string> knownColors;
        private static Dictionary<int, string> systemColors;

        #endregion

        #region Instance Fields

        private bool readOnly;
        private Color color;
        private TextureBrush alphaBrush;

        #endregion

        #endregion

        #region Events

        internal event EventHandler ColorEdited;

        #endregion

        #region Properties

        #region Static Properties

        private static Dictionary<int, string> KnownColors
        {
            get
            {
                if (knownColors == null)
                {
                    knownColors = new Dictionary<int, string>();

                    // non-system known colors: 27..168 (Transparent..YellowGreen)
                    for (KnownColor color = (KnownColor)27; color <= (KnownColor)167; color++)
                    {
                        int argb = Color.FromKnownColor(color).ToArgb();
                        string name;
                        if (knownColors.TryGetValue(argb, out name))
                            knownColors[argb] = name + ", " + Enum<KnownColor>.ToString(color);
                        else
                            knownColors.Add(argb, Enum<KnownColor>.ToString(color));
                    }
                }

                return knownColors;
            }
        }

        private static Dictionary<int, string> SystemColors
        {
            get
            {
                if (systemColors == null)
                {
                    systemColors = new Dictionary<int, string>();

                    // system colors: 1.. 174, except 27..168
                    for (KnownColor color = (KnownColor)1; color <= (KnownColor)174; color++)
                    {
                        if (color == (KnownColor)27)
                            color = (KnownColor)168;
                        int argb = Color.FromKnownColor(color).ToArgb();
                        string name;
                        if (systemColors.TryGetValue(argb, out name))
                            systemColors[argb] = name + ", " + Enum<KnownColor>.ToString(color);
                        else
                            systemColors.Add(argb, Enum<KnownColor>.ToString(color));
                    }
                }

                return systemColors;
            }
        }

        #endregion

        #region Instance Properties

        internal bool ReadOnly
        {
            get { return readOnly; }
            set
            {
                readOnly = value;
                tsMenu.Visible = !readOnly;
                tbAlpha.Visible = !readOnly;
                tbRed.Visible = !readOnly;
                tbGreen.Visible = !readOnly;
                tbBlue.Visible = !readOnly;
            }
        }

        internal Color Color
        {
            get { return color; }
            set
            {
                color = value;
                ColorUpdated();
            }
        }

        internal string SpecialInfo { get; set; }

        #endregion

        #endregion

        #region Construction and Destruction

        #region Constructors

        public ucColorVisualizer()
        {
            InitializeComponent();
            btnEdit.Image = Properties.Resources.Palette;
            pnlColor.SetDoubleBuffered(true);
            tbAlpha.SetDoubleBuffered(true);
            tbRed.SetDoubleBuffered(true);
            tbGreen.SetDoubleBuffered(true);
            tbBlue.SetDoubleBuffered(true);
            txtColor.SetDoubleBuffered(true);

            SystemColorsChanged += new EventHandler(ucColorVisualizer_SystemColorsChanged);
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
                if (alphaBrush != null)
                    alphaBrush.Dispose();
            }

            alphaBrush = null;
            pnlAlpha.Paint -= pnlColor_Paint;
            pnlColor.Paint -= pnlColor_Paint;
            btnEdit.Click -= btnEdit_Click;
            tbAlpha.Scroll -= tbAlpha_Scroll;
            tbRed.Scroll -= tbRed_Scroll;
            tbGreen.Scroll -= tbGreen_Scroll;
            tbBlue.Scroll -= tbBlue_Scroll;
            SystemColorsChanged -= ucColorVisualizer_SystemColorsChanged;
            base.Dispose(disposing);
        }

        #endregion

        #endregion

        #region Methods

        #region Static Methods

        private static string GetKnownColor(Color color)
        {
            string name;
            if (KnownColors.TryGetValue(color.ToArgb(), out name))
                return name;

            return "-";
        }

        private static string GetSystemColors(Color color)
        {
            string name;
            if (SystemColors.TryGetValue(color.ToArgb(), out name))
                return name;

            return "-";
        }

        #endregion

        #region Instance Methods

        private void ColorUpdated()
        {
            byte a = color.A;
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;

            tbAlpha.Value = a;
            tbRed.Value = r;
            tbGreen.Value = g;
            tbBlue.Value = b;

            lblAlpha.Text = String.Format("A: {0}", a);
            lblRed.Text = String.Format("R: {0}", r);
            lblGreen.Text = String.Format("G: {0}", g);
            lblBlue.Text = String.Format("B: {0}", b);

            pnlAlpha.Invalidate();
            tbAlpha.BackColor = Color.FromArgb(a, a, a);
            tbRed.BackColor = pnlRed.BackColor = Color.FromArgb(r, 0, 0);
            tbGreen.BackColor = pnlGreen.BackColor = Color.FromArgb(0, g, 0);
            tbBlue.BackColor = pnlBlue.BackColor = Color.FromArgb(0, 0, b);
            pnlColor.Invalidate();
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            StringBuilder sb = new StringBuilder();
            if (!String.IsNullOrEmpty(SpecialInfo))
                sb.AppendLine(SpecialInfo);
            sb.AppendFormat("ARGB value: {1:X8} ({1}){0}Equivalent known color(s): {2}{0}Equivalent System color(s): {3}{0}Hue: {4:F0}°{0}Saturation: {5:F0}%{0}Brightness: {6:F0}%",
                Environment.NewLine, color.ToArgb(), GetKnownColor(color), GetSystemColors(color), color.GetHue(), color.GetSaturation() * 100f, color.GetBrightness() * 100f);
            txtColor.Text = sb.ToString();
        }

        private void CreateAlphaBrush()
        {
            Bitmap bmpPattern = new Bitmap(10, 10);
            using (Graphics g = Graphics.FromImage(bmpPattern))
            {
                g.Clear(Color.White);
                Rectangle smallRect = new Rectangle(Point.Empty, new Size(bmpPattern.Width >> 1, bmpPattern.Height >> 1));
                g.FillRectangle(Brushes.Silver, smallRect);
                smallRect.Offset(smallRect.Width, smallRect.Height);
                g.FillRectangle(Brushes.Silver, smallRect);
            }

            alphaBrush = new TextureBrush(bmpPattern);
        }

        private void OnColorEdited()
        {
            if (ColorEdited != null)
                ColorEdited.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #endregion

        #region Event Handlers
        //ReSharper disable InconsistentNaming

        private void pnlColor_Paint(object sender, PaintEventArgs e)
        {
            // painting checked background
            if (color.A != 255)
            {
                if (alphaBrush == null)
                    CreateAlphaBrush();

                e.Graphics.FillRectangle(alphaBrush, e.ClipRectangle);
            }

            Color backColor = sender == pnlAlpha ? backColor = Color.FromArgb(color.A, Color.White) : color;
            using (Brush b = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(b, e.ClipRectangle);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (readOnly)
                return;

            colorDialog.Color = color;
            if (colorDialog.ShowDialog(this) == DialogResult.OK)
            {
                Color = colorDialog.Color;
                OnColorEdited();
            }
        }

        private void tbAlpha_Scroll(object sender, EventArgs e)
        {
            Color = Color.FromArgb(tbAlpha.Value, color.R, color.G, color.B);
            OnColorEdited();
        }

        private void tbRed_Scroll(object sender, EventArgs e)
        {
            Color = Color.FromArgb(color.A, tbRed.Value, color.G, color.B);
            OnColorEdited();
        }

        private void tbGreen_Scroll(object sender, EventArgs e)
        {
            Color = Color.FromArgb(color.A, color.R, tbGreen.Value, color.B);
            OnColorEdited();
        }

        private void tbBlue_Scroll(object sender, EventArgs e)
        {
            Color = Color.FromArgb(color.A, color.R, color.G, tbBlue.Value);
            OnColorEdited();
        }

        void ucColorVisualizer_SystemColorsChanged(object sender, EventArgs e)
        {
            systemColors = null;
            UpdateInfo();
        }

        //ReSharper restore InconsistentNaming
        #endregion
    }
}
