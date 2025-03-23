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

using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using KGySoft.Drawing.DebuggerVisualizers.Wpf;
using KGySoft.Drawing.DebuggerVisualizers.Wpf.Serialization;

#endregion

#region Assembly Attributes

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("KGySoft.Drawing.DebuggerVisualizers")]
[assembly: AssemblyDescription("KGy SOFT Drawing Debugger Visualizers for WPF")]
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

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("e4eba402-1c34-46c3-a9ca-b07b87a010d0")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("4.0.0")]
[assembly: AssemblyFileVersion("4.0.0")]
[assembly: AssemblyInformationalVersion("4.0.0")]

// ImageSource
[assembly: DebuggerVisualizer(typeof(ImageSourceDebuggerVisualizer), typeof(ImageSourceSerializer),
    Target = typeof(ImageSource),
    Description = "KGy SOFT ImageSource Debugger Visualizer")]

// Color
[assembly: DebuggerVisualizer(typeof(ColorDebuggerVisualizer), typeof(ColorSerializer),
    Target = typeof(Color),
    Description = "KGy SOFT Color Debugger Visualizer")]

// BitmapPalette
[assembly: DebuggerVisualizer(typeof(BitmapPaletteDebuggerVisualizer), typeof(BitmapPaletteSerializer),
    Target = typeof(BitmapPalette),
    Description = "KGy SOFT BitmapPalette Debugger Visualizer")]

#endregion
