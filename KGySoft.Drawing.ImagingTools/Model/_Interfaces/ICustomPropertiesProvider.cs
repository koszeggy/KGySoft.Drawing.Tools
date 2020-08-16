﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ICustomPropertiesProvider.cs
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

using System.ComponentModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    internal interface ICustomPropertiesProvider : ICustomTypeDescriptor
    {
        #region Methods

        object GetValue(CustomPropertyDescriptor property);
        void SetValue(CustomPropertyDescriptor property, object value);
        void ResetValue(CustomPropertyDescriptor property);

        #endregion
    }
}