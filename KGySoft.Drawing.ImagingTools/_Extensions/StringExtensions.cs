using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGySoft.Drawing.ImagingTools
{
    internal static class StringExtensions
    {
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
    }
}
