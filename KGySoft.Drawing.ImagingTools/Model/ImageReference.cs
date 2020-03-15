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
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;
using System.Security;

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    [Serializable]
    internal sealed class ImageReference : IObjectReference
    {
        #region Fields

        private readonly string fileName;
        private readonly int imageType;
        private readonly byte[] rawData;

        #endregion

        #region Constructors

        internal ImageReference(ImageTypes imageType, string fileName)
        {
            this.imageType = (int)imageType;
            this.fileName = fileName;
        }

        internal ImageReference(ImageTypes imageType, byte[] rawData)
        {
            this.imageType = (int)imageType;
            this.rawData = rawData;
        }

        #endregion

        #region Methods

        [SecurityCritical]
        public object GetRealObject(StreamingContext context)
        {
            ImageTypes imageType = (ImageTypes)this.imageType;
            if (imageType == ImageTypes.None || (rawData == null && String.IsNullOrEmpty(fileName)))
                return null;

            MemoryStream ms = new MemoryStream(rawData ?? File.ReadAllBytes(fileName));
            switch (imageType)
            {
                case ImageTypes.Bitmap:
                    return new Bitmap(ms);
                case ImageTypes.Metafile:
                    return new Metafile(ms);
                case ImageTypes.Icon:
                    return new Icon(ms);
                default:
                    throw new InvalidOperationException(Res.InternalError($"Unexpected image type: {imageType}"));
            }
        }

        #endregion
    }
}
