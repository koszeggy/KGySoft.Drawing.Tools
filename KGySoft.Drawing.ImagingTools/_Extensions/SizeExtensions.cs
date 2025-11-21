#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: SizeExtensions.cs
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

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal static class SizeExtensions
    {
        #region Methods

        internal static SizeF ScaleF(this Size size, PointF scale) =>
            new SizeF(scale.X * size.Width, scale.Y * size.Height);

        internal static Size Scale(this Size size, PointF scale) =>
            Size.Round(ScaleF(size, scale));

        internal static Size Scale(this Size size, float scale) => size.Scale(new PointF(scale, scale));

        #endregion
    }
}
