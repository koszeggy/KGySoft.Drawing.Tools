#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: MemoryHelper.cs
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

using KGySoft.Drawing.ImagingTools.WinApi;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal static class MemoryHelper
    {
        #region Constants

        private const int maxArrayLength = 0x7FEF_FFFF;
        private const int minFreeMemory = 1_048_576;

        #endregion

        #region Fields

        private static long? maxMemoryForGC;

        #endregion

        #region Properties

        private static long MaxMemoryForGC => maxMemoryForGC ??= Math.Min(IntPtr.Size == 4 ? 1_600_000_000 : Int64.MaxValue, Kernel32.GetTotalMemory());

        #endregion

        #region Methods

        /// <summary>
        /// Gets an educated guess whether an array of specified size can be allocated.
        /// It does not consider virtual memory and does not guarantee that out of memory can be avoided (especially in pre .NET 4.0 versions).
        /// We also ignore gcAllowVeryLargeObjects.
        /// </summary>
        internal static bool CanAllocate(long arraySize)
        {
            if (arraySize > maxArrayLength)
                return false;

            // TODO: in .NET Core 3.0 and above use GCMemoryInfo.TotalAvailableMemoryBytes for max (that should not be cached, it can change)
            // Using the total physical available memory (or 1.6GB on 32-bit systems, whichever is smaller) to determine free memory.
            // Virtual memory is ignored even if can be used to avoid slowing down the system very much.
            if (MaxMemoryForGC - GC.GetTotalMemory(false) > arraySize)
                return true;

            // Here GC is enforced
            return MaxMemoryForGC - GC.GetTotalMemory(true) - minFreeMemory > arraySize;
        }

        #endregion
    }
}
