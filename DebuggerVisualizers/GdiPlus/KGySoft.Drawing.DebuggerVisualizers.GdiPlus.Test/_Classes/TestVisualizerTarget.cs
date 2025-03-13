#if NET472_OR_GREATER
#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: TestVisualizerTarget.cs
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

using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.DebuggerVisualizers;
using Microsoft.VisualStudio.RpcContracts.DebuggerVisualizers;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.GdiPlus.Test
{
    internal class TestVisualizerTarget : IVisualizerTarget
    {
        #region Fields

        private readonly VisualizerObjectSource serializer;

        #endregion

        #region Properties

        internal bool IsReplaced { get; private set; }

        internal object Object { get; private set; }

        #endregion

        #region Constructors

        internal TestVisualizerTarget(object target, VisualizerObjectSource serializer)
        {
            Object = target;
            this.serializer = serializer;
        }

        #endregion

        #region Methods

        public Task<ReadOnlySequence<byte>?> RequestDataAsync(ReadOnlySequence<byte>? data, CancellationToken cancellationToken)
        {
            using var incomingData = new MemoryStream((data ?? default).ToArray());
            using var outgoingData = new MemoryStream();
            serializer.TransferData(Object, incomingData, outgoingData);
            return Task.FromResult<ReadOnlySequence<byte>?>(new ReadOnlySequence<byte>(outgoingData.ToArray()));
        }

        public Task ReplaceTargetObjectAsync(ReadOnlySequence<byte> data, CancellationToken cancellationToken)
        {
            IsReplaced = true;
            Object = serializer.CreateReplacementObject(Object, new MemoryStream(data.ToArray()));
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }

        #endregion
    }
}
#endif