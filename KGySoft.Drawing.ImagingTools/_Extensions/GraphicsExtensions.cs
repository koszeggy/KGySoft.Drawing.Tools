#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: GraphicsExtensions.cs
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

using System.Drawing;
using System.Drawing.Imaging;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal static class GraphicsExtensions
    {
        #region Methods

        // The same as in KGySoft.WinForms. Remove if AdvancedToolStrip/ControlPaintHelper will be migrated there.
        internal static void DrawImageColorized(this Graphics graphics, Image image, Rectangle destRect, Color targetColor)
        {
            ImageAttributes? attr = null;
            try
            {
                if (targetColor.ToArgb() != Color.Black.ToArgb())
                {
                    attr = new ImageAttributes();
                    var map = new ColorMap { OldColor = Color.Black, NewColor = targetColor };
                    attr.SetRemapTable([map], ColorAdjustType.Bitmap);
                }

                graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attr);
            }
            finally
            {
                attr?.Dispose();
            }
        }

        #endregion
    }
}