#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AsyncTaskBase.cs
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
using System.Threading;

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    /// <summary>
    /// Represents a cancellable and completable asynchronous task that is compatible with all targeted frameworks.
    /// </summary>
    internal abstract class AsyncTaskBase : IDisposable
    {
        #region Fields

        #region Internal Fields

        internal volatile bool IsCanceled;

        #endregion

        #region Protected Fields

        protected volatile bool IsDisposed;

        #endregion

        #region Private Fields

        private readonly ManualResetEventSlim completedEvent;

        #endregion

        #endregion

        #region Constructors

        protected AsyncTaskBase() => completedEvent = new ManualResetEventSlim();

        #endregion

        #region Methods

        #region Public Methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Internal Methods

        internal virtual void SetCompleted() => completedEvent.Set();

        internal void WaitForCompletion()
        {
            if (IsDisposed)
                return;

            try
            {
                completedEvent.Wait();
            }
            catch (ObjectDisposedException)
            {
                // it can happen that the task has just been completed after querying IsCompleted but this part
                // must not be in a lock because that may cause deadlocks
            }
        }

        #endregion

        #region Protected Methods

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;
            if (disposing)
            {
                completedEvent.Set();
                completedEvent.Dispose();
            }

            IsDisposed = true;
        }

        #endregion

        #endregion
    }
}
