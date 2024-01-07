#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CustomColorSerializationInfoBase.cs
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

using System;
using System.Collections.Generic;
using System.IO;

using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Serialization
{
    /// <summary>
    /// The base class for custom debugger visualizers to serialize custom colors.
    /// </summary>
    public abstract class CustomColorSerializationInfoBase : IDisposable
    {
        #region Properties
        
        #region Public Properties

        /// <summary>
        /// Gets the <see cref="CustomColorInfo"/> containing information about the color to debug.
        /// </summary>
        public CustomColorInfo? ColorInfo { get; protected set; }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets or sets an optional backing object that represents the actual color.
        /// Specify it to prevent the actual object from being garbage collected
        /// so the possibly native color data of <see cref="ColorInfo"/> will not be released as long as this instance is in use.
        /// </summary>
        protected object? BackingObject { get; set; }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="CustomColorSerializationInfoBase"/> class for serialization.
        /// <see cref="ColorInfo"/> and <see cref="BackingObject"/> should be set from the derived constructor.
        /// </summary>
        protected CustomColorSerializationInfoBase()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CustomColorSerializationInfoBase"/> class for deserialization.
        /// </summary>
        /// <param name="reader"></param>
        protected CustomColorSerializationInfoBase(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader), PublicResources.ArgumentNull);
            ReadFrom(reader);
        }

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Releases this <see cref="CustomBitmapSerializationInfoBase"/> instance.
        /// </summary>
        public void Dispose() => Dispose(true);

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

        #region Protected Methods

        /// <summary>
        /// Releases the resource held by this <see cref="CustomColorSerializationInfoBase"/> instance.
        /// </summary>
        /// <param name="disposing"><see langword="true"/>, if this instance is being disposed; otherwise, <see langword="false"/>.</param>
        protected virtual void Dispose(bool disposing) => BackingObject = null;

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
