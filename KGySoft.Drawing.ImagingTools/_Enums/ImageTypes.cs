#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageTypes.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2019 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution. If not, then this file is considered as
//  an illegal copy.
//
//  Unauthorized copying of this file, via any medium is strictly prohibited.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;

#endregion

namespace KGySoft.Drawing.ImagingTools
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
