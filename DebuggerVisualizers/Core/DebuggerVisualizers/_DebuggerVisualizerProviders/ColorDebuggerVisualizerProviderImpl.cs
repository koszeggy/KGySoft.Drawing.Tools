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

using System.Threading;
using System.Threading.Tasks;

using KGySoft.Drawing.DebuggerVisualizers.Core.Serialization;
using KGySoft.Drawing.DebuggerVisualizers.View;
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Drawing.ImagingTools.ViewModel;

using Microsoft.VisualStudio.Extensibility.DebuggerVisualizers;
using Microsoft.VisualStudio.Extensibility.VSSdkCompatibility;
using Microsoft.VisualStudio.RpcContracts.RemoteUI;
using Microsoft.VisualStudio.Shell;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Core
{
    /// <summary>
    /// Provides the implementation of a debugger visualizer extension for the <see cref="Color32"/>, <see cref="PColor32"/>,
    /// <see cref="Color64"/>, <see cref="PColor64"/>, <see cref="ColorF"/> and <see cref="PColorF"/> types.
    /// </summary>
    public class ColorDebuggerVisualizerProviderImpl : IDebuggerVisualizerProvider
    {
        #region Properties

        /// <summary>
        /// Gets the configuration of the image debugger visualizer provider.
        /// </summary>
        public DebuggerVisualizerProviderConfiguration DebuggerVisualizerProviderConfiguration => new(
            new VisualizerTargetType("KGy SOFT Color32 Debugger Visualizer", typeof(Color32)),
            new VisualizerTargetType("KGy SOFT PColor32 Debugger Visualizer", typeof(PColor32)),
            new VisualizerTargetType("KGy SOFT Color64 Debugger Visualizer", typeof(Color64)),
            new VisualizerTargetType("KGy SOFT PColor64 Debugger Visualizer", typeof(PColor64)),
            new VisualizerTargetType("KGy SOFT ColorF Debugger Visualizer", typeof(ColorF)),
            new VisualizerTargetType("KGy SOFT PColorF Debugger Visualizer", typeof(PColorF)))
        {
            Style = VisualizerStyle.ToolWindow,
            VisualizerObjectSourceType = new VisualizerObjectSourceType(typeof(ColorSerializer))
        };

        #endregion

        #region Methods

        /// <summary>
        /// Gets the view of the image debugger visualizer.
        /// </summary>
        /// <param name="visualizerTarget">The <see cref="VisualizerTarget" /> that provides information about the target process and object.</param>
        /// <param name="cancellationToken">Cancellation token for the async call.</param>
        /// <returns>The view of the color debugger visualizer</returns>
        public async Task<IRemoteUserControl> CreateVisualizerAsync(VisualizerTarget visualizerTarget, CancellationToken cancellationToken)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            var result = new VisualizerExtensionWpfAdapter<CustomColorInfo?>(visualizerTarget,
                (info, _) => ViewModelFactory.FromCustomColor(info),
                SerializationHelper.DeserializeCustomColorInfo, null);
            await result.InitializeAsync(true);
            return new WpfControlWrapper(result);
        }

        #endregion
    }
}
#endif