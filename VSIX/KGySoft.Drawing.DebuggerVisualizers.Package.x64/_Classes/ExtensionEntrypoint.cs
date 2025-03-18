#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ExtensionEntrypoint.cs
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

#region Used Namespaces

using System.Threading;
using System.Threading.Tasks;

using EnvDTE;

using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

#endregion

#region Used Aliases

using GlobalProvider = Microsoft.VisualStudio.Shell.ServiceProvider;

#endregion

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Package
{
    /// <summary>
    /// Extension entrypoint for the VisualStudio.Extensibility extension.
    /// </summary>
    [VisualStudioContribution]
    internal class ExtensionEntrypoint : Microsoft.VisualStudio.Extensibility.Extension
    {
        #region Properties

        public override ExtensionConfiguration ExtensionConfiguration => new()
        {
            RequiresInProcessHosting = true,
            LoadedWhen = ActivationConstraint.SolutionState(SolutionState.FullyLoaded)
        };

        #endregion

        #region Methods

        protected override async Task OnInitializedAsync(VisualStudioExtensibility extensibility, CancellationToken cancellationToken)
        {
            Services.ServiceProvider = GlobalProvider.GlobalProvider;
            Services.ShellService = await GlobalProvider.GetGlobalServiceAsync(typeof(SVsShell)) as IVsShell;
            Services.InfoBarUIFactory = await GlobalProvider.GetGlobalServiceAsync(typeof(SVsInfoBarUIFactory)) as IVsInfoBarUIFactory;
            Services.DTE = await GlobalProvider.GetGlobalServiceAsync(typeof(DTE)) as DTE;

            // TODO: check legacy visualizers installation. If old version found, choose [Upgrade|Uninstall|Don't ask again]
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            Notifications.Info("Hello from Extension");

            await base.OnInitializedAsync(extensibility, cancellationToken);
        }

        #endregion
    }
}
