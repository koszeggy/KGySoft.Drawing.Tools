using System.Windows.Forms;

namespace KGySoft.Drawing.ImagingTools.View
{
    internal static class ScrollbarExtensions
    {
        internal static void SetValueSafe(this ScrollBar scrollBar, int value)
        {
            if (value < scrollBar.Minimum)
                value = scrollBar.Minimum;
            else if (value > scrollBar.Maximum - scrollBar.LargeChange + 1)
                value = scrollBar.Maximum - scrollBar.LargeChange + 1;

            scrollBar.Value = value;
        }
    }
}
