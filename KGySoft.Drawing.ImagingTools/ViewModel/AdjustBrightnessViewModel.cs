﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdjustBrightnessViewModel.cs
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

using System;
using System.Drawing;

using KGySoft.Drawing.Imaging;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class AdjustBrightnessViewModel : AdjustColorsViewModelBase
    {
        #region Nested Classes

        private sealed class GenerateTask : AdjustColorsTaskBase
        {
            #region Constructors

            internal GenerateTask(float value, ColorChannels colorChannels)
                : base(value, colorChannels)
            {
            }

            #endregion

            #region Methods

            internal override IAsyncResult BeginGenerate(AsyncConfig asyncConfig)
                => BitmapData!.BeginAdjustBrightness(Value, channels: ColorChannels, asyncConfig: asyncConfig);

            internal override Bitmap? EndGenerate(IAsyncResult asyncResult)
            {
                asyncResult.EndAdjustBrightness();
                return base.EndGenerate(asyncResult);
            }

            #endregion
        }

        #endregion

        #region Constructors

        internal AdjustBrightnessViewModel(Bitmap image) : base(image)
        {
        }

        #endregion

        #region Methods

        protected override GenerateTaskBase CreateGenerateTask() => new GenerateTask(Value, ColorChannels);

        #endregion
    }
}
