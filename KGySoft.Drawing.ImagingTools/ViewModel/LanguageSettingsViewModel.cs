#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: LanguageSettingsViewModel.cs
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
using System.ComponentModel;
using System.Globalization;
using System.IO;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class LanguageSettingsViewModel : ViewModelBase, IValidatingObject
    {
        #region Constants

        internal const string StateSaveExecutedWithError = nameof(StateSaveExecutedWithError);

        #endregion

        #region Fields

        private readonly bool initializing;

        private List<CultureInfo>? neutralLanguages;
        private HashSet<CultureInfo>? availableResXLanguages;
        private List<CultureInfo>? selectableLanguages;
        private string lastSavedResourcesPath;
        private string? customPathError;
        private EventHandler? validationResultsChangedHandler;

        #endregion

        #region Events

        internal event EventHandler? ValidationResultsChanged
        {
            add => value.AddSafe(ref validationResultsChangedHandler);
            remove => value.RemoveSafe(ref validationResultsChangedHandler);
        }

        #endregion

        #region Properties

        #region Public Properties

        public bool IsValid { get => Get(true); private set => Set(value); }
        public ValidationResultsCollection ValidationResults { get => Get(DoValidation); private set => Set(value); }

        #endregion

        #region Internal Properties

        internal bool UseOSLanguage { get => Get<bool>(); set => Set(value); }
        internal bool AllowAnyLanguage { get => Get<bool>(); set => Set(value); }
        internal bool UseCustomResourcePath { get => Get<bool>(); set => Set(value); }
        internal string ResourceCustomPath { get => Get<string>(); set => Set(value); }
        internal CultureInfo CurrentLanguage { get => Get<CultureInfo>(); set => Set(value); }
        internal IList<CultureInfo> Languages { get => Get<IList<CultureInfo>>(); set => Set(value); }

        internal Func<string?>? SelectFolderCallback { get => Get<Func<string?>?>(); set => Set(value); }

        internal ICommand CancelCommand => Get(() => new SimpleCommand(OnCancelCommand));
        internal ICommand ApplyCommand => Get(() => new SimpleCommand(OnApplyCommand));
        internal ICommand SaveConfigCommand => Get(() => new SimpleCommand(OnSaveConfigCommand));
        internal ICommand EditResourcesCommand => Get(() => new SimpleCommand(OnEditResourcesCommand));
        internal ICommand DownloadResourcesCommand => Get(() => new SimpleCommand(OnDownloadResourcesCommand));

        internal ICommand SelectFolderCommand => Get(() => new SimpleCommand(OnSelectFolderCommand));
        internal ICommandState ApplyCommandState => Get(() => new CommandState());

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
            try
            {
                // Making sure that possibly non-existing but generated resource file is created now (if configuration contains such a language/path)
                LanguageSettings.SavePendingResources();
            }
            catch (Exception e) when (!e.IsCritical())
            {
                // Ignoring possible errors here. They will be considered when applying changes though.
            }

            initializing = true;
            CurrentLanguage = Res.DisplayLanguage;
            UseOSLanguage = Configuration.UseOSLanguage;
            lastSavedResourcesPath = Configuration.ResXResourcesCustomPath ?? String.Empty;
            UseCustomResourcePath = lastSavedResourcesPath.Length != 0;
            try
            {
                ResourceCustomPath = lastSavedResourcesPath.Length != 0
                    ? Path.GetFullPath(Path.IsPathRooted(lastSavedResourcesPath) ? lastSavedResourcesPath : Path.Combine(Files.GetExecutingPath(), lastSavedResourcesPath))
                    : Res.DefaultResourcesPath;
            }
            catch (Exception e) when (!e.IsCritical())
            {
                ResourceCustomPath = Res.DefaultResourcesPath;
            }

            initializing = false;
            ResetLanguages();
            UpdateApplyCommandState();
        }

        #endregion

        #region Methods

        #region Internal Methods

        internal void FinalizePath()
        {
            customPathError = null;
            Debug.Assert(UseCustomResourcePath);
            string path = ResourceCustomPath;
            
            if (PathHelper.HasInvalidChars(path))
                customPathError = Res.ErrorMessageInvalidPath;
            else
            {
                try
                {
                    path = Path.GetFullPath(Path.IsPathRooted(path) ? path : Path.Combine(Files.GetExecutingPath(), path));
                }
                catch (Exception e) when (!e.IsCritical())
                {
                    customPathError = e.Message;
                }
            }

            if (customPathError == null)
            {
                // this triggers validation
                ResourceCustomPath = path;
                SetResPath(path);
                return;
            }

            // applying custom error
            Validate();
        }

        #endregion

        #region Protected Methods

        internal override void ViewLoaded()
        {
            base.ViewLoaded();

            // If there is an invalid path in the configuration, then the language file could not be generated
            // and now the default language is selected, which must be able to be applied
            if (!Equals(CurrentLanguage, Res.DisplayLanguage))
                SetModified(true);
            Validate();
        }

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
                    validationResultsChangedHandler?.Invoke(this, EventArgs.Empty);
                    return;

                case nameof(UseOSLanguage):
                case nameof(AllowAnyLanguage):
                    ResetLanguages();
                    UpdateApplyCommandState();
                    break;

                case nameof(UseCustomResourcePath):
                    string path = e.NewValue is true ? Path.Combine(Files.GetExecutingPath(), Res.DefaultResourcesPath) : Res.DefaultResourcesPath;
                    ResourceCustomPath = path;
                    UpdateApplyCommandState();
                    SetResPath(e.NewValue is true ? path : String.Empty);
                    if (e.NewValue is true)
                        SelectFolder();
                    break;

                case nameof(ResourceCustomPath):
                    customPathError = null;
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

            validationResultsChangedHandler = null;
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void Validate() => ValidationResults = DoValidation();

        private ValidationResultsCollection DoValidation()
        {
            if (!UseCustomResourcePath)
                return ValidationResultsCollection.Empty;

            var result = new ValidationResultsCollection();
            if (customPathError != null)
            {
                result.AddError(nameof(ResourceCustomPath), customPathError);
                return result;
            }

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
            if (UseOSLanguage)
            {
                Languages = new[] { Res.OSLanguage };
                CurrentLanguage = Res.OSLanguage;
                return;
            }

            var result = new SortableBindingList<CultureInfo>(AllowAnyLanguage ? NeutralLanguages : SelectableLanguages);
            result.ApplySort(nameof(CultureInfo.NativeName), ListSortDirection.Ascending);
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
                || (Equals(selected, Res.DefaultLanguage) && AvailableLanguages.Contains(Res.DefaultLanguage));
        }

        private void ApplyAndSave()
        {
            if (!IsValid)
            {
                ShowError(Res.ErrorMessageCannotApplyLanguageSettings(ValidationResults.Errors.Message));
                return;
            }

            CultureInfo currentLanguage = CurrentLanguage;
            bool customPath = UseCustomResourcePath;
            if (customPath || !Equals(currentLanguage, Res.DefaultLanguage))
            {
                // If path does not exist, then trying to create it first. SavePendingResources would also create it,
                // but we try to prevent saving the configuration if the path is invalid
                string path = customPath ? ResourceCustomPath : Path.Combine(Files.GetExecutingPath(), Res.DefaultResourcesPath);
                if (!Directory.Exists(path))
                {
                    try
                    {
                        Directory.CreateDirectory(path);
                    }
                    catch (Exception e) when (!e.IsCritical())
                    {
                        customPathError = e.Message;
                        Validate();
                        ShowError(Res.ErrorMessageCannotApplyLanguageSettings(e.Message));
                        return;
                    }
                }
            }

            if (!IsModified)
                return;

            SaveConfiguration();

            // Applying the current language
            if (Equals(Res.DisplayLanguage, currentLanguage))
                Res.OnDisplayLanguageChanged();
            else
                Res.DisplayLanguage = currentLanguage;

            try
            {
                // This save is just for the possibly generated new resources and to check the path.
                // Actual edits are saved explicitly by EditResourcesViewModel
                // Note: Ensure is not really needed because main .resx is generated, while others are saved on demand in the editor, too
                LanguageSettings.SavePendingResources();
            }
            catch (Exception e) when (!e.IsCritical())
            {
                customPathError = e.Message;
                Validate();
                ShowError(Res.ErrorMessageCannotApplyLanguageSettings(e.Message));
            }
            finally
            {
                availableResXLanguages = null;
                selectableLanguages = null;
            }
        }

        private void SaveConfiguration()
        {
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

        private void SelectFolder()
        {
            string? path = SelectFolderCallback?.Invoke();
            if (path == null)
                return;
            ResourceCustomPath = path;
            FinalizePath();
        }

        #endregion

        #region Command Handlers

        private void OnCancelCommand() => Res.ResourcesDir = lastSavedResourcesPath;

        // Both Save and Apply do the same thing.
        // The only differences are that Apply has an Enabled state,
        // and that the View may bind Save to a button that closes the view is there was no error
        private void OnApplyCommand() => ApplyAndSave();
        private void OnSaveConfigCommand(ICommandState state)
        {
            ApplyAndSave();
            state[StateSaveExecutedWithError] = !IsValid;
        }

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

        private void OnSelectFolderCommand() => SelectFolder();

        #endregion

        #endregion
    }
}
