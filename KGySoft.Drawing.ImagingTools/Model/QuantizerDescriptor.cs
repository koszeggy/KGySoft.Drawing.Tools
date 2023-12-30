#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: QuantizerDescriptor.cs
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
using System.Linq;
using System.Reflection;

using KGySoft.CoreLibraries;
using KGySoft.Drawing.Imaging;

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    internal sealed class QuantizerDescriptor
    {
        #region Fields

        private readonly ParameterInfo[] parameters;

        #endregion

        #region Properties

        internal MethodInfo Method { get; }

        internal bool IsOptimized { get; }
        internal bool HasAlpha { get; }
        internal bool HasSingleBitAlpha { get; }
        internal bool HasWhiteThreshold { get; }
        internal bool HasDirectMapping { get; }

        #endregion

        #region Constructors

        internal QuantizerDescriptor(Type type, string methodName) : this(type.GetMethod(methodName)!)
        {
        }

        internal QuantizerDescriptor(MethodInfo method)
        {
            Method = method;
            parameters = method.GetParameters();
            IsOptimized = method.DeclaringType == typeof(OptimizedPaletteQuantizer);
            HasAlpha = parameters.Any(p => p.Name == "alphaThreshold");
            HasSingleBitAlpha = IsOptimized || method.Name is nameof(PredefinedColorsQuantizer.Argb1555) or nameof(PredefinedColorsQuantizer.SystemDefault8BppPalette);
            HasWhiteThreshold = method.Name == nameof(PredefinedColorsQuantizer.BlackAndWhite);
            HasDirectMapping = parameters.Any(p => p.Name == "directMapping");
        }

        #endregion

        #region Methods

        #region Public Methods

        public override string ToString() => $"{Method.DeclaringType!.Name}.{Method.Name}";

        #endregion

        #region Internal Methods

        internal IQuantizer Create(IQuantizerSettings settings)
        {
            object[] args = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                args[i] = parameters[i].Name switch
                {
                    "backColor" => settings.BackColor,
                    "alphaThreshold" => settings.AlphaThreshold,
                    "whiteThreshold" => settings.WhiteThreshold,
                    "directMapping" => settings.DirectMapping,
                    "maxColors" => settings.PaletteSize,
                    _ => throw new InvalidOperationException($"Unhandled parameter: {parameters[i].Name}")
                };
            }

            IQuantizer result = (IQuantizer)Method.Invoke(null, args)!;
            result = result switch
            {
                OptimizedPaletteQuantizer optimized => optimized.ConfigureBitLevel(settings.BitLevel).ConfigureColorSpace(settings.WorkingColorSpace),
                PredefinedColorsQuantizer predefined => predefined.ConfigureColorSpace(settings.WorkingColorSpace),
                _ => result
            };
            return result;
        }

        #endregion

        #endregion
    }
}
