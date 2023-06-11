#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ScalingCheckBox.cs
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
#if NET35
            // The scaling is different in .NET 3.5 so instead if subtracting a constant padding difference
            // we need to add some based on scaling, but only when visual styles are not applied
            if (!Application.RenderWithVisualStyles)
                result.Width += this.ScaleWidth(2);
#else
            result.Width -= 3;
#endif

            FlatStyle = FlatStyle.System;
            ResumeLayout();
            return result;
        }

        #endregion
    }
}