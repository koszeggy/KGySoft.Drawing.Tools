#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: SerializationHelper.cs
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

using System.Drawing;
using System.IO;

using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;

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
