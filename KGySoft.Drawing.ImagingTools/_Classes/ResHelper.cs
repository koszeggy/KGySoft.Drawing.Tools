#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ResHelper.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2023 - All Rights Reserved
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

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal static class ResHelper
    {
        #region Constants

        private const string coreLibrariesBaseName = "KGySoft.CoreLibraries.Messages";
        private const string drawingCoreLibrariesBaseName = "KGySoft.Drawing.Core.Messages";
        private const string drawingLibrariesBaseName = "KGySoft.Drawing.Messages";
        private const string imagingToolsBaseName = "KGySoft.Drawing.ImagingTools.Messages";

        #endregion

        #region Fields

        private static StringKeyedDictionary<CultureInfo>? culturesCache;

        #endregion

        #region Properties

        private static StringKeyedDictionary<CultureInfo> CulturesCache
            => culturesCache ??= CultureInfo.GetCultures(CultureTypes.AllCultures).ToStringKeyedDictionary(ci => ci.Name);

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
            LocalizableLibraries.DrawingCoreLibraries => drawingCoreLibrariesBaseName,
            LocalizableLibraries.DrawingLibraries => drawingLibrariesBaseName,
            LocalizableLibraries.ImagingTools => imagingToolsBaseName,
            _ => throw new ArgumentOutOfRangeException(nameof(library), PublicResources.EnumOutOfRange(library))
        };

        internal static Assembly GetAssembly(LocalizableLibraries library) => library switch
        {
            LocalizableLibraries.CoreLibraries => typeof(LanguageSettings).Assembly,
            LocalizableLibraries.DrawingCoreLibraries => typeof(DrawingCoreModule).Assembly,
            LocalizableLibraries.DrawingLibraries => typeof(DrawingModule).Assembly,
            LocalizableLibraries.ImagingTools => typeof(Res).Assembly,
            _ => throw new ArgumentOutOfRangeException(nameof(library), PublicResources.EnumOutOfRange(library))
        };

        #endregion
    }
}
