#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DrawingProgressManager.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2020 - All Rights Reserved
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
    internal class DrawingProgressManager : IDrawingProgress
    {
        #region Fields

        private readonly Action<DrawingProgress> reportCallback;
        private readonly object syncRoot = new object();

        private DrawingProgress current;

        #endregion

        #region Constructors

        internal DrawingProgressManager(Action<DrawingProgress> reportCallback) => this.reportCallback = reportCallback;

        #endregion

        #region Methods

        public void Report(DrawingProgress progress)
        {
            lock (syncRoot)
            {
                if (progress == current)
                    return;
                current = progress;
            }

            reportCallback.Invoke(progress);
        }

        public void New(DrawingOperation operationType, int maximumValue, int currentValue)
            => Report(new DrawingProgress(operationType, maximumValue, currentValue));

        public void Increment()
        {
            lock (syncRoot)
            {
                if (current.CurrentValue >= current.MaximumValue)
                    return;
                current = new DrawingProgress(current.OperationType, current.MaximumValue, current.CurrentValue + 1);
            }

            reportCallback.Invoke(current);
        }

        public void SetProgressValue(int value)
        {
            lock (syncRoot)
            {
                if (current.CurrentValue == value)
                    return;
                current = new DrawingProgress(current.OperationType, current.MaximumValue, Math.Min(value, current.MaximumValue));
            }

            reportCallback.Invoke(current);
        }

        public void Complete()
        {
            lock (syncRoot)
            {
                if (current.CurrentValue >= current.MaximumValue)
                    return;
                current = new DrawingProgress(current.OperationType, current.MaximumValue, current.MaximumValue);
            }

            reportCallback.Invoke(current);
        }

        #endregion
    }
}
