#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorPaletteDebuggerVisualizerProvider.cs
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

using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;

using KGySoft.Drawing.DebuggerVisualizers.GdiPlus.Serialization;

using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.DebuggerVisualizers;
using Microsoft.VisualStudio.RpcContracts.RemoteUI;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Package.DebuggerVisualizerProviders.GdiPlus
{
    [VisualStudioContribution]
    internal class ColorPaletteDebuggerVisualizerProvider : DebuggerVisualizerProvider
    {
        #region Fields

        private readonly DebuggerVisualizers.GdiPlus.ColorPaletteDebuggerVisualizerProviderImpl providerImpl = new();

        #endregion

        #region Properties

        public override DebuggerVisualizerProviderConfiguration DebuggerVisualizerProviderConfiguration
            // => providerImpl.DebuggerVisualizerProviderConfiguration; // ISSUE: CEE0005 - Could not evaluate compile-time constant
            => new("%DebuggerVisualizerProviders.GdiPlus.ColorPaletteDebuggerVisualizerProvider.Name%", typeof(ColorPalette))
            {
                Style = VisualizerStyle.ToolWindow,
                VisualizerObjectSourceType = new VisualizerObjectSourceType(typeof(ColorPaletteSerializer))
            };

        #endregion

        #region Methods

        public override Task<IRemoteUserControl> CreateVisualizerAsync(VisualizerTarget visualizerTarget, CancellationToken cancellationToken)
            => providerImpl.CreateVisualizerAsync(visualizerTarget, cancellationToken);

        #endregion
    }
}
