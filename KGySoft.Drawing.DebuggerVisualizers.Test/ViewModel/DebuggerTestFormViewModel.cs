using System;
using System.Collections.Generic;
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

        public bool Bmp32 { get => Get(true); set => Set(value); }
        public bool Bmp16 { get => Get<bool>(); set => Set(value); }
        public bool Bmp4 { get => Get<bool>(); set => Set(value); }
        public bool Metafile { get => Get<bool>(); set => Set(value); }
        public bool HIcon { get => Get<bool>(); set => Set(value); }
        public bool ManagedIcon { get => Get<bool>(); set => Set(value); }
        public bool GraphicsBitmap { get => Get<bool>(); set => Set(value); }
        public bool GraphicsControl { get => Get<bool>(); set => Set(value); }
        public bool BitmapData32 { get => Get<bool>(); set => Set(value); }
        public bool BitmapData8 { get => Get<bool>(); set => Set(value); }
        public bool Palette256 { get => Get<bool>(); set => Set(value); }
        public bool Palette2 { get => Get<bool>(); set => Set(value); }
        public bool Color { get => Get<bool>(); set => Set(value); }
        public bool ImageFromFile { get => Get<bool>(); set => Set(value); }

        public bool AsImage { get => Get(true); set => Set(value); }
        public bool AsBitmap { get => Get<bool>(); set => Set(value); }
        public bool AsMetafile { get => Get<bool>(); set => Set(value); }
        public bool AsIcon { get => Get<bool>(); set => Set(value); }

        public object TestObject { get => Get(GenerateObject); set => Set(value); }


        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.NewValue is true && radioGroups.FirstOrDefault(g => g.Contains(e.PropertyName)) is IEnumerable<string> group)
            {
                AdjustRadioGroup(e.PropertyName, group);
                TestObject = GenerateObject();
            }
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
            throw new NotImplementedException();
        }


    }
}
