#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: IViewModel`1.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    /// <summary>
    /// Represents a view model instance for an editable <typeparamref name="TModel"/> type.
    /// A new instance can be created by the <see cref="ViewModelFactory"/> class.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <seealso cref="IViewModel" />
    public interface IViewModel<out TModel> : IViewModel
    {
        #region Methods

        /// <summary>
        /// If <see cref="IViewModel.IsModified"/> returns <see langword="true"/>, then this method returns the edited model.
        /// </summary>
        /// <returns>The edited model.</returns>
        TModel GetEditedModel();

        #endregion
    }
}