#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CustomPaletteSerializationInfo.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2025 - All Rights Reserved
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
using System.IO;

using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Serialization
{
    /// <summary>
    /// A class for custom debugger visualizers to provide binary serialization for custom palettes.
    /// </summary>
    public class CustomPaletteSerializationInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="CustomPaletteInfo"/> containing information about the palette to debug.
        /// </summary>
        public CustomPaletteInfo? PaletteInfo { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="CustomPaletteSerializationInfo"/> class for serialization.
        /// <see cref="PaletteInfo"/> should be set by the caller.
        /// </summary>
        public CustomPaletteSerializationInfo()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CustomPaletteSerializationInfo"/> class for deserialization.
        /// </summary>
        /// <param name="reader">The reader that deserializes the <see cref="PaletteInfo"/> property.</param>
        public CustomPaletteSerializationInfo(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader), PublicResources.ArgumentNull);
            ReadFrom(reader);
        }

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Serializes the value of the <see cref="PaletteInfo"/> property.
        /// </summary>
        /// <param name="writer">The writer to serialize <see cref="PaletteInfo"/> to.</param>
        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer), PublicResources.ArgumentNull);
            if (PaletteInfo == null)
                throw new InvalidOperationException(PublicResources.PropertyNull(nameof(PaletteInfo)));

            // 1. Type
            writer.Write(PaletteInfo.Type != null);
            if (PaletteInfo.Type != null)
                writer.Write(PaletteInfo.Type);

            // 2. EntryType
            writer.Write(PaletteInfo.EntryType != null);
            if (PaletteInfo.EntryType != null)
                writer.Write(PaletteInfo.EntryType);

            // 3. Entries
            var entryInfo = new CustomColorSerializationInfo();
            writer.Write(PaletteInfo.Entries.Count);
            foreach (CustomColorInfo colorInfo in PaletteInfo.Entries)
            {
                entryInfo.ColorInfo = colorInfo;
                entryInfo.Write(writer);
            }

            // 4. Attributes
            writer.Write(PaletteInfo.CustomAttributes.Count);
            foreach (KeyValuePair<string, string> attribute in PaletteInfo.CustomAttributes)
            {
                writer.Write(attribute.Key);
                writer.Write(attribute.Value);
            }
        }

        #endregion

        #region Private Methods

        private void ReadFrom(BinaryReader br)
        {
            PaletteInfo = new CustomPaletteInfo();

            // 1. Type
            if (br.ReadBoolean())
                PaletteInfo.Type = br.ReadString();

            // 2. EntryType
            if (br.ReadBoolean())
                PaletteInfo.EntryType = br.ReadString();

            // 3. Entries
            int count = br.ReadInt32();
            for (int i = 0; i < count; i++)
                PaletteInfo.Entries.Add(new CustomColorSerializationInfo(br).ColorInfo!);

            // 4. Attributes
            count = br.ReadInt32();
            for (int i = 0; i < count; i++)
                PaletteInfo.CustomAttributes[br.ReadString()] = br.ReadString();
        }

        #endregion

        #endregion
    }
}