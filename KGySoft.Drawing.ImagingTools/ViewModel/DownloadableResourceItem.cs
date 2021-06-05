#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DownloadableResourceItem.cs
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

using System.Globalization;
using KGySoft.ComponentModel;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class DownloadableResourceItem : ObservableObjectBase
    {
        #region Fields

        private string? language;

        #endregion

        #region Properties

        #region Public Properties

        public bool Selected { get => Get<bool>(); set => Set(value); }
        public string Language => language ??= ResHelper.TryGetCulture(CultureName, out CultureInfo? culture)
            ? $"{culture.EnglishName} ({culture.NativeName})"
            : Res.TextUnsupportedCulture(CultureName);

        public string? Author => Info.Author;
        public string ImagingToolsVersion => Info.ImagingToolsVersion.ToString();
        public string? Description => Info.Description;

        #endregion

        #region Internal Properties

        internal LocalizationInfo Info { get; }

        internal string CultureName => Info.CultureName;

        #endregion

        #endregion

        #region Constructors

        internal DownloadableResourceItem(LocalizationInfo info) => this.Info = info;

        #endregion
    }
}