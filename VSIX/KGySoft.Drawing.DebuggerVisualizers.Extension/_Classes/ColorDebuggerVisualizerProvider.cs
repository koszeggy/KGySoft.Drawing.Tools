using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KGySoft.Drawing.DebuggerVisualizers.GdiPlus.Serialization;

using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.DebuggerVisualizers;
using Microsoft.VisualStudio.RpcContracts.RemoteUI;

namespace KGySoft.Drawing.DebuggerVisualizers.Extension
{
    [VisualStudioContribution]
    internal class ColorDebuggerVisualizerProvider : DebuggerVisualizerProvider
    {
        private readonly GdiPlus.ColorDebuggerVisualizerProviderImpl providerImpl = new();

        //public override DebuggerVisualizerProviderConfiguration DebuggerVisualizerProviderConfiguration => providerImpl.DebuggerVisualizerProviderConfiguration; // ISSUE: CEE0005 - Could not evaluate compile-time constant
        public override DebuggerVisualizerProviderConfiguration DebuggerVisualizerProviderConfiguration => new("Color debugger", typeof(Color))
        {
            Style = VisualizerStyle.ToolWindow,
            VisualizerObjectSourceType = new VisualizerObjectSourceType(typeof(ColorSerializer))
        };

        public override Task<IRemoteUserControl> CreateVisualizerAsync(VisualizerTarget visualizerTarget, CancellationToken cancellationToken) => providerImpl.CreateVisualizerAsync(visualizerTarget, cancellationToken);
    }
}
