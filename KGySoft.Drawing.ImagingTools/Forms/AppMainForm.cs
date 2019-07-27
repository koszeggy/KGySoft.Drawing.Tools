using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace KGySoft.Drawing.ImagingTools.Forms
{
    internal partial class AppMainForm : ImageDebuggerVisualizerForm
    {
        private const string title = "KGy SOFT Imaging Tools";

        public AppMainForm()
        {
            InitializeComponent();

            Notification = $"As a standalone application, {title} can be used to load images, save them in various formats, extract frames or pages, examine or change palette entries of indexed images, etc.{Environment.NewLine}{Environment.NewLine}"
                + $"But it can be used also as a debugger visualizer for {nameof(Image)}, {nameof(Bitmap)}, {nameof(Metafile)}, {nameof(BitmapData)}, {nameof(Graphics)}, {nameof(ColorPalette)} and {nameof(Color)} types."
                + $"Use the '{btnConfiguration.Text}' button.";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();
            base.Dispose(disposing);
        }

        public override string Text
        {
            get => base.Text;
            set => base.Text = String.IsNullOrEmpty(value) ? title : $"{title} - {value}";
        }
    }
}
