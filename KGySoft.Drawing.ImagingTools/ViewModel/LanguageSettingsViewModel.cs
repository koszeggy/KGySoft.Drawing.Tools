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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using KGySoft.ComponentModel;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Resources;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class LanguageSettingsViewModel : ViewModelBase
    {
        #region Fields

        private List<CultureInfo>? neutralLanguages;
        private HashSet<CultureInfo>? availableResXLanguages;
        private List<CultureInfo>? selectableLanguages;
        private CultureInfo? dirtyCulture;

        #endregion

        #region Properties

        #region Internal Properties

        internal bool AllowResXResources { get => Get<bool>(); set => Set(value); }
        internal bool UseOSLanguage { get => Get<bool>(); set => Set(value); }
        internal bool ExistingLanguagesOnly { get => Get(true); set => Set(value); }
        internal CultureInfo CurrentLanguage { get => Get<CultureInfo>(); set => Set(value); }
        internal IList<CultureInfo> Languages { get => Get<IList<CultureInfo>>(); set => Set(value); }

        internal ICommand ApplyCommand => Get(() => new SimpleCommand(OnApplyCommand));
        internal ICommand SaveConfigCommand => Get(() => new SimpleCommand(OnSaveConfigCommand));
        internal ICommand EditResourcesCommand => Get(() => new SimpleCommand(OnEditResourcesCommand));
        internal ICommand DownloadResourcesCommand => Get(() => new SimpleCommand(OnDownloadResourcesCommand));

        internal ICommandState ApplyCommandState => Get(() => new CommandState());
        internal ICommandState EditResourcesCommandState => Get(() => new CommandState());

        #endregion

        #region Private Properties

        private List<CultureInfo> NeutralLanguages
        {
            get
            {
                if (neutralLanguages == null)
                {
                    CultureInfo[] result = CultureInfo.GetCultures(CultureTypes.NeutralCultures);
                    neutralLanguages = new List<CultureInfo>(result.Length - 1);
                    foreach (CultureInfo ci in result)
                    {
                        if (Equals(ci, CultureInfo.InvariantCulture))
                            continue;
                        neutralLanguages.Add(ci);
                    }
                }

                return neutralLanguages;
            }
        }

        private HashSet<CultureInfo> AvailableLanguages => availableResXLanguages ??= ResHelper.GetAvailableLanguages();
        
        private List<CultureInfo> SelectableLanguages
        {
            get
            {
                if (selectableLanguages == null)
                {
                    selectableLanguages = new List<CultureInfo>(AvailableLanguages);
                    if (!AvailableLanguages.Contains(Res.DefaultLanguage))
                        selectableLanguages.Add(Res.DefaultLanguage);
                }

                return selectableLanguages;
            }
        }

        #endregion

        #endregion

        #region Constructors

        internal LanguageSettingsViewModel()
        {
            CurrentLanguage = LanguageSettings.DisplayLanguage;
            AllowResXResources = Configuration.AllowResXResources;
            UseOSLanguage = Configuration.UseOSLanguage;
            ExistingLanguagesOnly = true; // could be the default value but this way we spare one reset when initializing binding
            ResetLanguages();
            UpdateApplyCommandState();
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
                    EditResourcesCommandState.Enabled = e.NewValue is true;
                    ResetLanguages();
                    UpdateApplyCommandState();
                    break;

                case nameof(UseOSLanguage):
                case nameof(ExistingLanguagesOnly):
                    ResetLanguages();
                    UpdateApplyCommandState();
                    break;

                case nameof(Languages):
                    if (e.OldValue is SortableBindingList<CultureInfo> sbl)
                        sbl.Dispose();
                    break;

                case nameof(CurrentLanguage):
                    UpdateApplyCommandState();
                    break;
            }
        }

        protected override void ApplyDisplayLanguage()
        {
            Debug.Assert(Equals(LanguageSettings.DisplayLanguage, CurrentLanguage), "Only the selected language should be applied");
            UpdateApplyCommandState();
        }

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

            var result = new SortableBindingList<CultureInfo>(ExistingLanguagesOnly ? SelectableLanguages : NeutralLanguages);
            result.ApplySort(nameof(CultureInfo.EnglishName), ListSortDirection.Ascending);
            CultureInfo lastSelectedLanguage = CurrentLanguage;
            Languages = result;
            CurrentLanguage = result.Contains(lastSelectedLanguage) ? lastSelectedLanguage : Res.DefaultLanguage;
        }

        private void UpdateApplyCommandState()
        {
            // Apply is enabled if current language is different than display language,
            // or when turning on/off .resx resources for the default language matters because it also has a resource file
            CultureInfo selected = CurrentLanguage;
            ApplyCommandState.Enabled = !Equals(selected, LanguageSettings.DisplayLanguage)
                || selected.Equals(dirtyCulture)
                || (Equals(selected, Res.DefaultLanguage)
                    && (AllowResXResources ^ LanguageSettings.DynamicResourceManagersSource != ResourceManagerSources.CompiledOnly)
                    && AvailableLanguages.Contains(Res.DefaultLanguage));
        }

        #endregion

        #region Command Handlers

        private void OnApplyCommand()
        {
            dirtyCulture = null;
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
            availableResXLanguages = null;
            selectableLanguages = null;
        }

        private void OnSaveConfigCommand()
        {
            if (!IsModified)
                return;

            if (ApplyCommandState.Enabled)
                OnApplyCommand();

            // saving the configuration
            Configuration.AllowResXResources = AllowResXResources;
            Configuration.UseOSLanguage = UseOSLanguage;
            Configuration.DisplayLanguage = CurrentLanguage;
            try
            {
                Configuration.SaveSettings();
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
            dirtyCulture = viewModel.IsModified ? CurrentLanguage : null;
            availableResXLanguages = null;
            selectableLanguages = null;
            UpdateApplyCommandState();
        }

        private void OnDownloadResourcesCommand()
        {
            using IViewModel<ICollection<LocalizationInfo>> viewModel = ViewModelFactory.CreateDownloadResources();
            ShowChildViewCallback?.Invoke(viewModel);

            // If the language was overwritten, then enabling apply even if it was disabled
            dirtyCulture = viewModel.IsModified && viewModel.GetEditedModel().Any(i => i.CultureName == CurrentLanguage.Name) ? CurrentLanguage : null;
            availableResXLanguages = null;
            selectableLanguages = null;
            ResetLanguages();
            UpdateApplyCommandState();
        }

        #endregion

        #endregion
    }
}
