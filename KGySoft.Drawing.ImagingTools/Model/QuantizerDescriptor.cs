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
using System.Drawing;
using System.Reflection;

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    internal sealed class QuantizerDescriptor
    {
        #region Fields

        private static readonly CustomPropertyDescriptor backColor = new CustomPropertyDescriptor(nameof(backColor), typeof(Color));

        private static readonly CustomPropertyDescriptor alphaThreshold = new CustomPropertyDescriptor(nameof(alphaThreshold), typeof(byte))
        {
            DefaultValue = (byte)128
        };

        private static readonly CustomPropertyDescriptor whiteThreshold = new CustomPropertyDescriptor(nameof(whiteThreshold), typeof(byte))
        {
            DefaultValue = (byte)128
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
                Parameters[i] = GetParameterDescriptor(methodParams[i]);
        }

        #endregion

        #region Methods

        #region Static Methods

        private static CustomPropertyDescriptor GetParameterDescriptor(ParameterInfo methodParam) =>
            methodParam.Name switch
            {
                nameof(backColor) => backColor,
                nameof(alphaThreshold) => alphaThreshold,
                nameof(whiteThreshold) => whiteThreshold,
                _ => throw new InvalidOperationException(Res.InternalError($"Unexpected parameter: {methodParam}"))
            };

        #endregion

        #region Instance Methods

        public override string ToString() => $"{Method.DeclaringType.Name}.{Method.Name}";

        #endregion

        #endregion
    }
}
