#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DebuggerTestViewModel.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2023 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using KGySoft.Reflection;

using Microsoft.VisualStudio.DebuggerVisualizers;

#region Used Namespaces

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using KGySoft.ComponentModel;
#if NETFRAMEWORK
using KGySoft.CoreLibraries; 
#endif
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Drawing.Wpf;

#endregion

#region Used Aliases

using Color = System.Windows.Media.Color;

#endregion

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Wpf.Test.ViewModel
{
    internal class DebuggerTestViewModel : ObservableObjectBase
    {
        #region Fields

        private static readonly HashSet<string> radioButtons = new HashSet<string>
        {
            nameof(BitmapSource),
            nameof(ImageSource),
            nameof(Palette),
            nameof(SingleColorSrgb),
            nameof(SingleColorLinear),
            nameof(SingleColorFromProfile),
        };

        private static readonly Dictionary<Type, DebuggerVisualizerAttribute> debuggerVisualizers = WpfDebuggerHelper.GetDebuggerVisualizers();

        #endregion

        #region Properties

        #region Public Properties

        public PixelFormat[] PixelFormats { get; } = typeof(PixelFormats).GetProperties()
            .Where(p => p.Name != nameof(System.Windows.Media.PixelFormats.Default))
            .Select(p => (PixelFormat)p.GetValue(null, null)!)
            .ToArray();

        public PixelFormat SelectedFormat { get => Get(System.Windows.Media.PixelFormats.Indexed8); set => Set(value); }
        public ImageSource? PreviewImage { get => Get<ImageSource?>(); set => Set(value); }
        public bool BitmapSource { get => Get(true); set => Set(value); }
        public bool ImageSource { get => Get<bool>(); set => Set(value); }
        public bool Palette { get => Get<bool>(); set => Set(value); }
        public bool SingleColorSrgb { get => Get<bool>(); set => Set(value); }
        public bool SingleColorLinear { get => Get<bool>(); set => Set(value); }
        public bool SingleColorFromProfile { get => Get<bool>(); set => Set(value); }

        public ICommand DirectViewCommand => Get(() => new SimpleCommand(OnViewDirectCommand));
        public ICommand DebugCommand => Get(() => new SimpleCommand(OnDebugCommand));

        public ICommandState DebugCommandState => Get(() => new CommandState { Enabled = false });

        #endregion

        #region Internal Properties

        internal Action<string>? ErrorCallback { get => Get<Action<string>?>(); set => Set(value); }
        internal Func<IntPtr>? GetHwndCallback { get => Get<Func<IntPtr>?>(); set => Set(value); }

        #endregion

        #region Private Properties

        private object? TestObject { get => Get<object?>(); set => Set(value); }

        #endregion

        #endregion

        #region Methods

        #region Static Methods

        private static ImageSource GenerateDrawingImage()
        {
            var result = new DrawingGroup();
            var pen = new System.Windows.Media.Pen();
            result.Children.Add(new GeometryDrawing(new SolidColorBrush(Color.FromRgb(0x9B, 0x50, 0x96)), pen, Geometry.Parse("M0,95A1,1,0,0,1,100,-5")));
            result.Children.Add(new GeometryDrawing(new SolidColorBrush(Color.FromRgb(0x67, 0x16, 0x78)), pen, Geometry.Parse("M0,95A1,1,0,0,0,100,-5")));
            result.Children.Add(new GeometryDrawing(new SolidColorBrush(Colors.White), pen, Geometry.Parse("F1M3.65625,18.5L12.84375,18.5 12.84375,38.34375 15.75,33.4375 24.21875,18.5 35.46875,18.5 22.4375,38.5 35.6875,64 24.78125,64 16.46875,46.4375 12.84375,51 12.84375,64 3.65625,64 3.65625,18.5z")));
            result.Children.Add(new GeometryDrawing(new SolidColorBrush(Colors.White), pen, Geometry.Parse("F1M54.46875,17.875C59.40625,17.875 63.13020706176758,19.09375 65.640625,21.53125 68.15103912353516,23.96875 69.60416412353516,27.791667938232422 70,33L61.0625,33C60.8125,30.104167938232422 60.20833206176758,28.130210876464844 59.25,27.078125 58.291664123535156,26.026042938232422 56.791664123535156,25.5 54.75,25.5 52.27083206176758,25.5 50.46875,26.40625 49.34375,28.21875 48.21875,30.03125 47.635414123535156,32.91666793823242 47.59375,36.875L47.59375,45.125C47.59375,49.270835876464844 48.213539123535156,52.286460876464844 49.453125,54.171875 50.69270706176758,56.05729293823242 52.729164123535156,57 55.5625,57 57.375,57 58.84375,56.63541793823242 59.96875,55.90625L60.78125,55.34375 60.78125,47 54.34375,47 54.34375,40.0625 70,40.0625 70,58.84375C68.22916412353516,60.76041793823242 66.05728912353516,62.203125 63.484375,63.171875 60.91145706176758,64.140625 58.09375,64.625 55.03125,64.625 49.80208206176758,64.625 45.73958206176758,63.00520706176758 42.84375,59.765625 39.94791793823242,56.52604293823242 38.45833206176758,51.8125 38.375,45.625L38.375,37.4375C38.375,31.166667938232422 39.74479293823242,26.338542938232422 42.484375,22.953125 45.22395706176758,19.567710876464844 49.21875,17.875 54.46875,17.875z")));
            result.Children.Add(new GeometryDrawing(new SolidColorBrush(Colors.White), pen, Geometry.Parse("F1M73.34375,30.1875L82.84375,30.1875 86.90625,49.28125 91.40625,30.1875 100.84375,30.1875 89.71875,69.0625C88.05208587646484,74.79166412353516 84.91666412353516,77.65625 80.3125,77.65625 79.25,77.65625 78.04166412353516,77.45833587646484 76.6875,77.0625L76.6875,70.15625 77.71875,70.1875C79.09375,70.1875 80.13021087646484,69.921875 80.828125,69.390625 81.52603912353516,68.859375 82.05208587646484,67.9375 82.40625,66.625L83.09375,64.34375 73.34375,30.1875z")));
            return new DrawingImage(result);
        }

        private static BitmapSource FromPalette(BitmapPalette palette, PixelFormat format)
        {
            int size = palette.Colors.Count;
            var result = new WriteableBitmap(size, size, 96, 96, format, palette);
            using var bitmapData = result.GetReadWriteBitmapData();
            for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                bitmapData.SetColor32(x, y, palette.Colors[x].ToColor32());
            return result;
        }

        #endregion

        #region Instance Methods

        #region Internal Methods

        internal void ViewLoaded() => TestObject = GenerateObject();

        #endregion

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.NewValue is true && radioButtons.Contains(e.PropertyName!))
            {
                AdjustRadioButtons(e.PropertyName!, radioButtons);
                TestObject = GenerateObject();
                return;
            }

            if (e.PropertyName is nameof(SelectedFormat))
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
                FreeTestObject();
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void AdjustRadioButtons(string propertyName, IEnumerable<string> group)
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
                if (BitmapSource)
                {
                    using Icon icon = Icons.Shield;
                    using Bitmap bmp = icon.ExtractBitmap(0)!;
                    using IReadableBitmapData bmpData = bmp.GetReadableBitmapData();
                    return bmpData.ToWriteableBitmap(SelectedFormat);
                }

                if (ImageSource)
                    return GenerateDrawingImage();

                if (Palette)
                {
                    PixelFormat format = SelectedFormat;
                    return format.IsIndexed()
                        ? BitmapDataFactory.CreateBitmapData(new Size(1, 1)).ToWriteableBitmap(format).Palette
                        : null;
                }

                if (SingleColorSrgb)
                    return Colors.Red;

                if (SingleColorLinear)
                    return Color.FromScRgb(1f, 0f, 1f, 0f);

                if (SingleColorFromProfile)
                    return Color.FromValues([0f, 0f, 1f], new Uri("C:\\Windows\\System32\\spool\\drivers\\color\\AdobeRGB1998.icc"));

                // TODO
                //if (ImageFromFile)
                //    return FromFile(FileName);
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
                    case ImageSource image:
                        return image;
                    case BitmapPalette palette:
                        return FromPalette(palette, SelectedFormat);
                    case Color color:
                        return FromPalette(new BitmapPalette(new[] { color }), System.Windows.Media.PixelFormats.Indexed1);
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
            switch (obj)
            {
                case IDisposable disposable:
                    disposable.Dispose();
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
                    case BitmapSource bitmapSource:
                        DebuggerHelper.DebugCustomBitmap(new CustomBitmapInfo(true)
                        {
                            Type = bitmapSource.GetType().Name,
                            ShowPixelSize = true,
                            BitmapData = bitmapSource.GetReadableBitmapData(),
                        }, hwnd);
                        return;

                    case ImageSource imageSource:
                        // Cheating: using ImageSourceSerializationInfo.ToBitmapSource
                        var typeImageSourceSerializationInfo = Reflector.ResolveType(typeof(WpfDebuggerHelper).Assembly, "KGySoft.Drawing.DebuggerVisualizers.Wpf.Serialization.ImageSourceSerializationInfo")!;
                        var asBitmap = (BitmapSource)Reflector.InvokeMethod(typeImageSourceSerializationInfo, "ToBitmapSource", imageSource)!;
                        DebuggerHelper.DebugCustomBitmap(new CustomBitmapInfo(true)
                        {
                            Type = imageSource.GetType().Name,
                            ShowPixelSize = false,
                            BitmapData = asBitmap.GetReadableBitmapData(),
                        }, hwnd);
                        return;

                    case BitmapPalette palette:
                        var paletteInfo = new CustomPaletteInfo { Type = palette.GetType().Name };
                        foreach (Color color in palette.Colors)
                        {
                            paletteInfo.Entries.Add(new CustomColorInfo
                            {
                                Type = color.GetType().Name,
                                DisplayColor = color.ToColor32(),
                                Name = color.ToString()
                            });
                        }

                        DebuggerHelper.DebugCustomPalette(paletteInfo, hwnd);
                        return;

                    case Color color:
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

        private void OnDebugCommand()
        {
            object? testObject = TestObject;
            if (testObject == null)
                return;

            Type targetType = testObject is ImageSource
                ? typeof(ImageSource)
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
