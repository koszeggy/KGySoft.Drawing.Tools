#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DownloadProgressFooter.cs
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
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    internal class DownloadProgressFooter : ProgressFooter<(int MaximumValue, int CurrentValue)>
    {
        #region Properties

        internal override bool ProgressVisible
        {
            get => base.ProgressVisible;
            set
            {
                if (value)
                    ProgressText = Res.TextDownloading;
                base.ProgressVisible = value;
            }
        }

        #endregion

        #region Methods

        protected override void UpdateProgress()
        {
            var progress = Progress;
            if (progress.MaximumValue == 0)
                ProgressStyle = ProgressBarStyle.Marquee;
            else
            {
                ProgressStyle = ProgressBarStyle.Blocks;
                Maximum = progress.MaximumValue;
                Value = progress.CurrentValue;
            }
        }

        #endregion
    }
}