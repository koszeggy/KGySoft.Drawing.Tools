using System;
using System.Drawing;
using System.IO;

using Microsoft.VisualStudio.DebuggerVisualizers;

namespace StandardSerializerTest
{
    public class ImageSerializer : VisualizerObjectSource
    {
        #region Methods

        /// <summary>
        /// Called when the object to be debugged is about to be serialized
        /// </summary>
        public override void GetData(object target, Stream outgoingData) => SerializationHelper.SerializeImageInfo((Image)target, outgoingData);

        /// <summary>
        /// Called by the new type of visualizer debuggers when VisualizerTarget.ObjectSource.RequestDataAsync is called
        /// </summary>
        public override void TransferData(object target, Stream incomingData, Stream outgoingData) => GetData(target, outgoingData);

        ///// <summary>
        ///// Called when the debugged object has been replaced
        ///// </summary>
        //public override object? CreateReplacementObject(object target, Stream incomingData) => SerializationHelper.DeserializeReplacementImage(incomingData);

        #endregion
    }
}
