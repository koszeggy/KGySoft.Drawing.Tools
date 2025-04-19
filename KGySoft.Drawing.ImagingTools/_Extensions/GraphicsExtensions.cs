#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: GraphicsExtensions.cs
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

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal static class GraphicsExtensions
    {
        #region Methods

        internal static PointF GetScale(this Graphics graphics)
        {
            if (graphics == null)
                throw new ArgumentNullException(nameof(graphics));
            return new PointF(graphics.DpiX / 96f, graphics.DpiY / 96f);
        }

        internal static Size ScaleSize(this Graphics graphics, Size size) => size.Scale(graphics.GetScale());
        internal static int ScaleWidth(this Graphics graphics, int width) => width.Scale(graphics.GetScale().X);

        #endregion
    }
}
