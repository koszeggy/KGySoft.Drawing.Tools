#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DrawToolTipEventArgsExtensions.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2024 - All Rights Reserved
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

#endregion

namespace KGySoft.Drawing.ImagingTools.View
{
    internal static class DrawToolTipEventArgsExtensions
    {
        #region Methods

        internal static void DrawToolTipAdvanced(this DrawToolTipEventArgs e)
        {
            // Same as DrawBackground but will not recreate the brush again and again
            // Note: the background color of this tool tip may differ from default ToolTip but will be the same as the ones on Close/Minimize/Maximize buttons
            e.Graphics.FillRectangle(SystemBrushes.Info, e.Bounds);
            e.DrawBorder();

            var flags = TextFormatFlags.HidePrefix | TextFormatFlags.VerticalCenter | TextFormatFlags.LeftAndRightPadding;
            if (Res.DisplayLanguage.TextInfo.IsRightToLeft)
                flags |= TextFormatFlags.RightToLeft | TextFormatFlags.Right;
            
            e.DrawText(flags);
        }

        #endregion
    }
}