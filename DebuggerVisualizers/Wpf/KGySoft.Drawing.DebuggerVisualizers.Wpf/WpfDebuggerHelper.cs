#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: WpfDebuggerHelper.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2022 - All Rights Reserved
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

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Wpf
{
    /// <summary>
    /// A helper class to access the debugger visualizers of this assembly.
    /// </summary>
    public static class WpfDebuggerHelper
    {
        #region Methods

        /// <summary>
        /// Gets the debugger visualizers of this assembly.
        /// </summary>
        /// <returns>The debugger visualizers of this assembly.</returns>
        public static Dictionary<Type, DebuggerVisualizerAttribute> GetDebuggerVisualizers()
            => Attribute.GetCustomAttributes(typeof(WpfDebuggerHelper).Assembly, typeof(DebuggerVisualizerAttribute))
                .Cast<DebuggerVisualizerAttribute>().ToDictionary(a => a.Target!);

        #endregion
    }
}