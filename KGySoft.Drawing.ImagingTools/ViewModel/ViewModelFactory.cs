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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    /// <summary>
    /// Represents a class that can create view model instances.
    /// </summary>
    internal static class ViewModelFactory
    {
        #region Methods

        internal static ViewModelBase CreateDefault() => new DefaultViewModel();
        internal static ViewModelBase FromCommandLineArguments(string[] args) => new DefaultViewModel { CommandLineArguments = args };
        internal static ImageVisualizerViewModel FromImage(Image image, ImageTypes imageTypes = ImageTypes.All) => new ImageVisualizerViewModel { Image = image, ImageTypes = imageTypes };
        internal static ImageVisualizerViewModel FromIcon(Icon icon, ImageTypes imageTypes = ImageTypes.Icon) => new ImageVisualizerViewModel { Icon = icon, ImageTypes = imageTypes };
        internal static PaletteVisualizerViewModel FromPalette(IList<Color> palette) => new PaletteVisualizerViewModel { Palette = palette };
        internal static ColorVisualizerViewModel FromColor(Color color) => new ColorVisualizerViewModel { Color = color };
        internal static ViewModelBase FromBitmapData(Bitmap data, string info) => new BitmapDataVisualizerViewModel { Image = data, InfoText = info };
        internal static ViewModelBase FromGraphics(Bitmap data, string info, Matrix transform, Rectangle visibleRect) => new GraphicsVisualizerViewModel { Image = data, InfoText = info, Transform = transform, VisibleRect = visibleRect };
        internal static ViewModelBase CreateManageInstallations(string hintPath) => new ManageInstallationsViewModel(hintPath);

        #endregion

    }
}