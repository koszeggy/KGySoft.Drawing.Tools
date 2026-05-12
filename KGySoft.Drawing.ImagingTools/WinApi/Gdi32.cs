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
            /// The DeleteObject function deletes a logical pen, brush, font, bitmap, region, or palette, freeing all system resources associated with the object. After the object is deleted, the specified handle is no longer valid.
            /// </summary>
            /// <param name="hObject">A handle to a logical pen, brush, font, bitmap, region, or palette.</param>
            /// <returns>If the function succeeds, the return value is nonzero.</returns>
            [DllImport("gdi32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool DeleteObject(IntPtr hObject);

            /// <summary>
            /// This function creates a memory device context (DC) compatible with the specified device.
            /// </summary>
            /// <param name="hdc">[in] Handle to an existing device context.
            /// If this handle is NULL, the function creates a memory device context compatible with the application's current screen. </param>
            /// <returns>The handle to a memory device context indicates success.
            /// NULL indicates failure.
            /// To get extended error information, call GetLastError.</returns>
            [DllImport("gdi32.dll", SetLastError = true)]
            internal static extern IntPtr CreateCompatibleDC(IntPtr hdc);

            /// <summary>The DeleteDC function deletes the specified device context (DC).</summary>
            /// <param name="hdc">A handle to the device context.</param>
            /// <returns>If the function succeeds, the return value is <see langword="true"/>. If the function fails, the return value is <see langword="false"/>.</returns>
            /// <remarks>
            /// An application must not delete a DC whose handle was obtained by calling the GetDC function. Instead, it must call the ReleaseDC function to free the DC.
            /// </remarks>
            [DllImport("gdi32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool DeleteDC(IntPtr hdc);

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

            /// <summary>
            /// The CreateDIBSection function creates a DIB that applications can write to directly. The function gives you a pointer to the location of the bitmap bit values. You can supply a handle to a file-mapping object that the function will use to create the bitmap, or you can let the system allocate the memory for the bitmap.
            /// </summary>
            /// <param name="hdc">A handle to a device context. If the value of iUsage is DIB_PAL_COLORS, the function uses this device context's logical palette to initialize the DIB colors.</param>
            /// <param name="pbmi">A pointer to a BITMAPINFO structure that specifies various attributes of the DIB, including the bitmap dimensions and colors.</param>
            /// <param name="iUsage">The type of data contained in the bmiColors array member of the BITMAPINFO structure pointed to by pbmi (either logical palette indexes or literal RGB values). The following values are defined.
            /// <para>DIB_PAL_COLORS - The bmiColors member is an array of 16-bit indexes into the logical palette of the device context specified by hdc.</para>
            /// <para>DIB_RGB_COLORS - The BITMAPINFO structure contains an array of literal RGB values.</para>
            /// </param>
            /// <param name="ppvBits">A pointer to a variable that receives a pointer to the location of the DIB bit values.</param>
            /// <param name="hSection">A handle to a file-mapping object that the function will use to create the DIB. This parameter can be NULL.</param>
            /// <param name="dwOffset">The offset from the beginning of the file-mapping object referenced by hSection where storage for the bitmap bit values is to begin. This value is ignored if hSection is NULL. The bitmap bit values are aligned on doubleword boundaries, so dwOffset must be a multiple of the size of a DWORD.</param>
            /// <returns>If the function succeeds, the return value is a handle to the newly created DIB, and *ppvBits points to the bitmap bit values.
            /// If the function fails, the return value is NULL, and ppvBits is NULL.</returns>
            [DllImport("gdi32.dll")]
            internal static extern IntPtr CreateDIBSection(IntPtr hdc, IntPtr pbmi, int iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

            #endregion
        }

        #endregion

        #region Methods

        internal static void DeleteObject(IntPtr handle) => NativeMethods.DeleteObject(handle);
        internal static IntPtr CreateCompatibleDC(IntPtr hdc) => NativeMethods.CreateCompatibleDC(hdc);
        internal static bool DeleteDC(IntPtr hdc) => NativeMethods.DeleteDC(hdc);
        internal static uint GetMetaFileBitsEx(IntPtr handle, uint size, byte[]? buf) => NativeMethods.GetMetaFileBitsEx(handle, size, buf);
        internal static void DeleteMetaFile(IntPtr handle) => NativeMethods.DeleteMetaFile(handle);
        internal static IntPtr CopyEnhMetaFile(IntPtr handle) => NativeMethods.CopyEnhMetaFile(handle, IntPtr.Zero);
        internal static bool DeleteEnhMetaFile(IntPtr handle) => NativeMethods.DeleteEnhMetaFile(handle);

        internal static IntPtr SetWinMetaFileBits(uint size, byte[] wmfData, IntPtr hdcRef, ref METAFILEPICT metafilepict)
            => NativeMethods.SetWinMetaFileBits(size, wmfData, hdcRef, ref metafilepict);

        internal static IntPtr CreateDibSection(IntPtr hdc, IntPtr bitmapInfo, out IntPtr bits)
            => NativeMethods.CreateDIBSection(hdc, bitmapInfo, Constants.DIB_RGB_COLORS, out bits, IntPtr.Zero, 0);

        #endregion
    }
}
