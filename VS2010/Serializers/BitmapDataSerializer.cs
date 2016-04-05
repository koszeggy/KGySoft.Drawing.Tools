#region Used namespaces

using System.IO;
using KGySoft.DebuggerVisualizers.Common;
using Microsoft.VisualStudio.DebuggerVisualizers;

#endregion

namespace KGySoft.DebuggerVisualizers.VS2010
{
    internal class BitmapDataSerializer : VisualizerObjectSource
    {
        #region Methods

        /// <summary>
        /// Called when the object to be debugged is about to be serialized
        /// </summary>
        public override void GetData(object target, Stream outgoingData)
        {
            SerializationHelper.SerializeBitmapData(target, outgoingData);
        }

        #endregion
    }
}
