﻿#region Copyright

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
using System.Drawing.Imaging;

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

        /// <summary>
        /// Shows a debugger dialog for the specified <paramref name="image"/>.
        /// </summary>
        /// <param name="image">The image to debug.</param>
        /// <param name="isReplaceable"><see langword="true"/>, if the debugged instance can be replaced or edited; otherwise, <see langword="false"/>.</param>
        /// <param name="ownerWindowHandle">If specified, then the created dialog will be owned by the window that has specified handle. This parameter is optional.
        /// <br/>Default value: <see cref="IntPtr.Zero">IntPtr.Zero</see>.</param>
        /// <returns>An <see cref="Image"/> that is returned by the debugger. If <paramref name="isReplaceable"/> is <see langword="false"/>, then this will be always the original <paramref name="image"/>.</returns>
        public static Image? DebugImage(Image? image, bool isReplaceable = true, IntPtr ownerWindowHandle = default)
        {
            using (IViewModel<Image?> vm = ViewModelFactory.FromImage(image, !isReplaceable))
            {
                ViewFactory.ShowDialog(vm, ownerWindowHandle);
                return vm.IsModified ? vm.GetEditedModel() : image;
            }
        }

        /// <summary>
        /// Shows a debugger dialog for the specified <paramref name="bitmap"/>.
        /// </summary>
        /// <param name="bitmap">The bitmap to debug.</param>
        /// <param name="isReplaceable"><see langword="true"/>, if the debugged instance can be replaced or edited; otherwise, <see langword="false"/>.</param>
        /// <param name="ownerWindowHandle">If specified, then the created dialog will be owned by the window that has specified handle. This parameter is optional.
        /// <br/>Default value: <see cref="IntPtr.Zero">IntPtr.Zero</see>.</param>
        /// <returns>A <see cref="Bitmap"/> that is returned by the debugger. If <paramref name="isReplaceable"/> is <see langword="false"/>, then this will be always the original <paramref name="bitmap"/>.</returns>
        public static Bitmap? DebugBitmap(Bitmap? bitmap, bool isReplaceable = true, IntPtr ownerWindowHandle = default)
        {
            using (IViewModel<Bitmap?> vm = ViewModelFactory.FromBitmap(bitmap, !isReplaceable))
            {
                ViewFactory.ShowDialog(vm, ownerWindowHandle);
                return vm.IsModified ? vm.GetEditedModel() : bitmap;
            }
        }

        /// <summary>
        /// Shows a debugger dialog for the specified <paramref name="metafile"/>.
        /// </summary>
        /// <param name="metafile">The metafile to debug.</param>
        /// <param name="isReplaceable"><see langword="true"/>, if the debugged instance can be replaced or edited; otherwise, <see langword="false"/>.</param>
        /// <param name="ownerWindowHandle">If specified, then the created dialog will be owned by the window that has specified handle. This parameter is optional.
        /// <br/>Default value: <see cref="IntPtr.Zero">IntPtr.Zero</see>.</param>
        /// <returns>A <see cref="Metafile"/> that is returned by the debugger. If <paramref name="isReplaceable"/> is <see langword="false"/>, then this will be always the original <paramref name="metafile"/>.</returns>
        public static Metafile? DebugMetafile(Metafile? metafile, bool isReplaceable = true, IntPtr ownerWindowHandle = default)
        {
            using (IViewModel<Metafile?> vm = ViewModelFactory.FromMetafile(metafile, !isReplaceable))
            {
                ViewFactory.ShowDialog(vm, ownerWindowHandle);
                return vm.IsModified ? vm.GetEditedModel() : metafile;
            }
        }

        /// <summary>
        /// Shows a debugger dialog for the specified <paramref name="icon"/>.
        /// </summary>
        /// <param name="icon">The icon to debug.</param>
        /// <param name="isReplaceable"><see langword="true"/>, if the debugged instance can be replaced or edited; otherwise, <see langword="false"/>.</param>
        /// <param name="ownerWindowHandle">If specified, then the created dialog will be owned by the window that has specified handle. This parameter is optional.
        /// <br/>Default value: <see cref="IntPtr.Zero">IntPtr.Zero</see>.</param>
        /// <returns>An <see cref="Icon"/> that is returned by the debugger. If <paramref name="isReplaceable"/> is <see langword="false"/>, then this will be always the original <paramref name="icon"/>.</returns>
        public static Icon? DebugIcon(Icon? icon, bool isReplaceable = true, IntPtr ownerWindowHandle = default)
        {
            using (IViewModel<Icon?> vm = ViewModelFactory.FromIcon(icon, !isReplaceable))
            {
                ViewFactory.ShowDialog(vm, ownerWindowHandle);
                return vm.IsModified ? vm.GetEditedModel() : icon;
            }
        }

        /// <summary>
        /// Shows a debugger dialog for the specified <paramref name="bitmapData"/>.
        /// </summary>
        /// <param name="bitmapData">The bitmap data to debug.</param>
        /// <param name="ownerWindowHandle">If specified, then the created dialog will be owned by the window that has specified handle. This parameter is optional.
        /// <br/>Default value: <see cref="IntPtr.Zero">IntPtr.Zero</see>.</param>
        public static void DebugBitmapData(BitmapData bitmapData, IntPtr ownerWindowHandle = default)
        {
            if (bitmapData == null)
                throw new ArgumentNullException(nameof(bitmapData), PublicResources.ArgumentNull);
            using (IViewModel vm = ViewModelFactory.FromBitmapData(bitmapData))
                ViewFactory.ShowDialog(vm, ownerWindowHandle);
        }

        /// <summary>
        /// Shows a debugger dialog for the specified <paramref name="graphics"/>.
        /// </summary>
        /// <param name="graphics">The <see cref="Graphics"/> instance to debug.</param>
        /// <param name="ownerWindowHandle">If specified, then the created dialog will be owned by the window that has specified handle. This parameter is optional.
        /// <br/>Default value: <see cref="IntPtr.Zero">IntPtr.Zero</see>.</param>
        public static void DebugGraphics(Graphics graphics, IntPtr ownerWindowHandle = default)
        {
            if (graphics == null)
                throw new ArgumentNullException(nameof(graphics), PublicResources.ArgumentNull);
            using (IViewModel vm = ViewModelFactory.FromGraphics(graphics))
                ViewFactory.ShowDialog(vm, ownerWindowHandle);
        }

        /// <summary>
        /// Shows the debugger for a <see cref="ColorPalette"/> instance.
        /// </summary>
        /// <param name="palette">The palette object to debug.</param>
        /// <param name="isReplaceable">Indicates whether the palette is replaceable.</param>
        /// <param name="ownerWindowHandle">If specified, then the created dialog will be owned by the window that has specified handle. This parameter is optional.
        /// <br/>Default value: <see cref="IntPtr.Zero">IntPtr.Zero</see>.</param>
        /// <returns>A non-<see langword="null"/>&#160;instance, when the palette has been edited; otherwise, <see langword="null"/>.</returns>
        public static ColorPalette? DebugPalette(ColorPalette palette, bool isReplaceable, IntPtr ownerWindowHandle = default)
        {
            if (palette == null)
                throw new ArgumentNullException(nameof(palette), PublicResources.ArgumentNull);
            Color[] entries = palette.Entries;
            using (IViewModel<Color[]> vm = ViewModelFactory.FromPalette(entries, !isReplaceable))
            {
                ViewFactory.ShowDialog(vm, ownerWindowHandle);
                if (!isReplaceable || !vm.IsModified)
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
        /// <param name="ownerWindowHandle">If specified, then the created dialog will be owned by the window that has specified handle. This parameter is optional.
        /// <br/>Default value: <see cref="IntPtr.Zero">IntPtr.Zero</see>.</param>
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
