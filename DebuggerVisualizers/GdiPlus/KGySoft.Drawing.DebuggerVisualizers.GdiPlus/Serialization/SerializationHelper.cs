#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: SerializationHelper.cs
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

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Reflection;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.GdiPlus.Serialization
{
    /// <summary>
    /// Provides serialization and deserialization methods for debugger visualizers
    /// </summary>
    internal static class SerializationHelper
    {
        #region Constants

        private const PixelFormat Format32bppCmyk = (PixelFormat)0x200F;

        #endregion

        #region Internal Methods

        internal static void SerializeImageInfo(Image image, Stream outgoingData)
        {
            using var imageInfo = new ImageSerializationInfo(image);
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            imageInfo.Write(writer);
        }

        internal static void SerializeIconInfo(Icon icon, Stream outgoingData)
        {
            using var iconInfo = new ImageSerializationInfo(icon);
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            iconInfo.Write(writer);
        }

        internal static void SerializeReplacementImageInfo(ImageInfo imageInfo, Stream outgoingData)
        {
            using var replacementInfo = new ImageReplacementSerializationInfo(imageInfo);
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            replacementInfo.Write(writer);
        }

        internal static void SerializeGraphicsInfo(Graphics g, Stream outgoingData)
        {
            using var graphicsInfo = new GraphicsSerializationInfo(g);
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            graphicsInfo.Write(writer);
        }

        internal static void SerializeBitmapDataInfo(BitmapData bitmapData, Stream outgoingData)
        {
            using var bitmapDataInfo = new BitmapDataSerializationInfo(bitmapData);
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            bitmapDataInfo.Write(writer);
        }

        // color info is always serialized without a temp file
        internal static void SerializeColor(Color color, Stream outgoingData) => new ColorSerializationInfo(color).Write(new BinaryWriter(outgoingData));

        internal static void SerializeColorPalette(ColorPalette palette, Stream outgoingData)
        {
            using BinaryWriter writer = outgoingData.InitSerializationWriter();
            new ColorPaletteSerializationInfo(palette).Write(writer);
        }

        internal static ImageInfo DeserializeImageInfo(Stream stream)
        {
            using BinaryReader reader = stream.InitSerializationReader();
            return new ImageSerializationInfo(reader).ImageInfo;
        }

        internal static object? DeserializeReplacementImage(Stream stream)
        {
            using var imageInfo = new ImageReplacementSerializationInfo(stream.InitSerializationReader());
            return imageInfo.GetReplacementObject();
        }

        internal static BitmapDataInfo DeserializeBitmapDataInfo(Stream stream)
        {
            using BinaryReader reader = stream.InitSerializationReader();
            return new BitmapDataSerializationInfo(reader).BitmapDataInfo;
        }

        internal static GraphicsInfo DeserializeGraphicsInfo(Stream stream)
        {
            using BinaryReader reader = stream.InitSerializationReader();
            return new GraphicsSerializationInfo(reader).GraphicsInfo;
        }

        // color is always deserialized without a temp file (and there is no ColorInfo type in ImagingTools.Model)
        internal static Color DeserializeColor(Stream stream) => new ColorSerializationInfo(new BinaryReader(stream)).Color;

        // there is no PaletteInfo type in ImagingTools.Model
        internal static ColorPalette DeserializeColorPalette(Stream stream)
        {
            using BinaryReader reader = stream.InitSerializationReader();
            return new ColorPaletteSerializationInfo(reader).Palette;
        }

        internal static void WriteImage(BinaryWriter bw, Image image)
        {
            int bpp;

            // writing a decoder compatible stream if image is a metafile...
            bool asImage = image is Metafile
                // ... or is a TIFF with 48/64 BPP because saving as TIFF can preserve pixel format only if the raw format is also TIFF...
                || (bpp = image.GetBitsPerPixel()) > 32 && image.RawFormat.Guid == ImageFormat.Tiff.Guid
                // ... or is an animated GIF, which always has 32 BPP pixel format
                || bpp == 32 && image.RawFormat.Guid == ImageFormat.Gif.Guid
                // ... or image is an icon - actually needed only for Windows XP to prevent error from LockBits when sizes are not recognized
                || image.RawFormat.Guid == ImageFormat.Icon.Guid;

            bw.Write(asImage);
            if (asImage)
            {
                WriteAsImage(bw, image);
                return;
            }

            // writing the image without any encoder in a raw format preserving any pixel format
            WriteRawBitmap((Bitmap)image, bw);
        }

        internal static Image ReadImage(BinaryReader br) => br.ReadBoolean()
            ? Image.FromStream(new MemoryStream(br.ReadBytes(br.ReadInt32())))
            : ReadRawBitmap(br);

        internal static void WriteIcon(BinaryWriter bw, Icon icon)
        {
            using (var ms = new MemoryStream())
            {
                icon.SaveAsIcon(ms);
                bw.Write((int)ms.Length);
                bw.Write(ms.GetBuffer(), 0, (int)ms.Length);
            }
        }

        internal static Icon? ReadIcon(BinaryReader br) => Icons.FromStream(new MemoryStream(br.ReadBytes(br.ReadInt32())));

        #endregion

        #region Private Methods

        private static void WriteAsImage(BinaryWriter bw, Image image)
        {
            // we must use an inner stream because image.Save (at least TIFF encoder) may overwrite
            // the stream content before the original start position
            using (var ms = new MemoryStream())
            {
                if (image is Metafile metafile)
                    metafile.Save(ms);
                else if (image.RawFormat.Guid == ImageFormat.Gif.Guid)
                    image.SaveAsGif(ms);
                else if (image.RawFormat.Guid == ImageFormat.Tiff.Guid)
                    image.SaveAsTiff(ms);
                else if (image.RawFormat.Guid == ImageFormat.Icon.Guid)
                    image.SaveAsIcon(ms);
                else
                {
                    Debug.Fail("It is not expected to serialize an image as a PNG");
                    image.SaveAsPng(ms);
                }

                bw.Write((int)ms.Length);
                bw.Write(ms.GetBuffer(), 0, (int)ms.Length);
            }
        }

        private static void WriteRawBitmap(Bitmap bitmap, BinaryWriter bw)
        {
            Size size = bitmap.Size;
            bw.Write(size.Width);
            bw.Write(size.Height);
            PixelFormat pixelFormat = bitmap.PixelFormat;
            bw.Write((int)pixelFormat);
            if (pixelFormat.ToBitsPerPixel() <= 8)
            {
                Color[] palette = bitmap.Palette.Entries;
                bw.Write(palette.Length);
                foreach (Color color in palette)
                    bw.Write(color.ToArgb());
            }

            using (IReadableBitmapData data = bitmap.GetReadableBitmapData())
            {
                int width = data.RowSize >> 2;
                IReadableBitmapDataRowMovable row = data.FirstRow;
                do
                {
                    for (int x = 0; x < width; x++)
                        bw.Write(row.ReadRaw<int>(x));
                } while (row.MoveNextRow());
            }
        }

        private static Bitmap ReadRawBitmap(BinaryReader br)
        {
            var size = new Size(br.ReadInt32(), br.ReadInt32());
            var pixelFormat = (PixelFormat)br.ReadInt32();
            Color[]? palette = null;
            if (pixelFormat.ToBitsPerPixel() <= 8)
            {
                palette = new Color[br.ReadInt32()];
                for (int i = 0; i < palette.Length; i++)
                    palette[i] = Color.FromArgb(br.ReadInt32());
            }

            var result = new Bitmap(size.Width, size.Height, pixelFormat == Format32bppCmyk ? PixelFormat.Format24bppRgb : pixelFormat);
            if (palette != null)
            {
                ColorPalette resultPalette = result.Palette;
                if (resultPalette.Entries.Length != palette.Length)
                    resultPalette = (ColorPalette)Reflector.CreateInstance(typeof(ColorPalette), palette.Length);
                for (int i = 0; i < palette.Length; i++)
                    resultPalette.Entries[i] = palette[i];
                result.Palette = resultPalette;
            }

            using (IWritableBitmapData data = result.GetWritableBitmapData())
            {
                int width = data.RowSize >> 2;
                IWritableBitmapDataRowMovable row = data.FirstRow;
                do
                {
                    for (int x = 0; x < width; x++)
                        row.WriteRaw(x, br.ReadInt32());
                } while (row.MoveNextRow());
            }

            return result;
        }

        #endregion
    }
}
