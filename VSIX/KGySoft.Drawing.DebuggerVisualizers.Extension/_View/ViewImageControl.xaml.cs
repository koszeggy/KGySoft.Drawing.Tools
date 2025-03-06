using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.VisualStudio.Extensibility.DebuggerVisualizers;
using Microsoft.VisualStudio.RpcContracts.DebuggerVisualizers;

namespace KGySoft.Drawing.DebuggerVisualizers.Extension
{
    /// <summary>
    /// This control is used when debugging in-process. Here the data context is a local object and does not require the [DataContract] attribute,
    /// and even the Image property works.
    /// </summary>
    public partial class ViewImageControl : UserControl
    {
        private readonly VisualizerTarget visualizerTarget;
        private readonly ImageData dataContext;

        public ViewImageControl(VisualizerTarget visualizerTarget)
        {
            InitializeComponent();

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

                dataContext.Image = BitmapFrame.Create(new MemoryStream(data.ToArray()));
            }
            catch (Exception e)
            {
                dataContext.Image = null;
                dataContext.Info = e.Message;
            }
        }

        //protected override void Dispose(bool disposing)
        //{
        //    //dataContext.Dispose();
        //    base.Dispose(disposing);
        //}

    }
}
