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
            InitializeComponent();
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
            // VM.TitleCaption -> Text
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.TitleCaption), this, nameof(Text));

            // VM.ResourceFiles -> cmbResourceFiles.DataSource
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.ResourceFiles), nameof(cmbResourceFiles.DataSource), cmbResourceFiles);

            // VM.SelectedFile -> cmbResourceFiles.SelectedItem (cannot use two-way for SelectedItem because there is no SelectedItemChanged event)
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.SelectedFile), nameof(cmbResourceFiles.SelectedItem), cmbResourceFiles);

            // cmbResourceFiles.SelectedValue -> VM.SelectedFile (cannot use two-way for SelectedValue because ValueMember is not set)
            CommandBindings.AddPropertyBinding(cmbResourceFiles, nameof(cmbResourceFiles.SelectedValue), nameof(ViewModel.SelectedFile), ViewModel);

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
            CommandBindings.Add(ViewModel.SaveResourcesCommand, ViewModel.SaveResourcesCommandState)
                .AddSource(okCancelButtons.OKButton, nameof(okCancelButtons.OKButton.Click))
                .Executed += (_, args) => DialogResult = args.State[EditResourcesViewModel.CommandStateExecutedWithError] is true ? DialogResult.None : DialogResult.OK;
        }

        #endregion

        #endregion
    }
}
