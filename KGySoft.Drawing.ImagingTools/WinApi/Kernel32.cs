#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Kernel32.cs
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
#if NET45_OR_GREATER
using System.ComponentModel;
#endif
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;

#endregion

namespace KGySoft.Drawing.ImagingTools.WinApi
{
    [SecurityCritical]
    internal static class Kernel32
    {
        #region NativeMethods class

        [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass", Justification = "Not an issue, always the outer class calls the NativeMethods members")]
        private static class NativeMethods
        {
            #region Methods

#if NET45_OR_GREATER
            /// <summary>
            /// Establishes a hard link between an existing file and a new file. This function is only supported on the NTFS file system, and only for files, not directories.
            /// </summary>
            /// <param name="lpFileName">The name of the new file.
            /// This parameter may include the path but cannot specify the name of a directory.
            /// In the ANSI version of this function, the name is limited to MAX_PATH characters. To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path. For more information, see Naming a File. If you pass a name longer than MAX_PATH characters to the ANSI version of this function or to the Unicode version of this function without prepending "\\?\" to the path, the function returns ERROR_PATH_NOT_FOUND.</param>
            /// <param name="lpExistingFileName">The name of the existing file.
            /// This parameter may include the path cannot specify the name of a directory.
            /// In the ANSI version of this function, the name is limited to MAX_PATH characters. To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path. For more information, see Naming a File. If you pass a name longer than MAX_PATH characters to the ANSI version of this function or to the Unicode version of this function without prepending "\\?\" to the path, the function returns ERROR_PATH_NOT_FOUND.</param>
            /// <param name="lpSecurityAttributes">Reserved; must be NULL.</param>
            /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. To get extended error information, call GetLastError.</returns>
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes);
#endif

            /// <summary>
            /// Retrieves the thread identifier of the calling thread.
            /// </summary>
            /// <returns>The return value is the thread identifier of the calling thread.</returns>
            [DllImport("kernel32.dll")]
            internal static extern uint GetCurrentThreadId();

            /// <summary>
            /// Allocates the specified number of bytes from the heap.
            /// </summary>
            /// <param name="uFlags">The memory allocation attributes. If zero is specified, the default is GMEM_FIXED.</param>
            /// <param name="dwBytes">The number of bytes to allocate. If this parameter is zero and the uFlags parameter specifies GMEM_MOVEABLE,
            /// the function returns a handle to a memory object that is marked as discarded.</param>
            /// <returns>If the function succeeds, the return value is a handle to the newly allocated memory object.
            /// If the function fails, the return value is NULL. To get extended error information, call GetLastError.</returns>
            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern IntPtr GlobalAlloc(uint uFlags, nuint dwBytes);

            /// <summary>
            /// Frees the specified global memory object and invalidates its handle.
            /// </summary>
            /// <param name="hMem">A handle to the global memory object. This handle is returned by either the GlobalAlloc or GlobalReAlloc function. It is not safe to free memory allocated with LocalAlloc.</param>
            /// <returns>If the function succeeds, the return value is NULL.
            /// If the function fails, the return value is equal to a handle to the global memory object. To get extended error information, call GetLastError.</returns>
            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern IntPtr GlobalFree(IntPtr hMem);

            /// <summary>
            /// Locks a global memory object and returns a pointer to the first byte of the object's memory block.
            /// </summary>
            /// <param name="hMem">A handle to the global memory object. This handle is returned by either the GlobalAlloc or GlobalReAlloc function.</param>
            /// <returns>If the function succeeds, the return value is a pointer to the first byte of the memory block.
            /// If the function fails, the return value is NULL.To get extended error information, call GetLastError.</returns>
            /// <remarks>
            /// <para>The internal data structures for each memory object include a lock count that is initially zero. For movable memory objects, GlobalLock increments the count by one, and the GlobalUnlock function decrements the count by one. Each successful call that a process makes to GlobalLock for an object must be matched by a corresponding call to GlobalUnlock. Locked memory will not be moved or discarded, unless the memory object is reallocated by using the GlobalReAlloc function. The memory block of a locked memory object remains locked until its lock count is decremented to zero, at which time it can be moved or discarded.</para>
            /// <para>Memory objects allocated with GMEM_FIXED always have a lock count of zero. For these objects, the value of the returned pointer is equal to the value of the specified handle.</para>
            /// <para>If the specified memory block has been discarded or if the memory block has a zero-byte size, this function returns NULL.</para>
            /// <para>Discarded objects always have a lock count of zero.</para>
            /// </remarks>
            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern IntPtr GlobalLock(IntPtr hMem);

            /// <summary>
            /// Decrements the lock count associated with a memory object that was allocated with GMEM_MOVEABLE. This function has no effect on memory objects allocated with GMEM_FIXED.
            /// </summary>
            /// <param name="hMem">A handle to the global memory object. This handle is returned by either the GlobalAlloc or GlobalReAlloc function.</param>
            /// <returns>If the memory object is still locked after decrementing the lock count, the return value is a nonzero value. If the memory object is unlocked after decrementing the lock count, the function returns zero and GetLastError returns NO_ERROR.
            /// If the function fails, the return value is zero and GetLastError returns a value other than NO_ERROR.</returns>
            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool GlobalUnlock(IntPtr hMem);

            /// <summary>
            /// Retrieves the current size of the specified global memory object, in bytes.
            /// </summary>
            /// <param name="hMem">A handle to the global memory object. This handle is returned by either the GlobalAlloc or GlobalReAlloc function.</param>
            /// <returns>If the function succeeds, the return value is the size of the specified global memory object, in bytes.
            /// If the specified handle is not valid or if the object has been discarded, the return value is zero. To get extended error information, call GetLastError.</returns>
            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern nuint GlobalSize(IntPtr hMem);

            #endregion
        }

        #endregion

        #region Methods

#if NET45_OR_GREATER

        internal static void CreateHardLink(string linkName, string existingFileName)
        {
            const string allowLongPathPrefix = @"\\?\";
            if (!NativeMethods.CreateHardLink(allowLongPathPrefix + linkName, allowLongPathPrefix + existingFileName, IntPtr.Zero))
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }
#endif

        internal static uint GetCurrentThreadId() => NativeMethods.GetCurrentThreadId();
        internal static IntPtr GlobalAlloc(uint flags, int size) => NativeMethods.GlobalAlloc(flags, (nuint)size);
        internal static void GlobalFree(IntPtr handle) => NativeMethods.GlobalFree(handle);
        internal static IntPtr GlobalLock(IntPtr handle) => NativeMethods.GlobalLock(handle);
        internal static void GlobalUnlock(IntPtr handle) => NativeMethods.GlobalUnlock(handle);
        internal static nuint GlobalSize(IntPtr handle) => NativeMethods.GlobalSize(handle);

        #endregion
    }
}
