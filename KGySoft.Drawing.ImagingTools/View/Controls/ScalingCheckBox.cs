using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    /// <summary>
    /// A CheckBox, which is scaled properly even with System FlatStyle
    /// </summary>
    internal class ScalingCheckBox : CheckBox
    {
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
            result.Width -= 3;

            FlatStyle = FlatStyle.System;
            ResumeLayout();
            return result;
        }
    }
}
