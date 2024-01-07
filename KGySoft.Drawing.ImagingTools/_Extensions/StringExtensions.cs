#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: StringExtensions.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2024 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System.Globalization;
using System.Text;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal static class StringExtensions
    {
        #region Methods

        /// <summary>
        /// Removes accents from strings for better chances to match a filter pattern when searching.
        /// </summary>
        internal static string StripAccents(this string s)
        {
            string decomposed = s.Normalize(NormalizationForm.FormKD);
            int len = decomposed.Length;
            var stripped = new StringBuilder(len);
            for (int i = 0; i < len; i++)
            {
                UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(decomposed[i]);
                if (category != UnicodeCategory.NonSpacingMark)
                    stripped.Append(decomposed[i]);
            }

            // Note: the string is returned in a decomposed form, which is OK for searching but not for displaying it.
            // If it had to be displayed, then a recombining normalization would also be necessary.
            return stripped.ToString();
        }

        #endregion
    }
}