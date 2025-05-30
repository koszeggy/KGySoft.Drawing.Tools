﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ExceptionExtensions.cs
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

namespace KGySoft.Drawing.ImagingTools
{
    internal static class ExceptionExtensions
    {
        #region Methods

        internal static bool IsCritical(this Exception e) => e is OutOfMemoryException or StackOverflowException or AccessViolationException;

        // For GDI exceptions we allow even OutOfMemoryException
        internal static bool IsCriticalGdi(this Exception e) => e.IsCritical() && e is not OutOfMemoryException;

        #endregion
    }
}