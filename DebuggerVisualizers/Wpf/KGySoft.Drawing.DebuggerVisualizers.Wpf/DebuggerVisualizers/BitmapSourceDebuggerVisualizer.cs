#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageSourceDebuggerVisualizer.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2022 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using KGySoft.Drawing.DebuggerVisualizers.Wpf.Serialization;
using KGySoft.Drawing.ImagingTools.Model;

using Microsoft.VisualStudio.DebuggerVisualizers;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Wpf.DebuggerVisualizers
{
    internal class ImageSourceDebuggerVisualizer : DialogDebuggerVisualizer
    {
        #region Methods

        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            CustomBitmapInfo bitmapInfo = SerializationHelper.DeserializeCustomBitmapInfo(objectProvider.GetData());
            DebuggerHelper.DebugCustomBitmap(bitmapInfo);
            //ImageInfo? replacementObject = GdiPlusDebuggerHelper.DebugImage(imageInfo, objectProvider.IsObjectReplaceable);
            //if (replacementObject == null)
            //    return;

            //using var ms = new MemoryStream();
            //SerializationHelper.SerializeReplacementImageInfo(replacementObject, ms);
            //objectProvider.ReplaceData(ms);
        }

        #endregion
    }
}