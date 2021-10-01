#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: GraphicsVisualizerViewModel.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class GraphicsVisualizerViewModel : ImageVisualizerViewModel
    {
        #region Properties

        internal GraphicsInfo? GraphicsInfo { get => Get<GraphicsInfo?>(); init => Set(value); }
        internal bool Crop { get => Get<bool>(); set => Set(value); }
        internal bool HighlightVisibleClip { get => Get(true); set => Set(value); }
        internal Action<Graphics, Rectangle>? DrawFocusRectangleCallback { get => Get<Action<Graphics, Rectangle>?>(); set => Set(value); }

        internal ICommandState CropCommandState => Get(() => new CommandState());
        internal ICommandState HighlightVisibleClipCommandState => Get(() => new CommandState());

        internal ICommand CropCommand => Get(() => new SimpleCommand<bool>(OnSetCropCommand));
        internal ICommand HighlightVisibleClipCommand => Get(() => new SimpleCommand<bool>(OnHighlightVisibleClipCommand));

        #endregion

        #region Constructors

        internal GraphicsVisualizerViewModel() : base(AllowedImageTypes.Bitmap)
        {
            ReadOnly = true;
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName.In(nameof(GraphicsInfo), nameof(GraphicsInfo)))
                UpdateImageAndCommands();
        }

        protected override void UpdateInfo()
        {
            GraphicsInfo? graphicsInfo = GraphicsInfo;
            Matrix? transform = graphicsInfo?.Transform;
            if (graphicsInfo?.GraphicsImage == null || transform == null)
            {
                TitleCaption = Res.TitleNoImage;
                InfoText = null;
                return;
            }

            bool isTransformed = !transform.IsIdentity;
            TitleCaption = $"{Res.TitleType(nameof(Graphics))}{Res.TextSeparator}{(isTransformed ? Res.TitleOriginalVisibleClip(graphicsInfo.OriginalVisibleClipBounds) : Res.TitleVisibleClip(graphicsInfo.OriginalVisibleClipBounds))}";
            var sb = new StringBuilder();
            sb.Append(Res.InfoWorldTransformation);
            if (transform.IsIdentity)
                sb.Append(Res.InfoNoTransformation);
            else
            {
                // offset
                PointF offset = new PointF(transform.OffsetX, transform.OffsetY);
                if (offset != PointF.Empty)
                    sb.Append($"{Res.InfoTransformOffset(offset)}{Res.TextSeparator}");
                float[] elements = transform.Elements;

                // when there is rotation, the angle/zoom is mixed so displaying them together
                if (!elements[1].Equals(0f) || !elements[2].Equals(0f))
                    sb.Append(Res.InfoRotationZoom(elements[0], elements[1], elements[2], elements[3]));
                else if (elements[0].Equals(elements[3]))
                    sb.Append(Res.InfoZoom(elements[0]));
                else
                    sb.Append($"{Res.InfoHorizontalZoom(elements[0])}{Res.TextSeparator}{Res.InfoVerticalZoom(elements[1])}");
            }

            sb.AppendLine();
            if (isTransformed
                || graphicsInfo.OriginalVisibleClipBounds != graphicsInfo.TransformedVisibleClipBounds)
            {
                sb.AppendLine(Res.InfoOriginalVisibleClip(graphicsInfo.OriginalVisibleClipBounds));
                sb.AppendLine(Res.InfoTransformedVisibleClip(graphicsInfo.TransformedVisibleClipBounds, graphicsInfo.PageUnit));
            }
            else
                sb.AppendLine(Res.InfoVisibleClip(graphicsInfo.OriginalVisibleClipBounds));

            sb.Append(Res.InfoResolution(graphicsInfo.Resolution));
            InfoText = sb.ToString();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                GraphicsInfo?.Dispose();

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void UpdateImageAndCommands()
        {
            UpdateGraphicImage();
            GraphicsInfo? graphicsInfo = GraphicsInfo;
            Bitmap? backingImage = graphicsInfo?.GraphicsImage;
            bool commandsEnabled = backingImage != null && (backingImage.Size != graphicsInfo!.OriginalVisibleClipBounds.Size || graphicsInfo.OriginalVisibleClipBounds.Location != Point.Empty);
            CropCommandState.Enabled = HighlightVisibleClipCommandState.Enabled = commandsEnabled;
        }

        private void UpdateGraphicImage()
        {
            GraphicsInfo? graphicsInfo = GraphicsInfo;
            Bitmap? backingImage = graphicsInfo?.GraphicsImage;
            if (backingImage == null)
                return;

            Rectangle visibleRect = graphicsInfo!.OriginalVisibleClipBounds;
            if (Crop && (visibleRect.Size != backingImage.Size || visibleRect.Location != Point.Empty))
            {
                if (visibleRect.Width <= 0 || visibleRect.Height <= 0)
                    return;

                var newImage = new Bitmap(visibleRect.Width, visibleRect.Height);
                using (Graphics g = Graphics.FromImage(newImage))
                    g.DrawImage(backingImage, new Rectangle(Point.Empty, visibleRect.Size), visibleRect, GraphicsUnit.Pixel);

                Image = newImage;
                return;
            }

            if (HighlightVisibleClip && (visibleRect.Size != backingImage.Size || visibleRect.Location != Point.Empty))
            {
                var newImage = new Bitmap(backingImage);
                using (Graphics g = Graphics.FromImage(newImage))
                {
                    using (Brush b = new SolidBrush(Color.FromArgb(128, Color.Black)))
                    {
                        g.FillRectangle(b, 0, 0, newImage.Width, visibleRect.Top);
                        g.FillRectangle(b, 0, visibleRect.Bottom, newImage.Width, newImage.Height - visibleRect.Bottom);
                        g.FillRectangle(b, 0, visibleRect.Top, visibleRect.Left, visibleRect.Height);
                        g.FillRectangle(b, visibleRect.Right, visibleRect.Top, newImage.Width - visibleRect.Height, visibleRect.Height);
                        visibleRect.Inflate(1, 1);
                        DrawFocusRectangleCallback?.Invoke(g, visibleRect);
                    }
                }

                Image = newImage;
                return;
            }

            Image = (Image)backingImage.Clone();
        }

        #endregion

        #region Command Handlers

        private void OnSetCropCommand(bool newValue)
        {
            Crop = newValue;
            HighlightVisibleClipCommandState.Enabled = !newValue;
            UpdateGraphicImage();
        }

        private void OnHighlightVisibleClipCommand(bool newValue)
        {
            HighlightVisibleClip = newValue;
            UpdateGraphicImage();
        }

        #endregion

        #endregion
    }
}
