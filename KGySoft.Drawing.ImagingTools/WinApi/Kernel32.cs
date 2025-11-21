#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Kernel32.cs
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
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;

#endregion

namespace KGySoft.Drawing.ImagingTools.WinApi
{
    [SecurityCritical]
    internal static class Kernel32
    {
        #region NativeMethods class

        private static class NativeMethods
        {
            #region Methods

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

            /// <summary>
            /// Retrieves information about the system's current usage of both physical and virtual memory.
            /// </summary>
            /// <param name="lpBuffer">A pointer to a <see cref="MEMORYSTATUSEX"/> structure that receives information about current memory availability.</param>
            /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. To get extended error information, call GetLastError.</returns>
            [return: MarshalAs(UnmanagedType.Bool)]
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

            /// <summary>
            /// Retrieves the thread identifier of the calling thread.
            /// </summary>
            /// <returns>The return value is the thread identifier of the calling thread.</returns>
            [DllImport("kernel32.dll")]
            internal static extern uint GetCurrentThreadId();

            #endregion
        }

        #endregion

        #region Methods

        internal static void CreateHardLink(string linkName, string existingFileName)
        {
            const string allowLongPathPrefix = @"\\?\";
            if (!NativeMethods.CreateHardLink(allowLongPathPrefix + linkName, allowLongPathPrefix + existingFileName, IntPtr.Zero))
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        internal static long GetTotalMemory()
        {
            var status = new MEMORYSTATUSEX { dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX)) };
            if (!NativeMethods.GlobalMemoryStatusEx(ref status))
                throw new Win32Exception(Marshal.GetLastWin32Error());
            return (long)status.ullTotalPhys;
        }

        internal static uint GetCurrentThreadId() => NativeMethods.GetCurrentThreadId();

        #endregion
    }
}
