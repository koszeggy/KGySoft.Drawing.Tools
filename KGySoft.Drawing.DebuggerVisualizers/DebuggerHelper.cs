#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DebuggerHelper.cs
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

using KGySoft.Drawing.DebuggerVisualizers.Model;
using KGySoft.Drawing.DebuggerVisualizers.Serializers;
using KGySoft.Drawing.ImagingTools;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Drawing.ImagingTools.View;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers
{
    /// <summary>
    /// Provides debugger methods for debugger visualizers
    /// </summary>
    internal static class DebuggerHelper
    {
        #region Methods

        #region Internal Methods

        /// <summary>
        /// Shows the debugger for an <see cref="Image"/> or <see cref="Icon"/> object.
        /// </summary>
        /// <param name="imageInfo">The image info for debugging returned by <see cref="SerializationHelper.DeserializeImage"/></param>
        /// <param name="isReplaceable">Indicates whether the image is replaceable.</param>
        /// <returns>A non-<see langword="null"/>&#160;instance, when the image has been edited and should be serialized back; otherwise, <see langword="null"/>.</returns>
        internal static ImageReference DebugImage(ImageInfo imageInfo, bool isReplaceable) => DebugImage(imageInfo, isReplaceable, ImageTypes.All);

        /// <summary>
        /// Shows the debugger for a <see cref="Bitmap"/> or <see cref="Icon"/> object.
        /// </summary>
        /// <param name="imageInfo">The image infos for debugging returned by <see cref="SerializationHelper.DeserializeImage"/></param>
        /// <param name="isReplaceable">Indicates whether the image is replaceable.</param>
        /// <returns>A non-<see langword="null"/>&#160;instance, when the image has been edited and should be serialized back; otherwise, <see langword="null"/>.</returns>
        internal static ImageReference DebugBitmap(ImageInfo imageInfo, bool isReplaceable) => DebugImage(imageInfo, isReplaceable, ImageTypes.Bitmap | ImageTypes.Icon);

        /// <summary>
        /// Shows the debugger for a <see cref="Metafile"/>.
        /// </summary>
        /// <param name="imageInfo">The image infos for debugging returned by <see cref="SerializationHelper.DeserializeImage"/></param>
        /// <param name="isReplaceable">Indicates whether the image is replaceable.</param>
        /// <returns>A non-<see langword="null"/>&#160;instance, when the image has been edited and should be serialized back; otherwise, <see langword="null"/>.</returns>
        internal static ImageReference DebugMetafile(ImageInfo imageInfo, bool isReplaceable) => DebugImage(imageInfo, isReplaceable, ImageTypes.Metafile);

        /// <summary>
        /// Shows the debugger for an <see cref="Icon"/> object.
        /// </summary>
        /// <param name="imageInfo">The image infos for debugging returned by <see cref="SerializationHelper.DeserializeImage"/></param>
        /// <param name="isReplaceable">Indicates whether the image is replaceable.</param>
        /// <returns>A non-<see langword="null"/>&#160;instance, when the image has been edited and should be serialized back; otherwise, <see langword="null"/>.</returns>
        internal static ImageReference DebugIcon(ImageInfo imageInfo, bool isReplaceable) => DebugImage(imageInfo, isReplaceable, ImageTypes.Icon);

        /// <summary>
        /// Shows the debugger for a <see cref="BitmapData"/> object.
        /// </summary>
        /// <param name="bitmapDataInfo">The bitmap data infos for debugging returned by <see cref="SerializationHelper.DeserializeBitmapData"/>.</param>
        internal static void DebugBitmapData(ImageInfo bitmapDataInfo)
        {
            using (ViewModelBase vm = ViewModelFactory.FromBitmapData(bitmapDataInfo.MainImage, bitmapDataInfo.SpecialInfo))
                ViewFactory.ShowDialog(vm, null);
        }

        /// <summary>
        /// Shows the debugger for a <see cref="Graphics"/> object.
        /// </summary>
        /// <param name="graphicsInfo">The graphics infos for debugging returned by <see cref="SerializationHelper.DeserializeGraphics"/>.</param>
        internal static void DebugGraphics(GraphicsInfo graphicsInfo)
        {
            float[] elements = graphicsInfo.Elements;
            var matrix = new Matrix(elements[0], elements[1], elements[2], elements[3], elements[4], elements[5]);
            using (ViewModelBase vm = ViewModelFactory.FromGraphics(graphicsInfo.Data, matrix, graphicsInfo.VisibleRect, graphicsInfo.SpecialInfo))
                ViewFactory.ShowDialog(vm, null);
        }

        /// <summary>
        /// Shows the debugger for a <see cref="ColorPalette"/> instance.
        /// </summary>
        /// <param name="obj">The palette object to debug returned by <see cref="SerializationHelper.DeserializeAnyObject"/>.</param>
        /// <param name="isReplaceable">Indicates whether the palette is replaceable.</param>
        /// <returns>A non-<see langword="null"/>&#160;instance, when the palette has been edited and should be serialized back; otherwise, <see langword="null"/>.</returns>
        internal static ColorPalette DebugPalette(ColorPalette palette, bool isReplaceable)
        {
            IList<Color> colorList = palette.Entries;
            if (colorList.Count == 0)
                return null;
            using (PaletteVisualizerViewModel vm = ViewModelFactory.FromPalette(colorList))
            {
                vm.ReadOnly = !isReplaceable;
                ViewFactory.ShowDialog(vm, null);
                return isReplaceable && vm.IsModified ? palette : null;
            }
        }

        /// <summary>
        /// Shows the debugger for a <see cref="Color"/> instance.
        /// </summary>
        /// <param name="obj">The color object to debug.</param>
        /// <param name="isReplaceable">Indicates whether the color is replaceable.</param>
        /// <returns>A non-<see langword="null"/>&#160;instance, when the color has been edited and should be serialized back; otherwise, <see langword="null"/>.</returns>
        internal static Color? DebugColor(Color obj, bool isReplaceable)
        {
            using (ColorVisualizerViewModel vm = ViewModelFactory.FromColor(obj))
            {
                vm.ReadOnly = !isReplaceable;
                vm.Color = obj;
                ViewFactory.ShowDialog(vm, null);
                if (isReplaceable && vm.IsModified)
                    return vm.Color;
            }

            return null;
        }

        #endregion

        #region Private Methods

        private static ImageReference DebugImage(ImageInfo imageInfo, bool isReplaceable, ImageTypes imageTypes)
        {
            using (ImageVisualizerViewModel viewModel = ViewModelFactory.FromImageTypes(imageTypes))
            {
                viewModel.ReadOnly = !isReplaceable;

                if (imageInfo.Frames == null)
                    viewModel.InitFromSingleImage(imageInfo.MainImage, imageInfo.Icon);
                else
                    viewModel.InitFromFrames(imageInfo.MainImage, imageInfo.Frames, imageInfo.Icon);

                ViewFactory.ShowDialog(viewModel, null);
                if (isReplaceable && viewModel.IsModified)
                    return viewModel.GetImageReference();

                return null;
            }
        }

        #endregion

        #endregion
    }
}
