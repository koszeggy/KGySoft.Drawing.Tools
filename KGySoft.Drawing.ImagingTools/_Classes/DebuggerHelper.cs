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
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

using KGySoft.Drawing.ImagingTools.Forms;
using KGySoft.Serialization;

#endregion

namespace KGySoft.Drawing.ImagingTools
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
        internal static object DebugImage(object[] imageInfo, bool isReplaceable) => DebugImage(imageInfo, isReplaceable, new ImageDebuggerVisualizerForm());

        /// <summary>
        /// Shows the debugger for a <see cref="Bitmap"/> or <see cref="Icon"/> object.
        /// </summary>
        /// <param name="imageInfo">The image infos for debugging returned by <see cref="SerializationHelper.DeserializeImage"/></param>
        /// <param name="isReplaceable">Indicates whether the image is replaceable.</param>
        /// <returns>A non-<see langword="null"/>&#160;instance, when the image has been edited and should be serialized back; otherwise, <see langword="null"/>.</returns>
        internal static object DebugBitmap(object[] imageInfo, bool isReplaceable)
        {
            ImageDebuggerVisualizerForm form = new ImageDebuggerVisualizerForm { ImageTypes = ImageTypes.Bitmap | ImageTypes.Icon };
            return DebugImage(imageInfo, isReplaceable, form);
        }

        /// <summary>
        /// Shows the debugger for a <see cref="Metafile"/>.
        /// </summary>
        /// <param name="imageInfo">The image infos for debugging returned by <see cref="SerializationHelper.DeserializeImage"/></param>
        /// <param name="isReplaceable">Indicates whether the image is replaceable.</param>
        /// <returns>A non-<see langword="null"/>&#160;instance, when the image has been edited and should be serialized back; otherwise, <see langword="null"/>.</returns>
        internal static object DebugMetafile(object[] imageInfo, bool isReplaceable)
        {
            ImageDebuggerVisualizerForm form = new ImageDebuggerVisualizerForm { ImageTypes = ImageTypes.Metafile };
            return DebugImage(imageInfo, isReplaceable, form);
        }

        /// <summary>
        /// Shows the debugger for an <see cref="Icon"/> object.
        /// </summary>
        /// <param name="imageInfo">The image infos for debugging returned by <see cref="SerializationHelper.DeserializeImage"/></param>
        /// <param name="isReplaceable">Indicates whether the image is replaceable.</param>
        /// <returns>A non-<see langword="null"/>&#160;instance, when the image has been edited and should be serialized back; otherwise, <see langword="null"/>.</returns>
        internal static object DebugIcon(object[] imageInfo, bool isReplaceable)
        {
            ImageDebuggerVisualizerForm form = new ImageDebuggerVisualizerForm { ImageTypes = ImageTypes.Icon };
            return DebugImage(imageInfo, isReplaceable, form);
        }

        /// <summary>
        /// Shows the debugger for a <see cref="BitmapData"/> object.
        /// </summary>
        /// <param name="bitmapDataInfo">The bitmap data infos for debugging returned by <see cref="SerializationHelper.DeserializeBitmapData"/>.</param>
        internal static void DebugBitmapData(object[] bitmapDataInfo)
        {
            if (bitmapDataInfo == null)
                throw new ArgumentNullException(nameof(bitmapDataInfo));
            if (bitmapDataInfo.Length != 2)
                throw new ArgumentException("2 elements are expected", nameof(bitmapDataInfo));

            ImageData imageData = (ImageData)bitmapDataInfo[0];
            string specialInfo = (string)bitmapDataInfo[1];
            using (BitmapDataDebuggerVisualizerForm frm = new BitmapDataDebuggerVisualizerForm())
            {
                frm.ReadOnly = true;
                frm.SpecialInfo = specialInfo;
                frm.InitFromSingleImage(imageData, null);
                frm.ShowDialog();
            }
        }

        /// <summary>
        /// Shows the debugger for a <see cref="Graphics"/> object.
        /// </summary>
        /// <param name="graphicsInfo">The graphics infos for debugging returned by <see cref="SerializationHelper.DeserializeGraphics"/>.</param>
        internal static void DebugGraphics(object[] graphicsInfo)
        {
            if (graphicsInfo == null)
                throw new ArgumentNullException(nameof(graphicsInfo));
            if (graphicsInfo.Length != 4)
                throw new ArgumentException("4 elements are expected", nameof(graphicsInfo));

            Bitmap bmp = (Bitmap)graphicsInfo[0];
            float[] elements = (float[])graphicsInfo[1];
            Rectangle visibleRect = (Rectangle)graphicsInfo[2];
            string specialInfo = (string)graphicsInfo[3];
            using (GraphicsDebuggerVisualizerForm frm = new GraphicsDebuggerVisualizerForm())
            {
                frm.ReadOnly = true;
                frm.Transform = new Matrix(elements[0], elements[1], elements[2], elements[3], elements[4], elements[5]);
                frm.VisibleRect = visibleRect;
                frm.SpecialInfo = specialInfo;
                frm.Image = bmp;
                frm.ShowDialog();
            }
        }

        /// <summary>
        /// Shows the debugger for a <see cref="ColorPalette"/> instance or any <see cref="IList{T}"/> collection with <see cref="Color"/> element.
        /// </summary>
        /// <param name="obj">The palette object to debug returned by <see cref="SerializationHelper.DeserializeAnyObject"/>.</param>
        /// <param name="isReplaceable">Indicates whether the palette is replaceable.</param>
        /// <returns>A non-<see langword="null"/>&#160;instance, when the palette has been edited and should be serialized back; otherwise, <see langword="null"/>.</returns>
        internal static object DebugPalette(object obj, bool isReplaceable)
        {
            using (PaletteVisualizerForm frm = new PaletteVisualizerForm())
            {
                ColorPalette palette = obj as ColorPalette;
                IList<Color> colorList = palette?.Entries ?? obj as IList<Color>;
                if (colorList == null)
                    throw new ArgumentException("Object is not a color list", nameof(obj));

                if (colorList.Count == 0)
                {
                    Dialogs.InfoMessage("The palette contains no colors. Click OK to exit.");
                    return null;
                }

                frm.Palette = isReplaceable ? colorList : new ReadOnlyCollection<Color>(colorList);
                frm.ShowDialog();

                if (isReplaceable && frm.PaletteChanged)
                {
                    if (palette != null)
                        return new AnyObjectSerializerWrapper(palette, true);
                    else
                        return new AnyObjectSerializerWrapper(colorList, true);
                }

                return null;
            }
        }

        /// <summary>
        /// Shows the debugger for a <see cref="Color"/> instance.
        /// </summary>
        /// <param name="obj">The color object to debug.</param>
        /// <param name="isReplaceable">Indicates whether the color is replaceable.</param>
        /// <returns>A non-<see langword="null"/>&#160;instance, when the color has been edited and should be serialized back; otherwise, <see langword="null"/>.</returns>
        internal static object DebugColor(object obj, bool isReplaceable)
        {
            if (!(obj is Color))
                throw new ArgumentException("Object is not a Color", nameof(obj));

            using (ColorVisualizerForm frm = new ColorVisualizerForm())
            {
                frm.ReadOnly = !isReplaceable;
                frm.Color = (Color)obj;
                frm.ShowDialog();
                if (isReplaceable && frm.ColorChanged)
                    return frm.Color;
            }

            return null;
        }

        #endregion

        #region Private Methods

        private static object DebugImage(object[] imageInfo, bool isReplaceable, ImageDebuggerVisualizerForm form)
        {
            using (form)
            {
                if (imageInfo == null)
                    throw new ArgumentNullException(nameof(imageInfo));
                if (imageInfo.Length != 3)
                    throw new ArgumentException("3 elements are expected", nameof(imageInfo));

                Icon icon = (Icon)imageInfo[0];
                ImageData mainImage = (ImageData)imageInfo[1];
                ImageData[] frames = (ImageData[])imageInfo[2];
                form.ReadOnly = !isReplaceable;

                if (frames == null)
                    form.InitFromSingleImage(mainImage, icon);
                else
                    form.InitFromFrames(mainImage, frames, icon);

                form.ShowDialog();
                if (isReplaceable && form.IsModified)
                    return form.GetImageReference();

                return null;
            }
        }

        #endregion

        #endregion
    }
}
