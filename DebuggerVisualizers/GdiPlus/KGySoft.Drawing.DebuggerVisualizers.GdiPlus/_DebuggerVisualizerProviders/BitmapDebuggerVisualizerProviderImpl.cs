#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: BitmapDebuggerVisualizerProviderImpl.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2025 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System.Buffers;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

using Microsoft.VisualStudio.DebuggerVisualizers;
using Microsoft.VisualStudio.Extensibility.DebuggerVisualizers;
using Microsoft.VisualStudio.Extensibility.VSSdkCompatibility;
using Microsoft.VisualStudio.RpcContracts.RemoteUI;
using Microsoft.VisualStudio.Shell;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.GdiPlus
{
    /// <summary>
    /// Provides the implementation of a debugger visualizer extension for the <see cref="Bitmap"/> class.
    /// </summary>
    public class BitmapDebuggerVisualizerProviderImpl : IDebuggerVisualizerProvider
    {
        #region Properties

        public DebuggerVisualizerProviderConfiguration DebuggerVisualizerProviderConfiguration => new DebuggerVisualizerProviderConfiguration(
            new VisualizerTargetType("Bitmap debugger", typeof(Bitmap)))
        {
            Style = VisualizerStyle.ModalDialog, // TODO: ToolWindow
            VisualizerObjectSourceType = new VisualizerObjectSourceType(typeof(ImageSerializerMock)) // TODO: ImageSerializer
        };

        #endregion

        #region Methods

        public async Task<IRemoteUserControl> CreateVisualizerAsync(VisualizerTarget visualizerTarget, CancellationToken cancellationToken)
        {
            //var data = await visualizerTarget.ObjectSource.RequestDataAsync(default(ReadOnlySequence<byte>), cancellationToken);
            // in-proc: everything is allowed, even embedded WinForms can be used
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            //return new WpfControlWrapper(new ViewImageControl(visualizerTarget)); // TODO: custom ILocalControlWrapper with WinForms

            ReadOnlySequence<byte> data = (await visualizerTarget.ObjectSource.RequestDataAsync(default(ReadOnlySequence<byte>), CancellationToken.None))!.Value;
            //using var ms = new MemoryStream(data.ToArray());
            //using ImageInfo imageInfo = SerializationHelper.DeserializeImageInfo(ms);

            var image = BitmapFrame.Create(new MemoryStream(data.ToArray()));
            return new WpfControlWrapper(new System.Windows.Controls.Image { Source = image }); // TODO: custom ILocalControlWrapper with WinForms

            // out-of-proc: very limited, only property bindings are allowed in the XAML, which is retrieved as an embedded resource,
            // and even the properties must be serializable, so BitmapSource will not work here
            //return Task.FromResult<IRemoteUserControl>(new ImageVisualizerControl(visualizerTarget));
        }

        #endregion
    }

    // TODO: remove, and use the actual ImageSerializer
    public class ImageSerializerMock : VisualizerObjectSource
    {
        #region Methods

        /// <summary>
        /// Called when the object to be debugged is about to be serialized
        /// </summary>
        public override void GetData(object target, Stream outgoingData) =>
            ((Image)target).Save(outgoingData, ImageFormat.Png);

        /// <summary>
        /// Called by the new type of visualizer debuggers when VisualizerTarget.ObjectSource.RequestDataAsync is called
        /// </summary>
        public override void TransferData(object target, Stream incomingData, Stream outgoingData) => GetData(target, outgoingData);

        ///// <summary>
        ///// Called when the debugged object has been replaced
        ///// </summary>
        //public override object? CreateReplacementObject(object target, Stream incomingData) => SerializationHelper.DeserializeReplacementImage(incomingData);

        #endregion
    }

}
