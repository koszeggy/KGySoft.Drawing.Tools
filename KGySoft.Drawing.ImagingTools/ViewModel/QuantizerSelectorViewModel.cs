﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: QuantizerSelectorViewModel.cs
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
using System.Collections.Generic;
using System.Drawing;
using KGySoft.ComponentModel;
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Reflection;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class QuantizerSelectorViewModel : ViewModelBase
    {
        #region Properties

        // not a static property so always can be reinitialized with the current language
        internal IList<QuantizerDescriptor> Quantizers => Get(InitQuantizers);
        internal QuantizerDescriptor SelectedQuantizer { get => Get<QuantizerDescriptor>(); set => Set(value); }
        internal CustomPropertiesObject Parameters { get => Get<CustomPropertiesObject>(); set => Set(value); }


        #endregion

        #region Methods

        #region Static Methods

        private static IList<QuantizerDescriptor> InitQuantizers()
        {
            var result = new List<QuantizerDescriptor>
            {
                new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.Argb1555)),
                new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.Argb8888)),
                new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.BlackAndWhite)),
                //new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.FromCustomPalette)),
                //new QuantizerDescriptor(typeof(PredefinedColorsQuantizer).GetMethod(nameof(PredefinedColorsQuantizer.FromCustomPalette), new[] { typeof(Color[]), typeof(Color), typeof(byte) })),
                //new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.FromPixelFormat)),
                //new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.Grayscale)),
                //new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.Grayscale4)),
                //new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.Grayscale16)),
                //new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.SystemDefault1BppPalette)),
                //new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.SystemDefault4BppPalette)),
                //new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.SystemDefault8BppPalette)),
                //new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.Rgb888)),
                //new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.Rgb565)),
                //new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.Rgb555)),
                //new QuantizerDescriptor(typeof(PredefinedColorsQuantizer), nameof(PredefinedColorsQuantizer.Rgb332)),

                //new QuantizerDescriptor(typeof(OptimizedPaletteQuantizer), nameof(OptimizedPaletteQuantizer.Octree)),
                //new QuantizerDescriptor(typeof(OptimizedPaletteQuantizer), nameof(OptimizedPaletteQuantizer.MedianCut)),
                //new QuantizerDescriptor(typeof(OptimizedPaletteQuantizer), nameof(OptimizedPaletteQuantizer.Wu)),
            };

            //result.Sort(); // TODO
            return result;
        }

        #endregion

        #region Instance Methods

        #region Internal Methods

        internal override void ViewLoaded()
        {
            base.ViewLoaded();
        }

        internal IQuantizer CreateInstance()
        {
            QuantizerDescriptor descriptor = SelectedQuantizer;
            if (descriptor == null)
                return null;

            return (IQuantizer)MethodAccessor.GetAccessor(descriptor.Method).Invoke(null, EvaluateParameters(descriptor.Parameters));
        }

        #endregion

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == nameof(SelectedQuantizer))
            {
                CustomPropertiesObject previousParameters = Parameters;
                Parameters = previousParameters == null
                    ? new CustomPropertiesObject(SelectedQuantizer.Parameters)
                    : new CustomPropertiesObject(previousParameters, SelectedQuantizer.Parameters);
                return;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private object[] EvaluateParameters(CustomPropertyDescriptor[] parameters)
        {
            CustomPropertiesObject values = Parameters;
            var result = new object[parameters.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = parameters[i].GetValue(values);
            return result;
        }

        #endregion

        #endregion

        #endregion
    }
}