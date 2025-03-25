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
using KGySoft.Serialization.Binary;

using SkiaSharp;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.SkiaSharp.Serialization
{
    internal static class SerializationHelper
    {
        #region Methods

        #region Internal Methods

        internal static void SerializeCustomBitmapInfo(object target, Stream outgoingData)
        {
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            BitmapDataSerializationInfo info;
            try
            {
                info = target switch
                {
                    SKBitmap bitmap => new BitmapDataSerializationInfo(bitmap),
                    SKPixmap pixmap => new BitmapDataSerializationInfo(pixmap),
                    SKImage image => new BitmapDataSerializationInfo(image),
                    SKSurface surface => new BitmapDataSerializationInfo(surface),

                    // This point is reached only if the target assembly is different from our SkiaSharp so we must extract everything by reflection
                    _ => new BitmapDataSerializationInfo(target)
                };
            }
            catch (Exception e) when (e is not ArgumentException)
            {
                // If the debugged project uses a different version of KGySoft.Drawing.Core, a MissingMethodException can be thrown
                // (even though the signature is the same, just because of different assembly versions).
                info = new BitmapDataSerializationInfo(target);
#if DEBUG
                info.BitmapInfo!.CustomAttributes[$"{e.GetType()}"] = e.Message;
#endif
            }

            using (info)
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
            ColorSerializationInfo? info;
            switch (target)
            {
                case SKColor color:
                    info = new ColorSerializationInfo(color);
                    break;
                case SKPMColor pmColor:
                    info = new ColorSerializationInfo(pmColor);
                    break;
                case SKColorF colorF:
                    info = new ColorSerializationInfo(colorF);
                    break;

                default:
                    // This point is reached only if the target assembly is different from ours.
                    try
                    {
                        // Serializing the color as a byte array and deserializing it with the correct assembly identity.
                        byte[] bytes = BinarySerializer.SerializeValueType((ValueType)target);
                        info = target.GetType().Name switch
                        {
                            nameof(SKColor) => new ColorSerializationInfo(BinarySerializer.DeserializeValueType<SKColor>(bytes)),
                            nameof(SKPMColor) => new ColorSerializationInfo(BinarySerializer.DeserializeValueType<SKPMColor>(bytes)),
                            nameof(SKColorF) => new ColorSerializationInfo(BinarySerializer.DeserializeValueType<SKColorF>(bytes)),
                            _ => throw new ArgumentException(PublicResources.ArgumentInvalid, nameof(target))
                        };

#if DEBUG
                        info.ColorInfo!.CustomAttributes["SkiaSharp version mismatch"] = $"{target.GetType().Assembly} vs. {typeof(SKColor).Assembly}";
#endif

                    }
                    catch (Exception e) when (e is not ArgumentException)
                    {
                        throw new ArgumentException(PublicResources.ArgumentInvalid, nameof(target), e);
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

        #endregion

        #endregion
    }
}
