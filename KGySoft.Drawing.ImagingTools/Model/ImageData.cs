﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageData.cs
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
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    internal sealed class ImageData : IDisposable
    {
        #region Properties

        internal Image Image { get; set; }
        internal float HorizontalRes { get; set; }
        internal float VerticalRes { get; set; }
        internal Size Size { get; set; }
        internal PixelFormat PixelFormat { get; set; }
        internal Color[] Palette { get; set; }
        internal Guid RawFormat { get; set; }
        internal int Duration { get; set; }

        internal int BitsPerPixel => Image.GetPixelFormatSize(PixelFormat);

        #endregion

        #region Constructors

        internal ImageData(Image image, bool initMeta)
        {
            Image = image;
            Palette = new Color[0];
            if (initMeta)
                InitMetaFromImage(image);
        }

        internal ImageData(BinaryReader br)
        {
            int len = br.ReadInt32();
            if (len > 0)
            {
                byte[] imageData = br.ReadBytes(len);
                MemoryStream ms = new MemoryStream(imageData);
                Image = Image.FromStream(ms);
            }

            HorizontalRes = br.ReadSingle();
            VerticalRes = br.ReadSingle();
            Size = new Size(br.ReadInt32(), br.ReadInt32());
            PixelFormat = (PixelFormat)br.ReadInt32();
            Palette = new Color[br.ReadInt32()];
            for (int i = 0; i < Palette.Length; i++)
            {
                Palette[i] = Color.FromArgb(br.ReadInt32());
            }

            RawFormat = new Guid(br.ReadBytes(16));
            Duration = br.ReadInt32();

            //if (imageData != null && Image is Metafile)
            //{
            //    //using (Graphics refGraphics = form.CreateGraphics())
            //    //{
            //    //    IntPtr refHdc = refGraphics.GetHdc();
            //    Image = new Bitmap(Image, Size);

            //    //    refGraphics.ReleaseHdc(refHdc);
            //    //}
            //}
        }

        #endregion

        #region Methods

        #region Static Methods

        internal static void FromImage(Image image, bool toSerialize, out ImageData imageData, out ImageData[] frames)
        {
            // icon
            if (image is Bitmap && image.RawFormat.Guid == ImageFormat.Icon.Guid)
            {
                imageData = new ImageData(image, true);
                Bitmap[] iconImages = ((Bitmap)image).ExtractBitmaps();
                if (iconImages.Length == 1)
                {
                    iconImages[0].Dispose();
                    frames = null;
                    return;
                }

                // icon image will be assembled again after deserialization
                if (toSerialize)
                    imageData.Image = null;
                frames = new ImageData[iconImages.Length];
                for (int i = 0; i < iconImages.Length; i++)
                {
                    frames[i] = new ImageData(iconImages[i], true);

                    // setting raw format explicitly as it is at icons
                    frames[i].RawFormat = iconImages[i].Width == 256 || iconImages[i].Height == 256 ? ImageFormat.Png.Guid : ImageFormat.Bmp.Guid;
                }

                // selecting the maximum size for the compound icon
                imageData.Size = frames.Aggregate(Size.Empty, (size, i) => size.Width >= i.Size.Width ? size : i.Size);
                return;
            }

            // other image: check if it has multiple frames
            FrameDimension dimension = null;
            Guid[] dimensions = image.FrameDimensionsList;
            if (dimensions.Length > 0)
            {
                if (dimensions[0] == FrameDimension.Page.Guid)
                    dimension = FrameDimension.Page;
                else if (dimensions[0] == FrameDimension.Time.Guid)
                    dimension = FrameDimension.Time;
                else if (dimensions[0] == FrameDimension.Resolution.Guid)
                    dimension = FrameDimension.Resolution;
            }

            // single image, unknown dimension or not bitmap
            int frameCount = dimension != null ? image.GetFrameCount(dimension) : 0;
            if (frameCount <= 1 || dimension == null || !(image is Bitmap))
            {
                imageData = new ImageData(image, true);
                frames = null;
                return;
            }

            // multiple frames
            byte[] times = null;
            imageData = null;
            Bitmap bitmap = (Bitmap)image;

            // in case of animation there is a compound image
            if (dimension == FrameDimension.Time)
            {
                imageData = new ImageData(image, true);
                if (toSerialize)
                    imageData.Image = null;

                times = image.GetPropertyItem(0x5100).Value;
            }
            else if (!toSerialize)
                imageData = new ImageData(image, true);

            frames = new ImageData[frameCount];
            for (int frame = 0; frame < frameCount; frame++)
            {
                image.SelectActiveFrame(dimension, frame);
                frames[frame] = new ImageData(bitmap.CloneCurrentFrame(), false);
                frames[frame].InitMetaFromImage(image);
                if (times != null)
                {
                    int duration = BitConverter.ToInt32(times, frame << 2);
                    duration = duration == 0 ? 100 : duration * 10;
                    frames[frame].Duration = duration;
                    if (imageData.RawFormat == ImageFormat.Gif.Guid && imageData.BitsPerPixel > 8)
                        frames[frame].PixelFormat = PixelFormat.Format8bppIndexed;
                }
            }

            image.SelectActiveFrame(dimension, 0);
        }

        internal static void FromIcon(Icon icon, bool toSerialize, out ImageData iconData, out ImageData[] iconImages)
        {
            // obtaining icon images in original format
            Bitmap[] iconImagesOrigFormat = icon.ExtractBitmaps(true);

            // when single-image icon, creating image from bitmap to avoid bad quality of system icons (pixel format will be system dependent)
            if (iconImagesOrigFormat.Length == 1)
            {
                iconData = new ImageData(iconImagesOrigFormat[0], true);

                if (toSerialize)
                {
                    iconData.Image.Dispose();
                    iconData.Image = null;
                }
                else if (iconData.Image.PixelFormat != PixelFormat.Format32bppArgb)
                {
                    // after initializing adding transparent icon
                    iconData.Image.Dispose();
                    iconData.Image = icon.ToAlphaBitmap();
                    iconImagesOrigFormat[0].Dispose();
                }

                iconData.RawFormat = ImageFormat.Icon.Guid;
                iconImages = null;
                return;
            }

            // generating image from icon
            try
            {
                iconData = new ImageData(icon.ToMultiResBitmap(), true);
            }
            catch (ArgumentException)
            {
                // In Windows XP it can happen that multi-res bitmap throws an exception even if PNG images are uncompressed
                iconData = new ImageData(icon.ExtractNearestBitmap(new Size(UInt16.MaxValue, UInt16.MaxValue), PixelFormat.Format32bppArgb), true);
                iconData.RawFormat = ImageFormat.Icon.Guid;
            }

            // icon image will be assembled again after deserialization
            if (toSerialize)
            {
                iconData.Image.Dispose();
                iconData.Image = null;
            }

            Bitmap[] iconImagesAlpha = null;
            if (!toSerialize)
                iconImagesAlpha = icon.ExtractBitmaps();
            iconImages = new ImageData[iconImagesOrigFormat.Length];
            for (int i = 0; i < iconImages.Length; i++)
            {
                iconImages[i] = new ImageData(iconImagesOrigFormat[i], true);
                iconImages[i].Image = iconImagesAlpha?[i];

                // setting raw format explicitly as it is at icons
                iconImages[i].RawFormat = icon.IsCompressed(i) ? ImageFormat.Png.Guid : ImageFormat.Bmp.Guid;
                int bpp = icon.GetBitsPerPixel(i);
                iconImages[i].PixelFormat = bpp == 1 ? PixelFormat.Format1bppIndexed
                    : bpp == 4 ? PixelFormat.Format4bppIndexed
                    : bpp == 8 ? PixelFormat.Format8bppIndexed
                    : bpp == 24 ? PixelFormat.Format24bppRgb
                    : iconImages[i].RawFormat == ImageFormat.Bmp.Guid ? PixelFormat.Format32bppRgb
                    : PixelFormat.Format32bppArgb;
                iconImagesOrigFormat[i].Dispose();
            }

            // selecting the maximum size for the compound icon
            iconData.Size = iconImages.Aggregate(Size.Empty, (size, image) => size.Width >= image.Size.Width ? size : image.Size);
        }

        #endregion

        #region Instance Methods

        #region Public Methods

        public void Dispose() => Image?.Dispose();

        #endregion

        #region Internal Methods

        internal void Write(BinaryWriter bw)
        {
            Image image = Image;
            if (image == null)
                bw.Write(0);
            else
            {
                Metafile metafile = image as Metafile;
                using (MemoryStream ms = new MemoryStream())
                {
                    if (metafile != null)
                        metafile.Save(ms);
                    else
                        image.Save(ms, image.RawFormat.Guid == ImageFormat.Gif.Guid ? ImageFormat.Gif : ImageFormat.Png);
                    bw.Write((int)ms.Length);
                    bw.Write(ms.GetBuffer(), 0, (int)ms.Length);
                }
            }

            bw.Write(HorizontalRes);
            bw.Write(VerticalRes);
            bw.Write(Size.Width);
            bw.Write(Size.Height);
            bw.Write((int)PixelFormat);
            bw.Write(Palette.Length);
            foreach (Color color in Palette)
                bw.Write(color.ToArgb());

            bw.Write(RawFormat.ToByteArray());
            bw.Write(Duration);
        }

        #endregion

        #region Private Methods

        private void InitMetaFromImage(Image image)
        {
            if (image == null)
                return;

            Size = image.Size;
            HorizontalRes = image.HorizontalResolution;
            VerticalRes = image.VerticalResolution;
            PixelFormat = image.PixelFormat;
            Palette = image is Metafile ? new Color[0] : image.Palette.Entries;
            RawFormat = image.RawFormat.Guid;
        }

        #endregion

        #endregion

        #endregion
    }
}