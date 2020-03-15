#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ManageDebuggerVisualizerInstallationsCommand.cs
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
using System.ComponentModel.Design;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.View;
using KGySoft.Drawing.ImagingTools.ViewModel;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Package
{
    internal static class ManageDebuggerVisualizerInstallationsCommand
    {
        #region Fields

        private static MenuCommand commandInstance;
        private static IServiceProvider serviceProvider;
        private static IVsShell shellService;
        private static ViewModelBase manageInstallationsViewModel;
        private static Form manageInstallationsView;

        #endregion

        #region Methods

        #region Internal Methods

        internal static MenuCommand GetCreateCommand(IServiceProvider package, IVsShell vsShell)
        {
            if (commandInstance == null)
            {
                serviceProvider = package;
                shellService = vsShell;
                commandInstance = new OleMenuCommand(OnExecuteImagingToolsCommand, new CommandID(Ids.CommandSet, Ids.ManageDebuggerVisualizerInstallationsCommandId));
            }

            return commandInstance;
        }

        internal static void DestroyCommand()
        {
            manageInstallationsView?.Dispose();
            manageInstallationsView = null;
            manageInstallationsViewModel?.Dispose();
            manageInstallationsViewModel = null;
        }

        #endregion

        #region Event handlers

        private static void OnExecuteImagingToolsCommand(object sender, EventArgs e)
        {
            try
            {
                if (manageInstallationsView == null || manageInstallationsView.IsDisposed)
                {
                    manageInstallationsViewModel?.Dispose();
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread - invoked in UI thread. And ThreadHelper.ThrowIfNotOnUIThread() just emits another warning.
                    shellService.GetProperty((int)__VSSPROPID2.VSSPROPID_VisualStudioDir, out object documentsDirObj);
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
                    manageInstallationsViewModel = ViewModelFactory.CreateManageInstallations(documentsDirObj?.ToString());
                    manageInstallationsView = ViewFactory.CreateView(manageInstallationsViewModel);
                    manageInstallationsView.Show();
                    return;
                }

                manageInstallationsView.Activate();
                manageInstallationsView.BringToFront();

            }
            catch (Exception ex)
            {
                manageInstallationsView?.Dispose();
                manageInstallationsViewModel?.Dispose();
                manageInstallationsView = null;
                ShellDialogs.Error(serviceProvider, $"Unexpected error occurred: {ex.Message}");
            }
        }

        #endregion

        #endregion
    }
}
