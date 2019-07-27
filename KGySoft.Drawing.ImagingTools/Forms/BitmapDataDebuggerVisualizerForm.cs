#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: BitmapDataDebuggerVisualizerForm.cs
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

#endregion

namespace KGySoft.Drawing.ImagingTools.Forms
{
    internal partial class BitmapDataDebuggerVisualizerForm : ImageDebuggerVisualizerForm
    {
        #region Properties

        #region Internal Properties

        internal string SpecialInfo { get; set; }

        #endregion

        #region Protected Properties

        protected override bool IsPaletteReadOnly => false;

        #endregion

        #endregion

        #region Constructors

        public BitmapDataDebuggerVisualizerForm() => InitializeComponent();

        #endregion

        #region Methods

        protected override void UpdateInfo()
        {
            if (Image == null)
                return;

            Text = String.Format("Type: BitmapData; Size: {0}", Image.Size);
            txtInfo.Text = SpecialInfo;
        }

        protected override void ImageChanged()
        {
            base.ImageChanged();
            ImageData image = GetCurrentImage();
            if (image == null || image.Image == null)
                return;
            if (Image.GetPixelFormatSize(GetCurrentImage().PixelFormat) <= 8)
                Notification = "The palette of an indexed BitmapData cannot be reconstructed, therefore a default palette is used. You can change palette colors in the menu.";
        }

        #endregion
    }
}
