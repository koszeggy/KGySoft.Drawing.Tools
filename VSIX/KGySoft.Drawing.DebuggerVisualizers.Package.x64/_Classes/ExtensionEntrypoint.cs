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

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using EnvDTE;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools;
using KGySoft.Drawing.ImagingTools.Model;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

#endregion

#region Used Aliases

using GlobalProvider = Microsoft.VisualStudio.Shell.ServiceProvider;
using Process = System.Diagnostics.Process;

#endregion

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Package
{
    #region Usings

    using Resources = Properties.Resources;

    #endregion

    /// <summary>
    /// Extension entrypoint for the VisualStudio.Extensibility extension.
    /// </summary>
    [VisualStudioContribution]
    internal class ExtensionEntrypoint : Extension
    {
        #region Properties

        public override ExtensionConfiguration ExtensionConfiguration => new()
        {
            RequiresInProcessHosting = true,
            LoadedWhen = ActivationConstraint.SolutionState(SolutionState.FullyLoaded)
        };

        #endregion

        #region Methods

        #region Static Methods

        private static void CheckInstallations()
        {
            #region Local Methods

            // ReSharper disable RedundantDelegateCreation - not redundant for the x64 build where the implicit delegate creation would create Func<>
            static IVsInfoBarTextSpan[] GetReleaseNotesSpan() =>
            [
                new InfoBarTextSpan("\t"), new InfoBarHyperlink(Resources.InfoMessage_ChangeLog,
                    new Action(() => Process.Start(new ProcessStartInfo("https://github.com/koszeggy/KGySoft.Drawing.Tools/blob/master/changelog.txt") { UseShellExecute = true })))
            ];

            static IVsInfoBarActionItem[] GetOpenImagingToolsButton() =>
            [
                new InfoBarButton(Resources.InfoMessage_OpenImagingTools,
                    new Action(ExecuteImagingToolsCommand.ExecuteImagingTools))
            ];
            // ReSharper restore RedundantDelegateCreation

            #endregion

            ThreadHelper.ThrowIfNotOnUIThread();

            Version currentVersion = new(Ids.PackageVersion);
            Version? lastVersion = null;
            if (Configuration.LastVersion is string { Length: > 0 } ver)
                Version.TryParse(ver, out lastVersion);

            // Displaying notification about the new version
            if (lastVersion is null || lastVersion < currentVersion)
            {
                Notifications.Info(lastVersion is null ? Res.InfoMessagePackageInstalled(currentVersion) : Res.InfoMessagePackageUpgraded(lastVersion, currentVersion),
                    GetReleaseNotesSpan(), GetOpenImagingToolsButton());
                Configuration.LastVersion = Ids.PackageVersion;
                Configuration.SaveConfig();
            }

            if (Services.ShellService == null)
            {
                Notifications.Error(Resources.ErrorMessage_ShellServiceUnavailable);
                return;
            }

            // Checking and handling the installation of the classic visualizers
            Services.ShellService.GetProperty((int)__VSSPROPID2.VSSPROPID_VisualStudioDir, out object documentsDirObj);
            string documentsDir = documentsDirObj.ToString();
            string targetPath = Path.Combine(documentsDir, "Visualizers");
            InstallationInfo installedVersion = InstallationManager.GetInstallationInfo(targetPath);

            // Not installed: great, not needed for VS2022 and later
            if (!installedVersion.Installed)
                return;

            InstallationInfo availableVersion = InstallationManager.AvailableVersion;
            if (installedVersion.Version != null && installedVersion.Version >= availableVersion.Version)
                return;

            // Old version found (or version cannot be determined): taking it as an upgrade, trying to uninstall silently
            InstallationManager.Uninstall(targetPath, out string? error);
            if (error != null)
                Notifications.Error(Res.ErrorMessageFailedToUninstallClassic(targetPath, error));
        }

        #endregion

        #region Instance Methods

        protected override void InitializeServices(IServiceCollection serviceCollection)
        {
            typeof(Version).RegisterTypeConverter<VersionConverter>();
            base.InitializeServices(serviceCollection);
        }

        protected override async Task OnInitializedAsync(VisualStudioExtensibility extensibility, CancellationToken cancellationToken)
        {
            Services.ServiceProvider = GlobalProvider.GlobalProvider;
            Services.ShellService = await GlobalProvider.GetGlobalServiceAsync(typeof(SVsShell)) as IVsShell;
            Services.InfoBarUIFactory = await GlobalProvider.GetGlobalServiceAsync(typeof(SVsInfoBarUIFactory)) as IVsInfoBarUIFactory;
            Services.DTE = await GlobalProvider.GetGlobalServiceAsync(typeof(DTE)) as DTE;
            //Services.UIShell = await GlobalProvider.GetGlobalServiceAsync(typeof(SVsUIShell)) as IVsUIShell5;

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
#if DEBUG
            Notifications.Info("Debugging Visualizer Extensions"); 
#endif
            CheckInstallations();
            InitTheme();

            await base.OnInitializedAsync(extensibility, cancellationToken);
        }

        private static void InitTheme()
        {
            //ThemeColors.Control = VSColorTheme.GetThemedColor(CommonControlsColors.ButtonBrushKey);
            //ThemeColors.ControlText = VSColorTheme.GetThemedColor(CommonControlsColors.ButtonTextBrushKey);
        }

        #endregion

        #endregion
    }
}
