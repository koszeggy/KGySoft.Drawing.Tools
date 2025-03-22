#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorDebuggerVisualizerProvider.cs
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

using KGySoft.Drawing.DebuggerVisualizers.Core;
using KGySoft.Drawing.DebuggerVisualizers.Core.Serialization;
using KGySoft.Drawing.Imaging;

using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.DebuggerVisualizers;
using Microsoft.VisualStudio.RpcContracts.RemoteUI;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Package.DebuggerVisualizerProviders.Core
{
    [VisualStudioContribution]
    internal class ColorDebuggerVisualizerProvider : DebuggerVisualizerProvider
    {
        #region Fields

        private readonly ColorDebuggerVisualizerProviderImpl providerImpl = new();

        #endregion

        #region Properties

        public override DebuggerVisualizerProviderConfiguration DebuggerVisualizerProviderConfiguration => new(
            // => providerImpl.DebuggerVisualizerProviderConfiguration; // ISSUE: CEE0005 - Could not evaluate compile-time constant
            new VisualizerTargetType("%DebuggerVisualizerProviders.Core.Color32DebuggerVisualizerProvider.Name%", typeof(Color32)),
            new VisualizerTargetType("%DebuggerVisualizerProviders.Core.PColor32DebuggerVisualizerProvider.Name%", typeof(PColor32)),
            new VisualizerTargetType("%DebuggerVisualizerProviders.Core.Color64DebuggerVisualizerProvider.Name%", typeof(Color64)),
            new VisualizerTargetType("%DebuggerVisualizerProviders.Core.PColor64DebuggerVisualizerProvider.Name%", typeof(PColor64)),
            new VisualizerTargetType("%DebuggerVisualizerProviders.Core.ColorFDebuggerVisualizerProvider.Name%", typeof(ColorF)),
            new VisualizerTargetType("%DebuggerVisualizerProviders.Core.PColorFDebuggerVisualizerProvider.Name%", typeof(PColorF)))
        {
            Style = VisualizerStyle.ToolWindow,
            VisualizerObjectSourceType = new VisualizerObjectSourceType(typeof(ColorSerializer))
        };

        #endregion

        #region Methods

        public override Task<IRemoteUserControl> CreateVisualizerAsync(VisualizerTarget visualizerTarget, CancellationToken cancellationToken)
            => providerImpl.CreateVisualizerAsync(visualizerTarget, cancellationToken);

        #endregion
    }
}
