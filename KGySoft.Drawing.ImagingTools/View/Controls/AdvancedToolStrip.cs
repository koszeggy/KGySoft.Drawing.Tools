#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdvancedToolStrip.cs
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using KGySoft.Collections;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.View.Components;
using KGySoft.Drawing.ImagingTools.WinApi;
using KGySoft.Reflection;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    /// <summary>
    /// A <see cref="ToolStrip"/> with some additional features:
    /// - It can scale its content regardless of .NET version and app.config settings.
    /// - Custom renderer for corrected checked button appearance, scaled and correctly colored arrows, fixed high contrast appearance and more.
    /// - Tool tip supports right-to-left
    /// - Clicking works even if the owner form was not active
    /// </summary>
    internal class AdvancedToolStrip : ToolStrip
    {
        #region AdvancedToolStripRenderer class

        private class AdvancedToolStripRenderer : ToolStripProfessionalRenderer
        {
            #region ButtonStyle enum

            [Flags]
            private enum ButtonStyle : byte
            {
                None,
                Selected = 1,
                Pressed = 1 << 1,
                Checked = 1 << 2,
                Dropped = 1 << 3
            }

            #endregion

            #region Fields

            private static readonly Size referenceOffset = new Size(2, 2);
            private static readonly Size referenceOffsetDouble = new Size(4, 4);
            private static readonly Cache<Image, Image> disabledImagesCache = new(CreateDisabledImage, 8) { DisposeDroppedValues = true };

            #endregion

            #region Methods

            #region Static Methods

            private static void FillBackground(Graphics g, Rectangle rect, Color color1, Color color2)
            {
                if (color1.ToArgb() == color2.ToArgb())
                    g.FillRectangle(color1.GetBrush(), rect);
                else
                {
                    using var brush = new LinearGradientBrush(rect, color1, color2, LinearGradientMode.Vertical);
                    g.FillRectangle(brush, rect);
                }
            }

            private static void DrawArrow(Graphics g, Color color, Rectangle bounds, ArrowDirection direction)
            {
                Point middle = new Point(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height / 2);

                Point[] arrow;
                Size offset = g.ScaleSize(referenceOffset);
                Size offsetDouble = g.ScaleSize(referenceOffsetDouble);

                switch (direction)
                {
                    case ArrowDirection.Up:
                        arrow = new Point[]
                        {
                            new Point(middle.X - offset.Width - 1, middle.Y + 1),
                            new Point(middle.X + offset.Width + 1, middle.Y + 1),
                            new Point(middle.X, middle.Y - offset.Height - 1)
                        };
                        break;
                    case ArrowDirection.Left:
                        arrow = new Point[]
                        {
                            new Point(middle.X + offset.Width, middle.Y - offsetDouble.Height),
                            new Point(middle.X + offset.Width, middle.Y + offsetDouble.Height),
                            new Point(middle.X - offset.Width, middle.Y)
                        };
                        break;
                    case ArrowDirection.Right:
                        arrow = new Point[]
                        {
                            new Point(middle.X - offset.Width, middle.Y - offsetDouble.Height),
                            new Point(middle.X - offset.Width, middle.Y + offsetDouble.Height),
                            new Point(middle.X + offset.Width, middle.Y)
                        };
                        break;
                    default:
                        arrow = new Point[]
                        {
                            new Point(middle.X - offset.Width, middle.Y - 1),
                            new Point(middle.X + offset.Width + (OSUtils.IsMono && OSUtils.IsLinux ? 2 : 1), middle.Y - 1),
                            new Point(middle.X, middle.Y + offset.Height)
                        };
                        break;
                }

                g.FillPolygon(color.GetBrush(), arrow);
            }

            private static void DrawThemedButtonBackground(Graphics g, ProfessionalColorTable colorTable, Rectangle bounds, ButtonStyle style)
            {
                #region Local Methods
                
                static void RenderWithVisualStyles(Graphics g, ProfessionalColorTable colorTable, Rectangle bounds, ButtonStyle style)
                {
                    Color backgroundStart;
                    Color backgroundEnd;
                    if ((style & ButtonStyle.Pressed) != 0 || (style & ButtonStyle.Selected) != 0 && (style & ButtonStyle.Checked) != 0)
                    {
                        backgroundStart = colorTable.ButtonPressedGradientBegin;
                        backgroundEnd = colorTable.ButtonPressedGradientEnd;
                    }
                    else if ((style & ButtonStyle.Selected) != 0)
                    {
                        backgroundStart = colorTable.ButtonSelectedGradientBegin;
                        backgroundEnd = colorTable.ButtonSelectedGradientEnd;
                    }
                    else if ((style & ButtonStyle.Checked) != 0)
                    {
                        backgroundStart = colorTable.ButtonCheckedGradientBegin is { IsEmpty: false } c1 ? c1 : colorTable.ButtonCheckedHighlight;
                        backgroundEnd = colorTable.ButtonCheckedGradientEnd is { IsEmpty: false } c2 ? c2 : colorTable.ButtonCheckedHighlight;
                    }
                    else
                        return;

                    FillBackground(g, bounds, backgroundStart, backgroundEnd);
                }

                static void RenderBasicTheme(Graphics g, ProfessionalColorTable colorTable, Rectangle bounds, ButtonStyle style)
                {
                    Color backColor = (style & ButtonStyle.Pressed) != 0 || (style & ButtonStyle.Selected) != 0 && (style & ButtonStyle.Checked) != 0 ? colorTable.ButtonPressedHighlight
                        : (style & ButtonStyle.Selected) != 0 ? colorTable.ButtonSelectedHighlight
                        : (style & ButtonStyle.Checked) != 0 ? colorTable.ButtonCheckedHighlight
                        : Color.Empty;
                    g.FillRectangle(backColor.GetBrush(), bounds);
                }

                #endregion

                if (style == ButtonStyle.None)
                    return;
                if ((style & ButtonStyle.Dropped) != 0)
                {
                    FillBackground(g, bounds, colorTable.MenuItemPressedGradientBegin, colorTable.MenuItemPressedGradientEnd);
                    g.DrawRectangle(colorTable.MenuBorder.GetPen(), bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                    return;
                }

                if (Application.RenderWithVisualStyles)
                    RenderWithVisualStyles(g, colorTable, bounds, style);
                else
                    RenderBasicTheme(g, colorTable, bounds, style);
                g.DrawRectangle(colorTable.ButtonSelectedBorder.GetPen(), bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            }

            private static void DrawHighContrastButtonBackground(Graphics g, Rectangle bounds, ButtonStyle style)
            {
                if ((style & ButtonStyle.Dropped) == 0 && (style & (ButtonStyle.Selected | ButtonStyle.Checked | ButtonStyle.Pressed)) != 0)
                    g.FillRectangle(SystemBrushes.Highlight, bounds);

                Color borderColor = (style & ButtonStyle.Dropped) != 0 ? SystemColors.ButtonHighlight
                    : (style & ButtonStyle.Pressed) == 0 && (style & (ButtonStyle.Checked | ButtonStyle.Selected)) is ButtonStyle.Checked or ButtonStyle.Selected ? SystemColors.ControlLight
                    : Color.Empty;

                if (!borderColor.IsEmpty)
                    g.DrawRectangle(borderColor.GetPen(), bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            }

            #endregion

            #region Instance Methods

            /// <summary>
            /// Changes to original:
            /// - Fixed color
            /// - Fixed scaling
            /// - [Mono]: Ignoring ToolStripSplitButton because it is painted along the button just like in the MS world.
            /// - [Mono]: Fixing menu item arrow position in high DPI mode
            /// </summary>
            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
            {
                if (e.Item is ToolStripSplitButton)
                    return;
                Rectangle bounds = e.Item is ScalingToolStripDropDownButton scalingButton ? scalingButton.ArrowRectangle
                    : OSUtils.IsMono && e.Item is ToolStripMenuItem mi ? new Rectangle(e.ArrowRectangle.Left, 0, e.ArrowRectangle.Width, mi.Height)
                    : e.ArrowRectangle;
                Color color = !e.Item.Enabled ? SystemColors.ControlDark
                    : SystemInformation.HighContrast ? e.Item.Selected && !e.Item.Pressed ? SystemColors.HighlightText : e.ArrowColor
                    : e.Item is ToolStripDropDownItem ? SystemColors.ControlText
                    : e.ArrowColor;
                
                DrawArrow(e.Graphics, color, bounds, e.Direction);
            }

            /// <summary>
            /// Changes to original:
            /// - [HighContrast]: Not drawing the highlighted background if the menu item is disabled (this is already fixed in Core)
            /// - [HighContrast]: Fixed bounds of highlight rectangle (it was good in .NET Framework but is wrong in Core)
            /// </summary>
            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                if (!SystemInformation.HighContrast || e.Item is not ToolStripMenuItem item)
                {
                    base.OnRenderMenuItemBackground(e);
                    return;
                }

                // Selected/pressed menu point in high contrast mode: drawing the background only if enabled
                var bounds = new Rectangle(2, 0, item.Width - 3, item.Height);
                if (item.Pressed || item.Selected && item.Enabled)
                    e.Graphics.FillRectangle(SystemBrushes.Highlight, bounds);
                else if (item.Selected && !item.Enabled)
                    e.Graphics.DrawRectangle(SystemPens.Highlight, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            }

            /// <summary>
            /// Changes to original:
            /// - When a menu item is selected, then not using its possible custom colors
            /// - [HighContrast]: Fixing text color on highlighted menu items
            /// </summary>
            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                if (e.Item is ToolStripMenuItem mi)
                {
                    e.TextColor = !mi.Enabled ? SystemColors.GrayText
                        : SystemInformation.HighContrast ? mi.Selected || mi.Pressed ? SystemColors.HighlightText : SystemColors.ControlText
                        : mi.Selected || mi.Pressed ? SystemColors.ControlText
                        : e.Item.ForeColor;
                }

                base.OnRenderItemText(e);
            }

            /// <summary>
            /// Changes to original:
            /// - Background image is omitted
            /// - Not selected checked background uses fallback color if current theme has transparent checked background
            /// - [HighContrast]: Not drawing border if button is pressed and checked (this is how the .NET Core version also works)
            /// </summary>
            protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
            {
                ToolStripButton button = (ToolStripButton)e.Item;
                Rectangle bounds = new Rectangle(Point.Empty, button.Size);
                ButtonStyle style = (button.Pressed ? ButtonStyle.Pressed : 0)
                    | (button.Checked ? ButtonStyle.Checked : 0)
                    | (button.Selected ? ButtonStyle.Selected : 0);

                if (SystemInformation.HighContrast)
                    DrawHighContrastButtonBackground(e.Graphics, bounds, style);
                else if (button.Enabled && style != ButtonStyle.None)
                    DrawThemedButtonBackground(e.Graphics, ColorTable, bounds, style);
                else if (button.Owner != null && button.BackColor != button.Owner.BackColor)
                    e.Graphics.FillRectangle(button.BackColor.GetBrush(), bounds);
            }

            /// <summary>
            /// Changes to original:
            /// - [HighContrast]: Dropped border color matches the menu border color
            /// </summary>
            protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
            {
                ToolStripDropDownButton button = (ToolStripDropDownButton)e.Item;
                Rectangle bounds = new Rectangle(Point.Empty, button.Size);
                ButtonStyle style = (button.Pressed && button.HasDropDownItems ? ButtonStyle.Dropped : 0)
                    | (button.Pressed ? ButtonStyle.Pressed : 0)
                    | (button.Selected ? ButtonStyle.Selected : 0);

                if (SystemInformation.HighContrast)
                    DrawHighContrastButtonBackground(e.Graphics, bounds, style);
                else if (button.Enabled && style != ButtonStyle.None)
                    DrawThemedButtonBackground(e.Graphics, ColorTable, bounds, style);
                else if (button.Owner != null && button.BackColor != button.Owner.BackColor)
                    e.Graphics.FillRectangle(button.BackColor.GetBrush(), bounds);
            }

            protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
            {
                #region Local Methods

                // Changes to original:
                // - Background image is omitted
                // - Separator width is ignored
                // - The separator placement matches with high contrast mode. On 100% DPI this means 1 pixel shift so the image area is perfectly rectangular
                // - Supporting AdvancedToolStripSplitButton checked state (rendering the same way as OnRenderButtonBackground does it)
                static void DrawThemed(ToolStripItemRenderEventArgs e, ProfessionalColorTable colorTable, ButtonStyle style)
                {
                    var button = (ToolStripSplitButton)e.Item;
                    Rectangle bounds = new Rectangle(Point.Empty, button.Size);

                    // common part
                    ButtonStyle commonStyle = style & (ButtonStyle.Dropped | ButtonStyle.Selected);
                    if (commonStyle != ButtonStyle.None)
                        DrawThemedButtonBackground(e.Graphics, colorTable, bounds, commonStyle);
                    else if (button.Owner != null && button.BackColor != button.Owner.BackColor)
                        e.Graphics.FillRectangle(button.BackColor.GetBrush(), bounds);

                    // button part
                    if ((style & ButtonStyle.Pressed) != 0
                        || (style & ButtonStyle.Checked) != 0
                        || (style & ButtonStyle.Selected) != 0 && (style & ButtonStyle.Dropped) == 0)
                    {
                        bounds = button.ButtonBounds;
                        if (OSUtils.IsMono)
                            bounds.Location = Point.Empty;
                        bounds.Width += 2;
                        if (button.RightToLeft == RightToLeft.Yes)
                            bounds.X -= 2;

                        DrawThemedButtonBackground(e.Graphics, colorTable, bounds, style & ~ButtonStyle.Dropped);
                    }

                    // arrow
                    bounds = button.DropDownButtonBounds;
                    if (OSUtils.IsMono)
                        bounds.X -= button.ButtonBounds.Left;

                    DrawArrow(e.Graphics, button.Enabled ? SystemColors.ControlText : SystemColors.ControlDark, bounds, ArrowDirection.Down);
                }

                // Changes to original:
                // - Fixed arrow color
                // - Fixed border color when button is not dropped
                // - Supporting AdvancedToolStripSplitButton checked state (rendering the same way as OnRenderButtonBackground does it)
                static void DrawHighContrast(ToolStripItemRenderEventArgs e, ButtonStyle style)
                {
                    var button = (ToolStripSplitButton)e.Item;
                    Rectangle bounds = new Rectangle(Point.Empty, button.Size);
                    Rectangle dropBounds = button.DropDownButtonBounds;

                    // common part
                    ButtonStyle commonStyle = style & (ButtonStyle.Dropped | ButtonStyle.Selected);
                    if (commonStyle != ButtonStyle.None)
                        DrawHighContrastButtonBackground(e.Graphics, bounds, commonStyle);

                    // button part
                    if ((style & ButtonStyle.Pressed) != 0
                        || (style & ButtonStyle.Checked) != 0
                        || (style & ButtonStyle.Selected) != 0 && (style & ButtonStyle.Dropped) == 0)
                    {
                        bounds = button.ButtonBounds;
                        bounds.Width += 2;
                        if (button.RightToLeft == RightToLeft.Yes)
                            bounds.X -= 2;

                        DrawHighContrastButtonBackground(e.Graphics, bounds, style & ~ButtonStyle.Dropped);
                    }

                    // drop down border
                    Color arrowColor = SystemColors.ControlText;
                    if ((style & ButtonStyle.Dropped) == 0 && (style & ButtonStyle.Selected) != 0)
                    {
                        e.Graphics.DrawRectangle(SystemPens.ControlLight, dropBounds.X, dropBounds.Y, dropBounds.Width - 1, dropBounds.Height - 1);
                        arrowColor = SystemColors.HighlightText;
                    }

                    // arrow
                    DrawArrow(e.Graphics, arrowColor, button.DropDownButtonBounds, ArrowDirection.Down);
                }

                #endregion

                var button = (ToolStripSplitButton)e.Item;
                ButtonStyle style = (button.DropDownButtonPressed ? ButtonStyle.Dropped : 0)
                    | (button.ButtonPressed ? ButtonStyle.Pressed : 0)
                    | (button.Selected ? ButtonStyle.Selected : 0)
                    | (button is AdvancedToolStripSplitButton { Checked: true } ? ButtonStyle.Checked : 0);

                if (SystemInformation.HighContrast)
                    DrawHighContrast(e, style);
                else
                    DrawThemed(e, ColorTable, style);
            }

            /// <summary>
            /// Changes to original:
            /// - Not drawing the default (possibly unscaled) check image
            /// - Drawing the check background also in high contrast mode
            /// - When VisualStyles are enabled, using slightly different colors than the original
            /// </summary>
            protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
            {
                int size = e.Item.Height;
                Rectangle bounds = new Rectangle(e.Item.RightToLeft == RightToLeft.Yes ? e.Item.Width - size - 1 : OSUtils.IsMono ? 1 : 2, 0, size, size);
                if (SystemInformation.HighContrast)
                    DrawHighContrastButtonBackground(e.Graphics, bounds, ButtonStyle.Selected);
                else
                    DrawThemedButtonBackground(e.Graphics, ColorTable, bounds, e.Item.Selected ? ButtonStyle.Pressed : ButtonStyle.Selected);
            }

            /// <summary>
            /// Changes to original:
            /// - Unlike Windows' base implementation, not drawing the checked menu item background again, which is already done by OnRenderItemCheck
            /// - [Mono]: Scaling menu item images
            /// - [HighContrast]: Shifting also clicked ToolStripSplitButton images just like for buttons
            /// </summary>
            protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
            {
                if (e.Image == null)
                    return;
                Rectangle bounds = e.ImageRectangle;

                // Fixing image scaling in menu items on Mono
                if (OSUtils.IsMono && e.Item is ToolStripMenuItem)
                    bounds.Size = e.Item.Owner.ScaleSize(referenceSize);
                // In high contrast mode shifting the pressed buttons by 1 pixel, including ToolStripSplitButton
                else if (SystemInformation.HighContrast && e.Item is ToolStripButton { Pressed: true } or ToolStripSplitButton { ButtonPressed: true })
                    bounds.X += 1;
#if NETFRAMEWORK
                else if (e.Item is ScalingToolStripDropDownButton btn)
                    btn.AdjustImageRectangle(ref bounds);
#endif

                // On ToolStripSplitButtons the image originally is not quite centered
                if (e.Item is ToolStripSplitButton)
                    bounds.X += e.Item.RightToLeft == RightToLeft.Yes ? -1 : 1;

                Image image = e.Item.Enabled ? e.Image : disabledImagesCache[e.Image];
                if (e.Item.ImageScaling == ToolStripItemImageScaling.None)
                    e.Graphics.DrawImage(image, bounds, new Rectangle(Point.Empty, bounds.Size), GraphicsUnit.Pixel);
                else
                    e.Graphics.DrawImage(image, bounds);
            }

            #endregion

            #endregion
        }

        #endregion

        #region Fields

        #region Static Fields

        private static readonly Size referenceSize = new Size(16, 16);

        #endregion

        #region Instance Fields

        private readonly ToolTip? toolTip;

        private DockStyle explicitDock = DockStyle.Top;
        private bool isAdjustingRtl;

        #endregion

        #endregion

        #region Constructors

        public AdvancedToolStrip()
        {
            ImageScalingSize = Size.Round(this.ScaleSize(referenceSize));
            Renderer = new AdvancedToolStripRenderer();
            toolTip = Reflector.TryGetProperty(this, nameof(ToolTip), out object? result) ? (ToolTip)result!
                : Reflector.TryGetField(this, "tooltip_window", out result) ? (ToolTip)result!
                : null;

            if (toolTip == null)
                return;
            toolTip = (ToolTip)Reflector.GetProperty(this, nameof(ToolTip))!;
#if !NET35
            if (!OSUtils.IsWindows11OrLater) 
#endif
            {
                toolTip.AutoPopDelay = Int16.MaxValue;
            }

            // Effectively used only when RTL is true because OwnerDraw is enabled only in that case
            toolTip.Draw += ToolTip_Draw;
        }

        #endregion

        #region Methods

        #region Static Methods

        private static void ToolTip_Draw(object? sender, DrawToolTipEventArgs e) => e.DrawToolTipAdvanced();

        #endregion

        #region Instance Methods

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // ensuring that items can be clicked even if the container form is not activated
            if (m.Msg == Constants.WM_MOUSEACTIVATE && m.Result == (IntPtr)Constants.MA_ACTIVATEANDEAT)
                m.Result = (IntPtr)Constants.MA_ACTIVATE;
        }

        protected override void OnItemAdded(ToolStripItemEventArgs e)
        {
            if (e.Item is ToolStripSplitButton splitBtn)
                splitBtn.DropDownButtonWidth = this.ScaleWidth(11);

            base.OnItemAdded(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            // Preventing double scaling in Mono
            if (OSUtils.IsMono && Dock.In(DockStyle.Top, DockStyle.Bottom))
                Height = this.ScaleHeight(25);
        }

        protected override void OnDockChanged(EventArgs e)
        {
            base.OnDockChanged(e);
            if (isAdjustingRtl)
                return;
            explicitDock = Dock;
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            bool isRtl = RightToLeft == RightToLeft.Yes;
            if (toolTip != null)
                toolTip.OwnerDraw = isRtl;

            DockStyle dock = Dock;
            if (dock is not (DockStyle.Left or DockStyle.Right))
                return;

            if (isRtl ^ dock == explicitDock)
                return;
            isAdjustingRtl = true;
            Dock = isRtl
                ? explicitDock == DockStyle.Left ? DockStyle.Right : DockStyle.Left
                : explicitDock;
            isAdjustingRtl = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (toolTip != null)
                    toolTip.Draw -= ToolTip_Draw;
            }

            base.Dispose(disposing);
        }

        #endregion

        #endregion
    }
}
