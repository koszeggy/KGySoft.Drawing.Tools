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

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Core.Serialization
{
    internal static class SerializationHelper
    {
        internal static void SerializeCustomBitmapInfo(IReadableBitmapData bitmapData, Stream outgoingData)
        {
            using var imageInfo =  new ReadableBitmapDataSerializationInfo(bitmapData);
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            imageInfo.Write(writer);
        }

        internal static void SerializeCustomBitmapInfoSafe(object bitmapData, Stream outgoingData)
        {
            // This method is called when the target is an IReadableBitmapData, but its assembly version is different from ours,
            // so casting it to IReadableBitmapData would fail. Here we create a clone of the target and serialize that instead.
            IReadableBitmapData clone;

            try
            {
                using var ms = new MemoryStream();
                using (BinaryWriter writer = ms.InitSerializationWriter())
                {
                    // Invoking BitmapDataExtensions.Save(bitmapData, stream) in the target's assembly
                    Type bitmapDataExtensions = Reflector.ResolveType(bitmapData.GetType().Assembly, typeof(BitmapDataExtensions).FullName!, ResolveTypeOptions.ThrowError)!;
                    Reflector.InvokeMethod(bitmapDataExtensions, nameof(BitmapDataExtensions.Save), bitmapData, writer.BaseStream);
                    writer.Flush();
                }

                ms.Position = 0;

                // Deserializing the bitmap data with the correct assembly identity, 
                using (BinaryReader reader = ms.InitSerializationReader())
                    clone = BitmapDataFactory.Load(reader.BaseStream);
            }
            catch (Exception e)
            {
                throw new ArgumentException(PublicResources.NotAnInstanceOfType(typeof(IReadableBitmapData)), nameof(bitmapData), e);
            }

            using (clone)
                SerializeCustomBitmapInfo(clone, outgoingData);
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

        internal static void SerializeCustomColorInfo(Color32 color, Stream outgoingData)
        {
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            new ColorSerializationInfo(color).Write(writer);
        }

        internal static void SerializeCustomColorInfo(PColor32 color, Stream outgoingData)
        {
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            new ColorSerializationInfo(color).Write(writer);
        }

        internal static void SerializeCustomColorInfo(Color64 color, Stream outgoingData)
        {
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            new ColorSerializationInfo(color).Write(writer);
        }

        internal static void SerializeCustomColorInfo(PColor64 color, Stream outgoingData)
        {
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            new ColorSerializationInfo(color).Write(writer);
        }

        internal static void SerializeCustomColorInfo(ColorF color, Stream outgoingData)
        {
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            new ColorSerializationInfo(color).Write(writer);
        }

        internal static void SerializeCustomColorInfo(PColorF color, Stream outgoingData)
        {
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            new ColorSerializationInfo(color).Write(writer);
        }

        internal static CustomColorInfo DeserializeCustomColorInfo(Stream stream)
        {
            using BinaryReader reader = stream.InitSerializationReader();
            return new ColorSerializationInfo(reader).ColorInfo!;
        }
    }
}
