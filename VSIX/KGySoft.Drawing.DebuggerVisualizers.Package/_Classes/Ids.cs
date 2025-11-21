#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Ids.cs
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

#if !VS2022_OR_GREATER
using System;
#endif

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Package
{
    internal static class Ids
    {
        #region Constants

#if !VS2022_OR_GREATER
        internal const string PackageGuidString = "fd42f5a8-4449-4c07-8b60-b6bd58b67118";
        internal const string ResourceDetails = "112";
        internal const int IconResourceId = 400;
        internal const int ExecuteImagingToolsCommandId = 0x0100;
        internal const int ManageDebuggerVisualizerInstallationsCommandId = 0x0101;
#endif
        internal const string ResourceTitle = "110";
        internal const string Version = "5.1.0"; // Note: in .vsixmanifest it should be adjusted manually
        internal const string PackageVersion = "5.1.0"; // It can also be an informational version

        #endregion

        #region Fields

#if !VS2022_OR_GREATER
        internal static readonly Guid CommandSet = new Guid("63728687-1b7c-4508-9efd-bd8c81f87b71");
#endif

        #endregion
    }
}
