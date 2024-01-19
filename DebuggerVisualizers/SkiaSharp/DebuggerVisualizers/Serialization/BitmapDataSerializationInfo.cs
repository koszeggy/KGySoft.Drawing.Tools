#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: BitmapDataSerializationInfo.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2024 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System.Collections.Generic;
using System.IO;

using KGySoft.Drawing.DebuggerVisualizers.Serialization;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Drawing.SkiaSharp;

using SkiaSharp;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.SkiaSharp.Serialization
{
    internal sealed class BitmapDataSerializationInfo : CustomBitmapSerializationInfoBase
    {
        #region Constructors

        internal BitmapDataSerializationInfo(SKBitmap bitmap)
        {
            BackingObject = bitmap;
            BitmapInfo = new CustomBitmapInfo(true)
            {
                Type = bitmap.GetType().Name,
                BitmapData = bitmap.GetReadableBitmapData(),
            };
            PopulateCustomAttributes(BitmapInfo.CustomAttributes, bitmap.Info);
        }

        internal BitmapDataSerializationInfo(SKPixmap pixmap)
        {
            BackingObject = pixmap;
            BitmapInfo = new CustomBitmapInfo(true)
            {
                Type = pixmap.GetType().Name,
                BitmapData = pixmap.GetReadableBitmapData(),
            };
            PopulateCustomAttributes(BitmapInfo.CustomAttributes, pixmap.Info);
        }

        internal BitmapDataSerializationInfo(SKImage image)
        {
            BackingObject = image;
            BitmapInfo = new CustomBitmapInfo(true)
            {
                Type = image.GetType().Name,
                BitmapData = image.GetReadableBitmapData(),
            };
            PopulateCustomAttributes(BitmapInfo.CustomAttributes, image.Info);
        }

        internal BitmapDataSerializationInfo(SKSurface surface)
        {
            using var canvas = surface.Canvas;
            BackingObject = surface;

            BitmapInfo = new CustomBitmapInfo(true)
            {
                Type = surface.GetType().Name,
                BitmapData = surface.GetReadableBitmapData(),
            };

            // TODO: when https://github.com/mono/SkiaSharp/issues/2281 will be fixed: (until we could use Snapshot but it's better sparing with the allocations)
            PopulateCustomAttributes(BitmapInfo.CustomAttributes, surface.PeekPixels()?.Info ?? SKImageInfo.Empty);

            BitmapInfo.CustomAttributes[nameof(surface.Context.Backend)] = surface.Context?.Backend.ToString() ?? "Cpu";
            BitmapInfo.CustomAttributes[nameof(canvas.DeviceClipBounds)] = $"{canvas.DeviceClipBounds}";
            BitmapInfo.CustomAttributes[nameof(canvas.LocalClipBounds)] = $"{canvas.LocalClipBounds}";
        }

        internal BitmapDataSerializationInfo(BinaryReader reader)
            : base(reader)
        {
        }

        #endregion

        #region Methods

        private static void PopulateCustomAttributes(IDictionary<string, string> attributes, SKImageInfo info)
        {
            if (info.IsEmpty)
                return;

            attributes[nameof(info.Width)] = $"{info.Width}";
            attributes[nameof(info.Height)] = $"{info.Height}";
            attributes[nameof(info.ColorType)] = $"{info.ColorType}";
            attributes[nameof(info.AlphaType)] = $"{info.AlphaType}";
            attributes[nameof(info.ColorSpace)] = info.ColorSpace switch
            {
                null => "null",
                { IsSrgb: true } => "Srgb",
                { GammaIsLinear: true } => "Linear",
                _ => "Custom"
            };
        }

        #endregion
    }
}
