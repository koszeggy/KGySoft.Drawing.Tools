#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: METAFILEPICT.cs
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
    /// Defines the metafile picture format used for exchanging metafile data through the clipboard.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "WinAPI")]
    internal struct METAFILEPICT
    {
        #region Fields

        internal short mm;
        internal short xExt;
        internal short yExt;
        internal IntPtr hMF;

        #endregion
    }
}