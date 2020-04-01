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
            if (DesignMode)
                return;
            ViewModel = viewModel;

            var vm = VM;
            vm.ShowInfoCallback = Dialogs.InfoMessage;
            vm.ShowWarningCallback = Dialogs.WarningMessage;
            vm.ShowErrorCallback = Dialogs.ErrorMessage;
            vm.ConfirmCallback = Dialogs.ConfirmMessage;
            vm.ShowChildViewCallback = ShowChildView;

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
            ApplyResources();
            if (DesignMode)
                return;
            ApplyViewModel();
            ApplyViewModel(this, ViewModel);
        }

        protected virtual void ApplyResources() => this.ApplyStaticStringResources();

        protected virtual void ApplyViewModel() => VM.ViewCreated();

        protected void ShowChildView(IViewModel vm) => ViewFactory.ShowDialog(vm, this);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                CommandBindings?.Dispose();

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private static void ApplyViewModel(Control control, TViewModel viewModel)
        {
            foreach (Control child in control.Controls)
            {
                //if (child is MvvmBaseUserControl<TViewModel> userControl)
                //    userControl.ApplyViewModel(viewModel);
                //else
                    ApplyViewModel(child, viewModel);
            }
        }

        #endregion

        #region Explicit Interface Implementations

        void IView.ShowDialog(IWin32Window owner) => ShowDialog(owner);

        void IView.Show()
        {
            if (!Visible)
                Show();
            else
            {
                Activate();
                BringToFront();
            }
        }

        #endregion

        #endregion
    }
}