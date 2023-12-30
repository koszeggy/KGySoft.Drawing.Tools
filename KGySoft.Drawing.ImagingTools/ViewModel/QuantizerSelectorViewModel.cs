#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: QuantizerSelectorViewModel.cs
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
using System.Drawing;
using System.Drawing.Imaging;

using KGySoft.ComponentModel;
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class QuantizerSelectorViewModel : ViewModelBase
    {
        #region Properties

        // not a static property so always can be reinitialized with the current language
        internal IList<QuantizerDescriptor> Quantizers => Get(InitQuantizers);
        internal PixelFormat SelectedFormat { get => Get<PixelFormat>(); set => Set(value); }
        internal bool UseDithering { get => Get<bool>(); set => Set(value); }
        internal QuantizerDescriptor? SelectedQuantizer { get => Get<QuantizerDescriptor?>(); private set => Set(value); }
        // TODO internal CustomPropertiesObject? Parameters { get => Get<CustomPropertiesObject?>(); private set => Set(value); }
        internal bool UseLinearColorSpace { get => Get<bool>(); private set => Set(value); }
        internal bool BackColorEnabled { get => Get<bool>(); private set => Set(value); }
        internal Color BackColor { get => Get<Color>(); set => Set(value); }
        internal bool AlphaThresholdVisible { get => Get<bool>(); private set => Set(value); }
        internal bool AlphaThresholdEnabled { get => Get<bool>(); private set => Set(value); }
        internal byte AlphaThreshold { get => Get<byte>(128); set => Set(value); }
        internal bool WhiteThresholdVisible { get => Get<bool>(); private set => Set(value); }
        internal byte WhiteThreshold { get => Get<byte>(128); set => Set(value); }
        internal bool NumColorsVisible { get => Get<bool>(); private set => Set(value); }
        internal int NumColors { get => Get<int>(); set => Set(value); }
        internal bool DirectMappingVisible { get => Get<bool>(); private set => Set(value); }
        internal bool DirectMapping { get => Get<bool>(); set => Set(value); }
        internal bool BitLevelVisible { get => Get<bool>(); private set => Set(value); }
        internal byte BitLevel { get => Get<byte>(); set => Set(value); }
        internal bool CustomColorsVisible { get => Get<bool>(); private set => Set(value); }
        internal Palette? CustomColors { get => Get<Palette?>(); set => Set(value); }
        internal IQuantizer? Quantizer { get => Get<IQuantizer?>(); private set => Set(value); }
        internal Exception? CreateQuantizerError { get => Get<Exception?>(); set => Set(value); }

        #endregion

        #region Methods

        #region Static Methods

        private static IList<QuantizerDescriptor> InitQuantizers() =>
            new List<QuantizerDescriptor>
            {
                // TODO
                //new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.FromPixelFormat)),
                new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.BlackAndWhite)),
                //new QuantizerDescriptor(typeof(PredefinedColorsQuantizer).GetMethod(nameof(PredefinedColorsQuantizer.FromCustomPalette), new[] { typeof(Color[]), typeof(Color), typeof(byte) })!),
                new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.Grayscale4)),
                new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.Grayscale16)),
                new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.Grayscale)),
                new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.SystemDefault1BppPalette)),
                new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.SystemDefault4BppPalette)),
                new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.SystemDefault8BppPalette)),
                new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.Rgb332)),
                new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.Rgb555)),
                new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.Rgb565)),
                new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.Argb1555)),
                new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.Rgb888)),
                new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.Argb8888)),

                new QuantizerDescriptor(typeof(OptimizedPaletteQuantizer), nameof(OptimizedPaletteQuantizer.Octree)),
                new QuantizerDescriptor(typeof(OptimizedPaletteQuantizer), nameof(OptimizedPaletteQuantizer.MedianCut)),
                new QuantizerDescriptor(typeof(OptimizedPaletteQuantizer), nameof(OptimizedPaletteQuantizer.Wu)),
            };

        #endregion

        #region Instance Methods

        #region Internal Methods

        internal void ResetQuantizer()
        {
            QuantizerDescriptor? descriptor = SelectedQuantizer;

            // TODO
            //CustomPropertiesObject? parameters = Parameters;
            //CreateQuantizerError = null;
            //if (descriptor == null || parameters == null)
            //{
            //    Quantizer = null;
            //    return;
            //}

            //object?[] parameterValues = descriptor.EvaluateParameters(parameters);
            //try
            //{
            //    Quantizer = (IQuantizer)MethodAccessor.GetAccessor(descriptor.Method).Invoke(null, parameterValues)!;
            //}
            //catch (Exception e) when (!e.IsCritical())
            //{
            //    Quantizer = null;
            //    CreateQuantizerError = e;
            //}
        }

        #endregion

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SelectedFormat):
                    var selectedQuantizer = SelectedQuantizer ?? Quantizers[0];
                    AlphaThresholdEnabled = ((PixelFormat)e.NewValue!).HasAlpha() && (selectedQuantizer.HasSingleBitAlpha || selectedQuantizer.HasAlpha && UseDithering);
                    break;
                case nameof(SelectedQuantizer):
                    selectedQuantizer = (QuantizerDescriptor)e.NewValue!;
                    BackColorEnabled = !selectedQuantizer.HasAlpha || selectedQuantizer.HasSingleBitAlpha || UseDithering;
                    AlphaThresholdVisible = selectedQuantizer.HasAlpha;
                    AlphaThresholdEnabled = SelectedFormat.HasAlpha() && (selectedQuantizer.HasSingleBitAlpha || selectedQuantizer.HasAlpha && UseDithering);
                    WhiteThresholdVisible = selectedQuantizer.HasWhiteThreshold;
                    NumColorsVisible = selectedQuantizer.IsOptimized;
                    DirectMappingVisible = selectedQuantizer.HasDirectMapping;
                    BitLevelVisible = selectedQuantizer.IsOptimized;
                    break;
                case nameof(UseDithering):
                    BackColorEnabled = (bool)e.NewValue! || SelectedQuantizer is { HasAlpha: false } or { HasSingleBitAlpha: true };
                    break;
            }

            base.OnPropertyChanged(e);

            switch (e.PropertyName)
            {
                // TODO
                //case nameof(SelectedQuantizer):
                //    CustomPropertiesObject? previousParameters = Parameters;
                //    Parameters = previousParameters == null
                //        ? new CustomPropertiesObject(SelectedQuantizer!.Parameters)
                //        : new CustomPropertiesObject(previousParameters, SelectedQuantizer!.Parameters);
                //    return;

                //case nameof(Parameters):
                //    ResetQuantizer();
                //    return;
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods


        #endregion

        #endregion

        #endregion
    }
}