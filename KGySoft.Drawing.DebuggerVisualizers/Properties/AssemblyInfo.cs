using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using KGySoft.Drawing.DebuggerVisualizers;
using KGySoft.Drawing.DebuggerVisualizers.Serializers;

using Microsoft.VisualStudio.DebuggerVisualizers;

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
[assembly: AssemblyVersion("2.0.0.0")]
[assembly: AssemblyFileVersion("2.0.0.0")]

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
[assembly: DebuggerVisualizer(typeof(IconDebuggerVisualizer), typeof(ImageSerializer),
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

//// Color list. Note: PaletteDebuggerVisualizer and PaletteSerializer supports any color list and any color count,
//// though VS visualizers do not support interfaces and arrays. In case of a List<> visualizer, it can be re-used for Color element type
//[assembly: DebuggerVisualizer(typeof(PaletteDebuggerVisualizer), typeof(PaletteSerializer),
//    Target = typeof(IList<Color>),
//    Description = "KGy SOFT Color List Debugger Visualizer")]

// Color palette (serialized by AnySerialier, because ColorPalette is not serializable)
[assembly: DebuggerVisualizer(typeof(PaletteDebuggerVisualizer), typeof(AnySerializer),
    Target = typeof(ColorPalette),
    Description = "KGy SOFT Color Palette Debugger Visualizer")]

// Color (regular serialization)
[assembly: DebuggerVisualizer(typeof(ColorDebuggerVisualizer), typeof(VisualizerObjectSource),
    Target = typeof(Color),
    Description = "KGy SOFT Color Debugger Visualizer")]

[assembly: InternalsVisibleTo("KGySoft.Drawing.DebuggerVisualizers.Test, PublicKey=00240000048000009400000006020000002400005253413100040000010001003928BADFAA8C02789566AB7AC64A59DCDE30B798589A68EF92CBB04C9DED3FCBFE41F644D424DCF82F8A13F9148D45EE15785450318388E01AA8C4CF645E81C772E39DCA0D14B33CF48167B70F5C34A0E7B763141ED3AFDDAD0373D9FCD2E153E78D201C5C4EB61DBBD586EC6291EABFBE11879865C3776088605FA8820387C2")]
