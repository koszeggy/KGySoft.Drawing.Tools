#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorPaletteReference.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2020 - All Rights Reserved
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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;

using KGySoft.Drawing.DebuggerVisualizers.Serialization;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Model
{
    [Serializable]
    internal sealed class ColorPaletteReference : IObjectReference
    {
        #region Fields

        private readonly byte[] rawData;

        #endregion

        #region Constructors

        internal ColorPaletteReference(ColorPalette palette)
        {
            if (palette == null)
                return;

            using (var ms = new MemoryStream())
            {
                SerializationHelper.SerializeColorPalette(palette, ms);
                rawData = ms.ToArray();
            }
        }

        #endregion

        #region Methods

        [SecurityCritical]
        public object GetRealObject(StreamingContext context)
        {
            if (rawData == null)
                return null;

            using MemoryStream ms = new MemoryStream(rawData);
            return SerializationHelper.DeserializeColorPalette(ms);
        }

        #endregion
    }
}
