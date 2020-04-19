#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: IViewModel.cs
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

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    /// <summary>
    /// Represents a view model instance.
    /// A new instance can be created by the <see cref="ViewModelFactory"/> class.
    /// </summary>
    public interface IViewModel : IDisposable
    {
    }
}