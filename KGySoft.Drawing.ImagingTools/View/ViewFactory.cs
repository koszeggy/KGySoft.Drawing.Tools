#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ViewFactory.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2025 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.View.Forms;
using KGySoft.Drawing.ImagingTools.View.UserControls;
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

        #region Public Methods

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
                DefaultViewModel defaultViewModel => new MainUserControl(defaultViewModel),
                GraphicsVisualizerViewModel graphicsVisualizerViewModel => new GraphicsVisualizerControl(graphicsVisualizerViewModel),
                ImageVisualizerViewModel imageVisualizerViewModel => new ImageVisualizerControl(imageVisualizerViewModel), // also for BitmapData and CustomBitmap
                PaletteVisualizerViewModel paletteVisualizerViewModel => new PaletteVisualizerForm(paletteVisualizerViewModel), // also for custom palette
                ColorVisualizerViewModel colorVisualizerViewModel => new ColorVisualizerControl(colorVisualizerViewModel), // also for custom color
                ManageInstallationsViewModel manageInstallationsViewModel => new ManageInstallationsForm(manageInstallationsViewModel),
                ResizeBitmapViewModel resizeBitmapViewModel => new ResizeBitmapControl(resizeBitmapViewModel),
                ColorSpaceViewModel colorSpaceViewModel => new ColorSpaceControl(colorSpaceViewModel),
                CountColorsViewModel countColorsViewModel => new CountColorsControl(countColorsViewModel),
                AdjustBrightnessViewModel adjustBrightnessViewModel => new AdjustBrightnessControl(adjustBrightnessViewModel),
                AdjustContrastViewModel adjustContrastViewModel => new AdjustContrastControl(adjustContrastViewModel),
                AdjustGammaViewModel adjustGammaViewModel => new AdjustGammaControl(adjustGammaViewModel),
                LanguageSettingsViewModel languageSettingsViewModel => new LanguageSettingsForm(languageSettingsViewModel),
                EditResourcesViewModel editResourcesViewModel => new EditResourcesForm(editResourcesViewModel),
                DownloadResourcesViewModel downloadResourcesViewModel => new DownloadResourcesControl(downloadResourcesViewModel),
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

        #region Internal Methods

        internal static Form? TryGetForm(IView view)
        {
            if (view is not MvvmBaseUserControl mvvmControl)
                return view as Form;

            if (mvvmControl.ParentForm is MvvmParentForm parent)
                return parent;

            return mvvmControl.Parent != null ? null // Custom parent: not creating a parent form
                : mvvmControl is MainUserControl mainUserControl ? new AppMainForm(mainUserControl)
                : new MvvmParentForm(mvvmControl);
        }

        #endregion

        #endregion
    }
}