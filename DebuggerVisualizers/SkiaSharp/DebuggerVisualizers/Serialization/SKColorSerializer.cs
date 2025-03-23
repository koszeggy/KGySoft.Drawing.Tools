#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: SKColorSerializer.cs
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
using System.IO;

using Microsoft.VisualStudio.DebuggerVisualizers;

using SkiaSharp;

#endregion


namespace KGySoft.Drawing.DebuggerVisualizers.SkiaSharp.Serialization
{
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses",
        Justification = "False alarm, instantiated by VS debugger visualizers")]
    internal sealed class SKColorSerializer : VisualizerObjectSource
    {
        #region Methods

        /// <summary>
        /// Called when the object to be debugged is about to be serialized
        /// </summary>
        public override void GetData(object target, Stream outgoingData) => SerializationHelper.SerializeCustomColorInfo((SKColor)target, outgoingData);

        ///// <summary>
        ///// Called when the debugged object has been replaced
        ///// </summary>
        //public override object CreateReplacementObject(object target, Stream incomingData) => SerializationHelper.DeserializeSKColor(incomingData);

        #endregion
    }
}
