#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageSerializer.cs
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

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;

using KGySoft.Drawing.DebuggerVisualizers.Serialization;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.GdiPlus.Serialization
{
    /// <summary>
    /// Provides serialization for the <see cref="Image"/> type.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses",
        Justification = "False alarm, instantiated by VS debugger visualizers")]
    public class ImageSerializer : SerializerBase
    {
        #region Methods

        /// <summary>
        /// Called when the object to be debugged is about to be serialized
        /// </summary>
        public override void GetData(object target, Stream outgoingData) => SerializationHelper.SerializeImageInfo((Image)target, outgoingData);

        /// <summary>
        /// Called when the debugged object has been replaced
        /// </summary>
        public override object? CreateReplacementObject(object target, Stream incomingData) => SerializationHelper.DeserializeReplacementImage(incomingData);

        #endregion
    }
}
