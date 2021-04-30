#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: IntPtrExtensions.cs
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