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
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;

using KGySoft.Drawing.DebuggerVisualizers.GdiPlus;
using KGySoft.Drawing.DebuggerVisualizers.GdiPlus.Serialization;

#endregion

#region Assembly Attributes

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("KGySoft.Drawing.DebuggerVisualizers.GdiPlus")]
[assembly: AssemblyDescription("KGy SOFT Drawing Debugger Visualizers for GDI+")]
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
[assembly: Guid("12c2586c-561e-4a57-b12b-f48f3d9c2122")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("5.0.0")]
[assembly: AssemblyFileVersion("5.0.0")]
[assembly: AssemblyInformationalVersion("5.0.0")]

// Image
[
    assembly: DebuggerVisualizer(typeof(ImageDebuggerVisualizer), typeof(ImageSerializer),
    Target = typeof(Image),
#if NET472_OR_GREATER
    Description = "KGy SOFT Image Debugger Visualizer (Classic)")
#else
    Description = "KGy SOFT Image Debugger Visualizer")
#endif
]

// Bitmap
[
    assembly: DebuggerVisualizer(typeof(BitmapDebuggerVisualizer), typeof(ImageSerializer),
    Target = typeof(Bitmap),
#if NET472_OR_GREATER
    Description = "KGy SOFT Bitmap Debugger Visualizer (Classic)")
#else
    Description = "KGy SOFT Bitmap Debugger Visualizer")
#endif
]

// Metafile
[
    assembly: DebuggerVisualizer(typeof(MetafileDebuggerVisualizer), typeof(ImageSerializer),
    Target = typeof(Metafile),
#if NET472_OR_GREATER
    Description = "KGy SOFT Metafile Debugger Visualizer (Classic)")
#else
    Description = "KGy SOFT Metafile Debugger Visualizer")
#endif
]

// Icon
[
    assembly: DebuggerVisualizer(typeof(IconDebuggerVisualizer), typeof(IconSerializer),
    Target = typeof(Icon),
#if NET472_OR_GREATER
    Description = "KGy SOFT Icon Debugger Visualizer (Classic)")
#else
    Description = "KGy SOFT Icon Debugger Visualizer")
#endif
]

// Graphics
[
    assembly: DebuggerVisualizer(typeof(GraphicsDebuggerVisualizer), typeof(GraphicsSerializer),
    Target = typeof(Graphics),
#if NET472_OR_GREATER
    Description = "KGy SOFT Graphics Debugger Visualizer (Classic)")
#else
    Description = "KGy SOFT Graphics Debugger Visualizer")
#endif
]

// BitmapData
[
    assembly: DebuggerVisualizer(typeof(BitmapDataDebuggerVisualizer), typeof(BitmapDataSerializer),
    Target = typeof(BitmapData),
#if NET472_OR_GREATER
    Description = "KGy SOFT BitmapData Debugger Visualizer (Classic)")
#else
    Description = "KGy SOFT BitmapData Debugger Visualizer")
#endif
]

// ColorPalette
[
    assembly: DebuggerVisualizer(typeof(ColorPaletteDebuggerVisualizer), typeof(ColorPaletteSerializer),
    Target = typeof(ColorPalette),
#if NET472_OR_GREATER
    Description = "KGy SOFT ColorPalette Debugger Visualizer (Classic)")
#else
    Description = "KGy SOFT ColorPalette Debugger Visualizer")
#endif
]

// Color
[
    assembly: DebuggerVisualizer(typeof(ColorDebuggerVisualizer), typeof(ColorSerializer),
    Target = typeof(Color),
#if NET472_OR_GREATER
    Description = "KGy SOFT Color Debugger Visualizer (Classic)")
#else
    Description = "KGy SOFT Color Debugger Visualizer")
#endif
]

#endregion
