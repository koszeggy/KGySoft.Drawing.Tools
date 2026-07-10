#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ViewModelBase.cs
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

        #region Internal Properties
        
        internal Action<string, object[]?>? ShowErrorCallback { get => Get<Action<string, object[]?>?>(); set => Set(value); }
        internal Action<string, object[]?>? ShowWarningCallback { get => Get<Action<string, object[]?>?>(); set => Set(value); }
        internal Action<string, object[]?>? ShowInfoCallback { get => Get<Action<string, object[]?>?>(); set => Set(value); }
        internal Func<string, object[]?, bool, bool>? ConfirmCallback { get => Get<Func<string, object[]?, bool, bool>?>(); set => Set(value); }
        internal Func<string, object[]?, int, bool?>? CancellableConfirmCallback { get => Get<Func<string, object[]?, int, bool?>?>(); set => Set(value); }
        internal Action<IViewModel>? ShowChildViewCallback { get => Get<Action<IViewModel>?>(); set => Set(value); }
        internal Action? CloseViewCallback { get => Get<Action?>(); set => Set(value); }
        internal Action<Action>? SynchronizedInvokeCallback { private get => Get<Action<Action>?>(); set => Set(value); }

        #endregion

        #region Protected Properties

        protected bool IsViewLoaded { get; private set; }

        #endregion

        #endregion

        #region Constructors

        protected ViewModelBase() => Res.DisplayLanguageChanged += Res_DisplayLanguageChanged;

        #endregion

        #region Methods

        #region Internal Methods

        internal void SuspendChanges() => SuspendChangedEvent();
        internal void ResumeChanges() => ResumeChangedEvent();

        #endregion

        #region Protected Methods

        // NOTE: Func<string> args can be used to provide localized arguments that are reevaluated on language change
        protected void ShowError(string resourceId, params object[]? args) => ShowErrorCallback?.Invoke(resourceId, args);
        protected void ShowWarning(string resourceId, params object[]? args) => ShowWarningCallback?.Invoke(resourceId, args);
        protected void ShowInfo(string resourceId, params object[]? args) => ShowInfoCallback?.Invoke(resourceId, args);
        protected bool Confirm(string resourceId, object[]? args = null, bool isYesDefault = true) => ConfirmCallback?.Invoke(resourceId, args, isYesDefault) ?? true;
        protected bool? CancellableConfirm(string resourceId, object[]? args = null, int defaultButton = 0) => CancellableConfirmCallback?.Invoke(resourceId, args, defaultButton);

        protected bool TryInvokeSync(Action action)
        {
            if (IsDisposed)
                return false;

            try
            {
                Action<Action>? callback = SynchronizedInvokeCallback;
                if (callback == null)
                    return false;
                callback.Invoke(action);
                return true;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }

        protected virtual void ApplyDisplayLanguage() { }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            Res.DisplayLanguageChanged -= Res_DisplayLanguageChanged;
            base.Dispose(disposing);
        }

        #endregion

        #region Internal Methods

        internal virtual void ViewLoaded()
        {
            IsViewLoaded = true;
            SetModified(false);
        }

        internal virtual void ViewShown() { }
        internal virtual void ViewUnloading() { }

        #endregion

        #region Event Handlers

        private void Res_DisplayLanguageChanged(object? sender, EventArgs e)
            // Trying to apply the new language in the thread of the corresponding view
            => TryInvokeSync(ApplyDisplayLanguage);

        #endregion

        #endregion
    }
}