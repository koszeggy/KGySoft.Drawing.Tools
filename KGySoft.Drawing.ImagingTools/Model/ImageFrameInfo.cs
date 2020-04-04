#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ImageFrameInfo.cs
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
using KGySoft.ComponentModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    public sealed class ImageFrameInfo : ImageInfoBase
    {
        #region Properties

        public int Duration { get => Get<int>(); set => Set(value); }

        #endregion

        #region Constructors

        public ImageFrameInfo(Bitmap bitmap)
        {
            Image = bitmap;
            InitMeta(bitmap);
        }

        #endregion

        #region Methods

        protected override ValidationResultsCollection DoValidation()
        {
            ValidationResultsCollection result = base.DoValidation();
            if (Image == null)
                result.AddError(nameof(Image), PublicResources.ArgumentNull);
            return result;
        }

        #endregion
    }
}