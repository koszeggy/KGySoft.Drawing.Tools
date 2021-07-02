#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Int32Extensions.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
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
    internal static class Int32Extensions
    {
        #region Methods

        internal static int Scale(this int size, float scale) =>
            (int)MathF.Round(size * scale);

        #endregion
    }
}