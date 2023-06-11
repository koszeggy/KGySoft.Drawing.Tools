#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: IntPtrExtensions.cs
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

namespace KGySoft.Drawing.ImagingTools.View
{
    internal static class IntPtrExtensions
    {
        #region Methods

        internal static int GetSignedLoWord(this IntPtr p) => (short)(p.ToInt64() & 0xFFFF);
        internal static int GetSignedHiWord(this IntPtr p) => (short)((p.ToInt64() >> 16) & 0xFFFF);

        #endregion
    }
}