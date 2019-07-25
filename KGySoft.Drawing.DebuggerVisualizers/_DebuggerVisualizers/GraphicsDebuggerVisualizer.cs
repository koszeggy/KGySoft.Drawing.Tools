#region Used namespaces

using KGySoft.Drawing.ImagingTools.PublicApi;
using Microsoft.VisualStudio.DebuggerVisualizers;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers
{
    internal sealed class GraphicsDebuggerVisualizer : DialogDebuggerVisualizer
    {
        #region Methods

        /// <summary>
        /// Shows the specified window service.
        /// </summary>
        /// <param name="windowService">The window service.</param>
        /// <param name="objectProvider">The object provider.</param>
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            DebuggerHelper.DebugGraphics(SerializationHelper.DeserializeGraphics(objectProvider.GetData()));
        }

        #endregion
    }
}
