#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DefaultTheme.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2025 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    /// <summary>
    /// Represents the available default themes of the operating system. Has no effect below Windows 10, or when high contrast mode is enabled.
    /// To customize the individual colors of the theme, use the <see cref="ThemeColors"/> class.
    /// </summary>
    public enum DefaultTheme
    {
        // NOTE: The values of this enum are used in the native UxTheme.SetPreferredAppMode function, so they should not be rearranged.

        /// <summary>
        /// Represents the light or classic theme of the operating system. In Windows 8 and above this means the light theme,
        /// while in Windows 7 and below it represents the actual theme, including the customizable classic themes.
        /// </summary>
        Classic,

        /// <summary>
        /// Represents the preferred theme (light or dark) of the operating system. Applicable only in Windows 10 and above;
        /// otherwise it is equivalent to <see cref="Classic"/>.
        /// </summary>
        System,

        /// <summary>
        /// Represents the dark theme, if available. Applicable only in Windows 10 and above; otherwise it has no effect.
        /// </summary>
        Dark,
    }
}