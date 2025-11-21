#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: WINDOWCOMPOSITIONATTRIBDATA.cs
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
    /// <summary>
    /// Describes a key/value pair that specifies a window composition attribute and its value. This structure is used with the GetWindowCompositionAttribute and SetWindowCompositionAttribute functions.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Windows API")]
    internal struct WINDOWCOMPOSITIONATTRIBDATA
    {
        #region Fields

        /// <summary>
        /// A flag describing which value to get or set, specified as a value of the WINDOWCOMPOSITIONATTRIB enumeration.
        /// This parameter specifies which attribute to get or set, and the pvData member points to an object containing the attribute value.
        /// </summary>
        internal WINDOWCOMPOSITIONATTRIB Attrib;
        /// <summary>
        /// When used with the GetWindowCompositionAttribute function, this member contains a pointer to a variable that will hold the value of the requested attribute when the function returns.
        /// When used with the SetWindowCompositionAttribute function, it points an object containing the attribute value to set. The type of the value set depends on the value of the Attrib member.
        /// </summary>
        internal IntPtr pvData;
        /// <summary>
        /// The size of the object pointed to by the pvData member, in bytes.
        /// </summary>
        internal uint cbData;

        #endregion
    }
}
