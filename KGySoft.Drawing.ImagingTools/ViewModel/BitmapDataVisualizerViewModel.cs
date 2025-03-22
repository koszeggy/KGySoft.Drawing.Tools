#region Copyright

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

        #endregion

        #region Properties

        #region Internal Properties

        internal BitmapDataInfo? BitmapDataInfo { get => Get<BitmapDataInfo?>(); set => Set(value); }

        #endregion

        #region Protected Properties

        protected override bool IsPaletteReadOnly => false;

        #endregion

        #endregion

        #region Constructors

        internal BitmapDataVisualizerViewModel() : base(AllowedImageTypes.Bitmap)
        {
            ReadOnly = true;
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == nameof(BitmapDataInfo))
            {
                var bitmapDataInfo = (BitmapDataInfo?)e.NewValue;
                Image = bitmapDataInfo?.BackingImage;
                var pixelFormat = bitmapDataInfo?.BitmapData?.PixelFormat ?? PixelFormat.Format32bppArgb;
                if (pixelFormat != lastPixelFormat && pixelFormat.ToBitsPerPixel() <= 8)
                    SetNotification(Res.NotificationPaletteCannotBeRestoredId);
                lastPixelFormat = pixelFormat;
            }
        }

        protected override void UpdateInfo()
        {
            BitmapDataInfo? bitmapDataInfo = BitmapDataInfo;
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
                BitmapDataInfo?.Dispose();
            base.Dispose(disposing);
        }

        #region Explicitly Implemented Interface Methods

        BitmapDataInfo? IViewModel<BitmapDataInfo?>.GetEditedModel() => null; // not editable
        bool IViewModel<BitmapDataInfo?>.TrySetModel(BitmapDataInfo? model) => TryInvokeSync(() => BitmapDataInfo = model);

        #endregion

        #endregion

        #endregion
    }
}
