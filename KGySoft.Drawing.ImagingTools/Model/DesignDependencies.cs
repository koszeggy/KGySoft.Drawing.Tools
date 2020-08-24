#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DesignDependencies.cs
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

namespace KGySoft.Drawing.ImagingTools.Model
{
    internal static class DesignDependencies
    {
        #region Properties

        internal static Type QuantizerThresholdEditor { get; set; }
        internal static Type DithererStrengthEditor { get; set; }

        #endregion
    }
}