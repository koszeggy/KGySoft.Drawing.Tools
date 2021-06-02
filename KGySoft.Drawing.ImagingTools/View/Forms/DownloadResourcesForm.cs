﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DownloadResourcesForm.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution. If not, then this file is considered as
//  an illegal copy.
//
//  Unauthorized copying of this file, via any medium is strictly prohibited.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    internal partial class DownloadResourcesForm : MvvmBaseForm<DownloadResourcesViewModel>
    {
        #region Constructors

        #region Internal Constructors

        internal DownloadResourcesForm(DownloadResourcesViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            okCancelButtons.OKButton.Name = "btnDownload";
            okCancelButtons.OKButton.DialogResult = DialogResult.None;
            AcceptButton = okCancelButtons.OKButton;
            CancelButton = okCancelButtons.CancelButton;
        }

        #endregion

        #region Private Constructors

        private DownloadResourcesForm() : this(null!)
        {
            // this ctor is just for the designer
        }

        #endregion

        #endregion

        #region Methods

        #region Protected Methods

        protected override void ApplyResources()
        {
            Icon = Properties.Resources.Language;
            base.ApplyResources();
        }

        protected override void ApplyViewModel()
        {
            InitCommandBindings();
            InitPropertyBindings();
            base.ApplyViewModel();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            ViewModel.CancelIfRunning();
            base.OnFormClosing(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void InitCommandBindings()
        {
            CommandBindings.Add(ViewModel.DownloadCommand, ViewModel.DownloadCommandState)
                .AddSource(okCancelButtons.OKButton, nameof(okCancelButtons.OKButton.Click));
            CommandBindings.Add(ViewModel.CancelCommand)
                .AddSource(okCancelButtons.CancelButton, nameof(okCancelButtons.CancelButton.Click));
            CommandBindings.Add(OnDirtyStateChangedCommand)
                .AddSource(gridDownloadableResources, nameof(gridDownloadableResources.CurrentCellDirtyStateChanged));
        }

        private void InitPropertyBindings()
        {
            // VM.IsProcessing -> progress.ProgressVisible
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.IsProcessing), nameof(progress.ProgressVisible), progress);

            // VM.Progress -> progress.Progress
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.Progress), nameof(progress.Progress), progress);

            // VM.Items -> bindingSource.DataSource
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.Items), nameof(bindingSource.DataSource), bindingSource);
        }

        #endregion

        #region Command Handlers

        private void OnDirtyStateChangedCommand()
        {
            if (gridDownloadableResources.CurrentCell is DataGridViewCheckBoxCell { EditingCellValueChanged: true })
                gridDownloadableResources.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        #endregion

        #endregion
    }
}