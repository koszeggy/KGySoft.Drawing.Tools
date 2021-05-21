#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: EditResourcesForm.cs
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

using System.Collections.Generic;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    internal partial class EditResourcesForm : MvvmBaseForm<EditResourcesViewModel>
    {
        #region Constructors

        #region Internal Constructors

        internal EditResourcesForm(EditResourcesViewModel viewModel) : base(viewModel)
        {
            // Note: Not setting Accept/CancelButton because they would be very annoying during the editing
            InitializeComponent();
            cmbResourceFiles.ValueMember = nameof(KeyValuePair<ResourceOwner, string>.Key);
            cmbResourceFiles.DisplayMember = nameof(KeyValuePair<ResourceOwner, string>.Value);
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
            InitCommandBindings();
            InitPropertyBindings();
            base.ApplyViewModel();
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

            // VM.SelectedSet -> resourceEntryBindingSource.DataSource
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.SelectedSet), nameof(resourceEntryBindingSource.DataSource), resourceEntryBindingSource);

            // resourceEntryBindingSource.OriginalText -> txtOriginalText.Text
            txtOriginalText.DataBindings.Add(nameof(txtOriginalText.Text), resourceEntryBindingSource, nameof(ResourceEntry.OriginalText), false, DataSourceUpdateMode.Never);

            // resourceEntryBindingSource.TranslatedText <-> txtTranslatedText.Text
            txtTranslatedText.DataBindings.Add(nameof(txtTranslatedText.Text), resourceEntryBindingSource, nameof(ResourceEntry.TranslatedText), false, DataSourceUpdateMode.OnValidation);
        }

        private void InitCommandBindings()
        {
            // OKButton.Click -> ViewModel.SaveResourcesCommand, and preventing closing the form if the command has executed with errors
            CommandBindings.Add(ViewModel.SaveResourcesCommand)
                .AddSource(okCancelButtons.OKButton, nameof(okCancelButtons.OKButton.Click))
                .Executed += (_, args) => DialogResult = args.State[EditResourcesViewModel.StateSaveExecutedWithError] is true ? DialogResult.None : DialogResult.OK;

            // CancelButton.Click -> ViewModel.CancelResourcesCommand
            CommandBindings.Add(ViewModel.CancelEditCommand)
                .AddSource(okCancelButtons.CancelButton, nameof(okCancelButtons.CancelButton.Click));
        }

        #endregion

        #endregion
    }
}
