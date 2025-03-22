#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Color32Serializer.cs
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

using System.IO;

using KGySoft.Drawing.DebuggerVisualizers.Serialization;
using KGySoft.Drawing.Imaging;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Core.Serialization
{
    internal sealed class Color32Serializer : SerializerBase
    {
        #region Methods

        public override void GetData(object target, Stream outgoingData) => SerializationHelper.SerializeCustomColorInfo((Color32)target, outgoingData);

        #endregion
    }
}
