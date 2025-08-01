#if NETFRAMEWORK && !NET472_OR_GREATER
#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorDebuggerVisualizer.cs
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

using KGySoft.Drawing.DebuggerVisualizers.Core;
using KGySoft.Drawing.DebuggerVisualizers.Core.Serialization;
using KGySoft.Drawing.Imaging;

using Microsoft.VisualStudio.DebuggerVisualizers;

#endregion

#region Attributes

[assembly: DebuggerVisualizer(typeof(ColorDebuggerVisualizer), typeof(ColorSerializer),
    Target = typeof(Color32),
    Description = "KGy SOFT Color32 Debugger Visualizer")]

[assembly: DebuggerVisualizer(typeof(ColorDebuggerVisualizer), typeof(ColorSerializer),
    Target = typeof(PColor32),
    Description = "KGy SOFT PColor32 Debugger Visualizer")]

[assembly: DebuggerVisualizer(typeof(ColorDebuggerVisualizer), typeof(ColorSerializer),
    Target = typeof(Color64),
    Description = "KGy SOFT Color64 Debugger Visualizer")]

[assembly: DebuggerVisualizer(typeof(ColorDebuggerVisualizer), typeof(ColorSerializer),
    Target = typeof(PColor64),
    Description = "KGy SOFT PColor64 Debugger Visualizer")]

[assembly: DebuggerVisualizer(typeof(ColorDebuggerVisualizer), typeof(ColorSerializer),
    Target = typeof(ColorF),
    Description = "KGy SOFT ColorF Debugger Visualizer")]

[assembly: DebuggerVisualizer(typeof(ColorDebuggerVisualizer), typeof(ColorSerializer),
    Target = typeof(PColorF),
    Description = "KGy SOFT PColorF Debugger Visualizer")]

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Core
{
    internal sealed class ColorDebuggerVisualizer : DialogDebuggerVisualizer
    {
        #region Methods

        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
            => DebuggerHelper.DebugCustomColor(SerializationHelper.DeserializeCustomColorInfo(objectProvider.GetData()));

        #endregion
    }
}
#endif