#if NETFRAMEWORK && !NET472_OR_GREATER
#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: SkiaCustomColorDebuggerVisualizer.cs
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

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using KGySoft.Drawing.DebuggerVisualizers.SkiaSharp;
using KGySoft.Drawing.DebuggerVisualizers.SkiaSharp.Serialization;

using Microsoft.VisualStudio.DebuggerVisualizers;

using SkiaSharp;

#endregion


#region Attributes

[assembly: DebuggerVisualizer(typeof(SkiaCustomColorDebuggerVisualizer), typeof(SkiaColorSerializer),
    Target = typeof(SKColor),
    Description = "KGy SOFT SKColor Debugger Visualizer")]

[assembly: DebuggerVisualizer(typeof(SkiaCustomColorDebuggerVisualizer), typeof(SkiaColorSerializer),
    Target = typeof(SKPMColor),
    Description = "KGy SOFT SKPMColor Debugger Visualizer")]

[assembly: DebuggerVisualizer(typeof(SkiaCustomColorDebuggerVisualizer), typeof(SkiaColorSerializer),
    Target = typeof(SKColorF),
    Description = "KGy SOFT SKColorF Debugger Visualizer")]

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.SkiaSharp
{
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses",
        Justification = "False alarm, instantiated by VS debugger visualizers")]
    internal sealed class SkiaCustomColorDebuggerVisualizer : DialogDebuggerVisualizer
    {
        #region Methods

        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
            => DebuggerHelper.DebugCustomColor(SerializationHelper.DeserializeCustomColorInfo(objectProvider.GetData()));

        #endregion
    }
}
#endif