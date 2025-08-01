#if NETFRAMEWORK && !NET472_OR_GREATER
#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: PaletteDebuggerVisualizer.cs
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

[assembly: DebuggerVisualizer(typeof(PaletteDebuggerVisualizer), typeof(PaletteSerializer),
    Target = typeof(Palette),
       Description = "KGy SOFT Palette Debugger Visualizer")]

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Core
{
    internal sealed class PaletteDebuggerVisualizer : DialogDebuggerVisualizer
    {
        #region Methods

        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
            => DebuggerHelper.DebugCustomPalette(SerializationHelper.DeserializeCustomPaletteInfo(objectProvider.GetData()));

        #endregion
    }
}
#endif