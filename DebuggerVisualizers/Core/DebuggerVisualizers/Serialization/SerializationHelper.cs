#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: SerializationHelper.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2025 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Collections;
using System.IO;

using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Reflection;
using KGySoft.Serialization.Binary;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Core.Serialization
{
    internal static class SerializationHelper
    {
        internal static void SerializeCustomBitmapInfo(object target, Stream outgoingData)
        {
            using var imageInfo = new ReadableBitmapDataSerializationInfo(target);
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            imageInfo.Write(writer);
        }

        internal static CustomBitmapInfo DeserializeCustomBitmapInfo(Stream stream)
        {
            using BinaryReader reader = stream.InitSerializationReader();
            return new ReadableBitmapDataSerializationInfo(reader).BitmapInfo!;
        }

        internal static void SerializeCustomPaletteInfo(object target, Stream outgoingData)
        {
            if (target is not Palette palette)
            {
                // This part is reached when the debugged project uses a different version of KGySoft.Drawing.Core
                // so casting it to Palette would fail. Here we create a clone of the target and serialize that instead.
                try
                {
                    IList origEntries = (IList)Reflector.InvokeMethod(target, nameof(Palette.GetEntries))!;
                    var newEntries = new Color32[origEntries.Count];
                    for (int i = 0; i < origEntries.Count; i++)
                        newEntries[i] = Color32.FromArgb((int)Reflector.InvokeMethod(origEntries[i]!, nameof(Color32.ToArgb))!);
                    palette = new Palette(newEntries);
                }
                catch (Exception e)
                {
                    throw new ArgumentException(PublicResources.NotAnInstanceOfType(typeof(Palette)), nameof(target), e);
                }
            }

            var info = new PaletteSerializationInfo(palette);
#if DEBUG
            if (!ReferenceEquals(target, palette))
                info.PaletteInfo!.CustomAttributes["KGySoft.Drawing.Core version mismatch"] = $"{target.GetType().Assembly} vs. {typeof(Palette).Assembly}";
#endif
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            info.Write(writer);
        }

        internal static CustomPaletteInfo DeserializeCustomPaletteInfo(Stream stream)
        {
            using BinaryReader reader = stream.InitSerializationReader();
            return new PaletteSerializationInfo(reader).PaletteInfo!;
        }

        internal static void SerializeCustomColorInfo(object target, Stream outgoingData)
        {
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            ColorSerializationInfo info;

            switch (target)
            {
                case Color32 c32:
                    info = new ColorSerializationInfo(c32);
                    break;
                case PColor32 pc32:
                    info = new ColorSerializationInfo(pc32);
                    break;
                case Color64 c64:
                    info = new ColorSerializationInfo(c64);
                    break;
                case PColor64 pc64:
                    info = new ColorSerializationInfo(pc64);
                    break;
                case ColorF cf:
                    info = new ColorSerializationInfo(cf);
                    break;
                case PColorF pcf:
                    info = new ColorSerializationInfo(pcf);
                    break;

                default:
                    // This point is reached only if the target assembly is different from ours.
                    try
                    {
                        // Serializing the color as a byte array and deserializing it with the correct assembly identity.
                        byte[]? bytes = target is ValueType colorValue && colorValue.GetType().Namespace == typeof(Color32).Namespace
                            ? BinarySerializer.SerializeValueType(colorValue)
                            : null;

                        if (bytes == null)
                            throw new ArgumentException(PublicResources.NotAnInstanceOfType(typeof(Color32)));

                        info = target.GetType().Name switch
                        {
                            nameof(Color32) => new ColorSerializationInfo(BinarySerializer.DeserializeValueType<Color32>(bytes)),
                            nameof(PColor32) => new ColorSerializationInfo(BinarySerializer.DeserializeValueType<PColor32>(bytes)),
                            nameof(Color64) => new ColorSerializationInfo(BinarySerializer.DeserializeValueType<Color64>(bytes)),
                            nameof(PColor64) => new ColorSerializationInfo(BinarySerializer.DeserializeValueType<PColor64>(bytes)),
                            nameof(ColorF) => new ColorSerializationInfo(BinarySerializer.DeserializeValueType<ColorF>(bytes)),
                            nameof(PColorF) => new ColorSerializationInfo(BinarySerializer.DeserializeValueType<PColorF>(bytes)),
                            _ => throw new ArgumentException(PublicResources.NotAnInstanceOfType(typeof(Color32)))
                        };

#if DEBUG
                        info.ColorInfo!.CustomAttributes["KGySoft.Drawing.Core version mismatch"] = $"{target.GetType().Assembly} vs. {typeof(Color32).Assembly}";
#endif

                    }
                    catch (Exception e) when (e is not ArgumentException)
                    {
                        throw new ArgumentException(PublicResources.NotAnInstanceOfType(typeof(Color32)), nameof(target), e);
                    }

                    break;
            }

            info.Write(writer);
        }

        internal static CustomColorInfo DeserializeCustomColorInfo(Stream stream)
        {
            using BinaryReader reader = stream.InitSerializationReader();
            return new ColorSerializationInfo(reader).ColorInfo!;
        }
    }
}
