#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageExtensions.cs
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
using System.Drawing;
using System.Drawing.Drawing2D;

using KGySoft.WinForms;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal static class ImageExtensions
    {
        #region Constants

        private const int maxConvertedSize = 8192;
        private const int maxDownscaleAttempts = 4;

        #endregion

        #region Methods

        internal static Bitmap AsBitmap(this Image image)
        {
            if (image is Bitmap bitmap)
                return bitmap;

            Size origSize = image.Size;
            int max = Math.Max(origSize.Width, origSize.Height);
            float scale = max <= maxConvertedSize ? 1f : (float)maxConvertedSize / max;
            for (int i = 0; i < maxDownscaleAttempts; i++)
            {
                try
                {
                    var size = origSize.Scale(scale);
                    if (size.Width < 1)
                        size.Width = 1;
                    if (size.Height < 1)
                        size.Height = 1;

                    // NOTE: not using the image drawing constructor here, because it uses bilinear interpolation,
                    //       which may cause ugly black edges for bitmap drawing records in case of legacy GDI metafile types.
                    // Interpolation mode must always be NN here. Matters when the metafile contains image drawing records, and the metafile type is WMF or EmfOnly.
                    // In this case the resized result with interpolation may cause ugly black contours at transparent edges.
                    var result = new Bitmap(size.Width, size.Height);
                    using var g = Graphics.FromImage(result);
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.DrawImage(image, new Rectangle(Point.Empty, size));

                    return result;
                }
                catch (Exception e) when (i < maxDownscaleAttempts - 1 && !e.IsCriticalGdi())
                {
                    scale /= 2f;
                }
            }

            throw new InvalidOperationException(); // never reached
        }

        #endregion
    }
}