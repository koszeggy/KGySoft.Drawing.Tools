﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ShellDialogs.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

#endregion

#nullable enable

namespace KGySoft.Drawing.DebuggerVisualizers.Package
{
    internal static class ShellDialogs
    {
        #region Methods

        #region Internal Methods

        internal static void Error(IServiceProvider serviceProvider, string message)
            => ShowMessageBox(serviceProvider, message, OLEMSGICON.OLEMSGICON_CRITICAL);

        internal static void Warning(IServiceProvider serviceProvider, string message)
            => ShowMessageBox(serviceProvider, message, OLEMSGICON.OLEMSGICON_WARNING);

        internal static void Info(IServiceProvider serviceProvider, string message)
            => ShowMessageBox(serviceProvider, message, OLEMSGICON.OLEMSGICON_INFO);

        #endregion

        #region Private Methods

        private static void ShowMessageBox(IServiceProvider serviceProvider, string message, OLEMSGICON icon)
            => VsShellUtilities.ShowMessageBox(serviceProvider, message, Res.TitleMessageDialog, icon, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

        #endregion

        #endregion
    }
}
