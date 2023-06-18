#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: BitmapPaletteSerializer.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2023 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Media.Imaging;

using Microsoft.VisualStudio.DebuggerVisualizers;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Wpf.Serialization
{
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses",
        Justification = "False alarm, instantiated by VS debugger visualizers")]
    internal class BitmapPaletteSerializer : VisualizerObjectSource
    {
        #region Methods

        public override void GetData(object target, Stream outgoingData) => SerializationHelper.SerializeCustomPaletteInfo((BitmapPalette)target, outgoingData);

        #endregion
    }
}