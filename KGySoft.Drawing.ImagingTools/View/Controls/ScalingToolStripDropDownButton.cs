#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ScalingToolStripDropDownButton.cs
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

using System.Drawing;
using System.Windows.Forms;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    /// <summary>
    /// A <see cref="ToolStripDropDownButton"/> that can scale its arrow regardless of .NET version and app.config settings.
    /// </summary>
    internal class ScalingToolStripDropDownButton : ToolStripDropDownButton
    {
        #region Fields

        #region Static Fields

        private static readonly Size arrowSizeUnscaled = new Size(5, 3);
        private static readonly Size arrowPaddingUnscaled = new Size(2, 2);

        #endregion

        #region Instance Fields

        private Size arrowSize;
        private Padding arrowPadding;

        #endregion

        #endregion

        #region Properties

        internal Size ArrowSize
        {
            get
            {
                if (!arrowSize.IsEmpty)
                    return arrowSize;
                return arrowSize = Size.Round(Owner.ScaleSize(arrowSizeUnscaled));
            }
        }

        internal Padding ArrowPadding
        {
            get
            {
                if (arrowPadding != Padding.Empty)
                    return arrowPadding;
                var scaled = Size.Round(Owner.ScaleSize(arrowPaddingUnscaled));
                return arrowPadding = new Padding(scaled.Width, scaled.Height, scaled.Width, scaled.Height);
            }
        }

        internal Rectangle ArrowRectangle
        {
            get
            {
                var padding = ArrowPadding;
                var size = ArrowSize;
                var bounds = new Rectangle(Point.Empty, Size);
                if (TextDirection == ToolStripTextDirection.Horizontal)
                {
                    int x = size.Width + padding.Horizontal;
                    if (RightToLeft == RightToLeft.Yes)
                        return new Rectangle(padding.Left, 0, size.Width, bounds.Height);
                    return new Rectangle(bounds.Right - x, 0, size.Width, bounds.Height);
                }

                int y = size.Height + padding.Vertical;
                return new Rectangle(0, bounds.Bottom - y + padding.Top, bounds.Width - 1, size.Height);
            }
        }

        #endregion

        #region Methods

        public override Size GetPreferredSize(Size constrainingSize)
        {
            var showArrow = ShowDropDownArrow;
            ShowDropDownArrow = false;
            var preferredSize = base.GetPreferredSize(constrainingSize);
            if (!showArrow)
                return preferredSize;
            ShowDropDownArrow = true;
            if (TextDirection == ToolStripTextDirection.Horizontal)
                preferredSize.Width += ArrowSize.Width + ArrowPadding.Horizontal;
            else
                preferredSize.Height += ArrowSize.Height + ArrowPadding.Vertical;
            return preferredSize;
        }

        #endregion  
    }
}
