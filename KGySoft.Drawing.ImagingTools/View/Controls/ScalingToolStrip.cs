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

using System.Drawing;
using System.Windows.Forms;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    /// <summary>
    /// A <see cref="ToolStrip"/> that can scale its content regardless of .NET version and app.config settings.
    /// </summary>
    internal class ScalingToolStrip : ToolStrip
    {
        #region Nested classes

        #region ScalingToolStripMenuRenderer class

        private class ScalingToolStripMenuRenderer : ToolStripProfessionalRenderer
        {
            #region Fields

            private static readonly Size referenceOffset = new Size(2, 2);
            private static readonly Size referenceOffsetDouble = new Size(4, 4);

            #endregion

            #region Methods

            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
            {
                Graphics g = e.Graphics;
                Rectangle dropDownRect = e.Item is ScalingToolStripDropDownButton scalingButton ? scalingButton.ArrowRectangle : e.ArrowRectangle;
                using (Brush brush = new SolidBrush(e.ArrowColor))
                {
                    Point middle = new Point(dropDownRect.Left + dropDownRect.Width / 2, dropDownRect.Top + dropDownRect.Height / 2);

                    Point[] arrow;

                    var offset = g.ScaleSize(referenceOffset);
                    var offsetDouble = g.ScaleSize(referenceOffsetDouble);

                    switch (e.Direction)
                    {
                        case ArrowDirection.Up:

                            arrow = new Point[] {
                                new Point(middle.X - offset.Width, middle.Y + 1),
                                new Point(middle.X + offset.Width + 1, middle.Y + 1),
                                new Point(middle.X, middle.Y - offset.Height)};

                            break;
                        case ArrowDirection.Left:
                            arrow = new Point[] {
                                new Point(middle.X + offset.Width, middle.Y - offsetDouble.Height),
                                new Point(middle.X + offset.Width, middle.Y + offsetDouble.Height),
                                new Point(middle.X - offset.Width, middle.Y)};

                            break;
                        case ArrowDirection.Right:
                            arrow = new Point[] {
                                new Point(middle.X - offset.Width, middle.Y - offsetDouble.Height),
                                new Point(middle.X - offset.Width, middle.Y + offsetDouble.Height),
                                new Point(middle.X + offset.Width, middle.Y)};

                            break;
                        default:
                            arrow = new Point[] {
                                new Point(middle.X - offset.Width, middle.Y - 1),
                                new Point(middle.X + offset.Width + 1, middle.Y - 1),
                                new Point(middle.X, middle.Y + offset.Height) };
                            break;
                    }

                    g.FillPolygon(brush, arrow);
                }
            }

            protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
            {
                Rectangle imageRect = e.ImageRectangle;
                Image image = e.Image;

                if (imageRect != Rectangle.Empty && image != null)
                {
                    bool disposeImage = false;
                    if (!e.Item.Enabled)
                    {
                        image = CreateDisabledImage(image);
                        disposeImage = true;
                    }

                    // Draw the checkmark background (providing no image)
                    base.OnRenderItemCheck(new ToolStripItemImageRenderEventArgs(e.Graphics, e.Item, null, e.ImageRectangle));

                    // Draw the checkmark image scaled to the image rectangle
                    e.Graphics.DrawImage(image, imageRect, new Rectangle(Point.Empty, image.Size), GraphicsUnit.Pixel);

                    if (disposeImage)
                        image.Dispose();
                }
            }

            protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
            {
                if (e.Item is ToolStripButton button && button.Checked && button.Enabled)
                    e.Graphics.Clear(ProfessionalColors.ButtonSelectedGradientMiddle);

                base.OnRenderButtonBackground(e);
            }

            #endregion
        }

        #endregion

        #endregion

        #region Fields

        private static readonly Size referenceSize = new Size(16, 16);

        #endregion

        #region Constructors

        internal ScalingToolStrip()
        {
            ImageScalingSize = Size.Round(this.ScaleSize(referenceSize));
            Renderer = new ScalingToolStripMenuRenderer();
        }

        #endregion
    }
}
