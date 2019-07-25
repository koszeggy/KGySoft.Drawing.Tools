using KGySoft.Drawing.ImagingTools.PublicApi;
using Microsoft.VisualStudio.DebuggerVisualizers;

namespace KGySoft.Drawing.DebuggerVisualizers
{
    internal sealed class ImageDebuggerVisualizer : DialogDebuggerVisualizer
    {
        #region Methods

        /// <summary>
        /// Shows the specified window service.
        /// </summary>
        /// <param name="windowService">The window service.</param>
        /// <param name="objectProvider">The object provider.</param>
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            object replacementObject = DebuggerHelper.DebugImage(SerializationHelper.DeserializeImage(objectProvider.GetData()), objectProvider.IsObjectReplaceable);
            if (objectProvider.IsObjectReplaceable && replacementObject != null)
                objectProvider.ReplaceObject(replacementObject);
        }

        #endregion
    }
}
