#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: MvvmBaseUserControl.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2025 - All Rights Reserved
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
using System.Linq;
using System.Windows.Forms;

using KGySoft.ComponentModel;
using KGySoft.Drawing.ImagingTools.View.Components;
using KGySoft.Drawing.ImagingTools.View.Forms;
using KGySoft.Drawing.ImagingTools.ViewModel;
using KGySoft.Drawing.ImagingTools.WinApi;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal partial class MvvmBaseUserControl : BaseUserControl, IView
    {
        #region Fields

        private AdvancedErrorProvider? warningProvider;
        private AdvancedErrorProvider? infoProvider;
        private AdvancedErrorProvider? errorProvider;
        private ICommand? validationResultsChangesCommand;

        private ViewModelBase? viewModel;
        private MvvmParentForm? mvvmParent;
        private bool isLoaded;

        #endregion

        #region Events

        internal event EventHandler? ViewModelChanged
        {
            add => Events.AddHandler(nameof(ViewModelChanged), value);
            remove => Events.RemoveHandler(nameof(ViewModelChanged), value);
        }

        #endregion

        #region Properties

        #region Internal Properties

        internal CommandBindingsCollection CommandBindings { get; } = new WinFormsCommandBindingsCollection();
        internal virtual ParentViewProperties? ParentViewProperties => null;
        internal virtual Action<MvvmParentForm>? ParentViewPropertyBindingsInitializer => null;
        internal virtual Action<MvvmParentForm>? ParentViewCommandBindingsInitializer => null;

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets or sets the view model. Can be null before initializing, if this user control is a child control.
        /// </summary>
        protected ViewModelBase? ViewModel
        {
            get => viewModel;
            set
            {
                if (ReferenceEquals(viewModel, value))
                    return;

                viewModel = value;
                OnViewModelChanged(EventArgs.Empty);
            }
        }

        protected bool IsLoaded => isLoaded;
        protected AdvancedErrorProvider ErrorProvider => errorProvider ??= CreateProvider(ValidationSeverity.Error);
        protected AdvancedErrorProvider WarningProvider => warningProvider ??= CreateProvider(ValidationSeverity.Warning);
        protected AdvancedErrorProvider InfoProvider => infoProvider ??= CreateProvider(ValidationSeverity.Information);

        protected Dictionary<string, Control> ValidationMapping { get; } = new Dictionary<string, Control>();
        protected ICommand ValidationResultsChangedCommand => validationResultsChangesCommand ??= new SimpleCommand<ValidationResultsCollection>(OnValidationResultsChangedCommand);

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// The constructor for creating an MvvmBaseUserControl as a child control. In this case, the <see cref="ViewModel"/> property should be set manually.
        /// </summary>
        protected MvvmBaseUserControl()
        {
            ApplyRightToLeft();
            InitializeComponent();

#if !NET35
            if (!OSUtils.IsWindows11OrLater)
#endif
            {
                toolTip.AutoPopDelay = Int16.MaxValue;
            }
        }

        /// <summary>
        /// The constructor for creating an MvvmBaseUserControl as a (semi) top-level control that may or may not be embedded into an MvvmParentForm.
        /// Modern debugger visualizer extensions may embed this control directly into a WPF control, without using the MvvmParentForm.
        /// </summary>
        protected MvvmBaseUserControl(ViewModelBase? viewModel) : this()
        {
            this.viewModel = viewModel;
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // isLoaded can be true if handle was recreated
            if (isLoaded)
                return;

            isLoaded = true;
            ApplyResources();

            // Null VM occurs in design mode
            if (viewModel != null)
                ApplyViewModel();
        }

        protected override void ApplyTheme()
        {
            base.ApplyTheme();
            toolTip.ResetAppearance();
            errorProvider?.ResetAppearance();
            warningProvider?.ResetAppearance();
            infoProvider?.ResetAppearance();
        }

        protected virtual void ApplyResources() => ApplyStringResources();

        protected virtual void ApplyStringResources() => this.ApplyStringResources(toolTip);

        protected virtual void ApplyViewModel()
        {
            if (viewModel == null)
                return;

            viewModel.ShowInfoCallback = Dialogs.InfoMessage;
            viewModel.ShowWarningCallback = Dialogs.WarningMessage;
            viewModel.ShowErrorCallback = Dialogs.ErrorMessage;
            viewModel.ConfirmCallback = Dialogs.ConfirmMessage;
            viewModel.CancellableConfirmCallback = (msg, btn) => Dialogs.CancellableConfirmMessage(msg, btn switch { 0 => MessageBoxDefaultButton.Button1, 1 => MessageBoxDefaultButton.Button2, _ => MessageBoxDefaultButton.Button3 });
            viewModel.ShowChildViewCallback = ShowChildView;
            viewModel.SynchronizedInvokeCallback = InvokeIfRequired;
            if (viewModel is ViewModelBase vm && mvvmParent is MvvmParentForm parent)
                vm.CloseViewCallback = () => BeginInvoke(new Action(parent.Close));

            InitPropertyBindings();
            InitCommandBindings();

            viewModel.ViewLoaded();
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            mvvmParent = ParentForm as MvvmParentForm;
            if (viewModel is ViewModelBase vm && mvvmParent is MvvmParentForm parent)
                vm.CloseViewCallback = () => BeginInvoke(new Action(parent.Close));
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
            {
                components?.Dispose();
                CommandBindings.Dispose();
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

        private AdvancedErrorProvider CreateProvider(ValidationSeverity level) => new AdvancedErrorProvider(components)
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

        private void ApplyRightToLeft()
        {
            RightToLeft rtl = Res.DisplayLanguage.TextInfo.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
            RightToLeft = rtl;
            toolTip?.ResetAppearance();
        }

        private void OnViewModelChanged(EventArgs e)
        {
            if (!isLoaded)
                return;

            CommandBindings.Clear();
            ApplyViewModel();

            Events.GetHandler<EventHandler>(nameof(ViewModelChanged))?.Invoke(this, e);
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

        private MvvmParentForm? TryGetCreateParent() => mvvmParent ??= ViewFactory.TryGetForm(this) as MvvmParentForm;

        #endregion

        #region Explicit Interface Implementations

        [SuppressMessage("CodeQuality", "IDE0002:Name can be simplified",
            Justification = "Without the base qualifier executing in Mono causes StackOverflowException. See https://github.com/mono/mono/issues/21129")]
        void IDisposable.Dispose()
        {
            InvokeIfRequired(mvvmParent is IDisposable parent ? parent.Dispose : base.Dispose);
        }

        void IView.ShowDialog(IntPtr ownerHandle)
        {
            MvvmParentForm? parent = TryGetCreateParent();
            if (parent == null)
            {
                ParentForm?.ShowDialog(ownerHandle == IntPtr.Zero ? null : new OwnerWindowHandle(ownerHandle));
                return;
            }

            do
            {
                parent.ShowDialog(ownerHandle == IntPtr.Zero ? null : new OwnerWindowHandle(ownerHandle));

                // the handle of the owner may change, too
                if (parent.IsRtlChanging && !User32.IsWindow(ownerHandle))
                    ownerHandle = IntPtr.Zero;
            } while (parent.IsRtlChanging);
        }

        void IView.ShowDialog(IView? owner)
        {
            MvvmParentForm? parent = TryGetCreateParent();
            if (parent == null)
            {
                ParentForm?.ShowDialog(owner as IWin32Window);
                return;
            }

            do
            {
               parent.ShowDialog(owner as IWin32Window);
            } while (parent.IsRtlChanging);
        }

        void IView.Show() => InvokeIfRequired(() =>
        {
            Form? parent = TryGetCreateParent() ?? ParentForm;
            if (parent == null)
                return;

            if (!Visible)
            {
                parent.Show();
                return;
            }

            if (parent.WindowState == FormWindowState.Minimized)
                parent.WindowState = FormWindowState.Normal;
            parent.Activate();
            parent.BringToFront();
        });

        #endregion

        #endregion
    }
}
