#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: MvvmBaseUserControl.cs
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
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows.Forms;

using KGySoft.ComponentModel;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal class MvvmBaseUserControl<TViewModel> : BaseUserControl
        where TViewModel : IDisposable // BUG: Actually should be ViewModelBase but WinForms designer with derived types dies from that
    {
        #region Fields

        private readonly int threadId;
        private readonly ManualResetEventSlim handleCreated;

        private TViewModel? vm;
        private bool isLoaded;

        #endregion

        #region Properties

        #region Internal Properties

        /// <summary>
        /// Gets or sets the view model. Can be null before initializing. Not null if called from <see cref="ApplyViewModel"/>.
        /// </summary>
        [MaybeNull]
        internal TViewModel ViewModel
        {
            get => vm;
            set
            {
                if (ReferenceEquals(vm, value))
                    return;
                vm = value;
                if (isLoaded)
                    ApplyViewModel();
            }
        }

        #endregion

        #region Protected Properties

        protected CommandBindingsCollection CommandBindings { get; }

        #endregion

        #region Private Properties

        // this would not be needed if where TViewModel : ViewModelBase didn't conflict with WinForms designer
        private ViewModelBase? VM => (ViewModelBase?)(object?)ViewModel;

        #endregion

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

            if (vm != null)
                ApplyViewModel();
        }

        protected virtual void ApplyResources()
        {
            // Not calling ApplyStaticStringResources because we assume this UC belong to an MvvmBaseForm.
            // If not, then a derived instance still can call it.
        }

        protected virtual void ApplyViewModel()
        {
            ViewModelBase vmb = VM!;
            vmb.ShowInfoCallback = Dialogs.InfoMessage;
            vmb.ShowWarningCallback = Dialogs.WarningMessage;
            vmb.ShowErrorCallback = Dialogs.ErrorMessage;
            vmb.ConfirmCallback = Dialogs.ConfirmMessage;
            vmb.CancellableConfirmCallback = (msg, btn) => Dialogs.CancellableConfirmMessage(msg, btn switch { 0 => MessageBoxDefaultButton.Button1, 1 => MessageBoxDefaultButton.Button2, _ => MessageBoxDefaultButton.Button3 });
            vmb.SynchronizedInvokeCallback = InvokeIfRequired;

            vmb.ViewLoaded();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            handleCreated.Set();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                CommandBindings.Dispose();

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
