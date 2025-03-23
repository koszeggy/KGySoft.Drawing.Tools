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

using System.IO;

using KGySoft.Drawing.ImagingTools.Model;

using SkiaSharp;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.SkiaSharp.Serialization
{
    internal static class SerializationHelper
    {
        #region Methods

        internal static void SerializeCustomBitmapInfo(SKBitmap bitmap, Stream outgoingData)
        {
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            using var info = new BitmapDataSerializationInfo(bitmap);
            info.Write(writer);
        }

        internal static void SerializeCustomBitmapInfo(SKPixmap pixmap, Stream outgoingData)
        {
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            using var info = new BitmapDataSerializationInfo(pixmap);
            info.Write(writer);
        }

        internal static void SerializeCustomBitmapInfo(SKImage image, Stream outgoingData)
        {
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            using var info = new BitmapDataSerializationInfo(image);
            info.Write(writer);
        }

        internal static void SerializeCustomBitmapInfo(SKSurface image, Stream outgoingData)
        {
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            using var info = new BitmapDataSerializationInfo(image);
            info.Write(writer);
        }

        internal static CustomBitmapInfo DeserializeCustomBitmapInfo(Stream stream)
        {
            using BinaryReader reader = stream.InitSerializationReader();
            return new BitmapDataSerializationInfo(reader).BitmapInfo!;
        }

        internal static void SerializeCustomColorInfo(SKColor color, Stream outgoingData)
        {
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            new ColorSerializationInfo(color).Write(writer);
        }

        internal static void SerializeCustomColorInfo(SKPMColor color, Stream outgoingData)
        {
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            new ColorSerializationInfo(color).Write(writer);
        }

        internal static void SerializeCustomColorInfo(SKColorF color, Stream outgoingData)
        {
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            new ColorSerializationInfo(color).Write(writer);
        }

        internal static CustomColorInfo DeserializeCustomColorInfo(Stream stream)
        {
            using BinaryReader reader = stream.InitSerializationReader();
            return new ColorSerializationInfo(reader).ColorInfo!;
        }

        #endregion
    }
}
