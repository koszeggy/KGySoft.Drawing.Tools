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
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.DebuggerVisualizers.Test;
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Reflection;

using Microsoft.VisualStudio.DebuggerVisualizers;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Core.Test.ViewModel
{
    /// <summary>
    /// ViewModel class for DebuggerTestForm.
    /// NOTE: Debugging images when the debugger is attached to this project will likely NOT work because
    /// this project references a specific version of Microsoft.VisualStudio.DebuggerVisualizers, which may differ your version.
    /// Debugging works for other projects, which do not reference Microsoft.VisualStudio.DebuggerVisualizers (eg. ImagingTools).
    /// </summary>
    internal class DebuggerTestViewModel : ObservableObjectBase
    {
        #region Fields

        #region Static Fields
        
        private static readonly HashSet<string>[] radioGroups =
{
            [
                nameof(ManagedBitmapData),
                nameof(Palette),
                nameof(Color32),
                nameof(PColor32),
                nameof(Color64),
                nameof(PColor64),
                nameof(ColorF),
                nameof(PColorF),
                nameof(BitmapDataFromFile)
            ],
            [nameof(FileAsNative), nameof(FileAsManaged)],
        };

        private static readonly Dictionary<Type, DebuggerVisualizerAttribute> debuggerVisualizers = CoreDebuggerHelper.GetDebuggerVisualizers();

        private static Palette? palette16Bpp;

        #endregion

        #region Instance Fields

        private readonly IReadableBitmapData bmpShield;

        private Bitmap? bitmapDataOwner;

        #endregion

        #endregion

        #region Properties

        #region Static Properties

        private static Palette Palette16Bpp
        {
            get
            {
                #region Local Methods

                static int GetColorIndex(Color32 c, IPalette _)
                    => ((15 - (c.A >> 4)) << 12)
                        | ((c.R >> 4) << 8)
                        | ((c.G >> 4) << 4)
                        | (c.B >> 4);

                #endregion

                if (palette16Bpp == null)
                {
                    var colors = new Color32[65536];
                    for (int a = 15; a >= 0; a--)
                    for (int r = 0; r < 16; r++)
                    for (int g = 0; g < 16; g++)
                    for (int b = 0; b < 16; b++)
                        colors[((15 - a) << 12) | (r << 8) | (g << 4) | b] = new Color32((byte)(a * 17), (byte)(r * 17), (byte)(g * 17), (byte)(b * 17));

                    palette16Bpp = new Palette(colors, customGetNearestColorIndex: GetColorIndex);
                }

                return palette16Bpp;
            }
        }

        #endregion

        #region Instance Properties

        internal PixelFormatInfo[] PixelFormats { get; } = Enum<KnownPixelFormat>.GetValues()
            .Where(pf => pf.IsValidFormat())
            .Select(pf => pf.GetInfo())
            .OrderBy(pf => pf.BitsPerPixel)
            .Concat(new[] { new PixelFormatInfo(16) { Indexed = true }, new(4) { HasSingleBitAlpha = true } })
            .ToArray();

        internal PixelFormatInfo SelectedFormat { get => Get<PixelFormatInfo>(); set => Set(value); }
        internal bool PixelFormatEnabled { get => Get<bool>(); set => Set(value); }

        internal bool ManagedBitmapData { get => Get<bool>(); set => Set(value); }
        internal bool Palette { get => Get<bool>(); set => Set(value); }
        internal bool Color32 { get => Get<bool>(); set => Set(value); }
        internal bool PColor32 { get => Get<bool>(); set => Set(value); }
        internal bool Color64 { get => Get<bool>(); set => Set(value); }
        internal bool PColor64 { get => Get<bool>(); set => Set(value); }
        internal bool ColorF { get => Get<bool>(); set => Set(value); }
        internal bool PColorF { get => Get<bool>(); set => Set(value); }

        internal bool BitmapDataFromFile { get => Get<bool>(); set => Set(value); }
        internal string? FileName { get => Get<string?>(); set => Set(value); }
        internal bool FileAsNative { get => Get<bool>(); set => Set(value); }
        internal bool FileAsManaged { get => Get<bool>(); set => Set(value); }

        internal bool CanDebug { get => Get<bool>(); set => Set(value); }
        internal Image? PreviewImage { get => Get<Image?>(); set => Set(value); }

        internal Action<string>? ErrorCallback { get => Get<Action<string>?>(); set => Set(value); }
        internal Func<IntPtr>? GetHwndCallback { get => Get<Func<IntPtr>?>(); set => Set(value); }

        internal ICommand DebugCommand => Get(() => new SimpleCommand(OnDebugCommand));
        internal ICommand DirectViewCommand => Get(() => new SimpleCommand(OnViewDirectCommand));

        private object? TestObject { get => Get<object?>(); set => Set(value); }

        #endregion

        #endregion

        #region Constructor

        internal DebuggerTestViewModel()
        {
            using (Icon icon = Icons.Shield)
            using (Bitmap bmp = icon.ExtractBitmap(0)!)
            using (IReadableBitmapData bmpData = bmp.GetReadableBitmapData())
            {
                bmpShield = bmpData.Clone();
            }

            SelectedFormat = new PixelFormatInfo(KnownPixelFormat.Format8bppIndexed);
        }

        #endregion

        #region Methods

        #region Static Methods

        private static CustomBitmapDataConfigBase GetConfig(PixelFormatInfo info) => info switch
        {
            { BitsPerPixel: 4, Indexed: false } => new CustomBitmapDataConfig
            {
                PixelFormat = info,
                BackBufferIndependentPixelAccess = true,
                RowGetColor32 = (r, x) =>
                {
                    int nibbles = r.GetRefAs<byte>(x >> 1);
                    int color = (x & 1) == 0
                        ? nibbles >> 4
                        : nibbles & 0b00001111;
                    return new Color32((byte)((color & 8) == 0 ? 0 : 255),
                        (byte)((color & 4) == 0 ? 0 : 255),
                        (byte)((color & 2) == 0 ? 0 : 255),
                        (byte)((color & 1) == 0 ? 0 : 255));
                },
                RowSetColor32 = (r, x, c) =>
                {
                    ref byte nibbles = ref r.GetRefAs<byte>(x >> 1);
                    if (c.A != 255)
                        c = c.A >= r.BitmapData.AlphaThreshold ? c.Blend(r.BitmapData.BackColor, r.BitmapData.WorkingColorSpace) : default;
                    int color = ((c.A & 128) >> 4)
                        | ((c.R & 128) >> 5)
                        | ((c.G & 128) >> 6)
                        | ((c.B & 128) >> 7);
                    if ((x & 1) == 0)
                    {
                        nibbles &= 0b00001111;
                        nibbles |= (byte)(color << 4);
                    }
                    else
                    {
                        nibbles &= 0b11110000;
                        nibbles |= (byte)color;
                    }
                }
            },
            { BitsPerPixel: 16, Indexed: true } => new CustomIndexedBitmapDataConfig
            {
                PixelFormat = info,
                BackBufferIndependentPixelAccess = true,
                RowGetColorIndex = (r, x) => r.UnsafeGetRefAs<ushort>(x),
                RowSetColorIndex = (r, x, i) => r.UnsafeGetRefAs<ushort>(x) = (ushort)i,
                Palette = Palette16Bpp
            },
            _ => throw new ArgumentException()
        };

        private static Image FromPalette(Palette palette)
        {
            int count = palette.Count;
            int width = count % 256;
            if (width == 0)
                width = 256;
            int height = width == count ? width : (int)Math.Ceiling(count / 256d);

            var result = new Bitmap(width, height);
            using IWritableBitmapData bmpData = result.GetWritableBitmapData();
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                bmpData.SetColor32(x, y, palette[(y * width + x) % count]);
            return result;
        }

        private static CustomPaletteInfo? GetCustomPalette(Palette? palette)
        {
            if (palette == null)
                return null;
            var result = new CustomPaletteInfo
            {
                Type = palette.GetType().Name,
                EntryType = nameof(Imaging.Color32)
            };

            foreach (Color32 color in palette.GetEntries())
            {
                result.Entries.Add(new CustomColorInfo
                {
                    DisplayColor = color,
                    Name = color.ToString()
                });
            }

            return result;
        }

        #endregion

        #region Instance Methods

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.NewValue is true && radioGroups.FirstOrDefault(g => g.Contains(e.PropertyName!)) is HashSet<string> group)
            {
                AdjustRadioGroup(e.PropertyName!, group);
                PixelFormatEnabled = ManagedBitmapData || Palette || BitmapDataFromFile && FileAsManaged;
                TestObject = GenerateObject();
                return;
            }

            if (e.PropertyName == nameof(FileName) && BitmapDataFromFile
                || e.PropertyName == nameof(SelectedFormat) && PixelFormatEnabled)
            {
                TestObject = GenerateObject();
                return;
            }

            if (e.PropertyName == nameof(TestObject))
            {
                Image? preview = PreviewImage;
                PreviewImage = null;
                (preview as IDisposable)?.Dispose();

                object? obj = TestObject;
                PreviewImage = GetPreviewImage(obj);
                CanDebug = obj != null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;
            if (disposing)
                FreeTestObject();
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

            try
            {
                if (ManagedBitmapData)
                    return ToManagedBitmapData(bmpShield);

                if (Palette)
                {
                    if (!SelectedFormat.Indexed)
                        return null;
                    return SelectedFormat.BitsPerPixel switch
                    {
                        1 => Imaging.Palette.BlackAndWhite(),
                        4 => Imaging.Palette.SystemDefault4BppPalette(),
                        8 => Imaging.Palette.SystemDefault8BppPalette(),
                        16 => Palette16Bpp,
                        _ => throw new InvalidOperationException($"Unexpected BPP for palette: {SelectedFormat.BitsPerPixel}")
                    };
                }

                if (Color32)
                    return Color.Red.ToColor32();

                if (PColor32)
                    return Color.Yellow.ToPColor32();

                if (Color64)
                    return Color.Lime.ToColor64();

                if (PColor64)
                    return Color.Cyan.ToPColor64();

                if (ColorF)
                    return Color.Blue.ToColorF();

                if (PColorF)
                    return Color.Magenta.ToPColorF();

                if (BitmapDataFromFile)
                    return FromFile(FileName);
            }
            catch (Exception e) when (e is not StackOverflowException)
            {
                ErrorCallback?.Invoke($"Could not generate test object: {e.Message}");
                return null;
            }

            return null;
        }

        private IReadableBitmapData ToManagedBitmapData(IReadableBitmapData source)
        {
            #region Local Methods

            static byte[,] GetBuffer(PixelFormatInfo info, Size size) => new byte[size.Height, (size.Width * info.BitsPerPixel + 7) >> 3];

            #endregion

            PixelFormatInfo info = SelectedFormat;
            if (!info.IsCustomFormat)
                return source.Clone(info.ToKnownPixelFormat());

            CustomBitmapDataConfigBase cfg = GetConfig(info);
            var result = cfg switch
            {
                CustomBitmapDataConfig customCfg => BitmapDataFactory.CreateBitmapData(GetBuffer(info, source.Size), source.Width, customCfg),
                CustomIndexedBitmapDataConfig customIndexedCfg => BitmapDataFactory.CreateBitmapData(GetBuffer(info, source.Size), source.Width, customIndexedCfg),
                _ => throw new InvalidOperationException()
            };

            source.CopyTo(result);
            return result;
        }

        private Image? GetPreviewImage(object? obj)
        {
            try
            {
                return obj switch
                {
                    IReadableBitmapData bitmapData => bitmapData.ToBitmap(PixelFormat.Format32bppPArgb),
                    Palette palette => FromPalette(palette),
                    Color32 c => FromPalette(new Palette(new[] { c })),
                    PColor32 c => FromPalette(new Palette(new[] { c.ToColor32() })),
                    Color64 c => FromPalette(new Palette(new[] { c.ToColor32() })),
                    PColor64 c => FromPalette(new Palette(new[] { c.ToColor32() })),
                    ColorF c => FromPalette(new Palette(new[] { c.ToColor32() })),
                    PColorF c => FromPalette(new Palette(new[] { c.ToColor32() })),
                    _ => null
                };
            }
            catch (Exception e) when (e is not StackOverflowException)
            {
                ErrorCallback?.Invoke($"Could not generate preview image: {e.Message}");
                return null;
            }
        }

        private object? FromFile(string? fileName)
        {
            try
            {
                if (fileName == null || !File.Exists(fileName))
                    return null;
                var stream = new MemoryStream(File.ReadAllBytes(fileName));
                var image = Image.FromStream(stream);
                var bmp = image as Bitmap ?? new Bitmap(image);
                try
                {
                    IReadableBitmapData bmpData = bmp.GetReadableBitmapData();
                    if (FileAsNative)
                    {
                        bitmapDataOwner = bmp;
                        return bmpData;
                    }

                    IReadableBitmapData result = ToManagedBitmapData(bmpData);
                    bmpData.Dispose();
                    bmp.Dispose();
                    return result;
                }
                finally
                {
                    if (!ReferenceEquals(bmp, image))
                        image.Dispose();
                }
            }
            catch (Exception e) when (e is not StackOverflowException)
            {
                ErrorCallback?.Invoke($"Could not open file: {e.Message}");
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
            bitmapDataOwner?.Dispose();
            bitmapDataOwner = null;
        }

        #endregion

        #region Command Handlers
        
        private void OnViewDirectCommand()
        {
            IntPtr hwnd = GetHwndCallback?.Invoke() ?? IntPtr.Zero;

            try
            {
                switch (TestObject)
                {
                    case IReadableBitmapData bitmapData:
                        DebuggerHelper.DebugCustomBitmap(new CustomBitmapInfo(false)
                        {
                            Type = bitmapData.GetType().Name,
                            ShowPixelSize = true,
                            BitmapData = bitmapData,
                            CustomPalette = GetCustomPalette(bitmapData.Palette)
                        }, hwnd);
                        return;

                    case Palette palette:
                        CustomPaletteInfo paletteInfo = GetCustomPalette(palette)!;
                        DebuggerHelper.DebugCustomPalette(paletteInfo, hwnd);
                        return;

                    case Color32 c:
                        DebuggerHelper.DebugCustomColor(new CustomColorInfo
                        {
                            DisplayColor = c,
                            Type = nameof(Color32),
                            Name = c.ToString(),
                        }, hwnd);
                        return;

                    case PColor32 c:
                        DebuggerHelper.DebugCustomColor(new CustomColorInfo
                        {
                            DisplayColor = c.ToColor32(),
                            Type = nameof(PColor32),
                            Name = c.ToString(),
                        }, hwnd);
                        return;

                    case Color64 c:
                        DebuggerHelper.DebugCustomColor(new CustomColorInfo
                        {
                            DisplayColor = c.ToColor32(),
                            Type = nameof(Color64),
                            Name = c.ToString(),
                        }, hwnd);
                        return;

                    case PColor64 c:
                        DebuggerHelper.DebugCustomColor(new CustomColorInfo
                        {
                            DisplayColor = c.ToColor32(),
                            Type = nameof(PColor64),
                            Name = c.ToString(),
                        }, hwnd);
                        return;

                    case ColorF c:
                        DebuggerHelper.DebugCustomColor(new CustomColorInfo
                        {
                            DisplayColor = c.ToColor32(),
                            Type = nameof(ColorF),
                            Name = c.ToString(),
                        }, hwnd);
                        return;

                    case PColorF c:
                        DebuggerHelper.DebugCustomColor(new CustomColorInfo
                        {
                            DisplayColor = c.ToColor32(),
                            Type = nameof(PColorF),
                            Name = c.ToString(),
                        }, hwnd);
                        return;

                    default:
                        throw new InvalidOperationException($"Unexpected object type: {TestObject?.GetType()}");
                }
            }
            catch (Exception e) when (e is not (OutOfMemoryException or StackOverflowException))
            {
                ErrorCallback?.Invoke($"Failed to view object: {e.Message}");
            }
        }

        private void OnDebugCommand()
        {
            object? testObject = TestObject;
            if (testObject == null)
                return;

            Type targetType = testObject switch
            {
                IReadableBitmapData => Reflector.ResolveType("KGySoft.Drawing.Imaging.BitmapDataBase")!,
                //IPalette => typeof(IPalette),
                _ => testObject.GetType()
            };
            DebuggerVisualizerAttribute? attr = debuggerVisualizers.GetValueOrDefault(targetType);
            if (attr == null)
            {
                ErrorCallback?.Invoke($"No debugger visualizer found for type {targetType}");
                return;
            }

            try
            {
                var windowService = new TestWindowService();
                var objectProvider = new TestObjectProvider(testObject);
                DialogDebuggerVisualizer debugger = (DialogDebuggerVisualizer)Reflector.CreateInstance(Reflector.ResolveType(attr.VisualizerTypeName)!);
                objectProvider.Serializer = (VisualizerObjectSource)Reflector.CreateInstance(Reflector.ResolveType(attr.VisualizerObjectSourceTypeName!)!);
                Reflector.InvokeMethod(debugger, "Show", windowService, objectProvider);
                if (objectProvider.ObjectReplaced)
                    TestObject = objectProvider.Object;
            }
            catch (Exception e) when (e is not StackOverflowException)
            {
                ErrorCallback?.Invoke($"Failed to debug object: {e.Message}");
            }
        }

        #endregion


        #endregion

        #endregion
    }
}
