#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CustomBitmapVisualizerViewModel.cs
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
using System.Drawing;
using System.Linq;
using System.Text;

using KGySoft.CoreLibraries;
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal sealed class CustomBitmapVisualizerViewModel : ImageVisualizerViewModel, IViewModel<CustomBitmapInfo?>
    {
        #region Fields

        private CustomBitmapInfo? bitmapInfo;

        #endregion

        #region Constructors

        internal CustomBitmapVisualizerViewModel(CustomBitmapInfo? bitmapInfo)
            : base(AllowedImageTypes.Bitmap)
        {
            ReadOnly = true;
            ResetBitmapInfo(bitmapInfo);
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void UpdateInfo()
        {
            if (bitmapInfo == null)
            {
                TitleCaption = Res.TitleNoImage;
                InfoText = null;
                return;
            }

            IReadableBitmapData? bitmapData = bitmapInfo.BitmapData;
            string type = bitmapInfo.Type ?? bitmapData?.GetType().Name ?? PublicResources.Null;
            TitleCaption = $"{Res.TitleType(type)}{(bitmapInfo.ShowPixelSize ? $"{Res.TextSeparator}{Res.TitleSize(bitmapData?.Size ?? Size.Empty)}" : String.Empty)}";
            var sb = new StringBuilder(Res.TitleType(type));
            if (bitmapInfo.CustomAttributes.Count > 0)
            {
                sb.AppendLine();
                sb.Append(Res.InfoCustomProperties(bitmapInfo.CustomAttributes.Select(a => $"{a.Key}: {a.Value}").Join(Environment.NewLine)));
            }

            InfoText = sb.ToString();
        }

        protected override bool IsPaletteAvailable() => bitmapInfo?.CustomPalette != null || base.IsPaletteAvailable();

        protected override void ShowPalette()
        {
            if (bitmapInfo?.CustomPalette == null)
            {
                base.ShowPalette();
                return;
            }

            using IViewModel vmPalette = ViewModelFactory.FromCustomPalette(bitmapInfo.CustomPalette);
            ShowChildViewCallback?.Invoke(vmPalette);
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
                bitmapInfo?.Dispose();
            
            bitmapInfo = null;
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void ResetBitmapInfo(CustomBitmapInfo? model)
        {
            bitmapInfo = model;
            Image = model?.BitmapData?.ToBitmap();
        }

        #endregion

        #region Explicitly Implemented Interface Methods

        CustomBitmapInfo? IViewModel<CustomBitmapInfo?>.GetEditedModel() => null; // not editable
        bool IViewModel<CustomBitmapInfo?>.TrySetModel(CustomBitmapInfo? model) => TryInvokeSync(() => ResetBitmapInfo(model));

        #endregion

        #endregion
    }
}
