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

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

using KGySoft.ComponentModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    internal class ResourceEntry : ValidatingObjectBase
    {
        #region Nested Types

        private enum State
        {
            Text,
            Index,
            Padding,
            Format
        }

        #endregion

        #region Fields

        private int? placeholderCount;

        #endregion

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

        #region Methods

        #region Protected Methods
        
        protected override ValidationResultsCollection DoValidation()
        {
            #region Local Methods

            static void AddFormatError(ValidationResultsCollection validationResults) => validationResults.AddError(nameof(TranslatedText), Res.ErrorMessageResourceFormatError);

            bool HandlePlaceholder(ValidationResultsCollection validationResults, int index, ref int used)
            {
                if (index > placeholderCount - 1)
                {
                    validationResults.AddError(nameof(TranslatedText), Res.ErrorMessageResourcePlaceholderIndexInvalid(index));
                    return false;
                }

                used |= 1 << index;
                return true;
            }

            #endregion

            var result = new ValidationResultsCollection();
            EnsurePlaceholderCount();
            if (placeholderCount == 0)
                return result;

            Debug.Assert(placeholderCount < 32, "No resource is expected to contain more than 32 placeholders in KGy SOFT Libraries");
            int usedPlaceholders = 0;
            string value = TranslatedText;
            var state = State.Text;
            int currentIndex = 0;
            using var reader = new StringReader(value);
            while (reader.Read() is int c and >= 0)
            {
                switch (state)
                {
                    // Out of placeholder part
                    case State.Text:

                        switch (c)
                        {
                            // { - placeholder candidate: looking ahead one character
                            case '{':
                                switch (c = reader.Read())
                                {
                                    // {{ - escape
                                    case '{':
                                        continue;

                                    // 0..9 - an actual placeholder
                                    case >= '0' and <= '9':
                                        currentIndex = c - '0';
                                        state = State.Index;
                                        continue;

                                    // other character or end of string (whitespace is not allowed here)
                                    default:
                                        AddFormatError(result);
                                        return result;
                                }

                            // } - only escape is allowed as we are not in placeholder now
                            case '}':
                                if (reader.Read() == '}')
                                    continue;
                                AddFormatError(result);
                                return result;

                            // anything else - staying in text
                            default:
                                continue;
                        }

                    // Inside placeholder index
                    case State.Index:

                        // more digits: staying in index
                        if (c is >= '0' and <= '9')
                        {
                            currentIndex *= 10;
                            currentIndex += c - '0';
                            continue;
                        }

                        // consuming possible spaces (no other whitespace is allowed)
                        while (c == ' ')
                            c = reader.Read();

                        switch (c)
                        {
                            // end of placeholder
                            case '}':
                                if (!HandlePlaceholder(result, currentIndex, ref usedPlaceholders))
                                    return result;
                                state = State.Text;
                                continue;

                            // padding
                            case ',':
                                state = State.Padding;
                                continue;

                            // format specifier
                            case ':':
                                state = State.Format;
                                continue;

                            default:
                                AddFormatError(result);
                                return result;
                        }

                    // Inside placeholder padding
                    case State.Padding:

                        // consuming possible spaces before padding count
                        while (c == ' ')
                            c = reader.Read();

                        // consuming possible negative sign
                        if (c == '-')
                            c = reader.Read();

                        bool hasDigit = false;

                        // consuming digits: staying in index
                        while (c is >= '0' and <= '9')
                        {
                            c = reader.Read();
                            hasDigit = true;
                        }

                        // consuming possible spaces after padding count
                        while (c == ' ')
                            c = reader.Read();

                        if (!hasDigit)
                        {
                            AddFormatError(result);
                            return result;
                        }

                        switch (c)
                        {
                            // end of placeholder
                            case '}':
                                if (!HandlePlaceholder(result, currentIndex, ref usedPlaceholders))
                                    return result;
                                state = State.Text;
                                continue;

                            // format specifier
                            case ':':
                                state = State.Format;
                                continue;

                            default:
                                AddFormatError(result);
                                return result;
                        }

                    // Inside placeholder format specifier
                    case State.Format:
                        switch (c)
                        {
                            // possible end of placeholder: looking ahead one character
                            case '}':
                                if (reader.Peek() == '}')
                                {
                                    reader.Read();
                                    continue;
                                }

                                if (!HandlePlaceholder(result, currentIndex, ref usedPlaceholders))
                                    return result;
                                state = State.Text;
                                continue;

                            // in format specifier { is allowed only escaped as part of the format
                            case '{':
                                if (reader.Peek() == '{')
                                {
                                    reader.Read();
                                    continue;
                                }

                                AddFormatError(result);
                                return result;

                            default:
                                continue;
                        }
                }
            }

            if (state != State.Text)
                AddFormatError(result);
            else if (usedPlaceholders != (1 << placeholderCount) - 1)
                result.AddWarning(nameof(TranslatedText), Res.ErrorMessageResourcePlaceholderUnusedIndices);
            return result;
        }

        #endregion

        #region Private Methods

        private void EnsurePlaceholderCount()
        {
            if (placeholderCount.HasValue)
                return;

            // Just a shortcut: in all KGy SOFT Libraries format string resources end with 'Format'
            // Note: there are some resources that end with 'Format' and they are not format strings (eg. InfoMessage_SamePixelFormat)
            // but they contain no '{' so there will be no misinterpretation.
            if (!Key.EndsWith("Format", StringComparison.Ordinal))
            {
                placeholderCount = 0;
                return;
            }

            // -2: a placeholder is at least 3 chars long
            int len = OriginalText.Length - 2;
            int max = -1;
            for (int i = 0; i < len; i++)
            {
                if (OriginalText[i] != '{')
                    continue;

                if (OriginalText[i + 1] == '{')
                {
                    i += 1;
                    continue;
                }

                int posEnd = OriginalText.IndexOf('}', i + 2);
                Debug.Assert(posEnd > 1, "Valid original formats are expected");
                int posPadding = OriginalText.IndexOf(',', i + 2);
                if (posPadding < i || posPadding > posEnd)
                    posPadding = posEnd;
                int posFormat = OriginalText.IndexOf(':', i + 2);
                if (posFormat < i || posFormat > posEnd)
                    posFormat = posEnd;

                int indexLen = Math.Min(posEnd, Math.Min(posPadding, posFormat)) - 1 - i;
                int index = Int32.Parse(OriginalText.Substring(i + 1, indexLen), NumberStyles.None, CultureInfo.InvariantCulture);
                if (max < index)
                    max = index;
                i = posEnd;
            }

            placeholderCount = max + 1;
        }

        #endregion

        #endregion
    }
}