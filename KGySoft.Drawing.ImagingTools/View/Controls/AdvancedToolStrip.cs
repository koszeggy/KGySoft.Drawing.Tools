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
using System.Drawing.Imaging;
using System.Drawing.Text;
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
    /// - Custom renderer to fix rendering issues (both with and without visual styles, and in high contrast mode), scaled and correctly colored arrows, theming support.
    /// - Tool tip supports right-to-left
    /// - Clicking works even if the owner form was not active
    /// </summary>
    internal class AdvancedToolStrip : ToolStrip
    {
        #region AdvancedToolStripRenderer class
#if !SYSTEM_THEMING

        private sealed class AdvancedToolStripRenderer : ToolStripProfessionalRenderer
        {
            #region ButtonStyle enum

            [Flags]
            private enum ButtonStyle
            {
                None,
                Selected = 1,
                Pressed = 1 << 1,
                Checked = 1 << 2,
                Dropped = 1 << 3
            }

            #endregion

            #region Constants

            private const int referenceOverflowArrowOffsetY = 8;
            private const int referenceOverflowButtonWidth = 12;
            private const int referenceMenuItemPaddingWidth = 1;

            #endregion

            #region Fields

            private static readonly Size referenceOffset = new Size(2, 2);
            private static readonly Size referenceOffsetDouble = new Size(4, 4);
#if NETFRAMEWORK
            private static readonly Size referenceOverflowButtonBounds = new Size(16, 16);
#endif
            private static readonly Size referenceOverflowButtonSize = new Size(12, 12);
            private static readonly Size referenceOverflowArrowSize = new Size(9, 5);
            private static readonly Cache<Image, Image> disabledImagesCache = new(CreateDisabledImage, 8) { DisposeDroppedValues = true };

            #endregion

            #region Constructors

            internal AdvancedToolStripRenderer() : base(ThemeColors.ColorTable)
            {
            }

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
                var middle = new Point(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height / 2);

                Point[] arrow;
                Size offset = g.ScaleSize(referenceOffset);
                Size offsetDouble = g.ScaleSize(referenceOffsetDouble);

                switch (direction)
                {
                    case ArrowDirection.Up:
                        arrow =
                        [
                            new Point(middle.X - offset.Width - 1, middle.Y + 1),
                            new Point(middle.X + offset.Width + 1, middle.Y + 1),
                            new Point(middle.X, middle.Y - offset.Height - 1)
                        ];
                        break;
                    case ArrowDirection.Left:
                        arrow =
                        [
                            new Point(middle.X + offset.Width, middle.Y - offsetDouble.Height),
                            new Point(middle.X + offset.Width, middle.Y + offsetDouble.Height),
                            new Point(middle.X - offset.Width, middle.Y)
                        ];
                        break;
                    case ArrowDirection.Right:
                        arrow =
                        [
                            new Point(middle.X - offset.Width, middle.Y - offsetDouble.Height),
                            new Point(middle.X - offset.Width, middle.Y + offsetDouble.Height),
                            new Point(middle.X + offset.Width, middle.Y)
                        ];
                        break;
                    default:
                        arrow =
                        [
                            new Point(middle.X - offset.Width, middle.Y - 1),
                            new Point(middle.X + offset.Width + (OSUtils.IsMono && OSUtils.IsLinux ? 2 : 1), middle.Y - 1),
                            new Point(middle.X, middle.Y + offset.Height)
                        ];
                        break;
                }

                g.FillPolygon(color.GetBrush(), arrow);
            }

            /// <summary>
            /// In original, this is RenderArrowInternal. Changes to original:
            /// - Moving line drawing here from the caller.
            /// - .NET Framework 4.5.2-: Fixed scaling
            /// - Fixing arrow direction and line position in RTL mode, vertical orientation
            /// </summary>
            private static void DrawOverflowArrow(Graphics g, Rectangle arrowRect, ArrowDirection direction, Color color)
            {
                var middle = new Point(arrowRect.Left + arrowRect.Width / 2, arrowRect.Top + arrowRect.Height / 2);

                // if the width is odd - favor pushing it over one pixel right.
                middle.X += arrowRect.Width % 2;
                Size offset = g.ScaleSize(referenceOffset);

                Point[] arrow = direction switch
                {
                    ArrowDirection.Up =>
                    [
                        new(middle.X - offset.Width, middle.Y + 1),
                        new(middle.X + offset.Width + 1, middle.Y + 1),
                        new(middle.X, middle.Y - offset.Height)
                    ],
                    ArrowDirection.Left =>
                    [
                        new(middle.X + offset.Width, middle.Y - offset.Height - 1),
                        new(middle.X + offset.Width, middle.Y + offset.Height + 1),
                        new(middle.X - 1, middle.Y)
                    ],
                    ArrowDirection.Right =>
                    [
                        new(middle.X - offset.Width, middle.Y - offset.Height - 1),
                        new(middle.X - offset.Width, middle.Y + offset.Height + 1),
                        new(middle.X + 1, middle.Y)
                    ],
                    _ =>
                    [
                        new(middle.X - offset.Width, middle.Y - 1),
                        new(middle.X + offset.Width + 1, middle.Y - 1),
                        new(middle.X, middle.Y + offset.Height)
                    ],
                };

                g.FillPolygon(color.GetBrush(), arrow);

                // horizontal line
                if (direction is ArrowDirection.Up or ArrowDirection.Down)
                {
                    g.DrawLine(color.GetPen(), middle.X - offset.Width, arrowRect.Y - offset.Height, middle.X + offset.Width, arrowRect.Y - offset.Height);
                    return;
                }

                // vertical line
                if (direction is ArrowDirection.Right)
                    g.DrawLine(color.GetPen(), arrowRect.X, middle.Y - offset.Height, arrowRect.X, middle.Y + offset.Height);
                else
                    g.DrawLine(color.GetPen(), middle.X + offset.Width * 2, middle.Y - offset.Height, middle.X + offset.Width * 2, middle.Y + offset.Height);
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
                        backgroundStart = colorTable.ButtonCheckedGradientBegin;
                        backgroundEnd = colorTable.ButtonCheckedGradientEnd;
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
                    if (!backColor.IsEmpty)
                        g.FillRectangle(backColor.GetBrush(), bounds);
                }

                static void RenderBorder(Graphics g, ProfessionalColorTable colorTable, Rectangle bounds, ButtonStyle style)
                {
                    Color color = (style & ButtonStyle.Checked) != 0 ? ThemeColors.ToolStripButtonCheckedBorder
                        : (style & ButtonStyle.Pressed) != 0 ? ThemeColors.ToolStripButtonPressedBorder
                        : (style & ButtonStyle.Selected) != 0 ? colorTable.ButtonSelectedBorder
                        : Color.Empty;
                    if (!color.IsEmpty)
                        g.DrawRectangle(color.GetPen(), bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
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

                RenderBorder(g, colorTable, bounds, style);
            }

            private static void DrawHighContrastButtonBackground(Graphics g, Rectangle bounds, ButtonStyle style)
            {
                if ((style & ButtonStyle.Dropped) == 0 && (style & (ButtonStyle.Selected | ButtonStyle.Checked | ButtonStyle.Pressed)) != 0)
                    g.FillRectangle(SystemBrushes.Highlight, bounds);

                Color borderColor = (style & ButtonStyle.Dropped) != 0 ? SystemColors.ControlLightLight // NOTE: ButtonHighlight in original. Same in all high contrast colors but custom theming can make a difference, and ButtonHighlight is for text.
                    : (style & ButtonStyle.Pressed) == 0 && (style & (ButtonStyle.Checked | ButtonStyle.Selected)) is ButtonStyle.Checked or ButtonStyle.Selected ? SystemColors.ControlLight
                    : Color.Empty;

                if (!borderColor.IsEmpty)
                    g.DrawRectangle(borderColor.GetPen(), bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            }

            // No change compared to original. Unfortunately we must clone it because it's called from code using non color-table colors or wrong scaling.
            private static void FillWithDoubleGradient(Color beginColor, Color middleColor, Color endColor, Graphics g, Rectangle bounds, int firstGradientWidth, int secondGradientWidth, LinearGradientMode mode, bool flipHorizontal)
            {
                if ((bounds.Width == 0) || (bounds.Height == 0))
                    return;

                Rectangle endGradient = bounds;
                Rectangle beginGradient = bounds;
                bool useDoubleGradient;

                if (mode == LinearGradientMode.Horizontal)
                {
                    if (flipHorizontal)
                        (beginColor, endColor) = (endColor, beginColor);

                    beginGradient.Width = firstGradientWidth;
                    endGradient.Width = secondGradientWidth + 1;
                    endGradient.X = bounds.Right - endGradient.Width;
                    useDoubleGradient = (bounds.Width > (firstGradientWidth + secondGradientWidth));
                }
                else
                {
                    beginGradient.Height = firstGradientWidth;
                    endGradient.Height = secondGradientWidth + 1;
                    endGradient.Y = bounds.Bottom - endGradient.Height;
                    useDoubleGradient = (bounds.Height > (firstGradientWidth + secondGradientWidth));
                }

                if (useDoubleGradient)
                {
                    // Fill with middleColor
                    g.FillRectangle(middleColor.GetBrush(), bounds);

                    // draw first gradient
                    using (Brush b = new LinearGradientBrush(beginGradient, beginColor, middleColor, mode))
                        g.FillRectangle(b, beginGradient);

                    // draw second gradient
                    using (LinearGradientBrush b = new(endGradient, middleColor, endColor, mode))
                    {
                        if (mode == LinearGradientMode.Horizontal)
                        {
                            endGradient.X += 1;
                            endGradient.Width -= 1;
                        }
                        else
                        {
                            endGradient.Y += 1;
                            endGradient.Height -= 1;
                        }

                        g.FillRectangle(b, endGradient);
                    }
                }
                else
                {
                    // not big enough for a swath in the middle. Let's just do a single gradient.
                    using Brush b = new LinearGradientBrush(bounds, beginColor, endColor, mode);
                    g.FillRectangle(b, bounds);
                }
            }

            #endregion

            #region Instance Methods

            /// <summary>
            /// Changes to original:
            /// - Fixed color + theming
            /// - Fixed scaling
            /// - [Mono]: Ignoring ToolStripSplitButton because it is painted along with the button just like in the MS world.
            /// - [Mono]: Fixing menu item arrow position in high DPI mode
            /// </summary>
            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
            {
                if (e.Item is ToolStripSplitButton)
                    return;
                Rectangle bounds = e.Item is ScalingToolStripDropDownButton scalingButton ? scalingButton.ArrowRectangle
                    : OSUtils.IsMono && e.Item is ToolStripMenuItem mi ? new Rectangle(e.ArrowRectangle.Left, 0, e.ArrowRectangle.Width, mi.Height)
                    : e.ArrowRectangle;
                Color color = !e.Item.Enabled ? SystemInformation.HighContrast ? SystemColors.GrayText : ThemeColors.ControlTextDisabled
                    : SystemInformation.HighContrast ? e.Item.Selected && !e.Item.Pressed ? SystemColors.HighlightText : SystemColors.ControlText
                    : ThemeColors.ControlText;

                DrawArrow(e.Graphics, color, bounds, e.Direction);
            }

            /// <summary>
            /// Changes to original:
            /// - [HighContrast]: Not drawing the highlighted background if the menu item is disabled (this is already fixed in Core)
            /// - [HighContrast]: Fixed bounds of highlight rectangle (it was good in .NET Framework but is wrong in Core)
            /// - [Themed]: Background image is omitted
            /// - [Themed]: Using colorTable.MenuItemBorder (ToolStripMenuItemSelectedBorder) and ThemeColors.ToolStripMenuItemOpenedBorder
            ///   instead of SystemColors.Highlight even when visual styles are not enabled (they are usually the same anyway)
            /// - [Themed]: Allowing different colors for disabled, selected and pressed menu items (i.e. when submenu items are opened)
            /// - [Themed]: Not using ButtonSelectedHighlight instead of MenuItemSelectedGradientBegin/End when visual styles are not enabled
            /// </summary>
            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                #region Local Methods

                static void DrawHighContrast(ToolStripItemRenderEventArgs e)
                {
                    // Selected/pressed menu point in high contrast mode: drawing the background only if enabled
                    var item = e.Item;
                    var bounds = new Rectangle(2, 0, item.Width - 3, item.Height);
                    if (item.Pressed || item.Selected && item.Enabled)
                        e.Graphics.FillRectangle(SystemBrushes.Highlight, bounds);
                    else if (item.Selected && !item.Enabled)
                        e.Graphics.DrawRectangle(SystemPens.Highlight, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                }

                static void DrawThemed(ToolStripItemRenderEventArgs e, ProfessionalColorTable colorTable)
                {
                    ToolStripItem item = e.Item;
                    Graphics g = e.Graphics;
                    Rectangle bounds = new(Point.Empty, item.Size);

                    int scaledSize = g.ScaleWidth(referenceMenuItemPaddingWidth);
                    bounds.X += scaledSize + 1;
                    bounds.Width -= scaledSize * 2 + 1;
                    Color backgroundStart;
                    Color backgroundEnd;
                    if (!item.Selected)
                        backgroundStart = backgroundEnd = item.Owner is not null && item.BackColor != item.Owner.BackColor ? item.BackColor : Color.Empty;
                    else if (!item.Enabled)
                        backgroundStart = backgroundEnd = ThemeColors.ToolStripMenuItemDisabledBackground;
                    else if (item.Pressed)
                    {
                        backgroundStart = ThemeColors.ToolStripMenuItemOpenedGradientBegin;
                        backgroundEnd = ThemeColors.ToolStripMenuItemOpenedGradientEnd;
                    }
                    else
                    {
                        backgroundStart = colorTable.MenuItemSelectedGradientBegin;
                        backgroundEnd = colorTable.MenuItemSelectedGradientEnd;
                    }

                    FillBackground(g, bounds, backgroundStart, backgroundEnd);

                    Color borderColor = !item.Selected ? Color.Empty
                        : !item.Enabled ? ThemeColors.ToolStripMenuItemDisabledBorder
                        : item.Pressed ? ThemeColors.ToolStripMenuItemOpenedBorder
                        : colorTable.MenuItemBorder;
                    if (!borderColor.IsEmpty)
                        g.DrawRectangle(borderColor.GetPen(), bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                }

                #endregion

                if (e.Item is not ToolStripMenuItem || !e.Item.IsOnDropDown)
                {
                    base.OnRenderMenuItemBackground(e);
                    return;
                }

                if (SystemInformation.HighContrast)
                    DrawHighContrast(e);
                else
                    DrawThemed(e, ColorTable);
            }

            /// <summary>
            /// Changes to original:
            /// - When a menu item is selected, then not using its possible custom colors
            /// - [HighContrast]: Fixing text color on highlighted menu items
            /// - Theme colors, including disabled text color
            /// </summary>
            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                if (e.Item is not ToolStripMenuItem mi)
                {
                    base.OnRenderItemText(e);
                    return;
                }

                Rectangle textRect = e.TextRectangle;
                Color textColor = !mi.Enabled ? SystemInformation.HighContrast ? SystemColors.GrayText : ThemeColors.ControlTextDisabled
                    : SystemInformation.HighContrast ? mi.Selected || mi.Pressed ? SystemColors.HighlightText : SystemColors.ControlText
                    : mi.Selected || mi.Pressed ? ThemeColors.ControlText
                    : e.Item.ForeColor.ToThemeColor();

                if (e.TextDirection == ToolStripTextDirection.Horizontal || textRect.Width == 0 || textRect.Height == 0)
                {
                    TextRenderer.DrawText(e.Graphics, e.Text, e.TextFont, textRect, textColor, e.TextFormat);
                    return;
                }

                Size textSize = new(textRect.Height, textRect.Width);
                using Bitmap textBmp = new(textSize.Width, textSize.Height, PixelFormat.Format32bppPArgb);
                using Graphics textGraphics = Graphics.FromImage(textBmp);
                textGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                TextRenderer.DrawText(textGraphics, e.Text, e.TextFont, new Rectangle(Point.Empty, textSize), textColor, e.TextFormat);
                textBmp.RotateFlip((e.TextDirection == ToolStripTextDirection.Vertical90) ? RotateFlipType.Rotate90FlipNone : RotateFlipType.Rotate270FlipNone);
                e.Graphics.DrawImage(textBmp, textRect);
            }

            /// <summary>
            /// Changes to original:
            /// - Background image is omitted
            /// - Even with default theme colors, the checked background is not transparent
            /// - More theme colors than in original, e.g. allowing different border colors for selected, pressed and checked
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
                    e.Graphics.FillRectangle(button.BackColor.ToThemeColor().GetBrush(), bounds);
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
                    e.Graphics.FillRectangle(button.BackColor.ToThemeColor().GetBrush(), bounds);
            }

            protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
            {
                #region Local Methods

                // Changes to original:
                // - Background image is omitted
                // - Separator width is ignored
                // - The separator placement matches with high contrast mode. On 100% DPI this means 1 pixel shift so the image area is perfectly rectangular
                // - Supporting AdvancedToolStripSplitButton checked state (rendering the same way as OnRenderButtonBackground does it)
                // - Using theme colors
                static void DrawThemed(ToolStripItemRenderEventArgs e, ProfessionalColorTable colorTable, ButtonStyle style)
                {
                    var button = (ToolStripSplitButton)e.Item;
                    Rectangle bounds = new Rectangle(Point.Empty, button.Size);

                    // common part
                    ButtonStyle commonStyle = style & (ButtonStyle.Dropped | ButtonStyle.Selected);
                    if (commonStyle != ButtonStyle.None)
                        DrawThemedButtonBackground(e.Graphics, colorTable, bounds, commonStyle);
                    else if (button.Owner != null && button.BackColor != button.Owner.BackColor)
                        e.Graphics.FillRectangle(button.BackColor.ToThemeColor().GetBrush(), bounds);

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

                    DrawArrow(e.Graphics, button.Enabled ? ThemeColors.ControlText : ThemeColors.ControlTextDisabled, bounds, ArrowDirection.Down);
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
                    Color arrowColor = button.Enabled ? SystemColors.ControlText : SystemColors.GrayText;
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
            /// - When Visual Styles are enabled, falling back to use ButtonCheckedHighlight when ButtonCheckedGradientBegin/End are empty
            /// - When Visual Styles are not enabled, using ButtonPressedHighlight/ButtonSelectedHighlight/ButtonCheckedHighlight instead of CheckBackground/CheckSelectedBackground/CheckPressedBackground
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
                    bounds.Size = e.Item.Owner.ScaleSize(referenceImageSize);
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

            /// <summary>
            /// Changes to original:
            /// - [.NET Framework] Fixed scaling (without custom scaling would work in 4.5.2+ though)
            /// - [RTL/Vertical] Fixing arrow direction and line position
            /// - See more in the local methods comments
            /// </summary>
            protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
            {
                #region Local Methods

                // Changes to original:
                // - In RTL/Vertical mode the arrow points to the left, and the line is drawn to the right
                // - The arrow and its line is drawn together in DrawOverflowArrow (in original: RenderArrowInternal)
                // - Themed colors
                static void DrawThemed(ToolStripItemRenderEventArgs e, ProfessionalColorTable colorTable)
                {
                    ToolStripItem item = e.Item;
                    Graphics g = e.Graphics;

                    // fill in the background colors
                    bool rightToLeft = item.RightToLeft == RightToLeft.Yes;
                    PointF scale = e.Graphics.GetScale();
                    Size overflowArrowSize = referenceOverflowArrowSize.Scale(scale);
                    int overflowArrowOffsetY = referenceOverflowArrowOffsetY.Scale(scale.Y);
                    int overflowButtonWidth = referenceOverflowButtonWidth.Scale(scale.X);

                    RenderOverflowBackground(e, colorTable);

                    bool horizontal = e.ToolStrip is not null && e.ToolStrip.Orientation == Orientation.Horizontal;
                    Rectangle overflowArrowRect = rightToLeft
                        ? new Rectangle(0, item.Height - overflowArrowOffsetY, overflowArrowSize.Width, overflowArrowSize.Height)
                        : new Rectangle(item.Width - overflowButtonWidth, item.Height - overflowArrowOffsetY, overflowArrowSize.Width, overflowArrowSize.Height);

                    ArrowDirection direction = horizontal ? ArrowDirection.Down
                        : rightToLeft ? ArrowDirection.Left
                        : ArrowDirection.Right;

                    // in RTL the white highlight goes BEFORE the black triangle.
                    int rightToLeftShift = (rightToLeft && horizontal) ? -1 : 1;

                    // draw highlight
                    overflowArrowRect.Offset(1 * rightToLeftShift, 1);
                    DrawOverflowArrow(g, overflowArrowRect, direction, ThemeColors.ControlHighlight);

                    // draw black triangle
                    overflowArrowRect.Offset(-1 * rightToLeftShift, -1);
                    DrawOverflowArrow(g, overflowArrowRect, direction, ThemeColors.ControlText);
                }

                // Changes to original:
                // - Fixing selected arrow color (good in .NET 9)
                // - Fixed bounds (bad in .NET 9 - selection rectangle clashes with border, good in .NET 6 and earlier versions)
                static void DrawHighContrast(ToolStripItemRenderEventArgs e)
                {
                    var button = (ToolStripOverflowButton)e.Item;
                    Rectangle bounds = new Rectangle(Point.Empty, button.Size);
                    ButtonStyle style = (button.Pressed ? ButtonStyle.Dropped : 0)
                        | (button.Selected ? ButtonStyle.Selected : 0);
                    DrawHighContrastButtonBackground(e.Graphics, bounds, style);
                    DrawArrow(e.Graphics, style == ButtonStyle.Selected ? SystemColors.HighlightText : SystemColors.ControlText, bounds, ArrowDirection.Down);
                }

                static void RenderOverflowBackground(ToolStripItemRenderEventArgs e, ProfessionalColorTable colorTable)
                {
                    Size overflowButtonSize = e.Graphics.ScaleSize(referenceOverflowButtonSize);

                    Graphics g = e.Graphics;
                    var item = (ToolStripOverflowButton)e.Item!;
                    Rectangle overflowBoundsFill = new(Point.Empty, e.Item.Size);
                    Rectangle bounds = overflowBoundsFill;

                    bool drawCurve = (e.ToolStrip.Renderer as ToolStripProfessionalRenderer)?.RoundedEdges == true && (item.GetCurrentParent() is not MenuStrip);
                    bool horizontal = e.ToolStrip?.Orientation == Orientation.Horizontal;
                    bool rightToLeft = item.RightToLeft == RightToLeft.Yes;

                    if (horizontal)
                    {
                        overflowBoundsFill.X += overflowBoundsFill.Width - overflowButtonSize.Width + 1;
                        overflowBoundsFill.Width = overflowButtonSize.Width;
                        if (rightToLeft)
                            overflowBoundsFill.X = bounds.Width - overflowBoundsFill.Right;
                    }
                    else
                    {
                        overflowBoundsFill.Y = overflowBoundsFill.Height - overflowButtonSize.Height + 1;
                        overflowBoundsFill.Height = overflowButtonSize.Height;
                    }

                    Color overflowButtonGradientBegin, overflowButtonGradientMiddle, overflowButtonGradientEnd, overflowBottomLeftShadow, overflowTopShadow;

                    if (item.Pressed)
                    {
                        overflowButtonGradientBegin = colorTable.ButtonPressedGradientBegin;
                        overflowButtonGradientMiddle = colorTable.ButtonPressedGradientMiddle;
                        overflowButtonGradientEnd = colorTable.ButtonPressedGradientEnd;
                        overflowBottomLeftShadow = colorTable.ButtonPressedGradientBegin;
                        overflowTopShadow = overflowBottomLeftShadow;
                    }
                    else if (item.Selected)
                    {
                        overflowButtonGradientBegin = colorTable.ButtonSelectedGradientBegin;
                        overflowButtonGradientMiddle = colorTable.ButtonSelectedGradientMiddle;
                        overflowButtonGradientEnd = colorTable.ButtonSelectedGradientEnd;
                        overflowBottomLeftShadow = colorTable.ButtonSelectedGradientMiddle;
                        overflowTopShadow = overflowBottomLeftShadow;
                    }
                    else
                    {
                        overflowButtonGradientBegin = colorTable.OverflowButtonGradientBegin;
                        overflowButtonGradientMiddle = colorTable.OverflowButtonGradientMiddle;
                        overflowButtonGradientEnd = colorTable.OverflowButtonGradientEnd;
                        overflowBottomLeftShadow = colorTable.ToolStripBorder;
                        overflowTopShadow = horizontal ? colorTable.ToolStripGradientMiddle : colorTable.ToolStripGradientEnd;
                    }

                    if (drawCurve)
                    {
                        // draw shadow pixel on bottom left +1, +1
                        Point start = new(overflowBoundsFill.Left - 1, overflowBoundsFill.Height - 2);
                        Point end = new(overflowBoundsFill.Left, overflowBoundsFill.Height - 2);
                        if (rightToLeft)
                        {
                            start.X = overflowBoundsFill.Right + 1;
                            end.X = overflowBoundsFill.Right;
                        }

                        g.DrawLine(overflowBottomLeftShadow.GetPen(), start, end);
                    }

                    LinearGradientMode mode = horizontal ? LinearGradientMode.Vertical : LinearGradientMode.Horizontal;

                    // fill main body
                    FillWithDoubleGradient(overflowButtonGradientBegin, overflowButtonGradientMiddle, overflowButtonGradientEnd, g, overflowBoundsFill,
                        overflowButtonSize.Width, overflowButtonSize.Width, mode, false);

                    if (!drawCurve)
                        return;

                    // Render shadow pixels (ToolStrip only)

                    // top left and top right shadow pixels
                    if (horizontal)
                    {
                        Point top1 = new(overflowBoundsFill.X - 2, 0);
                        Point top2 = new(overflowBoundsFill.X - 1, 1);

                        if (rightToLeft)
                        {
                            top1.X = overflowBoundsFill.Right + 1;
                            top2.X = overflowBoundsFill.Right;
                        }

                        Brush brush = overflowTopShadow.GetBrush();
                        g.FillRectangle(brush, top1.X, top1.Y, 1, 1);
                        g.FillRectangle(brush, top2.X, top2.Y, 1, 1);
                    }
                    else
                    {
                        Brush brush = overflowTopShadow.GetBrush();
                        g.FillRectangle(brush, overflowBoundsFill.Width - 3, overflowBoundsFill.Top - 1, 1, 1);
                        g.FillRectangle(brush, overflowBoundsFill.Width - 2, overflowBoundsFill.Top - 2, 1, 1);
                    }

                    if (horizontal)
                    {
                        Rectangle fillRect = new(overflowBoundsFill.X - 1, 0, 1, 1);
                        if (rightToLeft)
                            fillRect.X = overflowBoundsFill.Right;

                        g.FillRectangle(overflowButtonGradientBegin.GetBrush(), fillRect);
                    }
                    else
                        g.FillRectangle(overflowButtonGradientBegin.GetBrush(), overflowBoundsFill.X, overflowBoundsFill.Top - 1, 1, 1);
                }

                #endregion

                var button = (ToolStripOverflowButton)e.Item;

#if NETFRAMEWORK
                // The scaling is wrong also in Mono, but it is not possible to fix it
                if (!OSUtils.IsMono)
                {
                    // On Windows the fix is also tricky, especially in .NET Framework 3.5 because the bounds
                    // are forcibly maxed with a constant 16, but fortunately we can exploit the fact that the
                    // Padding is respected, it's public, and it's actually not used for anything else.
                    var scaledSize = e.Graphics.ScaleSize(referenceOverflowButtonBounds);
                    if (e.ToolStrip.Orientation == Orientation.Horizontal && scaledSize.Width > button.Width)
                    {
                        var padding = button.Padding;
                        padding.Left = scaledSize.Width - referenceOverflowButtonBounds.Width;
                        button.Padding = padding;
                        return; // setting the padding invalidates the layout so a new paint event will be triggered
                    }
                    if (e.ToolStrip.Orientation == Orientation.Vertical && scaledSize.Height > button.Height)
                    {
                        var padding = button.Padding;
                        padding.Top = scaledSize.Height - referenceOverflowButtonBounds.Height;
                        button.Padding = padding;
                        return; // setting the padding invalidates the layout so a new paint event will be triggered
                    }
                }
#endif

                if (SystemInformation.HighContrast)
                    DrawHighContrast(e);
                else
                    DrawThemed(e, ColorTable);
            }

            #endregion

            #endregion
        }

#endif
        #endregion

        #region Constants

        private const int referenceDropDownButtonWidth = 11;
        private const int referenceItemHeight = 25;

        #endregion

        #region Fields

        #region Static Fields

        private static readonly Size referenceImageSize = new Size(16, 16);

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
            ImageScalingSize = this.ScaleSize(referenceImageSize);
#if !SYSTEM_THEMING
            Renderer = new AdvancedToolStripRenderer();
#endif
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

            toolTip.Draw += ToolTip_Draw;
        }

        #endregion

        #region Methods

        #region Static Methods

        private static void ToolTip_Draw(object? sender, DrawToolTipEventArgs e) => e.DrawToolTipAdvanced();

        #endregion

        #region Instance Methods

        #region Internal Methods

        // Needed only for the tooltip, as every other rendering applies the theme colors automatically
        internal void ApplyTheme() => ResetToolTipAppearance();

        #endregion

        #region Protected Methods

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
                splitBtn.DropDownButtonWidth = this.ScaleWidth(referenceDropDownButtonWidth);

            base.OnItemAdded(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            // Preventing double scaling in Mono
            if (OSUtils.IsMono && Dock.In(DockStyle.Top, DockStyle.Bottom))
                Height = this.ScaleHeight(referenceItemHeight);
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

            ResetToolTipAppearance();
            DockStyle dock = Dock;
            if (dock is not (DockStyle.Left or DockStyle.Right))
                return;

            bool isRtl = RightToLeft == RightToLeft.Yes;
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

        #region Private Methods

        private void ResetToolTipAppearance()
        {
            if (toolTip is not ToolTip instance)
                return;
            instance.OwnerDraw = RightToLeft == RightToLeft.Yes
                || ThemeColors.IsSet(ThemeColor.ToolTip) || ThemeColors.IsSet(ThemeColor.ToolTipBorder) || ThemeColors.IsSet(ThemeColor.ToolTipText);
        }

        #endregion

        #endregion

        #endregion
    }
}
