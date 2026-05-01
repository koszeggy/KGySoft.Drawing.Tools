#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ClipboardHelper.cs
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

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal static class ClipboardHelper
    {
        #region Events

        internal static event EventHandler ClipboardChanged; // TODO

        #endregion

        #region Properties

        internal static bool HasImage { get; } // TODO

        #endregion
    }
}