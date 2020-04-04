#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageDebuggerVisualizer.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2019 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution. If not, then this file is considered as
//  an illegal copy.
//
//  Unauthorized copying of this file, via any medium is strictly prohibited.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using KGySoft.Drawing.DebuggerVisualizers.Model;
using KGySoft.Drawing.DebuggerVisualizers.Serializers;
using KGySoft.Drawing.ImagingTools.Model;

using Microsoft.VisualStudio.DebuggerVisualizers;

#endregion

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
            ImageReference replacementObject = DebuggerHelper.DebugImage(SerializationHelper.DeserializeImageInfo(objectProvider.GetData()), objectProvider.IsObjectReplaceable);
            if (objectProvider.IsObjectReplaceable && replacementObject != null)
                objectProvider.ReplaceObject(replacementObject);
        }

        #endregion
    }
}
