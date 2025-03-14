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

using System.Drawing;
using System.Windows.Forms;

#endregion

namespace KGySoft.Drawing.ImagingTools.View
{
    internal sealed class ParentViewProperties
    {
        #region Properties

        internal string? Name { get; set; }
        internal FormBorderStyle BorderStyle { get; set; } = FormBorderStyle.Sizable;
        internal Icon? Icon { get; set; }
        internal IButtonControl? AcceptButton { get; set; }
        internal IButtonControl? CancelButton { get; set; }
        internal Size MinimumSize { get; set; }
        internal Size MaximumSize { get; set; }
        internal FormClosingEventHandler? ClosingCallback { get; set; }

        #endregion
    }
}