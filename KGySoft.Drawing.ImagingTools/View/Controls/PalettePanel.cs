﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: PalettePanel.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2025 - All Rights Reserved
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
using System.Windows.Forms;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    internal sealed partial class PalettePanel : BaseControl
    {
        #region Constants

        private const int steps = 25;

        #endregion

        #region Fields

        #region Static Fields

        private static readonly Color selectionFrameColor = Color.FromArgb(0, 204, 255);
        private static readonly Color selectionFrameColorAlternative = Color.DarkBlue;
        private static readonly Point distanceUnit = new(13, 13);
        private static readonly Point paddingUnit = new(2, 2);

        #endregion

        #region Instance Fields

        private readonly int scrollbarWidth;

        private IList<Color>? palette;
        private int selectedColorIndex = -1;
        private int firstVisibleColor;
        private int visibleRowCount;
        private int counter;
        private PointF scale = new PointF(1f, 1f);
        private Point scaledDistance = distanceUnit;
        private Point scaledPadding = paddingUnit;
        private int scrollFraction;
        private bool isRightToLeft;

        #endregion

        #endregion

        #region Events

        internal event EventHandler? SelectedColorIndexChanged
        {
            add => Events.AddHandler(nameof(SelectedColorIndexChanged), value);
            remove => Events.RemoveHandler(nameof(SelectedColorIndexChanged), value);
        }

        #endregion

        #region Properties

        #region Internal Properties

        internal IList<Color>? Palette
        {
            set
            {
                palette = value;
                if (ColorCount == 0)
                    timerSelection.Enabled = false;
                SelectedColorIndex = value is { Count: > 0 } ? 0 : -1;
                ResetLayout();
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
                    ResetLayout();
                    sbPalette.Value = firstVisibleColor >> 4;
                    Invalidate();
                }
                else
                    Invalidate(GetColorRect(selectedColorIndex));

                OnSelectedColorIndexChanged(EventArgs.Empty);
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

        public PalettePanel()
        {
            InitializeComponent();

            sbPalette.ValueChanged += sbPalette_ValueChanged;
            timerSelection.Tick += timerSelection_Tick;

            DoubleBuffered = true;
            SetStyle(ControlStyles.Selectable, true);
            scrollbarWidth = OSUtils.IsMono ? this.ScaleWidth(16) : SystemInformation.VerticalScrollBarWidth;
            sbPalette.Width = scrollbarWidth;
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();

            sbPalette.ValueChanged -= sbPalette_ValueChanged;
            timerSelection.Tick -= timerSelection_Tick;
            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var clientSize = ClientSize;
            float minScale = e.Graphics.GetScale().X;
            float maxScale = Math.Max(Math.Min(clientSize.Width / 240f, clientSize.Height / (distanceUnit.Y + paddingUnit.Y * 2f)), 0.25f);
            int colorRows = Math.Min(16, (int)Math.Ceiling(palette!.Count / 16d));
            float actualScale = maxScale <= minScale
                ? maxScale
                : Math.Min(maxScale, Math.Max(minScale, clientSize.Height / (distanceUnit.Y * colorRows + paddingUnit.Y * 2f)));
            var currentScale = new PointF(actualScale, actualScale);
            if (currentScale != scale)
            {
                scale = currentScale;
                ResetLayout();
            }

            base.OnPaint(e);
            if (ColorCount == 0)
                return;

            e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            int upper = Math.Min(palette!.Count, firstVisibleColor + (visibleRowCount << 4));

            // iterating through visible colors
            for (int i = firstVisibleColor; i < upper; i++)
            {
                Rectangle rect = GetColorRect(i);
                if (!rect.IntersectsWith(e.ClipRectangle))
                    continue;

                // background
                Color backColor = BackColor;
                bool isControlBrush = backColor == SystemColors.Control;
                var backBrush = isControlBrush ? SystemBrushes.Control : new SolidBrush(backColor);
                try
                {
                    e.Graphics.FillRectangle(backBrush, rect);
                }
                finally
                {
                    if (!isControlBrush)
                        backBrush.Dispose();
                }
                
                // selection frame
                if (i == selectedColorIndex)
                {
                    // For drawing lines using top-left PixelOffsetMode (Half cannot be used with DrawFocusRectangle, which accepts int coordinates only)
                    e.Graphics.PixelOffsetMode = PixelOffsetMode.Default;
                    Rectangle rectSelection = rect;
                    int r = (int)Math.Round(selectionFrameColor.R + Math.Abs(counter) * ((double)selectionFrameColorAlternative.R - selectionFrameColor.R) / steps);
                    int g = (int)Math.Round(selectionFrameColor.G + Math.Abs(counter) * ((double)selectionFrameColorAlternative.G - selectionFrameColor.G) / steps);
                    int b = (int)Math.Round(selectionFrameColor.B + Math.Abs(counter) * ((double)selectionFrameColorAlternative.B - selectionFrameColor.B) / steps);

                    // Using wider pen fails even with Inset alignment with every possible PixelOffsetMode. So using a 1 width pen and drawing possible more rectangles
                    int penWidth = Math.Max((int)scale.X, 1);
                    using (Pen pen = new Pen(Color.FromArgb(r, g, b)))
                    {
                        for (int x = 0; x < penWidth; x++)
                            e.Graphics.DrawRectangle(pen, rectSelection.Left + x, rectSelection.Top + x, rectSelection.Width - 1 - (x << 1), rectSelection.Height - 1 - (x << 1));
                    }

                    if (Focused && ShowFocusCues)
                    {
                        rectSelection.Inflate(-(int)scale.X, -(int)scale.X);
                        for (int x = 0; x < penWidth; x++)
                        {
                            ControlPaint.DrawFocusRectangle(e.Graphics, rectSelection);
                            rectSelection.Inflate(-1, -1);
                        }
                    }

                    e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
                }

                rect.Inflate(-(int)(2 * scale.X), -(int)(2 * scale.Y));

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

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();

            if (e.Button != MouseButtons.Left || ColorCount == 0)
                return;

            Point location = e.Location;
            if (isRightToLeft)
            {
                location.X -= scrollbarWidth;
                location.X = (scaledDistance.X << 4) + scaledPadding.X * 2 - location.X;
            }
            
            // same as before (using the raw location because GetColorRect translates it)
            if (GetColorRect(selectedColorIndex).Contains(e.Location))
                return;

            // out of range
            if (!new Rectangle(scaledPadding.X, scaledPadding.Y, scaledDistance.X << 4, scaledDistance.Y * visibleRowCount).Contains(location))
                return;

            int x = (location.X - scaledPadding.X) / scaledDistance.X;
            int y = (location.Y - scaledPadding.Y) / scaledDistance.Y;
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

            int newIndex;
            switch (e.KeyData)
            {
                case Keys.Right:
                case Keys.Left:
                    if (e.KeyData == Keys.Right ^ isRightToLeft)
                        newIndex = Math.Min(selectedColorIndex + 1, ColorCount - 1);
                    else
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
            scrollFraction = totalDelta % MouseWheelScrollDelta;
            int newValue = sbPalette.Value - totalDelta / MouseWheelScrollDelta;
            if (newValue < 0)
                newValue = 0;
            else if (newValue > sbPalette.Maximum - sbPalette.LargeChange + 1)
                newValue = sbPalette.Maximum - sbPalette.LargeChange + 1;

            sbPalette.Value = Math.Min(Math.Max(newValue, sbPalette.Minimum),sbPalette.Maximum);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            ResetLayout();
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            isRightToLeft = RightToLeft == RightToLeft.Yes;
            sbPalette.Dock = isRightToLeft ? DockStyle.Left : DockStyle.Right;
            Invalidate();
        }

        #endregion

        #region Private Methods

        private void ResetLayout()
        {
            if (ColorCount == 0)
            {
                sbPalette.Visible = false;
                visibleRowCount = 0;
                return;
            }

            Invalidate();

            scaledDistance = new Point((int)(scale.X * 13), (int)(scale.Y * 13));
            scaledPadding = new Point((int)(scale.X * 2), (int)(scale.Y * 2));

            // calculating visible rows
            int maxRows = (ClientSize.Height - scaledPadding.Y * 2) / scaledDistance.Y;
            if (maxRows == visibleRowCount)
            {
                timerSelection.Enabled = IsSelectedColorVisible();
                return;
            }

            visibleRowCount = maxRows;
            int colorRows = (int)Math.Ceiling(palette!.Count / 16d);
            if (visibleRowCount >= colorRows)
            {
                // scrollbar is not needed
                firstVisibleColor = 0;
                sbPalette.Visible = false;
                timerSelection.Enabled = true;
                visibleRowCount = colorRows;
                return;
            }

            // scrollbar is needed
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
            var unit = new Size((int)(scale.X * 13), (int)(scale.Y * 13));
            int left = index & 15;
            if (isRightToLeft)
                left = 15 - left;
            left = left * unit.Width + (int)(scale.X * 2);
            if (isRightToLeft)
                left += scrollbarWidth;

            int top = (index - firstVisibleColor) >> 4;
            top = top * unit.Height + (int)(scale.Y * 2);

            // ReSharper disable once CompareOfFloatsByEqualityOperator - intended
            return new Rectangle(left, top, (int)(13 * scale.X), (int)(13 * scale.Y));
        }

        private bool IsSelectedColorVisible()
            => !scaledPadding.IsEmpty && selectedColorIndex >= firstVisibleColor
                && selectedColorIndex < firstVisibleColor + (visibleRowCount << 4);

        private void OnSelectedColorIndexChanged(EventArgs e) => Events.GetHandler<EventHandler>(nameof(SelectedColorIndexChanged))?.Invoke(this, e);

        #endregion

        #region Event handlers
        //ReSharper disable InconsistentNaming
#pragma warning disable IDE1006 // Naming Styles

        private void sbPalette_ValueChanged(object? sender, EventArgs e)
        {
            firstVisibleColor = sbPalette.Value << 4;
            timerSelection.Enabled = IsSelectedColorVisible();
            ResetLayout();
        }

        private void timerSelection_Tick(object? sender, EventArgs e)
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

#pragma warning restore IDE1006 // Naming Styles
        //ReSharper disable InconsistentNaming
        #endregion

        #endregion
    }
}
