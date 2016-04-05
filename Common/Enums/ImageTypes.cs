using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KGySoft.DebuggerVisualizers.Common
{
    [Flags]
    internal enum ImageTypes
    {
        None = 0,
        Bitmap = 1,
        Metafile = 1 << 1,
        Icon = 1 << 2,
        All = Bitmap | Metafile | Icon
    }
}
