#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CustomColorSerializationInfo.cs
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

using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Serialization
{
    /// <summary>
    /// A class for custom debugger visualizers to provide binary serialization for custom colors.
    /// </summary>
    public class CustomColorSerializationInfo
    {
        #region Properties

        /// <summary>
        /// Gets the <see cref="CustomColorInfo"/> containing information about the color to debug.
        /// </summary>
        public CustomColorInfo? ColorInfo { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="CustomColorSerializationInfo"/> class for serialization.
        /// <see cref="ColorInfo"/> should be set by the caller.
        /// </summary>
        public CustomColorSerializationInfo()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CustomColorSerializationInfo"/> class for deserialization.
        /// </summary>
        /// <param name="reader">The reader that deserializes the <see cref="ColorInfo"/> property.</param>
        public CustomColorSerializationInfo(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader), PublicResources.ArgumentNull);
            ReadFrom(reader);
        }

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Serializes the value of the <see cref="ColorInfo"/> property.
        /// </summary>
        /// <param name="writer">The writer to serialize <see cref="ColorInfo"/> to.</param>
        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer), PublicResources.ArgumentNull);
            if (ColorInfo == null)
                throw new InvalidOperationException(PublicResources.PropertyNull(nameof(ColorInfo)));

            // 1. Type
            writer.Write(ColorInfo.Type != null);
            if (ColorInfo.Type != null)
                writer.Write(ColorInfo.Type);

            // 2. Name
            writer.Write(ColorInfo.Name != null);
            if (ColorInfo.Name != null)
                writer.Write(ColorInfo.Name);

            // 3. DisplayColor
            writer.Write(ColorInfo.DisplayColor.ToArgb());

            // 4. Custom components
            writer.Write(ColorInfo.CustomColorComponents != null);
            if (ColorInfo.CustomColorComponents != null)
            {
                writer.Write((byte)ColorInfo.CustomColorComponents.Length);
                foreach (KeyValuePair<string, string> attribute in ColorInfo.CustomColorComponents)
                {
                    writer.Write(attribute.Key);
                    writer.Write(attribute.Value);
                }
            }

            // 5. Attributes
            writer.Write(ColorInfo.CustomAttributes.Count);
            foreach (KeyValuePair<string, string> attribute in ColorInfo.CustomAttributes)
            {
                writer.Write(attribute.Key);
                writer.Write(attribute.Value);
            }
        }

        #endregion

        #region Private Methods

        private void ReadFrom(BinaryReader br)
        {
            ColorInfo = new CustomColorInfo();

            // 1. Type
            if (br.ReadBoolean())
                ColorInfo.Type = br.ReadString();

            // 2. Name
            if (br.ReadBoolean())
                ColorInfo.Name = br.ReadString();

            // 3. DisplayColor
            ColorInfo.DisplayColor = Color32.FromArgb(br.ReadInt32());

            // 4. Custom components
            if (br.ReadBoolean())
            {
                ColorInfo.CustomColorComponents = new KeyValuePair<string, string>[br.ReadByte()];
                for (int i = 0; i < ColorInfo.CustomColorComponents.Length; i++)
                    ColorInfo.CustomColorComponents[i] = new KeyValuePair<string, string>(br.ReadString(), br.ReadString());
            }

            // 5. Attributes
            int count = br.ReadInt32();
            for (int i = 0; i < count; i++)
                ColorInfo.CustomAttributes[br.ReadString()] = br.ReadString();
        }

        #endregion

        #endregion
    }
}
