using System;
using System.Buffers;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Microsoft.VisualStudio.DebuggerVisualizers;
//using System.Windows.Forms.Integration;


//using Microsoft.VisualStudio.DebuggerVisualizers;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.DebuggerVisualizers;
using Microsoft.VisualStudio.Extensibility.VSSdkCompatibility; // TODO: remove along with Microsoft.VisualStudio.Shell.15.0 package reference
using Microsoft.VisualStudio.RpcContracts.RemoteUI;
using Microsoft.VisualStudio.Shell;

using StandardSerializerTest;



namespace KGySoft.Drawing.DebuggerVisualizers.Extension
{
    // The serializer must be in a separate class library project when we have an out-of-proc visualizer.
    //using Image = System.Drawing.Image;
    //public class ImageSerializer : VisualizerObjectSource
    //{
    //    #region Methods

    //    /// <summary>
    //    /// Called when the object to be debugged is about to be serialized
    //    /// </summary>
    //    public override void GetData(object target, Stream outgoingData) =>
    //        ((Image)target).Save(outgoingData, ImageFormat.Png);

    //    /// <summary>
    //    /// Called by the new type of visualizer debuggers when VisualizerTarget.ObjectSource.RequestDataAsync is called
    //    /// </summary>
    //    public override void TransferData(object target, Stream incomingData, Stream outgoingData) => GetData(target, outgoingData);

    //    ///// <summary>
    //    ///// Called when the debugged object has been replaced
    //    ///// </summary>
    //    //public override object? CreateReplacementObject(object target, Stream incomingData) => SerializationHelper.DeserializeReplacementImage(incomingData);

    //    #endregion
    //}

    [VisualStudioContribution]
    internal class ImageDebuggerVisualizerProvider : DebuggerVisualizerProvider
    {
        public override DebuggerVisualizerProviderConfiguration DebuggerVisualizerProviderConfiguration => new DebuggerVisualizerProviderConfiguration(
            new VisualizerTargetType("Bitmap debugger", typeof(Bitmap)),
            new VisualizerTargetType("Metafile debugger", typeof(Metafile))
            )
        {
            Style = VisualizerStyle.ModalDialog, // TODO: ToolWindow
            VisualizerObjectSourceType = new VisualizerObjectSourceType(typeof(ImageSerializer))
        };

        public override async Task<IRemoteUserControl> CreateVisualizerAsync(VisualizerTarget visualizerTarget, CancellationToken cancellationToken)
        {
            //var data = await visualizerTarget.ObjectSource.RequestDataAsync(default(ReadOnlySequence<byte>), cancellationToken);

            // in-proc: everything is allowed, even embedded WinForms can be used
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            return new WpfControlWrapper(new ViewImageControl(visualizerTarget)); // TODO: custom ILocalControlWrapper with WinForms

            // out-of-proc: very limited, only property bindings are allowed in the XAML, which is retrieved as an embedded resource,
            // and even the properties must be serializable, so BitmapSource will not work here
            //return Task.FromResult<IRemoteUserControl>(new ImageVisualizerControl(visualizerTarget));
        }
    }
}
