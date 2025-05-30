﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DrawToolTipEventArgsExtensions.cs
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

#endregion

namespace KGySoft.Drawing.ImagingTools.View
{
    internal static class DrawToolTipEventArgsExtensions
    {
        #region Methods

        internal static void DrawToolTipAdvanced(this DrawToolTipEventArgs e)
        {
            Color backColor = ThemeColors.ToolTip;
            Color foreColor = ThemeColors.ToolTipText;
            Color frameColor = ThemeColors.ToolTipBorder;
            e = new DrawToolTipEventArgs(e.Graphics, e.AssociatedWindow, e.AssociatedControl, e.Bounds, e.ToolTipText, backColor, foreColor, e.Font);
            e.DrawBackground();

            // cannot use e.DrawBorder() here, because it hard-codes the border color to SystemColors.WindowFrame
            ControlPaint.DrawBorder(e.Graphics, e.Bounds, frameColor, ButtonBorderStyle.Solid);

            var flags = TextFormatFlags.HidePrefix | TextFormatFlags.VerticalCenter | TextFormatFlags.LeftAndRightPadding;
            if (Res.DisplayLanguage.TextInfo.IsRightToLeft)
                flags |= TextFormatFlags.RightToLeft | TextFormatFlags.Right;
            
            e.DrawText(flags);
        }

        #endregion
    }
}