#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdjustColorsViewModelBase.cs
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

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.Imaging;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal abstract class AdjustColorsViewModelBase : TransformBitmapViewModelBase
    {
        #region Nested Classes

        protected abstract class AdjustColorsTaskBase : GenerateTaskBase
        {
            #region Fields

            private Bitmap? result;

            #endregion

            #region Properties

            #region Internal Properties

            internal float Value { get; }
            internal ColorChannels ColorChannels { get; }

            #endregion

            #region Protected Properties

            protected IReadWriteBitmapData? BitmapData { get; private set; }

            #endregion

            #endregion

            #region Constructors

            protected AdjustColorsTaskBase(float value, ColorChannels colorChannels)
            {
                Value = value;
                ColorChannels = colorChannels;
            }

            #endregion

            #region Methods

            #region Internal Methods

            internal override void Initialize(Bitmap source, bool isInUse)
            {
                lock (source)
                    result = source.CloneCurrentFrame();
                BitmapData = result.GetReadWriteBitmapData();
            }

            internal override Bitmap? EndGenerate(IAsyncResult asyncResult)
            {
                // If there was no exception returning result and clearing the field to prevent disposing.
                // The caller will take care of disposing if the operation was canceled and the result is discarded.
                Bitmap? bmp = result;
                result = null;
                return bmp;
            }

            internal override void SetCompleted()
            {
                BitmapData?.Dispose();
                BitmapData = null;
                base.SetCompleted();
            }

            #endregion

            #region Protected Methods

            protected override void Dispose(bool disposing)
            {
                if (IsDisposed)
                    return;
                if (disposing)
                {
                    BitmapData?.Dispose();
                    result?.Dispose();
                }

                base.Dispose(disposing);
            }

            #endregion

            #endregion
        }

        #endregion

        #region Properties

        #region Internal Properties

        internal ColorChannels ColorChannels { get => Get(ColorChannels.Rgb); set => Set(value); }
        internal float Value { get => Get(DefaultValue); set => Set(value); }
        internal virtual float MinValue => -1f;
        internal virtual float MaxValue => 1f;

        #endregion

        #region Protected Properties

        protected virtual float DefaultValue => 0f;
        protected override bool AreSettingsChanged => !Value.Equals(DefaultValue) || ColorChannels != ColorChannels.Rgb;

        #endregion

        #endregion

        #region Constructors

        protected AdjustColorsViewModelBase(Bitmap image) : base(image)
        {
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override ValidationResultsCollection DoValidation()
        {
            ValidationResultsCollection result = base.DoValidation();
            float value = Value;
            float min = MinValue;
            float max = MaxValue;
            if (value < min || value > max)
                result.AddError(nameof(Value), Res.ErrorMessageValueMustBeBetween(min, max));
            return result;
        }

        protected override bool AffectsPreview(string propertyName) => propertyName.In(nameof(ColorChannels), nameof(Value));

        protected override bool MatchesSettings(GenerateTaskBase task)
        {
            var t = (AdjustColorsTaskBase)task;
            return t.Value.Equals(Value) && t.ColorChannels == ColorChannels;
        }

        protected override bool MatchesOriginal(GenerateTaskBase task)
        {
            var t = (AdjustColorsTaskBase)task;
            return t.Value.Equals(DefaultValue) && t.ColorChannels == ColorChannels.Rgb || t.ColorChannels == ColorChannels.None;
        }

        protected override void ResetParameters()
        {
            Value = DefaultValue;
            ColorChannels = ColorChannels.Rgb;
        }

        #endregion

        #endregion
    }
}
