#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: GdiPlusDebuggerHelper.cs
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Drawing.ImagingTools.View;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.GdiPlus
{
    /// <summary>
    /// A helper class to access the debugger visualizers of this assembly.
    /// </summary>
    public static class GdiPlusDebuggerHelper
    {
        #region Methods

        #region Public Methods

        /// <summary>
        /// Gets the debugger visualizers of this assembly.
        /// </summary>
        /// <returns>The debugger visualizers of this assembly.</returns>
        public static Dictionary<Type, DebuggerVisualizerAttribute> GetDebuggerVisualizers()
            => Attribute.GetCustomAttributes(typeof(GdiPlusDebuggerHelper).Assembly, typeof(DebuggerVisualizerAttribute))
                .Cast<DebuggerVisualizerAttribute>().ToDictionary(a => a.Target!);

        #endregion

        #region Internal Methods

        internal static ImageInfo? DebugImage(ImageInfo imageInfo, bool isReplaceable)
        {
            using (IViewModel<ImageInfo> viewModel = ViewModelFactory.FromImage(imageInfo, !isReplaceable))
                return DebugImageInfo(viewModel, isReplaceable);
        }

        internal static ImageInfo? DebugBitmap(ImageInfo bitmapInfo, bool isReplaceable)
        {
            using (IViewModel<ImageInfo> viewModel = ViewModelFactory.FromBitmap(bitmapInfo, !isReplaceable))
                return DebugImageInfo(viewModel, isReplaceable);
        }

        internal static ImageInfo? DebugMetafile(ImageInfo metafileInfo, bool isReplaceable)
        {
            using (IViewModel<ImageInfo> viewModel = ViewModelFactory.FromMetafile(metafileInfo, !isReplaceable))
                return DebugImageInfo(viewModel, isReplaceable);
        }

        internal static ImageInfo? DebugIcon(ImageInfo iconInfo, bool isReplaceable)
        {
            using (IViewModel<ImageInfo> viewModel = ViewModelFactory.FromIcon(iconInfo, !isReplaceable))
                return DebugImageInfo(viewModel, isReplaceable);
        }

        internal static void DebugBitmapData(BitmapDataInfo bitmapDataInfo)
        {
            using (IViewModel vm = ViewModelFactory.FromBitmapData(bitmapDataInfo))
                ViewFactory.ShowDialog(vm);
        }

        internal static void DebugGraphics(GraphicsInfo graphicsInfo)
        {
            using (IViewModel vm = ViewModelFactory.FromGraphics(graphicsInfo))
                ViewFactory.ShowDialog(vm);
        }

        #endregion

        #region Private Methods

        private static ImageInfo? DebugImageInfo(IViewModel<ImageInfo> viewModel, bool isReplaceable)
        {
            ViewFactory.ShowDialog(viewModel);
            if (isReplaceable && viewModel.IsModified)
                return viewModel.GetEditedModel();

            return null;
        }

        #endregion

        #endregion
    }
}
