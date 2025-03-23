﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageSourceSerializationInfo.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2025 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using KGySoft.Drawing.DebuggerVisualizers.Serialization;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Drawing.Wpf;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Wpf.Serialization
{
    internal sealed class ImageSourceSerializationInfo : CustomBitmapSerializationInfoBase
    {
        #region Fields

        private static int? systemDpi;

        #endregion

        #region Properties

        private static int SystemDpi
        {
            get
            {
                if (!systemDpi.HasValue)
                {
                    // Actually there are DpiX and Dpi properties for X/Y sizes but SystemParameters.PrimaryScreenWidth/Height both use Dpi.
                    PropertyInfo? dpiProp = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    systemDpi = dpiProp?.GetValue(null, null) is int dpi ? dpi : 96;
                }

                return systemDpi.Value;
            }
        }

        #endregion

        #region Constructors

        internal ImageSourceSerializationInfo(ImageSource image)
        {
            BitmapSource bitmapSource = ToBitmapSource(image);
            BackingObject = bitmapSource;
            BitmapInfo = new CustomBitmapInfo(true)
            {
                Type = image.GetType().Name,
                BitmapData = bitmapSource.GetReadableBitmapData(),
                CustomAttributes =
                {
                    { nameof(image.Width), $"{image.Width:#.##}" },
                    { nameof(image.Height), $"{image.Height:#.##}" },
                }
            };
        }

        internal ImageSourceSerializationInfo(BitmapSource bitmap)
        {
            BackingObject = bitmap;
            BitmapInfo = new CustomBitmapInfo(true)
            {
                ShowPixelSize = true,
                Type = bitmap.GetType().Name,
                BitmapData = bitmap.GetReadableBitmapData(),
                CustomPalette = BitmapPaletteSerializationInfo.GetPaletteInfo(bitmap.Palette),
                CustomAttributes =
                {
                    { nameof(bitmap.PixelWidth), $"{bitmap.PixelWidth}" },
                    { nameof(bitmap.PixelHeight), $"{bitmap.PixelHeight}" },
                    { nameof(bitmap.Format), bitmap.Format.ToString() },
                    { nameof(bitmap.DpiX), $"{bitmap.DpiX:#.##}" },
                    { nameof(bitmap.DpiY), $"{bitmap.DpiY:#.##}" },
                }
            };

            if (bitmap.Palette != null)
                BitmapInfo.CustomAttributes[$"{nameof(bitmap.Palette)}.{nameof(bitmap.Palette.Colors)}.{nameof(bitmap.Palette.Colors.Count)}"] = $"{bitmap.Palette.Colors.Count}";
        }

        internal ImageSourceSerializationInfo(BinaryReader reader)
            : base(reader)
        {
        }

        #endregion

        #region Methods

        private static BitmapSource ToBitmapSource(ImageSource image)
        {
            double sourceWidth = image.Width;
            double sourceHeight = image.Height;
            var visual = new DrawingVisual();
            using (DrawingContext context = visual.RenderOpen())
                context.DrawImage(image, new Rect(0, 0, sourceWidth, sourceHeight));

            // Going for screen size or image size, whichever is bigger, but up to 2x screen size
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double targetWidth = Math.Min(Math.Max(sourceWidth, screenWidth), screenWidth * 2);
            double targetHeight = Math.Min(Math.Max(sourceHeight, screenHeight), screenHeight * 2);
            double ratio = Math.Min(targetWidth / sourceWidth, targetHeight / sourceHeight);
            targetWidth = sourceWidth * ratio;
            targetHeight = sourceHeight * ratio;

            int dpi = SystemDpi;
            var bitmap = new RenderTargetBitmap((int)(targetWidth * dpi / 96), (int)(targetHeight * dpi / 96),
                dpi * targetWidth / sourceWidth, dpi * targetHeight / sourceHeight, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            return bitmap;
        }

        #endregion
    }
}
