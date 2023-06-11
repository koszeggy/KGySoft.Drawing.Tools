#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DownloadableResourceItem.cs
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

using System.Globalization;

using KGySoft.ComponentModel;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    // We could just derive from ValidatingObjectBase but the validation does not depend on property change so we provide a lightweight implementation
    internal class DownloadableResourceItem : ObservableObjectBase, IValidatingObject
    {
        #region Fields

        private string? language;
        private ValidationResultsCollection? validationResults;

        #endregion

        #region Properties

        #region Public Properties

        public bool Selected { get => Get<bool>(); set => Set(value); }

        // Note: this could be also observable and we could subscribe language change just to adjust unsupported culture
        // on-the-fly but this will never happen unless using this class from API. Subscribing language change from every item
        // just for updating the possible unsupported cultures is not worth it.
        public string Language => language ??= ResHelper.TryGetCulture(CultureName, out CultureInfo? culture)
            ? $"{culture.EnglishName} ({culture.NativeName})"
            : Res.TextUnsupportedCulture(CultureName);

        public string? Author => Info.Author;
        public string ImagingToolsVersion => Info.ImagingToolsVersion.ToString();
        public string? Description => Info.Description;

        public bool IsValid => true;
        public ValidationResultsCollection ValidationResults => validationResults ??= CreateValidationResults();

        #endregion

        #region Internal Properties

        internal LocalizationInfo Info { get; }

        internal string CultureName => Info.CultureName;

        #endregion

        #endregion

        #region Constructors

        internal DownloadableResourceItem(LocalizationInfo info) => Info = info;

        #endregion

        #region Methods

        private ValidationResultsCollection CreateValidationResults()
        {
            var result = new ValidationResultsCollection();

            if (!InstallationManager.ImagingToolsVersion.NormalizedEquals(Info.Version))
                result.AddInfo(nameof(ImagingToolsVersion), Res.InfoMessageResourceVersionMismatch);
            if (!ResHelper.TryGetCulture(CultureName, out var _))
                result.AddWarning(nameof(Language), Res.WarningMessageUnsupportedCulture);

            return result;
        }

        #endregion
    }
}