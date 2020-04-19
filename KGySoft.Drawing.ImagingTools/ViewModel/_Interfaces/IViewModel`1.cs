#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: IViewModel`1.cs
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
        #region Properties

        /// <summary>
        /// Gets whether the model instance that belongs this view model instance is modified.
        /// </summary>
        bool IsModified { get; }

        #endregion

        #region Methods

        /// <summary>
        /// If <see cref="IsModified"/> returns <see langword="true"/>, then this method return the edited model.
        /// </summary>
        /// <returns>The edited model.</returns>
        TModel GetEditedModel();

        #endregion
    }
}