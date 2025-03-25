#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: SkiaBitmapDebuggerVisualizerProvider.cs
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
    internal class SkiaBitmapDebuggerVisualizerProvider : DebuggerVisualizerProvider
    {
        #region Fields

        private readonly SkiaBitmapDebuggerVisualizerProviderImpl providerImpl = new();

        #endregion

        #region Properties

        public override DebuggerVisualizerProviderConfiguration DebuggerVisualizerProviderConfiguration
            // => providerImpl.DebuggerVisualizerProviderConfiguration; // ISSUE: CEE0005 - Could not evaluate compile-time constant
            => new(new VisualizerTargetType("%DebuggerVisualizerProviders.SkiaSharp.SKBitmapDebuggerVisualizerProvider.Name%", typeof(SKBitmap)),
                new VisualizerTargetType("%DebuggerVisualizerProviders.SkiaSharp.SKPixmapDebuggerVisualizerProvider.Name%", typeof(SKPixmap)),
                new VisualizerTargetType("%DebuggerVisualizerProviders.SkiaSharp.SKImageDebuggerVisualizerProvider.Name%", typeof(SKImage)),
                new VisualizerTargetType("%DebuggerVisualizerProviders.SkiaSharp.SKSurfaceDebuggerVisualizerProvider.Name%", typeof(SKSurface)))
            {
                Style = VisualizerStyle.ToolWindow,
                VisualizerObjectSourceType = new VisualizerObjectSourceType(typeof(SkiaBitmapSerializer))
            };

        #endregion

        #region Methods

        public override Task<IRemoteUserControl> CreateVisualizerAsync(VisualizerTarget visualizerTarget, CancellationToken cancellationToken)
            => providerImpl.CreateVisualizerAsync(visualizerTarget, cancellationToken);

        #endregion
    }
}
