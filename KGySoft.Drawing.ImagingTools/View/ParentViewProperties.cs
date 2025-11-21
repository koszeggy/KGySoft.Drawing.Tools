#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ParentViewProperties.cs
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
using System.Drawing;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.View.Forms;

#endregion

namespace KGySoft.Drawing.ImagingTools.View
{
    internal sealed class ParentViewProperties
    {
        #region Properties

        internal FormBorderStyle BorderStyle { get; set; } = FormBorderStyle.Sizable;
        internal bool HideMinimizeButton { get; set; }
        internal Icon? Icon { get; set; }
        internal IButtonControl? AcceptButton { get; set; }
        internal IButtonControl? CancelButton { get; set; }
        internal Size MinimumSize { get; set; }
        internal FormClosingEventHandler? ClosingCallback { get; set; }
        internal Func<MvvmParentForm, Keys, bool>? ProcessKeyCallback { get; set; }

        #endregion
    }
}