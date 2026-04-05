#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdvancedToolStripButton.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2026 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using KGySoft.WinForms;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Components
{
    /// <summary>
    /// A <see cref="ToolStripButton"/> with correct sizing.
    /// Needed for .NET 10(+?), where changing the DPI from high (initial) to low (custom) is stuck to the high DPI button width.
    /// </summary>
    internal class AdvancedToolStripButton : ToolStripButton
    {
        #region Fields

        private static readonly Size referencePadding = new Size(7, 6);

        #endregion

        #region Methods

        #region Public Methods

        public override Size GetPreferredSize(Size constrainingSize)
        {
            if (Owner == null || DisplayStyle != ToolStripItemDisplayStyle.Image)
                return base.GetPreferredSize(constrainingSize);

            // .NET 10(+?): forcing the correct button width of image-only buttons.
            // Older platforms work well when ImageScalingSize and Font are adjusted, but in .NET 10 the base.GetPreferredSize is broken.
            return Owner.ImageScalingSize + Owner.ScaleSize(referencePadding);
        }

        #endregion

        #endregion
    }
}
