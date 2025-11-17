#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CustomBitmapSerializationInfoBase.cs
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
using KGySoft.Reflection;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Serialization
{
    /// <summary>
    /// The base class for custom debugger visualizers to provide binary serialization for custom bitmaps.
    /// </summary>
    public abstract class CustomBitmapSerializationInfoBase : IDisposable
    {
        #region Properties

        #region Public Properties
        
        /// <summary>
        /// Gets the <see cref="CustomBitmapInfo"/> containing information about the bitmap to debug.
        /// </summary>
        public CustomBitmapInfo? BitmapInfo { get; protected set; }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets or sets an optional backing object that represents the actual bitmap.
        /// Specify it to prevent the actual object from being garbage collected
        /// so the possibly unmanaged bitmap data of <see cref="BitmapInfo"/> will not be released as long as this instance is in use.
        /// </summary>
        protected object? BackingObject { get; set; }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="CustomBitmapSerializationInfoBase"/> class for serialization.
        /// <see cref="BitmapInfo"/> and <see cref="BackingObject"/> should be set from the derived constructor.
        /// </summary>
        protected CustomBitmapSerializationInfoBase()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CustomBitmapSerializationInfoBase"/> class for deserialization.
        /// </summary>
        /// <param name="reader"></param>
        protected CustomBitmapSerializationInfoBase(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader), PublicResources.ArgumentNull);
            ReadFrom(reader);
        }

        #endregion

        #region Methods

        #region Static Methods

        /// <summary>
        /// Returns the given <paramref name="bitmapData"/> as an <see cref="IReadableBitmapData"/> instance.
        /// Works even if the <paramref name="bitmapData"/> is an <see cref="IReadableBitmapData"/> with a different assembly identity.
        /// In normal circumstances, an assembly cannot be loaded with two different assembly identities in the same AppDomain, but it still can happen with debugger visualizers.
        /// </summary>
        /// <param name="bitmapData">The bitmap data to cast or convert.</param>
        /// <returns>An <see cref="IReadableBitmapData"/> instance.</returns>
        /// <exception cref="ArgumentException"><paramref name="bitmapData"/> is not an instance of <see cref="IReadableBitmapData"/>.</exception>
        protected static IReadableBitmapData AsBitmapData(object bitmapData)
        {
            if (bitmapData is IReadableBitmapData readableBitmapData)
                return readableBitmapData;

            // Assuming bitmap data of a different assembly identity: saving into a temp stream using the loaded assembly by reflection, and reloading it by the expected assembly identity.
            // Can occur if the debugged application uses a different version of KGySoft.Drawing.Core than this visualizer.
            using var stream = StreamExtensions.GetTempStream();
            try
            {
                Reflector.InvokeMethod(Reflector.ResolveType(bitmapData.GetType().Assembly, typeof(BitmapDataExtensions).FullName!)!, nameof(BitmapDataExtensions.Save), bitmapData, stream);
            }
            catch (Exception e)
            {
                throw new ArgumentException(PublicResources.NotAnInstanceOfType(typeof(IReadableBitmapData)), nameof(bitmapData), e);
            }

            stream.Position = 0L;
            return BitmapDataFactory.Load(stream);
        }

        #endregion

        #region Instance Methods

        #region Public Methods

        /// <summary>
        /// Releases this <see cref="CustomBitmapSerializationInfoBase"/> instance.
        /// </summary>
        public void Dispose() => Dispose(true);

        /// <summary>
        /// Serializes the value of the <see cref="BitmapInfo"/> property.
        /// </summary>
        /// <param name="writer">The writer to serialize <see cref="BitmapInfo"/> to.</param>
        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer), PublicResources.ArgumentNull);
            if (BitmapInfo == null)
                throw new InvalidOperationException(PublicResources.PropertyNull(nameof(BitmapInfo)));

            // 1. Type
            writer.Write(BitmapInfo.Type != null);
            if (BitmapInfo.Type != null)
                writer.Write(BitmapInfo.Type);

            // 2. Attributes
            writer.Write(BitmapInfo.ShowPixelSize);
            writer.Write(BitmapInfo.CustomAttributes.Count);
            foreach (KeyValuePair<string, string> attribute in BitmapInfo.CustomAttributes)
            {
                writer.Write(attribute.Key);
                writer.Write(attribute.Value);
            }

            // 3. CustomPalette
            writer.Write(BitmapInfo.CustomPalette != null);
            if (BitmapInfo.CustomPalette != null)
                new CustomPaletteSerializationInfo { PaletteInfo = BitmapInfo.CustomPalette }.Write(writer);

            // 4. BitmapData
            writer.Write(BitmapInfo.BitmapData != null);
            BitmapInfo.BitmapData?.Save(writer.BaseStream);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Releases the resource held by this <see cref="CustomBitmapSerializationInfoBase"/> instance.
        /// </summary>
        /// <param name="disposing"><see langword="true"/>, if this instance is being disposed; otherwise, <see langword="false"/>.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                BitmapInfo?.Dispose();
            BackingObject = null;
        }

        #endregion

        #region Private Methods

        private void ReadFrom(BinaryReader br)
        {
            BitmapInfo = new CustomBitmapInfo(true);

            // 1. Type
            if (br.ReadBoolean())
                BitmapInfo.Type = br.ReadString();

            // 2. Attributes
            BitmapInfo.ShowPixelSize = br.ReadBoolean();
            int count = br.ReadInt32();
            for (int i = 0; i < count; i++)
                BitmapInfo.CustomAttributes[br.ReadString()] = br.ReadString();

            // 3. CustomPalette
            if (br.ReadBoolean())
                BitmapInfo.CustomPalette = new CustomPaletteSerializationInfo(br).PaletteInfo;

            // 4. BitmapData
            if (br.ReadBoolean())
                BitmapInfo.BitmapData = BitmapDataFactory.Load(br.BaseStream);
        }
        
        #endregion

        #endregion

        #endregion
    }
}
