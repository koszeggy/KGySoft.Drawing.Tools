#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: QuantizerDescriptor.cs
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
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;

using KGySoft.CoreLibraries;

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    internal sealed class QuantizerDescriptor
    {
        #region Fields

        private static readonly Dictionary<string, CustomPropertyDescriptor> parametersMapping = new Dictionary<string, CustomPropertyDescriptor>
        {
            ["backColor"] = new CustomPropertyDescriptor("backColor", typeof(Color)) { DefaultValue = Color.Black },
            ["alphaThreshold"] = new CustomPropertyDescriptor("alphaThreshold", typeof(byte))
            {
                DefaultValue = (byte)128,
                UITypeEditor = DesignDependencies.QuantizerThresholdEditor
            },
            ["whiteThreshold"] = new CustomPropertyDescriptor("whiteThreshold", typeof(byte))
            {
                DefaultValue = (byte)128,
                UITypeEditor = DesignDependencies.QuantizerThresholdEditor
            },
            ["palette"] = new CustomPropertyDescriptor("palette", typeof(Color[]))
            {
                DefaultValue = new[]
                {
                    Color.Black, Color.White, Color.Transparent, 
                    Color.Red, Color.Lime, Color.Blue,
                    Color.Cyan, Color.Yellow, Color.Magenta
                },
            },
            //["pixelFormat"] = new CustomPropertyDescriptor("pixelFormat", typeof(PixelFormat))
            //{
            //    AllowedValues = Enum<PixelFormat>.GetValues().Where(pf => pf.IsValidFormat()).OrderBy(pf => pf & PixelFormat.Max).Select(pf => (object)pf).ToArray(),
            //},
            ["directMapping"] = new CustomPropertyDescriptor("directMapping", typeof(bool)) { DefaultValue = false },
            ["maxColors"] = new CustomPropertyDescriptor("maxColors", typeof(int))
            {
                DefaultValue = 256,
                AdjustValue = value =>
                {
                    if (!(value is int i))
                        return 0;

                    return i < 0 ? 0
                        : i > 256 ? 256
                        : i;
                }
            },
        };

        #endregion

        #region Properties

        internal MethodInfo Method { get; }

        internal CustomPropertyDescriptor[] Parameters { get; }

        #endregion

        #region Constructors

        internal QuantizerDescriptor(Type type, string methodName) : this(type.GetMethod(methodName))
        {
        }

        internal QuantizerDescriptor(MethodInfo method)
        {
            this.Method = method;
            ParameterInfo[] methodParams = method.GetParameters();
            Parameters = new CustomPropertyDescriptor[methodParams.Length];
            for (int i = 0; i < Parameters.Length; i++)
                Parameters[i] = parametersMapping.GetValueOrDefault(methodParams[i].Name) ?? throw new InvalidOperationException(Res.InternalError($"Unexpected parameter: {methodParams[i].Name}"));
        }

        #endregion

        #region Methods

        #region Public Methods
        
        public override string ToString() => $"{Method.DeclaringType.Name}.{Method.Name}";

        #endregion

        #region Internal Methods

        internal object[] EvaluateParameters(CustomPropertiesObject values)
        {
            var result = new object[Parameters.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = Parameters[i].GetValue(values);
            return result;
        }

        #endregion

        #endregion
    }
}
