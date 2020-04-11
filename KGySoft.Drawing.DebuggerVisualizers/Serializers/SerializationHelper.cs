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

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using KGySoft.Drawing.DebuggerVisualizers.Model;
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Reflection;
using KGySoft.Serialization.Binary;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Serializers
{
    /// <summary>
    /// Provides serialization and deserialization methods for debugger visualizers
    /// </summary>
    internal static class SerializationHelper
    {
        #region Internal Methods

        /// <summary>
        /// Serializes an <see cref="Image"/> instance along with metadata.
        /// </summary>
        internal static void SerializeImageInfo(Image image, Stream outgoingData)
        {
            var imageInfo = new ImageSerializationInfo(image);
            imageInfo.Write(new BinaryWriter(outgoingData));
        }

        /// <summary>
        /// Serializes an <see cref="Icon"/> instance along with metadata.
        /// </summary>
        internal static void SerializeIconInfo(Icon icon, Stream outgoingData)
        {
            var iconInfo = new ImageSerializationInfo(icon);
            iconInfo.Write(new BinaryWriter(outgoingData));
        }

        /// <summary>
        /// Serializes a <see cref="Graphics"/> instance along with metadata.
        /// </summary>
        internal static void SerializeGraphicsInfo(Graphics g, Stream outgoingData)
        {
            BinaryWriter bw = new BinaryWriter(outgoingData);

            // 1. Bitmap
            Bitmap bmp = g.ToBitmap(false);
            if (bmp != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.SaveAsPng(ms);
                    bw.Write((int)ms.Length);
                    bw.Write(ms.ToArray());
                }
            }
            else
                bw.Write(0);

            // 2. Transformation matrix
            BinarySerializer.SerializeByWriter(bw, g.Transform.Elements);

            // 3. Visible clip in pixels without transformation (with identity matrix)
            GraphicsState state = g.Save();
            g.Transform = new Matrix();
            g.PageUnit = GraphicsUnit.Pixel;
            bw.Write(BinarySerializer.SerializeValueType(Rectangle.Truncate(g.VisibleClipBounds)));
            g.Restore(state);

            // 4. Info (as seen by user's transformation)
            using (Matrix m = g.Transform)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("World transformation: ");
                if (m.IsIdentity)
                    sb.Append("None (Identity Matrix)");
                else
                {
                    // offset
                    PointF offset = new PointF(m.OffsetX, m.OffsetY);
                    if (offset != PointF.Empty)
                        sb.AppendFormat("Offset: {0}; ", offset);
                    float[] elements = m.Elements;

                    // when there is rotation, the angle/zoom is mixed so displaying them together
                    if (!elements[1].Equals(0f) || !elements[2].Equals(0f))
                        sb.AppendFormat("Rotation and zoom matrix: [{0}; {1}] [{2}; {3}]", elements[0], elements[1], elements[2], elements[3]);
                    else if (elements[0].Equals(elements[3]))
                        sb.AppendFormat("Zoom: {0}", elements[0]);
                    else
                        sb.AppendFormat("Horizontal zoom: {0}; Vertical zoom: {1}", elements[0], elements[3]);
                }

                sb.AppendLine();
                string isTransformed = m.IsIdentity ? String.Empty : "Transformed ";
                sb.Append($"{isTransformed}Clip Bounds: {g.ClipBounds}{Environment.NewLine}"
                    + $"{isTransformed}Visible Clip Bounds (Unit = {g.PageUnit}): {g.VisibleClipBounds}{Environment.NewLine}"
                    + $"Resolution: {g.DpiX}x{g.DpiY} DPI{Environment.NewLine}"
                    + $"Page Scale: {g.PageScale}{Environment.NewLine}" 
                    + $"Compositing Mode: {g.CompositingMode}{Environment.NewLine}"
                    + $"Compositing Quality: {g.CompositingQuality}{Environment.NewLine}"
                    + $"Interpolation Mode: {g.InterpolationMode}{Environment.NewLine}"
                    + $"Pixel Offset Mode: {g.PixelOffsetMode}{Environment.NewLine}"
                    + $"Smoothing Mode: {g.SmoothingMode}{Environment.NewLine}"
                    + $"Text Rendering Hint: {g.TextRenderingHint}{Environment.NewLine}"
                    + $"Text Contrast: {g.TextContrast}{Environment.NewLine}"
                    + $"Rendering Origin: {g.RenderingOrigin}");
                bw.Write(sb.ToString());
            }
        }

        /// <summary>
        /// Serializes a <see cref="BitmapData"/> instance along with metadata.
        /// </summary>
        internal static void SerializeBitmapDataInfo(BitmapData bitmapData, Stream outgoingData)
        {
            BinaryWriter bw = new BinaryWriter(outgoingData);

            // 1. Bitmap
            using (Bitmap bmp = new Bitmap(bitmapData.Width, bitmapData.Height, bitmapData.Stride, bitmapData.PixelFormat, bitmapData.Scan0))
                WriteImage(bw, bmp);

            // 2. Info
            bw.Write(String.Format("Size: {1}{0}Stride: {2} bytes{0}Pixel Format: {3}",
                Environment.NewLine, new Size(bitmapData.Width, bitmapData.Height), bitmapData.Stride, bitmapData.PixelFormat));
        }

        /// <summary>
        /// Serializes any <see cref="object"/>, even non serializable ones.
        /// </summary>
        internal static void SerializeAnyObject(object target, Stream outgoingData) => BinarySerializer.SerializeToStream(outgoingData, target);

        internal static ImageInfo DeserializeImageInfo(Stream stream) => new ImageSerializationInfo(new BinaryReader(stream)).ImageInfo;

        /// <summary>
        /// Deserializes <see cref="BitmapData"/> infos from the stream that can be passed to <see cref="DebuggerHelper.DebugBitmapData"/>.
        /// </summary>
        internal static BitmapDataInfo DeserializeBitmapDataInfo(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);

            // 1. Bitmap
            var bitmap = (Bitmap)ReadImage(br);

            // 2. Info
            string specialInfo = br.ReadString();
            return new BitmapDataInfo { Data = bitmap, SpecialInfo = specialInfo };
        }

        /// <summary>
        /// Deserializes <see cref="Graphics"/> infos from the stream that can be passed to <see cref="DebuggerHelper.DebugGraphics"/>.
        /// </summary>
        internal static GraphicsInfo DeserializeGraphicsInfo(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);
            var result = new GraphicsInfo();

            // 1. Bitmap
            int bmpLen = br.ReadInt32();
            if (bmpLen > 0)
            {
                MemoryStream ms = new MemoryStream(br.ReadBytes(bmpLen));
                result.Data = new Bitmap(ms);
            }

            // 2. Transformation matrix
            result.Elements = (float[])BinarySerializer.DeserializeByReader(br);

            // 3. Visible rect in pixels
            result.VisibleRect = (Rectangle)BinarySerializer.DeserializeValueType(typeof(Rectangle), br.ReadBytes(Marshal.SizeOf(typeof(Rectangle))));

            // 4. Info
            result.SpecialInfo = br.ReadString();

            return result;
        }

        /// <summary>
        /// Deserializes any <see cref="object"/> that was serialized by <see cref="SerializeAnyObject"/>.
        /// </summary>
        internal static object DeserializeAnyObject(Stream stream) => BinarySerializer.DeserializeFromStream(stream);

        internal static void WriteImage(BinaryWriter bw, Image image)
        {
            Debug.Assert(image != null, "Image should not be null here");
            int bpp;

            // writing a decoder compatible stream if image is a metafile...
            bool asImage = image is Metafile
                // ... or is a TIFF with 48/64 BPP because saving as TIFF can preserve pixel format only if the raw format is also TIFF...
                || (bpp = image.GetBitsPerPixel()) > 32 && image.RawFormat.Guid == ImageFormat.Tiff.Guid
                // ... or is an animated GIF, which always have 32 BPP pixel format
                || bpp == 32 && image.RawFormat.Guid == ImageFormat.Gif.Guid;

            bw.Write(asImage);
            if (asImage)
            {
                WriteAsImage(bw, image);
                return;
            }

            WriteRawBitmap((Bitmap)image, bw);
        }

        internal static Image ReadImage(BinaryReader br) => br.ReadBoolean()
            ? Image.FromStream(new MemoryStream(br.ReadBytes(br.ReadInt32())))
            : ReadRawBitmap(br);

        #endregion

        #region Private Methods

        private static void WriteAsImage(BinaryWriter bw, Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                if (image is Metafile metafile)
                    metafile.Save(ms);
                else if (image.RawFormat.Guid == ImageFormat.Gif.Guid)
                    image.SaveAsGif(ms);
                else if (image.RawFormat.Guid == ImageFormat.Tiff.Guid)
                    image.SaveAsTiff(ms);
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
                int width = data.Stride >> 2;
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
            Color[] palette = null;
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
                    resultPalette = (ColorPalette)Reflector.CreateInstance(typeof(ColorPalette), resultPalette.Entries.Length);
                for (int i = 0; i < palette.Length; i++)
                    resultPalette.Entries[i] = palette[i];
                result.Palette = resultPalette;
            }

            using (IWritableBitmapData data = result.GetWritableBitmapData())
            {
                int width = data.Stride >> 2;
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
