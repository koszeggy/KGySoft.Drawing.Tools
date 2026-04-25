#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: MvvmBaseUserControl.cs
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using KGySoft.ComponentModel;
using KGySoft.Drawing.ImagingTools.View.Components;
using KGySoft.Drawing.ImagingTools.View.Forms;
using KGySoft.Drawing.ImagingTools.ViewModel;
using KGySoft.Drawing.ImagingTools.WinApi;
using KGySoft.WinForms;

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
        private PointF lastScale;

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
            lastScale = ScaleHelper.SystemScale;

#if !NET35
            if (!OSHelper.IsWindows11OrLater)
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

        #region Internal Methods

        /// <summary>
        /// Recommended to override for views with fixed size form border to avoid bad scaling on different platforms.
        /// </summary>
        internal virtual Size? GetDesiredSize(PointF scale) => null;

        /// <summary>
        /// This method is called before applying a new DPI. Use it to store the sizes of dynamically resizable controls if it is desirable to maintain their ratio.
        /// The new sizes should be applied in the <see cref="AdjustSizes"/> method.
        /// </summary>
        internal virtual void StoreDynamicSizes()
        {
        }

        /// <summary>
        /// This method is called the view is initialized in a host parent, or after applying a new DPI.
        /// If <paramref name="dynamicSizesScale"/> is not <see langword="null"/>, the provided scale can be applied to dynamically resizable controls,
        /// whose sizes could be stored in a previous call of the <see cref="StoreDynamicSizes"/> method.
        /// </summary>
        /// <param name="dynamicSizesScale">The scale factor to apply to the dynamically resizable controls. It is <see langword="null"/>,
        /// if this method is called on initializing the host view for the first time or in RTL layout change. If it has a value, the method is called due to a DPI change,
        /// in which case the <see cref="StoreDynamicSizes"/> method had been also called previously.</param>
        internal void AdjustSizes(PointF? dynamicSizesScale)
        {
            SuspendLayout();
            try
            {
                if (ParentViewProperties?.MinimumSize is Size { IsEmpty: false } size && mvvmParent is MvvmParentForm parent)
                    parent.MinimumSize = size.Scale(parent.GetScale());
                ApplySizeAdjustments(dynamicSizesScale);
                if (dynamicSizesScale == null)
                    return;
                UpdateProviderIcon(errorProvider, ValidationSeverity.Error);
                UpdateProviderIcon(warningProvider, ValidationSeverity.Warning);
                UpdateProviderIcon(infoProvider, ValidationSeverity.Information);
            }
            finally
            {
                ResumeLayout();
            }
        }

        internal void OnHostShown() => viewModel?.ViewShown();

        #endregion

        #region Protected Methods

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            lastScale = this.GetScale();
            if (Parent is not Form)
                AdjustFont();
        }

        protected override void OnLoad(EventArgs e)
        {
            // isLoaded can be true if handle was recreated
            bool isLoaded = IsLoaded;
            base.OnLoad(e);
            if (isLoaded)
                return;

            // Null VM occurs in design mode
            if (viewModel != null)
                ApplyViewModel();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Debug.WriteLine($"Size: {Size}");
        }

        protected override void ApplyTheme()
        {
            base.ApplyTheme();
            toolTip.ResetAppearance();
            errorProvider?.ResetAppearance();
            warningProvider?.ResetAppearance();
            infoProvider?.ResetAppearance();
        }

        protected override void ApplyStringResources() => LocalizationHelper.ApplyStringResources(this);

        protected virtual void ApplyViewModel()
        {
            if (viewModel == null)
                return;

            viewModel.ShowInfoCallback = Dialogs.InfoMessage;
            viewModel.ShowWarningCallback = Dialogs.WarningMessage;
            viewModel.ShowErrorCallback = Dialogs.ErrorMessage;
            viewModel.ConfirmCallback = Dialogs.ConfirmMessage;
            viewModel.CancellableConfirmCallback = Dialogs.CancellableConfirmMessage;
            viewModel.ShowChildViewCallback = ShowChildView;
            viewModel.SynchronizedInvokeCallback = InvokeOnUIThread;

            // Using BeginInvoke instead of InvokeOnUIThread is intended. It defers the invoke after other UI events, preventing possible infinite recursion
            // when ClosingCallback calls Close again.
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

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case Constants.WM_DPICHANGED_BEFOREPARENT:
                    StoreDynamicSizes();
                    base.WndProc(ref m);

                    // embedded into a WPF host (visualizer extension): adjusting the font
                    if (Parent is not Form)
                        AdjustFont();

                    return;

                case Constants.WM_DPICHANGED_AFTERPARENT:
                    base.WndProc(ref m);
                    PointF newScale = this.GetScale();
                    AdjustSizes(new PointF(newScale.X / lastScale.X, newScale.Y / lastScale.Y));
                    lastScale = newScale;
                    return;

                default:
                    base.WndProc(ref m);
                    return;
            }
        }

        /// <summary>
        /// Override this method to apply size adjustments after the view is initialized in a host parent, or after applying a new DPI.
        /// The layout is suspended when executing this method. If <paramref name="dynamicSizesScale"/> is not <see langword="null"/>,
        /// the provided scale can be applied to dynamically resizable controls, whose sizes could be stored in a previous call of the <see cref="StoreDynamicSizes"/> method.
        /// </summary>
        /// <param name="dynamicSizesScale">The scale factor to apply to the dynamically resizable controls. It is <see langword="null"/>,
        /// if this method is called on initializing the host view for the first time or in RTL layout change. If it has a value, the method is called due to a DPI change,
        /// in which case the <see cref="StoreDynamicSizes"/> method had been also called previously.</param>
        protected virtual void ApplySizeAdjustments(PointF? dynamicSizesScale)
        {
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
                _ => Icons.SystemInformation.ToScaledIcon(this.GetScale()),
            }
        };

        private void UpdateProviderIcon(AdvancedErrorProvider? provider, ValidationSeverity level)
        {
            if (provider == null)
                return;

            var prevIcon = provider.Icon;
            provider.Icon = level switch
            {
                ValidationSeverity.Error => Icons.SystemError.ToScaledIcon(this.GetScale()),
                ValidationSeverity.Warning => Icons.SystemWarning.ToScaledIcon(this.GetScale()),
                _ => Icons.SystemInformation.ToScaledIcon(this.GetScale()),
            };
            prevIcon.Dispose();
        }

        private void ShowChildView(IViewModel vm) => ViewFactory.ShowDialog(vm, this);

        private void ApplyRightToLeft()
        {
            RightToLeft rtl = Res.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
            try
            {
                RightToLeft = rtl;
            }
            catch (ArgumentException)
            {
                // Preventing sporadic argument exception on some platforms
            }

            toolTip?.ResetAppearance();
        }

        private void OnViewModelChanged(EventArgs e)
        {
            if (!IsLoaded)
                return;

            CommandBindings.Clear();
            ApplyViewModel();

            Events.GetHandler<EventHandler>(nameof(ViewModelChanged))?.Invoke(this, e);
        }

        private MvvmParentForm? TryGetCreateParent() => mvvmParent ??= ViewFactory.TryGetForm(this) as MvvmParentForm;

        private void AdjustFont()
        {
            Debug.Assert(Parent is not BaseForm, "Adjusting font in needed when the view in embedded into a WPF host");
            Font? font = SystemFonts.MessageBoxFont;
            if (font == null)
                return;

            PointF systemScale = ScaleHelper.SystemScale;
            PointF scale = this.GetScale();
            if (scale == systemScale)
            {
                Font = font;
                return;
            }

            float ratio = scale.Y / systemScale.Y;
            Font = new Font(font.FontFamily, font.SizeInPoints * ratio, font.Style, GraphicsUnit.Point, font.GdiCharSet, font.GdiVerticalFont);
            font.Dispose();
        }


        #endregion

        #region Command Handlers

        private void OnDisplayLanguageChangedCommand() => InvokeOnUIThread(() =>
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
            InvokeOnUIThread(mvvmParent is IDisposable parent ? parent.Dispose : base.Dispose);
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

        void IView.Show() => InvokeOnUIThread(() =>
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
