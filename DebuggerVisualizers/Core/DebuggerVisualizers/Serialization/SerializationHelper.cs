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
            if (target is not IReadableBitmapData bitmapData)
            {
                // This part is reached when debugging a .NET Framework project, and the target's assembly version is different from ours,
                // so casting it to IReadableBitmapData would fail. Here we create a clone of the target and serialize that instead.
                try
                {
                    // Actually we don't need the writer and reader here, just their BaseStream, which may be a file stream.
                    using var ms = new MemoryStream();
                    using (BinaryWriter writer = ms.InitSerializationWriter())
                    {
                        // Invoking BitmapDataExtensions.Save(bitmapData, stream) in the target's assembly
                        Type bitmapDataExtensions = Reflector.ResolveType(target.GetType().Assembly, typeof(BitmapDataExtensions).FullName!, ResolveTypeOptions.ThrowError)!;
                        Reflector.InvokeMethod(bitmapDataExtensions, nameof(BitmapDataExtensions.Save), target, writer.BaseStream);
                        writer.Flush();
                    }

                    ms.Position = 0;

                    // Deserializing the bitmap data with the correct assembly identity
                    using (BinaryReader reader = ms.InitSerializationReader())
                        bitmapData = BitmapDataFactory.Load(reader.BaseStream);
                }
                catch (Exception e)
                {
                    throw new ArgumentException(PublicResources.NotAnInstanceOfType(typeof(IReadableBitmapData)), nameof(bitmapData), e);
                }
            }

            using var imageInfo =  new ReadableBitmapDataSerializationInfo(bitmapData);
            using (BinaryWriter writer = outgoingData.InitSerializationWriter())
                imageInfo.Write(writer);

            if (!ReferenceEquals(target, bitmapData))
                bitmapData.Dispose();
        }

        internal static CustomBitmapInfo DeserializeCustomBitmapInfo(Stream stream)
        {
            using BinaryReader reader = stream.InitSerializationReader();
            return new ReadableBitmapDataSerializationInfo(reader).BitmapInfo!;
        }

        internal static void SerializeCustomPaletteInfo(IPalette palette, Stream outgoingData)
        {
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            new PaletteSerializationInfo(palette).Write(writer);
        }

        internal static CustomPaletteInfo DeserializeCustomPaletteInfo(Stream stream)
        {
            using BinaryReader reader = stream.InitSerializationReader();
            return new PaletteSerializationInfo(reader).PaletteInfo!;
        }

        internal static void SerializeCustomColorInfo(object color, Stream outgoingData)
        {
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            ColorSerializationInfo result;

            switch (color)
            {
                case Color32 c32:
                    result = new ColorSerializationInfo(c32);
                    break;
                case PColor32 pc32:
                    result = new ColorSerializationInfo(pc32);
                    break;
                case Color64 c64:
                    result = new ColorSerializationInfo(c64);
                    break;
                case PColor64 pc64:
                    result = new ColorSerializationInfo(pc64);
                    break;
                case ColorF cf:
                    result = new ColorSerializationInfo(cf);
                    break;
                case PColorF pcf:
                    result = new ColorSerializationInfo(pcf);
                    break;

                default:
                    // This point is reached only if the target assembly is different from ours.
                    try
                    {
                        // Serializing the color as a byte array and deserializing it with the correct assembly identity.
                        byte[] bytes = BinarySerializer.SerializeValueType((ValueType)color);
                        result = color.GetType().Name switch
                        {
                            nameof(Color32) => new ColorSerializationInfo(BinarySerializer.DeserializeValueType<Color32>(bytes)),
                            nameof(PColor32) => new ColorSerializationInfo(BinarySerializer.DeserializeValueType<PColor32>(bytes)),
                            nameof(Color64) => new ColorSerializationInfo(BinarySerializer.DeserializeValueType<Color64>(bytes)),
                            nameof(PColor64) => new ColorSerializationInfo(BinarySerializer.DeserializeValueType<PColor64>(bytes)),
                            nameof(ColorF) => new ColorSerializationInfo(BinarySerializer.DeserializeValueType<ColorF>(bytes)),
                            nameof(PColorF) => new ColorSerializationInfo(BinarySerializer.DeserializeValueType<PColorF>(bytes)),
                            _ => throw new ArgumentException(PublicResources.NotAnInstanceOfType(typeof(Color32)))
                        };
                    }
                    catch (Exception e) when (e is not ArgumentException)
                    {
                        throw new ArgumentException(PublicResources.NotAnInstanceOfType(typeof(Color32)), e);
                    }

                    break;

            }

            result.Write(writer);
        }

        internal static CustomColorInfo DeserializeCustomColorInfo(Stream stream)
        {
            using BinaryReader reader = stream.InitSerializationReader();
            return new ColorSerializationInfo(reader).ColorInfo!;
        }
    }
}
