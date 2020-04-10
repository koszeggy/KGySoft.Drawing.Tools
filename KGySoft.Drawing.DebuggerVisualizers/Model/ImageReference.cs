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

using KGySoft.Drawing.DebuggerVisualizers.Serializers;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Model
{
    [Serializable]
    internal sealed class ImageReference : IObjectReference
    {
        #region Fields

        private readonly string fileName;
        private readonly bool isIcon;
        private readonly byte[] rawData;

        #endregion

        #region Constructors

        internal ImageReference(ImageInfo imageInfo)
        {
            if (imageInfo.Type == ImageInfoType.None)
                return;

            isIcon = imageInfo.Type == ImageInfoType.Icon;
            fileName = imageInfo.FileName;
            if (fileName == null)
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
                if (isIcon)
                {
                    (imageInfo.Icon ?? imageInfo.GenerateIcon()).SaveAsIcon(ms);
                    return ms.ToArray();
                }

                using (var bw = new BinaryWriter(ms))
                {
                    // compound image is not available: not generating it unnecessarily
                    if (imageInfo.HasFrames && imageInfo.Image == null)
                    {
                        bw.Write(false); // not raw
                        switch (imageInfo.Type)
                        {
                            case ImageInfoType.Pages:
                                imageInfo.Frames.Select(f => f.Image).SaveAsMultipageTiff(ms);
                                return ms.ToArray();

                            case ImageInfoType.MultiRes:
                                (imageInfo.Icon ?? imageInfo.GenerateIcon()).SaveAsIcon(ms);
                                return ms.ToArray();

                            case ImageInfoType.Animation:
                                imageInfo.GenerateImage();
                                break;
                        }
                    }

                    SerializationHelper.WriteImage(bw, imageInfo.Image);
                    return ms.ToArray();
                }
            }
        }

        #endregion

        #region Instance Methods

        [SecurityCritical]
        public object GetRealObject(StreamingContext context)
        {
            if (fileName == null && rawData == null)
                return null;

            MemoryStream ms = new MemoryStream(rawData ?? File.ReadAllBytes(fileName));
            if (isIcon)
                return new Icon(ms);
            return SerializationHelper.ReadImage(new BinaryReader(ms));
        }

        #endregion

        #endregion
    }
}
