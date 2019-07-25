#region Used namespaces

using System.IO;
using KGySoft.Drawing.ImagingTools;
using KGySoft.Drawing.ImagingTools.PublicApi;
using Microsoft.VisualStudio.DebuggerVisualizers;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Serializers
{
    internal class ImageSerializer : VisualizerObjectSource
    {
        #region Methods

        /// <summary>
        /// Called when the object to be debugged is about to be serialized
        /// </summary>
        public override void GetData(object target, Stream outgoingData)
        {
            SerializationHelper.SerializeImage(target, outgoingData);
        }

        #endregion
    }
}
