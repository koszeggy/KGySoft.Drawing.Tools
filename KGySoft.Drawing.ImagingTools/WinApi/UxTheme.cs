#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: UxTheme.cs
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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

#endregion

namespace KGySoft.Drawing.ImagingTools.WinApi
{
    internal static class UxTheme
    {
        #region Nested classes

        #region NativeMethods class

        [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass", Justification = "Outer class methods are never called from here.")]
        private static class NativeMethods
        {
            #region Methods

            /// <summary>
            /// Causes a window to use a different set of visual style information than its class normally uses.
            /// </summary>
            /// <param name="hWnd">Handle to the window whose visual style information is to be changed.</param>
            /// <param name="pszSubAppName">Pointer to a string that contains the application name to use in place of the calling application's name. If this parameter is NULL, the calling application's name is used.</param>
            /// <param name="pszSubIdList">Pointer to a string that contains a semicolon-separated list of CLSID names to use in place of the actual list passed by the window's class. If this parameter is NULL, the ID list from the calling class is used.</param>
            /// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
            [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
            internal extern static int SetWindowTheme(IntPtr hWnd, string? pszSubAppName, string? pszSubIdList);

            /// <summary>
            /// Non-documented API, ordinal 133
            /// Applies the specified mode for the control represented by the specified window handle. Affects TextBox and ComboBox controls when their theme application name is set to "CFD".
            /// </summary>
            [DllImport("uxtheme.dll", EntryPoint = "#133", SetLastError = true)]
            internal static extern bool SetWindowDarkMode(IntPtr hWnd, bool isDarkModeAllowed);

            /// <summary>
            /// Non-documented API, ordinal 135
            /// Sets the preferred application mode (light or dark) for the current process. Affects the background color of the context menus.
            /// </summary>
            /// <param name="mode">In Windows 10 build 1809 (October 2018 Update) mode is interpreted as a bool: every non-zero value means that the dark mode is allowed.
            /// <br/>Starting with Windows 10 build 1903 (May 2019 Update) mode is an enum: 0 = default (light), 1 = system (allow dark), 2 = forced dark, 3 = forced light (not used in the DefaultTheme enum).</param>
            /// <returns>True if the operation was successful; otherwise, false.</returns>
            [DllImport("uxtheme.dll", EntryPoint = "#135", SetLastError = true)]
            internal static extern bool SetPreferredAppMode(int mode);

            /// <summary>
            /// Non-documented API, ordinal 136
            /// Applies the current theme for the already created taskbar menus of the current process.
            /// </summary>
            [DllImport("uxtheme.dll", EntryPoint = "#136")]
            internal static extern void FlushMenuThemes();

            #endregion
        }

        #endregion

        #endregion

        #region Methods

        internal static void SetWindowTheme(IntPtr hWnd, string? subAppName, string? subIdList)
        {
            if (!OSUtils.IsVistaOrLater)
                return;
            try
            {
                NativeMethods.SetWindowTheme(hWnd, subAppName, subIdList);
            }
            catch (Exception e) when (!e.IsCritical())
            {
            }
        }

        internal static void SetWindowDarkMode(IntPtr hWnd, bool isDarkModeAllowed)
        {
            if (!OSUtils.IsWindows10OrLater)
                return;
            try
            {
                NativeMethods.SetWindowDarkMode(hWnd, isDarkModeAllowed);
            }
            catch (Exception e) when (!e.IsCritical())
            {
            }
        }

        internal static void SetPreferredAppMode(DefaultTheme theme)
        {
            if (!OSUtils.IsWindows10OrLater)
                return;

            // In Windows 10 build 1809 (October 2018 Update) every non-default value means that the dark mode is allowed when it's the used one in the system.
            // Starting with Windows 10 build 1903 (May 2019 Update) the defined values of DefaultTheme enum can be used as they are.
            NativeMethods.SetPreferredAppMode((int)theme);
        }

        public static void FlushMenuThemes() => NativeMethods.FlushMenuThemes();

        #endregion

    }
}