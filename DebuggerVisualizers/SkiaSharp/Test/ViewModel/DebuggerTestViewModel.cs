#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DebuggerTestViewModel.cs
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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Media;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.DebuggerVisualizers.Test;
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Drawing.Wpf;
using KGySoft.Drawing.SkiaSharp;

using SkiaSharp;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.SkiaSharp.Test.ViewModel
{
    internal class DebuggerTestViewModel : ObservableObjectBase
    {
        #region Fields

        #region Static Fields

        private static readonly HashSet<string>[] radioGroups =
        {
            new HashSet<string>
            {
                nameof(ColorSpaceSrgb),
                nameof(ColorSpaceLinear),
                nameof(ColorSpaceAdobe),
            },
            new HashSet<string>
            {
                nameof(IsSKBitmap),
                nameof(IsSKPixmap),
                nameof(IsSKImage),
                nameof(IsSKSurface),
                nameof(IsSKColor),
                nameof(IsSKPMColor),
                nameof(IsSKColorF),
            }
        };

        private static readonly Dictionary<Type, DebuggerVisualizerAttribute> debuggerVisualizers = SkiaSharpDebuggerHelper.GetDebuggerVisualizers();

#if NET472_OR_GREATER
        private static readonly Dictionary<Type, IDebuggerVisualizerProvider> debuggerVisualizerProviders = SkiaSharpDebuggerHelper.GetDebuggerVisualizerProviders();
#endif

        #endregion

        #region Instance Fields

        private SKColorSpace? adobeColorSpace;
        private IReadableBitmapData? sampleBitmapData;
        private SKBitmap? backingBitmap;

        #endregion

        #endregion

        #region Properties

        #region Public Properties

        public SKColorType[] ColorTypes { get; } = Enum<SKColorType>.GetValues().Except(new[] { SKColorType.Unknown }).ToArray();
        public SKAlphaType[] AlphaTypes { get; } = Enum<SKAlphaType>.GetValues().Except(new[] { SKAlphaType.Unknown }).ToArray();

        public SKColorType SelectedColorType { get => Get(SKColorType.Argb4444); set => Set(value); }
        public SKAlphaType SelectedAlphaType { get => Get(SKAlphaType.Unpremul); set => Set(value); }

        public bool ColorSpaceSrgb { get => Get(true); set => Set(value); }
        public bool ColorSpaceLinear { get => Get<bool>(); set => Set(value); }
        public bool ColorSpaceAdobe { get => Get<bool>(); set => Set(value); }
        
        public ImageSource? PreviewImage { get => Get<ImageSource?>(); set => Set(value); }
        public bool IsSKBitmap { get => Get(true); set => Set(value); }
        public bool IsSKPixmap { get => Get<bool>(); set => Set(value); }
        public bool IsSKImage { get => Get<bool>(); set => Set(value); }
        public bool IsSKSurface { get => Get<bool>(); set => Set(value); }
        public bool IsSKColor { get => Get<bool>(); set => Set(value); }
        public bool IsSKPMColor { get => Get<bool>(); set => Set(value); }
        public bool IsSKColorF { get => Get<bool>(); set => Set(value); }

        public ICommand DirectViewCommand => Get(() => new SimpleCommand(OnViewDirectCommand));
        public ICommand ClassicDebugCommand => Get(() => new SimpleCommand(OnClassicDebugCommand));
        public ICommand ExtensionDebugCommand => Get(() => new SimpleCommand(OnExtensionDebugCommand));

        public ICommandState DebugCommandState => Get(() => new CommandState { Enabled = false });

        #endregion

        #region Internal Properties

        internal Action<string>? ErrorCallback { get => Get<Action<string>?>(); set => Set(value); }
        internal Func<IntPtr>? GetHwndCallback { get => Get<Func<IntPtr>?>(); set => Set(value); }

        #endregion

        #region Private Properties

        private object? TestObject { get => Get<object?>(); set => Set(value); }

        private SKColorSpace ColorSpace => ColorSpaceSrgb ? SKColorSpace.CreateSrgb()
            : ColorSpaceLinear ? SKColorSpace.CreateSrgbLinear()
            : ColorSpaceAdobe ? AdobeColorSpace
            : throw new InvalidOperationException("Unexpected color space");

        private SKColorSpace AdobeColorSpace => adobeColorSpace ??= SKColorSpace.CreateRgb(SKColorSpaceTransferFn.Srgb, SKColorSpaceXyz.AdobeRgb);

        private IReadableBitmapData SampleBitmapData
        {
            get
            {
                if (sampleBitmapData == null)
                {
                    using Icon icon = Icons.Shield;
                    using Bitmap bmp = icon.ExtractBitmap(0)!;
                    using IReadableBitmapData bmpData = bmp.GetReadableBitmapData();
                    sampleBitmapData = bmpData.Clone();
                }

                return sampleBitmapData;
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Internal Methods

        internal void ViewLoaded() => TestObject = GenerateObject();

        #endregion

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.NewValue is true && radioGroups.FirstOrDefault(g => g.Contains(e.PropertyName!)) is HashSet<string> group)
            {
                AdjustRadioGroup(e.PropertyName!, group);
                TestObject = GenerateObject();
                return;
            }

            if (e.PropertyName is nameof(SelectedColorType) or nameof(SelectedAlphaType))
            {
                TestObject = GenerateObject();
                return;
            }

            if (e.PropertyName == nameof(TestObject))
            {
                object? obj = TestObject;
                PreviewImage = GetPreviewImage(obj);
                DebugCommandState.Enabled = obj != null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;
            if (disposing)
            {
                FreeTestObject();
                adobeColorSpace?.Dispose();
                sampleBitmapData?.Dispose();
                backingBitmap?.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void AdjustRadioGroup(string propertyName, IEnumerable<string> group)
        {
            foreach (string prop in group)
            {
                if (prop != propertyName)
                    Set(false, propertyName: prop);
            }
        }

        private object? GenerateObject()
        {
            FreeTestObject();

            if (IsSKColor)
                return SKColors.Red;

            if (IsSKPMColor)
                return new SKPMColor(0xFF00FF00);

            if (IsSKColorF)
                return new SKColorF(0f, 0f, 1f, 1f);

            // TODO
            //if (ImageFromFile)
            //    return FromFile(FileName);

            IReadableBitmapData srcData = SampleBitmapData;
            var info = new SKImageInfo(srcData.Width, srcData.Height, SelectedColorType, SelectedAlphaType, ColorSpace);
            var bmp = new SKBitmap(info);
            using (IWritableBitmapData dstData = bmp.GetWritableBitmapData())
                srcData.CopyTo(dstData);

            try
            {
                if (IsSKBitmap)
                    return bmp;

                if (IsSKPixmap)
                {
                    backingBitmap = bmp;
                    return bmp.PeekPixels();
                }

                if (IsSKImage)
                {
                    using SKPixmap pixels = bmp.PeekPixels();
                    using SKSurface surface = SKSurface.Create(pixels);
                    SKImage result = surface.Snapshot();
                    bmp.Dispose();
                    return result;
                }

                if (IsSKSurface)
                {
                    backingBitmap = bmp;
                    using SKPixmap pixels = bmp.PeekPixels();
                    return SKSurface.Create(pixels);
                }

            }
            catch (Exception e) when (e is not StackOverflowException)
            {
                ErrorCallback?.Invoke($"Could not generate test object: {e.Message}");
                return null;
            }

            return null;
        }

        private ImageSource? GetPreviewImage(object? obj)
        {
            try
            {
                switch (obj)
                {
                    case SKBitmap bitmap:
                        return bitmap.GetReadableBitmapData().ToWriteableBitmap();
                    case SKPixmap bitmap:
                        return bitmap.GetReadableBitmapData().ToWriteableBitmap();
                    case SKImage image:
                        return image.GetReadableBitmapData().ToWriteableBitmap();
                    case SKSurface surface:
                        return surface.GetReadableBitmapData().ToWriteableBitmap();
                    case SKColor color:
                        {
                            using IReadWriteBitmapData bmpData = BitmapDataFactory.CreateBitmapData(new Size(1, 1));
                            bmpData.SetColor32(0, 0, color.ToColor32());
                            return bmpData.ToWriteableBitmap();
                        }
                    case SKPMColor color:
                        {
                            using IReadWriteBitmapData bmpData = BitmapDataFactory.CreateBitmapData(new Size(1, 1));
                            bmpData.SetColor32(0, 0, color.ToColor32());
                            return bmpData.ToWriteableBitmap();
                        }
                    case SKColorF color:
                        {
                            using IReadWriteBitmapData bmpData = BitmapDataFactory.CreateBitmapData(new Size(1, 1));
                            bmpData.SetColor32(0, 0, color.ToColor32());
                            return bmpData.ToWriteableBitmap();
                        }
                    default:
                        return null;
                }
            }
            catch (Exception e) when (e is not StackOverflowException)
            {
                ErrorCallback?.Invoke($"Could not generate preview image: {e.Message}");
                return null;
            }
        }

        private void FreeTestObject()
        {
            object? obj = TestObject;
            if (obj == null)
                return;

            TestObject = null;
            (obj as IDisposable)?.Dispose();
            backingBitmap?.Dispose();
            backingBitmap = null;
        }

        private void OnViewDirectCommand()
        {
            #region Local Methods

            static void PopulateInfo(IDictionary<string, string> attributes, SKImageInfo imageInfo)
            {
                attributes[nameof(imageInfo.ColorType)] = Enum<SKColorType>.ToString(imageInfo.ColorType);
                attributes[nameof(imageInfo.AlphaType)] = Enum<SKAlphaType>.ToString(imageInfo.AlphaType);
                SKColorSpace? colorSpace = imageInfo.ColorSpace;
                attributes[nameof(imageInfo.ColorSpace)] = colorSpace?.IsSrgb != false ? "sRGB"
                    : colorSpace.GammaIsLinear ? "Linear"
                    : "Custom";
            }

            #endregion

            IntPtr hwnd = GetHwndCallback?.Invoke() ?? IntPtr.Zero;

            try
            {
                switch (TestObject)
                {
                    case SKBitmap bitmap:
                        var info = new CustomBitmapInfo(true)
                        {
                            Type = bitmap.GetType().Name,
                            ShowPixelSize = true,
                            BitmapData = bitmap.GetReadableBitmapData(),
                        };

                        PopulateInfo(info.CustomAttributes, bitmap.Info);
                        DebuggerHelper.DebugCustomBitmap(info, hwnd);
                        return;

                    case SKPixmap pixmap:
                        info = new CustomBitmapInfo(true)
                        {
                            Type = pixmap.GetType().Name,
                            ShowPixelSize = true,
                            BitmapData = pixmap.GetReadableBitmapData(),
                        };

                        PopulateInfo(info.CustomAttributes, pixmap.Info);
                        DebuggerHelper.DebugCustomBitmap(info, hwnd);
                        return;

                    case SKImage image:
                        info = new CustomBitmapInfo(true)
                        {
                            Type = image.GetType().Name,
                            ShowPixelSize = true,
                            BitmapData = image.GetReadableBitmapData(),
                        };

                        PopulateInfo(info.CustomAttributes, image.Info);
                        DebuggerHelper.DebugCustomBitmap(info, hwnd);
                        return;

                    case SKSurface surface:
                        info = new CustomBitmapInfo(true)
                        {
                            Type = surface.GetType().Name,
                            ShowPixelSize = true,
                            BitmapData = surface.GetReadableBitmapData(),
                        };

                        PopulateInfo(info.CustomAttributes, backingBitmap!.Info);
                        DebuggerHelper.DebugCustomBitmap(info, hwnd);
                        return;

                    case SKColor color:
                        DebuggerHelper.DebugCustomColor(new CustomColorInfo
                        {
                            Type = color.GetType().Name,
                            DisplayColor = color.ToColor32(),
                            Name = color.ToString()
                        }, hwnd);
                        return;

                    case SKPMColor color:
                        DebuggerHelper.DebugCustomColor(new CustomColorInfo
                        {
                            Type = color.GetType().Name,
                            DisplayColor = color.ToColor32(),
                            Name = color.ToString()
                        }, hwnd);
                        return;

                    case SKColorF color:
                        DebuggerHelper.DebugCustomColor(new CustomColorInfo
                        {
                            Type = color.GetType().Name,
                            DisplayColor = color.ToColor32(),
                            Name = color.ToString()
                        }, hwnd);
                        return;

                    default:
                        throw new InvalidOperationException($"Unexpected object type: {TestObject?.GetType()}");
                }
            }
            catch (Exception e) when (e is not StackOverflowException)
            {
                ErrorCallback?.Invoke($"Failed to view object: {e.Message}");
            }
        }

        private void OnClassicDebugCommand()
        {
            object? testObject = TestObject;
            if (testObject == null)
                return;

            Type targetType = testObject.GetType();
            DebuggerVisualizerAttribute? attr = debuggerVisualizers.GetValueOrDefault(targetType);
            if (attr == null)
            {
                ErrorCallback?.Invoke($"No debugger visualizer found for type {targetType}");
                return;
            }

            try
            {
                if (DebuggerVisualizerHelper.ShowClassicVisualizer(attr, testObject, default, out object? replacementObject))
                    TestObject = replacementObject;
            }
            catch (Exception e) when (e is not StackOverflowException)
            {
                ErrorCallback?.Invoke($"Failed to debug object: {e.Message}");
            }
        }

        private void OnExtensionDebugCommand()
        {
            object? testObject = TestObject;
            if (testObject == null)
                return;

#if NET472_OR_GREATER
            Type targetType = testObject.GetType();
            IDebuggerVisualizerProvider? provider = debuggerVisualizerProviders.GetValueOrDefault(targetType);
            if (provider == null)
            {
                ErrorCallback?.Invoke($"No debugger visualizer extension found for type {targetType}");
                return;
            }

            try
            {
                DebuggerVisualizerHelper.ShowExtensionVisualizer(provider, testObject, default, o => TestObject = o);
            }
            catch (Exception e) when (e is not StackOverflowException)
            {
                ErrorCallback?.Invoke($"Failed to debug object: {e.Message}");
            }
#else
            ErrorCallback?.Invoke("Debugger visualizer extensions are supported only in .NET Framework 4.7.2 and above.");
#endif
        }

        #endregion

        #endregion
    }
}
