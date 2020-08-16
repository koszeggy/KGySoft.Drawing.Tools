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

using KGySoft.ComponentModel;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal partial class MvvmBaseUserControl<TViewModel> : BaseUserControl
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

        protected MvvmBaseUserControl(TViewModel viewModel)
        {
            if (DesignMode)
                return;
            ViewModel = viewModel;

            ViewModelBase vm = VM;
            vm.ShowInfoCallback = Dialogs.InfoMessage;
            vm.ShowWarningCallback = Dialogs.WarningMessage;
            vm.ShowErrorCallback = Dialogs.ErrorMessage;
            vm.ConfirmCallback = Dialogs.ConfirmMessage;

            CommandBindings = new WinformsCommandBindingsCollection();
        }

        #endregion

        #region Private Constructors

        private MvvmBaseUserControl()
        {
            // this ctor is just for the designer
        }

        #endregion

        #endregion

        #region Methods

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ApplyResources();
            if (DesignMode)
                return;
            ApplyViewModel();
        }

        protected virtual void ApplyResources()
        {
            // Not calling ApplyStaticStringResources because we assume this UC belong to an MvvmBaseForm.
            // If not, then a derived instance still can call it.
        }

        protected virtual void ApplyViewModel() => VM.ViewLoaded();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                CommandBindings?.Dispose();

            base.Dispose(disposing);
        }

        #endregion
    }
}
