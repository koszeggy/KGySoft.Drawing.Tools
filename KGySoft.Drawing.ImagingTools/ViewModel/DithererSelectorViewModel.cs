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
using System.Diagnostics;
using System.Reflection;

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
        internal DithererDescriptor? SelectedDitherer { get => Get<DithererDescriptor?>(); private set => Set(value); }
        internal CustomPropertiesObject? Parameters { get => Get<CustomPropertiesObject?>(); private set => Set(value); }
        internal IDitherer? Ditherer { get => Get<IDitherer?>(); private set => Set(value); }
        internal Exception? CreateDithererError { get => Get<Exception?>(); set => Set(value); }

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

                new DithererDescriptor(typeof(RandomNoiseDitherer).GetConstructor(new[] { typeof(float), typeof(int?) })!),
                new DithererDescriptor(typeof(InterleavedGradientNoiseDitherer).GetConstructor(new[] { typeof(float) })!),
            };

        #endregion

        #region Instance Methods

        #region Internal Methods

        internal void ResetDitherer()
        {
            DithererDescriptor? descriptor = SelectedDitherer;
            CustomPropertiesObject? parameters = Parameters;
            CreateDithererError = null;

            if (descriptor == null || parameters == null)
            {
                Ditherer = null;
                return;
            }

            IDitherer? ditherer = null;
            try
            {
                foreach (MemberInfo memberInfo in descriptor.InvokeChain)
                {
                    switch (memberInfo)
                    {
                        case ConstructorInfo ctor:
                            Debug.Assert(ditherer == null);
                            ditherer = (IDitherer)CreateInstanceAccessor.GetAccessor(ctor).CreateInstance(descriptor.EvaluateParameters(ctor.GetParameters(), parameters));
                            break;

                        case PropertyInfo property:
                            Debug.Assert(ditherer == null && property.GetGetMethod()!.IsStatic);
                            ditherer = (IDitherer)PropertyAccessor.GetAccessor(property).Get(null)!;
                            break;

                        case MethodInfo method:
                            Debug.Assert(ditherer != null && !method.IsStatic);
                            ditherer = (IDitherer)MethodAccessor.GetAccessor(method).Invoke(ditherer, descriptor.EvaluateParameters(method.GetParameters(), parameters))!;
                            break;

                        default:
                            throw new InvalidOperationException(Res.InternalError($"Unexpected member in invoke chain: {memberInfo}"));
                    }
                }

                Ditherer = ditherer;
            }
            catch (Exception e) when (!e.IsCritical())
            {
                Ditherer = null;
                CreateDithererError = e;
            }
        }

        #endregion

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            switch (e.PropertyName)
            {
                case nameof(SelectedDitherer):
                    CustomPropertiesObject? previousParameters = Parameters;
                    Parameters = previousParameters == null
                        ? new CustomPropertiesObject(SelectedDitherer!.Parameters)
                        : new CustomPropertiesObject(previousParameters, SelectedDitherer!.Parameters);
                    return;

                case nameof(Parameters):
                    ResetDitherer();
                    return;
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