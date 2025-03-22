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

using KGySoft.Drawing.DebuggerVisualizers.Core.Serialization;

using Microsoft.VisualStudio.DebuggerVisualizers;

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
