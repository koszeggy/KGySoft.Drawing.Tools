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

        internal DrawingProgressManager(Action<DrawingProgress> reportCallback)
        {
            this.reportCallback = reportCallback;
        }

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
            => Report(new DrawingProgress { OperationType = operationType, MaximumValue = maximumValue, CurrentValue = currentValue });

        public void Increment()
        {
            DrawingProgress copy;
            lock (syncRoot)
            {
                if (current.CurrentValue >= current.MaximumValue)
                    return;
                current.CurrentValue++;
                copy = current;
            }

            reportCallback.Invoke(copy);
        }

        public void SetProgressValue(int value)
        {
            DrawingProgress copy;
            lock (syncRoot)
            {
                if (current.CurrentValue == value)
                    return;
                current.CurrentValue = Math.Min(value, current.MaximumValue);
                copy = current;
            }

            reportCallback.Invoke(copy);
        }

        public void Complete()
        {
            DrawingProgress copy;
            lock (syncRoot)
            {
                if (current.CurrentValue >= current.MaximumValue)
                    return;
                current.CurrentValue = current.MaximumValue;
                copy = current;
            }

            reportCallback.Invoke(copy);
        }

        #endregion
    }
}
