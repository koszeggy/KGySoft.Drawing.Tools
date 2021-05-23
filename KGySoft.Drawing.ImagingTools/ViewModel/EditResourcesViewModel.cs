#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: EditResourcesViewModel.cs
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

using System.Diagnostics;
using System.Linq;
using KGySoft.Resources;

#region Used Namespaces

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Resources;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Reflection;

#endregion

#region Used Aliases

using ResXResourceSet = KGySoft.Resources.ResXResourceSet;

#endregion

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class EditResourcesViewModel : ViewModelBase
    {
        #region Constants

        internal const string StateSaveExecutedWithError = nameof(StateSaveExecutedWithError);

        #endregion

        #region Fields

        private readonly CultureInfo culture;
        private readonly bool useInvariant;
        private readonly Dictionary<ResourceOwner, (IList<ResourceEntry> ResourceSet, bool IsModified)> resources;

        #endregion

        #region Properties

        internal KeyValuePair<ResourceOwner, string>[] ResourceFiles { get; } // get only because never changes
        internal string TitleCaption { get => Get<string>(); set => Set(value); }
        internal ResourceOwner SelectedLibrary { get => Get<ResourceOwner>(); set => Set(value); }
        internal IList<ResourceEntry> SelectedSet { get => Get<IList<ResourceEntry>>(); set => Set(value); }

        internal ICommand ApplyResourcesCommand => Get(() => new SimpleCommand(OnApplyResourcesCommand));
        internal ICommand SaveResourcesCommand => Get(() => new SimpleCommand(OnSaveResourcesCommand));
        internal ICommand CancelEditCommand => Get(() => new SimpleCommand(OnCancelEditCommand));

        internal ICommandState ApplyResourcesCommandState => Get(() => new CommandState());

        #endregion

        #region Constructors

        internal EditResourcesViewModel(CultureInfo culture)
        {
            this.culture = culture ?? throw new ArgumentNullException(nameof(culture), PublicResources.ArgumentNull);

            // The default language is used as the invariant resource set.
            // The invariant file name is preferred, unless only the language-specific file exists.
            useInvariant = Equals(culture, Res.DefaultLanguage) && !File.Exists(ToFileNameWithPath(ResourceOwner.DrawingTools));
            resources = new Dictionary<ResourceOwner, (IList<ResourceEntry>, bool)>(3, EnumComparer<ResourceOwner>.Comparer);
            ResourceFiles = Enum<ResourceOwner>.GetValues().Select(owner => new KeyValuePair<ResourceOwner, string>(owner, ToFileName(owner))).ToArray();
            ApplyResourcesCommandState.Enabled = !Equals(LanguageSettings.DisplayLanguage, culture);
            UpdateTitle();
            SelectedLibrary = ResourceOwner.DrawingTools;
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
                    UpdateSelectedResources((ResourceOwner)e.NewValue!);
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
            resources.Values.ForEach(v => (v.ResourceSet as IDisposable)?.Dispose());
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void UpdateTitle() => TitleCaption = Res.TitleEditResources($"{culture.EnglishName} ({culture.NativeName})");

        private string ToFileName(ResourceOwner owner) => useInvariant
            ? ResHelper.GetBaseName(owner) + ".resx"
            : $"{ResHelper.GetBaseName(owner)}.{culture.Name}.resx";

        private string ToFileNameWithPath(ResourceOwner owner) => Path.Combine(Res.ResourcesDir, ToFileName(owner));

        private void UpdateSelectedResources(ResourceOwner owner)
        {
            if (resources.TryGetValue(owner, out var value))
            {
                SelectedSet = value.ResourceSet;
                return;
            }

            if (!TryReadResources(owner, out IList<ResourceEntry>? set, out Exception? error))
            {
                if (!Confirm(Res.ConfirmMessageTryRegenerateResource(ToFileName(owner), error.Message)))
                {
                    SelectedSet = Reflector.EmptyArray<ResourceEntry>();
                    return;
                }

                try
                {
                    File.Delete(ToFileNameWithPath(owner));
                }
                catch (Exception e) when (!e.IsCritical())
                {
                    ShowError(Res.ErrorMessageFailedToRegenerateResource(ToFileName(owner), error.Message));
                    SelectedSet = Reflector.EmptyArray<ResourceEntry>();
                    return;
                }

                if (!TryReadResources(owner, out set, out error))
                {
                    ShowError(Res.ErrorMessageFailedToRegenerateResource(ToFileName(owner), error.Message));
                    SelectedSet = Reflector.EmptyArray<ResourceEntry>();
                    return;
                }
            }

            resources[owner] = (set, false);
            SelectedSet = set;
        }

        private bool TryReadResources(ResourceOwner owner, [MaybeNullWhen(false)]out IList<ResourceEntry> set, [MaybeNullWhen(true)]out Exception error)
        {
            try
            {
                // Creating a local resource manager so we can generate the entries that currently found in the compiled resource set.
                // Auto appending only the queried keys so we can add the missing ones since the last creation and also remove the possibly removed ones.
                // Note that this will not generate any .resx files as we use the default AutoSave = None
                using var resourceManger = new DynamicResourceManager(ResHelper.GetBaseName(owner), ResHelper.GetAssembly(owner))
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
                        (string)entry.Value, 
                        resourceManger.GetString((string)entry.Key, culture) ?? LanguageSettings.UntranslatedResourcePrefix + (string)entry.Value));

                result.ListChanged += (_, args) =>
                {
                    if (args.ListChangedType != ListChangedType.ItemChanged)
                        return;

                    ApplyResourcesCommandState.Enabled = true;
                    if (!resources.TryGetValue(owner, out var value) || value.IsModified)
                        return;

                    resources[owner] = (value.ResourceSet, true);
                    SetModified(true);
                };
                result.ApplySort(nameof(ResourceEntry.Key), ListSortDirection.Ascending);
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

        private bool TrySaveResources(ResourceOwner owner, IList<ResourceEntry> set, [MaybeNullWhen(true)]out Exception error)
        {
            // Note: We do not use a DynamicResourceManager for saving. This works because we let the actual DRMs drop their content after saving.
            try
            {
                using var resx = new ResXResourceSet();
                foreach (ResourceEntry res in set)
                    resx.SetObject(res.Key, res.TranslatedText);

                resx.Save(ToFileNameWithPath(owner));
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
            LanguageSettings.DynamicResourceManagersSource = ResourceManagerSources.CompiledAndResX;
            if (Equals(LanguageSettings.DisplayLanguage, culture))
                ResHelper.RaiseLanguageChanged();
            else
                LanguageSettings.DisplayLanguage = culture;
            SetModified(false);
        }

        #endregion

        #region Command Handlers

        private void OnApplyResourcesCommand(ICommandState state)
        {
            Debug.Assert(IsModified || !Equals(culture, LanguageSettings.DisplayLanguage));
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
                    if (Equals(LanguageSettings.DisplayLanguage, culture))
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
