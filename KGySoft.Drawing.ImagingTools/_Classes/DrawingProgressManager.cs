#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DrawingProgressManager.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2025 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;

using KGySoft.Threading;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal class DrawingProgressManager : IAsyncProgress
    {
        #region Fields

        private readonly Action<AsyncProgress<DrawingOperation>> reportCallback;
        private readonly object syncRoot = new object();

        private AsyncProgress<DrawingOperation> current;

        #endregion

        #region Constructors

        internal DrawingProgressManager(Action<AsyncProgress<DrawingOperation>> reportCallback) => this.reportCallback = reportCallback;

        #endregion

        #region Methods

        public void Report<T>(AsyncProgress<T> progress)
        {
            if (progress is not AsyncProgress<DrawingOperation> drawingProgress)
                return;

            lock (syncRoot)
            {
                if (drawingProgress == current)
                    return;
                current = drawingProgress;
            }

            reportCallback.Invoke(drawingProgress);
        }

        public void New<T>(T operationType, int maximumValue = 0, int currentValue = 0)
            => Report(new AsyncProgress<T>(operationType, maximumValue, currentValue));

        public void Increment()
        {
            lock (syncRoot)
            {
                if (current.CurrentValue >= current.MaximumValue)
                    return;
                current = new AsyncProgress<DrawingOperation>(current.OperationType, current.MaximumValue, current.CurrentValue + 1);
            }

            reportCallback.Invoke(current);
        }

        public void SetProgressValue(int value)
        {
            lock (syncRoot)
            {
                if (current.CurrentValue == value)
                    return;
                current = new AsyncProgress<DrawingOperation>(current.OperationType, current.MaximumValue, Math.Min(value, current.MaximumValue));
            }

            reportCallback.Invoke(current);
        }

        public void Complete()
        {
            lock (syncRoot)
            {
                if (current.CurrentValue >= current.MaximumValue)
                    return;
                current = new AsyncProgress<DrawingOperation>(current.OperationType, current.MaximumValue, current.MaximumValue);
            }

            reportCallback.Invoke(current);
        }

        #endregion
    }
}
