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
    /// <summary>
    /// Represents a descriptor for a <see cref="Graphics"/> instance that can be used
    /// to display arbitrary debug information.
    /// </summary>
    public sealed class GraphicsInfo : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets or sets a <see cref="Bitmap"/> that represents the content of the corresponding <see cref="Graphics"/>.
        /// </summary>
        public Bitmap GraphicsImage { get; set; }

        /// <summary>
        /// Gets or sets the transformation of the corresponding <see cref="Graphics"/>.
        /// </summary>
        public Matrix Transform { get; set; }

        /// <summary>
        /// Gets or sets the original visible clip bounds in pixels, without applying any transformation.
        /// </summary>
        public Rectangle OriginalVisibleClipBounds { get; set; }

        /// <summary>
        /// Gets or sets the visible clip bounds in <see cref="GraphicsUnit"/> represented by the <see cref="PageUnit"/> property,
        /// after applying the transformations.
        /// </summary>
        public RectangleF TransformedVisibleClipBounds { get; set; }

        /// <summary>
        /// Gets or sets the page unit applies for the <see cref="TransformedVisibleClipBounds"/> property.
        /// </summary>
        public GraphicsUnit PageUnit { get; set; }

        /// <summary>
        /// Gets or sets the displayed resolution of the corresponding <see cref="Graphics"/> instance.
        /// </summary>
        public PointF Resolution { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes an empty instance of the <see cref="GraphicsInfo"/> class.
        /// The properties are expected to be initialized individually.
        /// </summary>
        public GraphicsInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicsInfo"/> class from a <see cref="Graphics"/> instance.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> instance to be used to initialize the created instance from.</param>
        public GraphicsInfo(Graphics g)
        {
            if (g == null)
                throw new ArgumentNullException(nameof(g), PublicResources.ArgumentNull);
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

        /// <summary>
        /// Releases the resources held by this instance.
        /// </summary>
        public void Dispose()
        {
            GraphicsImage?.Dispose();
            Transform?.Dispose();
        }

        #endregion
    }
}
