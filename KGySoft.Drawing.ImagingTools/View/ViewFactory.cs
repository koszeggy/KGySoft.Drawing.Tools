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

using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Drawing.ImagingTools.View.Design;
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
        #region Constructors

        static ViewFactory()
        {
            DesignDependencies.QuantizerThresholdEditor = typeof(QuantizerThresholdEditor);
            DesignDependencies.DithererStrengthEditor = typeof(DithererStrengthEditor);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a view for the specified <see cref="IViewModel"/> instance.
        /// </summary>
        /// <param name="viewModel">The view model to create the view for.</param>
        /// <returns>A view for the specified <see cref="IViewModel"/> instance.</returns>
        public static IView CreateView(IViewModel viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel), PublicResources.ArgumentNull);

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
                case ResizeBitmapViewModel resizeBitmapViewModel:
                    return new ResizeBitmapForm(resizeBitmapViewModel);
                case ColorSpaceViewModel colorSpaceViewModel:
                    return new ColorSpaceForm(colorSpaceViewModel);
                case CountColorsViewModel countColorsViewModel:
                    return new CountColorsForm(countColorsViewModel);
                case AdjustBrightnessViewModel adjustBrightnessViewModel:
                    return new AdjustBrightnessForm(adjustBrightnessViewModel);
                case AdjustContrastViewModel adjustContrastViewModel:
                    return new AdjustContrastForm(adjustContrastViewModel);
                case AdjustGammaViewModel adjustGammaViewModel:
                    return new AdjustGammaForm(adjustGammaViewModel);
                default:
                    throw new InvalidOperationException(Res.InternalError($"Unexpected viewModel type: {viewModel.GetType()}"));
            }
        }

        /// <summary>
        /// Shows an internally created view for the specified <see cref="IViewModel"/> instance,
        /// which will be discarded when the view is closed.
        /// </summary>
        /// <param name="viewModel">The view model to create the view for.</param>
        /// <param name="ownerWindowHandle">If specified, then the created dialog will be owned by the window that has specified handle. This parameter is optional.
        /// <br/>Default value: <see cref="IntPtr.Zero">IntPtr.Zero</see>.</param>
        /// <returns>A view for the specified <see cref="IViewModel"/> instance.</returns>
        public static void ShowDialog(IViewModel viewModel, IntPtr ownerWindowHandle = default)
        {
            using (IView view = CreateView(viewModel))
                view.ShowDialog(ownerWindowHandle);
        }

        #endregion
    }
}