#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AppMainForm.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2019 - All Rights Reserved
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
using System.Drawing;
using System.Drawing.Imaging;

#endregion

namespace KGySoft.Drawing.ImagingTools.Forms
{
    internal partial class AppMainForm : ImageDebuggerVisualizerForm
    {
        #region Constants

        private const string title = "KGy SOFT Imaging Tools";

        #endregion

        #region Properties

        public override string Text
        {
            get => base.Text;
            set => base.Text = String.IsNullOrEmpty(value) ? title : $"{title} - {value}";
        }

        #endregion

        #region Constructors

        public AppMainForm()
        {
            InitializeComponent();

            Notification = $"As a standalone application, {title} can be used to load images, save them in various formats, extract frames or pages, examine or change palette entries of indexed images, etc.{Environment.NewLine}{Environment.NewLine}"
                + $"But it can be used also as a debugger visualizer for {nameof(Image)}, {nameof(Bitmap)}, {nameof(Metafile)}, {nameof(BitmapData)}, {nameof(Graphics)}, {nameof(ColorPalette)} and {nameof(Color)} types.{Environment.NewLine}"
                + $"See the '{btnConfiguration.Text}' button.";
        }

        #endregion

        #region Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();
            base.Dispose(disposing);
        }

        #endregion
    }
}
