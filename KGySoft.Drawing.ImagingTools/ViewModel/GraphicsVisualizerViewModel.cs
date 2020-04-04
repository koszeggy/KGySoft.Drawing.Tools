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

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class GraphicsVisualizerViewModel : ImageVisualizerViewModel
    {
        #region Fields

        private Image origImage;

        #endregion

        #region Properties

        internal Rectangle VisibleRect { get => Get<Rectangle>(); set => Set(value); }
        internal Matrix Transform { get => Get<Matrix>(); set => Set(value); }
        internal bool Crop { get => Get<bool>(); set => Set(value); }
        internal bool HighlightVisibleClip { get => Get<bool>(); set => Set(value); }
        internal Action<Graphics, Rectangle> DrawFocusRectangleCallback { get => Get<Action<Graphics, Rectangle>>(); set => Set(value); }

        internal override Image Image
        {
            get => base.Image;
            set
            {
                origImage = value;
                UpdateImageAndCommands();
            }
        }

        internal ICommandState CropCommandState => Get(() => new CommandState());
        internal ICommandState HighlightVisibleClipCommandState => Get(() => new CommandState());

        internal ICommand CropCommand => Get(() => new SimpleCommand<bool>(OnSetCropCommand));
        internal ICommand HighlightVisibleClipCommand => Get(() => new SimpleCommand<bool>(OnHighlightVisibleClipCommand));

        #endregion

        #region Constructors

        internal GraphicsVisualizerViewModel() : base(AllowedImageTypes.None)
        {
            ReadOnly = true;
            OpenFileCommandState.Enabled = false;
            ClearCommandState.Enabled = false;
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == nameof(VisibleRect))
                UpdateImageAndCommands();
        }

        internal override void ViewCreated()
        {
            HighlightVisibleClip = true;
            base.ViewCreated();
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
                origImage?.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void UpdateImageAndCommands()
        {
            var visibleRect = VisibleRect;
            UpdateGraphicImage();
            bool commandsEnabled = origImage != null && (origImage.Size != visibleRect.Size || visibleRect.Location != Point.Empty);
            CropCommandState.Enabled = HighlightVisibleClipCommandState.Enabled = commandsEnabled;
        }

        private void UpdateGraphicImage()
        {
            Image oldImage = base.Image;
            if (oldImage != null)
            {
                oldImage.Dispose();
                base.Image = null;
            }

            if (origImage == null)
                return;

            Rectangle visibleRect = VisibleRect;
            if (Crop && (visibleRect.Size != origImage.Size || visibleRect.Location != Point.Empty))
            {
                if (visibleRect.Width <= 0 || visibleRect.Height <= 0)
                    return;

                Bitmap newImage = new Bitmap(visibleRect.Width, visibleRect.Height);
                using (Graphics g = Graphics.FromImage(newImage))
                    g.DrawImage(origImage, new Rectangle(Point.Empty, visibleRect.Size), visibleRect, GraphicsUnit.Pixel);

                base.Image = newImage;
                return;
            }

            if (HighlightVisibleClip && (visibleRect.Size != origImage.Size || visibleRect.Location != Point.Empty))
            {
                Bitmap newImage = new Bitmap(origImage);
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

                base.Image = newImage;
                return;
            }

            base.Image = (Image)origImage.Clone();
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
