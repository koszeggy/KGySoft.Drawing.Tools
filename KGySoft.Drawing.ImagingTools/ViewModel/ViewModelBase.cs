#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ViewModelBase.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2020 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution. If not, then this file is considered as
//  an illegal copy.
//
//  Unauthorized copying of this file, via any medium is strictly prohibited.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;

using KGySoft.ComponentModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    /// <summary>
    /// Represents a base class for ViewModel types in this project.
    /// </summary>
    internal abstract class ViewModelBase : ObservableObjectBase, IViewModel
    {
        #region Properties

        internal Action<string>? ShowErrorCallback { get => Get<Action<string>?>(); set => Set(value); }
        internal Action<string>? ShowWarningCallback { get => Get<Action<string>?>(); set => Set(value); }
        internal Action<string>? ShowInfoCallback { get => Get<Action<string>?>(); set => Set(value); }
        internal Func<string, bool>? ConfirmCallback { get => Get<Func<string, bool>?>(); set => Set(value); }
        internal Action<IViewModel>? ShowChildViewCallback { get => Get<Action<IViewModel>?>(); set => Set(value); }
        internal Action? CloseViewCallback { get => Get<Action?>(); set => Set(value); }
        internal Action<Action>? SynchronizedInvokeCallback { get => Get<Action<Action>?>(); set => Set(value); }

        #endregion

        #region Methods

        #region Protected Methods

        protected void ShowError(string message) => ShowErrorCallback?.Invoke(message);
        protected void ShowWarning(string message) => ShowWarningCallback?.Invoke(message);
        protected void ShowInfo(string message) => ShowInfoCallback?.Invoke(message);
        protected bool Confirm(string message) => ConfirmCallback?.Invoke(message) ?? true;

        #endregion

        #region Internal Methods

        internal virtual void ViewLoaded() => SetModified(false);

        #endregion

        #endregion
    }
}