#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorDebuggerVisualizer.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2022 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System.Diagnostics.CodeAnalysis;

using KGySoft.Drawing.DebuggerVisualizers.Wpf.Serialization;
using KGySoft.Drawing.Wpf;

using Microsoft.VisualStudio.DebuggerVisualizers;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Wpf.DebuggerVisualizers
{
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses",
        Justification = "False alarm, instantiated by VS debugger visualizers")]
    internal class ColorDebuggerVisualizer : DialogDebuggerVisualizer
    {
        #region Methods

        /// <summary>
        /// Shows the specified window service.
        /// </summary>
        /// <param name="windowService">The window service.</param>
        /// <param name="objectProvider">The object provider.</param>
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            DebuggerHelper.DebugCustomColor(SerializationHelper.DeserializeCustomColorInfo(objectProvider.GetData()));
            //Color? newColor = DebuggerHelper.DebugColor(SerializationHelper.DeserializeColor(objectProvider.GetData()), objectProvider.IsObjectReplaceable);
            //if (!objectProvider.IsObjectReplaceable || newColor == null)
            //    return;

            //using var ms = new MemoryStream();
            //SerializationHelper.SerializeColor(newColor.Value, ms);
            //objectProvider.ReplaceData(ms);
        }

        #endregion
    }
}