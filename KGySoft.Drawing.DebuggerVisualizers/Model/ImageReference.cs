#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageReference.cs
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
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;

using KGySoft.Drawing.DebuggerVisualizers.Serialization;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

#region Suppressions

#if NETCOREAPP3_0_OR_GREATER
#pragma warning disable 8766 // false alarm, GetRealObject CAN return null
#endif

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Model
{
    [Serializable]
    internal sealed class ImageReference : IObjectReference
    {
        #region Fields

        private readonly string? fileName;
        private readonly bool asIcon;
        private readonly byte[]? rawData;

        #endregion

        #region Constructors

        internal ImageReference(ImageInfo imageInfo)
        {
            if (imageInfo.Type == ImageInfoType.None)
                return;

            asIcon = imageInfo.Type == ImageInfoType.Icon;
            fileName = imageInfo.FileName;
            if (fileName != null)
                return;

            rawData = SerializeImage(imageInfo);
        }

        #endregion

        #region Methods

        #region Static Methods

        private byte[] SerializeImage(ImageInfo imageInfo)
        {
            using (var ms = new MemoryStream())
            {
                if (asIcon)
                {
                    (imageInfo.Icon ?? imageInfo.GetCreateIcon()!).SaveAsIcon(ms);
                    return ms.ToArray();
                }

                using (var bw = new BinaryWriter(ms))
                {
                    switch (imageInfo.Type)
                    {
                        case ImageInfoType.Pages:
                        case ImageInfoType.MultiRes:
                            // we must use an inner stream because image.Save (at least TIFF encoder) may overwrite
                            // the stream content before the original start position
                            using (var inner = new MemoryStream())
                            {
                                if (imageInfo.Type == ImageInfoType.Pages)
                                    imageInfo.Frames!.Select(f => f.Image!).SaveAsMultipageTiff(inner);
                                else
                                    (imageInfo.Icon ?? imageInfo.GetCreateIcon()!).SaveAsIcon(inner);

                                bw.Write(true); // AsImage
                                bw.Write((int)inner.Length);
                                inner.WriteTo(ms);
                            }

                            break;

                        case ImageInfoType.Animation:
                            SerializationHelper.WriteImage(bw, imageInfo.GetCreateImage()!);
                            break;

                        default:
                            SerializationHelper.WriteImage(bw, imageInfo.Image!);
                            break;
                    }

                    return ms.ToArray();
                }
            }
        }

        #endregion

        #region Instance Methods

        [SecurityCritical]
        public object? GetRealObject(StreamingContext context)
        {
            if (fileName == null && rawData == null)
                return null;

            if (fileName != null)
            {
                if (asIcon)
                    return new Icon(fileName);

                try
                {
                    return Image.FromFile(fileName);
                }
                catch (Exception)
                {
                    if (!fileName.EndsWith(".ico", StringComparison.OrdinalIgnoreCase))
                        throw;

                    // special handling for icon files: as a Bitmap icons may throw an exception
                    using (var info = new ImageInfo(new Icon(fileName)))
                        return info.GetCreateImage()!.Clone();
                }
            }

            var ms = new MemoryStream(rawData!);
            return asIcon ? new Icon(ms) : SerializationHelper.ReadImage(new BinaryReader(ms));
        }

        #endregion

        #endregion
    }
}
