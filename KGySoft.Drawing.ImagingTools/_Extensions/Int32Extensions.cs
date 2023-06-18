﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Int32Extensions.cs
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
    internal static class Int32Extensions
    {
        #region Methods

        internal static int Scale(this int size, float scale) =>
            (int)MathF.Round(size * scale);

        #endregion
    }
}