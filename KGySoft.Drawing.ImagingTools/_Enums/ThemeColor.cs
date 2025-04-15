#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ThemeColor.cs
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
    /// Represents the theme colors used in the application.
    /// </summary>
    public enum ThemeColor
    {
        /// <summary>
        /// Represents the background color of a control.
        /// </summary>
        Control,

        /// <summary>
        /// Represents the foreground color of a control.
        /// </summary>
        ControlText,

        /// <summary>
        /// Represents the foreground color of a disabled control.
        /// </summary>
        ControlTextDisabled,

        /// <summary>
        /// Represents the background color of the client area of a window.
        /// </summary>
        Window,

        /// <summary>
        /// Represents the foreground color of the client area of a window.
        /// </summary>
        WindowText,

        /// <summary>
        /// Represents the foreground color of the client area of a window when it is disabled.
        /// </summary>
        WindowTextDisabled,

        /// <summary>
        /// Represents the foreground color of a group box.
        /// </summary>
        GroupBoxText,
    }
}