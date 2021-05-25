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
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using KGySoft.Drawing.ImagingTools.View.Components;
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

            #region Static Methods

            private static void ClearButtonBackground(Graphics g, Rectangle rect, Color color)
            {
                GraphicsState state = g.Save();
                rect.Inflate(1, 1);
                g.SetClip(rect);
                g.Clear(color);
                g.Restore(state);
            }

            #endregion

            #region Instance Methods

            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
            {
                Graphics g = e.Graphics;
                Rectangle dropDownRect = e.Item is ScalingToolStripDropDownButton scalingButton ? scalingButton.ArrowRectangle : e.ArrowRectangle;
                using (Brush brush = new SolidBrush(e.Item.Enabled ? e.ArrowColor : SystemColors.ControlDark))
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
                if (e.Item is ToolStripButton { Checked: true, Enabled: true } btn)
                    ClearButtonBackground(e.Graphics, btn.ContentRectangle, ProfessionalColors.ButtonSelectedGradientMiddle);

                base.OnRenderButtonBackground(e);
            }

            protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
            {
                base.OnRenderSplitButtonBackground(e);
                if (e.Item is not AdvancedToolStripSplitButton btn)
                    return;

                // overriding background to behave the same way as ToolStripButton
                Rectangle rect = btn.ButtonBounds;
                if (!OSUtils.IsWindows)
                    rect.Location = Point.Empty;

                if (btn.Enabled && (btn.Checked || !btn.DropDownButtonPressed))
                {
                    rect.Inflate(-1, -1);
                    if (btn.ButtonPressed)
                        ClearButtonBackground(e.Graphics, rect, ProfessionalColors.ButtonPressedHighlight);
                    else if (btn.Selected)
                        ClearButtonBackground(e.Graphics, rect, btn.Checked ? ProfessionalColors.ButtonPressedHighlight : ProfessionalColors.ButtonSelectedGradientMiddle);
                    else if (btn.Checked)
                        ClearButtonBackground(e.Graphics, rect, ProfessionalColors.ButtonSelectedGradientMiddle);
                    rect.Inflate(1, 1);
                }

                // drawing border (maybe again, because it can be overridden by background)
                if (btn.Checked || !btn.DropDownButtonPressed && (btn.ButtonPressed || btn.ButtonSelected))
                {
                    using (Pen pen = new Pen(ProfessionalColors.ButtonSelectedBorder))
                        e.Graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height - 1);
                }
            }

            #endregion

            #endregion
        }

        #endregion

        #endregion

        #region Fields

        #region Static Fields
        
        private static readonly Size referenceSize = new Size(16, 16);

        #endregion

        #region Instance Fields

        private DockStyle explicitDock = DockStyle.Top;
        private bool isAdjustingRtl;

        #endregion

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

        protected override void OnItemAdded(ToolStripItemEventArgs e)
        {
            if (e.Item is ToolStripSplitButton splitBtn)
                splitBtn.DropDownButtonWidth = this.ScaleWidth(11);

            base.OnItemAdded(e);
        }

        protected override void OnDockChanged(EventArgs e)
        {
            base.OnDockChanged(e);
            if (isAdjustingRtl)
                return;
            explicitDock = Dock;
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            DockStyle dock = Dock;
            if (dock is not (DockStyle.Left or DockStyle.Right))
                return;

            bool isMirrored = RightToLeft == RightToLeft.Yes;
            if (isMirrored ^ dock == explicitDock)
                return;
            isAdjustingRtl = true;
            Dock = isMirrored
                ? explicitDock == DockStyle.Left ? DockStyle.Right : DockStyle.Left
                : explicitDock;
            isAdjustingRtl = false;
        }

        #endregion  
    }
}
