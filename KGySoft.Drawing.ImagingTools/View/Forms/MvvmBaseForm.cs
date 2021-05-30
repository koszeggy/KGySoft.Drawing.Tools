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
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using KGySoft.ComponentModel;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    internal partial class MvvmBaseForm<TViewModel> : BaseForm, IView
        where TViewModel : IDisposable // BUG: Actually should be ViewModelBase but WinForms designer with derived forms dies from that
    {
        #region Fields

        private readonly int threadId;
        private readonly ManualResetEventSlim handleCreated;

        private bool isClosing;
        private bool isLoaded;
        private bool isRtlChanging;
        private Point location;

        #endregion

        #region Properties

        #region Protected Properties

        protected TViewModel ViewModel { get; } = default!;
        protected CommandBindingsCollection CommandBindings { get; } = new WinFormsCommandBindingsCollection();

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
            threadId = Thread.CurrentThread.ManagedThreadId;
            handleCreated = new ManualResetEventSlim();
            ApplyRightToLeft();
            InitializeComponent();

            // occurs in design mode but DesignMode is false for grandchild forms
            if (viewModel == null!)
                return;

            ViewModel = viewModel;
            ViewModelBase vm = VM;
            vm.ShowInfoCallback = Dialogs.InfoMessage;
            vm.ShowWarningCallback = Dialogs.WarningMessage;
            vm.ShowErrorCallback = Dialogs.ErrorMessage;
            vm.ConfirmCallback = Dialogs.ConfirmMessage;
            vm.CancellableConfirmCallback = (msg, btn) => Dialogs.CancellableConfirmMessage(msg, btn switch { 0 => MessageBoxDefaultButton.Button1, 1 => MessageBoxDefaultButton.Button2, _ => MessageBoxDefaultButton.Button3 });
            vm.ShowChildViewCallback = ShowChildView;
            vm.CloseViewCallback = () => BeginInvoke(new Action(Close));
            vm.SynchronizedInvokeCallback = InvokeIfRequired;
        }

        #endregion

        #region Private Constructors

        private MvvmBaseForm() : this(default!)
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

            // Null VM occurs in design mode but DesignMode is false for grandchild forms
            // Loaded can be true if handle was recreated
            if (isLoaded || ViewModel == null!)
            {
                if (!isRtlChanging)
                    return;

                // dialog has been reopened after changing RTL
                isRtlChanging = false;
                Location = location;
                return;
            }

            isLoaded = true;
            ApplyResources();
            ApplyViewModel();
        }

        protected virtual void ApplyResources() => ApplyStringResources();

        protected virtual void ApplyStringResources() => this.ApplyStringResources(toolTip);

        protected virtual void ApplyViewModel()
        {
            InitCommandBindings();
            VM.ViewLoaded();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            handleCreated.Set();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Changing RightToLeft causes the dialog close. We let it happen because the parent may also change,
            // and if we cancel the closing here, then a dialog may turn a non-modal form. Reopen as a dialog is handled in IView.ShowDialog
            if (isRtlChanging)
            {
                if (DialogResult == DialogResult.OK)
                    isRtlChanging = false;
                else
                    location = Location;
            }

            if (!e.Cancel)
                isClosing = true;
            base.OnFormClosing(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                CommandBindings.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void InitCommandBindings()
        {
            CommandBindings.Add(OnDisplayLanguageChangedCommand)
                .AddSource(typeof(LanguageSettings), nameof(LanguageSettings.DisplayLanguageChanged));
        }

        private void ShowChildView(IViewModel vm) => ViewFactory.ShowDialog(vm, this);

        private void InvokeIfRequired(Action action)
        {
            if (isClosing || Disposing || IsDisposed)
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

        private void ApplyRightToLeft()
        {
            RightToLeft rtl = LanguageSettings.DisplayLanguage.TextInfo.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
            if (RightToLeft == rtl)
                return;

            if (IsHandleCreated)
                isRtlChanging = true;

            RightToLeft = rtl;
        }

        #endregion

        #region Command Handlers

        private void OnDisplayLanguageChangedCommand()
        {
            ApplyRightToLeft();
            ApplyStringResources();
        }

        #endregion

        #region Explicit Interface Implementations

        void IView.ShowDialog(IntPtr ownerHandle)
        {
            do
            {
                ShowDialog(ownerHandle == IntPtr.Zero ? null : new OwnerWindowHandle(ownerHandle));
            } while (isRtlChanging);
        }

        void IView.ShowDialog(IView? owner)
        {
            do
            {
                ShowDialog(owner is IWin32Window window ? window : null);
            } while (isRtlChanging);
        }

        void IView.Show() => InvokeIfRequired(() =>
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
        });

        #endregion

        #endregion
    }
}