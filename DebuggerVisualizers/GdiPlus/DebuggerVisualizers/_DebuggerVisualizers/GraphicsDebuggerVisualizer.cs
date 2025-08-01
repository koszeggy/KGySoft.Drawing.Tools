#if NETFRAMEWORK && !NET472_OR_GREATER
#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: GraphicsDebuggerVisualizer.cs
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
using System.Drawing;

using KGySoft.Drawing.DebuggerVisualizers.GdiPlus;
using KGySoft.Drawing.DebuggerVisualizers.GdiPlus.Serialization;
using KGySoft.Drawing.ImagingTools.Model;

using Microsoft.VisualStudio.DebuggerVisualizers;

#endregion

#region Attributes

[assembly: DebuggerVisualizer(typeof(GraphicsDebuggerVisualizer), typeof(GraphicsSerializer),
    Target = typeof(Graphics),
    Description = "KGy SOFT Graphics Debugger Visualizer")]

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.GdiPlus
{
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses",
        Justification = "False alarm, instantiated by VS debugger visualizers")]
    internal sealed class GraphicsDebuggerVisualizer : DialogDebuggerVisualizer
    {
        #region Methods

        /// <summary>
        /// Shows the specified window service.
        /// </summary>
        /// <param name="windowService">The window service.</param>
        /// <param name="objectProvider">The object provider.</param>
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            using (GraphicsInfo graphicsInfo = SerializationHelper.DeserializeGraphicsInfo(objectProvider.GetData()))
                GdiPlusDebuggerHelper.DebugGraphics(graphicsInfo);
        }

        #endregion
    }
}
#endif