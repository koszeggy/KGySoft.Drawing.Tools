#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: LanguageSettingsControl.cs
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
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using KGySoft.ComponentModel;
using KGySoft.Drawing.ImagingTools.View.Forms;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal partial class LanguageSettingsControl : MvvmBaseUserControl
    {
        #region Fields

        private ParentViewProperties? parentProperties;
        private ICommandBinding? saveCommandBinding;

        #endregion

        #region Properties

        #region Internal Properties

        internal override ParentViewProperties ParentViewProperties => parentProperties ??= new ParentViewProperties
        {
            BorderStyle = FormBorderStyle.FixedDialog,
            Icon = Properties.Resources.Language,
            AcceptButton = okCancelApplyButtons.OKButton,
            CancelButton = okCancelApplyButtons.CancelButton,
            ClosingCallback = (sender, e) =>
            {
                // if user (or system) closes the window without pressing cancel we need to execute the cancel command
                if (((Form)sender).DialogResult != DialogResult.OK && e.CloseReason != CloseReason.None)
                    okCancelApplyButtons.CancelButton.PerformClick();
            }
        };

        internal override Action<MvvmParentForm> ParentViewCommandBindingsInitializer => InitParentViewCommandBindings;

        #endregion

        #region Private Properties

        private new LanguageSettingsViewModel ViewModel => (LanguageSettingsViewModel)base.ViewModel!;

        #endregion

        #endregion

        #region Constructors

        #region Internal Constructors

        internal LanguageSettingsControl(LanguageSettingsViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            btnEditResources.Height = cmbLanguages.Height + 2; // helps aligning better for higher DPIs
        }

        #endregion

        #region Private Constructors

        private LanguageSettingsControl() : this(null!)
        {
            // this ctor is just for the designer
        }

        #endregion

        #endregion

        #region Methods

        #region Static Methods

        private static void OnFormatCultureCommand(ICommandSource<ListControlConvertEventArgs> source)
        {
            var culture = (CultureInfo)source.EventArgs.ListItem!;
            source.EventArgs.Value = $"{culture.NativeName} ({culture.EnglishName})";
        }

        #endregion

        #region Instance Methods

        #region Protected Methods

        protected override void OnLoad(EventArgs e)
        {
            // Fixing high DPI appearance on Mono
            PointF scale;
            if (OSUtils.IsMono && (scale = this.GetScale()) != new PointF(1f, 1f))
            {
                btnEditResources.Size = new Size(105, 23).Scale(scale);
                btnDownloadResources.Height = (int)(23 * scale.Y);
            }

            // Mono/Windows: ignoring because ToolTips throw an exception if set for an embedded control and
            // since they don't appear for negative padding there is simply no place for them.
            if (!IsLoaded && !(OSUtils.IsMono && OSUtils.IsWindows))
            {
                ValidationMapping[nameof(ViewModel.ResourceCustomPath)] = gbResxResourcesPath.CheckBox;
                ErrorProvider.SetIconAlignment(gbResxResourcesPath.CheckBox, ErrorIconAlignment.TopRight);
            }

            base.OnLoad(e);
        }

        protected override void ApplyViewModel()
        {
            InitViewModelDependencies();
            InitCommandBindings();
            InitPropertyBindings();
            base.ApplyViewModel();
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
                components?.Dispose();

            saveCommandBinding = null;
            parentProperties = null;
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void InitViewModelDependencies() => ViewModel.SelectFolderCallback = () => Dialogs.SelectFolder(ViewModel.ResourceCustomPath);

        private void InitPropertyBindings()
        {
            // VM.UseOSLanguage <-> chbUseOSLanguage.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.UseOSLanguage), chbUseOSLanguage, nameof(chbUseOSLanguage.Checked));

            // VM.AllowAnyLanguage <-> chbAllowAnyLanguage.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.AllowAnyLanguage), chbAllowAnyLanguage, nameof(chbAllowAnyLanguage.Checked));

            // VM.UseOSLanguage -> !cmbLanguages.Enabled
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.UseOSLanguage), nameof(cmbLanguages.Enabled), b => !((bool)b!), cmbLanguages);

            // VM.UseCustomResourcePath <-> gbResxResourcesPath.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.UseCustomResourcePath), gbResxResourcesPath, nameof(gbResxResourcesPath.Checked));

            // VM.ResourceCustomPath <-> txtResxResourcesPath.Text
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.ResourceCustomPath), txtResxResourcesPath, nameof(txtResxResourcesPath.Text));

            // VM.Languages -> cmbLanguages.DataSource
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.Languages), nameof(cmbLanguages.DataSource), cmbLanguages);

            // VM.CurrentLanguage -> cmbLanguages.SelectedItem (cannot use two-way for SelectedItem because there is no SelectedItemChanged event)
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.CurrentLanguage), nameof(cmbLanguages.SelectedItem), cmbLanguages);

            // cmbLanguages.SelectedValue -> VM.CurrentLanguage (cannot use two-way for SelectedValue because ValueMember is not set)
            CommandBindings.AddPropertyBinding(cmbLanguages, nameof(cmbLanguages.SelectedValue), nameof(ViewModel.CurrentLanguage), ViewModel);
        }

        private void InitCommandBindings()
        {
            CommandBindings.Add(ViewModel.CancelCommand)
                .AddSource(okCancelApplyButtons.CancelButton, nameof(okCancelApplyButtons.CancelButton.Click));

            saveCommandBinding = CommandBindings.Add(ViewModel.SaveConfigCommand)
                .AddSource(okCancelApplyButtons.OKButton, nameof(okCancelApplyButtons.OKButton.Click));

            CommandBindings.Add(ViewModel.ApplyCommand, ViewModel.ApplyCommandState)
                .AddSource(okCancelApplyButtons.ApplyButton, nameof(okCancelApplyButtons.ApplyButton.Click));

            CommandBindings.Add(ViewModel.EditResourcesCommand)
                .AddSource(btnEditResources, nameof(btnEditResources.Click));

            CommandBindings.Add(ViewModel.DownloadResourcesCommand)
                .AddSource(btnDownloadResources, nameof(btnDownloadResources.Click));

            CommandBindings.Add(ViewModel.FinalizePath)
                .AddSource(txtResxResourcesPath, nameof(txtResxResourcesPath.Validating));

            CommandBindings.Add(ViewModel.SelectFolderCommand)
                .AddSource(txtResxResourcesPath, nameof(txtResxResourcesPath.DoubleClick));

            // View commands
            CommandBindings.Add<ListControlConvertEventArgs>(OnFormatCultureCommand)
                .AddSource(cmbLanguages, nameof(cmbLanguages.Format));

            CommandBindings.Add(ValidationResultsChangedCommand)
                .AddSource(ViewModel, nameof(ViewModel.ValidationResultsChanged))
                .WithParameter(() => ViewModel.ValidationResults);
        }

        private void InitParentViewCommandBindings(MvvmParentForm parent)
        {
            // preventing closing the form if the command has executed with errors
            if (saveCommandBinding is ICommandBinding binding)
                binding.Executed += (_, args) => parent.DialogResult = args.State[LanguageSettingsViewModel.StateSaveExecutedWithError] is true ? DialogResult.None : DialogResult.OK;
        }

        #endregion

        #endregion

        #endregion
    }
}
