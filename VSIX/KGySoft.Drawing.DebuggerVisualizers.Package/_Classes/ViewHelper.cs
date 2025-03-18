#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ViewHelper.cs
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
using System.Threading;

using KGySoft.Drawing.ImagingTools.View;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Package
{
    internal static class ViewHelper
    {
        #region Methods

        /// <summary>
        /// Creates the view (along with the view model) in a new STA thread. This has two benefits:
        /// 1.) The possible lagging of Visual Studio will not affect the view
        /// 2.) Creation of the view model might change the display language of the current thread, which would be the thread of Visual Studio in this project.
        /// </summary>
        /// <param name="viewModelFactory">The view model factory.</param>
        /// <returns></returns>
        internal static IView CreateViewInNewThread(Func<IViewModel> viewModelFactory)
        {
            IView? result = null;
            using var created = new ManualResetEvent(false);

            // Creating a non-background STA thread for the view so the possible lagging of VisualStudio will not affect its performance
            var t = new Thread(() =>
            {
                using IViewModel viewModel = viewModelFactory.Invoke();
                result = ViewFactory.CreateView(viewModel);

                // ReSharper disable once AccessToDisposedClosure - disposed only after awaited
                created.Set();

                // Now the view is shown as a dialog and this thread is kept alive until it is closed.
                // The caller method returns once the view is created and the result is also stored and can
                // be re-used until closing the view and thus exiting the thread.
                result.ShowDialog();
                result.Dispose();
            });

            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = false;
            t.Start();
            created.WaitOne();
            return result!;
        }

        #endregion
    }
}
