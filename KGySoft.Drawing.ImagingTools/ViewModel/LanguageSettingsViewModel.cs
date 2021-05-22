#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: LanguageSettingsViewModel.cs
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

using KGySoft.ComponentModel;
using KGySoft.Drawing.ImagingTools.Properties;
using KGySoft.Resources;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class LanguageSettingsViewModel : ViewModelBase
    {
        #region Fields

        private readonly bool initializing;

        private CultureInfo[]? neutralLanguages;

        #endregion

        #region Properties

        #region Internal Properties

        internal bool AllowResXResources { get => Get<bool>(); set => Set(value); }
        internal bool UseOSLanguage { get => Get<bool>(); set => Set(value); }
        internal bool ExistingLanguagesOnly { get => Get<bool>(); set => Set(value); }
        internal CultureInfo CurrentLanguage { get => Get<CultureInfo>(); set => Set(value); }
        internal IList<CultureInfo> Languages { get => Get<IList<CultureInfo>>(); set => Set(value); }

        internal ICommand ApplyCommand => Get(() => new SimpleCommand(OnApplyCommand));
        internal ICommand SaveConfigCommand => Get(() => new SimpleCommand(OnSaveConfigCommand));
        internal ICommand EditResourcesCommand => Get(() => new SimpleCommand(OnEditResourcesCommand));

        internal ICommandState ApplyCommandState => Get(() => new CommandState());
        internal ICommandState EditResourcesCommandState => Get(() => new CommandState());

        #endregion

        #region Private Properties

        private CultureInfo[] NeutralLanguages => neutralLanguages ??= CultureInfo.GetCultures(CultureTypes.NeutralCultures);

        #endregion

        #endregion

        #region Constructors

        internal LanguageSettingsViewModel()
        {
            initializing = true;
            CurrentLanguage = LanguageSettings.DisplayLanguage;
            AllowResXResources = Settings.Default.AllowResXResources;
            UseOSLanguage = Settings.Default.UseOSLanguage;
            ExistingLanguagesOnly = true; // could be the default value but this way we spare one reset when initializing binding
            initializing = false;
            ResetLanguages();
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            switch (e.PropertyName)
            {
                case nameof(AllowResXResources):
                case nameof(UseOSLanguage):
                case nameof(ExistingLanguagesOnly):
                    if (initializing)
                        return;
                    ResetLanguages();
                    break;

                case nameof(Languages):
                    if (e.OldValue is SortableBindingList<CultureInfo> sbl)
                        sbl.Dispose();
                    break;

                case nameof(CurrentLanguage):
                    ApplyCommandState.Enabled = !Equals(e.NewValue, LanguageSettings.DisplayLanguage);
                    EditResourcesCommandState.Enabled = !Equals(e.NewValue, Res.DefaultLanguage);
                    break;
            }
        }

        protected override void ApplyDisplayLanguage() => ApplyCommandState.Enabled = false;

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
                (Languages as SortableBindingList<CultureInfo>)?.Dispose();

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void ResetLanguages()
        {
            if (!AllowResXResources)
            {
                Languages = new[] { Res.DefaultLanguage };
                CurrentLanguage = Res.DefaultLanguage;
                return;
            }

            if (UseOSLanguage)
            {
                Languages = new[] { Res.OSLanguage };
                CurrentLanguage = Res.OSLanguage;
                return;
            }

            var result = new SortableBindingList<CultureInfo>(ExistingLanguagesOnly ? ResHelper.GetAvailableLanguages() : NeutralLanguages);
            result.ApplySort(nameof(CultureInfo.EnglishName), ListSortDirection.Ascending);
            CultureInfo lastSelectedLanguage = CurrentLanguage;
            Languages = result;
            CurrentLanguage = result.Contains(lastSelectedLanguage) ? lastSelectedLanguage : Res.DefaultLanguage;
        }

        #endregion

        #region Command Handlers

        private void OnApplyCommand()
        {
            CultureInfo currentLanguage = CurrentLanguage;
            LanguageSettings.DynamicResourceManagersSource = AllowResXResources ? ResourceManagerSources.CompiledAndResX : ResourceManagerSources.CompiledOnly;

            if (Equals(LanguageSettings.DisplayLanguage, currentLanguage))
                ResHelper.RaiseLanguageChanged();
            else
                LanguageSettings.DisplayLanguage = currentLanguage;

            // Note: Ensure is not really needed because main .resx is generated, while others are saved on demand in the editor, too
            // TODO If used, then add to EditResourcesVM.Save, too, to be consistent ()
            //ResHelper.EnsureResourcesGenerated();
            ResHelper.SavePendingResources();
        }

        private void OnSaveConfigCommand()
        {
            if (!IsModified)
                return;

            if (ApplyCommandState.Enabled)
                OnApplyCommand();

            // saving the configuration
            Settings.Default.AllowResXResources = AllowResXResources;
            Settings.Default.UseOSLanguage = UseOSLanguage;
            Settings.Default.DisplayLanguage = CurrentLanguage;
            try
            {
                Settings.Default.Save();
            }
            catch (Exception e) when (!e.IsCritical())
            {
                ShowError(Res.ErrorMessageFailedToSaveSettings(e.Message));
            }
        }

        private void OnEditResourcesCommand()
        {
            using IViewModel viewModel = ViewModelFactory.CreateEditResources(CurrentLanguage);
            ShowChildViewCallback?.Invoke(viewModel);

            // If the language was edited, then enabling apply even if it was disabled
            if (viewModel.IsModified)
                ApplyCommandState.Enabled = true;
        }

        #endregion

        #endregion
    }
}
