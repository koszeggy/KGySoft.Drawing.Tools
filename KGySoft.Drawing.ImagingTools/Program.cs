#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Program.cs
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
using System.Globalization;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.View;
using KGySoft.Drawing.ImagingTools.View.Forms;
using KGySoft.Drawing.ImagingTools.View.UserControls;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal static class Program
    {
        #region Methods

        /// <summary>
        /// When executed as a standalone application, this is the entry point.
        /// </summary>
        [STAThread]
        internal static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //ViewModelFactory.CreateManageInstallations()
            //ViewModelFactory.FromBitmapData()
            //ViewModelFactory.FromCustomBitmap()
            //ViewModelFactory.FromCustomColor()
            //ViewModelFactory.FromCustomPalette()
            //ViewModelFactory.FromPalette()

            using IViewModel viewModel = ViewModelFactory.FromCommandLineArguments(args);
            using IView view = ViewFactory.CreateView(viewModel);
            Application.Run(ViewFactory.TryGetForm(view));

            //using var bmp = new System.Drawing.Bitmap(100, 100);
            //bmp.Clear(System.Drawing.Color.AliceBlue);

            //using (IViewModel viewModel = ViewModelFactory.CreateLanguageSettings())
            //{
            //    using IView view = ViewFactory.CreateView(viewModel);
            //    Application.Run(ViewFactory.TryGetForm(view));
            //}

            //// WPF test
            //using (IViewModel viewModel = ViewModelFactory.CreateLanguageSettings())
            //{
            //    using IView view = ViewFactory.CreateView(viewModel);
            //    System.Windows.Application app = new();
            //    app.Run(new System.Windows.Window { Content = new System.Windows.Forms.Integration.WindowsFormsHost { Child = (Control)view } });
            //}
        }

        #endregion
    }
}
