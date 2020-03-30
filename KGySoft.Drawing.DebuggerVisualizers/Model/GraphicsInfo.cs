using System.Drawing;

namespace KGySoft.Drawing.DebuggerVisualizers.Model
{
    internal sealed class GraphicsInfo
    {
        internal Bitmap Data { get; set; }
        internal float[] Elements { get; set; }
        internal Rectangle VisibleRect { get; set; }
        internal string SpecialInfo { get; set; }
    }
}