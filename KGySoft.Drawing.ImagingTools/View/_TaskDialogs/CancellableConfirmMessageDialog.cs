#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CancellableConfirmMessageDialog.cs
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
    internal class CancellableConfirmMessageDialog : MessageDialogBase
    {
        #region Properties

        #region Protected Properties

        protected override TaskDialogStandardButtons Buttons => TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No | TaskDialogStandardButtons.Cancel;

        protected override int DefaultButtonIndex => ViewModel.DefaultButtonIndex;

        #endregion

        #region Private Properties

        private new CancellableConfirmMessageViewModel ViewModel => (CancellableConfirmMessageViewModel)base.ViewModel;

        #endregion

        #endregion

        #region Constructors

        internal CancellableConfirmMessageDialog(CancellableConfirmMessageViewModel viewModel) : base(viewModel)
        {
            TaskDialog.Icon = TaskDialogStandardIcon.Question;
        }

        #endregion
    }
}