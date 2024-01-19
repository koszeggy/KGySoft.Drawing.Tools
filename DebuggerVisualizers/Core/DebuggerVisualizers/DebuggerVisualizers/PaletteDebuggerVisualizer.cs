#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: PaletteDebuggerVisualizer.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2024 - All Rights Reserved
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

namespace KGySoft.Drawing.DebuggerVisualizers.Core.DebuggerVisualizers
{
    internal sealed class PaletteDebuggerVisualizer : DialogDebuggerVisualizer
    {
        #region Methods

        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
            => DebuggerHelper.DebugCustomPalette(SerializationHelper.DeserializeCustomPaletteInfo(objectProvider.GetData()));

        #endregion
    }
}
