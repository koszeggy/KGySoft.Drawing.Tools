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

using KGySoft.Collections;
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

        internal const string CommandStateExecutedWithError = nameof(CommandStateExecutedWithError);

        #endregion

        #region Fields

        private readonly CultureInfo culture;
        private readonly StringKeyedDictionary<IList<ResourceEntry>> resources = new(3);

        #endregion

        #region Properties

        internal string TitleCaption { get => Get<string>(); set => Set(value); }
        internal string[] ResourceFiles { get => Get<string[]>(); set => Set(value); }
        internal string SelectedFile { get => Get<string>(); set => Set(value); }
        internal IList<ResourceEntry> SelectedSet { get => Get<IList<ResourceEntry>>(); set => Set(value); }

        internal ICommand SaveResourcesCommand => Get(() => new SimpleCommand(OnSaveResourcesCommand));
        internal ICommandState SaveResourcesCommandState => Get(() => new CommandState());

        #endregion

        #region Constructors

        internal EditResourcesViewModel(CultureInfo culture)
        {
            this.culture = culture ?? throw new ArgumentNullException(nameof(culture), PublicResources.ArgumentNull);
            ResourceFiles = new[] { ResHelper.CoreLibrariesBaseName, ResHelper.DrawingLibrariesBaseName, ResHelper.DrawingToolsBaseName };
            ResetTitle();
            SelectedFile = ResHelper.DrawingToolsBaseName;
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            switch (e.PropertyName)
            {
                case nameof(SelectedFile):
                    UpdateSelectedResources((string)e.NewValue!);
                    break;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;
            resources.Values.ForEach(set => (set as IDisposable)?.Dispose());
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void ResetTitle() => TitleCaption = Res.TitleEditResources($"{culture.EnglishName} ({culture.NativeName})");

        private string ToFileName(string baseName) => $"{baseName}.{culture.Name}.resx";

        private string ToFileNameWithPath(string baseName) => Path.Combine(Res.ResourcesDir, ToFileName(baseName));

        private void UpdateSelectedResources(string baseName)
        {
            IList<ResourceEntry>? set = resources.GetValueOrDefault(baseName);
            if (set != null)
            {
                SelectedSet = set;
                return;
            }

            if (!TryReadResources(baseName, out set, out Exception? error))
            {
                if (!Confirm(Res.ConfirmMessageTryRegenerateResource(ToFileName(baseName), error.Message)))
                {
                    SelectedSet = Reflector.EmptyArray<ResourceEntry>();
                    return;
                }

                try
                {
                    File.Delete(ToFileNameWithPath(baseName));
                }
                catch (Exception e) when (!e.IsCritical())
                {
                    ShowError(Res.ErrorMessageFailedToRegenerateResource(ToFileName(baseName), error.Message));
                    SelectedSet = Reflector.EmptyArray<ResourceEntry>();
                    return;
                }

                if (!TryReadResources(baseName, out set, out error))
                {
                    ShowError(Res.ErrorMessageFailedToRegenerateResource(ToFileName(baseName), error.Message));
                    SelectedSet = Reflector.EmptyArray<ResourceEntry>();
                    return;
                }
            }

            resources[baseName] = set;
            SelectedSet = set;
        }

        private bool TryReadResources(string baseName, [MaybeNullWhen(false)]out IList<ResourceEntry> set, [MaybeNullWhen(true)]out Exception error)
        {
            try
            {
                // Creating a local resource manager so we can generate the entries that currently found in the compiled resource set.
                // Auto appending only the queried keys so we can add the missing ones since the last creation and also remove the possibly removed ones.
                // Note that this will not generate any .resx files as we use the default AutoSave = None
                using var resourceManger = new DynamicResourceManager(baseName, ResHelper.GetAssembly(baseName))
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
                    result.Add(new ResourceEntry((string)entry.Key, (string)entry.Value, resourceManger.GetString((string)entry.Key, culture) ?? LanguageSettings.UntranslatedResourcePrefix + entry.Value));

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

        private bool TrySaveResources(string baseName, IList<ResourceEntry> set, [MaybeNullWhen(true)]out Exception error)
        {
            // Note: We do not use a DynamicResourceManager for saving. This works because we let the actual DRMs drop their content after saving.
            try
            {
                using var resx = new ResXResourceSet();
                foreach (ResourceEntry res in set)
                    resx.SetObject(res.Key, res.TranslatedText);

                resx.Save(ToFileNameWithPath(baseName));
                error = null;
                return true;
            }
            catch (Exception e) when (!e.IsCritical())
            {
                error = e;
                return false;
            }
        }

        #endregion

        #region Command Handlers

        private void OnSaveResourcesCommand(ICommandState state)
        {
            foreach (KeyValuePair<string, IList<ResourceEntry>> set in resources)
            {
                if (set.Value.Count == 0)
                    continue;
                if (TrySaveResources(set.Key, set.Value, out Exception? error))
                    continue;

                ShowError(Res.ErrorMessageFailedToSaveResource(ToFileName(set.Key), error.Message));
                state[CommandStateExecutedWithError] = true;
                return;
            }

            state[CommandStateExecutedWithError] = false;
            ResHelper.ReleaseAllResources();
        }

        #endregion

        #endregion
    }
}
