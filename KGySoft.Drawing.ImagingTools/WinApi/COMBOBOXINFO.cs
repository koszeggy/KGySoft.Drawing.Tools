﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: COMBOBOXINFO.cs
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

#endregion

namespace KGySoft.Drawing.ImagingTools.WinApi
{
    internal struct COMBOBOXINFO
    {
        #region Fields

        internal uint cbSize;
        internal RECT rcItem;
        internal RECT rcButton;
        internal uint stateButton;
        internal IntPtr hwndCombo;
        internal IntPtr hwndItem;
        internal IntPtr hwndList;

        #endregion
    }
}