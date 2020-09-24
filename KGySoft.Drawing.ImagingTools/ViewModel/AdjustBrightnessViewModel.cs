#region Copyright

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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

using KGySoft.CoreLibraries;
using KGySoft.Drawing.Imaging;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class AdjustBrightnessViewModel : TransformBitmapViewModelBase
    {
        #region Nested Classes

        private sealed class GenerateTask : GenerateTaskBase
        {
            #region Fields

            private Bitmap result;
            private IReadWriteBitmapData bitmapData;

            #endregion

            #region Properties

            internal float Value { get; }
            internal ColorChannels ColorChannels { get; }

            #endregion

            #region Constructors

            internal GenerateTask(float value, ColorChannels colorChannels)
            {
                Value = value;
                ColorChannels = colorChannels;
            }

            #endregion

            #region Methods

            #region Internal Methods

            [SuppressMessage("Reliability", "CA2002:Do not lock on objects with weak identity",
                Justification = "False alarm, source is never a remote object")]
            internal override void Initialize(Bitmap source, bool isInUse)
            {
                lock (source)
                    result = source.CloneCurrentFrame();
                bitmapData = result.GetReadWriteBitmapData();
            }

            internal override IAsyncResult BeginGenerate(AsyncConfig asyncConfig)
                => bitmapData.BeginAdjustBrightness(Value, channels: ColorChannels, asyncConfig: asyncConfig);

            internal override Bitmap EndGenerate(IAsyncResult asyncResult)
            {
                asyncResult.EndAdjustBrightness();

                // If there was no exception returning result and clearing the field to prevent disposing.
                // The caller will take care of disposing if the operation was canceled and the result is discarded.
                Bitmap bmp = result;
                result = null;
                return bmp;
            }

            internal override void SetCompleted()
            {
                bitmapData.Dispose();
                bitmapData = null;
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
                    bitmapData?.Dispose();
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
        internal float Value { get => Get<float>(); set => Set(value); }

        #endregion

        #region Protected Properties

        protected override bool AreSettingsChanged => !Value.Equals(0f) || ColorChannels != ColorChannels.Rgb;

        #endregion

        #endregion

        #region Constructors

        internal AdjustBrightnessViewModel(Bitmap image) : base(image)
        {
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override bool AffectsPreview(string propertyName) => propertyName.In(nameof(ColorChannels), nameof(Value));
        protected override GenerateTaskBase CreateGenerateTask() => new GenerateTask(Value, ColorChannels);

        protected override bool MatchesSettings(GenerateTaskBase task)
        {
            var t = (GenerateTask)task;
            return t.Value.Equals(Value) && t.ColorChannels == ColorChannels;
        }

        protected override bool MatchesOriginal(GenerateTaskBase task)
        {
            var t = (GenerateTask)task;
            return t.Value.Equals(0f) && t.ColorChannels == ColorChannels.Rgb;
        }

        protected override void ResetParameters()
        {
            Value = 0f;
            ColorChannels = ColorChannels.Rgb;
        }

        #endregion

        #endregion
    }
}
