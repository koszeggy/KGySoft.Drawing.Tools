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
using System.Collections.Generic;
using System.Drawing;

using KGySoft.Collections;
using KGySoft.CoreLibraries;

#endregion

namespace KGySoft.Drawing.ImagingTools.View
{
    internal static class ColorExtensions
    {
        #region Fields

        // The caches need to be thread-safe because there can be multiple UI threads. Using AsThreadSafe instead of GetThreadSafeAccessor because of clearing.
        private static readonly LockingDictionary<int, Pen> penCache = new Cache<int, Pen>(c => new Pen(Color.FromArgb(c)), 16)
        {
            DisposeDroppedValues = true,
            EnsureCapacity = true,
        }.AsThreadSafe();

        private static readonly LockingDictionary<int, Brush> brushCache = new Cache<int, Brush>(c => new SolidBrush(Color.FromArgb(c)), 16)
        {
            DisposeDroppedValues = true,
            EnsureCapacity = true,
        }.AsThreadSafe();

        #endregion

        #region Methods

        internal static void ClearCaches()
        {
            #region Local Methods
            
            static void Clear<TKey, TValue>(LockingDictionary<TKey, TValue> cache) where TKey : notnull
            {
                ICollection<TValue> values = cache.Values;
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