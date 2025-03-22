#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ReadableBitmapDataSerializer.cs
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

using System;
using System.IO;

using KGySoft.Drawing.DebuggerVisualizers.Serialization;
using KGySoft.Drawing.Imaging;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Core.Serialization
{
    /// <summary>
    /// Provides serialization for the <see cref="IReadableBitmapData"/> type.
    /// </summary>
    public sealed class ReadableBitmapDataSerializer : SerializerBase
    {
        #region Methods

        /// <summary>
        /// Called when the object to be debugged is about to be serialized.
        /// </summary>
        public override void GetData(object target, Stream outgoingData)
        {
            try
            {
                SerializationHelper.SerializeCustomBitmapInfo((IReadableBitmapData)target, outgoingData);
            }
            catch (InvalidCastException)
            {
                // happens in .NET Framework when the target's assembly version is different from the referenced one used in this serializer
                SerializationHelper.SerializeCustomBitmapInfoSafe(target, outgoingData);
            }
        }

        #endregion
    }
}
