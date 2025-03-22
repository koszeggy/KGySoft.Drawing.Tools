#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: SerializerBase.cs
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

#if NET472_OR_GREATER
using System.Buffers;
#endif
using System.IO;
using System.Threading;

using Microsoft.VisualStudio.DebuggerVisualizers;
#if NET472_OR_GREATER
using Microsoft.VisualStudio.Extensibility.DebuggerVisualizers;
#endif

#endregion

#region Suppressions

#if !NET472_OR_GREATER
#pragma warning disable CS1574 // the documentation contains types that are not available in every target
#endif

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Serialization
{
    /// <summary>
    /// Base class for implementing custom serializers for <see cref="VisualizerObjectSource"/>-based visualizers that are also compatible
    /// with the newer debugger visualizer extensions introduced in Visual Studio 2022.
    /// </summary>
    public abstract class SerializerBase : VisualizerObjectSource
    {
        #region Methods

        /// <summary>
        /// The base implementation just calls <see cref="VisualizerObjectSource.GetData"/> without processing the incoming data.
        /// May be called by the <see cref="VisualizerObjectSourceClient.RequestDataAsync(ReadOnlySequence{byte}?,CancellationToken)"/>
        /// of the new visualizer extensions introduced in Visual Studio 2022.
        /// </summary>
        public override void TransferData(object target, Stream incomingData, Stream outgoingData) => GetData(target, outgoingData);

        #endregion
    }
}