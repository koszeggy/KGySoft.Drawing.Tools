#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ManageInstallationsForm.cs
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

using System.Collections.Generic;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    internal partial class ManageInstallationsForm : MvvmBaseForm<ManageInstallationsViewModel>
    {
        #region Constructors

        #region Internal Constructors

        internal ManageInstallationsForm(ManageInstallationsViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
            cbInstallations.ValueMember = nameof(KeyValuePair<string, string>.Key);
            cbInstallations.DisplayMember = nameof(KeyValuePair<string, string>.Value);
        }

        #endregion

        #region Private Constructors

        private ManageInstallationsForm() : this(null)
        {
            // this ctor is just for the designer
        }

        #endregion

        #endregion

        #region Methods

        #region Protected Methods

        protected override void ApplyResources()
        {
            base.ApplyResources();
            Icon = Properties.Resources.Settings;
        }

        protected override void ApplyViewModel()
        {
            InitViewModelDependencies();
            InitPropertyBindings();
            InitCommandBindings();
            base.ApplyViewModel();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void InitViewModelDependencies()
        {
            ViewModel.SelectFolderCallback = SelectFolder;
        }

        private void InitPropertyBindings()
        {
            // will not change so not as an actual binding
            cbInstallations.DataSource = ViewModel.Installations;

            // VM.SelectedInstallation <-> cbInstallations.SelectedValue
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.SelectedInstallation), cbInstallations, nameof(cbInstallations.SelectedValue));

            // VM.CurrentPath <-> tbPath.Text
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.CurrentPath), tbPath, nameof(tbPath.Text));

            // VM.AvailableVersionText -> lblAvailableVersion.Text
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.AvailableVersionText), nameof(lblAvailableVersion.Text), lblAvailableVersion);

            // VM.StatusText -> lblStatusText.Text
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.StatusText), nameof(lblStatusText.Text), lblStatusText);
        }

        private void InitCommandBindings()
        {
            CommandBindings.Add(ViewModel.SelectFolderCommand, ViewModel.SelectFolderCommandState)
                .AddSource(tbPath, nameof(tbPath.DoubleClick));
            CommandBindings.Add(ViewModel.InstallCommand, ViewModel.InstallCommandState)
                .AddSource(btnInstall, nameof(btnInstall.Click));
            CommandBindings.Add(ViewModel.RemoveCommand, ViewModel.RemoveCommandState)
                .AddSource(btnRemove, nameof(btnRemove.Click));
        }

        private string SelectFolder()
        {
            using (var dlg = new FolderBrowserDialog { SelectedPath = ViewModel.CurrentPath })
                return dlg.ShowDialog() != DialogResult.OK ? null : dlg.SelectedPath;
        }

        #endregion

        #endregion
    }
}
