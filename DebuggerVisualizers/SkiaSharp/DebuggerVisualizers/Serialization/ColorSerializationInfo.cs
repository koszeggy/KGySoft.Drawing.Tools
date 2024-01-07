#region Copyright

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

using System.Collections.Generic;
using System.IO;

using KGySoft.Drawing.DebuggerVisualizers.Serialization;
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Drawing.SkiaSharp;

using SkiaSharp;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.SkiaSharp.Serialization
{
    internal sealed class ColorSerializationInfo : CustomColorSerializationInfoBase
    {
        #region Constructors

        internal ColorSerializationInfo(SKColor color) => ColorInfo = new CustomColorInfo
        {
            Type = nameof(SKColor),
            Name = color.ToString(),
            DisplayColor = color.ToColor32()
        };

        internal ColorSerializationInfo(SKPMColor color) => ColorInfo = new CustomColorInfo
        {
            Type = nameof(SKPMColor),
            Name = color.ToString(),
            DisplayColor = color.ToColor32(),
            CustomColorComponents = new KeyValuePair<string, string>[]
            {
                new(nameof(SKPMColor.Alpha), $"{color.Alpha}"),
                new(nameof(SKPMColor.Red), $"  {color.Red}"),
                new(nameof(SKPMColor.Green), $"{color.Green}"),
                new(nameof(SKPMColor.Blue), $" {color.Blue}"),
            }
        };

        internal ColorSerializationInfo(SKColorF color) => ColorInfo = new CustomColorInfo
        {
            Type = nameof(SKColorF),
            Name = color.ToString(),
            DisplayColor = color.ToColor32(),
            CustomColorComponents = new KeyValuePair<string, string>[]
            {
                new(nameof(SKColorF.Alpha), $"{color.Alpha:F6}"),
                new(nameof(SKColorF.Red), $"  {color.Red:F6}"),
                new(nameof(SKColorF.Green), $"{color.Green:F6}"),
                new(nameof(SKColorF.Blue), $" {color.Blue:F6}"),
            }
        };

        internal ColorSerializationInfo(BinaryReader reader)
            : base(reader)
        {
        }

        #endregion
    }
}
