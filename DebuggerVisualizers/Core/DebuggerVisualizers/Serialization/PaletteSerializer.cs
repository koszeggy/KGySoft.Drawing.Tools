﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: PaletteSerializer.cs
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

using System.IO;

using KGySoft.Drawing.Imaging;

using Microsoft.VisualStudio.DebuggerVisualizers;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Core.Serialization
{
    internal sealed class PaletteSerializer : VisualizerObjectSource
    {
        #region Methods

        public override void GetData(object target, Stream outgoingData) => SerializationHelper.SerializeCustomPaletteInfo((IPalette)target, outgoingData);

        #endregion
    }
}
