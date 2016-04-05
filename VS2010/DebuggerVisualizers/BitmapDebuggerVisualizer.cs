#region Used namespaces

using KGySoft.DebuggerVisualizers.Common;
using Microsoft.VisualStudio.DebuggerVisualizers;

#endregion

namespace KGySoft.DebuggerVisualizers.VS2010
{
    internal sealed class BitmapDebuggerVisualizer : DialogDebuggerVisualizer
    {
        #region Methods

        /// <summary>
        /// Shows the specified window service.
        /// </summary>
        /// <param name="windowService">The window service.</param>
        /// <param name="objectProvider">The object provider.</param>
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            object replacementObject = DebuggerHelper.DebugBitmap(SerializationHelper.DeserializeImage(objectProvider.GetData()), objectProvider.IsObjectReplaceable);
            if (objectProvider.IsObjectReplaceable && replacementObject != null)
                objectProvider.ReplaceObject(replacementObject);
        }

        #endregion
    }
}
