﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AllowedImageTypes.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2023 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    [Flags]
    internal enum AllowedImageTypes
    {
        None = 0,
        Bitmap = 1,
        Metafile = 1 << 1,
        Icon = 1 << 2,
        All = Bitmap | Metafile | Icon
    }
}
