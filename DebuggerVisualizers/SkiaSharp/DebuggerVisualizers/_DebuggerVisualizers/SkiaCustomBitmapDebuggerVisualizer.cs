#if NETFRAMEWORK && !NET472_OR_GREATER
#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: SkiaCustomBitmapDebuggerVisualizer.cs
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

using KGySoft.Drawing.DebuggerVisualizers.SkiaSharp;
using KGySoft.Drawing.DebuggerVisualizers.SkiaSharp.Serialization;
using KGySoft.Drawing.ImagingTools.Model;

using Microsoft.VisualStudio.DebuggerVisualizers;

using SkiaSharp;

#endregion

#region Attributes

[assembly: DebuggerVisualizer(typeof(SkiaCustomBitmapDebuggerVisualizer), typeof(SkiaBitmapSerializer),
    Target = typeof(SKBitmap),
    Description = "KGy SOFT SKBitmap Debugger Visualizer")]

[assembly: DebuggerVisualizer(typeof(SkiaCustomBitmapDebuggerVisualizer), typeof(SkiaBitmapSerializer),
    Target = typeof(SKPixmap),
    Description = "KGy SOFT SKPixmap Debugger Visualizer")]

[assembly: DebuggerVisualizer(typeof(SkiaCustomBitmapDebuggerVisualizer), typeof(SkiaBitmapSerializer),
    Target = typeof(SKImage),
    Description = "KGy SOFT SKImage Debugger Visualizer")]

[assembly: DebuggerVisualizer(typeof(SkiaCustomBitmapDebuggerVisualizer), typeof(SkiaBitmapSerializer),
    Target = typeof(SKSurface),
    Description = "KGy SOFT SKSurface Debugger Visualizer")]

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.SkiaSharp
{
    internal sealed class SkiaCustomBitmapDebuggerVisualizer : DialogDebuggerVisualizer
    {
        #region Methods

        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            using CustomBitmapInfo bitmapInfo = SerializationHelper.DeserializeCustomBitmapInfo(objectProvider.GetData());
            DebuggerHelper.DebugCustomBitmap(bitmapInfo);
        }

        #endregion
    }
}
#endif