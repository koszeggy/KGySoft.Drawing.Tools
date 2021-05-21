#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ResHelper.cs
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
using System.Globalization;
using System.IO;
using System.Reflection;

using KGySoft.Collections;
using KGySoft.CoreLibraries;
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
        private const string drawingToolsBaseName = "KGySoft.Drawing.ImagingTools.Messages";

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

        internal static IList<CultureInfo> GetAvailableLanguages()
        {
            string dir = Res.ResourcesDir;
            try
            {
                var result = new List<CultureInfo> { Res.DefaultLanguage };
                if (!Directory.Exists(dir))
                    return result;

                int startIndex = dir.Length + drawingToolsBaseName.Length + 2;
                string[] files = Directory.GetFiles(dir, $"{drawingToolsBaseName}.*.resx", SearchOption.TopDirectoryOnly);
                foreach (string file in files)
                {
                    StringSegment resName = file.AsSegment(startIndex, file.Length - startIndex - 5);
                    if (CulturesCache.TryGetValue(resName, out CultureInfo? ci) && !ci.In(CultureInfo.InvariantCulture, Res.DefaultLanguage))
                        result.Add(ci);
                }

                return result;
            }
            catch (Exception e) when (!e.IsCritical())
            {
                return new[] { Res.DefaultLanguage };
            }
        }

        internal static string GetBaseName(ResourceOwner owner) => owner switch
        {
            ResourceOwner.CoreLibraries => coreLibrariesBaseName,
            ResourceOwner.DrawingLibraries => drawingLibrariesBaseName,
            ResourceOwner.DrawingTools => drawingToolsBaseName,
            _ => throw new ArgumentOutOfRangeException(nameof(owner), PublicResources.EnumOutOfRange(owner))
        };

        internal static Assembly GetAssembly(ResourceOwner owner) => owner switch
        {
            ResourceOwner.CoreLibraries => typeof(LanguageSettings).Assembly,
            ResourceOwner.DrawingLibraries => typeof(DrawingModule).Assembly,
            ResourceOwner.DrawingTools => typeof(Res).Assembly,
            _ => throw new ArgumentOutOfRangeException(nameof(owner), PublicResources.EnumOutOfRange(owner))
        };

        /// <summary>
        /// Generates possible missing resources for the current language in memory so next Save will persist them.
        /// TODO: Remove when LanguageSettings.EnsureResourcesGenerated will be available.
        /// </summary>
        internal static void EnsureResourcesGenerated()
        {
            foreach (DynamicResourceManager resourceManager in KnownResourceManagers)
                resourceManager.GetExpandoResourceSet(LanguageSettings.DisplayLanguage, ResourceSetRetrieval.CreateIfNotExists, true);
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

        /// <summary>
        /// TODO: Remove when LanguageSettings.RaiseLanguageChanged will be available.
        /// </summary>
        internal static void RaiseLanguageChanged() => Reflector.InvokeMethod(typeof(LanguageSettings), "OnDisplayLanguageChanged", EventArgs.Empty);

        #endregion
    }
}
