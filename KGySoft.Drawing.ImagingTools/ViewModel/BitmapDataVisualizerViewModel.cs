#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: BitmapDataVisualizerViewModel.cs
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
using System.Drawing.Imaging;

using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class BitmapDataVisualizerViewModel : ImageVisualizerViewModel
    {
        #region Properties

        protected override bool IsPaletteReadOnly => false;

        #endregion

        #region Constructors

        internal BitmapDataVisualizerViewModel()
        {
            ReadOnly = true;
            OpenFileCommandState.Enabled = false;
            ClearCommandState.Enabled = false;
        }

        #endregion

        #region Methods

        protected override void UpdateInfo()
        {
            // InfoText is expected to be set already so setting caption and notification only
            ImageData image = GetCurrentImage();

            if (image?.Image == null)
            {
                TitleCaption = Res.TitleNoImage;
                return;
            }

            TitleCaption = $"{Res.TitleType(nameof(BitmapData))}; {Res.TitleSize(image.Size.ToString())}";
            if (GetCurrentImage().BitsPerPixel <= 8)
                Notification = Res.NotificationPaletteCannotBeRestored;
        }

        #endregion
    }
}
