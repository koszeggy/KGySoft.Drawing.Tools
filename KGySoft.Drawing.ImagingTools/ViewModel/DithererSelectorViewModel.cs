#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DithererSelectorViewModel.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2023 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Collections.Generic;

using KGySoft.ComponentModel;
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class DithererSelectorViewModel : ViewModelBase, IDithererSettings
    {
        #region Constants

        internal const int MaxStrength = 256;

        #endregion

        #region Fields

        private static readonly HashSet<string> affectsDitherer = new()
        {
            nameof(SelectedDitherer),
            nameof(Strength),
            nameof(Seed),
            nameof(SerpentineProcessing),
            nameof(ByBrightness),
        };

        #endregion

        #region Properties

        #region Internal Properties

        // not a static property so always can be reinitialized with the current language
        internal IList<DithererDescriptor> Ditherers => Get(InitDitherers);
        internal DithererDescriptor? SelectedDitherer { get => Get<DithererDescriptor?>(); private set => Set(value); }
        internal bool StrengthVisible { get => Get<bool>(); private set => Set(value); }
        internal int Strength { get => Get<int>(); set => Set(value); }
        internal bool SerpentineProcessingVisible { get => Get<bool>(); private set => Set(value); }
        internal bool SerpentineProcessing { get => Get<bool>(); set => Set(value); }
        internal bool ByBrightnessVisible { get => Get<bool>(); private set => Set(value); }
        internal bool? ByBrightness { get => Get<bool?>(); set => Set(value); }
        internal bool SeedVisible { get => Get<bool>(); private set => Set(value); }
        internal int? Seed { get => Get<int?>(); set => Set(value); }
        internal IDitherer? Ditherer { get => Get<IDitherer?>(); private set => Set(value); }
        internal Exception? CreateDithererError { get => Get<Exception?>(); set => Set(value); }

        #endregion

        #region Explicitly Implemented Interface Properties

        float IDithererSettings.Strength => Strength / (float)MaxStrength;
        int? IDithererSettings.Seed => Seed;
        bool? IDithererSettings.ByBrightness => ByBrightness;
        bool IDithererSettings.DoSerpentineProcessing => SerpentineProcessing;

        #endregion

        #endregion

        #region Methods

        #region Static Methods

        private static IList<DithererDescriptor> InitDitherers() =>
            new List<DithererDescriptor>
            {
                // TODO
                //new DithererDescriptor(typeof(OrderedDitherer).GetConstructor(new[] { typeof(byte[,]), typeof(float) })),
                new DithererDescriptor(typeof(OrderedDitherer), nameof(OrderedDitherer.Bayer2x2)),
                new DithererDescriptor(typeof(OrderedDitherer), nameof(OrderedDitherer.Bayer3x3)),
                new DithererDescriptor(typeof(OrderedDitherer), nameof(OrderedDitherer.Bayer4x4)),
                new DithererDescriptor(typeof(OrderedDitherer), nameof(OrderedDitherer.Bayer8x8)),
                new DithererDescriptor(typeof(OrderedDitherer), nameof(OrderedDitherer.DottedHalftone)),
                new DithererDescriptor(typeof(OrderedDitherer), nameof(OrderedDitherer.BlueNoise)),

                //new DithererDescriptor(typeof(ErrorDiffusionDitherer).GetConstructor(new[] { typeof(byte[,]), typeof(int), typeof(int), typeof(bool), typeof(bool?) })),
                new DithererDescriptor(typeof(ErrorDiffusionDitherer), nameof(ErrorDiffusionDitherer.Atkinson)),
                new DithererDescriptor(typeof(ErrorDiffusionDitherer), nameof(ErrorDiffusionDitherer.Burkes)),
                new DithererDescriptor(typeof(ErrorDiffusionDitherer), nameof(ErrorDiffusionDitherer.FloydSteinberg)),
                new DithererDescriptor(typeof(ErrorDiffusionDitherer), nameof(ErrorDiffusionDitherer.JarvisJudiceNinke)),
                new DithererDescriptor(typeof(ErrorDiffusionDitherer), nameof(ErrorDiffusionDitherer.Sierra3)),
                new DithererDescriptor(typeof(ErrorDiffusionDitherer), nameof(ErrorDiffusionDitherer.Sierra2)),
                new DithererDescriptor(typeof(ErrorDiffusionDitherer), nameof(ErrorDiffusionDitherer.SierraLite)),
                new DithererDescriptor(typeof(ErrorDiffusionDitherer), nameof(ErrorDiffusionDitherer.StevensonArce)),
                new DithererDescriptor(typeof(ErrorDiffusionDitherer), nameof(ErrorDiffusionDitherer.Stucki)),

                new DithererDescriptor(typeof(RandomNoiseDitherer).GetConstructor(new[] { typeof(float), typeof(int?) })!),
                new DithererDescriptor(typeof(InterleavedGradientNoiseDitherer).GetConstructor(new[] { typeof(float) })!),
            };

        #endregion

        #region Instance Methods

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SelectedDitherer):
                    var selectedDitherer = (DithererDescriptor)e.NewValue!;
                    StrengthVisible = selectedDitherer.HasStrength;
                    SerpentineProcessingVisible = selectedDitherer.HasSerpentineProcessing;
                    ByBrightnessVisible = selectedDitherer.HasByBrightness;
                    SeedVisible = selectedDitherer.HasSeed;
                    break;
            }

            base.OnPropertyChanged(e);

            if (affectsDitherer.Contains(e.PropertyName))
                ResetDitherer();
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void ResetDitherer()
        {
            DithererDescriptor? descriptor = SelectedDitherer;
            if (descriptor == null)
                return;

            try
            {
                Ditherer = descriptor.Create(this);
            }
            catch (Exception e) when (!e.IsCritical())
            {
                Ditherer = null;
                CreateDithererError = e;
            }
        }

        #endregion

        #endregion

        #endregion
    }
}