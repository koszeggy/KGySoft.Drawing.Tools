#if NET472_OR_GREATER
#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorDebuggerVisualizerProviderImpl.cs
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

using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

using KGySoft.Drawing.DebuggerVisualizers.GdiPlus.Serialization;
using KGySoft.Drawing.DebuggerVisualizers.View;
using KGySoft.Drawing.ImagingTools.ViewModel;

using Microsoft.VisualStudio.Extensibility.DebuggerVisualizers;
using Microsoft.VisualStudio.Extensibility.VSSdkCompatibility;
using Microsoft.VisualStudio.RpcContracts.RemoteUI;
using Microsoft.VisualStudio.Shell;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.GdiPlus
{
    /// <summary>
    /// Provides the implementation of a debugger visualizer extension for the <see cref="Color"/> struct.
    /// </summary>
    public class ColorDebuggerVisualizerProviderImpl : IDebuggerVisualizerProvider
    {
        #region Properties

        /// <summary>
        /// Gets the configuration of the color debugger visualizer provider.
        /// </summary>
        public DebuggerVisualizerProviderConfiguration DebuggerVisualizerProviderConfiguration
            => new("KGy SOFT Color Debugger Visualizer", typeof(Color))
        {
            Style = VisualizerStyle.ToolWindow,
            VisualizerObjectSourceType = new VisualizerObjectSourceType(typeof(ColorSerializer))
        };

        #endregion

        #region Methods

        /// <summary>
        /// Gets the view of the color debugger visualizer.
        /// </summary>
        /// <param name="visualizerTarget">The <see cref="VisualizerTarget" /> that provides information about the target process and object.</param>
        /// <param name="cancellationToken">Cancellation token for the async call.</param>
        /// <returns>The view of the color debugger visualizer</returns>
        public async Task<IRemoteUserControl> CreateVisualizerAsync(VisualizerTarget visualizerTarget, CancellationToken cancellationToken)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            var result = new VisualizerExtensionWpfAdapter<Color>(visualizerTarget,
                (c, vt) => ViewModelFactory.FromColor(c, !vt.IsTargetReplaceable),
                SerializationHelper.DeserializeColor,
                SerializationHelper.SerializeColor);
            await result.InitializeAsync(true);
            return new WpfControlWrapper(result);
        }

        #endregion
    }
}
#endif