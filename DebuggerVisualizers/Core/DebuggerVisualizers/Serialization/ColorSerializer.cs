#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorSerializer.cs
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

using KGySoft.Drawing.DebuggerVisualizers.Serialization;
using KGySoft.Drawing.Imaging;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Core.Serialization
{
    /// <summary>
    /// Provides serialization for the <see cref="Color32"/>, <see cref="PColor32"/>,
    /// <see cref="Color64"/>, <see cref="PColor64"/>, <see cref="ColorF"/> and <see cref="PColorF"/> types.
    /// </summary>
    public sealed class ColorSerializer : SerializerBase
    {
        #region Methods

        /// <summary>
        /// Called when the object to be debugged is about to be serialized.
        /// </summary>
        public override void GetData(object target, Stream outgoingData) => SerializationHelper.SerializeCustomColorInfo(target, outgoingData);

        #endregion
    }
}
