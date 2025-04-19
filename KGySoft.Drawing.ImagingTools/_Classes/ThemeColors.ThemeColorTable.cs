#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ThemeColors.ThemeColorTable.cs
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

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    public static partial class ThemeColors
    {
        #region ColorTable class

        [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass", Justification = "Intended")]
        private sealed class ThemeColorTable : ProfessionalColorTable
        {
            #region Properties

            public override Color ButtonSelectedHighlight => ToolStripButtonSelectedHighlight; // FromKnownColor(KnownColors.ButtonSelectedHighlight);
            //public override Color ButtonSelectedHighlightBorder => Color.Magenta; // ButtonPressedBorder; - not used
            public override Color ButtonPressedHighlight => ToolStripButtonPressedHighlight; // FromKnownColor(KnownColors.ButtonPressedHighlight);
            //public override Color ButtonPressedHighlightBorder => Color.Magenta; // SystemColors.Highlight; - not used
            public override Color ButtonCheckedHighlight => ToolStripButtonCheckedHighlight; // FromKnownColor(KnownColors.ButtonCheckedHighlight);
            //public override Color ButtonCheckedHighlightBorder => Color.Magenta; // SystemColors.Highlight; - not used
            //public override Color ButtonPressedBorder => Color.Magenta; // FromKnownColor(KnownColors.msocbvcrCBCtlBdrMouseOver); - not used
            public override Color ButtonSelectedBorder => ToolStripButtonSelectedBorder; // FromKnownColor(KnownColors.msocbvcrCBCtlBdrMouseOver);
            public override Color ButtonCheckedGradientBegin => ToolStripButtonCheckedGradientBegin; // FromKnownColor(KnownColors.msocbvcrCBGradSelectedBegin);
            //public override Color ButtonCheckedGradientMiddle => Color.Magenta; // FromKnownColor(KnownColors.msocbvcrCBGradSelectedMiddle); - not used
            public override Color ButtonCheckedGradientEnd => ToolStripButtonCheckedGradientEnd; // FromKnownColor(KnownColors.msocbvcrCBGradSelectedEnd);
            public override Color ButtonSelectedGradientBegin => ToolStripButtonSelectedGradientBegin; // FromKnownColor(KnownColors.msocbvcrCBGradMouseOverBegin);
            public override Color ButtonSelectedGradientMiddle => ToolStripButtonSelectedGradientMiddle; // FromKnownColor(KnownColors.msocbvcrCBGradMouseOverMiddle);
            public override Color ButtonSelectedGradientEnd => ToolStripButtonSelectedGradientEnd; // FromKnownColor(KnownColors.msocbvcrCBGradMouseOverEnd);
            public override Color ButtonPressedGradientBegin => ToolStripButtonPressedGradientBegin; // FromKnownColor(KnownColors.msocbvcrCBGradMouseDownBegin);
            public override Color ButtonPressedGradientMiddle => ToolStripButtonPressedGradientMiddle; // FromKnownColor(KnownColors.msocbvcrCBGradMouseDownMiddle);
            public override Color ButtonPressedGradientEnd => ToolStripButtonPressedGradientEnd; // FromKnownColor(KnownColors.msocbvcrCBGradMouseDownEnd);
            //public override Color CheckBackground => Color.Magenta; // FromKnownColor(KnownColors.msocbvcrCBCtlBkgdSelected); - not used by AdvancedToolStrip (would be used in OnRenderItemCheck with non-system color table or with disabled visual styles)
            //public override Color CheckSelectedBackground => Color.Magenta; // FromKnownColor(KnownColors.msocbvcrCBCtlBkgdSelectedMouseOver); - not used by AdvancedToolStrip (would be used in OnRenderItemCheck with non-system color table or with disabled visual styles)
            //public override Color CheckPressedBackground => Color.Magenta; // FromKnownColor(KnownColors.msocbvcrCBCtlBkgdSelectedMouseOver); - not used by AdvancedToolStrip (would be used in OnRenderItemCheck with non-system color table or with disabled visual styles)
            public override Color GripDark => ToolStripGripDark; // FromKnownColor(KnownColors.msocbvcrCBDragHandle);
            public override Color GripLight => ToolStripGripLight; // FromKnownColor(KnownColors.msocbvcrCBDragHandleShadow);
            public override Color ImageMarginGradientBegin => ToolStripImageMarginGradientBegin; // FromKnownColor(KnownColors.msocbvcrCBGradVertBegin);
            public override Color ImageMarginGradientMiddle => ToolStripImageMarginGradientMiddle; // FromKnownColor(KnownColors.msocbvcrCBGradVertMiddle);
            public override Color ImageMarginGradientEnd => ToolStripImageMarginGradientEnd; // (_usingSystemColors) ? SystemColors.Control : FromKnownColor(KnownColors.msocbvcrCBGradVertEnd);
            //public override Color ImageMarginRevealedGradientBegin => Color.Magenta; // FromKnownColor(KnownColors.msocbvcrCBGradMenuIconBkgdDroppedBegin); - not used
            //public override Color ImageMarginRevealedGradientMiddle => Color.Magenta; // FromKnownColor(KnownColors.msocbvcrCBGradMenuIconBkgdDroppedMiddle); - not used
            //public override Color ImageMarginRevealedGradientEnd => Color.Magenta; // FromKnownColor(KnownColors.msocbvcrCBGradMenuIconBkgdDroppedEnd); - not used
            //public override Color MenuStripGradientBegin => Color.Magenta; // FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzBegin); // Used in MenuStrip only
            //public override Color MenuStripGradientEnd => Color.Magenta; // FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzEnd); // Used in MenuStrip only
            //public override Color MenuItemSelected => Color.Magenta; // FromKnownColor(KnownColors.msocbvcrCBCtlBkgdMouseOver); // Used only by .NET Framework in the base OnRenderMenuItemBackground. Core and the AdvancedToolStrip uses MenuItemSelectedGradientBegin/End instead
            public override Color MenuItemBorder => ToolStripMenuItemBorder; // FromKnownColor(KnownColors.msocbvcrCBCtlBdrSelected);
            public override Color MenuBorder => ToolStripMenuBorder; // FromKnownColor(KnownColors.msocbvcrCBMenuBdrOuter);
            public override Color MenuItemSelectedGradientBegin => ToolStripMenuItemSelectedGradientBegin; // FromKnownColor(KnownColors.msocbvcrCBGradMouseOverBegin);
            public override Color MenuItemSelectedGradientEnd => ToolStripMenuItemSelectedGradientEnd; // FromKnownColor(KnownColors.msocbvcrCBGradMouseOverEnd);
            public override Color MenuItemPressedGradientBegin => ToolStripMenuItemPressedGradientBegin; // FromKnownColor(KnownColors.msocbvcrCBGradMenuTitleBkgdBegin);
            //public override Color MenuItemPressedGradientMiddle => Color.Magenta; // FromKnownColor(KnownColors.msocbvcrCBGradMenuIconBkgdDroppedMiddle); - not used
            public override Color MenuItemPressedGradientEnd => ToolStripMenuItemPressedGradientEnd; // FromKnownColor(KnownColors.msocbvcrCBGradMenuTitleBkgdEnd);
            //public override Color RaftingContainerGradientBegin => Color.Magenta; // FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzBegin); - not used
            //public override Color RaftingContainerGradientEnd => Color.Magenta; // FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzEnd); - not used
            public override Color SeparatorDark => ToolStripSeparatorDark; // FromKnownColor(KnownColors.msocbvcrCBSplitterLine);
            public override Color SeparatorLight => ToolStripSeparatorLight; // FromKnownColor(KnownColors.msocbvcrCBSplitterLineLight);
#if NET6_0_OR_GREATER
            public override Color StatusStripBorder => Color.Magenta; // SystemColors.ButtonHighlight; 
#endif
            public override Color StatusStripGradientBegin => Color.Magenta; // FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzBegin); - used for status strip
            public override Color StatusStripGradientEnd => Color.Magenta; // FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzEnd);
            public override Color ToolStripBorder => ToolStripBorderBottom; // FromKnownColor(KnownColors.msocbvcrCBShadow);
            public override Color ToolStripDropDownBackground => ThemeColors.ToolStripDropDownBackground; // FromKnownColor(KnownColors.msocbvcrCBMenuBkgd);
            public override Color ToolStripGradientBegin => ThemeColors.ToolStripGradientBegin; // FromKnownColor(KnownColors.msocbvcrCBGradVertBegin);
            public override Color ToolStripGradientMiddle => ThemeColors.ToolStripGradientMiddle; // FromKnownColor(KnownColors.msocbvcrCBGradVertMiddle);
            public override Color ToolStripGradientEnd => ThemeColors.ToolStripGradientEnd; // FromKnownColor(KnownColors.msocbvcrCBGradVertEnd); // ButtonFace
            //public override Color ToolStripContentPanelGradientBegin => Color.Magenta; // FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzBegin); - not used
            //public override Color ToolStripContentPanelGradientEnd => Color.Magenta; // FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzEnd); - used for ToolStripContentPanel only
            //public override Color ToolStripPanelGradientBegin => Color.Magenta; // FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzBegin); - used for ToolStripPanel only
            //public override Color ToolStripPanelGradientEnd => Color.Magenta; // FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzEnd); - used for ToolStripPanel only
            public override Color OverflowButtonGradientBegin => ToolStripOverflowButtonGradientBegin; // FromKnownColor(KnownColors.msocbvcrCBGradOptionsBegin);
            public override Color OverflowButtonGradientMiddle => ToolStripOverflowButtonGradientMiddle; // FromKnownColor(KnownColors.msocbvcrCBGradOptionsMiddle);
            public override Color OverflowButtonGradientEnd => ToolStripOverflowButtonGradientEnd; // FromKnownColor(KnownColors.msocbvcrCBGradOptionsEnd);

            #endregion
        }

        #endregion
    }
}