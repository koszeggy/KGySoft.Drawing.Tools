#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ThemeColor.cs
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

using System.Windows.Forms;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    /// <summary>
    /// Represents the theme colors used in the application.
    /// </summary>
    public enum ThemeColor
    {
        /// <summary>
        /// Represents the background color of a control.
        /// </summary>
        Control,

        /// <summary>
        /// Represents the foreground color of a control.
        /// </summary>
        ControlText,

        /// <summary>
        /// Represents the foreground color of a disabled control.
        /// </summary>
        ControlTextDisabled,

        /// <summary>
        /// Represents the highlight color of a control text. This is typically drawn below the actual text to create a 3D effect.
        /// </summary>
        ControlHighlight,

        /// <summary>
        /// Represents the background color of an input window area.
        /// </summary>
        Window,

        /// <summary>
        /// Represents the foreground color of an input window area.
        /// </summary>
        WindowText,

        /// <summary>
        /// Represents the foreground color of an input window area when it is disabled.
        /// </summary>
        WindowTextDisabled,

        /// <summary>
        /// Represents the alternating background color of an input window area.
        /// </summary>
        WindowAlternate,

        /// <summary>
        /// Represents the alternating foreground color of an input window area.
        /// </summary>
        WindowTextAlternate,

        /// <summary>
        /// Represents the background color of a highlighted element.
        /// </summary>
        Highlight,

        /// <summary>
        /// Represents the foreground color of a highlighted element.
        /// </summary>
        HighlightText,

        /// <summary>
        /// Represents the foreground color of a group box.
        /// </summary>
        GroupBoxText,

        /// <summary>
        /// Represents the color of grid lines.
        /// </summary>
        GridLine,

        /// <summary>
        /// Represents the color of unoccupied workspace area.
        /// </summary>
        Workspace,

        /// <summary>
        /// Represents the background color of a tool tip.
        /// </summary>
        ToolTip,

        /// <summary>
        /// Represents the foreground color of a tool tip.
        /// </summary>
        ToolTipText,

        /// <summary>
        /// Represents the border color of a tool tip.
        /// </summary>
        ToolTipBorder,

        /// <summary>
        /// Represents the starting color of the gradient in a <see cref="ToolStrip"/> background.
        /// </summary>
        ToolStripGradientBegin,

        /// <summary>
        /// Represents the middle color of the gradient in a <see cref="ToolStrip"/> background.
        /// </summary>
        ToolStripGradientMiddle,

        /// <summary>
        /// Represents the end color of the gradient in a <see cref="ToolStrip"/> background.
        /// </summary>
        ToolStripGradientEnd,

        /// <summary>
        /// Represents the bottom-edge border color of a <see cref="ToolStrip"/> control.
        /// </summary>
        ToolStripBorderBottom,

        /// <summary>
        /// Represents the solid background color of a dropped-down <see cref="ToolStrip"/> control.
        /// </summary>
        ToolStripDropDownBackground,

        /// <summary>
        /// Represents the solid highlight color of a <see cref="ToolStripButton"/> when it is selected.
        /// Used then visual styles are not enabled.
        /// </summary>
        ToolStripButtonSelectedHighlight,

        /// <summary>
        /// Represents the solid highlight color of a <see cref="ToolStripButton"/> when it is pressed.
        /// Used then visual styles are not enabled.
        /// </summary>
        ToolStripButtonPressedHighlight,

        /// <summary>
        /// Represents the solid highlight color of a <see cref="ToolStripButton"/> when it is checked.
        /// Used then visual styles are not enabled.
        /// </summary>
        ToolStripButtonCheckedHighlight,

        /// <summary>
        /// Represents the border color of a <see cref="ToolStripButton"/> when it is selected.
        /// </summary>
        ToolStripButtonSelectedBorder,

        /// <summary>
        /// Represents the border color of a <see cref="ToolStripButton"/> when it is pressed.
        /// </summary>
        ToolStripButtonPressedBorder,

        /// <summary>
        /// Represents the border color of a <see cref="ToolStripButton"/> when it is checked.
        /// </summary>
        ToolStripButtonCheckedBorder,

        /// <summary>
        /// Represents the border color of a <see cref="ToolStripMenuItem"/> when it is selected.
        /// </summary>
        ToolStripMenuItemSelectedBorder,

        /// <summary>
        /// Represents the border color of a <see cref="ToolStripMenuItem"/> when its submenu items are opened.
        /// </summary>
        ToolStripMenuItemOpenedBorder,

        /// <summary>
        /// Represents the border color of a <see cref="ToolStripMenuItem"/> when it is disabled.
        /// </summary>
        ToolStripMenuItemDisabledBorder,

        /// <summary>
        /// Represents the background color of a <see cref="ToolStripMenuItem"/> when it is disabled.
        /// </summary>
        ToolStripMenuItemDisabledBackground,

        /// <summary>
        /// Represents the border color of a top-level <see cref="ToolStrip"/> menu item when it is dropped down.
        /// </summary>
        ToolStripMenuBorder,

        /// <summary>
        /// Represents the starting color of the gradient of a <see cref="ToolStripMenuItem"/> when it is selected.
        /// </summary>
        ToolStripMenuItemSelectedGradientBegin,

        /// <summary>
        /// Represents the end color of the gradient of a <see cref="ToolStripMenuItem"/> when it is selected.
        /// </summary>
        ToolStripMenuItemSelectedGradientEnd,

        /// <summary>
        /// Represents the starting color of the gradient of a top-level <see cref="ToolStripMenuItem"/> when it is pressed.
        /// </summary>
        ToolStripMenuItemPressedGradientBegin,

        /// <summary>
        /// Represents the end color of the gradient of a top-level <see cref="ToolStripMenuItem"/> when it is pressed.
        /// </summary>
        ToolStripMenuItemPressedGradientEnd,

        /// <summary>
        /// Represents the starting color of the gradient of a <see cref="ToolStripMenuItem"/> when its submenu items are opened.
        /// </summary>
        ToolStripMenuItemOpenedGradientBegin,

        /// <summary>
        /// Represents the end color of the gradient of a <see cref="ToolStripMenuItem"/> when its submenu items are opened.
        /// </summary>
        ToolStripMenuItemOpenedGradientEnd,

        /// <summary>
        /// Represents the starting color of the gradient of a <see cref="ToolStripButton"/> when it is selected.
        /// </summary>
        ToolStripButtonSelectedGradientBegin,

        /// <summary>
        /// Represents the middle color of the gradient of a <see cref="ToolStripButton"/> when it is selected.
        /// </summary>
        ToolStripButtonSelectedGradientMiddle,

        /// <summary>
        /// Represents the end color of the gradient of a <see cref="ToolStripButton"/> when it is selected.
        /// </summary>
        ToolStripButtonSelectedGradientEnd,

        /// <summary>
        /// Represents the starting color of the gradient of a <see cref="ToolStripButton"/> when it is pressed.
        /// </summary>
        ToolStripButtonPressedGradientBegin,

        /// <summary>
        /// Represents the middle color of the gradient of a <see cref="ToolStripButton"/> when it is pressed.
        /// </summary>
        ToolStripButtonPressedGradientMiddle,

        /// <summary>
        /// Represents the end color of the gradient of a <see cref="ToolStripButton"/> when it is pressed.
        /// </summary>
        ToolStripButtonPressedGradientEnd,

        /// <summary>
        /// Represents the starting color of the gradient of a <see cref="ToolStripButton"/> when it is checked.
        /// </summary>
        ToolStripButtonCheckedGradientBegin,

        /// <summary>
        /// Represents the end color of the gradient of a <see cref="ToolStripButton"/> when it is checked.
        /// </summary>
        ToolStripButtonCheckedGradientEnd,

        /// <summary>
        /// Represents the starting color of the gradient in a <see cref="ToolStripOverflowButton"/>.
        /// </summary>
        ToolStripOverflowButtonGradientBegin,

        /// <summary>
        /// Represents the middle color of the gradient in a <see cref="ToolStripOverflowButton"/>.
        /// </summary>
        ToolStripOverflowButtonGradientMiddle,

        /// <summary>
        /// Represents the end color of the gradient in a <see cref="ToolStripOverflowButton"/>.
        /// </summary>
        ToolStripOverflowButtonGradientEnd,

        /// <summary>
        /// Represents the starting color of the gradient used in the image margin of <see cref="ToolStripDropDownMenu"/>.
        /// </summary>
        ToolStripImageMarginGradientBegin,

        /// <summary>
        /// Represents the middle color of the gradient used in the image margin of <see cref="ToolStripDropDownMenu"/>.
        /// </summary>
        ToolStripImageMarginGradientMiddle,

        /// <summary>
        /// Represents the end color of the gradient used in the image margin of <see cref="ToolStripDropDownMenu"/>.
        /// </summary>
        ToolStripImageMarginGradientEnd,

        /// <summary>
        /// Represents the color of the shadow effect on a tool strip grip.
        /// </summary>
        ToolStripGripDark,

        /// <summary>
        /// Represents the color of the highlight effect on a tool strip grip.
        /// </summary>
        ToolStripGripLight,

        /// <summary>
        /// Represents the color of the shadow effect on a <see cref="ToolStripSeparator"/>.
        /// </summary>
        ToolStripSeparatorDark,

        /// <summary>
        /// Represents the color of the highlight effect on a <see cref="ToolStripSeparator"/>.
        /// </summary>
        ToolStripSeparatorLight,

        /// <summary>
        /// Represents the background color of a <see cref="ProgressBar"/>.
        /// </summary>
        ProgressBarBackground,

        /// <summary>
        /// Represents the foreground color of a <see cref="ProgressBar"/>.
        /// </summary>
        ProgressBar,
    }
}