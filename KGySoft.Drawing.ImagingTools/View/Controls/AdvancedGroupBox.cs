#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdvancedGroupBox.cs
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

using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    /// <summary>
    /// A GroupBox that fixes the rendering in dark mode (see also ControlExtensions). Issue: with System FlatStyle text is always too dark, whereas with Standard FlatStyle just the disabled text is too dark.
    /// </summary>
    internal class AdvancedGroupBox : GroupBox
    {
        #region Methods

        protected override void OnPaint(PaintEventArgs e)
        {
            // Customizing only the disabled text color in dark mode. Not including the FlatStyle check here, because System FlatStyle is drawn in WM_PAINT
            if (!ThemeColors.IsDarkBaseTheme || Enabled || !Application.RenderWithVisualStyles || Width < 10 || Height < 10)
            {
                base.OnPaint(e);
                return;
            }

            // In dark mode the disable text is too dark, so we need to adjust it
            TextFormatFlags textFlags = TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak
                | TextFormatFlags.PreserveGraphicsTranslateTransform | TextFormatFlags.PreserveGraphicsClipping;
            if (!ShowKeyboardCues)
                textFlags |= TextFormatFlags.HidePrefix;
            if (RightToLeft == RightToLeft.Yes)
                textFlags |= (TextFormatFlags.Right | TextFormatFlags.RightToLeft);
            GroupBoxRenderer.DrawGroupBox(e.Graphics, new Rectangle(0, 0, Width, Height), Text, Font, ThemeColors.ControlTextDisabled, textFlags, GroupBoxState.Disabled);
        }

        #endregion
    }
}
