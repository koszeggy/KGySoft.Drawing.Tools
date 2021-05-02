#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: MetafileDebuggerVisualizer.cs
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

using System.Diagnostics.CodeAnalysis;
using System.IO;

using KGySoft.Drawing.DebuggerVisualizers.Serialization;
using KGySoft.Drawing.ImagingTools.Model;

using Microsoft.VisualStudio.DebuggerVisualizers;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers
{
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses",
        Justification = "False alarm, instantiated by VS debugger visualizers")]
    internal sealed class MetafileDebuggerVisualizer : DialogDebuggerVisualizer
    {
        #region Methods

        /// <summary>
        /// Shows the specified window service.
        /// </summary>
        /// <param name="windowService">The window service.</param>
        /// <param name="objectProvider">The object provider.</param>
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            using (ImageInfo imageInfo = SerializationHelper.DeserializeImageInfo(objectProvider.GetData()))
            {
                ImageInfo? replacementObject = DebuggerHelper.DebugMetafile(imageInfo, objectProvider.IsObjectReplaceable);
                if (!objectProvider.IsObjectReplaceable || replacementObject == null)
                    return;

                using var ms = new MemoryStream();
                SerializationHelper.SerializeImageInfo(replacementObject, ms);
                objectProvider.ReplaceData(ms);
            }
        }

        #endregion
    }
}
