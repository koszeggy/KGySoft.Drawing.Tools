#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ConfirmMessageDialog.cs
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
    internal class ConfirmMessageDialog : MessageDialogBase
    {
        #region Properties

        #region Protected Properties

        protected override TaskDialogStandardButtons Buttons => TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No;
        protected override int DefaultButtonIndex => ViewModel.IsYesDefault ? 0 : 1;

        #endregion

        #region Private Properties

        private new ConfirmMessageViewModel ViewModel => (ConfirmMessageViewModel)base.ViewModel;

        #endregion

        #endregion

        #region Constructors

        internal ConfirmMessageDialog(ConfirmMessageViewModel viewModel) : base(viewModel)
        {
            TaskDialog.Icon = TaskDialogStandardIcon.Question;
        }

        #endregion
    }
}