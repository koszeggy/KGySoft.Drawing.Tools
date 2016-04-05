using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KGySoft.DebuggerVisualizers.Common
{
    internal partial class BitmapDataDebuggerVisualizerForm : ImageDebuggerVisualizerForm
    {
        public BitmapDataDebuggerVisualizerForm()
        {
            InitializeComponent();
        }

        internal string SpecialInfo { get; set; }

        protected override void UpdateInfo()
        {
            if (Image == null)
                return;

            Text = String.Format("Type: BitmapData; Size: {0}", Image.Size);
            txtInfo.Text = SpecialInfo;
        }

        protected override void ImageChanged()
        {
            base.ImageChanged();
            ImageData image = GetCurrentImage();
            if (image == null || image.Image == null)
                return;
            if (Image.GetPixelFormatSize(GetCurrentImage().PixelFormat) <= 8)
                Warning = "The palette of an indexed BitmapData cannot be reconstructed, therefore a default palette is used. You can change palette colors in the menu.";
        }

        protected override bool IsPaletteReadOnly
        {
            get { return false; }
        }
    }
}
