#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ReadableBitmapDataSerializationInfo.cs
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

using System.IO;

using KGySoft.CoreLibraries;
using KGySoft.Drawing.DebuggerVisualizers.Serialization;
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Core.Serialization
{
    internal sealed class ReadableBitmapDataSerializationInfo : CustomBitmapSerializationInfoBase
    {
        #region Constructors
        
        internal ReadableBitmapDataSerializationInfo(IReadableBitmapData bitmapData)
        {
            BitmapInfo = new CustomBitmapInfo(false)
            {
                ShowPixelSize = true,
                Type = bitmapData.GetType().GetName(TypeNameKind.ShortName),
                BitmapData = bitmapData,
                CustomAttributes =
                {
                    { nameof(bitmapData.PixelFormat), $"{bitmapData.PixelFormat}" },
                    { nameof(bitmapData.BackColor), $"{bitmapData.BackColor}" },
                    { nameof(bitmapData.AlphaThreshold), $"{bitmapData.AlphaThreshold}" },
                    { nameof(bitmapData.WorkingColorSpace), $"{bitmapData.WorkingColorSpace}" },
                }
            };

            if (bitmapData.Palette != null)
                BitmapInfo.CustomAttributes[$"{nameof(bitmapData.Palette)}.{nameof(bitmapData.Palette.Count)}"] = $"{bitmapData.Palette.Count}";
        }

        internal ReadableBitmapDataSerializationInfo(BinaryReader reader)
            : base(reader)
        {
        }

        #endregion
    }
}