﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Ids.cs
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

using System;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Package
{
    internal static class Ids
    {
        #region Constants

        internal const string PackageGuidString =
#if VS2022_OR_GREATER
            "9029031a-4b33-48a7-ae94-138c537ee202";
#else
            "fd42f5a8-4449-4c07-8b60-b6bd58b67118";
#endif
        internal const string ResourceTitle = "110";
        internal const string ResourceDetails = "112";
        internal const int IconResourceId = 400;
        internal const string Version = "4.0.0"; // Note: in .vsixmanifest it should be adjusted manually
        internal const string PackageVersion = "4.0.0"; // It can also be an informational version
        internal const int ExecuteImagingToolsCommandId = 0x0100;
        internal const int ManageDebuggerVisualizerInstallationsCommandId = 0x0101;

        #endregion

        #region Fields

        internal static readonly Guid CommandSet = new Guid("63728687-1b7c-4508-9efd-bd8c81f87b71");

        #endregion
    }
}
