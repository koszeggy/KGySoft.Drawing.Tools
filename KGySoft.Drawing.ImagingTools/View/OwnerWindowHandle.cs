#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: OwnerWindowHandle.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2024 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Windows.Forms;

#endregion

namespace KGySoft.Drawing.ImagingTools.View
{
    internal class OwnerWindowHandle : IWin32Window
    {
        #region Constructors
        
        public OwnerWindowHandle(IntPtr ownerWindowHandle) => Handle = ownerWindowHandle;

        #endregion

        #region Properties

        public IntPtr Handle { get; }

        #endregion
    }
}