#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ICustomPropertiesProvider.cs
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

using System.ComponentModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    internal interface ICustomPropertiesProvider : ICustomTypeDescriptor
    {
        #region Methods

        object? GetValue(string propertyName, object? defaultValue);
        void SetValue(string propertyName, object? value);
        void ResetValue(string propertyName);

        #endregion
    }
}