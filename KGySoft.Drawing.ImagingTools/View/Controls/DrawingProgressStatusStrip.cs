#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DrawingProgressStatusStrip.cs
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

using System.Windows.Forms;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    internal class DrawingProgressStatusStrip : ProgressFooter<DrawingProgress>
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