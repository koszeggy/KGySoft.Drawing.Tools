#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ViewModelFactory.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2020 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution. If not, then this file is considered as
//  an illegal copy.
//
//  Unauthorized copying of this file, via any medium is strictly prohibited.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    /// <summary>
    /// Represents a class that can create view model instances.
    /// </summary>
    public static class ViewModelFactory
    {
        #region Methods

        public static IViewModel CreateDefault() => new DefaultViewModel();
        public static IViewModel FromCommandLineArguments(string[] args) => new DefaultViewModel { CommandLineArguments = args };

        public static IViewModel<Image> FromImage(Image image, bool readOnly = false) => new ImageVisualizerViewModel { Image = (Image)image?.Clone(), ReadOnly = readOnly };
        public static IViewModel<ImageInfo> FromImage(ImageInfo imageInfo, bool readOnly) => new ImageVisualizerViewModel { ImageInfo = imageInfo, ReadOnly = readOnly };

        public static IViewModel<Bitmap> FromBitmap(Bitmap bitmap, bool readOnly = false) => new ImageVisualizerViewModel(AllowedImageTypes.Bitmap | AllowedImageTypes.Icon) { Image = (Bitmap)bitmap?.Clone(), ReadOnly = readOnly };
        public static IViewModel<ImageInfo> FromBitmap(ImageInfo bitmapInfo, bool readOnly) => new ImageVisualizerViewModel(AllowedImageTypes.Bitmap | AllowedImageTypes.Icon) { ImageInfo = bitmapInfo, ReadOnly = readOnly };

        public static IViewModel<Metafile> FromMetafile(Metafile metafile, bool readOnly = false) => new ImageVisualizerViewModel(AllowedImageTypes.Metafile) { Image = (Metafile)metafile?.Clone(), ReadOnly = readOnly };
        public static IViewModel<ImageInfo> FromMetafile(ImageInfo metafileInfo, bool readOnly) => new ImageVisualizerViewModel(AllowedImageTypes.Metafile) { ImageInfo = metafileInfo, ReadOnly = readOnly };

        public static IViewModel<Icon> FromIcon(Icon icon, bool readOnly = false) => new ImageVisualizerViewModel(AllowedImageTypes.Icon) { Icon = (Icon)icon?.Clone(), ReadOnly = readOnly };
        public static IViewModel<ImageInfo> FromIcon(ImageInfo iconInfo, bool readOnly) => new ImageVisualizerViewModel(AllowedImageTypes.Icon) { ImageInfo = iconInfo, ReadOnly = readOnly };

        public static IViewModel<Color[]> FromPalette(Color[] palette, bool isReadOnly) => new PaletteVisualizerViewModel { Palette = palette, ReadOnly = isReadOnly };
        public static IViewModel<Color> FromColor(Color color, bool isReadOnly) => new ColorVisualizerViewModel { Color = color, ReadOnly = isReadOnly };

        public static IViewModel CreateManageInstallations(string hintPath) => new ManageInstallationsViewModel(hintPath);

        public static IViewModel FromBitmapData(BitmapData bitmapData) => new BitmapDataVisualizerViewModel { BitmapDataInfo = new BitmapDataInfo(bitmapData) };
        public static IViewModel FromBitmapData(BitmapDataInfo bitmapDataInfo) => new BitmapDataVisualizerViewModel { BitmapDataInfo = bitmapDataInfo };

        public static IViewModel FromGraphics(Graphics graphics) => new GraphicsVisualizerViewModel { GraphicsInfo = new GraphicsInfo(graphics)};
        public static IViewModel FromGraphics(GraphicsInfo graphicsInfo) => new GraphicsVisualizerViewModel { GraphicsInfo = graphicsInfo };

        #endregion
    }
}