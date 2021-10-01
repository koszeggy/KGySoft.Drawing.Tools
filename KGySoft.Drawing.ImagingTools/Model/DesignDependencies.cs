#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DesignDependencies.cs
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

namespace KGySoft.Drawing.ImagingTools.Model
{
    internal static class DesignDependencies
    {
        #region Properties

        internal static Type? QuantizerThresholdEditor { get; set; }
        internal static Type? DithererStrengthEditor { get; set; }

        #endregion
    }
}