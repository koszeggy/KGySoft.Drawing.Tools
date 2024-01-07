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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Windows.Media;

using KGySoft.CoreLibraries;
using KGySoft.Drawing.DebuggerVisualizers.Serialization;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Drawing.Wpf;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Wpf.Serialization
{
    internal sealed class ColorSerializationInfo : CustomColorSerializationInfoBase
    {
        #region Constructors

        [SuppressMessage("ReSharper", "PossiblyImpureMethodCallOnReadonlyVariable", Justification = "Color.GetNativeColorValues is pure")]
        internal ColorSerializationInfo(Color color)
        {
            ColorInfo = new CustomColorInfo
            {
                Type = nameof(Color),
                DisplayColor = color.ToColor32()
            };

            // Color from profile
            if (color.ColorContext != null)
            {
                float[] values = color.GetNativeColorValues()!;
                ColorInfo.Name = $"[{values.Select(f => $"{f:R}").Join("; ")}] ({Path.GetFileName(color.ColorContext!.ProfileUri.LocalPath)})";

                ColorInfo.CustomColorComponents = new KeyValuePair<string, string>[values.Length];
                for (int i = 0; i < values.Length; i++)
                    ColorInfo.CustomColorComponents[i] = new($"#{i}", $"{values[i]:F6}");
                
                return;
            }

            ColorInfo.Name = color.ToString();

            // sRGB color
            if (!ColorInfo.Name.StartsWith("sc#", StringComparison.Ordinal))
                return;

            // Linear color
            ColorInfo.CustomColorComponents = new KeyValuePair<string, string>[]
            {
                new(nameof(color.ScA), $"{color.ScA:F6}"),
                new(nameof(color.ScR), $"{color.ScR:F6}"),
                new(nameof(color.ScG), $"{color.ScG:F6}"),
                new(nameof(color.ScB), $"{color.ScB:F6}"),
            };
        }

        internal ColorSerializationInfo(BinaryReader reader)
            : base(reader)
        {
        }

        #endregion
    }
}