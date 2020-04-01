#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DebuggerTestFormViewModel.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2019 - All Rights Reserved
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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

using KGySoft.ComponentModel;
using KGySoft.Drawing.DebuggerVisualizers.Serializers;
using KGySoft.Drawing.ImagingTools;
using KGySoft.Reflection;

using Microsoft.VisualStudio.DebuggerVisualizers;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Test.ViewModel
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
                nameof(Bmp32), nameof(Bmp16), nameof(Bmp8),
                nameof(Metafile),
                nameof(HIcon), nameof(ManagedIcon),
                nameof(GraphicsBitmap), nameof(GraphicsHwnd),
                nameof(BitmapData32), nameof(BitmapData8),
                nameof(Palette256), nameof(Palette2), nameof(SingleColor),
                nameof(ImageFromFile)
            },
            new HashSet<string> { nameof(AsImage), nameof(AsBitmap), nameof(AsMetafile),nameof(AsIcon) },
        };

        #endregion

        #region Properties

        internal bool Bmp32 { get => Get<bool>(); set => Set(value); }
        internal bool Bmp16 { get => Get<bool>(); set => Set(value); }
        internal bool Bmp8 { get => Get<bool>(); set => Set(value); }
        internal bool Metafile { get => Get<bool>(); set => Set(value); }
        internal bool HIcon { get => Get<bool>(); set => Set(value); }
        internal bool ManagedIcon { get => Get<bool>(); set => Set(value); }
        internal bool GraphicsBitmap { get => Get<bool>(); set => Set(value); }
        internal bool GraphicsHwnd { get => Get<bool>(); set => Set(value); }
        internal bool BitmapData32 { get => Get<bool>(); set => Set(value); }
        internal bool BitmapData8 { get => Get<bool>(); set => Set(value); }
        internal bool Palette256 { get => Get<bool>(); set => Set(value); }
        internal bool Palette2 { get => Get<bool>(); set => Set(value); }
        internal bool SingleColor { get => Get<bool>(); set => Set(value); }

        internal bool ImageFromFile { get => Get<bool>(); set => Set(value); }
        internal string FileName { get => Get<string>(); set => Set(value); }
        internal bool AsImage { get => Get<bool>(); set => Set(value); }
        internal bool AsBitmap { get => Get<bool>(); set => Set(value); }
        internal bool AsMetafile { get => Get<bool>(); set => Set(value); }
        internal bool AsIcon { get => Get<bool>(); set => Set(value); }

        // TODO: UI
        internal bool AsReadOnly { get => Get<bool>(); set => Set(value); }

        internal object TestObject { get => Get<object>(); set => Set(value); }
        internal Image PreviewImage { get => Get<Image>(); set => Set(value); }
        internal ImageTypes ImageTypes { get => Get<ImageTypes>(); set => Set(value); }
        internal bool CanDebugDirectly { get => Get<bool>(); set => Set(value); }
        internal bool CanDebugByDebugger { get => Get<bool>(); set => Set(value); }
        internal Bitmap BitmapDataOwner { get => Get<Bitmap>(); set => Set(value); }
        internal Action<string> ErrorCallback { get => Get<Action<string>>(); set => Set(value); }

        internal Func<IntPtr> GetHwndCallback { get => Get<Func<IntPtr>>(); set => Set(value); }
        internal Func<Rectangle> GetClipCallback { get => Get<Func<Rectangle>>(); set => Set(value); }

        internal ICommand DebugCommand => Get(() => new SimpleCommand(OnDebugCommand));
        internal ICommand DirectViewCommand => Get(() => new SimpleCommand(OnViewDirectCommand));

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.NewValue is true && radioGroups.FirstOrDefault(g => g.Contains(e.PropertyName)) is IEnumerable<string> group)
            {
                AdjustRadioGroup(e.PropertyName, group);
                TestObject = GenerateObject();
                return;
            }

            if (e.PropertyName == nameof(FileName) && ImageFromFile)
            {
                TestObject = GenerateObject();
                return;
            }

            if (e.PropertyName == nameof(TestObject))
            {
                var obj = TestObject;
                ImageTypes = GetImageTypes(obj);
                PreviewImage = GetPreviewImage(obj);
                CanDebugByDebugger = obj != null;
                CanDebugDirectly = obj != null && !(obj is Graphics || obj is BitmapData);
            }
        }

        protected override void Dispose(bool disposing)
        {
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

        private object GenerateObject()
        {
            FreeTestObject();

            // actually the transient steps should be disposed, too... as this is just a test, now we rely on the destructor
            if (Bmp32)
                return Icons.Shield.ExtractBitmap(0);
            if (Bmp16)
                return Icons.Shield.ExtractBitmap(0).ConvertPixelFormat(PixelFormat.Format16bppRgb565);
            if (Bmp8)
                return Icons.Shield.ExtractBitmap(0).ConvertPixelFormat(PixelFormat.Format8bppIndexed);
            if (Metafile)
                return GenerateMetafile();
            if (HIcon)
                return SystemIcons.Application;
            if (ManagedIcon)
                return Icons.Application;
            if (GraphicsBitmap)
                return Graphics.FromImage(Icons.Shield.ExtractBitmap(0));
            if (GraphicsHwnd)
                return GetWindowGraphics();
            if (BitmapData32)
                return GetBitmapData(PixelFormat.Format32bppArgb);
            if (BitmapData8)
                return GetBitmapData(PixelFormat.Format8bppIndexed);
            if (Palette256)
                return new Bitmap(1, 1, PixelFormat.Format8bppIndexed).Palette;
            if (Palette2)
                return new Bitmap(1, 1, PixelFormat.Format1bppIndexed).Palette;
            if (SingleColor)
                return Color.Red;
            if (ImageFromFile)
                return FromFile(FileName);

            return null;
        }

        private Image GetPreviewImage(object obj)
        {
            switch (obj)
            {
                case Image image:
                    return image;
                case Icon icon:
                    return icon.ToMultiResBitmap();
                case Graphics graphics:
                    return graphics.ToBitmap(false);
                case BitmapData _:
                    return (Image)BitmapDataOwner.Clone();
                case ColorPalette palette:
                    return FromPalette(palette.Entries);
                case Color color:
                    return FromPalette(new[] { color });
                default:
                    return null;
            }
        }

        private Image FromPalette(IList<Color> palette)
        {
            var size = palette.Count;
            var result = new Bitmap(size, size, PixelFormat.Format32bppArgb);
            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                    result.SetPixel(x, y, palette[x]);
            return result;
        }

        private ImageTypes GetImageTypes(object obj)
        {
            if (ImageFromFile)
            {
                if (AsImage)
                    return ImageTypes.All;
                if (AsBitmap)
                    return ImageTypes.Bitmap;
                if (AsMetafile)
                    return ImageTypes.Metafile;
                if (AsIcon)
                    return ImageTypes.Icon;
            }

            switch (obj)
            {
                case Bitmap _:
                    return ImageTypes.Bitmap;
                case Metafile _:
                    return ImageTypes.Metafile;
                case Icon _:
                    return ImageTypes.Icon;
                default:
                    return ImageTypes.None;
            }
        }

        private object FromFile(string fileName)
        {
            try
            {
                if (fileName == null || !File.Exists(fileName))
                    return null;
                var stream = new MemoryStream(File.ReadAllBytes(fileName));
                if (AsIcon)
                    return Icons.FromStream(stream);
                var image = Image.FromStream(stream);
                if (AsBitmap && !(image is Bitmap))
                {
                    image.Dispose();
                    ErrorCallback?.Invoke("The file is not a Bitmap");
                    return null;
                }

                if (AsMetafile && !(image is Metafile))
                {
                    image.Dispose();
                    ErrorCallback?.Invoke("The file is not a Metafile");
                    return null;
                }

                return image;
            }
            catch (Exception e)
            {
                ErrorCallback?.Invoke($"Could not open file: {e.Message}");
                return null;
            }
        }

        private Metafile GenerateMetafile()
        {
            //Set up reference Graphic
            Graphics refGraph = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr hdc = refGraph.GetHdc();
            Metafile result = new Metafile(hdc, new Rectangle(0, 0, 100, 100), MetafileFrameUnit.Pixel, EmfType.EmfOnly, "Test");

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

            refGraph.ReleaseHdc(hdc); //cleanup
            refGraph.Dispose();
            return result;
        }

        private object GetWindowGraphics()
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
            var bitmap = Icons.Shield.ExtractBitmap(0);
            if (pixelFormat != bitmap.PixelFormat)
                bitmap = bitmap.ConvertPixelFormat(pixelFormat);
            BitmapDataOwner = bitmap;
            return bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size), ImageLockMode.ReadOnly, bitmap.PixelFormat);
        }

        private void FreeTestObject()
        {
            switch (TestObject)
            {
                case IDisposable disposable:
                    disposable.Dispose();
                    break;
                case BitmapData bitmapData:
                    var bitmap = BitmapDataOwner;
                    bitmap.UnlockBits(bitmapData);
                    bitmap.Dispose();
                    BitmapDataOwner = null;
                    break;
            }
        }

        private void OnViewDirectCommand()
        {
            IntPtr hwnd = GetHwndCallback?.Invoke() ?? IntPtr.Zero;

            switch (TestObject)
            {
                case Image image:
                    Image newImage = DebuggerHelper.DebugImage(image, AsReadOnly, hwnd);
                    if (newImage != null)
                    {
                        if (TestObject == newImage)
                            TestObject = null;
                        TestObject = newImage;
                    }

                    break;

                case Icon icon:
                    Icon newIcon = DebuggerHelper.DebugIcon(icon, AsReadOnly, hwnd);
                    if (newIcon != null)
                        TestObject = newIcon;
                    break;

                case ColorPalette palette:
                    ColorPalette newPalette = DebuggerHelper.DebugPalette(palette, AsReadOnly, hwnd);
                    if (newPalette != null)
                    {
                        if (TestObject == newPalette)
                            TestObject = null;
                        TestObject = newPalette;
                    }

                    break;

                case Color color:
                    Color? newColor = DebuggerHelper.DebugColor(color, AsReadOnly, hwnd);
                    if (newColor != null)
                        TestObject = newColor;
                    break;
            }
        }

        private void OnDebugCommand()
        {
            var windowService = new TestWindowService();
            var objectProvider = new TestObjectProvider(TestObject);
            DialogDebuggerVisualizer debugger;

            // TODO: visualizer and serializer by reflection
            switch (TestObject)
            {
                case Image _:
                    debugger = !ImageFromFile || AsImage ? new ImageDebuggerVisualizer()
                        : AsMetafile ? new MetafileDebuggerVisualizer()
                        : (DialogDebuggerVisualizer)new BitmapDebuggerVisualizer();
                    objectProvider.Serializer = new ImageSerializer();
                    objectProvider.IsObjectReplaceable = true;
                    break;
                case Icon _:
                    debugger = new IconDebuggerVisualizer();
                    objectProvider.Serializer = new IconSerializer();
                    objectProvider.IsObjectReplaceable = true;
                    break;
                case Graphics _:
                    debugger = new GraphicsDebuggerVisualizer();
                    objectProvider.Serializer = new GraphicsSerializer();
                    break;
                case BitmapData _:
                    debugger = new BitmapDataDebuggerVisualizer();
                    objectProvider.Serializer = new BitmapDataSerializer();
                    break;
                case ColorPalette _:
                    debugger = new PaletteDebuggerVisualizer();
                    objectProvider.IsObjectReplaceable = true;
                    objectProvider.Serializer = new AnySerializer();
                    break;
                case Color _:
                    debugger = new ColorDebuggerVisualizer();
                    objectProvider.IsObjectReplaceable = true;
                    objectProvider.Serializer = new VisualizerObjectSource();
                    break;
                default:
                    return;
            }

            Reflector.InvokeMethod(debugger, "Show", windowService, objectProvider);
            if (objectProvider.ObjectReplaced)
                TestObject = objectProvider.Object;
        }

        #endregion

        #endregion
    }
}
