using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;

using KGySoft.Drawing.DebuggerVisualizers.GdiPlus;
using KGySoft.Drawing.DebuggerVisualizers.GdiPlus.Serialization;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("KGySoft.Drawing.DebuggerVisualizers")]
[assembly: AssemblyDescription("KGy SOFT Drawing Debugger Visualizers")]
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
[assembly: AssemblyVersion("3.0.0")]
[assembly: AssemblyFileVersion("3.0.0")]
[assembly: AssemblyInformationalVersion("3.0.0")]

// Image
[assembly: DebuggerVisualizer(typeof(ImageDebuggerVisualizer), typeof(ImageSerializer),
    Target = typeof(Image),
    Description = "KGy SOFT Image Debugger Visualizer")]

// Bitmap
[assembly: DebuggerVisualizer(typeof(BitmapDebuggerVisualizer), typeof(ImageSerializer),
    Target = typeof(Bitmap),
    Description = "KGy SOFT Bitmap Debugger Visualizer")]

// Metafile
[assembly: DebuggerVisualizer(typeof(MetafileDebuggerVisualizer), typeof(ImageSerializer),
    Target = typeof(Metafile),
    Description = "KGy SOFT Metafile Debugger Visualizer")]

// Icon
[assembly: DebuggerVisualizer(typeof(IconDebuggerVisualizer), typeof(IconSerializer),
    Target = typeof(Icon),
    Description = "KGy SOFT Icon Debugger Visualizer")]

// Graphics
[assembly: DebuggerVisualizer(typeof(GraphicsDebuggerVisualizer), typeof(GraphicsSerializer),
    Target = typeof(Graphics),
    Description = "KGy SOFT Graphics Debugger Visualizer")]

// BitmapData
[assembly: DebuggerVisualizer(typeof(BitmapDataDebuggerVisualizer), typeof(BitmapDataSerializer),
    Target = typeof(BitmapData),
    Description = "KGy SOFT BitmapData Debugger Visualizer")]

// Color palette
[assembly: DebuggerVisualizer(typeof(ColorPaletteDebuggerVisualizer), typeof(ColorPaletteSerializer),
    Target = typeof(ColorPalette),
    Description = "KGy SOFT Color Palette Debugger Visualizer")]

// Color
[assembly: DebuggerVisualizer(typeof(ColorDebuggerVisualizer), typeof(ColorSerializer),
    Target = typeof(Color),
    Description = "KGy SOFT Color Debugger Visualizer")]
