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
        internal Func<string, bool, bool>? ConfirmCallback { get => Get<Func<string, bool, bool>?>(); set => Set(value); }
        internal Action<IViewModel>? ShowChildViewCallback { get => Get<Action<IViewModel>?>(); set => Set(value); }
        internal Action? CloseViewCallback { get => Get<Action?>(); set => Set(value); }
        internal Action<Action>? SynchronizedInvokeCallback { private get => Get<Action<Action>?>(); set => Set(value); }

        #endregion

        #region Constructors

        protected ViewModelBase() => LanguageSettings.DisplayLanguageChanged += LanguageSettings_DisplayLanguageChanged;

        #endregion

        #region Methods

        #region Protected Methods

        protected void ShowError(string message) => ShowErrorCallback?.Invoke(message);
        protected void ShowWarning(string message) => ShowWarningCallback?.Invoke(message);
        protected void ShowInfo(string message) => ShowInfoCallback?.Invoke(message);
        protected bool Confirm(string message, bool isYesDefault = true) => ConfirmCallback?.Invoke(message, isYesDefault) ?? true;

        protected bool TryInvokeSync(Action action)
        {
            if (IsDisposed)
                return false;

            Action<Action>? callback;
            try
            {
                callback = SynchronizedInvokeCallback;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }

            if (callback == null)
                return false;

            callback.Invoke(action);
            return true;
        }

        protected virtual void ApplyDisplayLanguage() { }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            LanguageSettings.DisplayLanguageChanged -= LanguageSettings_DisplayLanguageChanged;
            base.Dispose(disposing);
        }

        #endregion

        #region Internal Methods

        internal virtual void ViewLoaded() => SetModified(false);

        #endregion

        #region Event Handlers

        private void LanguageSettings_DisplayLanguageChanged(object sender, EventArgs e) => ApplyDisplayLanguage();

        #endregion

        #endregion
    }
}