using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using KGySoft.ComponentModel;

namespace KGySoft.Drawing.DebuggerVisualizers.Test.ViewModel
{
    internal class DebuggerTestFormViewModel : ObservableObjectBase
    {
        private static readonly HashSet<string>[] radioGroups =
        {
            new HashSet<string>
            {
                nameof(Bmp32), nameof(Bmp16), nameof(Bmp4),
                nameof(Metafile),
                nameof(HIcon), nameof(ManagedIcon),
                nameof(GraphicsBitmap), nameof(GraphicsControl),
                nameof(BitmapData32), nameof(BitmapData8),
                nameof(Palette256), nameof(Palette2), nameof(Color),
                nameof(ImageFromFile)
            },
            new HashSet<string> { nameof(AsImage), nameof(AsBitmap), nameof(AsMetafile),nameof(AsIcon) },
        };

        internal bool Bmp32 { get => Get<bool>(); set => Set(value); }
        internal bool Bmp16 { get => Get<bool>(); set => Set(value); }
        internal bool Bmp4 { get => Get<bool>(); set => Set(value); }
        internal bool Metafile { get => Get<bool>(); set => Set(value); }
        internal bool HIcon { get => Get<bool>(); set => Set(value); }
        internal bool ManagedIcon { get => Get<bool>(); set => Set(value); }
        internal bool GraphicsBitmap { get => Get<bool>(); set => Set(value); }
        internal bool GraphicsControl { get => Get<bool>(); set => Set(value); }
        internal bool BitmapData32 { get => Get<bool>(); set => Set(value); }
        internal bool BitmapData8 { get => Get<bool>(); set => Set(value); }
        internal bool Palette256 { get => Get<bool>(); set => Set(value); }
        internal bool Palette2 { get => Get<bool>(); set => Set(value); }
        internal bool Color { get => Get<bool>(); set => Set(value); }
        internal bool ImageFromFile { get => Get<bool>(); set => Set(value); }

        internal bool AsImage { get => Get<bool>(); set => Set(value); }
        internal bool AsBitmap { get => Get<bool>(); set => Set(value); }
        internal bool AsMetafile { get => Get<bool>(); set => Set(value); }
        internal bool AsIcon { get => Get<bool>(); set => Set(value); }

        internal object TestObject { get => Get<object>(); set => Set(value); }

        public Image PreviewImage { get => Get<Image>(); set => Set(value); }

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.NewValue is true && radioGroups.FirstOrDefault(g => g.Contains(e.PropertyName)) is IEnumerable<string> group)
            {
                AdjustRadioGroup(e.PropertyName, group);
                TestObject = GenerateObject();
                return;
            }

            if (e.PropertyName == nameof(TestObject))
                PreviewImage = GetPreviewImage(TestObject);
        }

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
            (TestObject as IDisposable)?.Dispose();
            if (Bmp32)
                return Icons.Shield.ExtractBitmap(0);

            return null;
        }

        private Image GetPreviewImage(object obj)
        {
            if (obj is Image image)
                return image;
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                (TestObject as IDisposable)?.Dispose();
        }
    }
}
