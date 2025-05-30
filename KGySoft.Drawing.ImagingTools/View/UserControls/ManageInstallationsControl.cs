﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ManageInstallationsControl.cs
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal partial class ManageInstallationsControl : MvvmBaseUserControl
    {
        #region Fields

        private ParentViewProperties? parentProperties;

        #endregion

        #region Properties

        #region Internal Properties

        internal override ParentViewProperties ParentViewProperties => parentProperties ??= new ParentViewProperties
        {
            BorderStyle = FormBorderStyle.FixedDialog,
            Icon = Properties.Resources.Settings
        };

        #endregion

        #region Private Properties

        private new ManageInstallationsViewModel ViewModel => (ManageInstallationsViewModel)base.ViewModel!;

        #endregion
        
        #endregion

        #region Constructors

        #region Internal Constructors

        internal ManageInstallationsControl(ManageInstallationsViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
#if NET472_OR_GREATER
            Name = "ManageClassicInstallationsControl"; // so a different title will be picked from the resources
#endif
        }

        #endregion

        #region Private Constructors

        private ManageInstallationsControl() : this(null!)
        {
            // this ctor is just for the designer
        }

        #endregion

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnLoad(EventArgs e)
        {
            // Fixing high DPI appearance on Mono
            PointF scale;
            if (OSUtils.IsMono && (scale = this.GetScale()) != new PointF(1f, 1f))
            {
                pnlButtons.Height = (int)(35 * scale.Y);
                var referenceButtonSize = new Size(75, 23);
                btnInstall.Size = referenceButtonSize.Scale(scale);
                btnRemove.Size = referenceButtonSize.Scale(scale);
            }

            base.OnLoad(e);
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
            if (IsDisposed)
                return;

            if (disposing)
                components?.Dispose();

            parentProperties = null;
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
            // VM.Installations -> cmbInstallations.DataSource
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.Installations), nameof(cmbInstallations.DataSource), cmbInstallations);

            // VM.SelectedInstallation <-> cmbInstallations.SelectedValue
            cmbInstallations.ValueMember = nameof(KeyValuePair<string, string>.Key);
            cmbInstallations.DisplayMember = nameof(KeyValuePair<string, string>.Value);
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.SelectedInstallation), cmbInstallations, nameof(cmbInstallations.SelectedValue));

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

        private string? SelectFolder() => Dialogs.SelectFolder(ViewModel.CurrentPath);

        #endregion

        #endregion
    }
}
