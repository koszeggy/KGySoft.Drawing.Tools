﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageSourceSerializer.cs
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

using System.IO;
using System.Windows.Media;

using KGySoft.Drawing.DebuggerVisualizers.Serialization;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Wpf.Serialization
{
    /// <summary>
    /// Provides serialization for the <see cref="ImageSource"/> type.
    /// </summary>
    public class ImageSourceSerializer : SerializerBase
    {
        #region Methods

        /// <summary>
        /// Called when the object to be debugged is about to be serialized.
        /// </summary>
        public override void GetData(object target, Stream outgoingData) => SerializationHelper.SerializeCustomBitmapInfo((ImageSource)target, outgoingData);

        #endregion
    }
}