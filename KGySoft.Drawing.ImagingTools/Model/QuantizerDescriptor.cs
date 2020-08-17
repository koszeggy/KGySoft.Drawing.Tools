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
using KGySoft.Reflection;

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    internal sealed class QuantizerDescriptor
    {
        #region Fields

        private static readonly Dictionary<string, CustomPropertyDescriptor> parametersMapping = new Dictionary<string, CustomPropertyDescriptor>
        {
            ["backColor"] = new CustomPropertyDescriptor("backColor", typeof(Color)) { DefaultValue = Color.Empty },
            ["alphaThreshold"] = new CustomPropertyDescriptor("alphaThreshold", typeof(byte)) { DefaultValue = (byte)128 },
            ["whiteThreshold"] = new CustomPropertyDescriptor("whiteThreshold", typeof(byte)) { DefaultValue = (byte)128 },
            ["palette"] = new CustomPropertyDescriptor("palette", typeof(Color[]))
            {
                DefaultValue = Reflector.EmptyArray<Color>(),
                AdjustValue = value => value is Color[] arr && arr.Length > 0 ? arr : new[] { Color.Black, Color.White }
            },
            ["pixelFormat"] = new CustomPropertyDescriptor("pixelFormat", typeof(PixelFormat))
            {
                AllowedValues = Enum<PixelFormat>.GetValues().Where(pf => pf.IsValidFormat()).OrderBy(pf => pf & PixelFormat.Max).Select(pf => (object)pf).ToArray(),
            },
            ["directMapping"] = new CustomPropertyDescriptor("directMapping", typeof(bool)) { DefaultValue = false },
            ["maxColors"] = new CustomPropertyDescriptor("maxColors", typeof(int))
            {
                DefaultValue = 256,
                AdjustValue = value => value is int i && i >= 2 && i <= 256 ? i : 256
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

        public override string ToString() => $"{Method.DeclaringType.Name}.{Method.Name}";

        #endregion
    }
}
