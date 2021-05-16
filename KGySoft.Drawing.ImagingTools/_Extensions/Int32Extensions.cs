using System;

namespace KGySoft.Drawing.ImagingTools
{
    internal static class Int32Extensions
    {
        internal static int Scale(this int size, float scale) =>
            (int)MathF.Round(size * scale);
    }
}
