#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ReadableBitmapDataDebuggerVisualizerProvider.cs
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

using KGySoft.Drawing.DebuggerVisualizers.Core.DebuggerVisualizerProviders;
using KGySoft.Drawing.DebuggerVisualizers.Core.Serialization;

using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.DebuggerVisualizers;
using Microsoft.VisualStudio.RpcContracts.RemoteUI;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Package.DebuggerVisualizerProviders.Core
{
    [VisualStudioContribution]
    internal class ReadableBitmapDataDebuggerVisualizerProvider : DebuggerVisualizerProvider
    {
        #region Fields

        private readonly ReadableBitmapDataDebuggerVisualizerProviderImpl providerImpl = new();

        #endregion

        #region Properties

        public override DebuggerVisualizerProviderConfiguration DebuggerVisualizerProviderConfiguration
            // => providerImpl.DebuggerVisualizerProviderConfiguration; // ISSUE: CEE0005 - Could not evaluate compile-time constant
            => new("%DebuggerVisualizerProviders.Core.ReadableBitmapDataDebuggerVisualizerProvider.Name%", "KGySoft.Drawing.Imaging.BitmapDataBase, KGySoft.Drawing.Core, Version=0.0.0.0")
            {
                Style = VisualizerStyle.ToolWindow,
                VisualizerObjectSourceType = new VisualizerObjectSourceType(typeof(ReadableBitmapDataSerializer))
            };

        #endregion

        #region Methods

        public override Task<IRemoteUserControl> CreateVisualizerAsync(VisualizerTarget visualizerTarget, CancellationToken cancellationToken)
            => providerImpl.CreateVisualizerAsync(visualizerTarget, cancellationToken);

        #endregion
    }
}
