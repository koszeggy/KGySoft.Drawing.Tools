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
using KGySoft.CoreLibraries;
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

        #region Public Methods

        public static IViewModel CreateDefault() => new DefaultViewModel();
        public static IViewModel FromCommandLineArguments(string[] args) => new DefaultViewModel { CommandLineArguments = args };

        public static IViewModel<Image> FromImage(Image image, bool readOnly = false) => new ImageVisualizerViewModel { Image = image, ReadOnly = readOnly };
        public static IViewModel<Bitmap> FromBitmap(Bitmap bitmap, bool readOnly = false) => new ImageVisualizerViewModel { Image = bitmap, ReadOnly = readOnly, ImageTypes = ImageTypes.Bitmap | ImageTypes.Icon };
        public static IViewModel<Metafile> FromMetafile(Metafile metafile, bool readOnly = false) => new ImageVisualizerViewModel { Image = metafile, ReadOnly = readOnly, ImageTypes = ImageTypes.Metafile };
        public static IViewModel<Icon> FromIcon(Icon icon, bool readOnly = false) => new ImageVisualizerViewModel { Icon = icon, ReadOnly = readOnly, ImageTypes = ImageTypes.Icon };
        public static IViewModel<Color[]> FromPalette(Color[] palette, bool isReadOnly) => new PaletteVisualizerViewModel { Palette = palette, ReadOnly = isReadOnly };
        public static IViewModel<Color> FromColor(Color color, bool isReadOnly) => new ColorVisualizerViewModel { Color = color, ReadOnly = isReadOnly };

        public static IViewModel CreateManageInstallations(string hintPath) => new ManageInstallationsViewModel(hintPath);

        #endregion

        #region Internal Methods

        internal static IViewModel<ImageReference> FromImageData(ImageTypes imageTypes, bool isReadOnly, ImageData mainImage, params ImageData[] frames)
        {
            var result = new ImageVisualizerViewModel { ImageTypes = imageTypes, ReadOnly = isReadOnly };
            if (frames.IsNullOrEmpty())
                result.InitFromSingleImage(mainImage, null);
            else
                result.InitFromFrames(mainImage, frames, null);
            return result;
        }

        internal static IViewModel<ImageReference> FromIconData(bool isReadOnly, Icon underlyingIcon, ImageData compoundIcon, params ImageData[] iconImages)
        {
            var result = new ImageVisualizerViewModel { ImageTypes = ImageTypes.Icon, ReadOnly = isReadOnly };
            if (iconImages.IsNullOrEmpty())
                result.InitFromSingleImage(compoundIcon, underlyingIcon);
            else
                result.InitFromFrames(compoundIcon, iconImages, underlyingIcon);
            return result;
        }

        internal static IViewModel FromBitmapData(ImageData data, string info)
        {
            var result = new BitmapDataVisualizerViewModel { InfoText = info };
            result.InitFromSingleImage(data, null);
            return result;
        }

        internal static IViewModel FromGraphics(Bitmap data, Matrix transform, Rectangle visibleRect, string info)
            => new GraphicsVisualizerViewModel { Image = data, InfoText = info, Transform = transform, VisibleRect = visibleRect };

        #endregion

        #endregion
    }
}