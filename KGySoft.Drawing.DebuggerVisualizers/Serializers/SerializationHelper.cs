﻿#region Copyright

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
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Serialization.Binary;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Serializers
{
    /// <summary>
    /// Provides serialization and deserialization methods for debugger visualizers
    /// </summary>
    internal static class SerializationHelper
    {
        #region Methods

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
            static bool SaveAsRaw(Image image)
            {
                return false;
                var bpp = image.GetBitsPerPixel();

                // 48/64 bpp: allowing TIFF only because it can be saved (only if the raw format is already a TIFF)
                if (bpp > 32)
                    return image.RawFormat.Guid != ImageFormat.Tiff.Guid;

                // indexed: raw if the palette has more transparent colors or at least one partially transparent color
                if (bpp <= 8)
                {
                    Color[] colors = image.Palette.Entries.Where(c => c.A < 255).Take(2).ToArray();
                    return colors.Length > 1 || colors.Length == 1 && colors[0].A > 0;
                }

                // any other case: 16 bpp formats must be saved as raw
                return bpp == 16;
            }


            // TODO: stream with seek
            Debug.Assert(image != null, "Image should not be null here");
            using (MemoryStream ms = new MemoryStream())
            {
                bool asRaw = SaveAsRaw(image);
                bw.Write(asRaw);

                if (asRaw)
                    image.SaveAsRaw(ms);
                else
                {
                    int bpp;
                    if (image is Metafile metafile)
                        metafile.Save(ms);
                    else if ((bpp = image.GetBitsPerPixel()) <= 8 || image.RawFormat.Guid == ImageFormat.Gif.Guid)
                        // or GIF condition: animated GIFs have 32 BPP format
                        image.SaveAsGif(ms);
                    else if (bpp > 32 && image.RawFormat.Guid == ImageFormat.Tiff.Guid)
                        // TIFF: only if 48/64 BPP because saving as TIFF can preserve pixel format only if the raw format is also TIFF
                        image.SaveAsTiff(ms);
                    else
                        image.SaveAsPng(ms);
                }

                image.Save(ms, image.RawFormat.Guid == ImageFormat.Gif.Guid ? ImageFormat.Gif : ImageFormat.Png);
                bw.Write((int)ms.Length);
                bw.Write(ms.GetBuffer(), 0, (int)ms.Length);
            }
        }

        internal static Image ReadImage(BinaryReader br)
        {
            // TODO: stream with seek
            bool savedAsRaw = br.ReadBoolean();
            MemoryStream ms = new MemoryStream(br.ReadBytes(br.ReadInt32()));
            return savedAsRaw
                ? ImageExtensions.LoadFromRaw(ms)
                : Image.FromStream(ms);
        }

        #endregion
    }

    // TODO: delete
    internal static class ImageExtensions
    {
        internal static void SaveAsRaw(this Image image, Stream s) => throw new NotImplementedException("TODO: use Drawing");

        public static Bitmap LoadFromRaw(Stream s) => throw new NotImplementedException();
    }

}
