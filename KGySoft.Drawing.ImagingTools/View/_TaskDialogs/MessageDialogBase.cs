#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: MessageDialogBase.cs
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

using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.ViewModel;
using KGySoft.WinForms.Components;

#endregion

namespace KGySoft.Drawing.ImagingTools.View
{
    internal abstract class MessageDialogBase : MvvmTaskDialogBase
    {
        #region Properties

        private new MessageBoxViewModel ViewModel => (MessageBoxViewModel)base.ViewModel;

        #endregion

        #region Constructors

        protected MessageDialogBase(MessageBoxViewModel viewModel) : base(viewModel)
        {
        }

        #endregion

        #region Methods

        protected override void ApplyStringResources()
        {
            base.ApplyStringResources();
            TaskDialog.Caption = Res.Get(ViewModel.CaptionId);
            TaskDialog.Message = Res.Get(ViewModel.MessageId, ViewModel.MessageArguments);
        }

        protected override void OnClosed(TaskDialogResult result)
        {
            base.OnClosed(result);
            ViewModel.SelectedButtonIndex = TaskDialog.SelectedButtonIndex;
        }

        #endregion
    }
}