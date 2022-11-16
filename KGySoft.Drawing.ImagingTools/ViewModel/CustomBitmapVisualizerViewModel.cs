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
using System.Text;

using KGySoft.CoreLibraries;
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal sealed class CustomBitmapVisualizerViewModel : ImageVisualizerViewModel
    {
        #region Fields

        private readonly CustomBitmapInfo bitmapInfo;

        #endregion

        #region Constructors

        internal CustomBitmapVisualizerViewModel(CustomBitmapInfo bitmapInfo)
            : base(AllowedImageTypes.Bitmap)
        {
            this.bitmapInfo = bitmapInfo;
            Image = bitmapInfo.BitmapData?.ToBitmap();
            ReadOnly = true;
        }

        #endregion

        #region Methods

        protected override void UpdateInfo()
        {
            IReadableBitmapData? bitmapData = bitmapInfo?.BitmapData;

            if (bitmapData == null)
            {
                TitleCaption = Res.TitleNoImage;
                InfoText = null;
                return;
            }

            string type = bitmapInfo!.Type ?? bitmapData.GetType().Name;
            TitleCaption = $"{Res.TitleType(type)}{(bitmapInfo.ShowPixelSize ? $"{Res.TextSeparator}{Res.TitleSize(bitmapData.Size)}" : String.Empty)}";
            var sb = new StringBuilder(Res.TitleType(type));
            if (bitmapInfo.CustomAttributes.Count > 0)
            {
                sb.AppendLine();
                sb.Append(Res.InfoCustomProperties(bitmapInfo.CustomAttributes.Select(a => $"{a.Key}: {a.Value}").Join(Environment.NewLine)));
            }

            InfoText = sb.ToString();
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
                bitmapInfo.Dispose();
            
            base.Dispose(disposing);
        }

        #endregion
    }
}
