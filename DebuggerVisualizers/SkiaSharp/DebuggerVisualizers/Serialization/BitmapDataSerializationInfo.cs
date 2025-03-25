#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: BitmapDataSerializationInfo.cs
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
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using KGySoft.Drawing.DebuggerVisualizers.Serialization;
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Drawing.SkiaSharp;
using KGySoft.Reflection;

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
            SKImageInfo info = bitmap.Info;
            BitmapInfo = new CustomBitmapInfo(true)
            {
                Type = bitmap.GetType().Name,
                BitmapData = info.IsEmpty ? null : bitmap.GetReadableBitmapData(),
            };
            PopulateCustomAttributes(BitmapInfo.CustomAttributes, info);
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
            var info = surface.PeekPixels()?.Info;
            if (info != null)
                PopulateCustomAttributes(BitmapInfo.CustomAttributes, info.Value);

            BitmapInfo.CustomAttributes[nameof(surface.Context.Backend)] = surface.Context?.Backend.ToString() ?? "Cpu";
            BitmapInfo.CustomAttributes[nameof(canvas.DeviceClipBounds)] = $"{canvas.DeviceClipBounds}";
            BitmapInfo.CustomAttributes[nameof(canvas.LocalClipBounds)] = $"{canvas.LocalClipBounds}";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapDataSerializationInfo"/> class from a foreign assembly identity.
        /// Called when the debugged project references a different version of SkiaSharp than the debugger visualizer.
        /// Here everything must be done by reflection, avoiding any direct cast to SkiaSharp types, including cloning into local SkiaSharp types that would
        /// access SKObject instances as the static constructor of SKObject throws an exception: SkiaSharpVersion.CheckNativeLibraryCompatible(throwIfIncompatible: true)
        /// </summary>
        /// <param name="srcObject">An <see cref="SKBitmap"/>, <see cref="SKPixmap"/>, <see cref="SKImage"/> or <see cref="SKSurface"/> instance with a foreign
        /// assembly identity, so an actual cast would end up in an <see cref="InvalidCastException"/>.</param>
        internal BitmapDataSerializationInfo(object srcObject)
        {
            try
            {
                Type srcType = srcObject.GetType();
                string type = srcType.Name;
                BitmapInfo = new CustomBitmapInfo(true) { Type = type };

                object info;
                object bmpSrc;
                switch (type)
                {
                    case nameof(SKBitmap):
                        info = Reflector.GetProperty(srcObject, nameof(SKBitmap.Info))!;
                        bmpSrc = srcObject;
                        break;

                    case nameof(SKPixmap):
                        info = Reflector.GetProperty(srcObject, nameof(SKPixmap.Info))!;

                        // bmpSrc = new SKBitmap();
                        // bmpSrc.InstallPixels(srcObject);
                        bmpSrc = Reflector.CreateInstance(Reflector.ResolveType(srcType.Assembly, $"{srcType.Namespace}.{nameof(SKBitmap)}")!);
                        Reflector.InvokeMethod(bmpSrc, nameof(SKBitmap.InstallPixels), srcObject);
                        break;

                    case nameof(SKImage):
                        info = Reflector.GetProperty(srcObject, nameof(SKImage.Info))!;

                        // pixels = new srcObject.PeekPixels();
                        object? pixels = Reflector.InvokeMethod(srcObject, nameof(SKImage.PeekPixels));
                        if (pixels != null)
                        {
                            // bmpSrc = new SKBitmap();
                            // bmpSrc.InstallPixels(pixels);
                            bmpSrc = Reflector.CreateInstance(Reflector.ResolveType(srcType.Assembly, $"{srcType.Namespace}.{nameof(SKBitmap)}")!);
                            Reflector.InvokeMethod(bmpSrc, nameof(SKBitmap.InstallPixels), pixels);
                        }
                        else
                        {
                            // bmpSrc = new SKBitmap(info);
                            // srcObject.ReadPixels(info, bmpSrc.GetPixels());
                            bmpSrc = Reflector.CreateInstance(Reflector.ResolveType(srcType.Assembly, $"{srcType.Namespace}.{nameof(SKBitmap)}")!, info);
                            Reflector.InvokeMethod(srcObject, nameof(SKImage.ReadPixels), info, Reflector.InvokeMethod(bmpSrc, nameof(SKBitmap.GetPixels)));
                        }
                        break;

                    case nameof(SKSurface):
                        // snapshot = srcObject.Snapshot();
                        // info = snapshot.Info;
                        // bmpSrc = new SKBitmap(info);
                        // snapshot.ReadPixels(info, bitmap.GetPixels());
                        // snapshot.Dispose();
                        object snapshot = Reflector.InvokeMethod(srcObject, nameof(SKSurface.Snapshot))!;
                        info = Reflector.GetProperty(snapshot, nameof(SKImage.Info))!;
                        bmpSrc = Reflector.CreateInstance(Reflector.ResolveType(srcType.Assembly, $"{srcType.Namespace}.{nameof(SKBitmap)}")!, info);
                        Reflector.InvokeMethod(snapshot, nameof(SKImage.ReadPixels), info, Reflector.InvokeMethod(bmpSrc, nameof(SKBitmap.GetPixels)));
                        (snapshot as IDisposable)?.Dispose();
                        break;

                    default:
                        throw new ArgumentException(PublicResources.ArgumentInvalid, nameof(srcObject));
                }

                var size = new Size((int)Reflector.GetProperty(info, nameof(SKImageInfo.Width))!, (int)Reflector.GetProperty(info, nameof(SKImageInfo.Height))!);

                IDictionary<string, string> attributes = BitmapInfo.CustomAttributes;
                attributes[nameof(size.Width)] = $"{size.Width}";
                attributes[nameof(size.Height)] = $"{size.Height}";
                attributes[nameof(SKImageInfo.ColorType)] = $"{Reflector.GetProperty(info, nameof(SKImageInfo.ColorType))}";
                attributes[nameof(SKImageInfo.AlphaType)] = $"{Reflector.GetProperty(info, nameof(SKImageInfo.AlphaType))}";
                attributes[nameof(SKImageInfo.ColorSpace)] = Reflector.GetProperty(info, nameof(SKImageInfo.ColorSpace)) is not object colorSpace ? "null" :
                    Reflector.GetProperty(colorSpace, nameof(SKColorSpace.IsSrgb)) is true ? "Srgb" :
                    Reflector.GetProperty(colorSpace, nameof(SKColorSpace.GammaIsLinear)) is true ? "Linear" :
                    "Custom";

                // Creating an sRGB BGRA8888 bitmap clone if necessary
                if (attributes[nameof(SKImageInfo.ColorType)] != nameof(SKColorType.Bgra8888) || attributes[nameof(SKImageInfo.ColorSpace)] is not ("null" or "Srgb"))
                {
                    var colorSpaceSrgb = Reflector.InvokeMethod(Reflector.ResolveType(srcType.Assembly, $"{srcType.Namespace}.{nameof(SKColorSpace)}")!, nameof(SKColorSpace.CreateSrgb))!;
                    info = Reflector.InvokeMethod(info, nameof(SKImageInfo.WithColorSpace), colorSpaceSrgb)!;
                    info = Reflector.InvokeMethod(info, nameof(SKImageInfo.WithColorType), (int)SKColorType.Bgra8888)!;
                    object clone = Reflector.CreateInstance(Reflector.ResolveType(srcType.Assembly, $"{srcType.Namespace}.{nameof(SKBitmap)}")!, info);
                    var canvas = Reflector.CreateInstance(Reflector.ResolveType(srcType.Assembly, $"{srcType.Namespace}.{nameof(SKCanvas)}")!, clone);
                    Reflector.InvokeMethod(canvas, nameof(SKCanvas.DrawBitmap), bmpSrc, 0f, 0f, null); // TODO: new SKPaint { BlendMode = SKBlendMode.Src } - not really needed because the target is empty but may improve performance
                    (canvas as IDisposable)?.Dispose();
                    if (bmpSrc != srcObject)
                        (bmpSrc as IDisposable)?.Dispose();
                    bmpSrc = clone;
                }

                IntPtr buffer = (IntPtr)Reflector.InvokeMethod(bmpSrc, nameof(SKBitmap.GetPixels))!;
                Action? disposeCallback = bmpSrc == srcObject ? null : () => (bmpSrc as IDisposable)?.Dispose();
                var pixelFormat = attributes[nameof(SKImageInfo.AlphaType)] switch
                {
                    nameof(SKAlphaType.Premul) => KnownPixelFormat.Format32bppPArgb,
                    nameof(SKAlphaType.Unpremul) => KnownPixelFormat.Format32bppArgb,
                    nameof(SKAlphaType.Opaque) => KnownPixelFormat.Format32bppRgb,
                    _ => KnownPixelFormat.Undefined
                };

                BackingObject = srcObject;
                BitmapInfo.BitmapData = buffer == IntPtr.Zero || pixelFormat == KnownPixelFormat.Undefined
                    ? null
                     // BitmapDataFactory.CreateBitmapData(buffer, size, size.Width * 4, pixelFromat, default(Color32), default(byte), disposeCallback),
                    : (IReadableBitmapData)Reflector.InvokeMethod(typeof(BitmapDataFactory), nameof(BitmapDataFactory.CreateBitmapData),
                        buffer, size, size.Width * 4, pixelFormat, default(Color32), default(byte), disposeCallback)!;

#if DEBUG
                if (srcType.Assembly != typeof(SKColor).Assembly)
                    BitmapInfo.CustomAttributes["SkiaSharp version mismatch"] = $"{srcType.Assembly} vs. {typeof(SKColor).Assembly}";
#endif

            }
            catch (Exception e) when (e is not ArgumentException)
            {
                throw new ArgumentException(PublicResources.ArgumentInvalid, nameof(srcObject), e);
            }
        }

        internal BitmapDataSerializationInfo(BinaryReader reader)
            : base(reader)
        {
        }

        #endregion

        #region Methods

        private static void PopulateCustomAttributes(IDictionary<string, string> attributes, SKImageInfo info)
        {
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
