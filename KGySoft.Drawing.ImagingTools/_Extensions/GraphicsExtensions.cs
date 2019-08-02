#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: GraphicsExtensions.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2019 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution. If not, then this file is considered as
//  an illegal copy.
//
//  Unauthorized copying of this file, via any medium is strictly prohibited.
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

        #endregion
    }
}
