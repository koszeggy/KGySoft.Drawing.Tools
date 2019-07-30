#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ScalingToolStrip.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2019 - All Rights Reserved
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
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace KGySoft.Drawing.ImagingTools.Controls
{
    internal class ScalingToolStrip : ToolStrip
    {
        #region Constructors

        internal ScalingToolStrip()
        {
            double scale;
            using (Graphics g = CreateGraphics())
                scale = Math.Round(Math.Max(g.DpiX, g.DpiY) / 96, 2);
            if (scale > 1)
            {
                ImageScalingSize = new Size((int)(ImageScalingSize.Width * scale), (int)(ImageScalingSize.Height * scale));
                AutoSize = false;
            }
        }

        #endregion
    }
}
