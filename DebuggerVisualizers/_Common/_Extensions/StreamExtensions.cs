#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: StreamExtensions.cs
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

using System;
using System.IO;
using System.Text;

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

        #endregion
    }
}
