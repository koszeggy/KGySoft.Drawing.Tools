#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: MvvmBaseUserControl.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2022 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Threading;
using System.Windows.Forms;

using KGySoft.ComponentModel;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal class MvvmBaseUserControl : BaseUserControl
    {
        #region Fields

        private readonly int threadId;
        private readonly ManualResetEventSlim handleCreated;

        private ViewModelBase? viewModel;
        private bool isLoaded;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the view model. Can be null before initializing. Not null if called from <see cref="ApplyViewModel"/>.
        /// </summary>
        protected ViewModelBase? ViewModel
        {
            get => viewModel;
            set
            {
                if (ReferenceEquals(viewModel, value))
                    return;

                viewModel = value;
                if (!isLoaded)
                    return;

                CommandBindings.Dispose();
                ApplyViewModel();
            }
        }

        protected CommandBindingsCollection CommandBindings { get; }

        #endregion

        #region Constructors

        protected MvvmBaseUserControl()
        {
            CommandBindings = new WinFormsCommandBindingsCollection();
            threadId = Thread.CurrentThread.ManagedThreadId;
            handleCreated = new ManualResetEventSlim();
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            isLoaded = true;
            ApplyResources();
            if (DesignMode)
                return;

            if (viewModel != null)
                ApplyViewModel();
        }

        protected virtual void ApplyResources()
        {
            // Not calling ApplyStaticStringResources because we assume this UC belong to an MvvmBaseForm.
            // If not, then a derived instance still can call it.
        }

        protected virtual void ApplyViewModel()
        {
            ViewModelBase? vm = ViewModel;
            if (vm == null)
                return;

            vm.ShowInfoCallback = Dialogs.InfoMessage;
            vm.ShowWarningCallback = Dialogs.WarningMessage;
            vm.ShowErrorCallback = Dialogs.ErrorMessage;
            vm.ConfirmCallback = Dialogs.ConfirmMessage;
            vm.CancellableConfirmCallback = (msg, btn) => Dialogs.CancellableConfirmMessage(msg, btn switch { 0 => MessageBoxDefaultButton.Button1, 1 => MessageBoxDefaultButton.Button2, _ => MessageBoxDefaultButton.Button3 });
            vm.SynchronizedInvokeCallback = InvokeIfRequired;

            vm.ViewLoaded();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            handleCreated.Set();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CommandBindings.Dispose();
                handleCreated.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void InvokeIfRequired(Action action)
        {
            if (Disposing || IsDisposed)
                return;

            try
            {
                // no invoke is required (not using InvokeRequired because that may return false if handle is not created yet)
                if (threadId == Thread.CurrentThread.ManagedThreadId)
                {
                    action.Invoke();
                    return;
                }

                if (!handleCreated.IsSet)
                    handleCreated.Wait();
                Invoke(action);
            }
            catch (ObjectDisposedException)
            {
                // it can happen that actual Invoke is started to execute only after querying isClosing and when Disposing and IsDisposed both return false
            }
        }

        #endregion

        #endregion
    }
}
