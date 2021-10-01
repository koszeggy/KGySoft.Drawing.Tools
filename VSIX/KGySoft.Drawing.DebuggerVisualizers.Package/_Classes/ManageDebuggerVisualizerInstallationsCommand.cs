#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ManageDebuggerVisualizerInstallationsCommand.cs
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
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;

using KGySoft.Drawing.ImagingTools.View;
using KGySoft.Drawing.ImagingTools.ViewModel;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

#endregion

#nullable enable

namespace KGySoft.Drawing.DebuggerVisualizers.Package
{
    internal static class ManageDebuggerVisualizerInstallationsCommand
    {
        #region Fields

        private static MenuCommand? commandInstance;
        private static IServiceProvider serviceProvider = default!;
        private static IVsShell? shellService;
        private static IView? manageInstallationsView;

        #endregion

        #region Methods

        #region Internal Methods

        internal static MenuCommand GetCreateCommand(IServiceProvider package, IVsShell? vsShell)
        {
            if (commandInstance == null)
            {
                serviceProvider = package;
                shellService = vsShell;
                commandInstance = new OleMenuCommand(OnExecuteManageDebuggerVisualizerInstallationsCommand, new CommandID(Ids.CommandSet, Ids.ManageDebuggerVisualizerInstallationsCommandId));
            }

            return commandInstance;
        }

        internal static void DestroyCommand()
        {
            manageInstallationsView?.Dispose();
            manageInstallationsView = null;
        }

        #endregion

        #region Event handlers

#if VS2022_OR_GREATER
        [SuppressMessage("Usage", "VSTHRD010:Invoke single-threaded types on Main thread", Justification = "False alarm, SwitchToMainThreadAsync is called from caller")]
#endif
        private static void OnExecuteManageDebuggerVisualizerInstallationsCommand(object sender, EventArgs e)
        {
            try
            {
                if (manageInstallationsView == null || manageInstallationsView.IsDisposed)
                {
                    object? documentsDirObj = null;
                    shellService?.GetProperty((int)__VSSPROPID2.VSSPROPID_VisualStudioDir, out documentsDirObj);
                    manageInstallationsView = ViewHelper.CreateViewInNewThread(() => ViewModelFactory.CreateManageInstallations(documentsDirObj?.ToString()));
                }
                else
                    manageInstallationsView.Show();
            }
            catch (Exception ex)
            {
                manageInstallationsView?.Dispose();
                manageInstallationsView = null;
                ShellDialogs.Error(serviceProvider, Res.ErrorMessageUnexpectedError(ex.Message));
            }
        }

        #endregion

        #endregion
    }
}
