#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DebuggerVisualizersPackage.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2024 - All Rights Reserved
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
#if !VS2022_OR_GREATER
using System.Diagnostics;
#endif
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using KGySoft.Drawing.ImagingTools;
using KGySoft.Drawing.ImagingTools.Model;

#if VS2022_OR_GREATER
using Microsoft.VisualStudio; 
#endif
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

#endregion

#nullable enable

namespace KGySoft.Drawing.DebuggerVisualizers.Package
{
    #region Usings

    using Resources = Properties.Resources;

    #endregion

    [Guid(Ids.PackageGuidString)]
    [ProvideBindingPath]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [InstalledProductRegistration("#" + Ids.ResourceTitle, "#" + Ids.ResourceDetails, Ids.Version, IconResourceID = Ids.IconResourceId)]
#if VS2022_OR_GREATER
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string, PackageAutoLoadFlags.BackgroundLoad)]
#else
    [PackageRegistrationAsync]
    [ProvideAutoLoadAsync]
#endif
    public sealed class DebuggerVisualizersPackage :
#if VS2022_OR_GREATER
        AsyncPackage
#else
        Microsoft.VisualStudio.Shell.Package, IAsyncLoadablePackageInitialize // VS2023-2019
#endif
    {
        #region Fields

        private bool initialized;

        #endregion

        #region Methods

        #region Public Methods

#if !VS2022_OR_GREATER
        /// <summary>
        /// The async initialization in VS2015-2019
        /// </summary>
        public IVsTask Initialize(IAsyncServiceProvider pServiceProvider, IProfferAsyncService pProfferService, IAsyncProgressCallback pProgressCallback)
        {
            return ThreadHelper.JoinableTaskFactory.RunAsync<object?>(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                var shellService = await GetServiceAsync<IVsShell>(pServiceProvider, typeof(SVsShell));
                var menuCommandService = await GetServiceAsync<IMenuCommandService>(pServiceProvider, typeof(IMenuCommandService));
                DoInitialize(shellService, menuCommandService);
                return null;
            }).AsVsTask();
        }
#endif

        #endregion

        #region Protected Methods

#if !VS2022_OR_GREATER
        /// <summary>
        /// The legacy initialization for VS2013. For versions 2015-2019 an explicit IAsyncLoadablePackageInitialize implementation is used.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            // returning if async initialization is supported
            Debug.Assert(ThreadHelper.CheckAccess());
            if (GetService(typeof(SAsyncServiceProvider)) is IAsyncServiceProvider)
                return;

            var uiShellService = GetService(typeof(SVsShell)) as IVsShell;
            var menuCommandService = GetService(typeof(IMenuCommandService)) as IMenuCommandService;

            DoInitialize(uiShellService, menuCommandService);
        }
#else
        /// <summary>
        /// Initialization in VS2022 and above.
        /// </summary>
        protected override async Task InitializeAsync(System.Threading.CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var shellService = await GetServiceAsync(typeof(SVsShell)) as IVsShell;
            var menuCommandService = await GetServiceAsync(typeof(IMenuCommandService)) as IMenuCommandService;
            DoInitialize(shellService, menuCommandService);
        }
#endif

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing)
                return;
            ExecuteImagingToolsCommand.DestroyCommand();
            ManageDebuggerVisualizerInstallationsCommand.DestroyCommand();
        }

        #endregion

        #region Private Methods

#if !VS2022_OR_GREATER
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
#endif

        private void DoInitialize(IVsShell? shellService, IMenuCommandService? menuCommandService)
        {
            if (initialized)
                return;

            initialized = true;
            InstallIfNeeded(shellService);
            InitCommands(shellService, menuCommandService);
        }

        private void InstallIfNeeded(IVsShell? shellService)
        {
            if (shellService == null)
            {
                ShellDialogs.Error(this, Resources.ErrorMessage_ShellServiceUnavailable);
                return;
            }

            ThreadHelper.ThrowIfNotOnUIThread();
            shellService.GetProperty((int)__VSSPROPID2.VSSPROPID_VisualStudioDir, out object documentsDirObj);
            string documentsDir = documentsDirObj.ToString();
            string targetPath = Path.Combine(documentsDir, "Visualizers");
            InstallationInfo installedVersion = InstallationManager.GetInstallationInfo(targetPath);
            InstallationInfo availableVersion = InstallationManager.AvailableVersion;
            if (installedVersion.Installed && (installedVersion.Version == null || installedVersion.Version >= availableVersion.Version))
                return;

            InstallationManager.Install(targetPath, out string? error, out string? warning);
            if (error != null)
                ShellDialogs.Error(this, Res.ErrorMessageFailedToInstall(targetPath, error));
            else if (warning != null)
                ShellDialogs.Warning(this, Res.WarningMessageInstallationFinishedWithWarning(targetPath, warning));
            else if (installedVersion.Installed && installedVersion.Version != null)
                ShellDialogs.Info(this, Res.InfoMessageUpgradeFinished(installedVersion.Version, availableVersion.Version!, targetPath));
            else
                ShellDialogs.Info(this, Res.InfoMessageInstallationFinished(availableVersion.Version!, targetPath));
        }

        private void InitCommands(IVsShell? shellService, IMenuCommandService? menuCommandService)
        {
            // no menu point will be added
            if (menuCommandService == null)
                return;

            menuCommandService.AddCommand(ExecuteImagingToolsCommand.GetCreateCommand(this));
            menuCommandService.AddCommand(ManageDebuggerVisualizerInstallationsCommand.GetCreateCommand(this, shellService));
        }

        #endregion

        #endregion
    }
}
