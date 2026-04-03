#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Debug.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2026 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

#region Used Namespaces

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using KGySoft.WinForms;

#endregion

#region Used Aliases

using SystemDebug = System.Diagnostics.Debug;

#endregion

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal static class Debug
    {
        #region Methods

#if !NETFRAMEWORK
        private static bool everAttached;
#endif

        [Conditional("DEBUG")]
        internal static void Assert(bool condition, [CallerArgumentExpression(nameof(condition))] string? message = null)
        {
#if NETFRAMEWORK
            if (OSHelper.IsMono)
            {
                if (condition)
                    return;
                Fail(message);
                return;
            }

            SystemDebug.Assert(condition, message);
#else
            if (!condition)
                Fail(message);
#endif
        }

        [Conditional("DEBUG")]
        internal static void Fail(string? message)
        {
#if NETFRAMEWORK
            if (OSHelper.IsMono)
            {
                message = $"Debug failure occurred - {message ?? "No message"}";
                WriteLine(message);
                throw new InvalidOperationException(message);
            }

            SystemDebug.Fail(message ?? "No message");
            SystemDebug.Fail(message);
#else
            SystemDebug.WriteLine("Debug failure occurred - " + (message ?? "No message"));

            // preventing the attach dialog come up if already attached it once
            if (!everAttached)
                everAttached = Debugger.IsAttached;
            if (!everAttached)
            {
                Debugger.Launch();
                everAttached = true;
            }
            else
                Debugger.Break();
#endif
        }

        [Conditional("DEBUG")]
        internal static void WriteLine(string? message = null)
        {
            if (OSHelper.IsMono || !Debugger.IsAttached)
                Console.WriteLine(message);
            else
                SystemDebug.WriteLine(message);
        }

        #endregion
    }
}