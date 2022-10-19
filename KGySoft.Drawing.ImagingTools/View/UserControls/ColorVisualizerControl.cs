#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorVisualizerControl.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

using KGySoft.CoreLibraries;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal partial class ColorVisualizerControl : BaseUserControl
    {
        #region Fields

        #region Static Fields

        private static Dictionary<int, string>? knownColors;
        private static Dictionary<int, string>? systemColors;

        #endregion

        #region Instance Fields

        private bool readOnly;
        private Color color;
        private Bitmap? alphaPattern;
        private ImageAttributes? attrTiles;
        private string? specialInfo;

        #endregion

        #endregion

        #region Events

        internal event EventHandler ColorEdited
        {
            add => Events.AddHandler(nameof(ColorEdited), value);
            remove => Events.RemoveHandler(nameof(ColorEdited), value);
        }

        #endregion

        #region Properties

        #region Static Properties

        private static Dictionary<int, string> KnownColors
        {
            get
            {
                if (knownColors == null)
                {
                    knownColors = new Dictionary<int, string> { { 0, nameof(Color.Empty) } };

                    // non-system known colors: 27..168 (Transparent..YellowGreen)
                    for (KnownColor color = (KnownColor)27; color <= (KnownColor)167; color++)
                    {
                        int argb = Color.FromKnownColor(color).ToArgb();
                        if (knownColors.TryGetValue(argb, out var name))
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
                        if (systemColors.TryGetValue(argb, out var name))
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
            get => readOnly;
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
            get => color;
            set
            {
                color = value;
                ColorUpdated();
            }
        }

        internal string? SpecialInfo
        {
            get => specialInfo;
            set
            {
                specialInfo = value;
                UpdateInfo();
            }
        }

        #endregion

        #endregion

        #region Constructors

        public ColorVisualizerControl()
        {
            InitializeComponent();
            btnSelectColor.Image = Images.Palette;
            pnlColor.SetDoubleBuffered(true);
            tbAlpha.SetDoubleBuffered(true);
            tbRed.SetDoubleBuffered(true);
            tbGreen.SetDoubleBuffered(true);
            tbBlue.SetDoubleBuffered(true);
            txtColor.SetDoubleBuffered(true);

            SystemColorsChanged += ucColorVisualizer_SystemColorsChanged;

            btnSelectColor.Click += btnEdit_Click;
            tbAlpha.Scroll += tbAlpha_Scroll;
            tbRed.Scroll += tbRed_Scroll;
            tbGreen.Scroll += tbGreen_Scroll;
            tbBlue.Scroll += tbBlue_Scroll;
            pnlColor.Paint += pnlColor_Paint;
            pnlAlpha.Paint += pnlColor_Paint;
        }

        #endregion

        #region Methods

        #region Static Methods

        private static string GetKnownColor(Color color) => KnownColors.GetValueOrDefault(color.ToArgb(), "–")!;
        private static string GetSystemColors(Color color) => SystemColors.GetValueOrDefault(color.ToArgb(), "–")!;

        #endregion

        #region Instance Methods

        #region Protected Methods

        protected override void OnLoad(EventArgs e)
        {
            // Fixing high DPI appearance on Mono
            PointF scale;
            if (OSUtils.IsMono && (scale = this.GetScale()) != new PointF(1f, 1f))
            {
                tblColor.ColumnStyles[0].Width = (int)(50 * scale.X);
                tblColor.ColumnStyles[1].Width = (int)(80 * scale.X);
            }

            base.OnLoad(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                alphaPattern?.Dispose();
                attrTiles?.Dispose();
            }

            alphaPattern = null;
            attrTiles = null;
            pnlAlpha.Paint -= pnlColor_Paint;
            pnlColor.Paint -= pnlColor_Paint;
            btnSelectColor.Click -= btnEdit_Click;
            tbAlpha.Scroll -= tbAlpha_Scroll;
            tbRed.Scroll -= tbRed_Scroll;
            tbGreen.Scroll -= tbGreen_Scroll;
            tbBlue.Scroll -= tbBlue_Scroll;
            SystemColorsChanged -= ucColorVisualizer_SystemColorsChanged;
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

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

            lblAlpha.Text = Res.TextAlphaValue(a);
            lblRed.Text = Res.TextRedValue(r);
            lblGreen.Text = Res.TextGreenValue(g);
            lblBlue.Text = Res.TextBlueValue(b);

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
            var sb = new StringBuilder();
            if (!String.IsNullOrEmpty(SpecialInfo))
                sb.AppendLine(SpecialInfo);
            sb.Append(Res.InfoColor(color.ToArgb(), GetKnownColor(color), GetSystemColors(color), color.GetHue(), color.GetSaturation() * 100f, color.GetBrightness() * 100f));
            txtColor.Text = sb.ToString();
        }

        private void CreateAlphaPattern()
        {
            Size size = new Size(10, 10).Scale(this.GetScale());
            var bmpPattern = new Bitmap(size.Width, size.Height);
            using (Graphics g = Graphics.FromImage(bmpPattern))
            {
                g.Clear(Color.White);
                var smallRect = new Rectangle(Point.Empty, new Size(bmpPattern.Width >> 1, bmpPattern.Height >> 1));
                g.FillRectangle(Brushes.Silver, smallRect);
                smallRect.Offset(smallRect.Width, smallRect.Height);
                g.FillRectangle(Brushes.Silver, smallRect);
            }

            // Using a TextureBrush would be simpler but that is not supported on Mono
            attrTiles = new ImageAttributes();
            attrTiles.SetWrapMode(WrapMode.Tile);
            alphaPattern = bmpPattern;
        }

        private void OnColorEdited() => Events.GetHandler<EventHandler>(nameof(ColorEdited))?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Event handlers
        //ReSharper disable InconsistentNaming
#pragma warning disable IDE1006 // Naming Styles

        private void pnlColor_Paint(object? sender, PaintEventArgs e)
        {
            // painting checked background
            if (color.A != 255)
            {
                if (alphaPattern == null)
                    CreateAlphaPattern();

                Size size = pnlColor.Size;
                e.Graphics.DrawImage(alphaPattern!, new Rectangle(Point.Empty, size), 0, 0 , size.Width, size.Height, GraphicsUnit.Pixel, attrTiles);
            }

            Color backColor = sender == pnlAlpha ? Color.FromArgb(color.A, Color.White) : color;
            using (Brush b = new SolidBrush(backColor))
                e.Graphics.FillRectangle(b, e.ClipRectangle);
        }

        private void btnEdit_Click(object? sender, EventArgs e)
        {
            if (readOnly)
                return;

            Color? newColor = Dialogs.PickColor(color);
            if (newColor.HasValue)
            {
                Color = newColor.Value;
                OnColorEdited();
            }
        }

        private void tbAlpha_Scroll(object? sender, EventArgs e)
        {
            Color = Color.FromArgb(tbAlpha.Value, color.R, color.G, color.B);
            OnColorEdited();
        }

        private void tbRed_Scroll(object? sender, EventArgs e)
        {
            Color = Color.FromArgb(color.A, tbRed.Value, color.G, color.B);
            OnColorEdited();
        }

        private void tbGreen_Scroll(object? sender, EventArgs e)
        {
            Color = Color.FromArgb(color.A, color.R, tbGreen.Value, color.B);
            OnColorEdited();
        }

        private void tbBlue_Scroll(object? sender, EventArgs e)
        {
            Color = Color.FromArgb(color.A, color.R, color.G, tbBlue.Value);
            OnColorEdited();
        }

        void ucColorVisualizer_SystemColorsChanged(object? sender, EventArgs e)
        {
            systemColors = null;
            UpdateInfo();
        }

#pragma warning restore IDE1006 // Naming Styles
        //ReSharper restore InconsistentNaming
        #endregion

        #endregion

        #endregion
    }
}
