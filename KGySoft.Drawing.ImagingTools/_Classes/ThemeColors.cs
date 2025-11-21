#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ThemeColors.cs
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
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.View;
using KGySoft.Drawing.ImagingTools.WinApi;

using Microsoft.Win32;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    /// <summary>
    /// Represents the colors used by a theme. If no theme is used, then the default colors are used.
    /// </summary>
    public static partial class ThemeColors
    {
        #region Constants

        private const ThemeColor unknownColor = (ThemeColor)(-1);

        #endregion

        #region Fields

        private static readonly Color[] defaultThemeColors =
        [
            SystemColors.Control,
            SystemColors.ControlText,
            SystemColors.ControlDarkDark, // ControlTextDisabled
            SystemColors.ButtonHighlight, // ControlHighlight
            SystemColors.Window,
            SystemColors.WindowText,
            SystemColors.GrayText, // WindowTextDisabled
            SystemColors.ControlLight, // WindowAlternate
            SystemColors.ControlText, // WindowTextAlternate
            SystemColors.Highlight,
            SystemColors.HighlightText,
            SystemColors.ControlText, // GroupBoxText - NOTE: with visual styles enabled it's returned by VisualStyleRenderer
            SystemColors.WindowFrame, // GridLine
            SystemColors.AppWorkspace, // Workspace
            SystemColors.Info, // ToolTip - NOTE: with visual styles enabled it is Window in Windows Vista and later
            SystemColors.InfoText, // ToolTipText - NOTE: with visual styles enabled it is WindowText in Windows Vista and later
            SystemColors.WindowFrame, // ToolTipBorder - NOTE: with visual styles enabled it is WindowText in Windows Vista and later
            ProfessionalColors.ToolStripGradientBegin,
            ProfessionalColors.ToolStripGradientMiddle,
            ProfessionalColors.ToolStripGradientEnd, // ButtonFace
            ProfessionalColors.ToolStripBorder, // ToolStripBorderBottom
            ProfessionalColors.ToolStripDropDownBackground,
            ProfessionalColors.ButtonSelectedHighlight, // ToolStripButtonSelectedHighlight
            ProfessionalColors.ButtonPressedHighlight, // ToolStripButtonPressedHighlight
            ProfessionalColors.ButtonCheckedHighlight, // ToolStripButtonCheckedHighlight
#if NET35
            SystemColors.Highlight, // ToolStripButtonSelectedBorder - In .NET Framework 3.5 ButtonSelectedBorder returns ButtonCheckedGradientBegin
            SystemColors.Highlight, // ToolStripButtonPressedBorder - In .NET Framework 3.5 ButtonSelectedBorder returns ButtonCheckedGradientBegin
            SystemColors.Highlight, // ToolStripButtonCheckedBorder - In .NET Framework 3.5 ButtonSelectedBorder returns ButtonCheckedGradientBegin
#else
            ProfessionalColors.ButtonSelectedBorder, // ToolStripButtonSelectedBorder (Highlight)  
            ProfessionalColors.ButtonSelectedBorder, // ToolStripButtonPressedBorder (Highlight)  
            ProfessionalColors.ButtonSelectedBorder, // ToolStripButtonCheckedBorder (Highlight)  
#endif
            ProfessionalColors.MenuItemBorder, // ToolStripMenuItemSelectedBorder (Highlight)
            ProfessionalColors.MenuItemBorder, // ToolStripMenuItemOpenedBorder (Highlight)
            ProfessionalColors.MenuItemBorder, // ToolStripMenuItemDisabledBorder (Highlight)
            Color.Empty, // ToolStripMenuItemDisabledBackground
            ProfessionalColors.MenuBorder, // ToolStripMenuBorder
            ProfessionalColors.MenuItemSelectedGradientBegin, // ToolStripMenuItemSelectedGradientBegin
            ProfessionalColors.MenuItemSelectedGradientEnd, // ToolStripMenuItemSelectedGradientEnd
            ProfessionalColors.MenuItemPressedGradientBegin, // ToolStripMenuItemPressedGradientBegin
            ProfessionalColors.MenuItemPressedGradientEnd, // ToolStripMenuItemPressedGradientEnd
            ProfessionalColors.MenuItemSelectedGradientBegin, // ToolStripMenuItemOpenedGradientBegin
            ProfessionalColors.MenuItemSelectedGradientEnd, // ToolStripMenuItemOpenedGradientEnd
            ProfessionalColors.ButtonSelectedGradientBegin, // ToolStripButtonSelectedGradientBegin
            ProfessionalColors.ButtonSelectedGradientMiddle, // ToolStripButtonSelectedGradientMiddle
            ProfessionalColors.ButtonSelectedGradientEnd, // ToolStripButtonSelectedGradientEnd
            ProfessionalColors.ButtonPressedGradientBegin, // ToolStripButtonPressedGradientBegin
            ProfessionalColors.ButtonPressedGradientMiddle, // ToolStripButtonPressedGradientMiddle
            ProfessionalColors.ButtonPressedGradientEnd, // ToolStripButtonPressedGradientEnd
            ProfessionalColors.ButtonCheckedHighlight, // ToolStripButtonCheckedGradientBegin (Original: Empty)
            ProfessionalColors.ButtonCheckedHighlight, // ToolStripButtonCheckedGradientEnd (Original: Empty)
            ProfessionalColors.OverflowButtonGradientBegin, // ToolStripOverflowButtonGradientBegin
            ProfessionalColors.OverflowButtonGradientMiddle, // ToolStripOverflowButtonGradientMiddle
            ProfessionalColors.OverflowButtonGradientEnd, // ToolStripOverflowButtonGradientEnd (ButtonShadow)
            ProfessionalColors.ImageMarginGradientMiddle, // ToolStripImageMarginGradientMiddle
            ProfessionalColors.OverflowButtonGradientMiddle, // ToolStripOverflowButtonGradientMiddle
            ProfessionalColors.ImageMarginGradientEnd, // ToolStripImageMarginGradientEnd (Control)
            ProfessionalColors.GripDark, // ToolStripGripDark
            ProfessionalColors.GripLight, // ToolStripGripLight (Window)
            ProfessionalColors.SeparatorDark, // ToolStripSeparatorDark
            ProfessionalColors.SeparatorLight, // ToolStripSeparatorLight (ButtonHighlight)
            SystemColors.Control, // ProgressBarBackground (actually not applied with visual styles, default theming)
            SystemColors.Highlight, // ProgressBar (actually not applied with visual styles, default theming)
        ];

        private static readonly Color[] darkThemeColors =
        [
            // Explorer / Gray Window / Gray Highlight / Context menu-like gray ToolStrip menu items
            Color.FromArgb((unchecked((int)0xFF383838))), // Control
            Color.FromArgb((unchecked((int)0xFFFFFFFF))), // ControlText
            Color.FromArgb((unchecked((int)0xFF797979))), // ControlTextDisabled // ToolStrip menu item text
            Color.FromArgb((unchecked((int)0xFF101010))), // ControlHighlight (.NET 9 ButtonHighlight) // e.g. highlight text on a control or a ToolStripOverflowButton
            Color.FromArgb((unchecked((int)0xFF383838))), // Window
            Color.FromArgb((unchecked((int)0xFFFFFFFF))), // WindowText
            Color.FromArgb((unchecked((int)0xFF6D6D6D))), // WindowTextDisabled // e.g. disabled TextBox
            Color.FromArgb((unchecked((int)0xFF191919))), // WindowAlternate // e.g. in DataGridView
            Color.FromArgb((unchecked((int)0xFFFFFFFF))), // WindowTextAlternate // e.g. in DataGridView
            Color.FromArgb((unchecked((int)0xFF505050))), // Highlight
            Color.FromArgb((unchecked((int)0xFFDEDEDE))), // HighlightText
            Color.FromArgb((unchecked((int)0xFFFFFFFF))), // GroupBoxText
            Color.FromArgb((unchecked((int)0xFF636363))), // GridLine (in Explorer, only the headers have visible grid line)
            Color.FromArgb((unchecked((int)0xFF3C3C3C))), // Workspace (custom, there is no such color in Explorer)
            Color.FromArgb((unchecked((int)0xFF2B2B2B))), // ToolTip
            Color.FromArgb((unchecked((int)0xFFFFFFFF))), // ToolTipText
            Color.FromArgb((unchecked((int)0xFF767676))), // ToolTipBorder
            Color.FromArgb((unchecked((int)0xFF202020))), // ToolStripGradientBegin
            Color.FromArgb((unchecked((int)0xFF202020))), // ToolStripGradientMiddle
            Color.FromArgb((unchecked((int)0xFF202020))), // ToolStripGradientEnd
            Color.FromArgb((unchecked((int)0xFF202020))), // ToolStripBorderBottom
            Color.FromArgb((unchecked((int)0xFF2C2C2C))), // ToolStripDropDownBackground
            Color.FromArgb((unchecked((int)0xFF4D4D4D))), // ToolStripButtonSelectedHighlight
            Color.FromArgb((unchecked((int)0xFF666666))), // ToolStripButtonPressedHighlight
            Color.FromArgb((unchecked((int)0xFF4D4D4D))), // ToolStripButtonCheckedHighlight
            Color.FromArgb((unchecked((int)0xFF636363))), // ToolStripButtonSelectedBorder (GridLine)
            Color.FromArgb((unchecked((int)0xFFC3C3C3))), // ToolStripButtonPressedBorder
            Color.FromArgb((unchecked((int)0xFFC3C3C3))), // ToolStripButtonCheckedBorder
            Color.FromArgb((unchecked((int)0xFF2C2C2C))), // ToolStripMenuItemSelectedBorder
            Color.FromArgb((unchecked((int)0xFF2C2C2C))), // ToolStripMenuItemOpenedBorder
            Color.FromArgb((unchecked((int)0xFF2C2C2C))), // ToolStripMenuItemDisabledBorder
            Color.FromArgb((unchecked((int)0xFF353535))), // ToolStripMenuItemDisabledBackground
            Color.FromArgb((unchecked((int)0xFF3E3E3E))), // ToolStripMenuBorder
            Color.FromArgb((unchecked((int)0xFF353535))), // ToolStripMenuItemSelectedGradientBegin
            Color.FromArgb((unchecked((int)0xFF353535))), // ToolStripMenuItemSelectedGradientEnd
            Color.FromArgb((unchecked((int)0xFF202020))), // ToolStripMenuItemPressedGradientBegin
            Color.FromArgb((unchecked((int)0xFF202020))), // ToolStripMenuItemPressedGradientEnd
            Color.FromArgb((unchecked((int)0xFF353535))), // ToolStripMenuItemOpenedGradientBegin
            Color.FromArgb((unchecked((int)0xFF353535))), // ToolStripMenuItemOpenedGradientEnd
            Color.FromArgb((unchecked((int)0xFF4D4D4D))), // ToolStripButtonSelectedGradientBegin
            Color.FromArgb((unchecked((int)0xFF4D4D4D))), // ToolStripButtonSelectedGradientMiddle
            Color.FromArgb((unchecked((int)0xFF4D4D4D))), // ToolStripButtonSelectedGradientEnd
            Color.FromArgb((unchecked((int)0xFF666666))), // ToolStripButtonPressedGradientBegin
            Color.FromArgb((unchecked((int)0xFF666666))), // ToolStripButtonPressedGradientMiddle
            Color.FromArgb((unchecked((int)0xFF666666))), // ToolStripButtonPressedGradientEnd
            Color.FromArgb((unchecked((int)0xFF4D4D4D))), // ToolStripButtonCheckedGradientBegin
            Color.FromArgb((unchecked((int)0xFF4D4D4D))), // ToolStripButtonCheckedGradientEnd
            Color.FromArgb((unchecked((int)0xFF202020))), // ToolStripOverflowButtonGradientBegin
            Color.FromArgb((unchecked((int)0xFF202020))), // ToolStripOverflowButtonGradientMiddle
            Color.FromArgb((unchecked((int)0xFF202020))), // ToolStripOverflowButtonGradientEnd
            Color.FromArgb((unchecked((int)0xFF2C2C2C))), // ToolStripImageMarginGradientBegin
            Color.FromArgb((unchecked((int)0xFF2C2C2C))), // ToolStripImageMarginGradientMiddle
            Color.FromArgb((unchecked((int)0xFF2C2C2C))), // ToolStripImageMarginGradientEnd
            Color.FromArgb((unchecked((int)0xFF383838))), // ToolStripGripDark
            Color.FromArgb((unchecked((int)0xFF2C2C2C))), // ToolStripGripLight
            Color.FromArgb((unchecked((int)0xFF3E3E3E))), // ToolStripSeparatorDark
            Color.FromArgb((unchecked((int)0xFF2C2C2C))), // ToolStripSeparatorLight
            Color.FromArgb((unchecked((int)0xFF707070))), // ProgressBarBackground
            Color.FromArgb((unchecked((int)0xFF7160E8))), // ProgressBar

            //// Explorer / Gray Window / TextBox highlight / Files ListView-like dark ToolStrip menu items
            //Color.FromArgb((unchecked((int)0xFF383838))), // Control
            //Color.FromArgb((unchecked((int)0xFFFFFFFF))), // ControlText
            //Color.FromArgb((unchecked((int)0xFF6D6D6D))), // ControlTextDisabled // now the same as WindowTextDisabled, though this differs from e.g. disabled CheckBox with FlatStyle.System
            //Color.FromArgb((unchecked((int)0xFF101010))), // ControlHighlight (.NET 9 ButtonHighlight) // e.g. highlight text on a control or a ToolStripOverflowButton
            //Color.FromArgb((unchecked((int)0xFF383838))), // Window
            //Color.FromArgb((unchecked((int)0xFFFFFFFF))), // WindowText
            //Color.FromArgb((unchecked((int)0xFF6D6D6D))), // WindowTextDisabled // e.g. disabled TextBox
            //Color.FromArgb((unchecked((int)0xFF464646))), // WindowAlternate (.NET 9 ButtonShadow) // e.g. in DataGridView
            //Color.FromArgb((unchecked((int)0xFFFFFFFF))), // WindowTextAlternate // e.g. in DataGridView
            //Color.FromArgb((unchecked((int)0xFF0078D7))), // Highlight
            //Color.FromArgb((unchecked((int)0xFFFFFFFF))), // HighlightText
            //Color.FromArgb((unchecked((int)0xFFFFFFFF))), // GroupBoxText
            //Color.FromArgb((unchecked((int)0xFF636363))), // GridLine (in Explorer, only the headers have visible grid line)
            //Color.FromArgb((unchecked((int)0xFF3C3C3C))), // Workspace (custom, there is no such color in Explorer)
            //Color.FromArgb((unchecked((int)0xFF2B2B2B))), // ToolTip
            //Color.FromArgb((unchecked((int)0xFFFFFFFF))), // ToolTipText
            //Color.FromArgb((unchecked((int)0xFF767676))), // ToolTipBorder
            //Color.FromArgb((unchecked((int)0xFF202020))), // ToolStripGradientBegin
            //Color.FromArgb((unchecked((int)0xFF202020))), // ToolStripGradientMiddle
            //Color.FromArgb((unchecked((int)0xFF202020))), // ToolStripGradientEnd
            //Color.FromArgb((unchecked((int)0xFF202020))), // ToolStripBorderBottom
            //Color.FromArgb((unchecked((int)0xFF191919))), // ToolStripDropDownBackground
            //Color.FromArgb((unchecked((int)0xFF4D4D4D))), // ToolStripButtonSelectedHighlight
            //Color.FromArgb((unchecked((int)0xFF666666))), // ToolStripButtonPressedHighlight
            //Color.FromArgb((unchecked((int)0xFF4D4D4D))), // ToolStripButtonCheckedHighlight (ToolStripButtonSelectedHighlight)
            //Color.FromArgb((unchecked((int)0xFF636363))), // ToolStripButtonSelectedBorder
            //Color.FromArgb((unchecked((int)0xFFC3C3C3))), // ToolStripButtonPressedBorder
            //Color.FromArgb((unchecked((int)0xFFC3C3C3))), // ToolStripButtonCheckedBorder
            //Color.FromArgb((unchecked((int)0xFF505050))), // ToolStripMenuItemSelectedBorder
            //Color.FromArgb((unchecked((int)0xFFC3C3C3))), // ToolStripMenuItemOpenedBorder
            //Color.FromArgb((unchecked((int)0xFF505050))), // ToolStripMenuItemDisabledBorder
            //Color.Empty, // ToolStripMenuItemDisabledBackground
            //Color.FromArgb((unchecked((int)0xFFC3C3C3))), // ToolStripMenuBorder
            //Color.FromArgb((unchecked((int)0xFF505050))), // ToolStripMenuItemSelectedGradientBegin
            //Color.FromArgb((unchecked((int)0xFF505050))), // ToolStripMenuItemSelectedGradientEnd
            //Color.FromArgb((unchecked((int)0xFF202020))), // ToolStripMenuItemPressedGradientBegin
            //Color.FromArgb((unchecked((int)0xFF202020))), // ToolStripMenuItemPressedGradientEnd
            //Color.FromArgb((unchecked((int)0xFF505050))), // ToolStripMenuItemOpenedGradientBegin
            //Color.FromArgb((unchecked((int)0xFF505050))), // ToolStripMenuItemOpenedGradientEnd
            //Color.FromArgb((unchecked((int)0xFF4D4D4D))), // ToolStripButtonSelectedGradientBegin
            //Color.FromArgb((unchecked((int)0xFF4D4D4D))), // ToolStripButtonSelectedGradientMiddle
            //Color.FromArgb((unchecked((int)0xFF4D4D4D))), // ToolStripButtonSelectedGradientEnd
            //Color.FromArgb((unchecked((int)0xFF666666))), // ToolStripButtonPressedGradientBegin
            //Color.FromArgb((unchecked((int)0xFF666666))), // ToolStripButtonPressedGradientMiddle
            //Color.FromArgb((unchecked((int)0xFF666666))), // ToolStripButtonPressedGradientEnd
            //Color.FromArgb((unchecked((int)0xFF4D4D4D))), // ToolStripButtonCheckedGradientBegin
            //Color.FromArgb((unchecked((int)0xFF4D4D4D))), // ToolStripButtonCheckedGradientEnd
            //Color.FromArgb((unchecked((int)0xFF202020))), // ToolStripOverflowButtonGradientBegin
            //Color.FromArgb((unchecked((int)0xFF202020))), // ToolStripOverflowButtonGradientMiddle
            //Color.FromArgb((unchecked((int)0xFF202020))), // ToolStripOverflowButtonGradientEnd
            //Color.FromArgb((unchecked((int)0xFF191919))), // ToolStripImageMarginGradientBegin
            //Color.FromArgb((unchecked((int)0xFF191919))), // ToolStripImageMarginGradientMiddle
            //Color.FromArgb((unchecked((int)0xFF191919))), // ToolStripImageMarginGradientEnd
            //Color.FromArgb((unchecked((int)0xFF383838))), // ToolStripGripDark
            //Color.FromArgb((unchecked((int)0xFF2C2C2C))), // ToolStripGripLight
            //Color.FromArgb((unchecked((int)0xFF383838))), // ToolStripSeparatorDark
            //Color.FromArgb((unchecked((int)0xFF2C2C2C))), // ToolStripSeparatorLight
            //Color.FromArgb((unchecked((int)0xFF707070))), // ProgressBarBackground
            //Color.FromArgb((unchecked((int)0xFF7160E8))), // ProgressBar

            //// .NET 9's dark theme by its AlternateSystemColors
            //Color.FromArgb((unchecked((int)0xFF202020))), // Control
            //Color.FromArgb((unchecked((int)0xFFFFFFFF))), // ControlText
            //Color.FromArgb((unchecked((int)0xFF969696))), // ControlTextDisabled (GrayText) // note though that a disabled CheckBox with FlatStyle.System is 0xFFCCCCCC, for example
            //Color.FromArgb((unchecked((int)0xFF101010))), // ControlHighlight (ButtonHighlight) // e.g. highlight text on a control or a ToolStripOverflowButton
            //Color.FromArgb((unchecked((int)0xFF323232))), // Window
            //Color.FromArgb((unchecked((int)0xFFF0F0F0))), // WindowText
            //Color.FromArgb((unchecked((int)0xFF969696))), // WindowTextDisabled (GrayText) // note though a disabled TextBox is 0xFF6D6D6D, for example
            //Color.FromArgb((unchecked((int)0xFF464646))), // WindowAlternate (ButtonShadow) // e.g. in DataGridView
            //Color.FromArgb((unchecked((int)0xFFFFFFFF))), // WindowTextAlternate // e.g. in DataGridView
            //Color.FromArgb((unchecked((int)0xFF2864B4))), // Highlight
            //Color.FromArgb((unchecked((int)0xFF000000))), // HighlightText
            //Color.FromArgb((unchecked((int)0xFFFFFFFF))), // GroupBoxText
            //Color.FromArgb((unchecked((int)0xFF282828))), // GridLine (WindowFrame)
            //Color.FromArgb((unchecked((int)0xFF3C3C3C))), // Workspace (AppWorkspace)
            //Color.FromArgb((unchecked((int)0xFF50503C))), // ToolTip (Info)
            //Color.FromArgb((unchecked((int)0xFFBEBEBE))), // ToolTipText (InfoText)
            //Color.FromArgb((unchecked((int)0xFF282828))), // ToolTipBorder (WindowFrame)
            //Color.FromArgb((unchecked((int)0xFF2E2E2E))), // ToolStripGradientBegin (GetAlphaBlendedColorHighRes(null, buttonFace, window, 23))
            //Color.FromArgb((unchecked((int)0xFF292929))), // ToolStripGradientMiddle (GetAlphaBlendedColorHighRes(null, buttonFace, window, 50))
            //Color.FromArgb((unchecked((int)0xFF202020))), // ToolStripGradientEnd (Control)
            //Color.FromArgb((unchecked((int)0xFF232323))), // ToolStripBorderBottom (GetAlphaBlendedColorHighRes(null, window, buttonFace, 165))
            //Color.FromArgb((unchecked((int)0xFF2F2F2F))), // ToolStripDropDownBackground (GetAlphaBlendedColorHighRes(null, buttonFace, window, 143))
            //Color.FromArgb((unchecked((int)0xFF2E3F56))), // ToolStripButtonSelectedHighlight (GetAlphaBlendedColor(screen, SystemColors.Window, GetAlphaBlendedColor(screen, SystemColors.Highlight, SystemColors.Window, 80), 20))
            //Color.FromArgb((unchecked((int)0xFF2C4A73))), // ToolStripButtonPressedHighlight (GetAlphaBlendedColor(screen, SystemColors.Window, GetAlphaBlendedColor(screen, SystemColors.Highlight, SystemColors.Window, 160), 50))
            //Color.FromArgb((unchecked((int)0xFF2E3F56))), // ToolStripButtonCheckedHighlight (ToolStripButtonSelectedHighlight)
            //Color.FromArgb((unchecked((int)0xFF2864B4))), // ToolStripButtonSelectedBorder (Highlight)
            //Color.FromArgb((unchecked((int)0xFF2864B4))), // ToolStripButtonPressedBorder (Highlight)
            //Color.FromArgb((unchecked((int)0xFF2864B4))), // ToolStripButtonCheckedBorder (Highlight)
            //Color.FromArgb((unchecked((int)0xFF2864B4))), // ToolStripMenuItemSelectedBorder (Highlight)
            //Color.FromArgb((unchecked((int)0xFF2864B4))), // ToolStripMenuItemOpenedBorder (Highlight)
            //Color.FromArgb((unchecked((int)0xFF2864B4))), // ToolStripMenuItemDisabledBorder (Highlight)
            //Color.Empty, //ToolStripMenuItemDisabledBackground
            //Color.FromArgb((unchecked((int)0xFF6B6B6B))), // ToolStripMenuBorder (GetAlphaBlendedColorHighRes(null, controlText, buttonShadow, 20))
            //Color.FromArgb((unchecked((int)0xFF2F4159))), // ToolStripMenuItemSelectedGradientBegin (GetAlphaBlendedColorHighRes(null, highlight, window, 30))
            //Color.FromArgb((unchecked((int)0xFF2F4159))), // ToolStripMenuItemSelectedGradientEnd (ToolStripMenuItemSelectedGradientBegin)
            //Color.FromArgb((unchecked((int)0xFF2E2E2E))), // ToolStripMenuItemPressedGradientBegin (GetAlphaBlendedColorHighRes(null, buttonFace, window, 23))
            //Color.FromArgb((unchecked((int)0xFF292929))), // ToolStripMenuItemPressedGradientEnd (GetAlphaBlendedColorHighRes(null, buttonFace, window, 50))
            //Color.FromArgb((unchecked((int)0xFF2F4159))), // ToolStripMenuItemOpenedGradientBegin (ToolStripMenuItemSelectedGradientBegin)
            //Color.FromArgb((unchecked((int)0xFF2F4159))), // ToolStripMenuItemOpenedGradientEnd (ToolStripMenuItemSelectedGradientBegin)
            //Color.FromArgb((unchecked((int)0xFF2F4159))), // ToolStripButtonSelectedGradientBegin (ToolStripMenuItemSelectedGradientBegin)
            //Color.FromArgb((unchecked((int)0xFF2F4159))), // ToolStripButtonSelectedGradientMiddle (ToolStripMenuItemSelectedGradientBegin)
            //Color.FromArgb((unchecked((int)0xFF2F4159))), // ToolStripButtonSelectedGradientEnd (ToolStripMenuItemSelectedGradientBegin)
            //Color.FromArgb((unchecked((int)0xFF2D4B73))), // ToolStripButtonPressedGradientBegin (GetAlphaBlendedColorHighRes(null, highlight, window, 50))
            //Color.FromArgb((unchecked((int)0xFF2D4B73))), // ToolStripButtonPressedGradientMiddle (ToolStripButtonPressedGradientBegin)
            //Color.FromArgb((unchecked((int)0xFF2D4B73))), // ToolStripButtonPressedGradientEnd (ToolStripButtonPressedGradientBegin)
            //Color.FromArgb((unchecked((int)0xFF2E3F56))), // ToolStripButtonCheckedGradientBegin (ToolStripButtonCheckedHighlight)
            //Color.FromArgb((unchecked((int)0xFF2E3F56))), // ToolStripButtonCheckedGradientEnd (ToolStripButtonCheckedHighlight)
            //Color.FromArgb((unchecked((int)0xFF252525))), // ToolStripOverflowButtonGradientBegin (GetAlphaBlendedColorHighRes(null, buttonFace, window, 70))
            //Color.FromArgb((unchecked((int)0xFF222222))), // ToolStripOverflowButtonGradientMiddle (GetAlphaBlendedColorHighRes(null, buttonFace, window, 90))
            //Color.FromArgb((unchecked((int)0xFF464646))), // ToolStripOverflowButtonGradientEnd (ButtonShadow)
            //Color.FromArgb((unchecked((int)0xFF2E2E2E))), // ToolStripImageMarginGradientBegin (GetAlphaBlendedColorHighRes(null, buttonFace, window, 23))
            //Color.FromArgb((unchecked((int)0xFF292929))), // ToolStripImageMarginGradientMiddle (GetAlphaBlendedColorHighRes(null, buttonFace, window, 50))
            //Color.FromArgb((unchecked((int)0xFF202020))), // ToolStripImageMarginGradientEnd (Control)
            //Color.FromArgb((unchecked((int)0xFF414141))), // ToolStripGripDark (GetAlphaBlendedColorHighRes(null, buttonShadow, window, 75))
            //Color.FromArgb((unchecked((int)0xFF323232))), // ToolStripGripLight (Window)
            //Color.FromArgb((unchecked((int)0xFF404040))), // ToolStripSeparatorDark (GetAlphaBlendedColorHighRes(null, buttonShadow, window, 70))
            //Color.FromArgb((unchecked((int)0xFF101010))), // ToolStripSeparatorLight (ButtonHighlight)
            //Color.FromArgb((unchecked((int)0xFF707070))), // ProgressBarBackground
            //Color.FromArgb((unchecked((int)0xFF7160E8))), // ProgressBar
        ];

        private static volatile bool isDarkBaseTheme;
        private static volatile bool isBaseThemeEverChanged;
        private static volatile bool isCustomThemeEverChanged;
        private static volatile bool useVisualStyles;
        private static volatile bool isHighContrast;
        private static bool? isDarkSystemTheme;
        private static DefaultTheme currentBaseTheme;
        private static Dictionary<ThemeColor, Color>? customColors; // changed to a simple dictionary because it is always replaced with a new one
        private static EventHandler? themeChangedHandler;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the theme has changed.
        /// </summary>
        public static event EventHandler? ThemeChanged
        {
            add => value.AddSafe(ref themeChangedHandler);
            remove => value.RemoveSafe(ref themeChangedHandler);
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets whether theming is enabled. Theming is enabled if high contrast mode is disabled,
        /// and either at least one color is defined in the current theme or the current base theme is dark.
        /// </summary>
        public static bool IsThemingEnabled => (isDarkBaseTheme || customColors is { Count: > 0 }) && !isHighContrast;

        #endregion

        #region Internal Properties

        internal static Color Control => Get(ThemeColor.Control);
        internal static Color ControlText => Get(ThemeColor.ControlText);
        internal static Color ControlTextDisabled => Get(ThemeColor.ControlTextDisabled);
        internal static Color ControlHighlight => Get(ThemeColor.ControlHighlight);
        internal static Color Window => Get(ThemeColor.Window);
        internal static Color WindowText => Get(ThemeColor.WindowText);
        internal static Color WindowTextDisabled => Get(ThemeColor.WindowTextDisabled);
        internal static Color WindowAlternate => Get(ThemeColor.WindowAlternate);
        internal static Color WindowTextAlternate => Get(ThemeColor.WindowTextAlternate);
        internal static Color Highlight => Get(ThemeColor.Highlight);
        internal static Color HighlightText => Get(ThemeColor.HighlightText);
        internal static Color GroupBoxText => Get(ThemeColor.GroupBoxText); // Special handling!
        internal static Color GridLine => Get(ThemeColor.GridLine);
        internal static Color Workspace => Get(ThemeColor.Workspace);
        internal static Color ToolTip => Get(ThemeColor.ToolTip); // Special handling!
        internal static Color ToolTipText => Get(ThemeColor.ToolTipText); // Special handling!
        internal static Color ToolTipBorder => Get(ThemeColor.ToolTipBorder); // Special handling!
        internal static Color ToolStripGradientBegin => Get(ThemeColor.ToolStripGradientBegin);
        internal static Color ToolStripGradientMiddle => Get(ThemeColor.ToolStripGradientMiddle);
        internal static Color ToolStripGradientEnd => Get(ThemeColor.ToolStripGradientEnd);
        internal static Color ToolStripBorderBottom => Get(ThemeColor.ToolStripBorderBottom);
        internal static Color ToolStripDropDownBackground => Get(ThemeColor.ToolStripDropDownBackground);
        internal static Color ToolStripButtonSelectedHighlight => Get(ThemeColor.ToolStripButtonSelectedHighlight);
        internal static Color ToolStripButtonPressedHighlight => Get(ThemeColor.ToolStripButtonPressedHighlight);
        internal static Color ToolStripButtonCheckedHighlight => Get(ThemeColor.ToolStripButtonCheckedHighlight);
        internal static Color ToolStripButtonSelectedBorder => Get(ThemeColor.ToolStripButtonSelectedBorder);
        internal static Color ToolStripButtonPressedBorder => Get(ThemeColor.ToolStripButtonPressedBorder);
        internal static Color ToolStripButtonCheckedBorder => Get(ThemeColor.ToolStripButtonCheckedBorder);
        internal static Color ToolStripMenuItemSelectedBorder => Get(ThemeColor.ToolStripMenuItemSelectedBorder);
        internal static Color ToolStripMenuItemOpenedBorder => Get(ThemeColor.ToolStripMenuItemOpenedBorder);
        internal static Color ToolStripMenuItemDisabledBorder => Get(ThemeColor.ToolStripMenuItemDisabledBorder);
        internal static Color ToolStripMenuItemDisabledBackground => Get(ThemeColor.ToolStripMenuItemDisabledBackground);
        internal static Color ToolStripMenuBorder => Get(ThemeColor.ToolStripMenuBorder);
        internal static Color ToolStripMenuItemSelectedGradientBegin => Get(ThemeColor.ToolStripMenuItemSelectedGradientBegin);
        internal static Color ToolStripMenuItemSelectedGradientEnd => Get(ThemeColor.ToolStripMenuItemSelectedGradientEnd);
        internal static Color ToolStripMenuItemPressedGradientBegin => Get(ThemeColor.ToolStripMenuItemPressedGradientBegin);
        internal static Color ToolStripMenuItemPressedGradientEnd => Get(ThemeColor.ToolStripMenuItemPressedGradientEnd);
        internal static Color ToolStripMenuItemOpenedGradientBegin => Get(ThemeColor.ToolStripMenuItemOpenedGradientBegin);
        internal static Color ToolStripMenuItemOpenedGradientEnd => Get(ThemeColor.ToolStripMenuItemOpenedGradientEnd);
        internal static Color ToolStripButtonSelectedGradientBegin => Get(ThemeColor.ToolStripButtonSelectedGradientBegin);
        internal static Color ToolStripButtonSelectedGradientMiddle => Get(ThemeColor.ToolStripButtonSelectedGradientMiddle);
        internal static Color ToolStripButtonSelectedGradientEnd => Get(ThemeColor.ToolStripButtonSelectedGradientEnd);
        internal static Color ToolStripButtonPressedGradientBegin => Get(ThemeColor.ToolStripButtonPressedGradientBegin);
        internal static Color ToolStripButtonPressedGradientMiddle => Get(ThemeColor.ToolStripButtonPressedGradientMiddle);
        internal static Color ToolStripButtonPressedGradientEnd => Get(ThemeColor.ToolStripButtonPressedGradientEnd);
        internal static Color ToolStripButtonCheckedGradientBegin => Get(ThemeColor.ToolStripButtonCheckedGradientBegin);
        internal static Color ToolStripButtonCheckedGradientEnd => Get(ThemeColor.ToolStripButtonCheckedGradientEnd);
        internal static Color ToolStripOverflowButtonGradientBegin => Get(ThemeColor.ToolStripOverflowButtonGradientBegin);
        internal static Color ToolStripOverflowButtonGradientMiddle => Get(ThemeColor.ToolStripOverflowButtonGradientMiddle);
        internal static Color ToolStripOverflowButtonGradientEnd => Get(ThemeColor.ToolStripOverflowButtonGradientEnd);
        internal static Color ToolStripImageMarginGradientBegin => Get(ThemeColor.ToolStripImageMarginGradientBegin);
        internal static Color ToolStripImageMarginGradientMiddle => Get(ThemeColor.ToolStripImageMarginGradientMiddle);
        internal static Color ToolStripImageMarginGradientEnd => Get(ThemeColor.ToolStripImageMarginGradientEnd);
        internal static Color ToolStripGripDark => Get(ThemeColor.ToolStripGripDark);
        internal static Color ToolStripGripLight => Get(ThemeColor.ToolStripGripLight);
        internal static Color ToolStripSeparatorDark => Get(ThemeColor.ToolStripSeparatorDark);
        internal static Color ToolStripSeparatorLight => Get(ThemeColor.ToolStripSeparatorLight);
        internal static Color ProgressBarBackground => Get(ThemeColor.ProgressBarBackground);
        internal static Color ProgressBar => Get(ThemeColor.ProgressBar);

        internal static ProfessionalColorTable ColorTable { get; } = new ThemeColorTable();

        // These are not configurable theme colors because they match the fix theme of a TextBox/ComboBox that cannot be changed. Used only in dark mode.
        // TODO: is it possible to retrieve them by VisualStyleRenderer (like the GroupBox color)? If so, they can be configurable after all.
        internal static Color FixedSingleBorder => Color.FromArgb(unchecked((int)(0xFFC8C8C8)));
        internal static Color FixedSingleBorderInactive => Color.FromArgb(unchecked((int)(0xFF9B9B9B)));

        internal static bool IsBaseThemeEverChanged => isBaseThemeEverChanged;
        internal static bool IsThemeEverChanged => isBaseThemeEverChanged || isCustomThemeEverChanged;
        internal static bool IsDarkBaseTheme => isDarkBaseTheme;

        #endregion

        #region Private Properties

        private static bool IsDarkSystemTheme
        {
            get
            {
                if (isDarkSystemTheme.HasValue)
                    return isDarkSystemTheme.Value;

                if (!OSUtils.IsWindows10OrLater)
                    return (isDarkSystemTheme = false).Value;

                const string path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";
                const string keyAppsUseLightTheme = "AppsUseLightTheme";
                try
                {
                    using RegistryKey? reg = Registry.CurrentUser.OpenSubKey(path);
                    isDarkSystemTheme = reg?.GetValue(keyAppsUseLightTheme) is int value && Math.Abs(value) == 0;
                }
                catch (Exception e) when (!e.IsCritical())
                {
                    isDarkSystemTheme = false;
                }

                return isDarkSystemTheme.Value;
            }
        }

        // TODO
        //private static Dictionary<ThemeColor, Color> CurrentTheme
        //{
        //    get
        //    {
        //        Dictionary<ThemeColor, Color>? result = customColors;
        //        if (result != null)
        //            return result;

        //        // Note: currentTheme can be set by ResetTheme (even to null), so it can happen that between the previous null check and this one
        //        // the currentTheme is already set. In such cases, we return the already set value.
        //        result = new Dictionary<ThemeColor, Color>();
        //        return Interlocked.CompareExchange(ref customColors, result, null) ?? result;
        //    }
        //}

        #endregion

        #endregion

        #region Constructors

        static ThemeColors()
        {
            try
            {
                useVisualStyles = Application.RenderWithVisualStyles;
                isHighContrast = SystemInformation.HighContrast;
                if (OSUtils.IsWindows)
                    SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
            }
            catch (Exception e) when (e is InvalidOperationException or ExternalException)
            {
            }
        }

        #endregion

        #region Methods

        #region Static Methods

        /// <summary>
        /// Sets the base theme for the application.
        /// </summary>
        /// <param name="theme">The base theme to set. This parameter is optional.
        /// <br/>Default value: <see cref="DefaultTheme.Classic"/>.</param>
        /// <param name="resetCustomColors"><see langword="true"/> to reset custom colors to the default values of the specified theme;
        /// <see langword="false"/> to keep the custom colors unchanged. This parameter is optional.
        /// <br/>Default value: <see langword="true"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void SetBaseTheme(DefaultTheme theme = DefaultTheme.Classic, bool resetCustomColors = true)
        {
            if (!theme.IsDefined())
                throw new ArgumentOutOfRangeException(nameof(theme), theme, PublicResources.EnumOutOfRange(theme));

            if (theme == DefaultTheme.Classic && !isBaseThemeEverChanged && (customColors is null || !resetCustomColors))
                return;

            if (!OSUtils.IsWindows10OrLater)
                return;

            bool isNewThemeDark = (theme == DefaultTheme.Dark || theme == DefaultTheme.System && IsDarkSystemTheme) && Application.RenderWithVisualStyles;
            bool defaultColorsChanged = isNewThemeDark != isDarkBaseTheme;
            currentBaseTheme = theme;

#if NET9_0_OR_GREATER && SYSTEM_THEMING
            if (defaultColorsChanged)
                Application.SetColorMode((SystemColorMode)theme);
#else
            isDarkBaseTheme = isNewThemeDark;
            if (defaultColorsChanged)
                isBaseThemeEverChanged = true; // the |= operator would trigger a warning because isBaseThemeEverChanged is volatile
            if (defaultColorsChanged)
                InitializeBaseTheme(theme);
            if (resetCustomColors)
                DoResetCustomColors(null, defaultColorsChanged);
            else if (defaultColorsChanged)
                OnThemeChanged(EventArgs.Empty);
#endif
        }

        #endregion

        #region Instance Methods

        #region Public Methods

        /// <summary>
        /// Resets the custom colors using the specified dictionary of theme colors.
        /// </summary>
        /// <param name="theme">An optional dictionary containing theme colors and their corresponding custom color values.
        /// If <paramref name="theme"/> is <see langword="null"/>, the default theme colors are reset. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        public static void ResetCustomColors(IDictionary<ThemeColor, Color>? theme = null) => DoResetCustomColors(theme, false);

        #endregion

        #region Internal Methods
        
        internal static bool IsSet(ThemeColor key, bool includingDefaultDarkTheme = true)
            => includingDefaultDarkTheme && isDarkBaseTheme || customColors?.ContainsKey(key) == true;

        internal static Color FromKnownColor(KnownColor color)
        {
            ThemeColor themeColor = color switch
            {
                // TODO: return something for every known system color
                //KnownColor.ActiveBorder => unknownColor,
                //KnownColor.ActiveCaption => unknownColor,
                //KnownColor.ActiveCaptionText => unknownColor,
                KnownColor.AppWorkspace => ThemeColor.Workspace,
                KnownColor.Control => ThemeColor.Control,
                //KnownColor.ControlDark => unknownColor,
                //KnownColor.ControlDarkDark => unknownColor,
                //KnownColor.ControlLight => unknownColor,
                //KnownColor.ControlLightLight => unknownColor,
                KnownColor.ControlText => ThemeColor.ControlText,
                //KnownColor.Desktop => unknownColor,
                KnownColor.GrayText => ThemeColor.ControlTextDisabled,
                KnownColor.Highlight => ThemeColor.Highlight,
                KnownColor.HighlightText => ThemeColor.HighlightText,
                //KnownColor.HotTrack => unknownColor,
                //KnownColor.InactiveBorder => unknownColor,
                //KnownColor.InactiveCaption => unknownColor,
                //KnownColor.InactiveCaptionText => unknownColor,
                KnownColor.Info => ThemeColor.ToolTip,
                KnownColor.InfoText => ThemeColor.ToolTipText,
                //KnownColor.Menu => unknownColor,
                //KnownColor.MenuText => unknownColor,
                //KnownColor.ScrollBar => unknownColor,
                KnownColor.Window => ThemeColor.Window,
                KnownColor.WindowFrame => ThemeColor.GridLine,
                KnownColor.WindowText => ThemeColor.WindowText,
                //KnownColor.ButtonFace => unknownColor,
                KnownColor.ButtonHighlight => ThemeColor.ControlHighlight,
                //KnownColor.ButtonShadow => unknownColor,
                //KnownColor.GradientActiveCaption => unknownColor,
                //KnownColor.GradientInactiveCaption => unknownColor,
                //KnownColor.MenuBar => unknownColor,
                //KnownColor.MenuHighlight => unknownColor,
                _ => unknownColor
            };

            return themeColor == unknownColor
                ? Color.FromKnownColor(color)
                : Get(themeColor);
        }

        #endregion


        #region Private Methods

        private static void InitializeBaseTheme(DefaultTheme theme)
        {
            // Context menus of the current process
            try
            {
                UxTheme.SetPreferredAppMode(theme);
                UxTheme.FlushMenuThemes();
            }
            catch (Exception e) when (!e.IsCritical())
            {
            }
        } 

        private static void DoResetCustomColors(IDictionary<ThemeColor, Color>? theme, bool defaultColorsChanged)
        {
            if (theme is null || theme.Count == 0)
            {
                if (Interlocked.Exchange(ref customColors, null) != null || defaultColorsChanged)
                {
                    isCustomThemeEverChanged = true;
                    OnThemeChanged(EventArgs.Empty);
                }

                return;
            }

            isCustomThemeEverChanged = true;
            Interlocked.Exchange(ref customColors, new Dictionary<ThemeColor, Color>(theme));
            OnThemeChanged(EventArgs.Empty);
        }

        private static Color Get(ThemeColor key)
        {
            if (customColors?.TryGetValue(key, out Color result) == true)
                return result;

            Debug.Assert(key.IsDefined() && (int)key < defaultThemeColors.Length && (int)key < darkThemeColors.Length);

            // Special handling for some cases that may be different when visual styles are enabled
            if (!isDarkBaseTheme && useVisualStyles && !isHighContrast)
            {
                switch (key)
                {
                    case ThemeColor.GroupBoxText: // may be different on Windows XP
                        return new VisualStyleRenderer(VisualStyleElement.Button.GroupBox.Normal).GetColor(ColorProperty.TextColor);
                    case ThemeColor.ToolTip:
                        if (OSUtils.IsWindows11OrLater)
                            return Color.FromArgb((unchecked((int)0xFFF9F9F9)));
                        if (OSUtils.IsWindows10OrLater)
                            return Color.FromArgb((unchecked((int)0xFFFFFFFF)));
                        if (OSUtils.IsVistaOrLater)
                            return Color.FromArgb((unchecked((int)0xFFF3F4F8))); // actually a gradient from white to 0xE4E5F0, this is the middle
                        break;
                    case ThemeColor.ToolTipText:
                        if (OSUtils.IsVistaOrLater)
                            return Color.FromArgb((unchecked((int)0xFF575757)));
                        break;
                    case ThemeColor.ToolTipBorder:
                        if (OSUtils.IsWindows11OrLater)
                            return Color.FromArgb((unchecked((int)0xFFE5E5E5)));
                        if (OSUtils.IsVistaOrLater)
                            return Color.FromArgb((unchecked((int)0xFF767676)));
                        break;
                }
            }

            return isDarkBaseTheme ? darkThemeColors[(int)key] : defaultThemeColors[(int)key];
        }

        private static void OnThemeChanged(EventArgs e)
        {
            ColorExtensions.ClearCaches();
            themeChangedHandler?.Invoke(null, e);
        }

        #endregion

        #region Event Handlers

        private static void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.General)
            {
                useVisualStyles = Application.RenderWithVisualStyles;
                isHighContrast = SystemInformation.HighContrast;
                isDarkSystemTheme = null;
                if (currentBaseTheme == DefaultTheme.System)
                    SetBaseTheme(DefaultTheme.System, false);
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
