#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: MvvmBaseForm.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
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

        private ErrorProvider? warningProvider;
        private ErrorProvider? infoProvider;
        private ErrorProvider? errorProvider;
        private ICommand? validationResultsChangesCommand;

        private bool isLoaded;
        private bool isRtlChanging;
        private Point location;

        #endregion

        #region Properties

        #region Protected Properties

        protected TViewModel ViewModel { get; } = default!;
        protected CommandBindingsCollection CommandBindings { get; } = new WinFormsCommandBindingsCollection();

        protected ErrorProvider ErrorProvider => errorProvider ??= CreateProvider(ValidationSeverity.Error);
        protected ErrorProvider WarningProvider => warningProvider ??= CreateProvider(ValidationSeverity.Warning);
        protected ErrorProvider InfoProvider => infoProvider ??= CreateProvider(ValidationSeverity.Information);

        protected Dictionary<string, Control> ValidationMapping { get; } = new Dictionary<string, Control>();
        protected ICommand ValidationResultsChangedCommand => validationResultsChangesCommand ??= new SimpleCommand<ValidationResultsCollection>(OnValidationResultsChangedCommand);

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
            StartPosition = OSUtils.IsMono && OSUtils.IsWindows ? FormStartPosition.WindowsDefaultLocation : FormStartPosition.CenterParent;


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
            // Not Using tool window appearance on Linux because looks bad an on high DPI the close is too small
            if (OSUtils.IsMono && OSUtils.IsLinux && FormBorderStyle == FormBorderStyle.SizableToolWindow)
            {
                FormBorderStyle = FormBorderStyle.Sizable;
                MinimizeBox = false;
            }

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
            InitPropertyBindings();
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

            base.OnFormClosing(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                CommandBindings.Dispose();
                handleCreated.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void InitPropertyBindings()
        {
            if (ValidationMapping.Count != 0)
            {
                // this.RightToLeft -> errorProvider/warningProvider/infoProvider.RightToLeft
                CommandBindings.AddPropertyBinding(this, nameof(RightToLeft), nameof(ErrorProvider.RightToLeft),
                    rtl => rtl is RightToLeft.Yes, ErrorProvider, WarningProvider, InfoProvider);
            }
        }

        private void InitCommandBindings()
        {
            CommandBindings.Add(OnDisplayLanguageChangedCommand)
                .AddSource(typeof(Res), nameof(Res.DisplayLanguageChanged));
        }

        private ErrorProvider CreateProvider(ValidationSeverity level) => new ErrorProvider(components)
        {
            ContainerControl = this,
            Icon = level switch
            {
                ValidationSeverity.Error => Icons.SystemError.ToScaledIcon(this.GetScale()),
                ValidationSeverity.Warning => Icons.SystemWarning.ToScaledIcon(this.GetScale()),
                ValidationSeverity.Information => Icons.SystemInformation.ToScaledIcon(this.GetScale()),
                _ => null
            }
        };

        private void ShowChildView(IViewModel vm) => ViewFactory.ShowDialog(vm, this);

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

        private void ApplyRightToLeft()
        {
            RightToLeft rtl = Res.DisplayLanguage.TextInfo.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
            if (RightToLeft == rtl)
                return;

            if (!OSUtils.IsMono && IsHandleCreated)
                isRtlChanging = true;

            RightToLeft = rtl;
        }

        #endregion

        #region Command Handlers

        private void OnDisplayLanguageChangedCommand() => InvokeIfRequired(() =>
        {
            ApplyRightToLeft();
            ApplyStringResources();
        });

        private void OnValidationResultsChangedCommand(ValidationResultsCollection? validationResults)
        {
            foreach (KeyValuePair<string, Control> mapping in ValidationMapping)
            {
                ValidationResultsCollection? propertyResults = validationResults?[mapping.Key];
                ValidationResult? error = propertyResults?.Errors.FirstOrDefault();
                ValidationResult? warning = error == null ? propertyResults?.Warnings.FirstOrDefault() : null;
                ValidationResult? info = error == null && warning == null ? propertyResults?.Infos.FirstOrDefault() : null;
                ErrorProvider.SetError(mapping.Value, error?.Message);
                WarningProvider.SetError(mapping.Value, warning?.Message);
                InfoProvider.SetError(mapping.Value, info?.Message);
            }
        }

        #endregion

        #region Explicit Interface Implementations

        [SuppressMessage("CodeQuality", "IDE0002:Name can be simplified",
            Justification = "Without the base qualifier executing in Mono causes StackOverflowException. See https://github.com/mono/mono/issues/21129")]
        void IDisposable.Dispose()
        {
            isRtlChanging = false;
            InvokeIfRequired(base.Dispose);
        }

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