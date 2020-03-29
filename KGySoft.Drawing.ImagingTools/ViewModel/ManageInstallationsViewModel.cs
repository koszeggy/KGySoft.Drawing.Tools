﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ManageInstallationsViewModel.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2020 - All Rights Reserved
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
using System.Collections.Generic;
using System.IO;
using System.Linq;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;
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

        private InstallationInfo currentStatus;
        private bool isSelectingPath;

        #endregion

        #region Properties

        internal IList<KeyValuePair<string, string>> Installations { get => Get<IList<KeyValuePair<string, string>>>(); set => Set(value); }
        internal string SelectedInstallation { get => Get<string>(); set => Set(value); }
        internal string CurrentPath { get => Get<string>(); set => Set(value); }
        internal string StatusText { get => Get("-"); set => Set(value); }

        internal Func<string> SelectFolderCallback { get => Get<Func<string>>(); set => Set(value); }

        internal ICommandState SelectFolderCommandState => Get(() => new CommandState());
        internal ICommandState RemoveCommandState => Get(() => new CommandState());

        internal ICommand SelectFolderCommand => Get(() => new SimpleCommand(OnSelectFolderCommand));
        internal ICommand InstallCommand => Get(() => new SimpleCommand(OnInstallCommand));
        internal ICommand RemoveCommand => Get(() => new SimpleCommand(OnRemoveCommand));

        #endregion

        #region Constructors

        internal ManageInstallationsViewModel(string hintPath)
        {
            InitVersions();
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
                UpdatePath((string)e.NewValue);
                return;
            }

            if (e.PropertyName == nameof(CurrentPath))
                UpdateStatus((string)e.NewValue);
        }

        #endregion

        #region Private Methods

        private void TrySelectPath(string hintPath)
        {
            if (hintPath?.Contains(visualStudioName, StringComparison.Ordinal) == true)
            {
                string preferredPath = Path.GetFileName(hintPath) == visualizersDir ? Path.GetDirectoryName(hintPath) : hintPath;
                SelectInstallation(Path.GetDirectoryName(preferredPath));
                return;
            }

            SelectInstallation(Installations.First().Key);
        }

        private void InitVersions()
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
                StatusText = currentStatus.RuntimeVersion == null
                    ? Res.InstallationsStatusInstalled(currentStatus.Version)
                    : Res.InstallationsStatusInstalledWithRuntime(currentStatus.Version, currentStatus.RuntimeVersion);
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
            if (currentStatus.Installed && !Confirm(Res.ConfirmMessageOverwriteInstallation))
                return;
            InstallationManager.Install(currentStatus.Path, out string error);
            if (error != null)
                ShowError(Res.ErrorMessageInstallationFailed(error));
            UpdateStatus(currentStatus.Path);
        }

        private void OnRemoveCommand()
        {
            if (!Confirm(Res.ConfirmMessageRemoveInstallation))
                return;
            InstallationManager.Uninstall(currentStatus.Path, out string error);
            if (error != null)
                ShowError(Res.ErrorMessageRemoveInstallationFailed(error));
            UpdateStatus(currentStatus.Path);
        }

        #endregion

        #endregion
    }
}