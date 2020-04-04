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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

using KGySoft.Drawing.DebuggerVisualizers.Model;
using KGySoft.Drawing.DebuggerVisualizers.Serializers;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Drawing.ImagingTools.View;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers
{
    /// <summary>
    /// Provides debugger methods for debugger visualizers
    /// </summary>
    public static class DebuggerHelper
    {
        #region Methods

        #region Public Methods

        public static Image DebugImage(Image image, bool isReplaceable = true, IntPtr ownerWindowHandle = default)
        {
            using (IViewModel<Image> vm = ViewModelFactory.FromImage(image, !isReplaceable))
            {
                ViewFactory.ShowDialog(vm, ownerWindowHandle);
                return vm.IsModified ? vm.GetEditedModel() : image;
            }
        }

        public static Bitmap DebugBitmap(Bitmap bitmap, bool isReplaceable = true, IntPtr ownerWindowHandle = default)
        {
            using (IViewModel<Image> vm = ViewModelFactory.FromBitmap(bitmap, !isReplaceable))
            {
                ViewFactory.ShowDialog(vm, ownerWindowHandle);
                return vm.IsModified ? vm.GetEditedModel() as Bitmap : null;
            }
        }

        public static Metafile DebugMetafile(Metafile metafile, bool isReplaceable = true, IntPtr ownerWindowHandle = default)
        {
            using (IViewModel<Image> vm = ViewModelFactory.FromMetafile(metafile, !isReplaceable))
            {
                ViewFactory.ShowDialog(vm, ownerWindowHandle);
                return vm.IsModified ? vm.GetEditedModel() as Metafile : null;
            }
        }

        public static Icon DebugIcon(Icon icon, bool isReplaceable = true, IntPtr ownerWindowHandle = default)
        {
            using (IViewModel<Icon> vm = ViewModelFactory.FromIcon(icon, !isReplaceable))
            {
                ViewFactory.ShowDialog(vm, ownerWindowHandle);
                return vm.IsModified ? vm.GetEditedModel() : null;
            }
        }

        /// <summary>
        /// Shows the debugger for a <see cref="ColorPalette"/> instance.
        /// </summary>
        /// <param name="palette">The palette object to debug.</param>
        /// <param name="isReplaceable">Indicates whether the palette is replaceable.</param>
        /// <returns>A non-<see langword="null"/>&#160;instance, when the palette has been edited; otherwise, <see langword="null"/>.</returns>
        public static ColorPalette DebugPalette(ColorPalette palette, bool isReplaceable, IntPtr ownerWindowHandle = default)
        {
            Color[] entries = palette.Entries;
            if (entries.Length == 0)
                return null;
            using (IViewModel<Color[]> vm = ViewModelFactory.FromPalette(entries, !isReplaceable))
            {
                ViewFactory.ShowDialog(vm, ownerWindowHandle);
                if (!isReplaceable)
                    return null;
                Color[] result = vm.GetEditedModel();

                // TODO: if length can change use the code from OnShowPaletteCommand
                Debug.Assert(result.Length == entries.Length, "Palette length is not expected to be changed");
                result.CopyTo(palette.Entries, 0);
                return palette;
            }
        }

        /// <summary>
        /// Shows the debugger for a <see cref="Color"/> instance.
        /// </summary>
        /// <param name="color">The color object to debug.</param>
        /// <param name="isReplaceable">Indicates whether the color is replaceable.</param>
        /// <returns>A non-<see langword="null"/>&#160;instance, when the color has been edited and should be serialized back; otherwise, <see langword="null"/>.</returns>
        public static Color? DebugColor(Color color, bool isReplaceable = true, IntPtr ownerWindowHandle = default)
        {
            using (IViewModel<Color> vm = ViewModelFactory.FromColor(color, !isReplaceable))
            {
                ViewFactory.ShowDialog(vm, ownerWindowHandle);
                if (isReplaceable && vm.IsModified)
                    return vm.GetEditedModel();
            }

            return null;
        }

        #endregion

        #region Internal Methods

        internal static ImageReference DebugImage(ImageInfo imageInfo, bool isReplaceable)
        {
            using (IViewModel<ImageInfo> viewModel = ViewModelFactory.FromImage(imageInfo, !isReplaceable))
                return DebugImageInfo(viewModel, isReplaceable);
        }

        internal static ImageReference DebugBitmap(ImageInfo bitmapInfo, bool isReplaceable)
        {
            using (IViewModel<ImageInfo> viewModel = ViewModelFactory.FromBitmap(bitmapInfo, !isReplaceable))
                return DebugImageInfo(viewModel, isReplaceable);
        }

        internal static ImageReference DebugMetafile(ImageInfo metafileInfo, bool isReplaceable)
        {
            using (IViewModel<ImageInfo> viewModel = ViewModelFactory.FromMetafile(metafileInfo, !isReplaceable))
                return DebugImageInfo(viewModel, isReplaceable);
        }

        internal static ImageReference DebugIcon(ImageInfo iconInfo, bool isReplaceable)
        {
            using (IViewModel<ImageInfo> viewModel = ViewModelFactory.FromIcon(iconInfo, !isReplaceable))
                return DebugImageInfo(viewModel, isReplaceable);
        }

        /// <summary>
        /// Shows the debugger for a <see cref="BitmapData"/> object.
        /// </summary>
        /// <param name="bitmapDataInfo">The bitmap data infos for debugging returned by <see cref="SerializationHelper.DeserializeBitmapDataInfo"/>.</param>
        internal static void DebugBitmapData(BitmapDataInfo bitmapDataInfo)
        {
            using (IViewModel vm = ViewModelFactory.FromBitmapData(bitmapDataInfo.Data, bitmapDataInfo.SpecialInfo))
                ViewFactory.ShowDialog(vm);
        }

        /// <summary>
        /// Shows the debugger for a <see cref="Graphics"/> object.
        /// </summary>
        /// <param name="graphicsInfo">The graphics infos for debugging returned by <see cref="SerializationHelper.DeserializeGraphicsInfo"/>.</param>
        internal static void DebugGraphics(GraphicsInfo graphicsInfo)
        {
            float[] elements = graphicsInfo.Elements;
            using var matrix = new Matrix(elements[0], elements[1], elements[2], elements[3], elements[4], elements[5]);
            using (IViewModel vm = ViewModelFactory.FromGraphics(graphicsInfo.Data, matrix, graphicsInfo.VisibleRect, graphicsInfo.SpecialInfo))
                ViewFactory.ShowDialog(vm);
        }

        #endregion

        #region Private Methods

        private static ImageReference DebugImageInfo(IViewModel<ImageInfo> viewModel, bool isReplaceable)
        {
            ViewFactory.ShowDialog(viewModel);
            if (isReplaceable && viewModel.IsModified)
                return new ImageReference(viewModel.GetEditedModel());

            return null;
        }

        #endregion

        #endregion
    }
}
