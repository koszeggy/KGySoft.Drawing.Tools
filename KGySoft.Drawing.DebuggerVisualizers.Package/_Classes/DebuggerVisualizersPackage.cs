#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DebuggerVisualizersPackage.cs
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
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using KGySoft.Drawing.DebuggerVisualizers.Package.Properties;
using KGySoft.Drawing.ImagingTools;
using KGySoft.Drawing.ImagingTools.Model;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Package
{
    #region Usings

    using KGySoft.Drawing.DebuggerVisualizers.Package.Properties;

    #endregion

    [Guid(Ids.PackageGuidString)]
    [ProvideBindingPath]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [InstalledProductRegistration("#" + Ids.ResourceTitle, "#" + Ids.ResourceDetails, Ids.Version, IconResourceID = Ids.IconResourceId)]
    [PackageRegistrationAync]
    [ProvideAutoLoadAsync]
    public sealed class DebuggerVisualizersPackage : Microsoft.VisualStudio.Shell.Package, IAsyncLoadablePackageInitialize
    {
        #region Fields

        private bool initialized;

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// The async initialization in VS2015 and above.
        /// </summary>
        public IVsTask Initialize(IAsyncServiceProvider pServiceProvider, IProfferAsyncService pProfferService, IAsyncProgressCallback pProgressCallback)
        {
            return ThreadHelper.JoinableTaskFactory.RunAsync<object>(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                var shellService = await GetServiceAsync<IVsShell>(pServiceProvider, typeof(SVsShell));
                var menuCommandService = await GetServiceAsync<IMenuCommandService>(pServiceProvider, typeof(IMenuCommandService));
                DoInitialize(shellService, menuCommandService);
                return null;
            }).AsVsTask();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// The legacy initialization for VS2013.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

#pragma warning disable VSTHRD108, VSTHRD010 // Invoke single-threaded types on Main thread: Initialize is on Main thread (see also the assert and IAsyncLoadablePackageInitialize.Initialize)

            // returning if async initialization is supported
            Debug.Assert(ThreadHelper.CheckAccess());
            if (GetService(typeof(SAsyncServiceProvider)) is IAsyncServiceProvider)
                return;

            var uiShellService = GetService(typeof(SVsShell)) as IVsShell;
            var menuCommandService = GetService(typeof(IMenuCommandService)) as IMenuCommandService;
#pragma warning restore VSTHRD010, VSTHRD010 // Invoke single-threaded types on Main thread

            DoInitialize(uiShellService, menuCommandService);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing)
                return;
            DestroyCommands();
        }

        #endregion

        #region Private Methods

        private async Task<T> GetServiceAsync<T>(IAsyncServiceProvider asyncServiceProvider, Type serviceType) where T : class
        {
            T result = null;
            await ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                Guid serviceTypeGuid = serviceType.GUID;
                object serviceInstance = await asyncServiceProvider.QueryServiceAsync(ref serviceTypeGuid);
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                result = serviceInstance as T ?? GetService(serviceType) as T;
            });

            return result;
        }

        private void DoInitialize(IVsShell shellService, IMenuCommandService menuCommandService)
        {
            if (initialized)
                return;

            initialized = true;
            InstallIfNeeded(shellService);
            InitCommands(shellService, menuCommandService);
        }

        private void InstallIfNeeded(IVsShell shellService)
        {
            if (shellService == null)
            {
                ShellDialogs.Error(this, "Shell service could not be obtained. Installation of the debugger visualizers cannot be checked.");
                return;
            }

            ThreadHelper.ThrowIfNotOnUIThread();
            shellService.GetProperty((int)__VSSPROPID2.VSSPROPID_VisualStudioDir, out object documentsDirObj);
            string documentsDir = documentsDirObj.ToString();
            string targetPath = Path.Combine(documentsDir, "Visualizers");
            InstallationInfo installationInfo = InstallationManager.GetInstallationInfo(targetPath);
            if (installationInfo.Installed && (installationInfo.Version == null || installationInfo.Version >= typeof(InstallationManager).Assembly.GetName().Version))
                return;

            InstallationManager.Install(targetPath, out string error);
            if (error != null)
            {
                ShellDialogs.Warning(this, $"Failed to install the visualizers to {targetPath}: {error}{Environment.NewLine}{Environment.NewLine}Make sure every running debugger is closed. Installing will be tried again on restarting Visual Studio.");
                return;
            }

            ShellDialogs.Info(this, $"{Resources.ResourceManager.GetString(Ids.ResourceTitle)} {installationInfo.Version} has been installed to {targetPath}.");
        }

        private void InitCommands(IVsShell shellService, IMenuCommandService menuCommandService)
        {
            menuCommandService.AddCommand(ExecuteImagingToolsCommand.GetCreateCommand(this));
            menuCommandService.AddCommand(ManageDebuggerVisualizerInstallationsCommand.GetCreateCommand(this, shellService));
        }

        private void DestroyCommands()
        {
            ExecuteImagingToolsCommand.DestroyCommand();
            ManageDebuggerVisualizerInstallationsCommand.DestroyCommand();
        }

        #endregion

        #endregion
    }
}
