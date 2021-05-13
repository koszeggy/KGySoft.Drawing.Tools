using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using KGySoft.Collections;

namespace KGySoft.Drawing.ImagingTools.View
{
    internal static class Cursors
    {
        private class CursorInfo
        {
            private readonly Icon icon;

            private readonly Dictionary<int, (CursorHandle Handle, Cursor Cursor)> createdCursors = new();

            internal CursorInfo(Icon icon) => this.icon = icon;

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
        }

        private static readonly Size referenceSize = new Size(16, 16);

        private static StringKeyedDictionary<CursorInfo> cursors = new StringKeyedDictionary<CursorInfo>();


        internal static Cursor HandOpen => GetCreateCursor();
        internal static Cursor HandGrab => GetCreateCursor();

        private static Cursor GetCreateCursor([CallerMemberName]string resourceName = null!)
        {
            if (!cursors.TryGetValue(resourceName, out CursorInfo? info))
                cursors[resourceName] = info = new CursorInfo((Icon)Properties.Resources.ResourceManager.GetObject(resourceName, CultureInfo.InvariantCulture)!);
            return info.GetCreateCursor(referenceSize.Scale(OSUtils.SystemScale));
        }
    }
}