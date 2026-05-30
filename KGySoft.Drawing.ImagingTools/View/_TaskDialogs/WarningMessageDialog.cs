#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: WarningMessageDialog.cs
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

using KGySoft.Drawing.ImagingTools.ViewModel;
using KGySoft.WinForms.Components;

#endregion

namespace KGySoft.Drawing.ImagingTools.View
{
    internal class WarningMessageDialog : MessageDialogBase
    {
        #region Properties

        protected override TaskDialogStandardButtons Buttons => TaskDialogStandardButtons.OK;

        #endregion

        #region Constructors

        internal WarningMessageDialog(WarningMessageViewModel viewModel) : base(viewModel)
        {
            TaskDialog.Icon = TaskDialogStandardIcon.Warning;
        }

        #endregion
    }
}