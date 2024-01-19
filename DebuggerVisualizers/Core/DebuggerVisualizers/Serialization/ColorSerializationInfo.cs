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

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Core.Serialization
{
    internal sealed class ColorSerializationInfo : CustomColorSerializationInfo
    {
        #region Constructors

        internal ColorSerializationInfo(Color32 color) => ColorInfo = GetColorInfo(color, true);
        
        internal ColorSerializationInfo(PColor32 color) => ColorInfo = new CustomColorInfo
        {
            Type = nameof(PColor32),
            Name = color.ToString(),
            DisplayColor = color.ToColor32(),
            CustomColorComponents = new KeyValuePair<string, string>[]
            {
                new(nameof(PColor32.A), $"{color.A}"),
                new(nameof(PColor32.R), $"{color.R}"),
                new(nameof(PColor32.G), $"{color.G}"),
                new(nameof(PColor32.B), $"{color.B}"),
            }
        };
        
        internal ColorSerializationInfo(Color64 color) => ColorInfo = new CustomColorInfo
        {
            Type = nameof(Color64),
            Name = color.ToString(),
            DisplayColor = color.ToColor32(),
            CustomColorComponents = new KeyValuePair<string, string>[]
            {
                new(nameof(Color64.A), $"{color.A}"),
                new(nameof(Color64.R), $"{color.R}"),
                new(nameof(Color64.G), $"{color.G}"),
                new(nameof(Color64.B), $"{color.B}"),
            }
        };
        
        internal ColorSerializationInfo(PColor64 color) => ColorInfo = new CustomColorInfo
        {
            Type = nameof(PColor64),
            Name = color.ToString(),
            DisplayColor = color.ToColor32(),
            CustomColorComponents = new KeyValuePair<string, string>[]
            {
                new(nameof(PColor64.A), $"{color.A}"),
                new(nameof(PColor64.R), $"{color.R}"),
                new(nameof(PColor64.G), $"{color.G}"),
                new(nameof(PColor64.B), $"{color.B}"),
            }
        };

        internal ColorSerializationInfo(ColorF color) => ColorInfo = new CustomColorInfo
        {
            Type = nameof(ColorF),
            Name = color.ToString(),
            DisplayColor = color.ToColor32(),
            CustomColorComponents = new KeyValuePair<string, string>[]
            {
                new(nameof(ColorF.A), $"{color.A:F8}"),
                new(nameof(ColorF.R), $"{color.R:F8}"),
                new(nameof(ColorF.G), $"{color.G:F8}"),
                new(nameof(ColorF.B), $"{color.B:F8}"),
            }
        };

        internal ColorSerializationInfo(PColorF color) => ColorInfo = new CustomColorInfo
        {
            Type = nameof(PColorF),
            Name = color.ToString(),
            DisplayColor = color.ToColor32(),
            CustomColorComponents = new KeyValuePair<string, string>[]
            {
                new(nameof(PColorF.A), $"{color.A:F8}"),
                new(nameof(PColorF.R), $"{color.R:F8}"),
                new(nameof(PColorF.G), $"{color.G:F8}"),
                new(nameof(PColorF.B), $"{color.B:F8}"),
            }
        };

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
