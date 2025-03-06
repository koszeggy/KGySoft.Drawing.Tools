#region Usings

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

#endregion

namespace StandardSerializerTest
{
    public static class SerializationHelper
    {
        internal static void SerializeImageInfo(Image image, Stream outgoingData)
        {
            // just for testing
            image.Save(outgoingData, ImageFormat.Png);
            //using var imageInfo = new ImageSerializationInfo(image);
            //using BinaryWriter writer = outgoingData.InitSerializationWriter();
            //imageInfo.Write(writer);
        }
    }
}
