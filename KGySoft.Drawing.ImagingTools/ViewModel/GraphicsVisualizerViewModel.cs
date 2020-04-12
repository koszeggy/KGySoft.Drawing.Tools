#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: GraphicsVisualizerViewModel.cs
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

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class GraphicsVisualizerViewModel : ImageVisualizerViewModel
    {
        #region Fields

        private bool viewInitialized;

        #endregion

        #region Properties

        internal Bitmap GraphicsImage { get => Get<Bitmap>(); set => Set(value); }
        internal Rectangle VisibleRect { get => Get<Rectangle>(); set => Set(value); }
        internal Matrix Transform { get => Get<Matrix>(); set => Set(value); }
        internal bool Crop { get => Get<bool>(); set => Set(value); }
        internal bool HighlightVisibleClip { get => Get<bool>(true); set => Set(value); }
        internal Action<Graphics, Rectangle> DrawFocusRectangleCallback { get => Get<Action<Graphics, Rectangle>>(); set => Set(value); }

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
            if (e.PropertyName.In(nameof(VisibleRect), nameof(GraphicsImage)))
                UpdateImageAndCommands();
        }

        internal override void ViewLoaded()
        {
            viewInitialized = true;
            UpdateGraphicImage();
            base.ViewLoaded();
        }

        protected override void UpdateInfo()
        {
            var transform = Transform;
            if (transform == null || InfoText == null)
                return;

            TitleCaption = $"{Res.TitleType(nameof(Graphics))}; {(transform.IsIdentity ? Res.TitleVisibleClip(VisibleRect) : Res.TitleUntransformedVisibleClip(VisibleRect))}";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Transform?.Dispose();
                GraphicsImage?.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void UpdateImageAndCommands()
        {
            var visibleRect = VisibleRect;
            UpdateGraphicImage();
            var backingImage = GraphicsImage;
            bool commandsEnabled = backingImage != null && (backingImage.Size != visibleRect.Size || visibleRect.Location != Point.Empty);
            CropCommandState.Enabled = HighlightVisibleClipCommandState.Enabled = commandsEnabled;
        }

        private void UpdateGraphicImage()
        {
            if (!viewInitialized)
                return;
            var backingImage = GraphicsImage;
            if (backingImage == null)
                return;

            Rectangle visibleRect = VisibleRect;
            if (Crop && (visibleRect.Size != backingImage.Size || visibleRect.Location != Point.Empty))
            {
                if (visibleRect.Width <= 0 || visibleRect.Height <= 0)
                    return;

                Bitmap newImage = new Bitmap(visibleRect.Width, visibleRect.Height);
                using (Graphics g = Graphics.FromImage(newImage))
                    g.DrawImage(backingImage, new Rectangle(Point.Empty, visibleRect.Size), visibleRect, GraphicsUnit.Pixel);

                Image = newImage;
                return;
            }

            if (HighlightVisibleClip && (visibleRect.Size != backingImage.Size || visibleRect.Location != Point.Empty))
            {
                Bitmap newImage = new Bitmap(backingImage);
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
