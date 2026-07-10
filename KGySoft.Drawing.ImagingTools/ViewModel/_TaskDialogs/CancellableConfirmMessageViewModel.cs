#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CancellableConfirmMessageViewModel.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2026 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class CancellableConfirmMessageViewModel : MessageBoxViewModel
    {
        #region Properties

        internal int DefaultButtonIndex { get; }

        #endregion

        #region Constructors

        internal CancellableConfirmMessageViewModel(string messageId, object[]? args, int defaultButton)
            : base(messageId, args, Res.TitleConfirmationId)
        {
            DefaultButtonIndex = defaultButton;
        }

        #endregion
    }
}