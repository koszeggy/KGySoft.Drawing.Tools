﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: IDithererSettings.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2025 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    internal interface IDithererSettings
    {
        #region Properties

        float Strength { get; }
        int? Seed { get; }
        bool? ByBrightness { get; }
        bool DoSerpentineProcessing { get; }

        #endregion
    }
}