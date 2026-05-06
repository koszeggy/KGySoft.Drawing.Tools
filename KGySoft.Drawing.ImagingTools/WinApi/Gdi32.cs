#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Gdi32.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2026 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

#endregion

namespace KGySoft.Drawing.ImagingTools.WinApi
{
    /// <summary>
    /// Contains external methods for Gdi32.dll
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo", Justification = "WinAPI")]
    internal static class Gdi32
    {
        #region NativeMethods class

        [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass", Justification = "Not an issue, always the outer class calls the NativeMethods members")]
        private static class NativeMethods
        {
            #region Methods

            /// <summary>
            /// The GetMetaFileBitsEx function retrieves the contents of a Windows-format metafile and copies them into the specified buffer.
            /// </summary>
            /// <param name="hmf">A handle to a Windows-format metafile.</param>
            /// <param name="nSize">The size, in bytes, of the buffer to receive the data.</param>
            /// <param name="lpvData">A pointer to a buffer that receives the metafile data. The buffer must be sufficiently large to contain the data. If lpvData is NULL, the function returns the number of bytes required to hold the data.</param>
            /// <returns>If the function succeeds and the buffer pointer is NULL, the return value is the number of bytes required for the buffer; if the function succeeds and the buffer pointer is a valid pointer, the return value is the number of bytes copied.
            /// If the function fails, the return value is zero.</returns>
            /// <remarks>
            /// Note: This function is provided only for compatibility with Windows-format metafiles. Enhanced-format metafiles provide superior functionality and are recommended for new applications. The corresponding function for an enhanced-format metafile is GetEnhMetaFileBits.
            /// After the Windows-metafile bits are retrieved, they can be used to create a memory-based metafile by calling the SetMetaFileBitsEx function.
            /// The GetMetaFileBitsEx function does not invalidate the metafile handle. An application must delete this handle by calling the DeleteMetaFile function.
            /// To convert a Windows-format metafile into an enhanced-format metafile, use the SetWinMetaFileBits function.</remarks>
            [DllImport("gdi32.dll")]
            internal static extern uint GetMetaFileBitsEx(IntPtr hmf, uint nSize, [Out]byte[]? lpvData);

            /// <summary>
            /// The DeleteMetaFile function deletes a Windows-format metafile or Windows-format metafile handle.
            /// </summary>
            /// <param name="hmf">A handle to a Windows-format metafile.</param>
            /// <returns>If the function succeeds, the return value is nonzero.
            /// If the function fails, the return value is zero.</returns>
            [DllImport("gdi32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool DeleteMetaFile(IntPtr hmf);

            /// <summary>
            /// The CopyEnhMetaFile function copies the contents of an enhanced-format metafile to a specified file.
            /// </summary>
            /// <param name="hEnh">A handle to the enhanced metafile to be copied.</param>
            /// <param name="lpFileName">A pointer to the name of the destination file. If this parameter is NULL, the source metafile is copied to memory.</param>
            /// <returns>If the function succeeds, the return value is a handle to the copy of the enhanced metafile.
            /// If the function fails, the return value is NULL.</returns>
            [DllImport("gdi32.dll")]
            internal static extern IntPtr CopyEnhMetaFile(IntPtr hEnh, IntPtr lpFileName);

            /// <summary>
            /// The DeleteEnhMetaFile function deletes an enhanced-format metafile or an enhanced-format metafile handle.
            /// </summary>
            /// <param name="hmf">A handle to an enhanced metafile.</param>
            /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
            [DllImport("gdi32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool DeleteEnhMetaFile(IntPtr hmf);

            /// <summary>
            /// The SetWinMetaFileBits function converts a metafile from the older Windows format to the new enhanced format and stores the new metafile in memory.
            /// </summary>
            /// <param name="nSize">The size, in bytes, of the buffer that contains the Windows-format metafile.</param>
            /// <param name="lpMeta16Data">A pointer to a buffer that contains the Windows-format metafile data. (It is assumed that the data was obtained by using the GetMetaFileBitsEx or GetWinMetaFileBits function.)</param>
            /// <param name="hdcRef">A handle to a reference device context.</param>
            /// <param name="lpMFP">A pointer to a METAFILEPICT structure that contains the suggested size of the metafile picture and the mapping mode that was used when the picture was created.</param>
            /// <returns>If the function succeeds, the return value is a handle to a memory-based enhanced metafile. If the function fails, the return value is NULL.-**</returns>
            [DllImport("gdi32.dll")]
            internal static extern IntPtr SetWinMetaFileBits(uint nSize, [In] byte[] lpMeta16Data, IntPtr hdcRef, [In] ref METAFILEPICT lpMFP);

            #endregion
        }

        #endregion

        #region Methods

        internal static uint GetMetaFileBitsEx(IntPtr handle, uint size, byte[]? buf) => NativeMethods.GetMetaFileBitsEx(handle, size, buf);
        internal static void DeleteMetaFile(IntPtr handle) => NativeMethods.DeleteMetaFile(handle);
        internal static IntPtr CopyEnhMetaFile(IntPtr handle) => NativeMethods.CopyEnhMetaFile(handle, IntPtr.Zero);
        internal static bool DeleteEnhMetaFile(IntPtr handle) => NativeMethods.DeleteEnhMetaFile(handle);

        internal static IntPtr SetWinMetaFileBits(uint size, byte[] wmfData, IntPtr hdcRef, ref METAFILEPICT metafilepict)
            => NativeMethods.SetWinMetaFileBits(size, wmfData, hdcRef, ref metafilepict);

        #endregion
    }
}
