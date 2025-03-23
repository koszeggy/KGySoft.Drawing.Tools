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

using KGySoft.Drawing.DebuggerVisualizers.SkiaSharp;
using KGySoft.Drawing.DebuggerVisualizers.SkiaSharp.Serialization;

using SkiaSharp;

#endregion

#region Assembly Attributes

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("KGySoft.Drawing.DebuggerVisualizers")]
[assembly: AssemblyDescription("KGy SOFT Drawing Debugger Visualizers for SkiaSharp")]
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
[assembly: Guid("f70b8baa-5987-411f-91fb-c31b316ce3aa")]

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

// SKBitmap
[assembly: DebuggerVisualizer(typeof(SkiaCustomBitmapDebuggerVisualizer), typeof(SkiaBitmapSerializer),
    Target = typeof(SKBitmap),
    Description = "KGy SOFT SKBitmap Debugger Visualizer")]

// SKPixmap
[assembly: DebuggerVisualizer(typeof(SkiaCustomBitmapDebuggerVisualizer), typeof(SkiaBitmapSerializer),
    Target = typeof(SKPixmap),
    Description = "KGy SOFT SKPixmap Debugger Visualizer")]

// SKImage
[assembly: DebuggerVisualizer(typeof(SkiaCustomBitmapDebuggerVisualizer), typeof(SkiaBitmapSerializer),
    Target = typeof(SKImage),
    Description = "KGy SOFT SKImage Debugger Visualizer")]

// SKSurface
[assembly: DebuggerVisualizer(typeof(SkiaCustomBitmapDebuggerVisualizer), typeof(SkiaBitmapSerializer),
    Target = typeof(SKSurface),
    Description = "KGy SOFT SKSurface Debugger Visualizer")]

// SKColor
[assembly: DebuggerVisualizer(typeof(SkiaCustomColorDebuggerVisualizer), typeof(SkiaColorSerializer),
    Target = typeof(SKColor),
    Description = "KGy SOFT SKColor Debugger Visualizer")]

// SKPMColor
[assembly: DebuggerVisualizer(typeof(SkiaCustomColorDebuggerVisualizer), typeof(SkiaColorSerializer),
    Target = typeof(SKPMColor),
    Description = "KGy SOFT SKPMColor Debugger Visualizer")]

// SKColorF
[assembly: DebuggerVisualizer(typeof(SkiaCustomColorDebuggerVisualizer), typeof(SkiaColorSerializer),
    Target = typeof(SKColorF),
    Description = "KGy SOFT SKColorF Debugger Visualizer")]

#endregion