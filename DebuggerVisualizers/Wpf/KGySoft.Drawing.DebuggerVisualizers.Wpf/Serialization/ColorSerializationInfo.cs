#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorSerializationInfo.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2023 - All Rights Reserved
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
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Drawing.Wpf;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Wpf.Serialization
{
    internal sealed class ColorSerializationInfo
    {
        #region Nested Types

        private enum ColorType
        {
            Srgb,
            Linear,
            FromProfile
        }

        #endregion

        #region Fields

        private readonly Color color;

        #endregion

        #region Properties

        internal CustomColorInfo? ColorInfo { get; private set; }

        #endregion

        #region Constructors

        internal ColorSerializationInfo(Color color) => this.color = color;

        internal ColorSerializationInfo(BinaryReader reader) => ReadFrom(reader);

        #endregion

        #region Methods

        #region Internal Methods

        [SuppressMessage("ReSharper", "PossiblyImpureMethodCallOnReadonlyVariable", Justification = "Color.GetNativeColorValues is pure")]
        internal void Write(BinaryWriter bw)
        {
            string asString = color.ToString();
            ColorType type = color.ColorContext != null ? ColorType.FromProfile
                : asString.StartsWith("sc#", StringComparison.Ordinal) ? ColorType.Linear
                : ColorType.Srgb;

            // 1. Type
            bw.Write((byte)type);

            switch (type)
            {
                case ColorType.Srgb:
                    // 2. Name
                    bw.Write(asString);

                    // 3. Color components
                    bw.Write(color.A);
                    bw.Write(color.R);
                    bw.Write(color.G);
                    bw.Write(color.B);
                    break;

                case ColorType.Linear:
                    // 2. Name
                    bw.Write(asString);

                    // 3. Color components
                    bw.Write(color.ScA);
                    bw.Write(color.ScR);
                    bw.Write(color.ScG);
                    bw.Write(color.ScB);
                    break;

                case ColorType.FromProfile:
                    // 2. Name
                    float[] values = color.GetNativeColorValues()!;
                    bw.Write($"[{values.Select(f => $"{f:R}").Join("; ")}] ({Path.GetFileName(color.ColorContext!.ProfileUri.LocalPath)})");

                    // 3. Color components
                    // 3.a. as 32-bit ARGB for the display color
                    bw.Write(color.ToColor32().ToArgb());

                    // 3.b. the native components
                    bw.Write((byte)values.Length);
                    foreach (float value in values)
                        bw.Write(value);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Private Methods

        private void ReadFrom(BinaryReader br)
        {
            ColorInfo = new CustomColorInfo { Type = nameof(Color) };

            // 1. Type
            ColorType type = (ColorType)br.ReadByte();

            // 2. Name
            ColorInfo.Name = br.ReadString();

            // 3. Color components
            switch (type)
            {
                case ColorType.Srgb:
                    ColorInfo.DisplayColor = new Color32(br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte());
                    break;

                case ColorType.Linear:
                    ColorF asLinear = new ColorF(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    ColorInfo.DisplayColor = asLinear.ToColor32();
                    ColorInfo.CustomColorComponents = new KeyValuePair<string, string>[]
                    {
                        new(nameof(Color.ScA), $"{asLinear.A:F6}"),
                        new(nameof(Color.ScR), $"{asLinear.R:F6}"),
                        new(nameof(Color.ScG), $"{asLinear.G:F6}"),
                        new(nameof(Color.ScB), $"{asLinear.B:F6}")
                    };
                    break;

                case ColorType.FromProfile:
                    ColorInfo.DisplayColor = Color32.FromArgb(br.ReadInt32());
                    int channelCount = br.ReadByte();
                    ColorInfo.CustomColorComponents = new KeyValuePair<string, string>[channelCount];
                    for (int i = 0; i < channelCount; i++)
                        ColorInfo.CustomColorComponents[i] = new($"#{i}", $"{br.ReadSingle():F6}");

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #endregion
    }
}