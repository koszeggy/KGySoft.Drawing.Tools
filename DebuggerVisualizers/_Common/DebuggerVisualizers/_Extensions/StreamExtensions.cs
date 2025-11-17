#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: StreamExtensions.cs
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
#if NET472_OR_GREATER
using System.Buffers;
#endif
using System.IO;
using System.Text;

#if NET472_OR_GREATER
using KGySoft.CoreLibraries;
#endif

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers
{
    /// <summary>
    /// Provides extension methods for the <see cref="Stream"/> type.
    /// </summary>
    public static class StreamExtensions
    {
        #region Nested Classes

        #region LeaveOpenReader class

        private sealed class LeaveOpenReader : BinaryReader
        {
            #region Constructors

            internal LeaveOpenReader(Stream stream)
#if NET35 || NET40
                : base(stream, Encoding.UTF8)
#else
                : base(stream, Encoding.UTF8, true)
#endif
            {
            }

            #endregion

            #region Methods

#if NET35 || NET40
            protected override void Dispose(bool disposing)
            {
                // just not calling the base to prevent from closing the stream
            }
#endif

            #endregion
        }

        #endregion

        #region LeaveOpenWriter class

        private sealed class LeaveOpenWriter : BinaryWriter
        {
            #region Fields
#if NET35 || NET40

            private bool isDisposed;

#endif
#endregion

            #region Constructors

            internal LeaveOpenWriter(Stream stream)
#if NET35 || NET40
                : base(stream, Encoding.UTF8)
#else
                : base(stream, Encoding.UTF8, true)
#endif
            {
            }

            #endregion

            #region Methods
#if NET35 || NET40

            protected override void Dispose(bool disposing)
            {
                if (isDisposed)
                    return;
                isDisposed = true;
                OutStream.Flush();
            }

#endif
#endregion
        }

        #endregion

        #region TempFileReader class

        private sealed class TempFileReader : BinaryReader
        {
            #region Fields

            private readonly string tempFileName;

            #endregion

            #region Constructors

            internal TempFileReader(string fileName) : base(File.OpenRead(fileName)) => tempFileName = fileName;

            #endregion

            #region Methods

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                try
                {
                    File.Delete(tempFileName);
                }
                catch (Exception e) when (!e.IsCritical())
                {
                }
            }

            #endregion
        }

        #endregion

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Gets a <see cref="BinaryWriter"/> for the specified serialization stream.
        /// </summary>
        public static BinaryWriter InitSerializationWriter(this Stream outgoingData)
        {
            // Whenever possible, we try to create a temp file and create the writer for that.
            // This is needed because if the debugger is experiencing large memory usage, then it starts to dispose
            // the objects in the watch window, including images. This may happen even in the middle of serialization
            // causing AccessViolationException while reading image pixels.
            string? fileName = null;
            Stream? fileStream = null;
            try
            {
                fileName = Path.GetTempFileName();
                fileStream = File.OpenWrite(fileName);
            }
            catch (Exception e) when (!e.IsCritical())
            {
                fileStream?.Dispose();
                fileStream = null;
                try
                {
                    if (fileName != null)
                        File.Delete(fileName);
                }
                catch (Exception ex) when (!ex.IsCritical())
                {
                }
                finally
                {
                    fileName = null;
                }
            }

            var outgoingWriter = new LeaveOpenWriter(outgoingData);
            outgoingWriter.Write(fileName != null);

            // Temp file could not be created: falling back to serializing in the outgoing stream (which is actually a memory stream)
            if (fileStream == null)
                return outgoingWriter;

            // We could create a temp file: we write only the path in the outgoing data.
            outgoingWriter.Write(fileName!);
            return new BinaryWriter(fileStream);
        }

        /// <summary>
        /// Gets a <see cref="BinaryReader"/> for the specified serialization stream.
        /// </summary>
        public static BinaryReader InitSerializationReader(this Stream incomingData)
        {
            var incomingReader = new LeaveOpenReader(incomingData);
            return incomingReader.ReadBoolean() ? new TempFileReader(incomingReader.ReadString()) : incomingReader;
        }

#if NET472_OR_GREATER
        /// <summary>
        /// Gets a <see cref="Stream"/> that reads from the specified <see cref="ReadOnlySequence{T}"/>.
        /// </summary>
        /// <param name="readOnlySequence">The <see cref="ReadOnlySequence{T}"/> to get as a stream.</param>
        /// <returns>A <see cref="Stream"/> that reads from the specified <see cref="ReadOnlySequence{T}"/>.</returns>
        public static Stream AsStream(this ReadOnlySequence<byte> readOnlySequence)
        {
            // TODO: try to get actual array, and use ToArray as a fallback only
            if (readOnlySequence.IsSingleSegment)
                return new MemoryStream(readOnlySequence.First.ToArray());

            // TODO: implement a custom stream that reads from the sequence
            // NOTE: In Nerdbank.Streams there is as an AsStream extension method, but it's risky to use because it's just a dependency of Microsoft.VisualStudio.Extensibility that may be removed in the future
            return new MemoryStream(readOnlySequence.ToArray());
        }

        /// <summary>
        /// Gets a <see cref="ReadOnlySequence{T}"/> of bytes from the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The stream to get as a sequence.</param>
        /// <param name="disposeWhenReadToEnd"><see langword="true"/> to dispose the <paramref name="stream"/> when the returned sequence is read to the end; <see langword="false"/> to keep the <paramref name="stream"/> open.</param>
        /// <returns>A <see cref="ReadOnlySequence{T}"/> of bytes from the specified <see cref="Stream"/>.</returns>
        public static ReadOnlySequence<byte> AsReadOnlySequence(this Stream stream, bool disposeWhenReadToEnd = true)
        {
            if (stream is MemoryStream ms && ms.TryGetBuffer(out ArraySegment<byte> segment))
                return new ReadOnlySequence<byte>(segment.Array, segment.Offset, segment.Count);

            // TODO: implement an optionally custom sequence (if stream.Length is large enough) that reads from the stream
            var result = stream.ToArray();
            return new ReadOnlySequence<byte>(result);
        }
#endif

        #endregion

        #region Internal Methods

        internal static Stream GetTempStream()
        {
            Stream? fileStream = null;
            try
            {
                string fileName = Path.GetTempFileName();
                fileStream = File.Create(fileName, 4096, FileOptions.DeleteOnClose);
                return fileStream;
            }
            catch (Exception e) when (!e.IsCritical())
            {
                fileStream?.Dispose();
                return new MemoryStream();
            }
        }

        #endregion

        #endregion
    }
}
