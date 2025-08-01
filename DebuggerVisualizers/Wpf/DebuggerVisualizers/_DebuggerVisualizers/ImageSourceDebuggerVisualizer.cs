#if NETFRAMEWORK && !NET472_OR_GREATER
#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageSourceDebuggerVisualizer.cs
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
using System.Windows.Media;

using KGySoft.Drawing.DebuggerVisualizers.Wpf;
using KGySoft.Drawing.DebuggerVisualizers.Wpf.Serialization;
using KGySoft.Drawing.ImagingTools.Model;

using Microsoft.VisualStudio.DebuggerVisualizers;

#endregion

#region Attributes

[assembly: DebuggerVisualizer(typeof(ImageSourceDebuggerVisualizer), typeof(ImageSourceSerializer),
    Target = typeof(ImageSource),
    Description = "KGy SOFT ImageSource Debugger Visualizer")]

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Wpf
{
    internal class ImageSourceDebuggerVisualizer : DialogDebuggerVisualizer
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