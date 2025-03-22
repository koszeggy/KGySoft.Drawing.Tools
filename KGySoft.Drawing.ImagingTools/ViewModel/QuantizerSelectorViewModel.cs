#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: QuantizerSelectorViewModel.cs
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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;

using KGySoft.ComponentModel;
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Reflection;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class QuantizerSelectorViewModel : ViewModelBase, IQuantizerSettings
    {
        #region Fields

        private static readonly HashSet<string> affectsQuantizer = new()
        {
            nameof(SelectedQuantizer),
            nameof(UseLinearColorSpace),
            nameof(BackColor),
            nameof(AlphaThreshold),
            nameof(WhiteThreshold),
            nameof(NumColors),
            nameof(DirectMapping),
            nameof(BitLevel),
            nameof(CustomColors),
        };

        #endregion

        #region Properties

        #region Internal Properties

        // not a static property so always can be reinitialized with the current language
        internal IList<QuantizerDescriptor> Quantizers => Get(InitQuantizers);
        internal PixelFormat PixelFormat { get => Get<PixelFormat>(); set => Set(value); }
        internal bool UseDithering { get => Get<bool>(); set => Set(value); }
        internal QuantizerDescriptor? SelectedQuantizer { get => Get<QuantizerDescriptor?>(); private set => Set(value); }
        internal bool UseLinearColorSpace { get => Get<bool>(); private set => Set(value); }
        internal bool BackColorEnabled { get => Get<bool>(); private set => Set(value); }
        internal Color BackColor { get => Get(Color.Black); set => Set(value); }
        internal bool AlphaThresholdVisible { get => Get<bool>(); private set => Set(value); }
        internal bool AlphaThresholdEnabled { get => Get<bool>(); private set => Set(value); }
        internal int AlphaThreshold { get => Get(128); set => Set(value); }
        internal bool WhiteThresholdVisible { get => Get<bool>(); private set => Set(value); }
        internal int WhiteThreshold { get => Get(128); set => Set(value); }
        internal bool NumColorsVisible { get => Get<bool>(); private set => Set(value); }
        internal int NumColors { get => Get(256); set => Set(value); }
        internal int MaxColors { get => Get(256); private set => Set(value); }
        internal bool DirectMappingVisible { get => Get<bool>(); private set => Set(value); }
        internal bool DirectMapping { get => Get<bool>(); set => Set(value); }
        internal bool BitLevelVisible { get => Get<bool>(); private set => Set(value); }
        internal int BitLevel { get => Get<int>(); set => Set(value); }
        internal bool CustomColorsVisible { get => Get<bool>(); private set => Set(value); } // TODO
        internal Palette? CustomColors { get => Get<Palette?>(); set => Set(value); }
        internal IQuantizer? Quantizer { get => Get<IQuantizer?>(); private set => Set(value); }
        internal Exception? CreateQuantizerError { get => Get<Exception?>(); set => Set(value); }

        #endregion

        #region Explicitly Implemented Interface Properties

        Color IQuantizerSettings.BackColor => BackColor;
        byte IQuantizerSettings.AlphaThreshold => (byte)AlphaThreshold;
        byte IQuantizerSettings.WhiteThreshold => (byte)WhiteThreshold;
        bool IQuantizerSettings.DirectMapping => DirectMapping;
        int IQuantizerSettings.PaletteSize => NumColors;
        byte? IQuantizerSettings.BitLevel => BitLevel is int i and > 0 ? (byte)i : null;
        WorkingColorSpace IQuantizerSettings.WorkingColorSpace => UseLinearColorSpace ? WorkingColorSpace.Linear : WorkingColorSpace.Srgb;

        #endregion

        #endregion

        #region Methods

        #region Static Methods

        // Note that it evaluates the body whenever it's called, so it's always initialized by the current language.
        private static IList<QuantizerDescriptor> InitQuantizers() =>
            new List<QuantizerDescriptor>
            {
                //new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.FromPixelFormat)),
                new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.BlackAndWhite), false),
                new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.Grayscale4), false),
                new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.Grayscale16), false),
                new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.Grayscale), false),
                //new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.SystemDefault1BppPalette)),
                new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.SystemDefault4BppPalette), false),
                new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.SystemDefault8BppPalette), true),
                new QuantizerDescriptor($"{nameof(PredefinedColorsQuantizer)}.Argb1111",
                    (MethodInfo)Reflector.MemberOf(() => PredefinedColorsQuantizer.FromCustomPalette(Reflector.EmptyArray<Color32>(), default, default)),
                    true, true,
                    new Dictionary<string, object?>
                    {
                        ["palette"] = new[] { Color.Black, Color.Red, Color.Lime, Color.Blue, Color.Yellow, Color.Cyan, Color.Magenta, Color.White, Color.Transparent }.Select(c => c.ToColor32())
                    }),
                new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.Rgb332), false),
                new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.Rgb555), false),
                new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.Rgb565), false),
                new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.Argb1555), true),
                new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.Rgb888), false),
                new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.Argb8888), true),

                new QuantizerDescriptor(typeof(OptimizedPaletteQuantizer), nameof(OptimizedPaletteQuantizer.Octree), true),
                new QuantizerDescriptor(typeof(OptimizedPaletteQuantizer), nameof(OptimizedPaletteQuantizer.MedianCut), true),
                new QuantizerDescriptor(typeof(OptimizedPaletteQuantizer), nameof(OptimizedPaletteQuantizer.Wu), true),
            };

        #endregion

        #region Instance Methods

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            #region Local Methods

            void AdjustMaxColors()
            {
                if (!NumColorsVisible)
                    return;

                int maxBpp = Math.Min(16, PixelFormat.ToBitsPerPixel());
                MaxColors = 1 << maxBpp;
            }

            #endregion

            switch (e.PropertyName)
            {
                case nameof(PixelFormat):
                    var pixelFormat = (PixelFormat)e.NewValue!;
                    AlphaThresholdEnabled = (pixelFormat.HasAlpha() || pixelFormat.IsIndexed()) && SelectedQuantizer is { HasAlpha: true };
                    AdjustMaxColors();
                    break;
                case nameof(SelectedQuantizer):
                    var selectedQuantizer = (QuantizerDescriptor)e.NewValue!;
                    pixelFormat = PixelFormat;
                    BackColorEnabled = !selectedQuantizer.HasAlpha || selectedQuantizer.HasSingleBitAlpha || UseDithering;
                    AlphaThresholdVisible = selectedQuantizer.HasAlpha;
                    AlphaThresholdEnabled = (pixelFormat.HasAlpha() || pixelFormat.IsIndexed()) && selectedQuantizer.HasAlpha;
                    WhiteThresholdVisible = selectedQuantizer.HasWhiteThreshold;
                    NumColorsVisible = selectedQuantizer.IsOptimized;
                    DirectMappingVisible = selectedQuantizer.HasDirectMapping;
                    BitLevelVisible = selectedQuantizer.IsOptimized;
                    AdjustMaxColors();
                    break;
                case nameof(UseDithering):
                    BackColorEnabled = (bool)e.NewValue! || SelectedQuantizer is { HasAlpha: false } or { HasSingleBitAlpha: true };
                    break;
                case nameof(MaxColors):
                    int newValue = (int)e.NewValue!;
                    if (NumColors > newValue)
                        NumColors = newValue;
                    break;
            }

            base.OnPropertyChanged(e);

            if (affectsQuantizer.Contains(e.PropertyName!))
                ResetQuantizer();
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void ResetQuantizer()
        {
            QuantizerDescriptor? descriptor = SelectedQuantizer;
            if (descriptor == null)
                return;

            try
            {
                Quantizer = descriptor.Create(this);
            }
            catch (Exception e) when (!e.IsCritical())
            {
                Quantizer = null;
                CreateQuantizerError = e;
            }
        }

        #endregion

        #endregion

        #endregion
    }
}