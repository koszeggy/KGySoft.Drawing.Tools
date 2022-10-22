#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DebuggerTestFormViewModel.cs
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;
using KGySoft.Reflection;

using Microsoft.VisualStudio.DebuggerVisualizers;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.GdiPlus.Test.ViewModel
{
    /// <summary>
    /// ViewModel class for DebuggerTestForm.
    /// NOTE: Debugging images when the debugger is attached to this project will likely NOT work because
    /// this project references a specific version of Microsoft.VisualStudio.DebuggerVisualizers, which may differ your version.
    /// Debugging works for other projects, which do not reference Microsoft.VisualStudio.DebuggerVisualizers (eg. ImagingTools).
    /// </summary>
    internal class DebuggerTestFormViewModel : ObservableObjectBase
    {
        #region Fields

        private static readonly HashSet<string>[] radioGroups =
        {
            new HashSet<string>
            {
                nameof(Bitmap),
                nameof(Metafile),
                nameof(HIcon), nameof(ManagedIcon),
                nameof(GraphicsBitmap), nameof(GraphicsHwnd),
                nameof(BitmapData),
                nameof(Palette), nameof(SingleColor),
                nameof(ImageFromFile)
            },
            new HashSet<string> { nameof(FileAsImage), nameof(FileAsBitmap), nameof(FileAsMetafile),nameof(FileAsIcon) },
        };

        private static readonly Dictionary<Type, DebuggerVisualizerAttribute> debuggerVisualizers = Attribute.GetCustomAttributes(typeof(DebuggerHelper).Assembly, typeof(DebuggerVisualizerAttribute))
            .Cast<DebuggerVisualizerAttribute>().ToDictionary(a => a.Target!, a => a);

        #endregion

        #region Properties

        internal bool AsImage { get => Get<bool>(); set => Set(value); }
        internal bool AsImageEnabled { get => Get<bool>(); set => Set(value); }

        internal PixelFormat[] PixelFormats => Get(() => Enum<PixelFormat>.GetValues()
            .Where(pf => pf.IsValidFormat())
            // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
            .OrderBy(pf => pf & PixelFormat.Max)
            .ToArray());
        
        internal PixelFormat PixelFormat { get => Get(PixelFormat.Format32bppArgb); set => Set(value); }
        internal bool PixelFormatEnabled { get => Get<bool>(); set => Set(value); }

        internal bool Bitmap { get => Get<bool>(); set => Set(value); }
        internal bool Metafile { get => Get<bool>(); set => Set(value); }
        internal bool HIcon { get => Get<bool>(); set => Set(value); }
        internal bool ManagedIcon { get => Get<bool>(); set => Set(value); }
        internal bool GraphicsBitmap { get => Get<bool>(); set => Set(value); }
        internal bool GraphicsHwnd { get => Get<bool>(); set => Set(value); }
        internal bool BitmapData { get => Get<bool>(); set => Set(value); }
        internal bool Palette { get => Get<bool>(); set => Set(value); }
        internal bool SingleColor { get => Get<bool>(); set => Set(value); }

        internal bool ImageFromFile { get => Get<bool>(); set => Set(value); }
        internal string? FileName { get => Get<string?>(); set => Set(value); }
        internal bool FileAsImage { get => Get<bool>(); set => Set(value); }
        internal bool FileAsBitmap { get => Get<bool>(); set => Set(value); }
        internal bool FileAsMetafile { get => Get<bool>(); set => Set(value); }
        internal bool FileAsIcon { get => Get<bool>(); set => Set(value); }

        internal bool AsReadOnly { get => Get<bool>(); set => Set(value); }
        internal bool AsReadOnlyEnabled { get => Get<bool>(); set => Set(value); }

        internal bool CanDebug { get => Get<bool>(); set => Set(value); }
        internal Image? PreviewImage { get => Get<Image?>(); set => Set(value); }

        internal Action<string>? ErrorCallback { get => Get<Action<string>?>(); set => Set(value); }
        internal Func<IntPtr>? GetHwndCallback { get => Get<Func<IntPtr>?>(); set => Set(value); }
        internal Func<Rectangle>? GetClipCallback { get => Get<Func<Rectangle>?>(); set => Set(value); }

        internal ICommand DebugCommand => Get(() => new SimpleCommand(OnDebugCommand));
        internal ICommand DirectViewCommand => Get(() => new SimpleCommand(OnViewDirectCommand));

        private object? TestObject { get => Get<object?>(); set => Set(value); }
        private Bitmap? BitmapDataOwner { get => Get<Bitmap?>(); set => Set(value); }

        #endregion

        #region Methods

        #region Static Methods

        private static Image? FromPalette(IList<Color> palette)
        {
            var size = palette.Count;
            if (size == 0)
                return null;
            var result = new Bitmap(size, size, PixelFormat.Format32bppArgb);
            for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                result.SetPixel(x, y, palette[x]);
            return result;
        }

        private static Metafile GenerateMetafile()
        {
            //Set up reference Graphic
            Graphics refGraph = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr hdc = refGraph.GetHdc();
            try
            {
                var result = new Metafile(hdc, new Rectangle(0, 0, 100, 100), MetafileFrameUnit.Pixel, EmfType.EmfOnly, "Test");

                //Draw some silly drawing
                using (var g = Graphics.FromImage(result))
                {
                    var r = new Rectangle(0, 0, 100, 100);
                    var leftEye = new Rectangle(20, 20, 20, 30);
                    var rightEye = new Rectangle(60, 20, 20, 30);
                    g.FillEllipse(Brushes.Yellow, r);
                    g.FillEllipse(Brushes.White, leftEye);
                    g.FillEllipse(Brushes.White, rightEye);
                    g.DrawEllipse(Pens.Black, leftEye);
                    g.DrawEllipse(Pens.Black, rightEye);
                    g.DrawBezier(Pens.Red, new Point(10, 50), new Point(10, 100), new Point(90, 100), new Point(90, 50));
                }

                return result;
            }
            finally
            {
                refGraph.ReleaseHdc(hdc);
                refGraph.Dispose();
            }
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
                if (group.Contains(nameof(Bitmap)))
                {
                    AsImageEnabled = e.PropertyName.In(nameof(Bitmap), nameof(Metafile), nameof(HIcon), nameof(ManagedIcon));
                    PixelFormatEnabled = e.PropertyName.In(nameof(Bitmap), nameof(BitmapData), nameof(Palette), nameof(GraphicsBitmap));
                    AsReadOnlyEnabled = !e.PropertyName.In(nameof(GraphicsBitmap), nameof(GraphicsHwnd), nameof(BitmapData));
                }

                TestObject = GenerateObject();
                return;
            }

            if (e.PropertyName == nameof(FileName) && ImageFromFile
                || e.PropertyName == nameof(PixelFormat) && PixelFormatEnabled
                || e.PropertyName == nameof(AsImage))
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
                if (Bitmap)
                {
                    using Icon icon = Icons.Shield;
                    using Bitmap bmp = icon.ExtractBitmap(0)!;
                    return bmp.ConvertPixelFormat(PixelFormat);
                }

                if (Metafile)
                    return GenerateMetafile();
                if (HIcon)
                    return AsImage ? SystemIcons.Application.ToMultiResBitmap() : SystemIcons.Application;

                if (ManagedIcon)
                {
                    if (!AsImage)
                        return Icons.Application;
                    using Icon icon = Icons.Application;
                    return icon.ToMultiResBitmap();
                }

                if (GraphicsBitmap)
                    return GetBitmapGraphics();
                if (GraphicsHwnd)
                    return GetWindowGraphics();
                if (BitmapData)
                    return GetBitmapData(PixelFormat);
                
                if (Palette)
                {
                    using var bmp = new Bitmap(1, 1, PixelFormat);
                    return bmp.Palette;
                }

                if (SingleColor)
                    return Color.Black;
                if (ImageFromFile)
                    return FromFile(FileName);
            }
            catch (Exception e) when (e is not StackOverflowException)
            {
                ErrorCallback?.Invoke($"Could not generate test object: {e.Message}");
                return null;
            }

            return null;
        }

        private Image? GetPreviewImage(object? obj)
        {
            try
            {
                static Image ToSupportedFormat(Image image) =>
                    image.PixelFormat == PixelFormat.Format16bppGrayScale ? image.ConvertPixelFormat(PixelFormat.Format24bppRgb)
                    : !OSUtils.IsWindows && image.PixelFormat.In(PixelFormat.Format16bppRgb555, PixelFormat.Format16bppRgb565) ? image.ConvertPixelFormat(PixelFormat.Format24bppRgb)
                    : image;

                switch (obj)
                {
                    case Image image:
                        return ToSupportedFormat(image);
                    case Icon icon:
                        return icon.ToMultiResBitmap();
                    case Graphics graphics:
                        return graphics.ToBitmap(false);
                    case BitmapData _:
                        return ToSupportedFormat((Image)BitmapDataOwner!.Clone());
                    case ColorPalette palette:
                        return FromPalette(palette.Entries);
                    case Color color:
                        return FromPalette(new[] { color });
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

        private object? FromFile(string? fileName)
        {
            try
            {
                if (fileName == null || !File.Exists(fileName))
                    return null;
                var stream = new MemoryStream(File.ReadAllBytes(fileName));
                if (FileAsIcon)
                    return Icons.FromStream(stream);
                var image = Image.FromStream(stream);
                if (FileAsBitmap && image is not System.Drawing.Bitmap)
                {
                    image.Dispose();
                    ErrorCallback?.Invoke("The file is not a Bitmap");
                    return null;
                }

                if (FileAsMetafile && image is not System.Drawing.Imaging.Metafile)
                {
                    image.Dispose();
                    ErrorCallback?.Invoke("The file is not a Metafile");
                    return null;
                }

                return image;
            }
            catch (Exception e) when (e is not StackOverflowException)
            {
                ErrorCallback?.Invoke($"Could not open file: {e.Message}");
                return null;
            }
        }

        private Graphics? GetBitmapGraphics()
        {
            try
            {
                using Icon icon = Icons.Shield;
                using Bitmap bmp = icon.ExtractBitmap(0)!;
                return Graphics.FromImage(bmp.ConvertPixelFormat(PixelFormat));
            }
            catch (Exception e) when (e is not StackOverflowException)
            {
                ErrorCallback?.Invoke($"Could not create Graphics from a Bitmap with PixelFormat '{PixelFormat}': {e.Message}");
                return null;
            }
        }

        private Graphics GetWindowGraphics()
        {
            IntPtr hwnd = GetHwndCallback?.Invoke() ?? IntPtr.Zero;
            var graphics = Graphics.FromHwnd(hwnd);
            var clip = GetClipCallback?.Invoke();
            if (clip.HasValue)
                graphics.SetClip(clip.Value, CombineMode.Intersect);
            return graphics;
        }

        private BitmapData GetBitmapData(PixelFormat pixelFormat)
        {
            Bitmap bitmap = Icons.Shield.ExtractBitmap(0)!;
            if (pixelFormat != bitmap.PixelFormat)
                bitmap = bitmap.ConvertPixelFormat(pixelFormat);
            BitmapDataOwner = bitmap;
            return bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size), ImageLockMode.ReadOnly, bitmap.PixelFormat);
        }

        private void FreeTestObject()
        {
            object? obj = TestObject;
            if (obj == null)
                return;

            TestObject = null;
            switch (obj)
            {
                case IDisposable disposable:
                    disposable.Dispose();
                    break;
                case BitmapData bitmapData:
                    Bitmap bitmap = BitmapDataOwner!;
                    bitmap.UnlockBits(bitmapData);
                    bitmap.Dispose();
                    BitmapDataOwner = null;
                    break;
            }
        }

        private void OnViewDirectCommand()
        {
            IntPtr hwnd = GetHwndCallback?.Invoke() ?? IntPtr.Zero;

            try
            {
                switch (TestObject)
                {
                    case Image image:
                        if (!ImageFromFile && AsImage || ImageFromFile && FileAsImage)
                        {
                            Image? newImage = DebuggerHelper.DebugImage(image, !AsReadOnly, hwnd);
                            if (newImage != image)
                                TestObject = newImage;
                        }
                        else if (image is Metafile metafile)
                        {
                            Metafile? newMetafile = DebuggerHelper.DebugMetafile(metafile, !AsReadOnly, hwnd);
                            if (newMetafile != image)
                                TestObject = newMetafile;
                        }
                        else if (image is Bitmap bitmap)
                        {
                            Bitmap? newBitmap = DebuggerHelper.DebugBitmap(bitmap, !AsReadOnly, hwnd);
                            if (newBitmap != bitmap)
                                TestObject = newBitmap;
                        }

                        break;

                    case Icon icon:
                        Icon? newIcon = DebuggerHelper.DebugIcon(icon, !AsReadOnly, hwnd);
                        if (newIcon != icon)
                            TestObject = newIcon;
                        break;

                    case BitmapData bitmapData:
                        DebuggerHelper.DebugBitmapData(bitmapData, hwnd);
                        break;

                    case Graphics graphics:
                        DebuggerHelper.DebugGraphics(graphics, hwnd);
                        break;

                    case ColorPalette palette:
                        ColorPalette? newPalette = DebuggerHelper.DebugPalette(palette, !AsReadOnly, hwnd);
                        if (newPalette != null)
                        {
                            TestObject = newPalette;
                            if (ReferenceEquals(palette, newPalette))
                                OnPropertyChanged(new PropertyChangedExtendedEventArgs(palette, newPalette, nameof(TestObject)));
                        }

                        break;

                    case Color color:
                        Color? newColor = DebuggerHelper.DebugColor(color, !AsReadOnly, hwnd);
                        if (newColor != null)
                            TestObject = newColor;
                        break;

                    default:
                        throw new InvalidOperationException($"Unexpected object type: {TestObject?.GetType()}");
                }
            }
            catch (Exception e) when (e is not StackOverflowException)
            {
                ErrorCallback?.Invoke($"Failed to view object: {e.Message}");
            }
        }

        private void OnDebugCommand()
        {
            object? testObject = TestObject;
            if (testObject == null)
                return;

            Type targetType = testObject is Image && (!ImageFromFile && AsImage || ImageFromFile && FileAsImage)
                ? typeof(Image)
                : testObject.GetType();
            DebuggerVisualizerAttribute? attr = debuggerVisualizers.GetValueOrDefault(targetType);
            if (attr == null)
            {
                ErrorCallback?.Invoke($"No debugger visualizer found for type {targetType}");
                return;
            }

            try
            {
                var windowService = new TestWindowService();
                var objectProvider = new TestObjectProvider(testObject) { IsObjectReplaceable = !AsReadOnly };
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
