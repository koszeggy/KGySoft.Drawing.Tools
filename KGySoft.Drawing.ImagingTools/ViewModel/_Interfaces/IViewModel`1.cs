#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: IViewModel`1.cs
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

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    /// <summary>
    /// Represents a view model instance for an editable <typeparamref name="TModel"/> type.
    /// A new instance can be created by the <see cref="ViewModelFactory"/> class.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <seealso cref="IViewModel" />
    public interface IViewModel<TModel> : IViewModel
    {
        #region Events

        /// <summary>
        /// Occurs when the edited changes are explicitly applied by user action.
        /// Can be subscribed from a non-dialog debugger visualizer when the debugged <typeparamref name="TModel"/> insteance is replaceable.
        /// The changed model can be obtained by the <see cref="GetEditedModel"/> method.
        /// After this event is raised, <see cref="IViewModel.IsModified"/> returns <see langword="false"/>.
        /// </summary>
        event EventHandler ChangesApplied;

        #endregion

        #region Methods

        /// <summary>
        /// If <see cref="IViewModel.IsModified"/> returns <see langword="true"/>, then this method returns the possibly edited (changed) model.
        /// Can be used when the corresponding view has been closed after it was shown as a dialog,
        /// or whenever the <see cref="ChangesApplied"/> event is raised (e.g. from a non-dialog debugger visualizer).
        /// </summary>
        /// <returns>The edited model.</returns>
        TModel GetEditedModel();

        /// <summary>
        /// Tries to set the specified model for this view model by a potentially different instance.
        /// Can be used for example from a non-dialog debugger visualizer when the debugged <typeparamref name="TModel"/> is updated.
        /// </summary>
        /// <param name="model">The model instance to set.</param>
        /// <returns><see langword="true"/>, if this view model supports resetting the model, and it was successfully set; otherwise, <see langword="false"/>.</returns>
        bool TrySetModel(TModel model);

        #endregion
    }
}