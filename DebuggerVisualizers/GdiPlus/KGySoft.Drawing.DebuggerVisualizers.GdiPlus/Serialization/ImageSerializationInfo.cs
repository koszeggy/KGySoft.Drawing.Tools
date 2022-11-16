#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageSerializationInfo.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.GdiPlus.Serialization
{
    internal sealed class ImageSerializationInfo : IDisposable
    {
        #region Properties

        internal ImageInfo ImageInfo { get; private set; } = default!;

        #endregion

        #region Constructors

        internal ImageSerializationInfo(Image image) => ImageInfo = new ImageInfo(image, false);

        internal ImageSerializationInfo(Icon icon) => ImageInfo = new ImageInfo(icon, false);

        internal ImageSerializationInfo(BinaryReader reader)
        {
            ReadFrom(reader);
            ImageInfo.SetModified(false);
        }

        #endregion

        #region Methods

        #region Static Methods

        private static void WriteMeta(BinaryWriter bw, ImageInfoBase imageInfo)
        {
            bw.Write(imageInfo.HorizontalRes);
            bw.Write(imageInfo.VerticalRes);
            bw.Write(imageInfo.Size.Width);
            bw.Write(imageInfo.Size.Height);
            bw.Write((int)imageInfo.PixelFormat);
            bw.Write(imageInfo.RawFormat.ToByteArray());

            bw.Write(imageInfo.Palette.Length);
            foreach (Color color in imageInfo.Palette)
                bw.Write(color.ToArgb());
        }

        private static void ReadMeta(BinaryReader br, ImageInfoBase imageInfo)
        {
            imageInfo.HorizontalRes = br.ReadSingle();
            imageInfo.VerticalRes = br.ReadSingle();
            imageInfo.Size = new Size(br.ReadInt32(), br.ReadInt32());
            imageInfo.PixelFormat = (PixelFormat)br.ReadInt32();
            imageInfo.RawFormat = new Guid(br.ReadBytes(16));

            var palette = new Color[br.ReadInt32()];
            imageInfo.Palette = palette;
            for (int i = 0; i < palette.Length; i++)
                palette[i] = Color.FromArgb(br.ReadInt32());
        }

        #endregion

        #region Instance Methods

        #region Public Methods

        public void Dispose()
        {
            // image was not cloned so disposing only possibly created frames
            if (!ImageInfo.HasFrames)
                return;

            foreach (ImageFrameInfo frame in ImageInfo.Frames!)
                frame.Dispose();
        }

        #endregion

        #region Internal Methods

        internal void Write(BinaryWriter bw)
        {
            // 1. Image type
            bw.Write((byte)ImageInfo.Type);
            if (ImageInfo.Type == ImageInfoType.None)
                return;

            // 2. How multi-frame image is saved
            bool saveAsSingleImage = ImageInfo.Type.In(ImageInfoType.SingleImage, ImageInfoType.Icon) || ForceSaveCompoundImage();
            if (ImageInfo.Type != ImageInfoType.SingleImage)
                bw.Write(saveAsSingleImage);

            // 3. Main image (if any)
            if (saveAsSingleImage)
            {
                if (ImageInfo.Type == ImageInfoType.Icon)
                    SerializationHelper.WriteIcon(bw, ImageInfo.GetCreateIcon()!);
                else
                    SerializationHelper.WriteImage(bw, ImageInfo.GetCreateImage()!);
            }

            // Meta is saved even for pages so we will have a general size, etc.
            WriteMeta(bw, ImageInfo);

            // 4. Frames (if any)
            if (!ImageInfo.HasFrames)
                return;

            ImageFrameInfo[] frames = ImageInfo.Frames!;
            int length = frames.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                ImageFrameInfo frame = frames[i];
                if (!saveAsSingleImage)
                    SerializationHelper.WriteImage(bw, GetFrame(i));
                WriteMeta(bw, frame);
                if (ImageInfo.Type == ImageInfoType.Animation)
                    bw.Write(frame.Duration);

            }
        }

        #endregion

        #region Private Methods

        private Image GetFrame(int imageIndex)
        {
            Debug.Assert(ImageInfo.Frames?.Length > imageIndex);

            if (ImageInfo.Frames![imageIndex].Image is Image frame)
                return frame;

            FrameDimension dimension = ImageInfo.Type switch
            {
                ImageInfoType.Animation => FrameDimension.Time,
                ImageInfoType.Pages => FrameDimension.Page,
                _ => throw new InvalidOperationException($"Unexpected image type: {ImageInfo.Type}")
            };

            Image result = ImageInfo.Image!;
            result.SelectActiveFrame(dimension, imageIndex);
            return result;
        }

        private void ReadFrom(BinaryReader br)
        {
            // 1. Image type
            var imageType = (ImageInfoType)br.ReadByte();
            ImageInfo = new ImageInfo(imageType);
            if (imageType == ImageInfoType.None)
                return;

            // 2. How multi-frame image is saved
            bool savedAsSingleImage = imageType == ImageInfoType.SingleImage || br.ReadBoolean();

            // 3. Main/single image (if any)
            if (savedAsSingleImage)
            {
                if (imageType == ImageInfoType.Icon)
                    ImageInfo.Icon = SerializationHelper.ReadIcon(br);
                else
                    ImageInfo.Image = SerializationHelper.ReadImage(br);
            }

            ReadMeta(br, ImageInfo);

            if (imageType == ImageInfoType.SingleImage || imageType == ImageInfoType.Icon && ImageInfo.Icon!.GetImagesCount() <= 1)
                return;

            // 4. Frames (if any)
            int len = br.ReadInt32();
            var frames = new ImageFrameInfo[len];
            Bitmap?[] frameImages = savedAsSingleImage
                ? imageType == ImageInfoType.Icon ? ImageInfo.Icon!.ExtractBitmaps() : ((Bitmap)ImageInfo.Image!).ExtractBitmaps()
                : new Bitmap[len];
            Debug.Assert(frameImages.Length == frames.Length);
            for (int i = 0; i < len; i++)
            {
                var frame = new ImageFrameInfo(frameImages[i]);
                frames[i] = frame;
                if (!savedAsSingleImage)
                    frame.Image = SerializationHelper.ReadImage(br);
                ReadMeta(br, frame);
                if (imageType == ImageInfoType.Animation)
                    frame.Duration = br.ReadInt32();
            }

            ImageInfo.Frames = frames;
        }

        private bool ForceSaveCompoundImage() => ImageInfo.RawFormat == ImageFormat.Gif.Guid;

        #endregion

        #endregion

        #endregion
    }
}
