﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: SKBitmapSerializer.cs
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

using Microsoft.VisualStudio.DebuggerVisualizers;

using SkiaSharp;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.SkiaSharp.Serialization
{
    internal sealed class SKBitmapSerializer : VisualizerObjectSource
    {
        #region Methods

        /// <summary>
        /// Called when the object to be debugged is about to be serialized
        /// </summary>
        public override void GetData(object target, Stream outgoingData) => SerializationHelper.SerializeCustomBitmapInfo((SKBitmap)target, outgoingData);

        #endregion
    }
}