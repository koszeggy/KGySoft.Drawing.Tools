#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: LanguageSettingsViewModel.cs
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Resources;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class LanguageSettingsViewModel : ViewModelBase, IValidatingObject
    {
        #region Fields

        private readonly bool initializing;

        private List<CultureInfo>? neutralLanguages;
        private HashSet<CultureInfo>? availableResXLanguages;
        private List<CultureInfo>? selectableLanguages;
        private string lastSavedResourcesPath;

        #endregion

        #region Events

        internal event EventHandler? ValidationResultsChanged
        {
            add => ValidationResultsChangedHandler += value;
            remove => ValidationResultsChangedHandler -= value;
        }

        #endregion

        #region Properties

        #region Public Properties

        public bool IsValid { get => Get<bool>(); private set => Set(value); }
        public ValidationResultsCollection ValidationResults { get => Get(DoValidation); private set => Set(value); }

        #endregion

        #region Internal Properties

        internal bool AllowResXResources { get => Get<bool>(); set => Set(value); }
        internal bool UseOSLanguage { get => Get<bool>(); set => Set(value); }
        internal bool ExistingLanguagesOnly { get => Get<bool>(); set => Set(value); }
        internal bool UseCustomResourcePath { get => Get<bool>(); set => Set(value); }
        internal string ResourceCustomPath { get => Get<string>(); set => Set(value); }
        internal CultureInfo CurrentLanguage { get => Get<CultureInfo>(); set => Set(value); }
        internal IList<CultureInfo> Languages { get => Get<IList<CultureInfo>>(); set => Set(value); }

        internal ICommand CancelCommand => Get(() => new SimpleCommand(OnCancelCommand));
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

        private EventHandler? ValidationResultsChangedHandler { get => Get<EventHandler?>(); set => Set(value); }

        #endregion

        #endregion

        #region Constructors

        internal LanguageSettingsViewModel()
        {
            // Making sure that possibly non-existing but generated resource file is created now (if configuration contains such a language/path)
            LanguageSettings.SavePendingResources();

            initializing = true;
            CurrentLanguage = Res.DisplayLanguage;
            AllowResXResources = Configuration.AllowResXResources;
            UseOSLanguage = Configuration.UseOSLanguage;
            ExistingLanguagesOnly = true; // could be the default value but this way we spare one reset when initializing binding
            lastSavedResourcesPath = Configuration.ResXResourcesCustomPath ?? String.Empty;
            UseCustomResourcePath = lastSavedResourcesPath.Length != 0;
            ResourceCustomPath = lastSavedResourcesPath.Length != 0 ? Path.GetFullPath(Files.GetRelativePath(lastSavedResourcesPath, Files.GetExecutingPath())) : Res.DefaultResourcesPath;
            initializing = false;
            ResetLanguages();
            UpdateApplyCommandState();
        }

        #endregion

        #region Methods

        #region Internal Methods

        internal void FinalizePath()
        {
            Debug.Assert(UseCustomResourcePath);
            string path = ResourceCustomPath;
            path = Path.GetFullPath(Path.IsPathRooted(path) ? path : Path.Combine(Files.GetExecutingPath(), path));
            ResourceCustomPath = path;
            SetResPath(path);
        }

        #endregion

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (initializing)
                return;

            switch (e.PropertyName)
            {
                case nameof(ValidationResults):
                    var validationResults = (ValidationResultsCollection)e.NewValue!;
                    IsValid = !validationResults.HasErrors;
                    ValidationResultsChangedHandler?.Invoke(this, EventArgs.Empty);
                    return;

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

                case nameof(UseCustomResourcePath):
                    string path = e.NewValue is true ? Path.Combine(Files.GetExecutingPath(), Res.DefaultResourcesPath) : Res.DefaultResourcesPath;
                    ResourceCustomPath = path;
                    UpdateApplyCommandState();
                    SetResPath(e.NewValue is true ? path : String.Empty);
                    break;

                case nameof(ResourceCustomPath):
                    Validate();
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

        protected override void ApplyDisplayLanguage() => UpdateApplyCommandState();

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

        private void Validate()
        {
            if (!IsViewLoaded)
                return;
            ValidationResults = DoValidation();
        }

        private ValidationResultsCollection DoValidation()
        {
            if (!UseCustomResourcePath)
                return ValidationResultsCollection.Empty;

            var result = new ValidationResultsCollection();
            string path = ResourceCustomPath;
            if (path.Length == 0)
                result.AddError(nameof(ResourceCustomPath), Res.ErrorMessagePathIsEmpty);
            else if (PathHelper.HasInvalidChars(path))
                result.AddError(nameof(ResourceCustomPath), Res.ErrorMessageInvalidPath);
            else if (File.Exists(path))
                result.AddError(nameof(ResourceCustomPath), Res.ErrorMessageFileNotExpected);
            else if (!Directory.Exists(path))
                result.AddWarning(nameof(ResourceCustomPath), Res.WarningMessageDirectoryNotExists);
            return result;
        }

        private void SetResPath(string path)
        {
            Res.ResourcesDir = path;
            availableResXLanguages = null;
            selectableLanguages = null;
            ResetLanguages();
        }

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
            CultureInfo selected = CurrentLanguage;
            ApplyCommandState.Enabled = !Equals(selected, Res.DisplayLanguage)
                // or when path has been changed
                || ((String.IsNullOrEmpty(lastSavedResourcesPath) ^ !UseCustomResourcePath)
                    || (!String.IsNullOrEmpty(lastSavedResourcesPath) && lastSavedResourcesPath != ResourceCustomPath))
                // or when turning on/off .resx resources for the default language matters because it also has a resource file
                || (Equals(selected, Res.DefaultLanguage)
                    && (AllowResXResources ^ LanguageSettings.DynamicResourceManagersSource != ResourceManagerSources.CompiledOnly)
                    && AvailableLanguages.Contains(Res.DefaultLanguage));
        }

        private void ApplyAndSave()
        {
            if (!IsModified)
                return;

            SaveConfiguration();

            // Applying the current language
            CultureInfo currentLanguage = CurrentLanguage;
            LanguageSettings.DynamicResourceManagersSource = AllowResXResources ? ResourceManagerSources.CompiledAndResX : ResourceManagerSources.CompiledOnly;

            if (Equals(Res.DisplayLanguage, currentLanguage))
                Res.OnDisplayLanguageChanged();
            else
                Res.DisplayLanguage = currentLanguage;

            // This save is just for the possibly generated new resources. Actual edits are saved explicitly by EditResourcesViewModel
            // Note: Ensure is not really needed because main .resx is generated, while others are saved on demand in the editor, too
            LanguageSettings.SavePendingResources();
            availableResXLanguages = null;
            selectableLanguages = null;
        }

        private void SaveConfiguration()
        {
            Configuration.AllowResXResources = AllowResXResources;
            Configuration.UseOSLanguage = UseOSLanguage;
            Configuration.DisplayLanguage = CurrentLanguage;
            string path = UseCustomResourcePath ? ResourceCustomPath : String.Empty;
            if (path != lastSavedResourcesPath)
            {
                Configuration.ResXResourcesCustomPath = Res.ResourcesDir = path;
                Res.ApplyResourcesDir();
                lastSavedResourcesPath = path;
            }

            try
            {
                Configuration.SaveSettings();
            }
            catch (Exception e) when (!e.IsCritical())
            {
                ShowError(Res.ErrorMessageFailedToSaveSettings(e.Message));
            }
        }

        #endregion

        #region Command Handlers

        private void OnCancelCommand() => Res.ResourcesDir = lastSavedResourcesPath;

        // Both Save and Apply do the same thing.
        // The only difference is that Apply has an Enabled state and the View may bind Save to a button that closes the view.
        private void OnApplyCommand() => ApplyAndSave();
        private void OnSaveConfigCommand() => ApplyAndSave();

        private void OnEditResourcesCommand()
        {
            using IViewModel viewModel = ViewModelFactory.CreateEditResources(CurrentLanguage, 
                ResourceCustomPath != (lastSavedResourcesPath.Length == 0 ? Res.DefaultResourcesPath : lastSavedResourcesPath));
            if (viewModel is EditResourcesViewModel vm)
                vm.SaveConfigurationCallback = SaveConfiguration;
            ShowChildViewCallback?.Invoke(viewModel);
            availableResXLanguages = null;
            selectableLanguages = null;
        }

        private void OnDownloadResourcesCommand()
        {
            using IViewModel<ICollection<LocalizationInfo>> viewModel = ViewModelFactory.CreateDownloadResources();
            ShowChildViewCallback?.Invoke(viewModel);
            availableResXLanguages = null;
            selectableLanguages = null;
            ResetLanguages();
        }

        #endregion

        #endregion
    }
}
