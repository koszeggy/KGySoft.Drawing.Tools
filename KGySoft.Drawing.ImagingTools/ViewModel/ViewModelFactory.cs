#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ViewModelFactory.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2024 - All Rights Reserved
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
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;

#if NETFRAMEWORK
using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;
#endif
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
        #region Constructors

        static ViewModelFactory()
        {
            Res.EnsureInitialized();
#if NETFRAMEWORK
            typeof(Version).RegisterTypeConverter<VersionConverter>();
#endif
        }

        #endregion

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
        public static IViewModel FromCommandLineArguments(string[]? args) => new DefaultViewModel { CommandLineArguments = args };

        /// <summary>
        /// Creates a view model from an <see cref="Image"/>.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="readOnly"><see langword="true"/>, to create a read-only instance; otherwise, <see langword="false"/>.</param>
        /// <returns>An <see cref="IViewModel{TModel}"/> instance that represents a view model for an <see cref="Image"/>.</returns>
        public static IViewModel<Image?> FromImage(Image? image, bool readOnly = false)
            => new ImageVisualizerViewModel { Image = (Image?)image?.Clone(), ReadOnly = readOnly };

        /// <summary>
        /// Creates a view model for an <see cref="Image"/> from arbitrary debug information.
        /// </summary>
        /// <param name="imageInfo">The debug information for an image.</param>
        /// <param name="readOnly"><see langword="true"/>, to create a read-only instance; otherwise, <see langword="false"/>.</param>
        /// <returns>An <see cref="IViewModel{TModel}"/> instance that represents a view model for an <see cref="Image"/>.</returns>
        public static IViewModel<ImageInfo> FromImage(ImageInfo? imageInfo, bool readOnly)
            => new ImageVisualizerViewModel { ImageInfo = imageInfo, ReadOnly = readOnly };

        /// <summary>
        /// Creates a view model from a <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="readOnly"><see langword="true"/>, to create a read-only instance; otherwise, <see langword="false"/>.</param>
        /// <returns>An <see cref="IViewModel{TModel}"/> instance that represents a view model for a <see cref="Bitmap"/>.</returns>
        public static IViewModel<Bitmap?> FromBitmap(Bitmap? bitmap, bool readOnly = false)
            => new ImageVisualizerViewModel(AllowedImageTypes.Bitmap | AllowedImageTypes.Icon) { Image = (Bitmap?)bitmap?.Clone(), ReadOnly = readOnly };

        /// <summary>
        /// Creates a view model for a <see cref="Bitmap"/> from arbitrary debug information.
        /// </summary>
        /// <param name="bitmapInfo">The debug information for a bitmap.</param>
        /// <param name="readOnly"><see langword="true"/>, to create a read-only instance; otherwise, <see langword="false"/>.</param>
        /// <returns>An <see cref="IViewModel{TModel}"/> instance that represents a view model for a <see cref="Bitmap"/>.</returns>
        public static IViewModel<ImageInfo> FromBitmap(ImageInfo? bitmapInfo, bool readOnly)
            => new ImageVisualizerViewModel(AllowedImageTypes.Bitmap | AllowedImageTypes.Icon) { ImageInfo = bitmapInfo, ReadOnly = readOnly };

        /// <summary>
        /// Creates a view model from a <see cref="Metafile"/>.
        /// </summary>
        /// <param name="metafile">The metafile.</param>
        /// <param name="readOnly"><see langword="true"/>, to create a read-only instance; otherwise, <see langword="false"/>.</param>
        /// <returns>An <see cref="IViewModel{TModel}"/> instance that represents a view model for a <see cref="Metafile"/>.</returns>
        public static IViewModel<Metafile?> FromMetafile(Metafile? metafile, bool readOnly = false)
            => new ImageVisualizerViewModel(AllowedImageTypes.Metafile) { Image = (Metafile?)metafile?.Clone(), ReadOnly = readOnly };

        /// <summary>
        /// Creates a view model for a <see cref="Metafile"/> from arbitrary debug information.
        /// </summary>
        /// <param name="metafileInfo">The debug information for a metafile.</param>
        /// <param name="readOnly"><see langword="true"/>, to create a read-only instance; otherwise, <see langword="false"/>.</param>
        /// <returns>An <see cref="IViewModel{TModel}"/> instance that represents a view model for a <see cref="Metafile"/>.</returns>
        public static IViewModel<ImageInfo> FromMetafile(ImageInfo? metafileInfo, bool readOnly)
            => new ImageVisualizerViewModel(AllowedImageTypes.Metafile) { ImageInfo = metafileInfo, ReadOnly = readOnly };

        /// <summary>
        /// Creates a view model from an <see cref="Icon"/>.
        /// </summary>
        /// <param name="icon">The icon.</param>
        /// <param name="readOnly"><see langword="true"/>, to create a read-only instance; otherwise, <see langword="false"/>.</param>
        /// <returns>An <see cref="IViewModel{TModel}"/> instance that represents a view model for an <see cref="Icon"/>.</returns>
        public static IViewModel<Icon?> FromIcon(Icon? icon, bool readOnly = false)
            => new ImageVisualizerViewModel(AllowedImageTypes.Icon) { Icon = (Icon?)icon?.Clone(), ReadOnly = readOnly };

        /// <summary>
        /// Creates a view model for an <see cref="Icon"/> from arbitrary debug information.
        /// </summary>
        /// <param name="iconInfo">The debug information for an icon.</param>
        /// <param name="readOnly"><see langword="true"/>, to create a read-only instance; otherwise, <see langword="false"/>.</param>
        /// <returns>An <see cref="IViewModel{TModel}"/> instance that represents a view model for an <see cref="Icon"/>.</returns>
        public static IViewModel<ImageInfo> FromIcon(ImageInfo? iconInfo, bool readOnly)
            => new ImageVisualizerViewModel(AllowedImageTypes.Icon) { ImageInfo = iconInfo, ReadOnly = readOnly };

        /// <summary>
        /// Creates a view model from a palette.
        /// </summary>
        /// <param name="palette">The palette.</param>
        /// <param name="readOnly"><see langword="true"/>, to create a read-only instance; otherwise, <see langword="false"/>.</param>
        /// <returns>An <see cref="IViewModel{TModel}"/> instance that represents a view model for a palette.</returns>
        public static IViewModel<Color[]> FromPalette(Color[] palette, bool readOnly) => new PaletteVisualizerViewModel { Palette = palette, ReadOnly = readOnly };

        /// <summary>
        /// Creates a view model from a palette.
        /// </summary>
        /// <param name="palette">The palette.</param>
        /// <param name="readOnly"><see langword="true"/>, to create a read-only instance; otherwise, <see langword="false"/>.</param>
        /// <returns>An <see cref="IViewModel{TModel}"/> instance that represents a view model for a palette.</returns>
        public static IViewModel<ColorPalette> FromPalette(ColorPalette palette, bool readOnly) => new ColorPaletteVisualizerViewModel(palette) { ReadOnly = readOnly };

        /// <summary>
        /// Creates a view model from a custom palette.
        /// </summary>
        /// <param name="customPaletteInfo">The debug information for a custom palette.</param>
        /// <returns>An <see cref="IViewModel"/> instance that represents a view model for a palette.</returns>
        public static IViewModel FromCustomPalette(CustomPaletteInfo customPaletteInfo)
            => new CustomPaletteVisualizerViewModel(customPaletteInfo ?? throw new ArgumentNullException(nameof(customPaletteInfo), PublicResources.ArgumentNull));

        /// <summary>
        /// Creates a view model from a <see cref="Color"/>.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="readOnly"><see langword="true"/>, to create a read-only instance; otherwise, <see langword="false"/>.</param>
        /// <returns>An <see cref="IViewModel{TModel}"/> instance that represents a view model for a <see cref="Color"/>.</returns>
        public static IViewModel<Color> FromColor(Color color, bool readOnly) => new ColorVisualizerViewModel { Color = color, ReadOnly = readOnly };

        /// <summary>
        /// Creates a view model for a <see cref="CustomColorInfo"/> from arbitrary debug information.
        /// </summary>
        /// <param name="customColorInfo">The debug information for a custom color.</param>
        /// <returns>An <see cref="IViewModel"/> instance that represents a view model for a <see cref="CustomColorInfo"/>.</returns>
        public static IViewModel FromCustomColor(CustomColorInfo? customColorInfo)
            => new CustomColorVisualizerViewModel(customColorInfo ?? throw new ArgumentNullException(nameof(customColorInfo), PublicResources.ArgumentNull));

        /// <summary>
        /// Creates a view model for managing debugger visualizer installations.
        /// </summary>
        /// <param name="hintPath">If the provided path is among the detected Visual Studio installations, then it will be preselected in the view. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        /// <returns>An <see cref="IViewModel"/> instance that represents a view model for managing debugger visualizer installations.</returns>
        public static IViewModel CreateManageInstallations(string? hintPath = null) => new ManageInstallationsViewModel(hintPath);

        /// <summary>
        /// Creates a view model from a <see cref="BitmapData"/>.
        /// </summary>
        /// <param name="bitmapData">The bitmap data.</param>
        /// <returns>An <see cref="IViewModel"/> instance that represents a view model for a <see cref="BitmapData"/>.</returns>
        public static IViewModel FromBitmapData(BitmapData? bitmapData)
            => new BitmapDataVisualizerViewModel { BitmapDataInfo = bitmapData == null ? null : new BitmapDataInfo(bitmapData) };

        /// <summary>
        /// Creates a view model for a <see cref="BitmapData"/> from arbitrary debug information.
        /// </summary>
        /// <param name="bitmapDataInfo">The debug information for a <see cref="BitmapData"/>.</param>
        /// <returns>An <see cref="IViewModel"/> instance that represents a view model for a <see cref="BitmapData"/>.</returns>
        public static IViewModel FromBitmapData(BitmapDataInfo? bitmapDataInfo) => new BitmapDataVisualizerViewModel { BitmapDataInfo = bitmapDataInfo };

        /// <summary>
        /// Creates a view model for a <see cref="CustomBitmapInfo"/> from arbitrary debug information.
        /// </summary>
        /// <param name="customBitmapInfo">The debug information for a custom bitmap.</param>
        /// <returns>An <see cref="IViewModel"/> instance that represents a view model for a <see cref="CustomBitmapInfo"/>.</returns>
        public static IViewModel FromCustomBitmap(CustomBitmapInfo customBitmapInfo)
            => new CustomBitmapVisualizerViewModel(customBitmapInfo ?? throw new ArgumentNullException(nameof(customBitmapInfo), PublicResources.ArgumentNull));

        /// <summary>
        /// Creates a view model from a <see cref="Graphics"/>.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <returns>An <see cref="IViewModel"/> instance that represents a view model for a <see cref="Graphics"/>.</returns>
        public static IViewModel FromGraphics(Graphics? graphics)
            => new GraphicsVisualizerViewModel { GraphicsInfo = graphics == null ? null : new GraphicsInfo(graphics) };

        /// <summary>
        /// Creates a view model for a <see cref="Graphics"/> from arbitrary debug information.
        /// </summary>
        /// <param name="graphicsInfo">The debug information for a <see cref="Graphics"/>.</param>
        /// <returns>An <see cref="IViewModel"/> instance that represents a view model for a <see cref="Graphics"/>.</returns>
        public static IViewModel<GraphicsInfo?> FromGraphics(GraphicsInfo? graphicsInfo) => new GraphicsVisualizerViewModel { GraphicsInfo = graphicsInfo };

        /// <summary>
        /// Creates a view model for counting colors of a <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="bitmap">The bitmap to count its colors.</param>
        /// <returns>An <see cref="IViewModel{TResult}"/> instance that represents a view model for counting colors of a <see cref="Bitmap"/>.</returns>
        public static IViewModel<int?> CreateCountColors(Bitmap bitmap) => new CountColorsViewModel(bitmap);

        /// <summary>
        /// Creates a view model for resizing a <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="bitmap">The bitmap to resize.</param>
        /// <returns>An <see cref="IViewModel{TResult}"/> instance that represents a view model for resizing a <see cref="Bitmap"/>.</returns>
        public static IViewModel<Bitmap?> CreateResizeBitmap(Bitmap bitmap) => new ResizeBitmapViewModel(bitmap);

        /// <summary>
        /// Creates a view model for adjusting <see cref="PixelFormat"/> of a <see cref="Bitmap"/> with quantizing and dithering.
        /// </summary>
        /// <param name="bitmap">The bitmap to transform.</param>
        /// <returns>An <see cref="IViewModel{TResult}"/> instance that represents a view model for adjusting the colors of a <see cref="Bitmap"/>.</returns>
        public static IViewModel<Bitmap?> CreateAdjustColorSpace(Bitmap bitmap) => new ColorSpaceViewModel(bitmap);

        /// <summary>
        /// Creates a view model for adjusting the brightness of a <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="bitmap">The bitmap to adjust.</param>
        /// <returns>An <see cref="IViewModel{TResult}"/> instance that represents a view model for adjusting the brightness of a <see cref="Bitmap"/>.</returns>
        public static IViewModel<Bitmap?> CreateAdjustBrightness(Bitmap bitmap) => new AdjustBrightnessViewModel(bitmap);

        /// <summary>
        /// Creates a view model for adjusting the contrast of a <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="bitmap">The bitmap to adjust.</param>
        /// <returns>An <see cref="IViewModel{TResult}"/> instance that represents a view model for adjusting the contrast of a <see cref="Bitmap"/>.</returns>
        public static IViewModel<Bitmap?> CreateAdjustContrast(Bitmap bitmap) => new AdjustContrastViewModel(bitmap);

        /// <summary>
        /// Creates a view model for adjusting the gamma of a <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="bitmap">The bitmap to adjust.</param>
        /// <returns>An <see cref="IViewModel{TResult}"/> instance that represents a view model for adjusting the gamma of a <see cref="Bitmap"/>.</returns>
        public static IViewModel<Bitmap?> CreateAdjustGamma(Bitmap bitmap) => new AdjustGammaViewModel(bitmap);

        /// <summary>
        /// Creates a view model for managing language settings.
        /// </summary>
        /// <returns>An <see cref="IViewModel"/> instance that represents a view model for managing language settings.</returns>
        public static IViewModel CreateLanguageSettings() => new LanguageSettingsViewModel();

        /// <summary>
        /// Creates a view model for managing language settings.
        /// </summary>
        /// <param name="culture">The language of the resources to edit.</param>
        /// <param name="hasPendingChanges"><see langword="true"/>, if there are pending changes that can be applied immediately; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="false"/>.</param>
        /// <returns>An <see cref="IViewModel"/> instance that represents a view model for managing language settings.</returns>
        public static IViewModel CreateEditResources(CultureInfo culture, bool hasPendingChanges = false) => new EditResourcesViewModel(culture, hasPendingChanges);

        /// <summary>
        /// Creates a view model for downloading resources.
        /// </summary>
        /// <returns>An <see cref="IViewModel{TResult}"/> instance that represents a view model for managing language settings.</returns>
        public static IViewModel<ICollection<LocalizationInfo>> CreateDownloadResources() => new DownloadResourcesViewModel();

        #endregion
    }
}