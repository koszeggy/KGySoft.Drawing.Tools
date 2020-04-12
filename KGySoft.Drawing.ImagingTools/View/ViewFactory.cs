#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ViewFactory.cs
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

using KGySoft.Drawing.ImagingTools.View.Forms;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View
{
    /// <summary>
    /// Represents a class that can create view instances.
    /// </summary>
    public static class ViewFactory
    {
        #region Methods

        /// <summary>
        /// Creates a <see cref="Form"/> for the specified <see cref="ViewModelBase"/> instance.
        /// </summary>
        /// <param name="viewModel">The view model to create the view for.</param>
        /// <returns></returns>
        public static IView CreateView(IViewModel viewModel)
        {
            switch (viewModel)
            {
                case DefaultViewModel defaultViewModel:
                    return new AppMainForm(defaultViewModel);
                case GraphicsVisualizerViewModel graphicsVisualizerViewModel:
                    return new GraphicsVisualizerForm(graphicsVisualizerViewModel);
                case ImageVisualizerViewModel imageVisualizerViewModel: // also for BitmapData
                    return new ImageVisualizerForm(imageVisualizerViewModel);
                case PaletteVisualizerViewModel paletteVisualizerViewModel:
                    return new PaletteVisualizerForm(paletteVisualizerViewModel);
                case ColorVisualizerViewModel colorVisualizerViewModel:
                    return new ColorVisualizerForm(colorVisualizerViewModel);
                case ManageInstallationsViewModel manageInstallationsViewModel:
                    return new ManageInstallationsForm(manageInstallationsViewModel);
                default:
                    throw new InvalidOperationException(Res.InternalError($"Unexpected viewModel type: {viewModel.GetType()}"));
            }
        }

        public static void ShowDialog(IViewModel viewModel, IntPtr ownerWindowHandle = default)
        {
            using (IView view = CreateView(viewModel))
                view.ShowDialog(ownerWindowHandle == IntPtr.Zero ? null : new OwnerWindowHandle(ownerWindowHandle));
        }

        #endregion
    }
}