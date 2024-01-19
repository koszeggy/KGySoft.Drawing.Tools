#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ManageInstallationsViewModel.cs
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
using System.Collections.Generic;
using System.IO;
using System.Linq;

using KGySoft.ComponentModel;
#if NETFRAMEWORK
using KGySoft.CoreLibraries; 
#endif
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class ManageInstallationsViewModel : ViewModelBase
    {
        #region Constants

        private const string visualStudioName = "Visual Studio";
        private const string installDirsPattern = visualStudioName + " ????";
        private const string visualizersDir = "Visualizers";

        #endregion

        #region Fields

        private InstallationInfo currentStatus = default!;
        private bool isSelectingPath;

        #endregion

        #region Properties

        internal IList<KeyValuePair<string, string>> Installations { get => Get<IList<KeyValuePair<string, string>>>(); set => Set(value); }
        internal string SelectedInstallation { get => Get(String.Empty); set => Set(value); }
        internal string CurrentPath { get => Get(String.Empty); set => Set(value); }
        internal string StatusText { get => Get("–"); set => Set(value); }
        internal string AvailableVersionText { get => Get("–"); set => Set(value); }

        internal Func<string?>? SelectFolderCallback { get => Get<Func<string?>?>(); set => Set(value); }

        internal ICommandState SelectFolderCommandState => Get(() => new CommandState());
        internal ICommandState InstallCommandState => Get(() => new CommandState());
        internal ICommandState RemoveCommandState => Get(() => new CommandState());

        internal ICommand SelectFolderCommand => Get(() => new SimpleCommand(OnSelectFolderCommand));
        internal ICommand InstallCommand => Get(() => new SimpleCommand(OnInstallCommand));
        internal ICommand RemoveCommand => Get(() => new SimpleCommand(OnRemoveCommand));

        #endregion

        #region Constructors

        internal ManageInstallationsViewModel(string? hintPath)
        {
            InitAvailableVersion();
            InitInstallations();
            TrySelectPath(hintPath);
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == nameof(SelectedInstallation))
            {
                UpdatePath((string)e.NewValue!);
                return;
            }

            if (e.PropertyName == nameof(CurrentPath))
                UpdateStatus((string)e.NewValue!);
        }

        protected override void ApplyDisplayLanguage()
        {
            InitAvailableVersion();
            string currentPath = CurrentPath;
            InitInstallations();
            CurrentPath = currentPath;
            UpdateStatus(currentPath);
        }

        #endregion

        #region Private Methods

        private void InitAvailableVersion()
        {
            InstallationInfo availableVersion = InstallationManager.AvailableVersion;
            bool available = availableVersion.Installed;
            if (!available)
                AvailableVersionText = Res.InstallationNotAvailable;
            else if (availableVersion.Version == null)
                AvailableVersionText = Res.InstallationsStatusUnknown;
            else
                AvailableVersionText = availableVersion.TargetFramework != null ? Res.InstallationsAvailableWithTargetFramework(availableVersion.Version, availableVersion.TargetFramework)
                    : availableVersion.RuntimeVersion != null ? Res.InstallationsAvailableWithRuntime(availableVersion.Version, availableVersion.RuntimeVersion)
                    : Res.InstallationAvailable(availableVersion.Version);
            InstallCommandState.Enabled = available;
        }

        private void TrySelectPath(string? hintPath)
        {
            if (hintPath?.Contains(visualStudioName, StringComparison.Ordinal) == true)
            {
                string preferredPath = Path.GetFileName(hintPath) == visualizersDir ? Path.GetDirectoryName(hintPath)! : hintPath;
                SelectInstallation(preferredPath);
                return;
            }

            IList<KeyValuePair<string, string>> installations = Installations;
            string selected = (installations.Count > 1 ? installations[installations.Count - 2] : installations.Last()).Key;
            SelectInstallation(selected);
        }

        private void InitInstallations()
        {
            string docsDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var list = new List<KeyValuePair<string, string>>();
            list.AddRange(Directory.GetDirectories(docsDir, installDirsPattern).Select(d => new KeyValuePair<string, string>(d, Path.GetFileName(d))));
            list.Sort((d1, d2) => String.CompareOrdinal(d1.Value, d2.Value));
            list.Add(new KeyValuePair<string, string>(String.Empty, Res.InstallationsCustomDir));
            Installations = list;
        }

        private void SelectInstallation(string path)
        {
            isSelectingPath = true;
            try
            {
                if (Installations.Any(i => i.Key == path))
                    SelectedInstallation = path;
                else
                {
                    SelectedInstallation = String.Empty;
                    CurrentPath = path;
                }
            }
            finally
            {
                isSelectingPath = false;
            }
        }

        private void UpdatePath(string dir)
        {
            bool isCustom = dir.Length == 0;
            SelectFolderCommandState.Enabled = isCustom;
            if (isCustom && !isSelectingPath)
                SelectFolder();
            else
                CurrentPath = Path.Combine(dir, visualizersDir);
        }

        private void UpdateStatus(string path)
        {
            currentStatus = InstallationManager.GetInstallationInfo(path);
            if (!currentStatus.Installed)
                StatusText = Res.InstallationsStatusNotInstalled;
            else if (currentStatus.Version == null)
                StatusText = Res.InstallationsStatusUnknown;
            else
                StatusText = currentStatus.TargetFramework != null ? Res.InstallationsStatusInstalledWithTargetFramework(currentStatus.Version, currentStatus.TargetFramework)
                    : currentStatus.RuntimeVersion != null ? Res.InstallationsStatusInstalledWithRuntime(currentStatus.Version, currentStatus.RuntimeVersion)
                    : Res.InstallationsStatusInstalled(currentStatus.Version);
            RemoveCommandState.Enabled = currentStatus.Installed;
        }

        private void SelectFolder()
        {
            var path = SelectFolderCallback?.Invoke();
            if (path != null)
                SelectInstallation(path);
        }

        #endregion

        #region Command Handlers

        private void OnSelectFolderCommand()
        {
            if (isSelectingPath)
                return;
            SelectFolder();
        }

        private void OnInstallCommand()
        {
            if (currentStatus.Installed && !Confirm(Res.ConfirmMessageOverwriteInstallation, currentStatus.Version != null && InstallationManager.AvailableVersion.Version > currentStatus.Version))
                return;
#if NETCOREAPP
            if (!Confirm(Res.ConfirmMessageNetCoreVersion, false))
                return;
#endif

            InstallationManager.Install(currentStatus.Path, out string? error, out string? warning);
            if (error != null)
                ShowError(Res.ErrorMessageInstallationFailed(error));
            else if (warning != null)
                ShowWarning(Res.WarningMessageInstallationWarning(warning));
            UpdateStatus(currentStatus.Path);
        }

        private void OnRemoveCommand()
        {
            if (!Confirm(Res.ConfirmMessageRemoveInstallation, false))
                return;
            InstallationManager.Uninstall(currentStatus.Path, out string? error);
            if (error != null)
                ShowError(Res.ErrorMessageRemoveInstallationFailed(error));
            UpdateStatus(currentStatus.Path);
        }

        #endregion

        #endregion
    }
}
