#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: EditResourcesViewModel.cs
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


#region Used Namespaces

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Reflection;
using KGySoft.Resources;

#endregion

#region Used Aliases

using ResXResourceSet = KGySoft.Resources.ResXResourceSet;

#endregion

#endregion

#region Suppressions

#if NETCOREAPP3_0
#pragma warning disable CS8605 // Unboxing a possibly null value. - false alarm for iterating through a non-generic dictionary
#endif

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class EditResourcesViewModel : ViewModelBase
    {
        #region Constants

        #region Internal Constants
        
        internal const string StateSaveExecutedWithError = nameof(StateSaveExecutedWithError);

        #endregion

        #region Private Constants

        private const CompareOptions cultureSpecificCompareOptions = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth;

        #endregion

        #endregion

        #region Fields

        private readonly CultureInfo culture;
        private readonly bool useInvariant;
        private readonly Dictionary<LocalizableLibraries, (IList<ResourceEntry> ResourceSet, bool IsModified)> resources;

        #endregion

        #region Properties

        internal KeyValuePair<LocalizableLibraries, string>[] ResourceFiles { get; } // get only because never changes
        internal string TitleCaption { get => Get<string>(); set => Set(value); }
        internal LocalizableLibraries SelectedLibrary { get => Get<LocalizableLibraries>(); set => Set(value); }
        internal string Filter { get => Get<string>(""); set => Set(value); }
        internal IList<ResourceEntry>? FilteredSet { get => Get<IList<ResourceEntry>?>(); set => Set(value); }

        internal ICommand ApplyResourcesCommand => Get(() => new SimpleCommand(OnApplyResourcesCommand));
        internal ICommand SaveResourcesCommand => Get(() => new SimpleCommand(OnSaveResourcesCommand));
        internal ICommand CancelEditCommand => Get(() => new SimpleCommand(OnCancelEditCommand));

        internal ICommandState ApplyResourcesCommandState => Get(() => new CommandState());

        internal Action? SaveConfigurationCallback { get; set; }

        #endregion

        #region Constructors

        internal EditResourcesViewModel(CultureInfo culture)
        {
            this.culture = culture ?? throw new ArgumentNullException(nameof(culture), PublicResources.ArgumentNull);

            // The default language is used as the invariant resource set.
            // The invariant file name is preferred, unless only the language-specific file exists.
            useInvariant = Equals(culture, Res.DefaultLanguage) && !File.Exists(ToFileNameWithPath(LocalizableLibraries.ImagingTools));
            resources = new Dictionary<LocalizableLibraries, (IList<ResourceEntry>, bool)>(3, EnumComparer<LocalizableLibraries>.Comparer);
            Set(String.Empty, false, nameof(Filter));
            ResourceFiles = Enum<LocalizableLibraries>.GetFlags().Select(lib => new KeyValuePair<LocalizableLibraries, string>(lib, ToFileName(lib))).ToArray();
            ApplyResourcesCommandState.Enabled = !Equals(Res.DisplayLanguage, culture);
            UpdateTitle();
            SelectedLibrary = LocalizableLibraries.ImagingTools;
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override bool AffectsModifiedState(string propertyName) => false; // set explicitly

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            switch (e.PropertyName)
            {
                case nameof(SelectedLibrary):
                case nameof(Filter):
                    ApplySelection();
                    break;
            }
        }

        protected override void ApplyDisplayLanguage()
        {
            UpdateTitle();
            ApplyResourcesCommandState.Enabled = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
            {
                resources.Values.ForEach(v => (v.ResourceSet as IDisposable)?.Dispose());
                (FilteredSet as IDisposable)?.Dispose();
            }

            SaveConfigurationCallback = null;
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void UpdateTitle() => TitleCaption = Res.TitleEditResources($"{culture.EnglishName} ({culture.NativeName})");

        private string ToFileName(LocalizableLibraries library) => useInvariant
            ? ResHelper.GetBaseName(library) + ".resx"
            : $"{ResHelper.GetBaseName(library)}.{culture.Name}.resx";

        private string ToFileNameWithPath(LocalizableLibraries library) => Path.Combine(Res.ResourcesDir, ToFileName(library));

        private void ApplySelection()
        {
            LocalizableLibraries library = SelectedLibrary;

            if (resources.TryGetValue(library, out var value))
            {
                ApplyFilter(value.ResourceSet);
                return;
            }

            if (!TryReadResources(library, out IList<ResourceEntry>? set, out Exception? error))
            {
                if (!Confirm(Res.ConfirmMessageTryRegenerateResource(ToFileName(library), error.Message)))
                {
                    ApplyFilter(Reflector.EmptyArray<ResourceEntry>());
                    return;
                }

                try
                {
                    File.Delete(ToFileNameWithPath(library));
                }
                catch (Exception e) when (!e.IsCritical())
                {
                    ShowError(Res.ErrorMessageFailedToRegenerateResource(ToFileName(library), error.Message));
                    ApplyFilter(Reflector.EmptyArray<ResourceEntry>());
                    return;
                }

                if (!TryReadResources(library, out set, out error))
                {
                    ShowError(Res.ErrorMessageFailedToRegenerateResource(ToFileName(library), error.Message));
                    ApplyFilter(Reflector.EmptyArray<ResourceEntry>());
                    return;
                }
            }

            resources[library] = (set, false);
            ApplyFilter(set);
        }

#if NETCOREAPP3_0_OR_GREATER
        [SuppressMessage("Usage", "CA2249:Consider using 'string.Contains' instead of 'string.IndexOf'",
            Justification = "Cannot use Contains because it is not available on all targeted platforms")] 
#endif
        private void ApplyFilter(IList<ResourceEntry> set)
        {
            if (FilteredSet is SortableBindingList<ResourceEntry> oldSet)
                oldSet.ListChanged -= FilteredSet_ListChanged;

            if (set.Count == 0)
            {
                FilteredSet = set;
                return;
            }

            Debug.Assert(set is SortableBindingList<ResourceEntry>, "Non-empty set is expected to be a SortableBindingList");
            string filter = Filter.StripAccents();
            SortableBindingList<ResourceEntry> newSet;
            if (filter.Length == 0)
                newSet = (SortableBindingList<ResourceEntry>)set;
            else
            {
                newSet = new SortableBindingList<ResourceEntry>(new List<ResourceEntry>());
                CompareInfo cultureSpecificInfo = culture.CompareInfo;
                CompareInfo invariantInfo = CultureInfo.InvariantCulture.CompareInfo;
                foreach (ResourceEntry entry in set)
                {
                    // Using ordinal search for key, invariant for original text (to allow ignoring char width, for example),
                    // and both ordinal and culture-specific search for the translated text because culture specific fails to match some patterns,
                    // eg.: "Vissza" is not found with the search term "viss" using the Hungarian culture.
                    // Stripping is because IgnoreNonSpace fails with some accents, eg. "ö" matches "o" using German culture but does not match with Hungarian.
                    string strippedTranslated;
                    if (entry.Key.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0
                        || invariantInfo.IndexOf(entry.OriginalText, filter, cultureSpecificCompareOptions) >= 0
                        || (strippedTranslated = entry.TranslatedText.StripAccents()).IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0
                        || cultureSpecificInfo.IndexOf(strippedTranslated, filter, cultureSpecificCompareOptions) >= 0)
                    {
                        newSet.Add(entry);
                    }
                }
            }

            newSet.ListChanged += FilteredSet_ListChanged;
            ApplySort(newSet);
            FilteredSet = newSet;
        }

        private bool TryReadResources(LocalizableLibraries library, [MaybeNullWhen(false)]out IList<ResourceEntry> set, [MaybeNullWhen(true)]out Exception error)
        {
            try
            {
                // Creating a local resource manager so we can generate the entries that currently found in the compiled resource set.
                // Auto appending only the queried keys so we can add the missing ones since the last creation and also remove the possibly removed ones.
                // Note that this will not generate any .resx files as we use the default AutoSave = None
                using var resourceManger = new DynamicResourceManager(ResHelper.GetBaseName(library), ResHelper.GetAssembly(library))
                {
                    SafeMode = true,
                    Source = ResourceManagerSources.CompiledOnly,
                    AutoAppend = AutoAppendOptions.AppendFirstNeutralCulture,
                };

                ResourceSet compiled = resourceManger.GetResourceSet(CultureInfo.InvariantCulture, true, false)!;
                resourceManger.Source = ResourceManagerSources.CompiledAndResX;

                // Note: this way we add even possibly missing entries that were added to compiled resources since last creation while removed entries will be skipped
                var result = new SortableBindingList<ResourceEntry>();
                foreach (DictionaryEntry entry in compiled)
                    result.Add(new ResourceEntry((string)entry.Key,
                        (string)entry.Value!,
                        resourceManger.GetString((string)entry.Key, culture) ?? LanguageSettings.UntranslatedResourcePrefix + (string)entry.Value!));

                error = null;
                set = result;
                return true;
            }
            catch (Exception e) when (!e.IsCritical())
            {
                error = e;
                set = null;
                return false;
            }
        }

        private void ApplySort(SortableBindingList<ResourceEntry> set)
        {
            var hint = FilteredSet as IBindingList;
            ListSortDirection direction = hint?.IsSorted == true ? hint.SortDirection : ListSortDirection.Ascending;
            PropertyDescriptor? sortProperty = hint?.SortProperty;

            if (sortProperty != null)
                set.ApplySort(sortProperty, direction);
            else
                set.ApplySort(nameof(ResourceEntry.Key), direction);
        }

        private bool TrySaveResources(LocalizableLibraries library, IList<ResourceEntry> set, [MaybeNullWhen(true)]out Exception error)
        {
            // Note: We do not use a DynamicResourceManager for saving. This works because we let the actual DRMs drop their content after saving.
            try
            {
                using var resx = new ResXResourceSet();
                foreach (ResourceEntry res in set)
                    resx.SetObject(res.Key, res.TranslatedText);

                string fileName = ToFileNameWithPath(library);
                string dirName = Path.GetDirectoryName(fileName)!;
                if (!Directory.Exists(dirName))
                    Directory.CreateDirectory(dirName);
                resx.Save(fileName);
                error = null;
                return true;
            }
            catch (Exception e) when (!e.IsCritical())
            {
                error = e;
                return false;
            }
        }

        private bool TrySaveResources()
        {
            foreach (var set in resources)
            {
                if (!set.Value.IsModified)
                    continue;
                if (TrySaveResources(set.Key, set.Value.ResourceSet, out Exception? error))
                    continue;

                ShowError(Res.ErrorMessageFailedToSaveResource(ToFileName(set.Key), error.Message));
                return false;
            }

            return true;
        }

        private void ApplyResources()
        {
            // As a first step, we save the configuration explicitly.
            // Otherwise, it would be possible to select a language without applying it, then editing the resources and applying the new language here,
            // in which case the configuration may remain unsaved.
            SaveConfigurationCallback?.Invoke();
            LanguageSettings.DynamicResourceManagersSource = ResourceManagerSources.CompiledAndResX;
            if (Equals(Res.DisplayLanguage, culture))
                Res.OnDisplayLanguageChanged();
            else
                Res.DisplayLanguage = culture;
            SetModified(false);
        }

        #endregion

        #region Event Handlers

        private void FilteredSet_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (!IsViewLoaded || e.ListChangedType != ListChangedType.ItemChanged || e.PropertyDescriptor?.Name != nameof(ResourceEntry.TranslatedText))
                return;

            ApplyResourcesCommandState.Enabled = true;
            LocalizableLibraries library = SelectedLibrary;
            if (resources.TryGetValue(library, out var value) && !value.IsModified)
                resources[library] = (value.ResourceSet, true);

            SetModified(true);
        }


        #endregion

        #region Command Handlers

        private void OnApplyResourcesCommand(ICommandState state)
        {
            Debug.Assert(IsModified || !Equals(culture, Res.DisplayLanguage));
            ResHelper.ReleaseAllResources();
            bool success = TrySaveResources();
            if (success)
                ApplyResources();
        }

        private void OnSaveResourcesCommand(ICommandState state)
        {
            bool success = true;
            if (IsModified)
            {
                ResHelper.ReleaseAllResources();
                success = TrySaveResources();
                if (success)
                {
                    if (Equals(Res.DisplayLanguage, culture))
                        ApplyResources();
                }
            }

            state[StateSaveExecutedWithError] = !success;
        }

        private void OnCancelEditCommand() => SetModified(false);

        #endregion

        #endregion
    }
}
