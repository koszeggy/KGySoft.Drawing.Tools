﻿#if NET472_OR_GREATER
#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: SkiaColorDebuggerVisualizerProviderImpl.cs
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
using System.Windows.Media;

using KGySoft.Drawing.DebuggerVisualizers.SkiaSharp.Serialization;
using KGySoft.Drawing.DebuggerVisualizers.View;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Drawing.ImagingTools.ViewModel;

using Microsoft.VisualStudio.Extensibility.DebuggerVisualizers;
using Microsoft.VisualStudio.Extensibility.VSSdkCompatibility;
using Microsoft.VisualStudio.RpcContracts.RemoteUI;
using Microsoft.VisualStudio.Shell;

using SkiaSharp;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.SkiaSharp
{
    /// <summary>
    /// Provides the implementation of a debugger visualizer extension for the <see cref="SKColor"/>,
    /// <see cref="SKPMColor"/> and <see cref="SKColorF"/> structs.
    /// </summary>
    public class SkiaColorDebuggerVisualizerProviderImpl : IDebuggerVisualizerProvider
    {
        #region Properties

        /// <summary>
        /// Gets the configuration of the color debugger visualizer provider.
        /// </summary>
        public DebuggerVisualizerProviderConfiguration DebuggerVisualizerProviderConfiguration => new(
            new VisualizerTargetType("KGy SOFT SKColor Debugger Visualizer", typeof(SKColor)),
            new VisualizerTargetType("KGy SOFT SKPMColor Debugger Visualizer", typeof(SKPMColor)),
            new VisualizerTargetType("KGy SOFT SKColorF Debugger Visualizer", typeof(SKColorF)))
        {
            Style = VisualizerStyle.ToolWindow,
            VisualizerObjectSourceType = new VisualizerObjectSourceType(typeof(SkiaColorSerializer))
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