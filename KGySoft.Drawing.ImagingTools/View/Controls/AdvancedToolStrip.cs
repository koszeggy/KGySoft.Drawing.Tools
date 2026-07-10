#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdvancedToolStrip.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2026 - All Rights Reserved
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

using KGySoft.Drawing.ImagingTools.Reflection;
using KGySoft.Drawing.ImagingTools.View.Components;
using KGySoft.Drawing.ImagingTools.WinApi;
using KGySoft.WinForms;

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

            private const int referenceMenuItemPaddingWidth = 1;
            private const int referenceUpDownArrowHeight = 3;
            private const int referenceLeftRightArrowWidth = 4;
            private const int referenceOverflowButtonArrowWidth = 3;

            #endregion

            #region Fields

            private static readonly Size referenceOverflowButtonSize = new Size(16, 16);
            private static readonly Size referenceOverflowButtonThemedSize = new Size(12, 12);

            #endregion

            #region Constructors

            #region Instance Constructors

            internal AdvancedToolStripRenderer() : base(ThemeColors.ColorTable)
            {
            }

            #endregion

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

            // Changes to original: using ControlPaintHelper instead of Graphics.FillPolygon to ensure the same result on all platforms (real Windows/Mono/Wine)
            private static void DrawArrow(ToolStrip owner, Graphics g, Color color, Rectangle bounds, ArrowDirection direction)
            {
                int arrowSize = direction is ArrowDirection.Up or ArrowDirection.Down
                    ? owner.ScaleHeight(referenceUpDownArrowHeight)
                    : owner.ScaleWidth(referenceLeftRightArrowWidth);

                var bitmap = ControlPaintHelper.GetArrowImage(direction, arrowSize);
                Rectangle rect = RectangleExtensions.FromCenter(bounds.GetCenter(), bitmap.Size);
                g.DrawImageColorized(bitmap, rect, color);
            }

            /// <summary>
            /// In original, this is RenderArrowInternal. Changes to original:
            /// - using ControlPaintHelper instead of Graphics.FillPolygon/DrawLine to ensure the same result on all platforms (real Windows/Mono/Wine)
            /// - Moving line drawing here from the caller.
            /// - .NET Framework 4.5.2-: Fixed scaling
            /// - Fixing arrow direction and line position in RTL mode, vertical orientation
            /// </summary>
            private static void DrawOverflowArrow(ToolStrip owner, Graphics g, Rectangle arrowRect, ArrowDirection direction, Color color)
            {
                int arrowSize = owner.ScaleHeight(referenceOverflowButtonArrowWidth);

                var bitmap = ControlPaintHelper.GetOverflowArrowImage(direction, arrowSize);
                Point middle = arrowRect.GetCenter();

                //// if the width is odd pushing it over one pixel right.
                //middle.X += arrowRect.Width & 1;

                Rectangle rect = RectangleExtensions.FromCenter(middle, bitmap.Size);
                g.DrawImageColorized(bitmap, rect, color);
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

                if (ThemeColors.RenderWithVisualStyles)
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
                if (e.Item is ToolStripSplitButton or null)
                    return;
                Rectangle bounds = e.Item is AdvancedToolStripDropDownButton scalingButton ? scalingButton.ArrowRectangle
                    : OSHelper.IsFrameworkMono && e.Item is ToolStripMenuItem mi ? new Rectangle(e.ArrowRectangle.Left, 0, e.ArrowRectangle.Width, mi.Height)
                    : e.ArrowRectangle;
                Color color = !e.Item.Enabled ? ThemeColors.HighContrast ? SystemColors.GrayText : ThemeColors.ControlTextDisabled
                    : ThemeColors.HighContrast ? e.Item.Selected && !e.Item.Pressed ? SystemColors.HighlightText : SystemColors.ControlText
                    : ThemeColors.ControlText;

                DrawArrow(e.Item.Owner!, e.Graphics, color, bounds, e.Direction);
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

                    ToolStrip owner = e.ToolStrip!;
                    int scaledSize = owner.ScaleWidth(referenceMenuItemPaddingWidth);
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

                if (ThemeColors.HighContrast)
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
                Color textColor = !mi.Enabled ? ThemeColors.HighContrast ? SystemColors.GrayText : ThemeColors.ToolStripMenuItemTextDisabled
                    : ThemeColors.HighContrast ? mi.Selected || mi.Pressed ? SystemColors.HighlightText : SystemColors.ControlText
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

                if (ThemeColors.HighContrast)
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

                if (ThemeColors.HighContrast)
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
                // - Supporting AdvancedToolStripSplitButton disabled button state (highlighting the drop-down part only)
                // - Using theme colors
                static void DrawThemed(ToolStripItemRenderEventArgs e, ProfessionalColorTable colorTable, ButtonStyle style)
                {
                    var button = (ToolStripSplitButton)e.Item;
                    Rectangle bounds = new Rectangle(Point.Empty, button.Size);
                    ButtonStyle commonStyle = style & (ButtonStyle.Dropped | ButtonStyle.Selected);
                    bool buttonDisabled = (style & ButtonStyle.Dropped) == 0 && button is AdvancedToolStripSplitButton { ButtonEnabled: false };

                    // common part
                    if (commonStyle != ButtonStyle.None && !buttonDisabled)
                        DrawThemedButtonBackground(e.Graphics, colorTable, bounds, commonStyle);
                    else if (button.Owner != null && button.BackColor != button.Owner.BackColor)
                        e.Graphics.FillRectangle(button.BackColor.ToThemeColor().GetBrush(), bounds);

                    // drop-down part only
                    if (buttonDisabled)
                    {
                        if (((AdvancedToolStripSplitButton)button).IsDropDownHovered)
                            DrawThemedButtonBackground(e.Graphics, colorTable, button.DropDownButtonBounds, commonStyle);
                    }
                    // button part
                    else if ((style & ButtonStyle.Pressed) != 0
                        || (style & ButtonStyle.Checked) != 0
                        || (style & ButtonStyle.Selected) != 0 && (style & ButtonStyle.Dropped) == 0)
                    {
                        bounds = button.ButtonBounds;
                        if (OSHelper.IsFrameworkMono)
                            bounds.Location = Point.Empty;
                        bounds.Width += 2;
                        if (button.RightToLeft == RightToLeft.Yes)
                            bounds.X -= 2;

                        DrawThemedButtonBackground(e.Graphics, colorTable, bounds, style & ~ButtonStyle.Dropped);
                    }

                    // arrow
                    bounds = button.DropDownButtonBounds;
                    if (OSHelper.IsFrameworkMono)
                        bounds.X -= button.ButtonBounds.Left;

                    DrawArrow(e.ToolStrip!, e.Graphics, button.Enabled ? ThemeColors.ControlText : ThemeColors.ControlTextDisabled, bounds, ArrowDirection.Down);
                }

                // Changes to original:
                // - Fixed arrow color
                // - Fixed border color when button is not dropped
                // - Supporting AdvancedToolStripSplitButton checked state (rendering the same way as OnRenderButtonBackground does it)
                // - Supporting AdvancedToolStripSplitButton disabled button state (highlighting the drop-down part only)
                static void DrawHighContrast(ToolStripItemRenderEventArgs e, ButtonStyle style)
                {
                    var button = (ToolStripSplitButton)e.Item;
                    Rectangle bounds = new Rectangle(Point.Empty, button.Size);
                    Rectangle dropBounds = button.DropDownButtonBounds;
                    ButtonStyle commonStyle = style & (ButtonStyle.Dropped | ButtonStyle.Selected);
                    bool buttonDisabled = (style & ButtonStyle.Dropped) == 0 && button is AdvancedToolStripSplitButton { ButtonEnabled: false };
                    bool drawDropDownBackground = button is not AdvancedToolStripSplitButton advancedButton || advancedButton.IsDropDownHovered || advancedButton.ButtonEnabled;

                    // common part
                    if (commonStyle != ButtonStyle.None && !buttonDisabled)
                        DrawHighContrastButtonBackground(e.Graphics, bounds, commonStyle);

                    // drop-down part only
                    if (buttonDisabled)
                    {
                        if (drawDropDownBackground)
                            DrawHighContrastButtonBackground(e.Graphics, button.DropDownButtonBounds, style);
                    }
                    // button part
                    else if ((style & ButtonStyle.Pressed) != 0
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
                    if (drawDropDownBackground && (style & ButtonStyle.Dropped) == 0 && (style & ButtonStyle.Selected) != 0)
                    {
                        e.Graphics.DrawRectangle(SystemPens.ControlLight, dropBounds.X, dropBounds.Y, dropBounds.Width - 1, dropBounds.Height - 1);
                        arrowColor = SystemColors.HighlightText;
                    }

                    // arrow
                    DrawArrow(e.ToolStrip!, e.Graphics, arrowColor, button.DropDownButtonBounds, ArrowDirection.Down);
                }

                #endregion

                var button = (ToolStripSplitButton)e.Item;
                ButtonStyle style = (button.DropDownButtonPressed ? ButtonStyle.Dropped : 0)
                    | (button.ButtonPressed ? ButtonStyle.Pressed : 0)
                    | (button.Selected ? ButtonStyle.Selected : 0)
                    | (button is AdvancedToolStripSplitButton { Checked: true } ? ButtonStyle.Checked : 0);

                if (ThemeColors.HighContrast)
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
                Rectangle bounds = new Rectangle(e.Item.RightToLeft == RightToLeft.Yes ? e.Item.Width - size - 1 : OSHelper.IsFrameworkMono ? 1 : 2, 0, size, size);
                if (ThemeColors.HighContrast)
                    DrawHighContrastButtonBackground(e.Graphics, bounds, ButtonStyle.Selected);
                else
                    DrawThemedButtonBackground(e.Graphics, ColorTable, bounds, e.Item.Selected ? ButtonStyle.Pressed : ButtonStyle.Selected);
            }

            /// <summary>
            /// Changes to original:
            /// - Unlike Windows' base implementation, not drawing the checked menu item background again, which is already done by OnRenderItemCheck
            /// - Fixing the size of a disabled image when the raw format of the image is icon.
            /// - Slightly different disabled image coloring, adjusted to light/dark theme
            /// - [Mono]: Scaling menu item images
            /// - [HighContrast]: Shifting also clicked ToolStripSplitButton images just like for buttons
            /// - ToolStripDropDownButton image: manually calculated image bounds. Fixes .NET 10 per-monitor DPI awareness issue,
            ///   where e.ImageRectangle can be very distorted (e.g. 9x16) when changing DPI from 150% to 100%
            /// - AdvancedToolStripDropDownButton: respecting the ButtonEnabled property
            /// </summary>
            protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
            {
                if (e.Image == null || e.ToolStrip is null)
                    return;
                Rectangle bounds = e.Item switch
                {
                    AdvancedToolStripDropDownButton dropDownButton => dropDownButton.ImageRectangle,
                    _ => e.ImageRectangle
                };

                // Fixing image scaling in menu items on Mono
                if (OSHelper.IsFrameworkMono && e.Item is ToolStripMenuItem)
                    bounds.Size = e.ToolStrip.ScaleSize(referenceImageSize);
                // In high contrast mode shifting the pressed buttons by 1 pixel, including ToolStripSplitButton
                else if (ThemeColors.HighContrast && e.Item is ToolStripButton { Pressed: true } or ToolStripSplitButton { ButtonPressed: true })
                    bounds.X += 1;

                bool enabled = e.Item.Enabled;

                // On ToolStripSplitButtons the image originally is not quite centered, and the AdvancedToolStripSplitButton may have a disabled button
                if (e.Item is ToolStripSplitButton)
                {
                    bounds.X += e.Item.RightToLeft == RightToLeft.Yes ? -1 : 1;
                    if (e.Item is AdvancedToolStripSplitButton { ButtonEnabled: false })
                        enabled = false;
                }

                Image image = enabled ? e.Image : ControlPaintHelper.GetDisabledImage(e.Image, bounds.Size);
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
                    RenderOverflowBackground(e, colorTable);

                    bool horizontal = e.ToolStrip is not null && e.ToolStrip.Orientation == Orientation.Horizontal;
                    PointF scale = e.ToolStrip?.GetScale() ?? ScaleHelper.SystemScale;
                    bool rightToLeft = item.RightToLeft == RightToLeft.Yes;
                    Size overflowArrowSize = referenceOverflowButtonThemedSize.Scale(scale);

                    Rectangle overflowArrowRect = rightToLeft
                        ? new Rectangle(horizontal ? -2 : 2, item.Height - overflowArrowSize.Height, overflowArrowSize.Width, overflowArrowSize.Height)
                        : new Rectangle(item.Width - overflowArrowSize.Width, item.Height - overflowArrowSize.Height, overflowArrowSize.Width, overflowArrowSize.Height);
                    if (!horizontal)
                        overflowArrowRect.Offset(rightToLeft ? 0 : -1, 1);

                    ArrowDirection direction = horizontal ? ArrowDirection.Down
                        : rightToLeft ? ArrowDirection.Left
                        : ArrowDirection.Right;

                    // in RTL the white highlight goes BEFORE the black triangle.
                    int rightToLeftShift = (rightToLeft && horizontal) ? -1 : 1;

                    // draw highlight
                    DrawOverflowArrow(e.ToolStrip!, g, overflowArrowRect, direction, ThemeColors.ControlHighlight);

                    // draw black triangle
                    overflowArrowRect.Offset(-1 * rightToLeftShift, -1);
                    DrawOverflowArrow(e.ToolStrip!, g, overflowArrowRect, direction, ThemeColors.ControlText);
                }

                // Changes to original:
                // - Fixing selected arrow color (good in .NET 9)
                // - Fixed bounds (bad in .NET 9 - selection rectangle clashes with border, good in .NET 6 and earlier versions)
                // - Proper scaling for DPI (known issue on .NET Core: mouse events still may reflect to the original size)
                static void DrawHighContrast(ToolStripItemRenderEventArgs e)
                {
                    var button = (ToolStripOverflowButton)e.Item;
                    Size overflowButtonSize = e.ToolStrip!.ScaleSize(referenceOverflowButtonSize);
                    Rectangle bounds = new Rectangle(Point.Empty, button.Size);
                    if (e.ToolStrip!.Orientation is Orientation.Horizontal)
                    {
                        if (bounds.Width != overflowButtonSize.Width)
                        {
                            bounds.Width = overflowButtonSize.Width;
                            if (e.Item.RightToLeft != RightToLeft.Yes)
                                bounds.X = Math.Max(0, button.Width - overflowButtonSize.Width);
                        }
                    }
                    else if (bounds.Height != overflowButtonSize.Height)
                    {
                        bounds.Height = overflowButtonSize.Height;
                        bounds.Y = Math.Max(0, button.Height - overflowButtonSize.Height);
                    }

                    ButtonStyle style = (button.Pressed ? ButtonStyle.Dropped : 0)
                        | (button.Selected ? ButtonStyle.Selected : 0);
                    DrawHighContrastButtonBackground(e.Graphics, bounds, style);
                    DrawArrow(e.ToolStrip!, e.Graphics, style == ButtonStyle.Selected ? SystemColors.HighlightText : SystemColors.ControlText, bounds, ArrowDirection.Down);
                }

                // Changes to original:
                // - Proper scaling for DPI
                static void RenderOverflowBackground(ToolStripItemRenderEventArgs e, ProfessionalColorTable colorTable)
                {
                    Size overflowButtonSize = e.ToolStrip!.ScaleSize(referenceOverflowButtonThemedSize);

                    Graphics g = e.Graphics;
                    var item = (ToolStripOverflowButton)e.Item;
                    Rectangle overflowBoundsFill = new(Point.Empty, e.Item.Size);
                    Rectangle bounds = overflowBoundsFill;

                    bool drawCurve = e.ToolStrip?.Renderer is ToolStripProfessionalRenderer { RoundedEdges: true } && (item.GetCurrentParent() is not MenuStrip);
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

                // The scaling is wrong also in Mono, but it is not possible to fix it
                if (!OSHelper.IsFrameworkMono && e.ToolStrip != null)
                {
                    // On Windows the fix is also tricky, especially in .NET Framework 3.5, because the bounds
                    // are forcibly maxed with a constant 16, but fortunately we can exploit the fact that the
                    // Padding is respected, it's public, and it's actually not used for anything else.
                    // .NET Core is problematic in a different way, because it forcibly scales based on the primary display.
                    // The padding trick works if the current display has a larger DPI; otherwise, using the original (larger) size,
                    // but rendering with the scaled (smaller) size. The only visible issue is that mouse events work for a larger size.
                    var button = (ToolStripOverflowButton)e.Item;
                    var scaledSize = e.ToolStrip.ScaleSize(referenceOverflowButtonSize);
                    bool horizontal = e.ToolStrip.Orientation == Orientation.Horizontal;
                    if (horizontal && scaledSize.Width != button.Width || !horizontal && scaledSize.Height != button.Height)
                    {
                        var padding = button.Padding;
                        if (horizontal)
                            padding.Left = Math.Max(0, scaledSize.Width - button.Width);
                        else
                            padding.Top = Math.Max(0, scaledSize.Height - button.Height);

                        // Preventing endless repaint loops: repainting only when applying the padding adjustment for the first time.
                        // Note: setting the padding invalidates the button, but the size is changed only when performing the layout explicitly
                        if (button.Padding != padding)
                        {
                            button.Padding = padding;
                            e.ToolStrip.PerformLayout();
                            return;
                        }
                    }
                }

                if (ThemeColors.HighContrast)
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

        #endregion

        #region Fields

        #region Static Fields

        private static readonly Size referenceSize = new Size(35, 25);
        private static readonly Size referenceImageSize = new Size(16, 16);

        #endregion

        #region Instance Fields

        private readonly ToolTip? toolTip;

        private DockStyle explicitDock = DockStyle.Top;
        private bool isAdjustingRtl;
        private bool isProcessingCmdKey;

        #endregion

        #endregion

        #region Constructors

        public AdvancedToolStrip()
        {
#if !SYSTEM_THEMING
            Renderer = new AdvancedToolStripRenderer();
#endif
            toolTip = this.TryGetToolTip();
            if (toolTip == null)
                return;
            toolTip.AutoPopDelay = Int16.MaxValue; // for the tool strip it must be set explicitly
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

        internal bool ProcessCmdKeyInternal(ref Message m, Keys keyData)
        {
            if (isProcessingCmdKey)
                return false;
            isProcessingCmdKey = true;
            try
            {
                return ProcessCmdKey(ref m, keyData);
            }
            finally
            {
                isProcessingCmdKey = false;
            }
        }

        #endregion

        #region Protected Methods

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                // ensuring that items can be clicked even if the container form is not activated
                case Constants.WM_MOUSEACTIVATE:
                    base.WndProc(ref m);
                    if (m.Result == Constants.MA_ACTIVATEANDEAT)
                        m.Result = Constants.MA_ACTIVATE;
                    return;

                case Constants.WM_DPICHANGED_AFTERPARENT:
                    base.WndProc(ref m);
                    Font = Parent!.Font;
                    AdjustSizes();
                    return;

                default:
                    base.WndProc(ref m);
                    return;
            }
        }

        protected override void OnItemAdded(ToolStripItemEventArgs e)
        {
            if (e.Item is ToolStripSplitButton splitBtn)
                splitBtn.DropDownButtonWidth = this.ScaleWidth(referenceDropDownButtonWidth);

            base.OnItemAdded(e);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            AdjustSizes();
        }

        protected override void OnParentFontChanged(EventArgs e)
        {
            base.OnParentFontChanged(e);
            Font = Parent!.Font; // to prevent bad default scaling on some platforms
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
                toolTip?.Draw -= ToolTip_Draw;

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

        private void AdjustSizes()
        {
            ImageScalingSize = this.ScaleSize(referenceImageSize);
            foreach (ToolStripItem item in Items)
            {
                if (item is ToolStripSplitButton splitBtn)
                    splitBtn.DropDownButtonWidth = this.ScaleWidth(referenceDropDownButtonWidth);
            }

            if (Orientation == Orientation.Horizontal)
                Height = this.ScaleHeight(referenceSize.Height);
            else
                Width = this.ScaleWidth(referenceSize.Width);
        }

        #endregion

        #endregion

        #endregion
    }
}
