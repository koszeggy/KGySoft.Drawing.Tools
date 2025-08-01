#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DebuggerVisualizersPackage.cs
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
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using EnvDTE;

using KGySoft.Drawing.ImagingTools;
using KGySoft.Drawing.ImagingTools.Model;

using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

#endregion

#region Used Aliases

using Process = System.Diagnostics.Process;

#endregion

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Package
{
    #region Usings

    using Resources = Properties.Resources;

    #endregion

    [Guid(Ids.PackageGuidString)]
    [ProvideBindingPath]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [InstalledProductRegistration("#" + Ids.ResourceTitle, "#" + Ids.ResourceDetails, Ids.Version, IconResourceID = Ids.IconResourceId)]
    [PackageRegistrationAsync]
    [ProvideAutoLoadAsync]
    public sealed class DebuggerVisualizersPackage : Microsoft.VisualStudio.Shell.Package, IAsyncLoadablePackageInitialize
    {
        #region Fields

        private bool initialized;

        #endregion

        #region Methods

        #region Static Methods

        #region Private Methods

        private static void ResetTheme()
        {
            // Unlike in the x64 package, here we cannot use CommonControlsColors, because that's in a package that older versions of Visual Studio cannot reference.
            // Note that this won't work the classic debugger visualizers, only for the ImagingTools instances opened by the package commands.
            ThemeColors.SetBaseTheme(VSColorTheme.GetThemedColor(EnvironmentColors.SystemButtonFaceBrushKey).GetBrightness() < 0.5f ? DefaultTheme.Dark : DefaultTheme.Classic);
        }

        #endregion

        #region Event Handlers

        private static void VSColorTheme_ThemeChanged(ThemeChangedEventArgs e) => ResetTheme();

        #endregion

        #endregion


        #region Instance Methods

        #region Public Methods

        /// <summary>
        /// The async initialization in VS2015-2019
        /// </summary>
        public IVsTask Initialize(IAsyncServiceProvider pServiceProvider, IProfferAsyncService pProfferService, IAsyncProgressCallback pProgressCallback)
        {
            return ThreadHelper.JoinableTaskFactory.RunAsync<object?>(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                Services.ShellService = await GetServiceAsync<IVsShell>(pServiceProvider, typeof(SVsShell));
                Services.MenuCommandService = await GetServiceAsync<IMenuCommandService>(pServiceProvider, typeof(IMenuCommandService));
                Services.InfoBarUIFactory = await GetServiceAsync<IVsInfoBarUIFactory>(pServiceProvider, typeof(SVsInfoBarUIFactory));
                Services.DTE = await GetServiceAsync<DTE>(pServiceProvider, typeof(DTE));
                DoInitialize();
                return null;
            }).AsVsTask();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// The legacy initialization for VS2013. For versions 2015-2019 the implicit IAsyncLoadablePackageInitialize implementation is used.
        /// </summary>
        protected override void Initialize()
        {
            Services.ServiceProvider = this;
            base.Initialize();

            // returning if async initialization is supported (initialization is done in public Initialize)
            Debug.Assert(ThreadHelper.CheckAccess());
            if (GetService(typeof(SAsyncServiceProvider)) is IAsyncServiceProvider)
                return;

            Services.ShellService = GetService(typeof(SVsShell)) as IVsShell;
            Services.MenuCommandService = GetService(typeof(IMenuCommandService)) as IMenuCommandService;
            Services.InfoBarUIFactory = GetService(typeof(SVsInfoBarUIFactory)) as IVsInfoBarUIFactory;
            Services.DTE = GetService(typeof(DTE)) as DTE;

            DoInitialize();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing)
                return;
            ExecuteImagingToolsCommand.DestroyCommand();
            ManageDebuggerVisualizerInstallationsCommand.DestroyCommand();
            VSColorTheme.ThemeChanged -= VSColorTheme_ThemeChanged;
        }

        #endregion

        #region Private Methods

        private async Task<T?> GetServiceAsync<T>(IAsyncServiceProvider asyncServiceProvider, Type serviceType) where T : class
        {
            T? result = null;
            await ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                Guid serviceTypeGuid = serviceType.GUID;
                object serviceInstance = await asyncServiceProvider.QueryServiceAsync(ref serviceTypeGuid);
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                result = serviceInstance as T ?? GetService(serviceType) as T;
            });

            return result;
        }

        private void DoInitialize()
        {
            if (initialized)
                return;

            initialized = true;
            InstallIfNeeded();
            InitCommands();
            VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged;
            ResetTheme();
        }

        private void InstallIfNeeded()
        {
            #region Local Methods

            // ReSharper disable RedundantDelegateCreation - not redundant for the x64 build where the implicit delegate creation would create Func<>
            static IVsInfoBarTextSpan[] GetReleaseNotesSpan() => [new InfoBarTextSpan("\t"), new InfoBarHyperlink(Resources.InfoMessage_ChangeLog,
                new Action(() => Process.Start(new ProcessStartInfo("https://github.com/koszeggy/KGySoft.Drawing.Tools/blob/master/changelog.txt") { UseShellExecute = true })))];

            static IVsInfoBarActionItem[] GetOpenImagingToolsButton() => [new InfoBarButton(Resources.InfoMessage_OpenImagingTools,
                new Action(ExecuteImagingToolsCommand.ExecuteImagingTools))];
            // ReSharper restore RedundantDelegateCreation

            #endregion

            Debug.Assert(initialized);

            if (Services.ShellService == null)
            {
                Notifications.Error(Resources.ErrorMessage_ShellServiceUnavailable);
                return;
            }

            ThreadHelper.ThrowIfNotOnUIThread();
            Services.ShellService.GetProperty((int)__VSSPROPID2.VSSPROPID_VisualStudioDir, out object documentsDirObj);
            string documentsDir = documentsDirObj.ToString();
            string targetPath = Path.Combine(documentsDir, "Visualizers");
            InstallationInfo installedVersion = InstallationManager.GetInstallationInfo(targetPath);
            InstallationInfo availableVersion = InstallationManager.AvailableVersion;
            if (installedVersion.Installed && (installedVersion.Version == null || installedVersion.Version >= availableVersion.Version))
                return;

            InstallationManager.Install(targetPath, out string? error, out string? warning);
            if (error != null)
                Notifications.Error(Res.ErrorMessageFailedToInstall(targetPath, error));
            else if (warning != null)
                Notifications.Warning(Res.WarningMessageInstallationFinishedWithWarning(targetPath, warning));
            else if (installedVersion.Installed && installedVersion.Version != null)
            {
                Notifications.Info(Res.InfoMessageUpgradeFinished(installedVersion.Version, availableVersion.Version!, targetPath),
                    GetReleaseNotesSpan(), GetOpenImagingToolsButton());
            }
            else
            {
                Notifications.Info(Res.InfoMessageInstallationFinished(availableVersion.Version!, targetPath),
                    GetReleaseNotesSpan(), GetOpenImagingToolsButton());
            }
        }

        private void InitCommands()
        {
            Debug.Assert(initialized);

            // no menu point will be added
            if (Services.MenuCommandService == null)
                return;

            Services.MenuCommandService.AddCommand(ExecuteImagingToolsCommand.GetCreateCommand());
            Services.MenuCommandService.AddCommand(ManageDebuggerVisualizerInstallationsCommand.GetCreateCommand());
        }

        #endregion

        #endregion

        #endregion
    }
}
