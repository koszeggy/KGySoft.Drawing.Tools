using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

using KGySoft.DebuggerVisualizers.VS2012;

using Microsoft.VisualStudio.DebuggerVisualizers;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Debugger Visualizers VS2012")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("KGy SOFT")]
[assembly: AssemblyProduct("KGy SOFT Debugger Visualizers VS2012")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("3f9bafa2-e9ee-4571-bf35-8bda7df08e99")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: AllowPartiallyTrustedCallers]

// Image
[assembly: DebuggerVisualizer(typeof(ImageDebuggerVisualizer), typeof(ImageSerializer),
    Target = typeof(Image),
    Description = "KGy Soft Image Debugger Visualizer")]

// Bitmap
[assembly: DebuggerVisualizer(typeof(BitmapDebuggerVisualizer), typeof(ImageSerializer),
    Target = typeof(Bitmap),
    Description = "KGy Soft Bitmap Debugger Visualizer")]

// Metafile
[assembly: DebuggerVisualizer(typeof(MetafileDebuggerVisualizer), typeof(ImageSerializer),
    Target = typeof(Metafile),
    Description = "KGy Soft Metafile Debugger Visualizer")]

// Icon
[assembly: DebuggerVisualizer(typeof(IconDebuggerVisualizer), typeof(ImageSerializer),
    Target = typeof(Icon),
    Description = "KGy Soft Icon Debugger Visualizer")]

// Graphics
[assembly: DebuggerVisualizer(typeof(GraphicsDebuggerVisualizer), typeof(GraphicsSerializer),
    Target = typeof(Graphics),
    Description = "KGy Soft Graphics Debugger Visualizer")]

// BitmapData
[assembly: DebuggerVisualizer(typeof(BitmapDataDebuggerVisualizer), typeof(BitmapDataSerializer),
    Target = typeof(BitmapData),
    Description = "KGy Soft BitmapData Debugger Visualizer")]

//// Color list. Note: PaletteDebuggerVisualizer and PaletteSerializer supports any color list and any color count,
//// though VS visualizers do not support interfaces and arrays. In case of a List<> visualizer, it can be re-used for Color element type
//[assembly: DebuggerVisualizer(typeof(PaletteDebuggerVisualizer), typeof(PaletteSerializer),
//    Target = typeof(IList<Color>),
//    Description = "KGy Soft Color List Debugger Visualizer")]

// Color palette (serialized by AnySerialier, because ColorPalette is not serializable)
[assembly: DebuggerVisualizer(typeof(PaletteDebuggerVisualizer), typeof(AnySerializer),
    Target = typeof(ColorPalette),
    Description = "KGy Soft Color Palette Debugger Visualizer")]

// Color (regular serialization)
[assembly: DebuggerVisualizer(typeof(ColorDebuggerVisualizer), typeof(VisualizerObjectSource),
    Target = typeof(Color),
    Description = "KGy Soft Color Debugger Visualizer")]
