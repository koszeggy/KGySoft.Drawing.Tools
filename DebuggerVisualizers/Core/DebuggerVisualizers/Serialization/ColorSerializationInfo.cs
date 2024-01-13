﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorSerializationInfo.cs
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

using System.IO;

using KGySoft.Drawing.DebuggerVisualizers.Serialization;
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Core.Serialization
{
    internal sealed class ColorSerializationInfo : CustomColorSerializationInfo
    {
        #region Constructors

        internal ColorSerializationInfo(Color32 color) => ColorInfo = GetColorInfo(color, true);

        internal ColorSerializationInfo(BinaryReader reader)
            : base(reader)
        {
        }

        #endregion

        #region Methods

        internal static CustomColorInfo GetColorInfo(Color32 color, bool setType) => new CustomColorInfo
        {
            Type = setType ? nameof(Color32) : null,
            Name = color.ToString(),
            DisplayColor = color
        };

        #endregion
    }
}
