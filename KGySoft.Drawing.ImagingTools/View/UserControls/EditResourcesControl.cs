#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: EditResourcesControl.cs
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

using KGySoft.ComponentModel;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Drawing.ImagingTools.View.Forms;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal partial class EditResourcesControl : MvvmBaseUserControl
    {
        #region Fields

        private ParentViewProperties? parentProperties;
        private ICommandBinding? saveCommandBinding;

        #endregion

        #region Properties

        #region Internal Properties

        internal override ParentViewProperties ParentViewProperties => parentProperties ??= new ParentViewProperties
        {
            Icon = Properties.Resources.Language,
            HideMinimizeButton = true,
            MinimumSize = new Size(300, 350),
            ClosingCallback = (_,_) =>
            {
                if (gridResources.IsCurrentCellInEditMode)
                {
                    gridResources.CancelEdit();
                    gridResources.EndEdit();
                }
            }
        };

        internal override Action<MvvmParentForm> ParentViewPropertyBindingsInitializer => InitParentViewPropertyBindings;
        internal override Action<MvvmParentForm> ParentViewCommandBindingsInitializer => InitParentViewCommandBindings;

        #endregion

        #region Private Properties

        private new EditResourcesViewModel ViewModel => (EditResourcesViewModel)base.ViewModel!;

        #endregion

        #endregion

        #region Constructors

        #region Internal Constructors

        internal EditResourcesControl(EditResourcesViewModel viewModel) : base(viewModel)
        {
            // Note: Not setting Accept/CancelButton because they would be very annoying during the editing
            InitializeComponent();
            BackColor = Color.Transparent; // to make the resize grip in the parent form visible

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

        private EditResourcesControl() : this(null!)
        {
            // this ctor is just for the designer
        }

        #endregion

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnLoad(EventArgs e)
        {
            if (!IsLoaded)
            {
                ErrorProvider.SetIconAlignment(gbTranslatedText, ErrorIconAlignment.MiddleLeft);
                WarningProvider.SetIconAlignment(gbTranslatedText, ErrorIconAlignment.MiddleLeft);
                ValidationMapping[nameof(ResourceEntry.TranslatedText)] = gbTranslatedText;
            }

            base.OnLoad(e);
        }

        protected override void ApplyViewModel()
        {
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
            saveCommandBinding = null;
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void InitPropertyBindings()
        {
            // VM.HideDependentResources <-> chbHideDependencies.Checked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.HideDependentResources), chbHideDependencies, nameof(chbHideDependencies.Checked));

            // VM.ResourceFiles -> cmbResourceFiles.DataSource
            cmbResourceFiles.ValueMember = nameof(KeyValuePair<LocalizableLibraries, string>.Key);
            cmbResourceFiles.DisplayMember = nameof(KeyValuePair<LocalizableLibraries, string>.Value);
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.ResourceFiles), nameof(cmbResourceFiles.DataSource), cmbResourceFiles);

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

        private void InitParentViewPropertyBindings(MvvmParentForm parent)
        {
            // VM.TitleCaption -> parent.Text
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.TitleCaption), nameof(Text), parent);
        }

        private void InitCommandBindings()
        {
            // ApplyButton.Click -> ViewModel.ApplyResourcesCommand
            CommandBindings.Add(ViewModel.ApplyResourcesCommand, ViewModel.ApplyResourcesCommandState)
                .AddSource(okCancelApplyButtons.ApplyButton, nameof(okCancelApplyButtons.ApplyButton.Click));

            // OKButton.Click -> ViewModel.SaveResourcesCommand
            saveCommandBinding = CommandBindings.Add(ViewModel.SaveResourcesCommand)
                .AddSource(okCancelApplyButtons.OKButton, nameof(okCancelApplyButtons.OKButton.Click));
                //.Executed += (_, args) => DialogResult = args.State[EditResourcesViewModel.StateSaveExecutedWithError] is true ? DialogResult.None : DialogResult.OK;

            // CancelButton.Click -> ViewModel.CancelResourcesCommand
            CommandBindings.Add(ViewModel.CancelEditCommand)
                .AddSource(okCancelApplyButtons.CancelButton, nameof(okCancelApplyButtons.CancelButton.Click));

            // View commands
            CommandBindings.Add(ValidationResultsChangedCommand)
                .AddSource(bindingSource, nameof(bindingSource.CurrentItemChanged))
                .WithParameter(() => (bindingSource.Current as ResourceEntry)?.ValidationResults);
        }

        private void InitParentViewCommandBindings(MvvmParentForm parent)
        {
            // preventing closing the form if the command has executed with errors
            if (saveCommandBinding is ICommandBinding binding)
                binding.Executed += (_, args) => parent.DialogResult = args.State[EditResourcesViewModel.StateSaveExecutedWithError] is true ? DialogResult.None : DialogResult.OK;
        }

        #endregion

        #endregion
    }
}
