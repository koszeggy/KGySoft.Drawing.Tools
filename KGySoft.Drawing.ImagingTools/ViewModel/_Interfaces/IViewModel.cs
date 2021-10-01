#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: IViewModel.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
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
    /// Represents a view model instance.
    /// A new instance can be created by the <see cref="ViewModelFactory"/> class.
    /// </summary>
    public interface IViewModel : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets whether the model instance that belongs this view model instance is modified.
        /// </summary>
        bool IsModified { get; }

        #endregion
    }
}