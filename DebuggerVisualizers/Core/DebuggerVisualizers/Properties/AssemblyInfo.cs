﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AssemblyInfo.cs
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

using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;

using KGySoft.Drawing.DebuggerVisualizers.Core.DebuggerVisualizers;
using KGySoft.Drawing.DebuggerVisualizers.Core.Serialization;
using KGySoft.Drawing.Imaging;

#endregion

#region Assembly Attributes

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("KGySoft.Drawing.DebuggerVisualizers.Core")]
[assembly: AssemblyDescription("KGy SOFT Drawing Core Debugger Visualizers")]
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
[assembly: Guid("fed0273c-6328-4cff-a1b0-2d115dca686e")]

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

// BitmapData
[assembly: DebuggerVisualizer(typeof(ReadableBitmapDataDebuggerVisualizer), typeof(ReadableBitmapDataSerializer),
    TargetTypeName = "KGySoft.Drawing.Imaging.BitmapDataBase, KGySoft.Drawing.Core",
    Description = "KGy SOFT BitmapDataBase Debugger Visualizer")]

// Palette
[assembly: DebuggerVisualizer(typeof(PaletteDebuggerVisualizer), typeof(PaletteSerializer),
    Target = typeof(Palette),
    Description = "KGy SOFT Palette Debugger Visualizer")]

// Color32
[assembly: DebuggerVisualizer(typeof(ColorDebuggerVisualizer), typeof(Color32Serializer),
    Target = typeof(Color32),
    Description = "KGy SOFT Color32 Debugger Visualizer")]

// PColor32
[assembly: DebuggerVisualizer(typeof(ColorDebuggerVisualizer), typeof(PColor32Serializer),
    Target = typeof(PColor32),
    Description = "KGy SOFT PColor32 Debugger Visualizer")]

// Color64
[assembly: DebuggerVisualizer(typeof(ColorDebuggerVisualizer), typeof(Color64Serializer),
    Target = typeof(Color64),
    Description = "KGy SOFT Color64 Debugger Visualizer")]

// PColor64
[assembly: DebuggerVisualizer(typeof(ColorDebuggerVisualizer), typeof(PColor64Serializer),
    Target = typeof(PColor64),
    Description = "KGy SOFT PColor64 Debugger Visualizer")]

// ColorF
[assembly: DebuggerVisualizer(typeof(ColorDebuggerVisualizer), typeof(ColorFSerializer),
    Target = typeof(ColorF),
    Description = "KGy SOFT ColorF Debugger Visualizer")]

// PColorF
[assembly: DebuggerVisualizer(typeof(ColorDebuggerVisualizer), typeof(PColorFSerializer),
    Target = typeof(PColorF),
    Description = "KGy SOFT PColorF Debugger Visualizer")]

#endregion
