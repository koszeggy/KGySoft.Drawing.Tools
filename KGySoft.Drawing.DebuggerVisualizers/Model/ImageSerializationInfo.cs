#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageSerializationInfo.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2020 - All Rights Reserved
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
using System.Drawing.Imaging;
using System.IO;

using KGySoft.Drawing.DebuggerVisualizers.Serializers;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Model
{
    internal sealed class ImageSerializationInfo
    {
        #region Properties

        internal ImageInfo ImageInfo { get; private set; }

        #endregion

        #region Constructors

        internal ImageSerializationInfo(Image image)
        {
            ImageInfo = new ImageInfo(image);
        }

        internal ImageSerializationInfo(Icon icon)
        {
            ImageInfo = new ImageInfo(icon);
        }

        internal ImageSerializationInfo(BinaryReader br)
        {
            ReadFrom(br);
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

        #region Internal Methods

        internal void Write(BinaryWriter bw)
        {
            // 1. Image type
            bw.Write((byte)ImageInfo.Type);
            if (ImageInfo.Type == ImageInfoType.None)
                return;

            // 2. How multi-frame image is saved
            bool saveAsSingleImage = ImageInfo.Type == ImageInfoType.SingleImage || ForceSaveCompoundImage();
            if (ImageInfo.Type != ImageInfoType.SingleImage)
                bw.Write(saveAsSingleImage);

            // 3. Main image (if any). For pages meta is not saved for main image even if compound saving was forced.
            if (saveAsSingleImage)
                SerializationHelper.WriteImage(bw, ImageInfo.Image);
            if (ImageInfo.Type != ImageInfoType.Pages)
                WriteMeta(bw, ImageInfo);

            // 4. Frames (if any)
            if (ImageInfo.Type != ImageInfoType.SingleImage)
            {
                Debug.Assert(ImageInfo.Frames != null, "Frames should not be null");
                bw.Write(ImageInfo.Frames.Length);
                foreach (ImageFrameInfo frame in ImageInfo.Frames)
                {
                    if (!saveAsSingleImage)
                        SerializationHelper.WriteImage(bw, frame.Image);
                    WriteMeta(bw, frame);
                    if (ImageInfo.Type == ImageInfoType.Animation)
                        bw.Write(frame.Duration);
                }
            }
        }

        #endregion

        #region Private Methods

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
                ImageInfo.Image = SerializationHelper.ReadImage(br);
            if (imageType != ImageInfoType.Pages)
                ReadMeta(br, ImageInfo);

            if (imageType == ImageInfoType.SingleImage)
                return;

            // 4. Frames (if any)
            int len = br.ReadInt32();
            ImageInfo.Frames = new ImageFrameInfo[len];
            foreach (ImageFrameInfo frame in ImageInfo.Frames)
            {
                if (!savedAsSingleImage)
                    frame.Image = SerializationHelper.ReadImage(br);
                ReadMeta(br, frame);
                if (imageType == ImageInfoType.Animation)
                    frame.Duration = br.ReadInt32();
            }

            if (savedAsSingleImage)
            {
                Bitmap[] frames = ((Bitmap)ImageInfo.Image).ExtractBitmaps();
                Debug.Assert(frames.Length == ImageInfo.Frames.Length);
                for (int i = 0; i < frames.Length; i++)
                    ImageInfo.Frames[i].Image = frames[i];
            }
        }

        private bool ForceSaveCompoundImage() => ImageInfo.RawFormat == ImageFormat.Gif.Guid;

        #endregion

        #endregion

        #endregion
    }
}
