#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ResourceLibraries.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
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
    [Flags]
    internal enum ResourceLibraries
    {
        None,
        CoreLibraries = 1,
        DrawingLibraries = 1 << 1,
        ImagingTools = 1 << 2
    }
}