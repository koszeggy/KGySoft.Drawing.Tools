#region Used namespaces

using KGySoft.DebuggerVisualizers.Common;
using Microsoft.VisualStudio.DebuggerVisualizers;

#endregion

namespace KGySoft.DebuggerVisualizers.VS2008
{
    internal sealed class BitmapDataDebuggerVisualizer : DialogDebuggerVisualizer
    {
        #region Methods

        /// <summary>
        /// Shows the specified window service.
        /// </summary>
        /// <param name="windowService">The window service.</param>
        /// <param name="objectProvider">The object provider.</param>
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            DebuggerHelper.DebugBitmapData(SerializationHelper.DeserializeBitmapData(objectProvider.GetData()));
        }

        #endregion
    }
}
