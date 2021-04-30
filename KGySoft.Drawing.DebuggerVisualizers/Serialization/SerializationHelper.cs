#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: SerializationHelper.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2019 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution. If not, then this file is considered as
//  an illegal copy.
//
//  Unauthorized copying of this file, via any medium is strictly prohibited.
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

namespace KGySoft.Drawing.DebuggerVisualizers.Serialization
{
    /// <summary>
    /// Provides serialization and deserialization methods for debugger visualizers
    /// </summary>
    internal static class SerializationHelper
    {
        #region Internal Methods

        internal static void SerializeImageInfo(Image image, Stream outgoingData)
        {
            using (var imageInfo = new ImageSerializationInfo(image))
                imageInfo.Write(new BinaryWriter(outgoingData));
        }

        internal static void SerializeIconInfo(Icon icon, Stream outgoingData)
        {
            using (var iconInfo = new ImageSerializationInfo(icon))
                iconInfo.Write(new BinaryWriter(outgoingData));
        }

        internal static void SerializeGraphicsInfo(Graphics g, Stream outgoingData)
        {
            using (var graphicsInfo = new GraphicsSerializationInfo(g))
                graphicsInfo.Write(new BinaryWriter(outgoingData));
        }

        internal static void SerializeBitmapDataInfo(BitmapData bitmapData, Stream outgoingData)
        {
            using (var bitmapDataInfo = new BitmapDataSerializationInfo(bitmapData))
                bitmapDataInfo.Write(new BinaryWriter(outgoingData));
        }

        internal static void SerializeColor(Color color, Stream outgoingData) => new ColorSerializationInfo(color).Write(new BinaryWriter(outgoingData));

        internal static void SerializeColorPalette(ColorPalette palette, Stream outgoingData) => new ColorPaletteSerializationInfo(palette).Write(new BinaryWriter(outgoingData));

        internal static ImageInfo DeserializeImageInfo(Stream stream) => new ImageSerializationInfo(stream).ImageInfo;

        internal static BitmapDataInfo DeserializeBitmapDataInfo(Stream stream) => new BitmapDataSerializationInfo(stream).BitmapDataInfo;

        internal static GraphicsInfo DeserializeGraphicsInfo(Stream stream) => new GraphicsSerializationInfo(stream).GraphicsInfo;

        internal static Color DeserializeColor(Stream stream) => new ColorSerializationInfo(stream).Color;

        internal static ColorPalette DeserializeColorPalette(Stream stream) => new ColorPaletteSerializationInfo(stream).Palette;

        internal static void WriteImage(BinaryWriter bw, Image image)
        {
            int bpp;

            // writing a decoder compatible stream if image is a metafile...
            bool asImage = image is Metafile
                // ... or is a TIFF with 48/64 BPP because saving as TIFF can preserve pixel format only if the raw format is also TIFF...
                || (bpp = image.GetBitsPerPixel()) > 32 && image.RawFormat.Guid == ImageFormat.Tiff.Guid
                // ... or is an animated GIF, which always have 32 BPP pixel format
                || bpp == 32 && image.RawFormat.Guid == ImageFormat.Gif.Guid
                // ... or image is an icon - actually needed only for Windows XP to prevent error from LockBits when sizes are not recognized
                || image.RawFormat.Guid == ImageFormat.Icon.Guid;

            bw.Write(asImage);
            if (asImage)
            {
                WriteAsImage(bw, image);
                return;
            }

            WriteRawBitmap((Bitmap)image, bw);
        }

        internal static void WriteIcon(BinaryWriter bw, Icon icon)
        {
            using (var ms = new MemoryStream())
            {
                icon.SaveAsIcon(ms);
                bw.Write((int)ms.Length);
                bw.Write(ms.GetBuffer(), 0, (int)ms.Length);
            }
        }

        internal static Image ReadImage(BinaryReader br) => br.ReadBoolean()
            ? Image.FromStream(new MemoryStream(br.ReadBytes(br.ReadInt32())))
            : ReadRawBitmap(br);

        internal static Icon ReadIcon(BinaryReader br) => new Icon(new MemoryStream(br.ReadBytes(br.ReadInt32())));

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
                IReadableBitmapDataRow row = data.FirstRow;
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

            var result = new Bitmap(size.Width, size.Height, pixelFormat);
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
                IWritableBitmapDataRow row = data.FirstRow;
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
