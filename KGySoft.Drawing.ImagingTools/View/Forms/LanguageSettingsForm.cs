#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: LanguageSettingsForm.cs
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
            AcceptButton = okCancelApplyButtons.OKButton;
            CancelButton = okCancelApplyButtons.CancelButton;
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

            // VM.Languages -> cmbLanguages.DataSource
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.Languages), nameof(cmbLanguages.DataSource), cmbLanguages);

            // VM.CurrentLanguage -> cmbLanguages.SelectedItem (cannot use two-way for SelectedItem because there is no SelectedItemChanged event)
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.CurrentLanguage), nameof(cmbLanguages.SelectedItem), cmbLanguages);

            // cmbLanguages.SelectedValue -> VM.CurrentLanguage (cannot use two-way for SelectedValue because ValueMember is not set)
            CommandBindings.AddPropertyBinding(cmbLanguages, nameof(cmbLanguages.SelectedValue), nameof(ViewModel.CurrentLanguage), ViewModel);
        }

        private void InitCommandBindings()
        {
            CommandBindings.Add<ListControlConvertEventArgs>(OnFormatCultureCommand)
                .AddSource(cmbLanguages, nameof(cmbLanguages.Format));

            CommandBindings.Add(ViewModel.SaveConfigCommand)
                .AddSource(okCancelApplyButtons.OKButton, nameof(okCancelApplyButtons.OKButton.Click));

            CommandBindings.Add(ViewModel.ApplyCommand, ViewModel.ApplyCommandState)
                .AddSource(okCancelApplyButtons.ApplyButton, nameof(okCancelApplyButtons.ApplyButton.Click));

            CommandBindings.Add(ViewModel.EditResourcesCommand, ViewModel.EditResourcesCommandState)
                .AddSource(btnEditResources, nameof(btnEditResources.Click));

            CommandBindings.Add(ViewModel.DownloadResourcesCommand)
                .AddSource(btnDownloadResources, nameof(btnDownloadResources.Click));
        }

        #endregion

        #endregion

        #endregion
    }
}
