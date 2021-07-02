#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorExtensions.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution. If not, then this file is considered as
//  an illegal copy.
//
//  Unauthorized copying of this file, via any medium is strictly prohibited.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System.Drawing;

using KGySoft.Collections;

#endregion

namespace KGySoft.Drawing.ImagingTools.View
{
    internal static class ColorExtensions
    {
        #region Fields

        private static readonly Cache<int, Pen> penCache = new(c => new Pen(Color.FromArgb(c)), 4)
        {
            DisposeDroppedValues = true,
            EnsureCapacity = true,
        };

        private static readonly Cache<int, Brush> brushCache = new(c => new SolidBrush(Color.FromArgb(c)), 4)
        {
            DisposeDroppedValues = true,
            EnsureCapacity = true,
        };

        #endregion

        #region Methods

        internal static Pen GetPen(this Color color) => color.IsSystemColor
            ? SystemPens.FromSystemColor(color)
            : penCache[color.ToArgb()];

        internal static Brush GetBrush(this Color color) => color.IsSystemColor
            ? SystemBrushes.FromSystemColor(color)
            : brushCache[color.ToArgb()];

        #endregion
    }
}