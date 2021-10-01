#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DrawingProgressFooter.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System.Windows.Forms;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    internal class DrawingProgressFooter : ProgressFooter<DrawingProgress>
    {
        #region Fields

        private DrawingProgress? displayedProgress;

        #endregion

        #region Methods

        protected override void UpdateProgress()
        {
            DrawingProgress progress = Progress;
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