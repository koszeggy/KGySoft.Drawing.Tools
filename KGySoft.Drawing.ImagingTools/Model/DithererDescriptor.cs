#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DithererDescriptor.cs
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
using KGySoft.Drawing.Imaging;
using KGySoft.Reflection;

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    internal sealed class DithererDescriptor
    {
        #region Fields

        private static readonly Dictionary<string, CustomPropertyDescriptor> parametersMapping = new Dictionary<string, CustomPropertyDescriptor>
        {
            //["matrix"] = new CustomPropertyDescriptor("matrix", typeof(byte[,]))
            //{
            //    DefaultValue = new byte[,] { { 0, 1 }, { 2, 3 } }
            //},
            ["strength"] = new CustomPropertyDescriptor("strength", typeof(float))
            {
                DefaultValue = 0f,
                AdjustValue = value => value is float f && f >= 0f && f <= 1f ? f : 0f,
                UITypeEditor = DesignDependencies.DithererStrengthEditor
            },
            //["divisor"] = new CustomPropertyDescriptor("divisor", typeof(int)) { DefaultValue = 16 },
            //["matrixFirstPixelIndex"] = new CustomPropertyDescriptor("matrixFirstPixelIndex", typeof(int)) { DefaultValue = 0 },
            //["serpentineProcessing"] = new CustomPropertyDescriptor("serpentineProcessing", typeof(bool)) { DefaultValue = false },
            ["serpentine"] = new CustomPropertyDescriptor("serpentine", typeof(bool)) { DefaultValue = false },
            ["byBrightness"] = new CustomPropertyDescriptor("byBrightness", typeof(bool?)),
            ["seed"] = new CustomPropertyDescriptor("seed", typeof(int?)),
        };

        #endregion

        #region Properties

        internal List<MemberInfo> InvokeChain { get; }

        internal List<CustomPropertyDescriptor> Parameters { get; }

        #endregion

        #region Constructors

        internal DithererDescriptor(Type type, string propertyName) : this(type.GetProperty(propertyName))
        {
        }

        internal DithererDescriptor(MemberInfo member)
        {
            var chain = new List<MemberInfo> { member };
            var parameters = new List<CustomPropertyDescriptor>();

            switch (member)
            {
                case ConstructorInfo ctor:
                    AddParameters(parameters, ctor.GetParameters());
                    break;
                case PropertyInfo property:
                    if (property.DeclaringType == typeof(OrderedDitherer))
                        AddMethodChain(chain, parameters, typeof(OrderedDitherer).GetMethod(nameof(OrderedDitherer.ConfigureStrength)));
                    else if (property.DeclaringType == typeof(ErrorDiffusionDitherer))
                    {
                        AddMethodChain(chain, parameters, typeof(ErrorDiffusionDitherer).GetMethod(nameof(ErrorDiffusionDitherer.ConfigureProcessingDirection)));
                        AddMethodChain(chain, parameters, typeof(ErrorDiffusionDitherer).GetMethod(nameof(ErrorDiffusionDitherer.ConfigureErrorDiffusionMode)));
                    }

                    break;
                default:
                    throw new InvalidOperationException(Res.InternalError($"Unexpected member: {member}"));
            }

            InvokeChain = chain;
            Parameters = parameters;
        }

        #endregion

        #region Methods

        #region Static Methods

        private static void AddParameters(List<CustomPropertyDescriptor> descriptors, ParameterInfo[] reflectedParameters)
        {
            foreach (ParameterInfo pi in reflectedParameters)
                descriptors.Add(parametersMapping.GetValueOrDefault(pi.Name) ?? throw new InvalidOperationException(Res.InternalError($"Unexpected parameter: {pi.Name}")));
        }

        private static void AddMethodChain(List<MemberInfo> chain, List<CustomPropertyDescriptor> parameters, MethodInfo method)
        {
            chain.Add(method);
            AddParameters(parameters, method.GetParameters());
        }

        #endregion

        #region Instance Methods

        #region Public Methods

        public override string ToString() => InvokeChain[0] is ConstructorInfo ctor
            ? ctor.DeclaringType.Name
            : $"{InvokeChain[0].DeclaringType.Name}.{InvokeChain[0].Name}";

        #endregion

        #region Internal Methods

        internal object[] EvaluateParameters(ParameterInfo[] parameters, CustomPropertiesObject values)
        {
            var result = new object[parameters.Length];
            for (int i = 0; i < result.Length; i++)
            {
                string paramName = parameters[i].Name;
                result[i] = Parameters.Find(d => d.Name == paramName).GetValue(values);
            }

            return result;
        }

        #endregion

        #endregion

        #endregion
    }
}
