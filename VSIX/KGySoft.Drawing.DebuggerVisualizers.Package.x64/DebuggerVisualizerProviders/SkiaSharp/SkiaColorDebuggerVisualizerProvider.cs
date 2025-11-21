#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: SkiaColorDebuggerVisualizerProvider.cs
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

using KGySoft.Drawing.DebuggerVisualizers.SkiaSharp;
using KGySoft.Drawing.DebuggerVisualizers.SkiaSharp.Serialization;

using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.DebuggerVisualizers;
using Microsoft.VisualStudio.RpcContracts.RemoteUI;

using SkiaSharp;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Package.DebuggerVisualizerProviders.SkiaSharp
{
    [VisualStudioContribution]
    internal class SkiaColorDebuggerVisualizerProvider : DebuggerVisualizerProvider
    {
        #region Fields

        private readonly SkiaColorDebuggerVisualizerProviderImpl providerImpl = new();

        #endregion

        #region Properties

        public override DebuggerVisualizerProviderConfiguration DebuggerVisualizerProviderConfiguration
            // => providerImpl.DebuggerVisualizerProviderConfiguration; // ISSUE: CEE0005 - Could not evaluate compile-time constant
            => new(new VisualizerTargetType("%DebuggerVisualizerProviders.SkiaSharp.SKColorDebuggerVisualizerProvider.Name%", typeof(SKColor)),
                new VisualizerTargetType("%DebuggerVisualizerProviders.SkiaSharp.SKPMColorDebuggerVisualizerProvider.Name%", typeof(SKPMColor)),
                new VisualizerTargetType("%DebuggerVisualizerProviders.SkiaSharp.SKColorFDebuggerVisualizerProvider.Name%", typeof(SKColorF)))
            {
                Style = VisualizerStyle.ToolWindow,
                VisualizerObjectSourceType = new VisualizerObjectSourceType(typeof(SkiaColorSerializer))
            };

        #endregion

        #region Methods

        public override Task<IRemoteUserControl> CreateVisualizerAsync(VisualizerTarget visualizerTarget, CancellationToken cancellationToken)
            => providerImpl.CreateVisualizerAsync(visualizerTarget, cancellationToken);

        #endregion
    }
}
