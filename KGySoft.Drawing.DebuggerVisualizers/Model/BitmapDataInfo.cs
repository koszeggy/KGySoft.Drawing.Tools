#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: BitmapDataInfo.cs
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

using System.Drawing;

using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Model
{
    internal sealed class BitmapDataInfo
    {

        #region Properties

        internal ImageData Data { get; set; }
        internal string SpecialInfo { get; set; }

        #endregion
    }
}