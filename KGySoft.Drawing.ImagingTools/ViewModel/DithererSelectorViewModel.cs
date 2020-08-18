#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DithererSelectorViewModel.cs
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
using System.ComponentModel;
using System.Drawing;

using KGySoft.ComponentModel;
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Reflection;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class DithererSelectorViewModel : ViewModelBase
    {
        #region Properties

        // not a static property so always can be reinitialized with the current language
        internal IList<DithererDescriptor> Ditherers => Get(InitDitherers);
        internal DithererDescriptor SelectedDitherer { get => Get<DithererDescriptor>(); private set => Set(value); }
        internal CustomPropertiesObject Parameters { get => Get<CustomPropertiesObject>(); private set => Set(value); }
        internal IDitherer Ditherer { get => Get<IDitherer>(); private set => Set(value); }

        #endregion

        #region Methods

        #region Static Methods

        private static IList<DithererDescriptor> InitDitherers() =>
            new List<DithererDescriptor>
            {
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

                new DithererDescriptor(typeof(RandomNoiseDitherer).GetConstructor(new[] { typeof(float), typeof(int?) })),
                new DithererDescriptor(typeof(InterleavedGradientNoiseDitherer).GetConstructor(new[] { typeof(float) })),
            };

        #endregion

        #region Instance Methods

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == nameof(SelectedDitherer))
            {
                CustomPropertiesObject previousParameters = Parameters;
                Parameters = previousParameters == null
                    ? new CustomPropertiesObject(SelectedDitherer.Parameters)
                    : new CustomPropertiesObject(previousParameters, SelectedDitherer.Parameters);
                return;
            }

            if (e.PropertyName == nameof(Parameters))
            {
                if (e.OldValue is CustomPropertiesObject oldParams)
                    oldParams.PropertyChanged -= ParametersPropertyChanged;
                if (e.NewValue is CustomPropertiesObject newParams)
                    newParams.PropertyChanged += ParametersPropertyChanged;
                ResetDitherer();
                return;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;
            CustomPropertiesObject parameters = Parameters;
            if (parameters != null)
                parameters.PropertyChanged -= ParametersPropertyChanged;
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

        private void ResetDitherer()
        {
            DithererDescriptor descriptor = SelectedDitherer;
            Ditherer = null;
            if (descriptor == null)
                return;

            // TODO
            //object[] parameters = EvaluateParameters(descriptor.Parameters);
            //Ditherer = (IDitherer)MethodAccessor.GetAccessor(descriptor.Method).Invoke(null, parameters);
        }

        #endregion

        #region Event Handlers

        private void ParametersPropertyChanged(object sender, PropertyChangedEventArgs e) => ResetDitherer();

        #endregion

        #endregion

        #endregion
    }
}