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

using KGySoft.Drawing.ImagingTools.Model;

using SkiaSharp;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.SkiaSharp.Serialization
{
    internal static class SerializationHelper
    {
        #region Methods

        internal static void SerializeCustomBitmapInfo(object target, Stream outgoingData)
        {
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            using var info = target switch
            {
                SKBitmap bitmap => new BitmapDataSerializationInfo(bitmap),
                SKPixmap pixmap => new BitmapDataSerializationInfo(pixmap),
                SKImage image => new BitmapDataSerializationInfo(image),
                SKSurface surface => new BitmapDataSerializationInfo(surface),
                _ => throw new ArgumentException(PublicResources.ArgumentInvalid, nameof(target))
            };

            info.Write(writer);
        }

        internal static CustomBitmapInfo DeserializeCustomBitmapInfo(Stream stream)
        {
            using BinaryReader reader = stream.InitSerializationReader();
            return new BitmapDataSerializationInfo(reader).BitmapInfo!;
        }

        internal static void SerializeCustomColorInfo(object target, Stream outgoingData)
        {
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            var info = target switch
            {
                SKColor color => new ColorSerializationInfo(color),
                SKPMColor pmColor => new ColorSerializationInfo(pmColor),
                SKColorF colorF => new ColorSerializationInfo(colorF),
                _ => throw new ArgumentException(PublicResources.ArgumentInvalid, nameof(target))
            };

            info.Write(writer);
        }

        internal static CustomColorInfo DeserializeCustomColorInfo(Stream stream)
        {
            using BinaryReader reader = stream.InitSerializationReader();
            return new ColorSerializationInfo(reader).ColorInfo!;
        }

        #endregion
    }
}
