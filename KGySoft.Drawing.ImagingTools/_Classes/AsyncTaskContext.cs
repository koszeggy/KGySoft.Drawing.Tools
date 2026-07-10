#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AsyncTaskContext.cs
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

using System;
using System.Threading;

using KGySoft.WinForms;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal abstract class AsyncTaskContext : AsyncTaskBase
    {
        #region Properties

        internal SynchronizationContext Context { get; }
            = SynchronizationContext.Current ?? throw new InvalidOperationException(Res.InternalError("Should be instantiated from the UI thread"));

        #endregion
    }
}