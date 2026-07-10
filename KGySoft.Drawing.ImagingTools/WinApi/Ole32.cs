#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Ole32.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2026 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

#endregion

namespace KGySoft.Drawing.ImagingTools.WinApi
{
    internal static class Ole32
    {
        #region Methods

        /// <summary>
        /// Frees the specified storage medium.
        /// </summary>
        /// <remarks>
        /// <para>The ReleaseStgMedium function calls the appropriate method or function to release the specified storage medium. Use this function during data transfer operations where storage medium structures are parameters, such as IDataObject::GetData or IDataObject::SetData. In addition to identifying the type of the storage medium, this structure specifies the appropriate Release method for releasing the storage medium when it is no longer needed.</para>
        /// <para>It is common to pass a STGMEDIUM from one body of code to another, such as in IDataObject::GetData, in which the one called can allocate a medium and return it to the caller. ReleaseStgMedium permits flexibility in whether the receiving body of code owns the medium, or whether the original provider of the medium still owns it, in which case the receiving code needs to inform the provider that it can free the medium.</para>
        /// <para>When the original provider of the medium is responsible for freeing the medium, the provider calls ReleaseStgMedium, specifying the medium and the appropriate IUnknown pointer as the punkForRelease structure member. Depending on the type of storage medium being freed, one of the following actions is taken, followed by a call to the IUnknown::Release method on the specified IUnknown pointer.</para>
        /// </remarks>
        [DllImport("ole32.dll")]
        internal static extern void ReleaseStgMedium(ref STGMEDIUM medium);

        #endregion
    }
}
