#region Used namespaces

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;
using System.Security;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    [Serializable]
    internal sealed class ImageReference : IObjectReference
    {
        #region Fields

        private string fileName;
        private int imageType;
        private byte[] rawData;

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
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}
