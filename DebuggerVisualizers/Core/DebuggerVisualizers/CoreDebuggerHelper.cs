#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: GdiPlusDebuggerHelper.cs
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using KGySoft.Reflection;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Core
{
    /// <summary>
    /// A helper class to access the debugger visualizers of this assembly.
    /// </summary>
    public static class CoreDebuggerHelper
    {
        #region Methods

        /// <summary>
        /// Gets the debugger visualizers of this assembly.
        /// </summary>
        /// <returns>The debugger visualizers of this assembly.</returns>
        public static Dictionary<Type, DebuggerVisualizerAttribute> GetDebuggerVisualizers()
            => Attribute.GetCustomAttributes(typeof(CoreDebuggerHelper).Assembly, typeof(DebuggerVisualizerAttribute))
                .Cast<DebuggerVisualizerAttribute>().ToDictionary(a => a.Target ?? Reflector.ResolveType(a.TargetTypeName!, ResolveTypeOptions.AllowPartialAssemblyMatch)!);

        #endregion
    }
}
