﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdjustGammaViewModel.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
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

using KGySoft.Drawing.Imaging;
using KGySoft.Threading;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class AdjustGammaViewModel : AdjustColorsViewModelBase
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
                => BitmapData!.BeginAdjustGamma(Value, channels: ColorChannels, asyncConfig: asyncConfig);

            internal override Bitmap? EndGenerate(IAsyncResult asyncResult)
            {
                asyncResult.EndAdjustGamma();
                return base.EndGenerate(asyncResult);
            }

            #endregion
        }

        #endregion

        #region Constructors

        internal AdjustGammaViewModel(Bitmap image) : base(image)
        {
        }

        #endregion

        #region Properties

        #region Internal Properties
        
        internal override float MinValue => 0f;
        internal override float MaxValue => 10f;

        #endregion

        #region Protected Properties
        
        protected override float DefaultValue => 1f;

        #endregion

        #endregion

        #region Methods

        protected override GenerateTaskBase CreateGenerateTask() => new GenerateTask(Value, ColorChannels);

        #endregion
    }
}
