#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: MessageBoxViewModel.cs
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
    internal abstract class MessageBoxViewModel : ViewModelBase<int>
    {
        #region Properties

        internal string MessageId { get; }
        internal object[]? MessageArguments { get; }
        internal string CaptionId { get; }

        internal int SelectedButtonIndex { get => Get(-1); set => Set(value); }

        #endregion

        #region Constructors

        protected MessageBoxViewModel(string messageId, object[]? args, string captionId)
        {
            MessageId = messageId;
            MessageArguments = args;
            CaptionId = captionId;
        }

        #endregion

        #region Methods

        public override int GetEditedModel() => SelectedButtonIndex;

        #endregion
    }
}