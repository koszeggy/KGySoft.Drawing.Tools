#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ViewModelBase`1.cs
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

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal abstract class ViewModelBase<TModel> : ViewModelBase, IViewModel<TModel>
    {
        #region Fields

        private EventHandler? changesAppliedHandler;

        #endregion

        #region Events

        public event EventHandler? ChangesApplied
        {
            add => value.AddSafe(ref changesAppliedHandler);
            remove => value.RemoveSafe(ref changesAppliedHandler);
        }

        #endregion

        #region Properties

        #region Internal Properties

        internal bool ReadOnly { get => Get<bool>(); set => Set(value); }
        internal bool IsBusy { get => Get<bool>(); set => Set(value); }

        // Binding the Accept/Discard commands are not really needed if the corresponding buttons in the view set the DialogResult.
        // But it still can be useful as their command state may change if VM uses asynchronous operations that should not allow closing the view by the user.
        internal ICommand AcceptWithCloseCommand => Get(() => new SimpleCommand(OnAcceptWithCloseCommand));
        internal ICommand DiscardWithCloseCommand => Get(() => new SimpleCommand(OnDiscardWithCloseCommand));
        internal ICommand ApplyChangesCommand => Get(() => new SimpleCommand(OnApplyChangesCommand));

        internal ICommandState AcceptWithCloseCommandState => Get(() => new CommandState { Enabled = false });
        internal ICommandState DiscardWithCloseCommandState => Get(() => new CommandState());
        internal ICommandState ApplyChangesCommandCommandState => Get(() => new CommandState { Enabled = false });

        #endregion

        #region Protected Properties

        protected bool ClosedWithAccept { get; private set; }

        #endregion

        #endregion

        #region Methods

        #region Public Methods

        public abstract TModel GetEditedModel();

        public virtual bool TrySetModel(TModel model) => false;

        #endregion

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            switch (e.PropertyName)
            {
                case nameof(IsModified):
                    AcceptWithCloseCommandState.Enabled = ApplyChangesCommandCommandState.Enabled = e.NewValue is true && !IsBusy;
                    break;

                case nameof(IsBusy):
                    AcceptWithCloseCommandState.Enabled = ApplyChangesCommandCommandState.Enabled = IsModified && e.NewValue is false;
                    DiscardWithCloseCommandState.Enabled = e.NewValue is false;
                    break;
            }
        }

        protected void OnChangesApplied(EventArgs e) => changesAppliedHandler?.Invoke(this, e);

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;
            changesAppliedHandler = null;
            base.Dispose(disposing);
        }

        #endregion

        #region Command Handlers

        private void OnAcceptWithCloseCommand()
        {
            ClosedWithAccept = true;
            CloseViewCallback?.Invoke();
        }

        private void OnDiscardWithCloseCommand() => CloseViewCallback?.Invoke();

        private void OnApplyChangesCommand()
        {
            if (!IsModified)
                return;
            SetModified(false);
            OnChangesApplied(EventArgs.Empty);
        }

        #endregion

        #endregion
    }
}
