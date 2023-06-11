#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DrawingProgressFooter.cs
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

using System.Windows.Forms;

using KGySoft.Threading;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    internal class DrawingProgressFooter : ProgressFooter<AsyncProgress<DrawingOperation>>
    {
        #region Fields

        private AsyncProgress<DrawingOperation>? displayedProgress;

        #endregion

        #region Methods

        protected override void UpdateProgress()
        {
            AsyncProgress<DrawingOperation> progress = Progress;
            if (progress == displayedProgress)
                return;

            if (displayedProgress?.OperationType != progress.OperationType)
                ProgressText = Res.Get(progress.OperationType);
            if (progress.MaximumValue == 0)
                ProgressStyle = ProgressBarStyle.Marquee;
            else
            {
                ProgressStyle = ProgressBarStyle.Blocks;
                Maximum = progress.MaximumValue;
                Value = progress.CurrentValue;
            }

            displayedProgress = progress;
        }

        #endregion
    }
}