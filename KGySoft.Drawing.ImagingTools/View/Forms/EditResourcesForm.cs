#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: EditResourcesForm.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2023 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System.Collections.Generic;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    internal partial class EditResourcesForm : MvvmBaseForm
    {
        #region Properties

        private new EditResourcesViewModel ViewModel => (EditResourcesViewModel)base.ViewModel;

        #endregion

        #region Constructors

        #region Internal Constructors

        internal EditResourcesForm(EditResourcesViewModel viewModel) : base(viewModel)
        {
            // Note: Not setting Accept/CancelButton because they would be very annoying during the editing
            InitializeComponent();
            cmbResourceFiles.ValueMember = nameof(KeyValuePair<LocalizableLibraries, string>.Key);
            cmbResourceFiles.DisplayMember = nameof(KeyValuePair<LocalizableLibraries, string>.Value);
            ErrorProvider.SetIconAlignment(gbTranslatedText, ErrorIconAlignment.MiddleLeft);
            WarningProvider.SetIconAlignment(gbTranslatedText, ErrorIconAlignment.MiddleLeft);
            ValidationMapping[nameof(ResourceEntry.TranslatedText)] = gbTranslatedText;
            
            // For Linux/Mono adding an empty column in the middle so the error provider icon will not appear in a new row
            if (!OSUtils.IsWindows)
            {
                pnlEditResourceEntry.ColumnCount = 3;
                pnlEditResourceEntry.SetColumn(gbTranslatedText, 2);
                pnlEditResourceEntry.ColumnStyles.Insert(1, new ColumnStyle(SizeType.AutoSize));
            }
        }

        #endregion

        #region Private Constructors

        private EditResourcesForm() : this(null!)
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
            Icon = Properties.Resources.Language;
        }

        protected override void ApplyViewModel()
        {
            InitPropertyBindings();
            InitCommandBindings();
            base.ApplyViewModel();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (gridResources.IsCurrentCellInEditMode)
            {
                gridResources.CancelEdit();
                gridResources.EndEdit();
            }

            base.OnFormClosing(e);
        }

        #endregion

        #region Private Methods

        private void InitPropertyBindings()
        {
            // VM.ResourceFiles -> cmbResourceFiles.DataSource
            cmbResourceFiles.DataSource = ViewModel.ResourceFiles;

            // VM.TitleCaption -> Text
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.TitleCaption), this, nameof(Text));

            // VM.SelectedLibrary <-> cmbResourceFiles.SelectedValue
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.SelectedLibrary), cmbResourceFiles, nameof(cmbResourceFiles.SelectedValue));

            // txtFilter.Text -> VM.Filter
            CommandBindings.AddPropertyBinding(txtFilter, nameof(txtFilter.Text), nameof(ViewModel.Filter), ViewModel);

            // VM.FilteredSet -> bindingSource.DataSource
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.FilteredSet), nameof(bindingSource.DataSource), bindingSource);

            // bindingSource.OriginalText -> txtOriginalText.Text
            txtOriginalText.DataBindings.Add(nameof(txtOriginalText.Text), bindingSource, nameof(ResourceEntry.OriginalText), false, DataSourceUpdateMode.Never);

            // bindingSource.TranslatedText <-> txtTranslatedText.Text
            txtTranslatedText.DataBindings.Add(nameof(txtTranslatedText.Text), bindingSource, nameof(ResourceEntry.TranslatedText), false, DataSourceUpdateMode.OnValidation);
        }

        private void InitCommandBindings()
        {
            // ApplyButton.Click -> ViewModel.ApplyResourcesCommand
            CommandBindings.Add(ViewModel.ApplyResourcesCommand, ViewModel.ApplyResourcesCommandState)
                .AddSource(okCancelApplyButtons.ApplyButton, nameof(okCancelApplyButtons.ApplyButton.Click));

            // OKButton.Click -> ViewModel.SaveResourcesCommand, and preventing closing the form if the command has executed with errors
            CommandBindings.Add(ViewModel.SaveResourcesCommand)
                .AddSource(okCancelApplyButtons.OKButton, nameof(okCancelApplyButtons.OKButton.Click))
                .Executed += (_, args) => DialogResult = args.State[EditResourcesViewModel.StateSaveExecutedWithError] is true ? DialogResult.None : DialogResult.OK;

            // CancelButton.Click -> ViewModel.CancelResourcesCommand
            CommandBindings.Add(ViewModel.CancelEditCommand)
                .AddSource(okCancelApplyButtons.CancelButton, nameof(okCancelApplyButtons.CancelButton.Click));

            // View commands
            CommandBindings.Add(ValidationResultsChangedCommand)
                .AddSource(bindingSource, nameof(bindingSource.CurrentItemChanged))
                .WithParameter(() => (bindingSource.Current as ResourceEntry)?.ValidationResults);
        }

        #endregion

        #endregion
    }
}
