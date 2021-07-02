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
#if NETFRAMEWORK
using System.Security;
using KGySoft.Drawing.ImagingTools.WinApi; 
#endif

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

#if NETFRAMEWORK
        private static long? maxMemoryForGC; 
#endif

        #endregion

        #region Properties

        private static long MaxMemoryForGC
#if NETFRAMEWORK
        {
            [SecuritySafeCritical]
            get
            {
                if (maxMemoryForGC == null)
                {
                    maxMemoryForGC = Math.Min(
                        IntPtr.Size == 4 ? 1_600_000_000 : Int64.MaxValue,
                        OSUtils.IsWindows ? Kernel32.GetTotalMemory() : Int64.MaxValue);
                }

                return maxMemoryForGC.Value;
            }
        }
#else
            => GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
#endif

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

            var maxMem = MaxMemoryForGC;
            if (maxMem == Int64.MaxValue)
                return true;

            // Using the total physical available memory (or 1.6GB on 32-bit systems, whichever is smaller) to determine free memory.
            // Virtual memory is ignored even if can be used to avoid slowing down the system very much.
            if (maxMem - GC.GetTotalMemory(false) > arraySize)
                return true;

            return maxMem - GC.GetTotalMemory(true) - minFreeMemory > arraySize;
        }

        #endregion
    }
}
