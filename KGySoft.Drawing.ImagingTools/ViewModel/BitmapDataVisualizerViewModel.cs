﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: BitmapDataVisualizerViewModel.cs
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

using System.Drawing;
using System.Drawing.Imaging;

using KGySoft.ComponentModel;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class BitmapDataVisualizerViewModel : ImageVisualizerViewModel, IViewModel<BitmapDataInfo?>
    {
        #region Fields

        private PixelFormat lastPixelFormat;
        private BitmapDataInfo? bitmapDataInfo;

        #endregion

        #region Properties

        protected override bool IsPaletteReadOnly => false;

        #endregion

        #region Constructors

        internal BitmapDataVisualizerViewModel(BitmapDataInfo? bitmapDataInfo)
            : base(AllowedImageTypes.Bitmap)
        {
            ReadOnly = true;
            ResetBitmapDataInfo(bitmapDataInfo, true);
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void UpdateInfo()
        {
            BitmapData? bitmapData = bitmapDataInfo?.BitmapData;

            if (bitmapDataInfo?.BackingImage == null || bitmapData == null)
            {
                TitleCaption = Res.TitleNoImage;
                InfoText = null;
                return;
            }

            var size = new Size(bitmapData.Width, bitmapData.Height);
            TitleCaption = $"{Res.TitleType(nameof(BitmapData))}{Res.TextSeparator}{Res.TitleSize(size)}";
            InfoText = Res.InfoBitmapData(size, bitmapData.Stride, bitmapData.PixelFormat);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                bitmapDataInfo?.Dispose();
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void ResetBitmapDataInfo(BitmapDataInfo? model, bool resetPreview)
        {
            bitmapDataInfo?.Dispose();
            bitmapDataInfo = model;
            SetImageInfo(new ImageInfo(bitmapDataInfo?.BackingImage), resetPreview);
            var pixelFormat = bitmapDataInfo?.BitmapData?.PixelFormat ?? PixelFormat.Format32bppArgb;
            if (pixelFormat != lastPixelFormat && pixelFormat.ToBitsPerPixel() <= 8)
                SetNotification(Res.NotificationPaletteCannotBeRestoredId);
            lastPixelFormat = pixelFormat;
        }

        #endregion

        #region Explicitly Implemented Interface Methods

        BitmapDataInfo? IViewModel<BitmapDataInfo?>.GetEditedModel() => null; // not editable
        bool IViewModel<BitmapDataInfo?>.TrySetModel(BitmapDataInfo? model) => TryInvokeSync(() => ResetBitmapDataInfo(model, false));

        #endregion

        #endregion
    }
}
