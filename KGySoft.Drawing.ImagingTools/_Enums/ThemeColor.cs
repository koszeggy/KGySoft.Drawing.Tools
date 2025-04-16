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
        /// Represents the background color of an input window area.
        /// </summary>
        Window,

        /// <summary>
        /// Represents the foreground color of an input window area.
        /// </summary>
        WindowText,

        /// <summary>
        /// Represents the foreground color of an input window area when it is disabled.
        /// </summary>
        WindowTextDisabled,

        /// <summary>
        /// Represents the alternating background color of an input window area.
        /// </summary>
        WindowAlternate,

        /// <summary>
        /// Represents the alternating foreground color of an input window area.
        /// </summary>
        WindowTextAlternate,

        /// <summary>
        /// Represents the foreground color of a group box.
        /// </summary>
        GroupBoxText,

        /// <summary>
        /// Represents the color of grid lines.
        /// </summary>
        GridLine,

        /// <summary>
        /// Represents the color of unoccupied workspace area.
        /// </summary>
        Workspace,
    }
}