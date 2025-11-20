#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ReadableBitmapDataSerializationInfo.cs
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

using KGySoft.CoreLibraries;
using KGySoft.Drawing.DebuggerVisualizers.Serialization;
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Reflection;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Core.Serialization
{
    internal sealed class ReadableBitmapDataSerializationInfo : CustomBitmapSerializationInfoBase
    {
        #region Constructors
        
        internal ReadableBitmapDataSerializationInfo(object target)
        {
            IReadableBitmapData bitmapData = AsBitmapData(target);
            bool disposeBitmapData = !ReferenceEquals(bitmapData, target);

            BitmapInfo = new CustomBitmapInfo(disposeBitmapData)
            {
                ShowPixelSize = true,
                Type = target.GetType().GetName(TypeNameKind.ShortName),
                BitmapData = bitmapData,
                CustomPalette = PaletteSerializationInfo.GetPaletteInfo(bitmapData.Palette),
                CustomAttributes =
                {
                    { nameof(bitmapData.Width), $"{bitmapData.Width}" },
                    { nameof(bitmapData.Height), $"{bitmapData.Height}" },
                    { nameof(bitmapData.PixelFormat), $"{(target == bitmapData ? bitmapData.PixelFormat : Reflector.GetProperty(target, nameof(bitmapData.PixelFormat)))}" },
                    { nameof(bitmapData.BackColor), $"{bitmapData.BackColor}" },
                    { nameof(bitmapData.AlphaThreshold), $"{bitmapData.AlphaThreshold}" },
                    { nameof(bitmapData.WorkingColorSpace), $"{(target == bitmapData ? bitmapData.WorkingColorSpace : Reflector.GetProperty(target, nameof(bitmapData.WorkingColorSpace)))}" },
                }
            };

            if (bitmapData.Palette != null)
                BitmapInfo.CustomAttributes[$"{nameof(bitmapData.Palette)}.{nameof(bitmapData.Palette.Count)}"] = $"{bitmapData.Palette.Count}";

#if DEBUG
            if (!Equals(target.GetType().Assembly, typeof(IReadableBitmapData).Assembly))
                BitmapInfo?.CustomAttributes["KGySoft.Drawing.Core version mismatch"] = $"{target.GetType().Assembly} vs. {typeof(IReadableBitmapData).Assembly}";
#endif
        }

        internal ReadableBitmapDataSerializationInfo(BinaryReader reader)
            : base(reader)
        {
        }

        #endregion
    }
}