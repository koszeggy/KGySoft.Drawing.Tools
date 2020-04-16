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

namespace KGySoft.Drawing.ImagingTools.Model
{
    public sealed class GraphicsInfo : IDisposable
    {
        #region Properties

        public Bitmap GraphicsImage { get; set; }
        public Matrix Transform { get; set; }
        public Rectangle OriginalVisibleClipBounds { get; set; }
        public RectangleF TransformedVisibleClipBounds { get; set; }
        public GraphicsUnit PageUnit { get; set; }
        public PointF Resolution { get; set; }

        #endregion

        #region Constructors

        public GraphicsInfo()
        {
        }

        public GraphicsInfo(Graphics g)
        {
            GraphicsImage = g.ToBitmap(false);
            TransformedVisibleClipBounds = g.VisibleClipBounds;
            Transform = g.Transform;
            Resolution = new PointF(g.DpiX, g.DpiY);
            PageUnit = g.PageUnit;

            GraphicsState state = g.Save();
            g.Transform = new Matrix();
            g.PageUnit = GraphicsUnit.Pixel;
            OriginalVisibleClipBounds = Rectangle.Truncate(g.VisibleClipBounds);
            g.Restore(state);
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            GraphicsImage?.Dispose();
            Transform?.Dispose();
        }

        #endregion
    }
}
