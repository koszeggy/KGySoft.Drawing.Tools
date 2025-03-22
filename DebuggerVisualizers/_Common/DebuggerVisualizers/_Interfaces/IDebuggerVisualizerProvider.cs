#if NET472_OR_GREATER
#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: IDebuggerVisualizerProvider.cs
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

using Microsoft.VisualStudio.Extensibility.DebuggerVisualizers;
using Microsoft.VisualStudio.RpcContracts.RemoteUI;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers
{
    /// <summary>
    /// Represents a debugger visualizer provider. It allows getting the configuration and the visualizer itself without a Visual Studio instance,
    /// allowing to test the visualizer in a standalone environment.
    /// </summary>
    public interface IDebuggerVisualizerProvider
    {
        #region Properties

        /// <summary>
        /// Gets the configuration of the debugger visualizer provider.
        /// NOTE: In an actual extension this property may not return the implementation of this interface directly, because the actual implementation
        /// must be a compile-time constant. Still, the <see cref="IDebuggerVisualizerProvider"/> also has this property to allow testing the visualizer.
        /// </summary>
        DebuggerVisualizerProviderConfiguration DebuggerVisualizerProviderConfiguration { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the visualizer control asynchronously. To be able to test the visualizer, this method should return an <see cref="ILocalControlWrapper"/> implementation.
        /// </summary>
        /// <param name="visualizerTarget">The <see cref="VisualizerTarget" /> that provides information about the target process and object.</param>
        /// <param name="cancellationToken">Cancellation token for the async call.</param>
        /// <returns>An <see cref="IRemoteUserControl" /> that will show the representation of the target object.</returns>
        Task<IRemoteUserControl> CreateVisualizerAsync(VisualizerTarget visualizerTarget, CancellationToken cancellationToken);

        #endregion
    }
}
#endif