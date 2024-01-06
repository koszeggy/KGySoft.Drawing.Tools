#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorVisualizerControl.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2023 - All Rights Reserved
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
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal partial class ColorVisualizerControl : MvvmBaseUserControl
    {
        #region Fields

        private Bitmap? alphaPattern;
        private ImageAttributes? attrTiles;

        #endregion

        #region Properties

        internal new ColorVisualizerViewModel? ViewModel
        {
            get => (ColorVisualizerViewModel?)base.ViewModel;
            set => base.ViewModel = value;
        }

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

            pnlColor.Paint += pnlColor_Paint;
            pnlAlpha.Paint += pnlColor_Paint;
        }

        #endregion

        #region Methods

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

        protected override void ApplyViewModel()
        {
            InitCommandBindings();
            InitPropertyBindings();
            base.ApplyViewModel();
            UpdateColor();
        }

        private void InitPropertyBindings()
        {
            ColorVisualizerViewModel vm = ViewModel!;

            // !VM.Visible -> tsMenu.Visible, tbAlpha.Visible, tbRed.Visible, tbGreen.Visible, tbBlue.Visible
            CommandBindings.AddPropertyBinding(vm, nameof(vm.ReadOnly), nameof(Visible), ro => ro is false, tsMenu, tbAlpha, tbRed, tbGreen, tbBlue);

            // VM.InfoText -> txtColor.Text
            CommandBindings.AddPropertyBinding(vm, nameof(ViewModel.InfoText), nameof(txtColor.Text), txtColor);

            // Removing color channel panels if there are custom components (this cannot be undone in this instance)
            if (vm.CustomColorComponents != null)
                ApplyCustomColorComponentsLayout();
        }

        private void ApplyCustomColorComponentsLayout()
        {
            #region Local Methods

            static void DestroyPanel(ref Panel? c)
            {
                if (c == null)
                    return;

                c.Parent = null;
                c.Dispose();
                c = null;
            }

            #endregion

            // Already applied
            if (pnlAlpha == null)
                return;

            pnlAlpha.Paint -= pnlColor_Paint;

            // We need to remove the color component panels to be able to extend the custom labels
            DestroyPanel(ref pnlAlpha);
            DestroyPanel(ref pnlRed);
            DestroyPanel(ref pnlGreen);
            DestroyPanel(ref pnlBlue);

            tblColor.SetColumnSpan(lblAlpha, 2);
            tblColor.SetColumnSpan(lblRed, 2);
            tblColor.SetColumnSpan(lblGreen, 2);
            tblColor.SetColumnSpan(lblBlue, 2);
        }

        private void InitCommandBindings()
        {
            ColorVisualizerViewModel vm = ViewModel!;

            // VM.PropertyChanged(Color) -> UpdateColor
            CommandBindings.AddPropertyChangedHandlerBinding(vm, UpdateColor, nameof(ViewModel.Color));

            // btnSelectColor.Click -> OnSelectColorCommand
            CommandBindings.Add(OnSelectColorCommand)
                .AddSource(btnSelectColor, nameof(btnSelectColor.Click));

            // tbAlpha.Scroll -> OnAlphaScrollCommand
            CommandBindings.Add(OnAlphaScrollCommand)
                .AddSource(tbAlpha, nameof(tbAlpha.Scroll));
            
            // tbRed.Scroll -> OnRedScrollCommand
            CommandBindings.Add(OnRedScrollCommand)
                .AddSource(tbRed, nameof(tbRed.Scroll));
            
            // tbGreen.Scroll -> OnGreenScrollCommand
            CommandBindings.Add(OnGreenScrollCommand)
                .AddSource(tbGreen, nameof(tbGreen.Scroll));

            // tbBlue.Scroll -> OnBlueScrollCommand
            CommandBindings.Add(OnBlueScrollCommand)
                .AddSource(tbBlue, nameof(tbBlue.Scroll));

            // SystemColorsChanged -> VM.ResetSystemColors
            CommandBindings.Add(vm.ResetSystemColors)
                .AddSource(this, nameof(SystemColorsChanged));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                alphaPattern?.Dispose();
                attrTiles?.Dispose();
            }

            alphaPattern = null;
            attrTiles = null;
            if (pnlAlpha != null)
                pnlAlpha.Paint -= pnlColor_Paint;
            pnlColor.Paint -= pnlColor_Paint;
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

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

        private void UpdateColor()
        {
            ColorVisualizerViewModel vm = ViewModel!;
            Color color = vm.Color;
            byte a = color.A;
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;

            pnlColor.Invalidate();
            KeyValuePair<string, string>[]? customComponents = vm.CustomColorComponents;
            if (customComponents is null)
            {
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
                return;
            }

            lblAlpha.Text = customComponents.Length > 0 ? $"{customComponents[0].Key}: {customComponents[0].Value}" : String.Empty;
            lblRed.Text = customComponents.Length > 1 ? $"{customComponents[1].Key}: {customComponents[1].Value}" : String.Empty;
            lblGreen.Text = customComponents.Length > 2 ? $"{customComponents[2].Key}: {customComponents[2].Value}" : String.Empty;
            lblBlue.Text = customComponents.Length > 3 ? $"{customComponents[3].Key}: {customComponents[3].Value}" : String.Empty;
        }

        #endregion

        #region Event handlers
        //ReSharper disable InconsistentNaming
#pragma warning disable IDE1006 // Naming Styles

        private void pnlColor_Paint(object? sender, PaintEventArgs e)
        {
            Color color = ViewModel?.Color ?? default;

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

#pragma warning restore IDE1006 // Naming Styles
        //ReSharper restore InconsistentNaming
        #endregion

        #region Command Handlers

        private void OnSelectColorCommand()
        {
            ColorVisualizerViewModel vm = ViewModel!;
            if (vm.ReadOnly)
                return;

            Color? newColor = Dialogs.PickColor(vm.Color);
            if (newColor.HasValue)
                vm.Color = newColor.Value;
        }

        private void OnAlphaScrollCommand()
        {
            Color color = ViewModel!.Color;
            ViewModel!.Color = Color.FromArgb(tbAlpha.Value, color.R, color.G, color.B);
        }

        private void OnRedScrollCommand()
        {
            Color color = ViewModel!.Color;
            ViewModel!.Color = Color.FromArgb(color.A, tbRed.Value, color.G, color.B);
        }

        private void OnGreenScrollCommand()
        {
            Color color = ViewModel!.Color;
            ViewModel!.Color = Color.FromArgb(color.A, color.R, tbGreen.Value, color.B);
        }

        private void OnBlueScrollCommand()
        {
            Color color = ViewModel!.Color;
            ViewModel!.Color = Color.FromArgb(color.A, color.R, color.G, tbBlue.Value);
        }

        #endregion

        #endregion
    }
}
