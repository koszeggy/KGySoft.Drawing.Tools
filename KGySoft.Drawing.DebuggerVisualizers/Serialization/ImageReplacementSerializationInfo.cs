#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageReplacementSerializationInfo.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
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

using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Serialization
{
    internal sealed class ImageReplacementSerializationInfo : IDisposable
    {
        #region Fields

        private readonly ImageInfo? imageInfo;
        private readonly Stream? stream;

        #endregion

        #region Constructors

        internal ImageReplacementSerializationInfo(ImageInfo imageInfo) => this.imageInfo = imageInfo;

        internal ImageReplacementSerializationInfo(Stream stream) => this.stream = stream;

        #endregion

        #region Methods

        #region Public Methods

        public void Dispose()
        {
            imageInfo?.Dispose();
            stream?.Dispose();
        }

        #endregion

        #region Internal Methods

        internal void Write(BinaryWriter bw)
        {
            // 1.) Image type
            bw.Write((byte)imageInfo!.Type);
            if (imageInfo.Type == ImageInfoType.None)
                return;

            // 2.) File reference
            string? fileName = imageInfo.FileName;
            bw.Write(fileName != null);
            if (fileName != null)
            {
                bw.Write(fileName);
                return;
            }

            // 3.) Image content
            switch (imageInfo.Type)
            {
                case ImageInfoType.Icon:
                case ImageInfoType.MultiRes:
                    SerializationHelper.WriteIcon(bw, imageInfo.GetCreateIcon()!);
                    return;

                case ImageInfoType.Pages:
                    // we must use an inner stream because image.Save (at least TIFF encoder) may overwrite
                    // the stream content before the original start position
                    using (var inner = new MemoryStream())
                    {
                        imageInfo.Frames!.Select(f => f.Image!).SaveAsMultipageTiff(inner);
                        bw.Write((int)inner.Length);
                        bw.Flush();
                        inner.WriteTo(bw.BaseStream);
                    }

                    return;

                case ImageInfoType.Animation:
                    // Using GetCreateImage so in case of a changed animated GIF the possible exception is thrown
                    SerializationHelper.WriteImage(bw, imageInfo.GetCreateImage()!);
                    return;

                default:
                    SerializationHelper.WriteImage(bw, imageInfo.Image!);
                    return;
            }
        }

        internal object? GetReplacementObject()
        {
            var br = new BinaryReader(stream!);

            // 1.) Image type
            ImageInfoType type = (ImageInfoType)br.ReadByte();
            if (type == ImageInfoType.None)
                return null;

            // 2.) From file
            string? fileName = br.ReadBoolean() ? br.ReadString() : null;
            if (fileName != null)
            {
                if (type == ImageInfoType.Icon)
                {
                    using FileStream fs = File.OpenRead(fileName);
                    return Icons.FromStream(fs);
                }

                try
                {
                    return Image.FromFile(fileName);
                }
                catch (Exception)
                {
                    if (!fileName.EndsWith(".ico", StringComparison.OrdinalIgnoreCase))
                        throw;

                    // special handling for icon files: as a Bitmap icons may throw an exception
                    using FileStream fs = File.OpenRead(fileName);
                    using Icon? icon = Icons.FromStream(fs);
                    return icon?.ExtractNearestBitmap(new Size(UInt16.MaxValue, UInt16.MaxValue), PixelFormat.Format32bppArgb);
                }
            }

            // 3.) From stream
            switch (type)
            {
                case ImageInfoType.Icon:
                    return SerializationHelper.ReadIcon(br);

                case ImageInfoType.MultiRes:
                    using (Icon? icon = SerializationHelper.ReadIcon(br))
                    {
                        try
                        {
                            return icon?.ToMultiResBitmap();
                        }
                        catch (Exception)
                        {
                            // special handling for icon files: as a Bitmap icons may throw an exception
                            return icon?.ExtractNearestBitmap(new Size(UInt16.MaxValue, UInt16.MaxValue), PixelFormat.Format32bppArgb);
                        }
                    }

                case ImageInfoType.Pages:
                    return Image.FromStream(new MemoryStream(br.ReadBytes(br.ReadInt32())));

                default:
                    return SerializationHelper.ReadImage(br);
            }
        }

        #endregion

        #endregion
    }
}
