#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdvancedToolStripDropDownButton.cs
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

using System.Drawing;
using System.Windows.Forms;

using KGySoft.WinForms;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Components
{
    /// <summary>
    /// A <see cref="ToolStripDropDownButton"/> that can scale its arrow regardless of .NET version and app.config settings.
    /// </summary>
    internal class AdvancedToolStripDropDownButton : ToolStripDropDownButton
    {
        #region Fields

        private static readonly Size arrowSizeUnscaled = new Size(5, 3);
        private static readonly Size arrowPaddingUnscaled = new Size(2, 2);

        #endregion

        #region Properties

        #region Properties

        internal Rectangle ArrowRectangle
        {
            get
            {
                Padding padding = ArrowPadding;
                Size size = ArrowSize;
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

        internal Rectangle ImageRectangle
        {
            get
            {
                var bounds = new Rectangle(Point.Empty, Size);
                if (TextDirection == ToolStripTextDirection.Horizontal)
                {
                    int arrowRectWidth = ArrowSize.Width + ArrowPadding.Vertical;
                    bounds.Width -= arrowRectWidth;
                    if (RightToLeft == RightToLeft.Yes)
                        bounds.X += arrowRectWidth;
                }

                bounds.Inflate(-1, -1);
                Size imageSize = Owner.ImageScalingSize;
                Rectangle imageRect = new Rectangle(bounds.X + bounds.Width / 2 - imageSize.Width / 2, bounds.Y + bounds.Height / 2 - imageSize.Height / 2, imageSize.Width, imageSize.Height);
                return Rectangle.Intersect(bounds, imageRect);
            }
        }

        #endregion

        #region Private Properties

        private PointF Scale => Owner?.GetScale() ?? ScaleHelper.SystemScale;
        private Size ArrowSize => arrowSizeUnscaled.Scale(Scale);

        private Padding ArrowPadding
        {
            get
            {
                Size scaled = arrowPaddingUnscaled.Scale(Scale);
                return new Padding(scaled.Width, scaled.Height, scaled.Width, scaled.Height);
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Methods

        public override Size GetPreferredSize(Size constrainingSize)
        {
            bool showArrow = ShowDropDownArrow;
            ShowDropDownArrow = false;
            var preferredSize = base.GetPreferredSize(constrainingSize);
            if (!showArrow)
                return preferredSize;
            ShowDropDownArrow = true;
            Size arrowSize = ArrowSize;
            Padding arrowPadding = ArrowPadding;
            if (TextDirection == ToolStripTextDirection.Horizontal)
                preferredSize.Width += arrowSize.Width + arrowPadding.Horizontal;
            else
                preferredSize.Height += arrowSize.Height + arrowPadding.Vertical;
            return preferredSize;
        }

        #endregion

        #region Internal Methods

#if NETFRAMEWORK
        internal void AdjustImageRectangle(ref Rectangle imageBounds)
        {
            if (RightToLeft == RightToLeft.Yes)
                imageBounds.X = Width - 2 - imageBounds.Width;
            else
                imageBounds.X = 2;
        }
#endif

        #endregion

        #endregion
    }
}
