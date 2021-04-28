﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ScalingCheckBox.cs
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

using System.Drawing;
using System.Windows.Forms;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    /// <summary>
    /// A CheckBox, which is scaled properly even with System FlatStyle
    /// </summary>
    internal class ScalingCheckBox : CheckBox
    {
        #region Methods

        public override Size GetPreferredSize(Size proposedSize)
        {
            var flatStyle = FlatStyle;
            if (flatStyle != FlatStyle.System)
                return base.GetPreferredSize(proposedSize);

            // preventing auto resize while changing style
            SuspendLayout();
            FlatStyle = FlatStyle.Standard;

            // The gap between the CheckBox and the text is 3px smaller with System at every DPI
            Size result = base.GetPreferredSize(proposedSize);
#if !NET35
            // The scaling is different in .NET 3.5 so there we don't subtract the padding difference
            result.Width -= 3;
#endif

            FlatStyle = FlatStyle.System;
            ResumeLayout();
            return result;
        }

        #endregion
    }
}