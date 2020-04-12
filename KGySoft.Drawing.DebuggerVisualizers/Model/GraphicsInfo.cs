#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: GraphicsInfo.cs
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

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Model
{
    internal sealed class GraphicsInfo : IDisposable
    {
        #region Properties

        internal Bitmap Data { get; set; }
        internal float[] Elements { get; set; }
        internal Rectangle VisibleRect { get; set; }
        internal string SpecialInfo { get; set; }

        #endregion

        #region Constructors

        internal GraphicsInfo()
        {
        }

        internal GraphicsInfo(Graphics g)
        {
            Data = g.ToBitmap(false);
            Elements = g.Transform.Elements;
            GraphicsState state = g.Save();
            g.Transform = new Matrix();
            g.PageUnit = GraphicsUnit.Pixel;
            Rectangle bounds = Rectangle.Truncate(g.VisibleClipBounds);
            g.Restore(state);
            VisibleRect = bounds;

            using (Matrix m = g.Transform)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("World transformation: ");
                if (m.IsIdentity)
                    sb.Append("None (Identity Matrix)");
                else
                {
                    // offset
                    PointF offset = new PointF(m.OffsetX, m.OffsetY);
                    if (offset != PointF.Empty)
                        sb.AppendFormat("Offset: {0}; ", offset);
                    float[] elements = m.Elements;

                    // when there is rotation, the angle/zoom is mixed so displaying them together
                    if (!elements[1].Equals(0f) || !elements[2].Equals(0f))
                        sb.AppendFormat("Rotation and zoom matrix: [{0}; {1}] [{2}; {3}]", elements[0], elements[1], elements[2], elements[3]);
                    else if (elements[0].Equals(elements[3]))
                        sb.AppendFormat("Zoom: {0}", elements[0]);
                    else
                        sb.AppendFormat("Horizontal zoom: {0}; Vertical zoom: {1}", elements[0], elements[3]);
                }

                sb.AppendLine();
                string isTransformed = m.IsIdentity ? String.Empty : "Transformed ";
                sb.Append($"{isTransformed}Clip Bounds: {g.ClipBounds}{Environment.NewLine}"
                        + $"{isTransformed}Visible Clip Bounds (Unit = {g.PageUnit}): {g.VisibleClipBounds}{Environment.NewLine}"
                        + $"Resolution: {g.DpiX}x{g.DpiY} DPI{Environment.NewLine}"
                        + $"Page Scale: {g.PageScale}{Environment.NewLine}"
                        + $"Compositing Mode: {g.CompositingMode}{Environment.NewLine}"
                        + $"Compositing Quality: {g.CompositingQuality}{Environment.NewLine}"
                        + $"Interpolation Mode: {g.InterpolationMode}{Environment.NewLine}"
                        + $"Pixel Offset Mode: {g.PixelOffsetMode}{Environment.NewLine}"
                        + $"Smoothing Mode: {g.SmoothingMode}{Environment.NewLine}"
                        + $"Text Rendering Hint: {g.TextRenderingHint}{Environment.NewLine}"
                        + $"Text Contrast: {g.TextContrast}{Environment.NewLine}"
                        + $"Rendering Origin: {g.RenderingOrigin}");
                SpecialInfo = sb.ToString();
            }

        }

        #endregion

        #region Methods

        public void Dispose() => Data?.Dispose();

        #endregion
    }
}
