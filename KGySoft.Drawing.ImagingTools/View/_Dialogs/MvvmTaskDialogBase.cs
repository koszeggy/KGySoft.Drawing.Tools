#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: MvvmTaskDialogBase.cs
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

using System.Drawing;

#region Used Namespaces

using System;
using System.Threading;
using System.Windows.Forms;

using KGySoft.ComponentModel;
using KGySoft.Drawing.ImagingTools.ViewModel;
using KGySoft.Drawing.ImagingTools.WinApi;
using KGySoft.WinForms.Components;

#endregion

#region Used Aliases

using TaskDialog = KGySoft.WinForms.Components.TaskDialog;

#endregion

#endregion

namespace KGySoft.Drawing.ImagingTools.View
{
    internal abstract class MvvmTaskDialogBase : IView, IWin32Window
    {
        #region Fields

        private readonly SynchronizationContext context = SynchronizationContext.Current!;
        private readonly int threadId = ThreadHelper.ManagedThreadId;

        private bool isDisposed;

        #endregion

        #region Properties

        #region Public Properties

        public bool IsDisposed => isDisposed;
        public IntPtr Handle => TaskDialog.Handle;

        #endregion

        #region Protected Properties

        protected TaskDialog TaskDialog { get; }
        protected ViewModelBase ViewModel { get; }
        protected WinFormsCommandBindingsCollection CommandBindings { get; } = new();

        #endregion

        #endregion

        #region Constructors

        protected MvvmTaskDialogBase(ViewModelBase viewModel)
        {
            ViewModel = viewModel;
            TaskDialog = new TaskDialog
            {
                ForceCompatibilityMode = true, // for theme changes and the extra features
                Options = TaskDialogOptions.AllowCancel | TaskDialogOptions.ForceShowSysMenu
            };

            // not in InitCommandBindings, because without this we cannot init the other commands
            TaskDialog.Created += TaskDialog_Created;
            ApplyStringResources();
        }

        #endregion

        #region Methods

        #region Public Methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void ShowDialog(IntPtr ownerWindowHandle = default)
        {
            if (ownerWindowHandle == IntPtr.Zero)
                ownerWindowHandle = User32.GetActiveWindow();
            Execute(ownerWindowHandle == IntPtr.Zero ? null : new OwnerWindowHandle(ownerWindowHandle));
        }

        public void ShowDialog(IView? owner)
        {
            if (owner is IWin32Window ownerWindow)
            {
                // Trying to obtain the form of the owner if owner is just a control.
                // This provides better dialog handling, such bringing the form on top or blinking the form when the owner is clicked.
                Control? ownerControl = owner as Control;
                if (ownerControl != null)
                    ownerControl = ownerControl as Form ?? ownerControl.FindForm() ?? ownerControl;
                Execute(ownerControl ?? ownerWindow);
            }
            else
                ShowDialog();
        }

        public void Show() => Execute(null);

        #endregion

        #region Protected Methods

        protected virtual void ApplyViewModel()
        {
            ViewModel.ShowInfoCallback = Dialogs.InfoMessage;
            ViewModel.ShowWarningCallback = Dialogs.WarningMessage;
            ViewModel.ShowErrorCallback = Dialogs.ErrorMessage;
            ViewModel.ConfirmCallback = Dialogs.ConfirmMessage;
            ViewModel.CancellableConfirmCallback = Dialogs.CancellableConfirmMessage;
            ViewModel.ShowChildViewCallback = ShowChildView;
            ViewModel.SynchronizedInvokeCallback = InvokeOnUIThread;
            ViewModel.CloseViewCallback = () => InvokeOnUIThread(TaskDialog.Close);

            InitCommandBindings();

            ViewModel.ViewLoaded();
        }

        protected virtual void ApplyStringResources()
        {
        }

        protected void ApplyTheme()
        {
            // NOTE: this executes only on Dark<->Light/HighContrast theme change, and not when switching from light to high contrast theme, for example.
            if (TaskDialog.Handle == IntPtr.Zero || IsDisposed || !ThemeColors.IsBaseThemeEverChanged)
                return;

            Control? form = Control.FromHandle(TaskDialog.Handle);
            if (form == null)
                return;

            // Form header, parent colors, theme for children.
            // Should be called also when switching to themed high contrast theme, for example, so the dark UxTheme is removed from the child controls
            form.ApplyTheme();

            if (ThemeColors.IsDarkBaseTheme)
            {
                // These controls have explicitly set colors that we need to override. As TaskDialogForm subscribes earlier to theme changes
                // (from its constructor), we can be sure that these execute after TaskDialogForm applied the theme.
                // TODO: other explicitly set colors by TaskDialogForm for pnlDividerControlsBottom, pnlDividerFooterTop, pnlDividerFooterBottom, pnlDividerDetailsFooterTop, lblMainInstruction, pnlCommandLinks
                form.Controls["pnlDividerMainBottom"]?.BackColor = ThemeColors.TaskDialogDivider;
                Control? pnlMain = form.Controls["pnlMain"];
                Debug.Assert(pnlMain != null);
                if (pnlMain != null)
                {
                    pnlMain.BackColor = ThemeColors.Window;
                    pnlMain.ForeColor = ThemeColors.WindowText;
                    pnlMain.Controls["pnlMainIcon"]?.Controls["pnlMainIconBackground"]?.BackColor = ThemeColors.Window;
                }

                return;
            }

            // If the current theme is not dark, we must reset the colors that are not set by TaskDialogForm, so further visual style changes without theme change will work properly.
            form.BackColor = form.ForeColor = Color.Empty;
            // TODO: other explicitly set colors by ControlExtensions.ApplyTheme for AdvancedButton (disabled color), AdvancedRadioButton (disabled color), AdvancedProgressBar
        }

        protected void InvokeOnUIThread(Action action)
        {
            if (IsDisposed)
                return;

            try
            {
                // no invoke is required (not using owner.InvokeRequired because that may return false if handle is not created yet)
                if (threadId == ThreadHelper.ManagedThreadId)
                {
                    action.Invoke();
                    return;
                }

                // invoking from a foreign thread
                context.Send(_ => action.Invoke(), null);
            }
            catch (ObjectDisposedException)
            {
                // it can happen that IsDisposed returned false, but actual Send is started to execute only after disposing has started
            }
        }

        protected virtual void OnClosed(TaskDialogResult result)
        {
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
                return;
            isDisposed = true;
            TaskDialog.Created -= TaskDialog_Created; // though disposing the TaskDialog removes the subscriptions
            if (disposing)
            {
                TaskDialog.Dispose();
                CommandBindings.Dispose();
            }
        }

        #endregion

        #region Private Methods

        private void Execute(IWin32Window? owner)
        {
            // If the handle is null here, the dialog is shown as a non-modal window. The call is still blocking until the dialog is closed.
            TaskDialogResult result = TaskDialog.Show(owner);
            ViewModel.ViewUnloading();
            OnClosed(result);
        }

        private void InitCommandBindings()
        {
            CommandBindings.Add(OnThemeChangedCommand)
                .AddSource(typeof(ThemeColors), nameof(ThemeColors.ThemeChanged));
            CommandBindings.Add(OnDisplayLanguageChangedCommand)
                .AddSource(typeof(Res), nameof(Res.DisplayLanguageChanged));
        }

        private void ApplyRightToLeft()
        {
            if (IsDisposed)
                return;
            if (Res.IsRightToLeft)
                TaskDialog.Options |= TaskDialogOptions.RightToLeftLayout;
            else
                TaskDialog.Options &= ~TaskDialogOptions.RightToLeftLayout;
        }

        private void ShowChildView(IViewModel vm) => ViewFactory.ShowDialog(vm, this);

        #endregion

        #region Command Handlers

        private void OnThemeChangedCommand() => InvokeOnUIThread(ApplyTheme);

        private void OnDisplayLanguageChangedCommand() => InvokeOnUIThread(() =>
        {
            ApplyRightToLeft();
            ApplyStringResources();
        });

        #endregion

        #region Event Handlers

        private void TaskDialog_Created(object? sender, EventArgs e)
        {
            ApplyTheme();
            ApplyViewModel();
            ViewModel.ViewShown(); // though actually it's not shown yet
        }


        #endregion

        #endregion
    }
}
