#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ResourceEntry.cs
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

using KGySoft.ComponentModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    internal class ResourceEntry : ObservableObjectBase
    {
        #region Properties

        public string Key { get; }
        public string OriginalText { get; }
        public string TranslatedText { get => Get<string>(); set => Set(value); }

        #endregion

        #region Constructors

        internal ResourceEntry(string key, string originalText, string translatedText)
        {
            Key = key;
            OriginalText = originalText;
            TranslatedText = translatedText;
        }

        #endregion
    }
}