#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ViewModelBase`1.cs
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

        internal ICommandState ApplyChangesCommandCommandState => Get(() => new CommandState { Enabled = false });
        internal ICommand ApplyChangesCommand => Get(() => new SimpleCommand(OnApplyChangesCommand));

        #endregion

        #region Methods

        #region Public Methods

        public abstract TModel GetEditedModel();

        public virtual bool TrySetModel(TModel model) => false;

        #endregion

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            if (e.PropertyName is nameof(IsModified))
                ApplyChangesCommandCommandState.Enabled = IsModified;

            base.OnPropertyChanged(e);
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

        #region Private Methods

        private void ApplyChanges()
        {
            if (!IsModified)
                return;

            SetModified(false);
            OnChangesApplied(EventArgs.Empty);
        }

        #endregion

        #region Command Handlers

        private void OnApplyChangesCommand() => ApplyChanges();

        #endregion

        #endregion
    }
}
