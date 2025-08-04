#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Services.cs
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
using System.ComponentModel.Design;

using EnvDTE;

using Microsoft.VisualStudio.Shell.Interop;

#endregion

#if !VS2022_OR_GREATER
#nullable enable
#endif

namespace KGySoft.Drawing.DebuggerVisualizers.Package
{
    /// <summary>
    /// Contains the services used by the package. Mind the initializations from 3 different places!
    /// </summary>
    internal static class Services
    {
        #region Properties

        internal static IServiceProvider ServiceProvider { get; set; } = default!;
        internal static IVsShell? ShellService { get; set; }
        internal static IMenuCommandService? MenuCommandService { get; set; }
        internal static IVsInfoBarUIFactory? InfoBarUIFactory { get; set; }
        internal static DTE? DTE { get; set; }

        #endregion
    }
}