using System.Reflection;
using System.Runtime.InteropServices;
using KGySoft.Drawing.DebuggerVisualizers.Package;

[assembly: AssemblyTitle("KGySoft.Drawing.DebuggerVisualizers.Package")]
[assembly: AssemblyDescription("VSIX package for KGy SOFT Drawing Debugger Visualizers")]
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
