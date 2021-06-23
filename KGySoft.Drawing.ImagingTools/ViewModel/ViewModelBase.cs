﻿#region Copyright

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
using System.Globalization;
using System.Threading;

using KGySoft.ComponentModel;
using KGySoft.Resources;

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
        internal Func<string, int, bool?>? CancellableConfirmCallback { get => Get<Func<string, int, bool?>?>(); set => Set(value); }
        internal Action<IViewModel>? ShowChildViewCallback { get => Get<Action<IViewModel>?>(); set => Set(value); }
        internal Action? CloseViewCallback { get => Get<Action?>(); set => Set(value); }
        internal Action<Action>? SynchronizedInvokeCallback { private get => Get<Action<Action>?>(); set => Set(value); }

        #endregion

        #region Constructors

        protected ViewModelBase()
        {
            // Applying the configuration settings in each VM might seem to be an overkill when executed as a regular application but by using the factories from a
            // consumer code any VM can be created in any order, in any thread. The VS extension creates the default VM always in a new thread, for example.
            bool allowResXResources = Configuration.AllowResXResources;
            CultureInfo desiredDisplayLanguage = allowResXResources
                ? Configuration.UseOSLanguage ? Res.OSLanguage : Configuration.DisplayLanguage // here, allowing specific languages, too
                : Res.DefaultLanguage;

            LanguageSettings.DisplayLanguage = Equals(desiredDisplayLanguage, CultureInfo.InvariantCulture) ? Res.DefaultLanguage : desiredDisplayLanguage;
            LanguageSettings.DynamicResourceManagersSource = allowResXResources ? ResourceManagerSources.CompiledAndResX : ResourceManagerSources.CompiledOnly;

            LanguageSettings.DisplayLanguageChangedGlobal += LanguageSettings_DisplayLanguageChangedGlobal;
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected void ShowError(string message) => ShowErrorCallback?.Invoke(message);
        protected void ShowWarning(string message) => ShowWarningCallback?.Invoke(message);
        protected void ShowInfo(string message) => ShowInfoCallback?.Invoke(message);
        protected bool Confirm(string message, bool isYesDefault = true) => ConfirmCallback?.Invoke(message, isYesDefault) ?? true;
        protected bool? CancellableConfirm(string message, int defaultButton = 0) => CancellableConfirmCallback?.Invoke(message, defaultButton);

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

            LanguageSettings.DisplayLanguageChangedGlobal -= LanguageSettings_DisplayLanguageChangedGlobal;
            base.Dispose(disposing);
        }

        #endregion

        #region Internal Methods

        internal virtual void ViewLoaded() => SetModified(false);

        #endregion

        #region Event Handlers

        private void LanguageSettings_DisplayLanguageChangedGlobal(object sender, EventArgs e)
        {
            // As we get notification from any thread we read the current thread settings from the configuration
            bool allowResXResources = Configuration.AllowResXResources;
            CultureInfo desiredDisplayLanguage = allowResXResources
                ? Configuration.UseOSLanguage ? Res.OSLanguage : Configuration.DisplayLanguage
                : Res.DefaultLanguage;
            if (Equals(desiredDisplayLanguage, CultureInfo.InvariantCulture))
                desiredDisplayLanguage = Res.DefaultLanguage;

            // The event was raised from another thread
            if (!Equals(LanguageSettings.DisplayLanguage, desiredDisplayLanguage))
            {
                LanguageSettings.DisplayLanguage = desiredDisplayLanguage;

                // we exit here because the line above end up calling this handler again from current thread
                return;
            }

            // Trying to apply the new language in the thread of the corresponding view
            if (!TryInvokeSync(ApplyDisplayLanguage))
                ApplyDisplayLanguage();
        }

        #endregion

        #endregion
    }
}