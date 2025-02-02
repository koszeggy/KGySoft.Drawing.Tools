#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: TypeExtensions.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2025 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Reflection;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal static class TypeExtensions
    {
        #region Methods

        internal static MethodInfo GetVisibleMethod(this Type type, string methodName)
        {
            MemberInfo[] methods = type.GetMember(methodName, MemberTypes.Method, BindingFlags.Public | BindingFlags.Static);
            foreach (MemberInfo method in methods)
            {
                if (!Attribute.IsDefined(method, typeof(ObsoleteAttribute)))
                    return (MethodInfo)method;
            }

            throw new ArgumentException(Res.InternalError($"Method not found: {methodName}"), nameof(methodName));
        }

        #endregion
    }
}