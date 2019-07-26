using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using KGySoft.CoreLibraries;

namespace KGySoft.Drawing.ImagingTools.Forms
{
    internal partial class AppMainForm : ImageDebuggerVisualizerForm
    {
        private const string title = "KGy SOFT Imaging Tools";

        private readonly ToolStripButton btnInstall;

        public AppMainForm()
        {
            InitializeComponent();

            Notification = $"As a standalone application, {title} can be used to load images, save them various formats, extract frames or pages, examine or change palette entries of indexed images, etc.{Environment.NewLine}{Environment.NewLine}"
                + $"But it can be also used as debugger visualizer for {nameof(Image)}, {nameof(Bitmap)}, {nameof(Metafile)}, {nameof(BitmapData)}, {nameof(Graphics)}, {nameof(ColorPalette)} and {nameof(Color)} types."
                + $"Use the Install menu to manage installations.";

            ToolStripItem separator = new ToolStripSeparator();
            btnInstall = new ToolStripButton(Properties.Resources.Gear)
            {
                Text = "Manage debugger visualizer installations",
                DisplayStyle = ToolStripItemDisplayStyle.Image
            };

            btnInstall.Click += btnInstall_Click;

            tsMenu.Items.AddRange(new ToolStripItem[] { separator, btnInstall });

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();
            btnInstall.Click -= btnInstall_Click;
            base.Dispose(disposing);
        }


        private void btnInstall_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public override string Text
        {
            get => base.Text;
            set => base.Text = String.IsNullOrEmpty(value) ? title : $"{title} - {value}";
        }
    }
}
