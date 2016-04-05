#region Used namespaces

using Microsoft.VisualStudio.DebuggerVisualizers;
using KGySoft.DebuggerVisualizers.Common;

#endregion

namespace KGySoft.DebuggerVisualizers.VS2010
{
    internal sealed class PaletteDebuggerVisualizer : DialogDebuggerVisualizer
    {
        #region Methods

        /// <summary>
        /// Shows the specified window service.
        /// </summary>
        /// <param name="windowService">The window service.</param>
        /// <param name="objectProvider">The object provider.</param>
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            object replacementObject = DebuggerHelper.DebugPalette(SerializationHelper.DeserializeAnyObject(objectProvider.GetData()), objectProvider.IsObjectReplaceable);
            if (objectProvider.IsObjectReplaceable && replacementObject != null)
                objectProvider.ReplaceObject(replacementObject);
        }

        #endregion
    }
}
