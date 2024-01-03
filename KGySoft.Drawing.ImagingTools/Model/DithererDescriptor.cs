#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DithererDescriptor.cs
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
using System.Linq;
using System.Reflection;

#if NETFRAMEWORK
using KGySoft.CoreLibraries;
#endif
using KGySoft.Drawing.Imaging;
using KGySoft.Reflection;

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    internal sealed class DithererDescriptor
    {
        #region Fields

        private readonly CreateInstanceAccessor? ctor;
        private readonly ParameterInfo[]? parameters;
        private readonly PropertyAccessor? property;
        private readonly string displayName;

        #endregion

        #region Properties

        internal bool HasStrength { get; }
        internal bool HasSerpentineProcessing { get; }
        internal bool HasByBrightness { get; }
        internal bool HasSeed { get; }

        #endregion

        #region Constructors

        internal DithererDescriptor(Type type, string propertyName) : this(type.GetProperty(propertyName)!)
        {
        }

        internal DithererDescriptor(MemberInfo member)
        {
            switch (member)
            {
                case ConstructorInfo ci:
                    displayName = Res.Get(ci.DeclaringType!.Name);
                    parameters = ci.GetParameters();
                    ctor = CreateInstanceAccessor.GetAccessor(ci);
                    HasStrength = parameters.Any(p => p.Name == "strength");
                    HasSeed = parameters.Any(p => p.Name == "seed");
                    break;

                case PropertyInfo pi:
                    displayName = Res.Get($"{pi.DeclaringType!.Name}.{pi.Name}");
                    property = PropertyAccessor.GetAccessor(pi);
                    HasStrength = pi.DeclaringType == typeof(OrderedDitherer);
                    HasSerpentineProcessing = HasByBrightness = pi.DeclaringType == typeof(ErrorDiffusionDitherer);
                    break;

                default:
                    throw new ArgumentException($"Unexpected member: {member}");
            }
        }

        #endregion

        #region Methods

        #region Instance Methods

        #region Public Methods

        public override string ToString() => displayName;

        #endregion

        #region Internal Methods

        internal IDitherer Create(IDithererSettings settings)
        {
            IDitherer result;
            if (ctor != null)
            {
                object?[] args = new object[parameters!.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    args[i] = parameters[i].Name switch
                    {
                        "strength" => settings.Strength,
                        "seed" => settings.Seed,
                        _ => throw new InvalidOperationException($"Unhandled parameter: {parameters[i].Name}")
                    };
                }

                result = (IDitherer)ctor.CreateInstance(args);
            }
            else
            {
#if NET35
                // In .NET Framework 3.5 we cannot use the generic version due to the lack of Func<T> covariance
                result = (IDitherer)property!.Get(null)!;
#else
                result = property!.GetStaticValue<IDitherer>();
#endif
                result = result switch
                {
                    OrderedDitherer ordered => ordered.ConfigureStrength(settings.Strength),
                    ErrorDiffusionDitherer errorDiffusion => errorDiffusion.ConfigureErrorDiffusionMode(settings.ByBrightness)
                        .ConfigureProcessingDirection(settings.DoSerpentineProcessing),
                    _ => result
                };
            }

            return result;
        }

        #endregion

        #endregion

        #endregion
    }
}
