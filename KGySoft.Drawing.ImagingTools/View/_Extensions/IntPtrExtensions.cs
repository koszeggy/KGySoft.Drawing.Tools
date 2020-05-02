using System;

namespace KGySoft.Drawing.ImagingTools.View
{
    internal static class IntPtrExtensions
    {
        internal static int GetSignedLoWord(this IntPtr p) => (short)((int)p & 0xFFFF);
        internal static int GetSignedHiWord(this IntPtr p) => (short)(((int)p >> 16) & 0xFFFF);
    }
}
