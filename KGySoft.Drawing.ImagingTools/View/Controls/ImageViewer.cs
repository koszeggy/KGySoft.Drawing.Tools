#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageViewer.cs
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
using System.Drawing.Imaging;
using System.Windows.Forms;
using KGySoft.CoreLibraries;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    /// <summary>
    /// Represents an image display control with zooming. Does not support implicit animation.
    /// </summary>
    internal partial class ImageViewer : Panel
    {
        #region Constants

        private const int WS_BORDER = 0x00800000;

        #endregion

        #region Fields

        private Image image;
        //private Bitmap previewImage;
        private Rectangle destRectangle;
        private Rectangle sourceRectangle;
        private bool antiAliasing;
        private bool autoZoom;
        private float zoom = 1;
        private Size scrollbarSize;

        #endregion

        #region Properties

        #region Internal Properties

        internal Image Image
        {
            get => image;
            set
            {
                if (image == value)
                    return;
                image = value;
                InvalidateImage();
            }
        }

        internal bool AutoZoom
        {
            get => autoZoom;
            set
            {
                if (autoZoom == value)
                    return;
                autoZoom = value;
                if (!autoZoom)
                    zoom = 1;
                InvalidateImage();
            }
        }

        internal bool AntiAliasing
        {
            get => antiAliasing;
            set
            {
                if (antiAliasing == value)
                    return;
                antiAliasing = value;
                InvalidateImage();
            }
        }

        #endregion

        #region Protected Properties

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;

                // Fixed single border
                cp.Style |= WS_BORDER;
                return cp;
            }
        }

        #endregion

        #endregion

        #region Constructors

        internal ImageViewer()
        {
            InitializeComponent();
            SetStyle(ControlStyles.Selectable | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            scrollbarSize = new Size(SystemInformation.VerticalScrollBarWidth, SystemInformation.HorizontalScrollBarHeight);
            vScrollBar.Width = scrollbarSize.Width;
            hScrollBar.Height = scrollbarSize.Height;
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            // TODO: when !autoZoom, only previewRectangle has to be changed
            InvalidateImage();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (image == null || e.ClipRectangle.Width <= 0 || e.ClipRectangle.Height <= 0)
                return;

            if (destRectangle.IsEmpty)
                AdjustSizes();
            if (!destRectangle.IsEmpty)
                PaintImage(e.Graphics);

            //if (previewImage == null)
            //{
            //    GeneratePreview();
            //    if (previewRectangle.IsEmpty)
            //        return;
            //}

            //e.Graphics.DrawImage(previewImage, previewRectangle);


        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (ModifierKeys == Keys.Control)
            {
                if (autoZoom)
                    return;
                float delta = (float)e.Delta / SystemInformation.MouseWheelScrollDelta / 5;
                Zoom(delta);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
            {
                components?.Dispose();
                //previewImage?.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        [Obsolete]
        private void InvalidateImage()
        {
            //previewImage?.Dispose();
            //previewImage = null;
            destRectangle = default;
            Invalidate();
        }

        private void AdjustSizes()
        {
            Size clientSize = ClientSize;
            if (clientSize.Width < 1 || clientSize.Height < 1)
            {
                destRectangle = Rectangle.Empty;
                return;
            }

            Size sourceSize = image.Size;
            Rectangle sourceRect = new Rectangle(Point.Empty, sourceSize);
            Size targetSize = autoZoom ? clientSize : sourceSize.Scale(new PointF(zoom, zoom));

            float ratio = Math.Min((float)targetSize.Width / sourceSize.Width, (float)targetSize.Height / sourceSize.Height);
            targetSize = new Size((int)(sourceSize.Width * ratio), (int)(sourceSize.Height * ratio));
            var targetLocation = new Point(Math.Max(0, (clientSize.Width >> 1) - (targetSize.Width >> 1)),
                Math.Max(0, (clientSize.Height >> 1) - (targetSize.Height >> 1)));

            if (autoZoom || targetSize.Width <= clientSize.Width && targetSize.Height <= clientSize.Height)
            {
                hScrollBar.Visible = vScrollBar.Visible = false;
                sourceRectangle = sourceRect;
            }
            else
            {
                if (targetSize.Width > clientSize.Width - scrollbarSize.Width
                    && targetSize.Height > clientSize.Height - scrollbarSize.Height)
                {
                    // both scrollbars
                    hScrollBar.Dock = vScrollBar.Dock = DockStyle.None;
                    hScrollBar.Width = clientSize.Width - scrollbarSize.Width;
                    hScrollBar.Top = clientSize.Height - scrollbarSize.Height;
                    vScrollBar.Height = clientSize.Height - scrollbarSize.Height;
                    vScrollBar.Left = clientSize.Width - scrollbarSize.Width;
                    hScrollBar.Visible = vScrollBar.Visible = true;
                    targetSize.Width = Math.Min(targetSize.Width, clientSize.Width - scrollbarSize.Width);
                    targetSize.Height = Math.Min(targetSize.Height, clientSize.Height - scrollbarSize.Height);
                    //sourceRectangle.Size = targetSize; // TODO: scale!
                }
                else if (targetSize.Width > clientSize.Width)
                {
                    // horizontal scrollbar
                    hScrollBar.Dock = DockStyle.Bottom;
                    hScrollBar.Visible = true;
                    vScrollBar.Visible = false;
                    targetSize.Height = Math.Min(targetSize.Height, clientSize.Height - scrollbarSize.Height);
                   // sourceRectangle.Height = targetSize.Height;
                }
                else // if (targetSize.Height < clientSize.Height)
                {
                    // vertical scrollbar
                    vScrollBar.Dock = DockStyle.Right;
                    vScrollBar.Visible = true;
                    hScrollBar.Visible = false;
                    targetSize.Width = Math.Min(targetSize.Width, clientSize.Width - scrollbarSize.Width);
                    //sourceRectangle.Width = targetSize.Width;
                }

                sourceRectangle.Size = targetSize; // TODO: scale!

            }

            destRectangle = new Rectangle(targetLocation, targetSize);
        }

        private void PaintImage(Graphics g)
        {
            g.DrawImage(image, destRectangle, sourceRectangle, GraphicsUnit.Pixel);
        }

        //private void GeneratePreview()
        //{
        //    Rectangle clientRect = ClientRectangle;
        //    if (clientRect.Width < 1 || clientRect.Height < 1)
        //    {
        //        previewRectangle = Rectangle.Empty;
        //        return;
        //    }

        //    Size sourceSize = image.Size;
        //    Rectangle sourceRect = new Rectangle(Point.Empty, sourceSize);
        //    Size targetSize = autoZoom ? clientRect.Size : sourceSize.Scale(new PointF(zoom, zoom));

        //    float ratio = Math.Min((float)targetSize.Width / sourceSize.Width, (float)targetSize.Height / sourceSize.Height);
        //    targetSize = new Size((int)(sourceSize.Width * ratio), (int)(sourceSize.Height * ratio));
        //    Point targetLocation = new Point((clientRect.Width >> 1) - (targetSize.Width >> 1), (clientRect.Height >> 1) - (targetSize.Height >> 1));

        //    previewRectangle = new Rectangle(targetLocation, targetSize);
        //    previewImage = image switch
        //    {
        //        Bitmap bmp => ResizeBitmap(bmp, targetSize),
        //        Metafile metafile => metafile.ToBitmap(targetSize, antiAliasing),
        //        _ => throw new InvalidOperationException(Res.InternalError($"Unexpected image type: {image.GetType()}"))
        //    };
        //}

        //private Bitmap ResizeBitmap(Bitmap bmp, Size targetSize)
        //{
        //    var result = new Bitmap(targetSize.Width, targetSize.Height);
        //    Size origSize = bmp.Size;
        //    using (Graphics graphics = Graphics.FromImage(result))
        //    {
        //        graphics.InterpolationMode = antiAliasing || targetSize.Width < origSize.Width ? InterpolationMode.HighQualityBicubic : InterpolationMode.NearestNeighbor;
        //        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        //        graphics.DrawImage(bmp, new Rectangle(Point.Empty, targetSize), new Rectangle(Point.Empty, origSize), GraphicsUnit.Pixel);
        //        graphics.Flush();
        //        return result;
        //    }

        //}

        private void Zoom(float delta)
        {
            if (delta == 0f)
                return;
            delta += 1;
            float newValue = zoom * delta;
            if (newValue < 0.1f)
                newValue = 0.1f;
            if (newValue > 10f)
                newValue = 10f;
            if (zoom == newValue)
                return;
            zoom = newValue;
            InvalidateImage();
        }

        #endregion

        #endregion
    }
}
