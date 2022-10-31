#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CustomBitmapVisualizerViewModel.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2022 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Linq;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class CustomBitmapVisualizerViewModel : ImageVisualizerViewModel
    {
        #region Properties

        internal CustomBitmapInfo? CustomBitmapInfo { get => Get<CustomBitmapInfo?>(); init => Set(value); }

        #endregion

        #region Constructors

        internal CustomBitmapVisualizerViewModel()
            : base(AllowedImageTypes.Bitmap)
        {
            ReadOnly = true;
        }

        #endregion

        #region Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == nameof(CustomBitmapInfo))
            {
                var customBitmapInfo = (CustomBitmapInfo?)e.NewValue;
                Image = customBitmapInfo?.BitmapData?.ToBitmap();
            }
        }

        protected override void UpdateInfo()
        {
            CustomBitmapInfo? info = CustomBitmapInfo;
            IReadableBitmapData? bitmapData = info?.BitmapData;

            if (bitmapData == null)
            {
                TitleCaption = Res.TitleNoImage;
                InfoText = null;
                return;
            }

            string type = info!.Type ?? bitmapData.GetType().Name;
            TitleCaption = $"{Res.TitleType(type)}{(info.ShowPixelSize ? $"{Res.TextSeparator}{Res.TitleSize(bitmapData.Size)}" : String.Empty)}";
            InfoText = Res.InfoCustomBitmap(type, info.CustomAttributes.Select(a => $"{a.Key}: {a.Value}").Join(Environment.NewLine));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                CustomBitmapInfo?.Dispose();
            base.Dispose(disposing);
        }

        #endregion
    }
}
