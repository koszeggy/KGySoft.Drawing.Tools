#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorExtensions.cs
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

        private static readonly Cache<int, Brush> brushCache = new(c => new SolidBrush(Color.FromArgb(c)), 8)
        {
            DisposeDroppedValues = true,
            EnsureCapacity = true,
        };

        #endregion

        #region Methods

        internal static void ClearCaches()
        {
            #region Local Methods
            
            static void Clear(ICache cache)
            {
                var values = cache.Values;
                cache.Clear();
                foreach (var value in values)
                    (value as IDisposable)?.Dispose();
            }

            #endregion

            Clear(penCache);
            Clear(brushCache);
        }

        internal static Pen GetPen(this Color color) => color.IsSystemColor
            ? SystemPens.FromSystemColor(color)
            : penCache[color.ToArgb()];

        internal static Brush GetBrush(this Color color) => color.IsSystemColor
            ? SystemBrushes.FromSystemColor(color)
            : brushCache[color.ToArgb()];

        internal static Color ToThemeColor(this Color color) => color.IsSystemColor
            ? ThemeColors.FromKnownColor(color.ToKnownColor())
            : color;

        #endregion
    }
}