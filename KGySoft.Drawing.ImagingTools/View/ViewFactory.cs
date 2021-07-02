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

            return viewModel switch
            {
                DefaultViewModel defaultViewModel => new AppMainForm(defaultViewModel),
                GraphicsVisualizerViewModel graphicsVisualizerViewModel => new GraphicsVisualizerForm(graphicsVisualizerViewModel),
                ImageVisualizerViewModel imageVisualizerViewModel => new ImageVisualizerForm(imageVisualizerViewModel), // also for BitmapData
                PaletteVisualizerViewModel paletteVisualizerViewModel => new PaletteVisualizerForm(paletteVisualizerViewModel),
                ColorVisualizerViewModel colorVisualizerViewModel => new ColorVisualizerForm(colorVisualizerViewModel),
                ManageInstallationsViewModel manageInstallationsViewModel => new ManageInstallationsForm(manageInstallationsViewModel),
                ResizeBitmapViewModel resizeBitmapViewModel => new ResizeBitmapForm(resizeBitmapViewModel),
                ColorSpaceViewModel colorSpaceViewModel => new ColorSpaceForm(colorSpaceViewModel),
                CountColorsViewModel countColorsViewModel => new CountColorsForm(countColorsViewModel),
                AdjustBrightnessViewModel adjustBrightnessViewModel => new AdjustBrightnessForm(adjustBrightnessViewModel),
                AdjustContrastViewModel adjustContrastViewModel => new AdjustContrastForm(adjustContrastViewModel),
                AdjustGammaViewModel adjustGammaViewModel => new AdjustGammaForm(adjustGammaViewModel),
                LanguageSettingsViewModel languageSettingsViewModel => new LanguageSettingsForm(languageSettingsViewModel),
                EditResourcesViewModel editResourcesViewModel => new EditResourcesForm(editResourcesViewModel),
                DownloadResourcesViewModel downloadResourcesViewModel => new DownloadResourcesForm(downloadResourcesViewModel),
                _ => throw new InvalidOperationException(Res.InternalError($"Unexpected viewModel type: {viewModel.GetType()}"))
            };
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
            using IView view = CreateView(viewModel);
            view.ShowDialog(ownerWindowHandle);
        }

        /// <summary>
        /// Shows an internally created view for the specified <see cref="IViewModel"/> instance,
        /// which will be discarded when the view is closed.
        /// </summary>
        /// <param name="viewModel">The view model to create the view for.</param>
        /// <param name="owner">If not <see langword="null"/>, then the created dialog will be owned by the specified <see cref="IView"/> instance.</param>
        /// <returns>A view for the specified <see cref="IViewModel"/> instance.</returns>
        public static void ShowDialog(IViewModel viewModel, IView? owner)
        {
            using IView view = CreateView(viewModel);
            view.ShowDialog(owner);
        }

        #endregion
    }
}