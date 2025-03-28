#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AssemblyInfo.cs
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

using System.Reflection;
using System.Runtime.InteropServices;

using KGySoft.Drawing.DebuggerVisualizers.Package;

#endregion

#region Assembly Attributes

[assembly: AssemblyTitle("KGySoft.Drawing.DebuggerVisualizers.Package")]
[assembly: AssemblyDescription("VSIX package for KGy SOFT Image Debugger Visualizers")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("KGy SOFT")]
[assembly: AssemblyProduct("KGy SOFT Debugger Visualizers")]
[assembly: AssemblyCopyright("Copyright © KGy SOFT. All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: AssemblyVersion(Ids.Version)]
[assembly: AssemblyFileVersion(Ids.Version)]
[assembly: AssemblyInformationalVersion(Ids.PackageVersion)]

#endregion
