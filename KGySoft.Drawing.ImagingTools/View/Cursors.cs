#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Cursors.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

using KGySoft.Collections;

#endregion

namespace KGySoft.Drawing.ImagingTools.View
{
    internal static class Cursors
    {
        #region Nested classes

        private class CursorInfo
        {
            #region Fields

            private readonly Icon icon;
            private readonly Dictionary<int, (CursorHandle Handle, Cursor Cursor)> createdCursors = new();

            #endregion

            #region Constructors

            internal CursorInfo(Icon icon) => this.icon = icon;

            #endregion

            #region Methods

            internal Cursor GetCreateCursor(Size desiredSize)
            {
                if (createdCursors.TryGetValue(desiredSize.Width, out var value))
                    return value.Cursor;

                // extracting bitmap and not icon so any sizes should work on all platforms
                using Bitmap image = icon.ExtractNearestBitmap(desiredSize, PixelFormat.Format32bppArgb);
                CursorHandle handle = image.ToCursorHandle(new Point(image.Width >> 1, image.Height >> 1));
                Cursor result = new Cursor(handle);
                createdCursors[desiredSize.Width] = (handle, result);
                return result;
            }

            #endregion
        }

        #endregion

        #region Fields

        private static readonly Size referenceSize = new Size(16, 16);
        private static readonly StringKeyedDictionary<CursorInfo> cursors = new StringKeyedDictionary<CursorInfo>();

        #endregion

        #region Properties

        internal static Cursor HandOpen => GetCreateCursor() ?? System.Windows.Forms.Cursors.Hand;
        internal static Cursor HandGrab => GetCreateCursor() ?? System.Windows.Forms.Cursors.NoMove2D;

        #endregion

        #region Methods

        private static Cursor? GetCreateCursor([CallerMemberName] string resourceName = null!)
        {
            if (!OSUtils.IsWindows)
                return null;
            if (!cursors.TryGetValue(resourceName, out CursorInfo? info))
                cursors[resourceName] = info = new CursorInfo((Icon)Properties.Resources.ResourceManager.GetObject(resourceName, CultureInfo.InvariantCulture)!);
            return info.GetCreateCursor(referenceSize.Scale(OSUtils.SystemScale));
        }

        #endregion
    }
}
