#if NETFRAMEWORK && !NET472_OR_GREATER || NETCOREAPP && DEBUG
#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: BitmapPaletteDebuggerVisualizer.cs
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
using System.Windows.Media.Imaging;

using KGySoft.Drawing.DebuggerVisualizers.Wpf;
using KGySoft.Drawing.DebuggerVisualizers.Wpf.Serialization;

using Microsoft.VisualStudio.DebuggerVisualizers;

#endregion

#region Attributes

[assembly: DebuggerVisualizer(typeof(BitmapPaletteDebuggerVisualizer), typeof(BitmapPaletteSerializer),
    Target = typeof(BitmapPalette),
    Description = "KGy SOFT BitmapPalette Debugger Visualizer")]

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Wpf
{
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses",
        Justification = "False alarm, instantiated by VS debugger visualizers")]
    internal class BitmapPaletteDebuggerVisualizer : DialogDebuggerVisualizer
    {
        #region Methods

        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
            => DebuggerHelper.DebugCustomPalette(SerializationHelper.DeserializeCustomPaletteInfo(objectProvider.GetData()));

        #endregion
    }
}
#endif