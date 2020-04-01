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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
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
        /// Serializes an <see cref="Image"/> instance.
        /// </summary>
        internal static void SerializeImage(Image image, Stream outgoingData)
        {
            // Bitmap/Metafile: ImageData
            ImageData.FromImage(image, true, out ImageData imageData, out ImageData[] frames);
            BinaryWriter bw = new BinaryWriter(outgoingData);

            // 1. Image type
            if (frames == null)
                bw.Write((byte)ImageDataTypes.SingleImage);
            else if (imageData == null)
                bw.Write((byte)ImageDataTypes.Pages);
            else
                bw.Write(frames[0].Duration > 0 ? (byte)ImageDataTypes.Animation : (byte)ImageDataTypes.IconBitmap);

            // 2. Image type != single image => images count
            int count = frames == null ? 1 : frames.Length + (imageData == null ? 0 : 1);
            if (frames != null)
                bw.Write(count);

            // 3. Main image (if any)
            imageData?.Write(bw);

            // 4. Frames (if any)
            if (frames != null)
            {
                foreach (ImageData frame in frames)
                    frame.Write(bw);
            }
        }

        /// <summary>
        /// Serializes an <see cref="Icon"/> instance.
        /// </summary>
        internal static void SerializeIcon(Icon icon, Stream outgoingData)
        {
            // icon: if needed, creating a new icon from stream and resetting target
            ImageData.FromIcon(icon, true, out ImageData iconData, out ImageData[] iconImages);
            BinaryWriter bw = new BinaryWriter(outgoingData);

            // 1. The icon data itself (length must be stored because Icon.ctor(Stream) consumes the whole stream, so using a new stream)
            using (MemoryStream ms = new MemoryStream())
            {
                icon.SaveHighQuality(ms);
                byte[] iconRawData = ms.ToArray();
                bw.Write(iconRawData.Length);
                bw.Write(iconRawData);
            }

            // 2. Icon images (without image)
            if (iconImages == null)
            {
                bw.Write(1);
                iconData.Write(bw);
            }
            else
            {
                bw.Write(iconImages.Length + 1);
                iconData.Write(bw);
                foreach (ImageData iconImage in iconImages)
                    iconImage.Write(bw);
            }
        }

        /// <summary>
        /// Serializes a <see cref="Graphics"/> instance.
        /// </summary>
        internal static void SerializeGraphics(Graphics g, Stream outgoingData)
        {
            BinaryWriter bw = new BinaryWriter(outgoingData);

            // 1. Bitmap
            Bitmap bmp = g.ToBitmap(false);
            if (bmp != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
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
        /// Serializes a <see cref="BitmapData"/> instance.
        /// </summary>
        internal static void SerializeBitmapData(BitmapData bitmapData, Stream outgoingData)
        {
            BinaryWriter bw = new BinaryWriter(outgoingData);

            // 1. Bitmap as ImageData
            using (Bitmap bmp = new Bitmap(bitmapData.Width, bitmapData.Height, bitmapData.Stride, bitmapData.PixelFormat, bitmapData.Scan0))
            {
                ImageData imageData = new ImageData(bmp, true);
                imageData.Write(bw);
            }

            // 2. Info
            bw.Write(String.Format("Size: {1}{0}Stride: {2} bytes{0}Pixel Format: {3}",
                Environment.NewLine, new Size(bitmapData.Width, bitmapData.Height), bitmapData.Stride, bitmapData.PixelFormat));
        }

        /// <summary>
        /// Serializes any <see cref="object"/>, even non serializable ones.
        /// </summary>
        internal static void SerializeAnyObject(object target, Stream outgoingData) => BinarySerializer.SerializeToStream(outgoingData, target);

        /// <summary>
        /// Deserializes <see cref="Image"/> infos (<see cref="Bitmap"/> or <see cref="Metafile"/>) from the stream
        /// that can be passed to <see cref="DebuggerHelper.DebugImage(ImageInfo,bool)"/>, <see cref="DebuggerHelper.DebugBitmap"/> or <see cref="DebuggerHelper.DebugMetafile"/>.
        /// </summary>
        internal static ImageInfo DeserializeImage(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);
            ImageDataTypes imageType = (ImageDataTypes)br.ReadByte();

            // single image
            if (imageType == ImageDataTypes.SingleImage)
            {
                ImageData imageData = new ImageData(br);
                return new ImageInfo { MainImage = imageData };
            }

            // multi-page image
            ImageData mainImage = null;
            int len = br.ReadInt32();
            if (imageType != ImageDataTypes.Pages)
            {
                mainImage = new ImageData(br);
                len--;
            }

            ImageData[] frames = new ImageData[len];
            for (int i = 0; i < len; i++)
                frames[i] = new ImageData(br);

            return new ImageInfo { MainImage = mainImage, Frames = frames };
        }

        /// <summary>
        /// Deserializes <see cref="Icon"/> from the stream
        /// that can be passed to <see cref="DebuggerHelper.DebugImage(ImageInfo,bool)"/>, <see cref="DebuggerHelper.DebugBitmap"/> or <see cref="DebuggerHelper.DebugMetafile"/>.
        /// </summary>
        internal static IconInfo DeserializeIcon(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);

            // icon image
            byte[] iconData = br.ReadBytes(br.ReadInt32());
            int len = br.ReadInt32();
            Icon icon = new Icon(new MemoryStream(iconData));
            ImageData compoundIcon = new ImageData(br);
            try
            {
                compoundIcon.Image = icon.ToMultiResBitmap();
            }
            catch (ArgumentException)
            {
                // In Windows XP it can happen that multi-res bitmap throws an exception even if PNG images are uncompressed
                compoundIcon.Image = icon.ExtractNearestBitmap(new Size(UInt16.MaxValue, UInt16.MaxValue), PixelFormat.Format32bppArgb);
            }

            // single icon image
            if (len == 1)
                return new IconInfo { Icon = icon, CompoundIcon = compoundIcon };

            // multi-image icon
            len--;
            Bitmap[] iconBitmaps = icon.ExtractBitmaps();
            ImageData[] iconImages = new ImageData[len];
            for (int i = 0; i < len; i++)
            {
                iconImages[i] = new ImageData(br);
                iconImages[i].Image = iconBitmaps[i];
            }

            return new IconInfo { Icon = icon, CompoundIcon = compoundIcon, IconImages = iconImages };
        }

        /// <summary>
        /// Deserializes <see cref="BitmapData"/> infos from the stream that can be passed to <see cref="DebuggerHelper.DebugBitmapData"/>.
        /// </summary>
        internal static BitmapDataInfo DeserializeBitmapData(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);

            // 1. Bitmap
            ImageData imageData = new ImageData(br);

            // 2. Info
            string specialInfo = br.ReadString();

            return new BitmapDataInfo { Data = imageData, SpecialInfo = specialInfo };
        }

        /// <summary>
        /// Deserializes <see cref="Graphics"/> infos from the stream that can be passed to <see cref="DebuggerHelper.DebugGraphics"/>.
        /// </summary>
        internal static GraphicsInfo DeserializeGraphics(Stream stream)
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

        #endregion
    }
}
