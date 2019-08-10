#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ShellDialogs.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2019 - All Rights Reserved
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

using KGySoft.Drawing.DebuggerVisualizers.Package.Properties;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Package
{
    internal static class ShellDialogs
    {
        #region Methods

        #region Internal Methods

        internal static void Error(IServiceProvider serviceProvider, string message)
            => ShowMesasgeBox(serviceProvider, message, OLEMSGICON.OLEMSGICON_CRITICAL);

        internal static void Warning(IServiceProvider serviceProvider, string message)
            => ShowMesasgeBox(serviceProvider, message, OLEMSGICON.OLEMSGICON_WARNING);

        internal static void Info(IServiceProvider serviceProvider, string message)
            => ShowMesasgeBox(serviceProvider, message, OLEMSGICON.OLEMSGICON_INFO);

        #endregion

        #region Private Methods

        private static void ShowMesasgeBox(IServiceProvider serviceProvider, string message, OLEMSGICON icon)
            => VsShellUtilities.ShowMessageBox(serviceProvider, message, Resources.ResourceManager.GetString(Ids.ResourceTitle), icon, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

        #endregion

        #endregion
    }
}
