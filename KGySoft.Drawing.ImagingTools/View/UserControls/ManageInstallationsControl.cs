#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ManageInstallationsControl.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2026 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.ViewModel;
using KGySoft.WinForms;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal partial class ManageInstallationsControl : MvvmBaseUserControl
    {
        #region Constants

        // Adjusted for Segoe UI 9 font on 100% DPI
        private const int gbAvailableVersionRefHeight = 46;
        private const int gbVisualStudioVersionsRefHeight = 53;

        #endregion

        #region Fields

        #region Static Fields

        private static readonly Size referenceSize = new Size(470, 240); // Adjusted for Segoe UI 9 font on 100% DPI.
        private static readonly Size buttonReferenceSize = new Size(75, 23);
        private static readonly Padding referencePadding = new Padding(3);
        private static readonly Padding buttonReferenceMargin = new Padding(3);
        private static readonly Padding panelReferencePadding = new Padding(3);

        #endregion

        #region Instance Fields

        private ParentViewProperties? parentProperties;

        #endregion

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

        protected override void ApplyViewModel()
        {
            InitViewModelDependencies();
            InitPropertyBindings();
            InitCommandBindings();
            base.ApplyViewModel();
        }

        internal override Size? GetDesiredSize(PointF scale) => referenceSize.Scale(scale);

        internal override void AdjustSizes(PointF? dynamicSizesScale)
        {
            base.AdjustSizes(dynamicSizesScale);
            SuspendLayout();
            pnlButtons.SuspendLayout();
            PointF scale = this.GetScale();
            Padding = gbAvailableVersion.Padding = gbVisualStudioVersions.Padding = gbInstallation.Padding = referencePadding.Scale(scale);
            gbAvailableVersion.Height = gbAvailableVersionRefHeight.Scale(scale.Y);
            gbVisualStudioVersions.Height = gbVisualStudioVersionsRefHeight.Scale(scale.Y);
            try
            {
                Size minSize = buttonReferenceSize.Scale(scale);
                Padding margin = buttonReferenceMargin.Scale(scale);
                foreach (Control control in pnlButtons.Controls)
                {
                    if (control is not Button button)
                        continue;

                    button.MinimumSize = minSize;
                    button.Size = button.GetPreferredSize(new Size(0, minSize.Height));
                    button.Margin = margin;
                }

                pnlButtons.Padding = panelReferencePadding.Scale(scale);
                pnlButtons.Height = minSize.Height + pnlButtons.Padding.Vertical + margin.Vertical;
            }
            finally
            {
                pnlButtons.ResumeLayout();
                ResumeLayout();
            }
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
