#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: MvvmBaseForm.cs
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
using System.Windows.Forms;

using KGySoft.ComponentModel;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    internal class MvvmBaseForm<TViewModel> : BaseForm, IView
        where TViewModel : IDisposable // BUG: Actually should be ViewModelBase but WinForms designer with derived forms dies from that
    {
        #region Fields

        private bool isClosing;

        #endregion

        #region Properties

        #region Protected Properties

        protected TViewModel ViewModel { get; }
        protected CommandBindingsCollection CommandBindings { get; }

        #endregion

        #region Private Properties

        // this would not be needed if where TViewModel : ViewModelBase didn't conflict with WinForms designer
        private ViewModelBase VM => (ViewModelBase)(object)ViewModel;

        #endregion

        #endregion

        #region Constructors

        #region Protected Constructors

        protected MvvmBaseForm(TViewModel viewModel)
        {
            // occurs in design mode but DesignMode is false for grandchild forms
            if (viewModel == null)
                return;
            ViewModel = viewModel;

            ViewModelBase vm = VM;
            vm.ShowInfoCallback = Dialogs.InfoMessage;
            vm.ShowWarningCallback = Dialogs.WarningMessage;
            vm.ShowErrorCallback = Dialogs.ErrorMessage;
            vm.ConfirmCallback = Dialogs.ConfirmMessage;
            vm.ShowChildViewCallback = ShowChildView;
            vm.CloseViewCallback = () => BeginInvoke(new Action(Close));
            vm.SynchronizedInvokeCallback = InvokeIfRequired;

            CommandBindings = new WinformsCommandBindingsCollection();
        }

        #endregion

        #region Private Constructors

        private MvvmBaseForm()
        {
            // this ctor is just for the designer
        }

        #endregion

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (ViewModel == null)
                return;
            ApplyResources();
            ApplyViewModel();
        }

        protected virtual void ApplyResources() => this.ApplyStaticStringResources();

        protected virtual void ApplyViewModel() => VM.ViewLoaded();

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!e.Cancel)
                isClosing = true;
            base.OnFormClosing(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                CommandBindings?.Dispose();

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void ShowChildView(IViewModel vm) => ViewFactory.ShowDialog(vm, Handle);

        private void InvokeIfRequired(Action action)
        {
            if (isClosing || Disposing || IsDisposed)
                return;
            try
            {
                if (InvokeRequired)
                    Invoke(action);
                else
                    action.Invoke();
            }
            catch (ObjectDisposedException)
            {
                // it can happen that actual Invoke is started to execute only after querying isClosing and when Disposing and IsDisposed both return false
            }
        }

        #endregion

        #region Explicit Interface Implementations

        void IView.ShowDialog(IntPtr ownerHandle) => ShowDialog(ownerHandle == IntPtr.Zero ? null : new OwnerWindowHandle(ownerHandle));

        void IView.Show()
        {
            if (!Visible)
            {
                Show();
                return;
            }

            if (WindowState == FormWindowState.Minimized)
                WindowState = FormWindowState.Normal;
            Activate();
            BringToFront();
        }

        #endregion

        #endregion
    }
}