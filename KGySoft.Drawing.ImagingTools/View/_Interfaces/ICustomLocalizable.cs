#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ICustomLocalizable.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2024 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System.Windows.Forms;

#endregion

namespace KGySoft.Drawing.ImagingTools.View
{
    internal interface ICustomLocalizable
    {
        #region Methods

        void ApplyStringResources(ToolTip? toolTip);

        #endregion
    }
}