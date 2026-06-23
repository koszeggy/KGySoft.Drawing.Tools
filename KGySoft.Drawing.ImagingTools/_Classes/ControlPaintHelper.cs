#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ControlPaintHelper.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2026 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

using KGySoft.Collections;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.Shapes;
using KGySoft.WinForms;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal static class ControlPaintHelper
    {
        #region Enums

        [Flags]
        private enum ControlElement
        {
            ArrowLeft = ArrowDirection.Left, // 0
            ArrowRight = ArrowDirection.Right, // 0x10
            ArrowUp = ArrowDirection.Up, // 0 | IsVertical
            ArrowDown = ArrowDirection.Down, // 0x10 | IsVertical

            OverflowArrowLeft = ArrowLeft | HasBaseLine,
            OverflowArrowRight = ArrowRight | HasBaseLine,
            OverflowArrowDown = ArrowDown | HasBaseLine,

            // Due to how ArrowDirection + Orientation handle it, modifiers are on the 8 LSB bits
            IsVertical = Orientation.Vertical,
            HasBaseLine = 1 << IsVertical,
        }

        #endregion

        #region Fields

        private static readonly LockingDictionary<(Size, Image), Bitmap> disabledImagesCache = new Cache<(Size, Image), Bitmap>(GenerateDisabledImage, 16)
        {
            DisposeDroppedValues = true
        }.AsThreadSafe();

        // Need to use a locking cache to be able to use DisposeDroppedValues, but it shouldn't be an issue as we don't expect many concurrent UI threads.
        // Contains black drawings on a transparent background. Use GraphicsExtensions.DrawImageColorized to paint the result with a custom color.
        // Though we could use indexed bitmaps, the pixel format is 32 bpp PArgb for the best rendering performance.
        private static readonly IThreadSafeCacheAccessor<(ControlElement, int), Bitmap> bitmapsCache = new Cache<(ControlElement, int), Bitmap>(GetBitmap, 8)
        {
            EnsureCapacity = true,
            DisposeDroppedValues = true
        }.GetThreadSafeAccessor();

        #endregion

        #region Constructors

        static ControlPaintHelper()
        {
            if (OSHelper.IsWindows10OrLater)
            {
                ThemeColors.ThemeChanged += (_, _) =>
                {
                    ICollection<Bitmap> toDispose = disabledImagesCache.Values;
                    disabledImagesCache.Clear();
                    foreach (Bitmap image in toDispose)
                        image.Dispose();
                };
            }
        }

        #endregion

        #region Methods

        #region Internal Methods

        internal static Bitmap GetDisabledImage(Image image, Size size) => disabledImagesCache[(size, image)];

        internal static Bitmap GetArrowImage(ArrowDirection direction, int arrowSize) => bitmapsCache[((ControlElement)direction, arrowSize)];
        internal static Bitmap GetOverflowArrowImage(ArrowDirection direction, int arrowSize) => bitmapsCache[((ControlElement)direction | ControlElement.HasBaseLine, arrowSize)];

        #endregion

        #region Private Methods

        private static Bitmap GenerateDisabledImage((Size, Image) key)
        {
            (Size boundsSize, Image image) = key;
            if (image.RawFormat.Equals(ImageFormat.Icon) && image.Size != boundsSize)
            {
                // Icons only: if the desired size does not match, drawing the icon into a temp bitmap of the desired size first.
                // If the icon has multiple resolutions, it may change the value of its Size property. We can discard the temp bitmap immediately.
                using var resized = new Bitmap(image, boundsSize);
            }

            // when system dark mode is enabled, this returns a way too dark/faint image in .NET 9+
            //return ToolStripRenderer.CreateDisabledImage(image);

            var result = new Bitmap(image); // TODO: to AsBitmap if this will be migrated to WinForms and Image can potentially be a metafile
            using IReadWriteBitmapData bitmapData = result.GetReadWriteBitmapData(); // sRGB color space is alright for grayscale transformations
            bitmapData.MakeGrayscale();
            bitmapData.AdjustBrightness(ThemeColors.IsDarkBaseTheme ? 0.1f : -0.1f); // darker for light theme and vice versa, so white/black becomes a bit grayer
            bitmapData.TransformColors(c => Color32.FromArgb((byte)(c.A * 0.5f), c));
            return result;
        }

        private static Bitmap GetBitmap((ControlElement Element, int size) key) => key.Element switch
        {
            ControlElement.ArrowUp or ControlElement.ArrowDown => GetVerticalArrowBitmap(key.Element == ControlElement.ArrowDown, key.size),
            ControlElement.ArrowLeft or ControlElement.ArrowRight => GetHorizontalArrowBitmap(key.Element == ControlElement.ArrowRight, key.size),
            ControlElement.OverflowArrowDown => GetOverflowArrowDownBitmap(key.size),
            ControlElement.OverflowArrowLeft or ControlElement.OverflowArrowRight => GetHorizontalOverflowArrowBitmap(key.Element == ControlElement.OverflowArrowRight, key.size),
            _ => throw new InvalidOperationException(Res.InternalError($"Unexpected element: {key.Element}"))
        };

        private static Bitmap GetVerticalArrowBitmap(bool isDown, int height)
        {
            var result = new Bitmap((height << 1) - 1, height, PixelFormat.Format32bppPArgb);
            using IReadWriteBitmapData bmpData = result.GetReadWriteBitmapData();
            DrawVerticalArrow(bmpData, height, isDown);
            return result;
        }

        private static Bitmap GetHorizontalArrowBitmap(bool isRight, int width)
        {
            var result = new Bitmap(width, (width << 1) - 1, PixelFormat.Format32bppPArgb);
            using IReadWriteBitmapData bmpData = result.GetReadWriteBitmapData();
            DrawHorizontalArrow(bmpData, width, isRight);
            return result;
        }

        private static Bitmap GetOverflowArrowDownBitmap(int arrowHeight)
        {
            var result = new Bitmap((arrowHeight << 1) - 1, arrowHeight << 1, PixelFormat.Format32bppPArgb);
            using IReadWriteBitmapData bmpData = result.GetReadWriteBitmapData();
            DrawVerticalArrow(bmpData.Clip(new Rectangle(0, arrowHeight, bmpData.Width, arrowHeight)), arrowHeight, true);
            bmpData.DrawLine(Color.Black, 0, 0, bmpData.Width, 0);
            return result;
        }

        private static Bitmap GetHorizontalOverflowArrowBitmap(bool isRight, int arrowWidth)
        {
            var result = new Bitmap(arrowWidth << 1, (arrowWidth << 1) - 1, PixelFormat.Format32bppPArgb);
            using IReadWriteBitmapData bmpData = result.GetReadWriteBitmapData();
            Rectangle arrowRect = isRight
                ? new Rectangle(arrowWidth, 0, arrowWidth, bmpData.Height)
                : new Rectangle(0, 0, arrowWidth, bmpData.Height);
            DrawHorizontalArrow(bmpData.Clip(arrowRect), arrowWidth, isRight);
            if (isRight)
                bmpData.DrawLine(Color.Black, 0, 0, 0, bmpData.Height);
            else
                bmpData.DrawLine(Color.Black, bmpData.Width - 1, 0, bmpData.Width - 1, bmpData.Height);
            return result;
        }

        private static void DrawVerticalArrow(IReadWriteBitmapData bitmapData, int height, bool isDown)
        {
            int width = (height << 1) - 1;
            Color32 black = Color.Black;
            for (int i = 0; i < height; i++)
            {
                int y = isDown ? i : height - i - 1;
                bitmapData.DrawLine(black, i, y, width - i - 1, y);
            }
        }

        private static void DrawHorizontalArrow(IReadWriteBitmapData bitmapData, int width, bool isRight)
        {
            int height = (width << 1) - 1;
            Color32 black = Color.Black;
            for (int i = 0; i < width; i++)
            {
                int x = isRight ? i : width - i - 1;
                bitmapData.DrawLine(black, x, i, x, height - i - 1);
            }
        }

        #endregion

        #endregion
    }
}
