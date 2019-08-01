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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#endregion

namespace KGySoft.Drawing.ImagingTools.Forms
{
    internal partial class ManageInstallationsForm : BaseForm
    {
        #region Constants

        private const string installDirsPattern = "Visual Studio ????";
        private const string customDir = "<Custom Path...>";
        private const string visualizersDir = "Visualizers";

        #endregion

        #region Fields

        private InstallationInfo currentStatus;

        #endregion

        #region Constructors

        public ManageInstallationsForm()
        {
            InitializeComponent();
            Icon = Properties.Resources.Settings;
            cbInstallations.ValueMember = nameof(KeyValuePair<string, string>.Key);
            cbInstallations.DisplayMember = nameof(KeyValuePair<string, string>.Value);

            cbInstallations.SelectedValueChanged += cbInstallations_SelectedValueChanged;
            tbPath.TextChanged += tbPath_TextChanged;
            btnInstall.Click += btnInstall_Click;
            btnRemove.Click += btnRemove_Click;

            InitVersions();
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();

            cbInstallations.SelectedValueChanged -= cbInstallations_SelectedValueChanged;
            tbPath.TextChanged -= tbPath_TextChanged;
            btnInstall.Click -= btnInstall_Click;
            btnRemove.Click -= btnRemove_Click;

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void InitVersions()
        {
            string docsDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var list = new List<KeyValuePair<string, string>>();
            list.AddRange(Directory.GetDirectories(docsDir, installDirsPattern).Select(d => new KeyValuePair<string, string>(d, Path.GetFileName(d))));
            list.Sort((d1, d2) => String.CompareOrdinal(d1.Value, d2.Value));
            list.Add(new KeyValuePair<string, string>(String.Empty, customDir));
            cbInstallations.DataSource = list;
        }

        private void UpdatePath(string dir)
        {
            bool isCustom = String.IsNullOrEmpty(dir);
            tbPath.Enabled = isCustom;
            if (!isCustom)
                tbPath.Text = Path.Combine(dir, visualizersDir);
        }

        private void UpdateStatus(string path)
        {
            currentStatus = InstallationManager.GetInstallationInfo(path);
            if (!currentStatus.Installed)
                lblStatusText.Text = "Not Installed";
            else if (currentStatus.Version == null)
                lblStatusText.Text = "Unknown version (incompatible runtime?)";
            else
                lblStatusText.Text = $"Installed: {currentStatus.Version}. Runtime: {currentStatus.RuntimeVersion}";
            btnRemove.Enabled = currentStatus.Installed;
        }

        #endregion

        #region Event handlers

        private void cbInstallations_SelectedValueChanged(object sender, EventArgs e) => UpdatePath(cbInstallations.SelectedValue.ToString());

        private void tbPath_TextChanged(object sender, EventArgs e) => UpdateStatus(tbPath.Text);

        private void btnInstall_Click(object sender, EventArgs e)
        {
            if (currentStatus.Installed && !Dialogs.ConfirmMessage("Are you sure you want to overwrite this installation?"))
                return;
            InstallationManager.Install(currentStatus.Path, out string error);
            if (error != null)
                Dialogs.ErrorMessage("Installation failed: {0}", error);
            UpdateStatus(currentStatus.Path);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (!Dialogs.ConfirmMessage("Are you sure you want to remove this installation?"))
                return;
            InstallationManager.Uninstall(currentStatus.Path, out string error);
            if (error != null)
                Dialogs.ErrorMessage("Removing failed: {0}", error);
            UpdateStatus(currentStatus.Path);
        }

        #endregion

        #endregion
    }
}
