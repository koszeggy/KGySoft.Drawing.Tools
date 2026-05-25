#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ThreadHelper.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2026 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

#if NET45_OR_GREATER || NETCOREAPP
using System;
#else
using System.Threading;
#endif

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal static class ThreadHelper
    {
        #region Properties

#if NET45_OR_GREATER || NETCOREAPP
        internal static int ManagedThreadId => Environment.CurrentManagedThreadId;
#else
        internal static int ManagedThreadId => Thread.CurrentThread.ManagedThreadId;
#endif

        #endregion
    }
}