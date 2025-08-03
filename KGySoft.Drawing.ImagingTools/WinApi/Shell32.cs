#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Shell32.cs
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
using System.IO;
using System.Runtime.InteropServices;

#endregion

namespace KGySoft.Drawing.ImagingTools.WinApi
{
    internal static class Shell32
    {
        #region Nested classes

        #region NativeMethods class

        private static class NativeMethods
        {
            #region Methods

            /// <summary>
            /// Translates a Shell namespace object's display name into an item identifier list and returns the attributes of the object. This function is the preferred method to convert a string to a pointer to an item identifier list (PIDL).
            /// </summary>
            /// <param name="pszName">A pointer to a zero-terminated wide string that contains the display name to parse.</param>
            /// <param name="pbc">A bind context that controls the parsing operation. This parameter is normally set to NULL.</param>
            /// <param name="ppidl">The address of a pointer to a variable of type ITEMIDLIST that receives the item identifier list for the object. If an error occurs, then this parameter is set to NULL.</param>
            /// <param name="sfgaoIn">A ULONG value that specifies the attributes to query. To query for one or more attributes, initialize this parameter with the flags that represent the attributes of interest. For a list of available SFGAO flags, see SFGAO.</param>
            /// <param name="psfgaoOut">A pointer to a ULONG. On return, those attributes that are true for the object and were requested in sfgaoIn are set. An object's attribute flags can be zero or a combination of SFGAO flags. For a list of available SFGAO flags, see SFGAO.</param>
            /// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
            [DllImport("shell32.dll", SetLastError = true)]
            public static extern int SHParseDisplayName([MarshalAs(UnmanagedType.LPWStr)]string pszName, IntPtr pbc, out IntPtr ppidl, uint sfgaoIn, out uint psfgaoOut);

            /// <summary>
            /// Opens a Windows Explorer window with specified items in a particular folder selected.
            /// </summary>
            /// <param name="pidlFolder">A pointer to a fully qualified item ID list that specifies the folder.</param>
            /// <param name="cidl">A count of items in the selection array, apidl. If cidl is zero, then pidlFolder must point to a fully specified ITEMIDLIST describing a single item to select. This function opens the parent folder and selects that item.</param>
            /// <param name="apidl">A pointer to an array of PIDL structures, each of which is an item to select in the target folder referenced by pidlFolder.</param>
            /// <param name="dwFlags">The optional flags. Under Windows XP this parameter is ignored.</param>
            /// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
            [DllImport("shell32.dll", SetLastError = true)]
            public static extern int SHOpenFolderAndSelectItems(IntPtr pidlFolder, uint cidl, [In, MarshalAs(UnmanagedType.LPArray)]IntPtr[] apidl, uint dwFlags);

            #endregion
        }

        #endregion

        #endregion

        #region Methods

        internal static bool OpenFolderAndSelectItems(string path, params string[] fileNames)
        {
            IntPtr pidFolder = IntPtr.Zero;
            IntPtr[] pidFiles = new IntPtr[fileNames.Length];
            try
            {
                if (NativeMethods.SHParseDisplayName(path, IntPtr.Zero, out pidFolder, 0, out var _) != Constants.S_OK)
                    return false;

                for (int i = 0; i < fileNames.Length; i++)
                {
                    if (NativeMethods.SHParseDisplayName(Path.Combine(path, fileNames[i]), IntPtr.Zero, out pidFiles[i], 0, out var _) != Constants.S_OK)
                        return false;
                }

                return NativeMethods.SHOpenFolderAndSelectItems(pidFolder, (uint)pidFiles.Length, pidFiles, 0) == Constants.S_OK;
            }
            finally
            {
                if (pidFolder != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(pidFolder);
                foreach (IntPtr pid in pidFiles)
                {
                    if (pid != IntPtr.Zero)
                        Marshal.FreeCoTaskMem(pid);
                }
            }
        }

        #endregion
    }
}
