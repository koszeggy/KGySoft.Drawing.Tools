#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ResHelper.cs
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;

using KGySoft.Collections;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Reflection;
using KGySoft.Resources;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal static class ResHelper
    {
        #region Constants

        private const string coreLibrariesBaseName = "KGySoft.CoreLibraries.Messages";
        private const string drawingLibrariesBaseName = "KGySoft.Drawing.Messages";
        private const string imagingToolsBaseName = "KGySoft.Drawing.ImagingTools.Messages";

        #endregion

        #region Fields

        private static StringKeyedDictionary<CultureInfo>? culturesCache;
        private static DynamicResourceManager[]? knownResourceManagers;

        #endregion

        #region Properties

        private static StringKeyedDictionary<CultureInfo> CulturesCache
            => culturesCache ??= CultureInfo.GetCultures(CultureTypes.AllCultures).ToStringKeyedDictionary(ci => ci.Name);

        private static DynamicResourceManager[] KnownResourceManagers
            => knownResourceManagers ??= new[]
        {
            (DynamicResourceManager)Reflector.GetField(Reflector.ResolveType(typeof(LanguageSettings).Assembly, "KGySoft.Res")!, "resourceManager")!,
            (DynamicResourceManager)Reflector.GetField(Reflector.ResolveType(typeof(DrawingModule).Assembly, "KGySoft.Res")!, "resourceManager")!,
            (DynamicResourceManager)Reflector.GetField(typeof(Res), "resourceManager")!
        };

        #endregion

        #region Methods

        internal static HashSet<CultureInfo> GetAvailableLanguages()
        {
            string dir = Res.ResourcesDir;
            var result = new HashSet<CultureInfo>();
            try
            {
                if (!Directory.Exists(dir))
                    return result;

                int startIndex = dir.Length + imagingToolsBaseName.Length + 2;
                string[] files = Directory.GetFiles(dir, $"{imagingToolsBaseName}.*.resx", SearchOption.TopDirectoryOnly);
                foreach (string file in files)
                {
                    StringSegment resName = file.AsSegment(startIndex, file.Length - startIndex - 5);
                    if (CulturesCache.TryGetValue(resName, out CultureInfo? ci) && !ci.Equals(CultureInfo.InvariantCulture))
                        result.Add(ci);
                }

                // checking the invariant resource as it should act as default language
                if (!result.Contains(Res.DefaultLanguage) && File.Exists(Path.Combine(dir, $"{imagingToolsBaseName}.resx")))
                    result.Add(Res.DefaultLanguage);
                return result;
            }
            catch (Exception e) when (!e.IsCritical())
            {
                result.Clear();
                return result;
            }
        }

        internal static bool TryGetCulture(string name, [MaybeNullWhen(false)] out CultureInfo culture) => CulturesCache.TryGetValue(name, out culture);

        internal static string GetBaseName(LocalizableLibraries library) => library switch
        {
            LocalizableLibraries.CoreLibraries => coreLibrariesBaseName,
            LocalizableLibraries.DrawingLibraries => drawingLibrariesBaseName,
            LocalizableLibraries.ImagingTools => imagingToolsBaseName,
            _ => throw new ArgumentOutOfRangeException(nameof(library), PublicResources.EnumOutOfRange(library))
        };

        internal static Assembly GetAssembly(LocalizableLibraries library) => library switch
        {
            LocalizableLibraries.CoreLibraries => typeof(LanguageSettings).Assembly,
            LocalizableLibraries.DrawingLibraries => typeof(DrawingModule).Assembly,
            LocalizableLibraries.ImagingTools => typeof(Res).Assembly,
            _ => throw new ArgumentOutOfRangeException(nameof(library), PublicResources.EnumOutOfRange(library))
        };

        /// <summary>
        /// Generates possible missing resources for the current language in memory so next Save will persist them.
        /// TODO: Remove when LanguageSettings.EnsureResourcesGenerated will be available.
        /// </summary>
        internal static void EnsureResourcesGenerated()
        {
            foreach (DynamicResourceManager resourceManager in KnownResourceManagers)
                resourceManager.GetExpandoResourceSet(Res.DisplayLanguage, ResourceSetRetrieval.CreateIfNotExists, true);
        }

        /// <summary>
        /// Saves the pending resources of centralized DRMs.
        /// TODO: Remove when LanguageSettings.SavePendingResources will be available.
        /// </summary>
        internal static void SavePendingResources()
        {
            foreach (DynamicResourceManager resourceManager in KnownResourceManagers)
                resourceManager.SaveAllResources();
        }

        /// <summary>
        /// Releases all resource sets without saving of centralized DRMs.
        /// TODO: Remove when LanguageSettings.ReleaseAllResources will be available.
        /// </summary>
        internal static void ReleaseAllResources()
        {
            foreach (DynamicResourceManager resourceManager in KnownResourceManagers)
                resourceManager.ReleaseAllResources();
        }

        #endregion
    }
}
