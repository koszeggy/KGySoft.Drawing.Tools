﻿#region Copyright

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
    public interface IViewModel<out TModel> : IViewModel
    {
        #region Properties

        bool IsModified { get; }

        #endregion

        #region Methods

        TModel GetEditedModel();

        #endregion
    }
}