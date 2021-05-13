#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ScrollbarExtensions.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
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

namespace KGySoft.Drawing.ImagingTools.View
{
    internal static class ScrollbarExtensions
    {
        #region Methods

        internal static void SetValueSafe(this ScrollBar scrollBar, int value)
        {
            if (value < scrollBar.Minimum)
                value = scrollBar.Minimum;
            else if (value > scrollBar.Maximum - scrollBar.LargeChange + 1)
                value = scrollBar.Maximum - scrollBar.LargeChange + 1;

            scrollBar.Value = value;
        }

        #endregion
    }
}