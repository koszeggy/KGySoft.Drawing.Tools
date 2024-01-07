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

using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Wpf.Serialization
{
    internal static class SerializationHelper
    {
        #region Methods

        internal static void SerializeCustomBitmapInfo(ImageSource image, Stream outgoingData)
        {
            using var imageInfo = image switch
            {
                BitmapSource bitmap => new ImageSourceSerializationInfo(bitmap),
                _ => new ImageSourceSerializationInfo(image)
            };
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            imageInfo.Write(writer);
        }

        internal static CustomBitmapInfo DeserializeCustomBitmapInfo(Stream stream)
        {
            using BinaryReader reader = stream.InitSerializationReader();
            return new ImageSourceSerializationInfo(reader).BitmapInfo!;
        }

        internal static void SerializeCustomColorInfo(Color color, Stream outgoingData)
        {
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            new ColorSerializationInfo(color).Write(writer);
        }

        internal static CustomColorInfo DeserializeCustomColorInfo(Stream stream)
        {
            using BinaryReader reader = stream.InitSerializationReader();
            return new ColorSerializationInfo(reader).ColorInfo!;
        }

        internal static void SerializeCustomPaletteInfo(BitmapPalette palette, Stream outgoingData)
        {
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            new BitmapPaletteSerializationInfo(palette).Write(writer);
        }

        internal static CustomPaletteInfo DeserializeCustomPaletteInfo(Stream stream)
        {
            using BinaryReader reader = stream.InitSerializationReader();
            return new BitmapPaletteSerializationInfo(reader).PaletteInfo!;
        }

        #endregion
    }
}