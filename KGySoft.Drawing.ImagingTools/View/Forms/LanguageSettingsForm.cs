#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: LanguageSettingsForm.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
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
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    internal partial class LanguageSettingsForm : MvvmBaseForm<LanguageSettingsViewModel>
    {
        #region Constructors

        #region Internal Constructors

        internal LanguageSettingsForm(LanguageSettingsViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            btnEditResources.Height = cmbLanguages.Height + 2; // helps aligning better for higher DPIs
            AcceptButton = okCancelApplyButtons.OKButton;
            CancelButton = okCancelApplyButtons.CancelButton;

            // Mono/Windows: exiting because ToolTips throw an exception if set for an embedded control and
            // since they don't appear for negative padding there is simply no place for them.
            if (OSUtils.IsMono && OSUtils.IsWindows)
                return;

            ValidationMapping[nameof(viewModel.ResourceCustomPath)] = gbResxResourcesPath.CheckBox;
            ErrorProvider.SetIconAlignment(gbResxResourcesPath.CheckBox, ErrorIconAlignment.TopRight);
        }

        #endregion

        #region Private Constructors

        private LanguageSettingsForm() : this(null!)
        {
            // this ctor is just for the designer
        }

        #endregion

        #endregion

        #region Methods

        #region Static Methods

        private static void OnFormatCultureCommand(ICommandSource<ListControlConvertEventArgs> source)
        {
            var culture = (CultureInfo)source.EventArgs.ListItem;
            source.EventArgs.Value = $"{culture.EnglishName} ({culture.NativeName})";
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

            base.OnLoad(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // if user (or system) closes the window without pressing cancel we need to execute the cancel command
            if (DialogResult != DialogResult.OK && e.CloseReason != CloseReason.None)
                okCancelApplyButtons.CancelButton.PerformClick();
            base.OnFormClosing(e);
        }

        protected override void ApplyResources()
        {
            base.ApplyResources();
            Icon = Properties.Resources.Language;
        }

        protected override void ApplyViewModel()
        {
            InitCommandBindings();
            InitPropertyBindings();
            base.ApplyViewModel();
        }

        #endregion

        #region Private Methods

        private void InitPropertyBindings()
        {
            // VM.AllowResXResources <-> gbAllowResxResources.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.AllowResXResources), gbAllowResxResources, nameof(gbAllowResxResources.Checked));

            // VM.UseOSLanguage <-> chbUseOSLanguage.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.UseOSLanguage), chbUseOSLanguage, nameof(chbUseOSLanguage.Checked));

            // VM.ExistingLanguagesOnly <-> chbExistingResourcesOnly.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.ExistingLanguagesOnly), chbExistingResourcesOnly, nameof(chbExistingResourcesOnly.Checked));

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

            CommandBindings.Add(ViewModel.SaveConfigCommand)
                .AddSource(okCancelApplyButtons.OKButton, nameof(okCancelApplyButtons.OKButton.Click));

            CommandBindings.Add(ViewModel.ApplyCommand, ViewModel.ApplyCommandState)
                .AddSource(okCancelApplyButtons.ApplyButton, nameof(okCancelApplyButtons.ApplyButton.Click));

            CommandBindings.Add(ViewModel.EditResourcesCommand, ViewModel.EditResourcesCommandState)
                .AddSource(btnEditResources, nameof(btnEditResources.Click));

            CommandBindings.Add(ViewModel.DownloadResourcesCommand)
                .AddSource(btnDownloadResources, nameof(btnDownloadResources.Click));

            CommandBindings.Add(ViewModel.FinalizePath)
                .AddSource(txtResxResourcesPath, nameof(txtResxResourcesPath.Validating));

            // View commands
            CommandBindings.Add<ListControlConvertEventArgs>(OnFormatCultureCommand)
                .AddSource(cmbLanguages, nameof(cmbLanguages.Format));

            CommandBindings.Add(ValidationResultsChangedCommand)
                .AddSource(ViewModel, nameof(ViewModel.ValidationResultsChanged))
                .WithParameter(() => ViewModel.ValidationResults);
        }

        #endregion

        #endregion

        #endregion
    }
}
