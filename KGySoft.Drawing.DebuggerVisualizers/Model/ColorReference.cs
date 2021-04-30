#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorReference.cs
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
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Security;

using KGySoft.Drawing.DebuggerVisualizers.Serialization;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Model
{
    [Serializable]
    internal sealed class ColorReference : IObjectReference
    {
        #region Fields

        private readonly byte[] rawData;

        #endregion

        #region Constructors

        internal ColorReference(Color color)
        {
            using (var ms = new MemoryStream())
            {
                SerializationHelper.SerializeColor(color, ms);
                rawData = ms.ToArray();
            }
        }

        #endregion

        #region Methods

        [SecurityCritical]
        public object GetRealObject(StreamingContext context)
        {
            using var ms = new MemoryStream(rawData);
            return SerializationHelper.DeserializeColor(ms);
        }

        #endregion
    }
}
