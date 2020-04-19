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
using System.Drawing.Imaging;

using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    /// <summary>
    /// Represents a class that can create view model instances.
    /// They can be used to create a view by the <see cref="View.ViewFactory"/> class.
    /// </summary>
    public static class ViewModelFactory
    {
        #region Methods

        /// <summary>
        /// Creates a view model for the default view without any loaded image.
        /// </summary>
        /// <returns>An <see cref="IViewModel"/> instance that represents a view model for the default view without any loaded image</returns>
        public static IViewModel CreateDefault() => new DefaultViewModel();

        /// <summary>
        /// Creates a view model for command line arguments.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>An <see cref="IViewModel"/> instance that represents a view model for the specified command line arguments.</returns>
        public static IViewModel FromCommandLineArguments(string[] args) => new DefaultViewModel { CommandLineArguments = args };

        /// <summary>
        /// Creates a view model from an <see cref="Image"/>.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="readOnly"><see langword="true"/>, to create a read-only instance; otherwise, <see langword="false"/>.</param>
        /// <returns>An <see cref="IViewModel{TModel}"/> instance that represents a view model for an <see cref="Image"/>.</returns>
        public static IViewModel<Image> FromImage(Image image, bool readOnly = false) => new ImageVisualizerViewModel { Image = (Image)image?.Clone(), ReadOnly = readOnly };

        /// <summary>
        /// Creates a view model for an <see cref="Image"/> from arbitrary debug information.
        /// </summary>
        /// <param name="imageInfo">The debug information for an image.</param>
        /// <param name="readOnly"><see langword="true"/>, to create a read-only instance; otherwise, <see langword="false"/>.</param>
        /// <returns>An <see cref="IViewModel{TModel}"/> instance that represents a view model for an <see cref="Image"/>.</returns>
        public static IViewModel<ImageInfo> FromImage(ImageInfo imageInfo, bool readOnly) => new ImageVisualizerViewModel { ImageInfo = imageInfo, ReadOnly = readOnly };

        /// <summary>
        /// Creates a view model from a <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="readOnly"><see langword="true"/>, to create a read-only instance; otherwise, <see langword="false"/>.</param>
        /// <returns>An <see cref="IViewModel{TModel}"/> instance that represents a view model for a <see cref="Bitmap"/>.</returns>
        public static IViewModel<Bitmap> FromBitmap(Bitmap bitmap, bool readOnly = false) => new ImageVisualizerViewModel(AllowedImageTypes.Bitmap | AllowedImageTypes.Icon) { Image = (Bitmap)bitmap?.Clone(), ReadOnly = readOnly };

        /// <summary>
        /// Creates a view model for a <see cref="Bitmap"/> from arbitrary debug information.
        /// </summary>
        /// <param name="bitmapInfo">The debug information for a bitmap.</param>
        /// <param name="readOnly"><see langword="true"/>, to create a read-only instance; otherwise, <see langword="false"/>.</param>
        /// <returns>An <see cref="IViewModel{TModel}"/> instance that represents a view model for a <see cref="Bitmap"/>.</returns>
        public static IViewModel<ImageInfo> FromBitmap(ImageInfo bitmapInfo, bool readOnly) => new ImageVisualizerViewModel(AllowedImageTypes.Bitmap | AllowedImageTypes.Icon) { ImageInfo = bitmapInfo, ReadOnly = readOnly };

        /// <summary>
        /// Creates a view model from a <see cref="Metafile"/>.
        /// </summary>
        /// <param name="metafile">The metafile.</param>
        /// <param name="readOnly"><see langword="true"/>, to create a read-only instance; otherwise, <see langword="false"/>.</param>
        /// <returns>An <see cref="IViewModel{TModel}"/> instance that represents a view model for a <see cref="Metafile"/>.</returns>
        public static IViewModel<Metafile> FromMetafile(Metafile metafile, bool readOnly = false) => new ImageVisualizerViewModel(AllowedImageTypes.Metafile) { Image = (Metafile)metafile?.Clone(), ReadOnly = readOnly };

        /// <summary>
        /// Creates a view model for a <see cref="Metafile"/> from arbitrary debug information.
        /// </summary>
        /// <param name="metafileInfo">The debug information for a metafile.</param>
        /// <param name="readOnly"><see langword="true"/>, to create a read-only instance; otherwise, <see langword="false"/>.</param>
        /// <returns>An <see cref="IViewModel{TModel}"/> instance that represents a view model for a <see cref="Metafile"/>.</returns>
        public static IViewModel<ImageInfo> FromMetafile(ImageInfo metafileInfo, bool readOnly) => new ImageVisualizerViewModel(AllowedImageTypes.Metafile) { ImageInfo = metafileInfo, ReadOnly = readOnly };

        /// <summary>
        /// Creates a view model from an <see cref="Icon"/>.
        /// </summary>
        /// <param name="icon">The icon.</param>
        /// <param name="readOnly"><see langword="true"/>, to create a read-only instance; otherwise, <see langword="false"/>.</param>
        /// <returns>An <see cref="IViewModel{TModel}"/> instance that represents a view model for an <see cref="Icon"/>.</returns>
        public static IViewModel<Icon> FromIcon(Icon icon, bool readOnly = false) => new ImageVisualizerViewModel(AllowedImageTypes.Icon) { Icon = (Icon)icon?.Clone(), ReadOnly = readOnly };

        /// <summary>
        /// Creates a view model for an <see cref="Icon"/> from arbitrary debug information.
        /// </summary>
        /// <param name="iconInfo">The debug information for an icon.</param>
        /// <param name="readOnly"><see langword="true"/>, to create a read-only instance; otherwise, <see langword="false"/>.</param>
        /// <returns>An <see cref="IViewModel{TModel}"/> instance that represents a view model for an <see cref="Icon"/>.</returns>
        public static IViewModel<ImageInfo> FromIcon(ImageInfo iconInfo, bool readOnly) => new ImageVisualizerViewModel(AllowedImageTypes.Icon) { ImageInfo = iconInfo, ReadOnly = readOnly };

        /// <summary>
        /// Creates a view model from a palette.
        /// </summary>
        /// <param name="palette">The palette.</param>
        /// <param name="readOnly"><see langword="true"/>, to create a read-only instance; otherwise, <see langword="false"/>.</param>
        /// <returns>An <see cref="IViewModel{TModel}"/> instance that represents a view model for a palette.</returns>
        public static IViewModel<Color[]> FromPalette(Color[] palette, bool readOnly) => new PaletteVisualizerViewModel { Palette = palette, ReadOnly = readOnly };

        /// <summary>
        /// Creates a view model from a <see cref="Color"/>.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="readOnly"><see langword="true"/>, to create a read-only instance; otherwise, <see langword="false"/>.</param>
        /// <returns>An <see cref="IViewModel{TModel}"/> instance that represents a view model for a <see cref="Color"/>.</returns>
        public static IViewModel<Color> FromColor(Color color, bool readOnly) => new ColorVisualizerViewModel { Color = color, ReadOnly = readOnly };

        /// <summary>
        /// Creates a view model for managing debugger visualizer installations.
        /// </summary>
        /// <param name="hintPath">If the provided path is among the detected Visual Studio installations, then it will be preselected in the view. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        /// <returns>An <see cref="IViewModel"/> instance that represents a view model for managing debugger visualizer installations.</returns>
        public static IViewModel CreateManageInstallations(string hintPath = null) => new ManageInstallationsViewModel(hintPath);

        /// <summary>
        /// Creates a view model from a <see cref="BitmapData"/>.
        /// </summary>
        /// <param name="bitmapData">The bitmap data.</param>
        /// <returns>An <see cref="IViewModel"/> instance that represents a view model for a <see cref="BitmapData"/>.</returns>
        public static IViewModel FromBitmapData(BitmapData bitmapData) => new BitmapDataVisualizerViewModel { BitmapDataInfo = new BitmapDataInfo(bitmapData) };

        /// <summary>
        /// Creates a view model for a <see cref="BitmapData"/> from arbitrary debug information.
        /// </summary>
        /// <param name="bitmapDataInfo">The debug information for a <see cref="BitmapData"/>.</param>
        /// <returns>An <see cref="IViewModel"/> instance that represents a view model for a <see cref="BitmapData"/>.</returns>
        public static IViewModel FromBitmapData(BitmapDataInfo bitmapDataInfo) => new BitmapDataVisualizerViewModel { BitmapDataInfo = bitmapDataInfo };

        /// <summary>
        /// Creates a view model from a <see cref="Graphics"/>.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <returns>An <see cref="IViewModel"/> instance that represents a view model for a <see cref="Graphics"/>.</returns>
        public static IViewModel FromGraphics(Graphics graphics) => new GraphicsVisualizerViewModel { GraphicsInfo = new GraphicsInfo(graphics)};

        /// <summary>
        /// Creates a view model for a <see cref="Graphics"/> from arbitrary debug information.
        /// </summary>
        /// <param name="graphicsInfo">The debug information for a <see cref="Graphics"/>.</param>
        /// <returns>An <see cref="IViewModel"/> instance that represents a view model for a <see cref="Graphics"/>.</returns>
        public static IViewModel FromGraphics(GraphicsInfo graphicsInfo) => new GraphicsVisualizerViewModel { GraphicsInfo = graphicsInfo };

        #endregion
    }
}