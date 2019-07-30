﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: PalettePanel.cs
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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace KGySoft.Drawing.ImagingTools.Controls
{
    internal sealed class PalettePanel : Panel
    {
        #region Constants

        private const int steps = 25;

        #endregion

        #region Fields

        #region Static Fields

        private static readonly Color selectionFrameColor = Color.FromArgb(0, 204, 255);
        private static readonly Color selectionFrameColorAlternative = Color.DarkBlue;

        #endregion

        #region Instance Fields

        private VScrollBar sbPalette;
        private Timer timerSelection;
        private IContainer components;

        private IList<Color> palette;
        private int selectedColorIndex = -1;
        private int firstVisibleColor;
        private int visibleRowCount;
        private int counter;
        private float scaleX = 1f;
        private float scaleY = 1f;
        private int scrollFraction;

        #endregion

        #endregion

        #region Events

        internal event EventHandler SelectedColorChanged;

        #endregion

        #region Properties

        #region Internal Properties

        internal IList<Color> Palette
        {
            set
            {
                palette = value;
                if (ColorCount == 0)
                    timerSelection.Enabled = false;

                CheckPaletteLayout();
            }
        }

        internal int SelectedColorIndex
        {
            get => selectedColorIndex;
            set
            {
                if (selectedColorIndex == value)
                    return;

                if (IsSelectedColorVisible())
                    Invalidate(GetColorRect(selectedColorIndex));

                selectedColorIndex = value;
                bool invalidateAll = false;
                if (selectedColorIndex < firstVisibleColor)
                {
                    firstVisibleColor = selectedColorIndex - (selectedColorIndex % 16);
                    invalidateAll = true;
                }
                else if (firstVisibleColor + (visibleRowCount << 4) <= selectedColorIndex)
                {
                    firstVisibleColor = selectedColorIndex - (selectedColorIndex % 16) - ((visibleRowCount - 1) << 4);
                    invalidateAll = true;
                }

                timerSelection.Enabled = true;
                if (invalidateAll)
                {
                    sbPalette.Value = firstVisibleColor >> 4;
                    Invalidate();
                }
                else
                    Invalidate(GetColorRect(selectedColorIndex));

                OnSelectedColorChanged(EventArgs.Empty);
            }
        }

        internal Color SelectedColor
        {
            get
            {
                if (palette == null || selectedColorIndex < 0 || selectedColorIndex >= palette.Count)
                    return Color.Empty;

                return palette[selectedColorIndex];
            }
            set
            {
                if (palette == null || selectedColorIndex < 0 || selectedColorIndex >= palette.Count)
                    return;

                palette[selectedColorIndex] = value;
                if (!IsSelectedColorVisible())
                    return;

                Invalidate(GetColorRect(selectedColorIndex));
            }
        }

        #endregion

        #region Private Properties

        private int ColorCount => palette?.Count ?? 0;

        #endregion

        #endregion

        #region Constructors

        internal PalettePanel()
        {
            InitializeComponent();

            DoubleBuffered = true;
            SetStyle(ControlStyles.Selectable, true);
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
            if (disposing && (components != null))
                components.Dispose();

            sbPalette.ValueChanged -= sbPalette_ValueChanged;
            timerSelection.Tick -= timerSelection_Tick;
            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (ColorCount == 0)
                return;

            scaleX = (float)Math.Round(e.Graphics.DpiX / 96f, 2);
            scaleY = (float)Math.Round(e.Graphics.DpiY / 96f, 2);
            int upper = Math.Min(palette.Count, firstVisibleColor + (visibleRowCount << 4));

            // iterating through visible colors
            for (int i = firstVisibleColor; i < upper; i++)
            {
                Rectangle rect = GetColorRect(i);
                if (!rect.IntersectsWith(e.ClipRectangle))
                    continue;

                // background
                e.Graphics.FillRectangle(SystemBrushes.Control, rect);

                // selection frame
                if (i == selectedColorIndex)
                {
                    Rectangle rectSelection = rect;
                    int r = (int)Math.Round(selectionFrameColor.R + Math.Abs(counter) * ((double)selectionFrameColorAlternative.R - selectionFrameColor.R) / steps);
                    int g = (int)Math.Round(selectionFrameColor.G + Math.Abs(counter) * ((double)selectionFrameColorAlternative.G - selectionFrameColor.G) / steps);
                    int b = (int)Math.Round(selectionFrameColor.B + Math.Abs(counter) * ((double)selectionFrameColorAlternative.B - selectionFrameColor.B) / steps);
                    using (Pen pen = new Pen(Color.FromArgb(r, g, b), scaleX))
                        e.Graphics.DrawRectangle(pen, new Rectangle(rectSelection.Left, rectSelection.Top, rectSelection.Width - 1, rectSelection.Height - 1));

                    if (Focused && ShowFocusCues)
                    {
                        rectSelection.Inflate(-1, -1);
                        ControlPaint.DrawFocusRectangle(e.Graphics, rectSelection);
                    }
                }

                rect.Inflate(-2, -2);

                // alpha background
                Color c = palette[i];
                if (c.A != 255)
                {
                    e.Graphics.FillRectangle(Brushes.White, rect);
                    Rectangle smallRect = new Rectangle(rect.Location, new Size(rect.Width >> 1, rect.Height >> 1));
                    e.Graphics.FillRectangle(Brushes.Silver, smallRect);
                    smallRect.Offset(smallRect.Width, smallRect.Height);
                    e.Graphics.FillRectangle(Brushes.Silver, smallRect);
                }

                // color
                using (Brush b = new SolidBrush(c))
                    e.Graphics.FillRectangle(b, rect);
            }
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            CheckPaletteLayout();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();

            if (e.Button != MouseButtons.Left || ColorCount == 0)
                return;

            if (GetColorRect(selectedColorIndex).Contains(e.Location))
                return;

            if (!Rectangle.Round(new RectangleF(2 * scaleX, 2 * scaleY, (13 << 4) * scaleX, 13 * visibleRowCount * scaleY)).Contains(e.Location))
                return;

            int x = ((int)(e.X / scaleX) - 2) / 13;
            int y = ((int)(e.Y / scaleY) - 2) / 13;
            int index = firstVisibleColor + (y << 4) + x;

            if (index >= ColorCount)
                return;

            SelectedColorIndex = index;
        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            if (ColorCount == 0)
            {
                base.OnPreviewKeyDown(e);
                return;
            }

            int newIndex = selectedColorIndex;
            switch (e.KeyData)
            {
                case Keys.Right:
                    newIndex = Math.Min(selectedColorIndex + 1, ColorCount - 1);
                    break;
                case Keys.Left:
                    newIndex = Math.Max(selectedColorIndex - 1, 0);
                    break;
                case Keys.Down:
                    newIndex = Math.Min(selectedColorIndex + 16, ColorCount - 1);
                    break;
                case Keys.Up:
                    newIndex = Math.Max(selectedColorIndex - 16, 0);
                    break;
                case Keys.PageDown:
                    newIndex = Math.Min(selectedColorIndex + (visibleRowCount << 4), ColorCount - 1);
                    break;
                case Keys.PageUp:
                    newIndex = Math.Max(selectedColorIndex - (visibleRowCount << 4), 0);
                    break;
                case Keys.End:
                    newIndex = ColorCount - 1;
                    break;
                case Keys.Home:
                    newIndex = 0;
                    break;
                default:
                    base.OnPreviewKeyDown(e);
                    return;
            }

            SelectedColorIndex = newIndex;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!sbPalette.Visible)
                return;

            // When scrolling by mouse, delta is always +-120 so this will be 1 change on the scrollbar.
            // But we collect the fractional changes caused by the touchpad scrolling so it will not be lost either.
            int totalDelta = scrollFraction + e.Delta;
            scrollFraction = totalDelta % SystemInformation.MouseWheelScrollDelta;
            int newValue = sbPalette.Value - totalDelta / SystemInformation.MouseWheelScrollDelta;
            if (newValue < 0)
                newValue = 0;
            else if (newValue > sbPalette.Maximum - sbPalette.LargeChange + 1)
                newValue = sbPalette.Maximum - sbPalette.LargeChange + 1;

            sbPalette.Value = newValue;
        }

        #endregion

        #region Private Methods

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.sbPalette = new System.Windows.Forms.VScrollBar();
            this.timerSelection = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // sbPalette
            // 
            this.sbPalette.Dock = System.Windows.Forms.DockStyle.Right;
            this.sbPalette.Location = new System.Drawing.Point(183, 0);
            this.sbPalette.Name = "sbPalette";
            this.sbPalette.Size = new System.Drawing.Size(17, 100);
            this.sbPalette.TabIndex = 0;
            this.sbPalette.Visible = false;
            this.sbPalette.ValueChanged += new System.EventHandler(this.sbPalette_ValueChanged);
            // 
            // timerSelection
            // 
            this.timerSelection.Interval = 20;
            this.timerSelection.Tick += new System.EventHandler(this.timerSelection_Tick);
            // 
            // PalettePanel
            // 
            this.Controls.Add(this.sbPalette);
            this.ResumeLayout(false);
        }

        private void CheckPaletteLayout()
        {
            if (ColorCount == 0)
            {
                sbPalette.Visible = false;
                visibleRowCount = 0;
                return;
            }

            // calculating visible rows
            int maxRows = ((int)(Height / scaleY) - 5) / 13;
            if (maxRows == visibleRowCount)
                return;

            Invalidate();
            visibleRowCount = maxRows;
            int colorRows = (int)Math.Ceiling((double)palette.Count / 16);
            if (visibleRowCount >= colorRows)
            {
                // scrollbar is not needed
                firstVisibleColor = 0;
                sbPalette.Visible = false;
                timerSelection.Enabled = true;
                return;
            }

            //scrollbar is needed
            sbPalette.Maximum = colorRows - 1;
            sbPalette.LargeChange = visibleRowCount;
            if (firstVisibleColor + (visibleRowCount << 4) >= palette.Count + 16)
                firstVisibleColor = palette.Count - (visibleRowCount << 4);
            sbPalette.Value = firstVisibleColor >> 4;
            sbPalette.Visible = true;
            timerSelection.Enabled = IsSelectedColorVisible();
        }

        private Rectangle GetColorRect(int index)
        {
            var rect = new RectangleF((2 + (index % 16) * 13) * scaleX, (2 + ((index - firstVisibleColor) >> 4) * 13) * scaleY, 13 * scaleX, 13 * scaleY);
            return Rectangle.Round(rect);
        }

        private bool IsSelectedColorVisible()
            => selectedColorIndex >= firstVisibleColor
                && selectedColorIndex < firstVisibleColor + (visibleRowCount << 4);

        private void OnSelectedColorChanged(EventArgs e) => SelectedColorChanged?.Invoke(this, e);

        #endregion

        #region Event handlers
        //ReSharper disable InconsistentNaming

        private void sbPalette_ValueChanged(object sender, EventArgs e)
        {
            firstVisibleColor = sbPalette.Value << 4;
            timerSelection.Enabled = IsSelectedColorVisible();
            Invalidate();
        }

        private void timerSelection_Tick(object sender, EventArgs e)
        {
            if (!IsSelectedColorVisible())
            {
                timerSelection.Enabled = false;
                return;
            }

            if (counter++ == steps)
                counter = -steps;

            Invalidate(GetColorRect(selectedColorIndex));
        }

        //ReSharper disable InconsistentNaming
        #endregion

        #endregion
    }
}