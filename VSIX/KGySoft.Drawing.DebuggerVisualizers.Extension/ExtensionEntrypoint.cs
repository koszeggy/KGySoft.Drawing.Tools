using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Extensibility;

namespace KGySoft.Drawing.DebuggerVisualizers.Extension
{
    /// <summary>
    /// Extension entrypoint for the VisualStudio.Extensibility extension.
    /// </summary>
    [VisualStudioContribution]
    internal class ExtensionEntrypoint : Microsoft.VisualStudio.Extensibility.Extension
    {
        /// <inheritdoc />
        public override ExtensionConfiguration ExtensionConfiguration => new()
        {
            // Needed for out-of-process hosting of non-VSSDK extensions when there is no vsixmanifest file
            Metadata = new ExtensionMetadata("acb4aeb6-77ea-465f-a305-982511fe70c3",
                this.ExtensionAssemblyVersion,
                "KGy SOFT",
                "KGy SOFT Image DebuggerVisualizers x64",
                "Debugger visualizers for GDI+, WPF, SkiaSharp and KGy SOFT types like Bitmap, BitmapSource, SKBitmap, Metafile, ImageSource, SKImage, Icon, Graphics, SKSurface, BitmapData, SKPixmap, WriteableBitmap, ColorPalette, BitmapPalette and more."),
            //RequiresInProcessHosting = true,
        };

        /// <inheritdoc />
        protected override void InitializeServices(IServiceCollection serviceCollection)
        {
            base.InitializeServices(serviceCollection);

            // You can configure dependency injection here by adding services to the serviceCollection.
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
