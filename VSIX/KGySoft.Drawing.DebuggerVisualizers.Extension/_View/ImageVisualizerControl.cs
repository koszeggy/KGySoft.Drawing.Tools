using System.Buffers;
using System.IO;
using System.Windows.Media.Imaging;

using Microsoft.VisualStudio.Extensibility.DebuggerVisualizers;
using Microsoft.VisualStudio.Extensibility.UI;
using Microsoft.VisualStudio.RpcContracts.DebuggerVisualizers;

namespace KGySoft.Drawing.DebuggerVisualizers.Extension
{
    /// <summary>
    /// This control is used when debugging out-of-process. Here the data context is a remote object and requires the [DataContract] attribute.
    /// Unfortunately, the Image property does not work here, because the serialization of BitmapSource is not supported.
    /// </summary>
    internal class ImageVisualizerControl : RemoteUserControl
    {
        private readonly VisualizerTarget visualizerTarget;
        private readonly ImageData dataContext;

        public ImageVisualizerControl(VisualizerTarget visualizerTarget) : base(new ImageData())
        {
            visualizerTarget.StateChanged += VisualizerTarget_StateChanged;

            this.dataContext = (ImageData)DataContext!;
            this.visualizerTarget = visualizerTarget;
        }

        private Task VisualizerTarget_StateChanged(object? sender, VisualizerTargetStateNotification args)
        {
            switch (args)
            {
                case VisualizerTargetStateNotification.Available:
                case VisualizerTargetStateNotification.ValueUpdated:
                    return UpdateImageData();
            }

            return Task.CompletedTask;
        }

        private async Task UpdateImageData()
        {
            try
            {
                ReadOnlySequence<byte> data = (await visualizerTarget.ObjectSource.RequestDataAsync(default(ReadOnlySequence<byte>), CancellationToken.None))!.Value;
                dataContext.Info = $"{data.Length} bytes";
                //using var ms = new MemoryStream(data.ToArray());
                //using ImageInfo imageInfo = SerializationHelper.DeserializeImageInfo(ms);

                // This will not work because BitmapSource is not serializable
                dataContext.Image = BitmapFrame.Create(new MemoryStream(data.ToArray()));
            }
            catch (Exception e)
            {
                dataContext.Image = null;
                dataContext.Info = e.Message;
            }
        }

        protected override void Dispose(bool disposing)
        {
            //dataContext.Dispose();
            base.Dispose(disposing);
        }
    }
}