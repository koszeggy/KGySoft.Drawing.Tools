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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.WinApi;

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

            [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "False alarm, see the disposing at the end")]
            protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
            {
                Rectangle imageRect = e.ImageRectangle;
                Image image = e.Image;
                if (imageRect == Rectangle.Empty || image == null)
                    return;

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

            protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
            {
                if (e.Item is ToolStripButton button && button.Checked && button.Enabled)
                {
                    if (OSUtils.IsWindows)
                        e.Graphics.Clear(ProfessionalColors.ButtonSelectedGradientMiddle);
                    else
                    {
                        // In Mono without this clipping the whole tool strip container is cleared
                        GraphicsState state = e.Graphics.Save();
                        Rectangle rect = e.Item.ContentRectangle;
                        rect.Inflate(1, 1);
                        e.Graphics.SetClip(rect);
                        e.Graphics.Clear(ProfessionalColors.ButtonSelectedGradientMiddle);
                        e.Graphics.Restore(state);
                    }
                }

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

        public ScalingToolStrip()
        {
            ImageScalingSize = Size.Round(this.ScaleSize(referenceSize));
            Renderer = new ScalingToolStripMenuRenderer();
        }

        #endregion

        #region Methods

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // ensuring that items can be clicked even if the container form is not activated
            if (m.Msg == Constants.WM_MOUSEACTIVATE && m.Result == (IntPtr)Constants.MA_ACTIVATEANDEAT)
                m.Result = (IntPtr)Constants.MA_ACTIVATE;
        }

        #endregion
    }
}
