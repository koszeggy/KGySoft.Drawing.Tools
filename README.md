[![KGy SOFT .net](http://docs.kgysoft.net/drawing/icons/logo.png)](https://kgysoft.net)

# KGy SOFT Drawing Tools

KGy SOFT Drawing Tools repository contains [Debugger Visualizers](#debugger-visualizers) for several `System.Drawing` types such as `Bitmap`, `Metafile`, `Icon`, `BitmapData`, `Graphics`, etc. (see also below). The visualizers use [KGy SOFT Imaging Tools](#kgy-soft-imaging-tools) to display these types visually, which can be executed as a standalone application as well. Along with the [Debugger Visualizers Test Tool](#debugger-visualizers-test-tool) it can be considered also as a demonstration of the features of the [KGy SOFT Drawing Libraries](https://kgysoft.net/drawing).

[![Website](https://img.shields.io/website/https/kgysoft.net/corelibraries.svg)](https://kgysoft.net/drawing)
[![Drawing Libraries Repo](https://img.shields.io/github/repo-size/koszeggy/KGySoft.Drawing.svg?label=Drawing%20Libraries)](https://github.com/koszeggy/KGySoft.Drawing)

## Table of Contents
1. [KGy SOFT Imaging Tools](#kgy-soft-imaging-tools)
2. [Debugger Visualizers](#debugger-visualizers)
   - [Installing Debugger Visualizers](#installing-debugger-visualizers)
   - [Troubleshooting](#troubleshooting) 
3. [Download](#download)
4. [Release Notes](#release-notes)
5. [Debugger Visualizers Test Tool](#debugger-visualizers-test-tool)
6. [License](#license)


## KGy SOFT Imaging Tools
![KGySoft.Drawing.ImagingTools](https://kgysoft.net/images/ImagingTools.png)

The Imaging Tools application makes possible to load images and icons from file, provides detailed information about images and you can convert them to other formats.

## Debugger Visualizers

Imaging Tools is packed with several debugger visualizers for Visual Studio (compatible with all versions starting with Visual Studio 2008, and supports even .NET Core 2.1 and newer platform targets). When a type is debugged in Visual Studio and there is a debugger visualizer installed for that type, then a magnifier icon appears that you can click to open the visualizer.

![Debugger Visualizer Usage](https://kgysoft.net/images/DebuggerVisualizerUsage.png)

Either click the magnifier icon or choose a debugger visualizer from the drop down list (if more visualizers are applicable).

![Debugging Graphics](https://kgysoft.net/images/DebugGraphics.png)

The `KGySoft.Drawing.DebuggerVisualizers` assembly provides debugger visualizers for the following types:
- `System.Drawing.Image`: If executed for a non read-only variable or member of type `Image`, then the actual value can be replaced by any `Bitmap` or `Metafile`.
- `System.Drawing.Bitmap`: Supports multi-page, multi-resolution and animated `Bitmap` instances. The bitmap can be saved in many formats. In a non read-only context the bitmap can be replaced from file and its palette (for indexed bitmaps) can be edited.
- `System.Drawing.Imaging.Metafile`: Unlike many image debugger visualizers around the net, this one does not transform the metafile into a low-resolution PNG image because it is able to serialize the actual metafile content. The metafile can be saved into EMF/WMF formats. In a non read-only context the image can be replaced from file.
- `System.Drawing.Icon`: Supports compound and huge icons even in Windows XP. The icon can be saved, and in a non read-only context it can be replaced from file.
- `System.Drawing.Imaging.BitmapData`: Even for indexed data; however, the palette information cannot be retrieved.
- `System.Drawing.Graphics`: Supports both image and native window graphics.
- `System.Drawing.Imaging.ColorPalette`: In a non read-only context the colors can be edited.
- `System.Drawing.Color`: In a non read-only context the color can be replaced.

![Debugging Palette](https://kgysoft.net/images/DebugPalette.png)

### Installing Debugger Visualizers

#### By VSIX Installer

If you use Visual Studio 2013 or newer the simplest way is to download the [installer package](https://marketplace.visualstudio.com/items?itemName=KGySoft.drawing-debugger-visualizers) from the VisualStudio Marketplace. 

You can perform the install also from Visual Studio by the _Tools/Extensions and Updates..._ (Visual Studio 2019: _Extensions/Manage Extensions_) menu if you search for the "_KGy SOFT Drawing DebuggerVisualizers_" extension.

#### Manual Install

1. [Download](#download) the binaries and extract the .zip file to any folder.
2. Open the folder with the extracted content. You will find three folders there:
  - `NET35` contains the .NET 3.5 version. Compatible with all Visual Studio versions starting with Visual Studio 2008 (tested with versions 2008-2019).
  - `NET40` contains the .NET 4.0 version. You cannot use this one for Visual Studio 2008.
  - `NET45` contains the .NET 4.5 version. You cannot use this one for Windows XP and Visual Studio 2008/2010.
3. Execute `KGySoft.Drawing.ImagingTools.exe` from one of the folders listed above. Click the _Manage Debugger Visualizer Installations..._ button (the gear icon) on the toolbar.

> _Note:_ Starting with version 2.1.0 the debugger visualizers can be used also for .NET Core projects from Visual Studio 2019, even though no .NET Core binaries are used.

![Select Install](https://kgysoft.net/images/InstallSelectMenu.png)

4. In the drop down list you will see the identified Visual Studio versions in your Documents folder. You can select either one of them or the _&lt;Custom Path...&gt;_ menu item to install the visualizer debuggers into a custom folder.

![Select Visual Studio version](https://kgysoft.net/images/InstallSelectVSVersion.png)

5. Click on the _Install_ button. On success the status will display the installed version.

![Installation Complete](https://kgysoft.net/images/InstallComplete.png)

### Troubleshooting

If Visual Studio cannot load the visualizer or you have other debugger visualizer related problems check this table.

| **Issue** | **Solution** |
|-----------|--------------|
| The magnifier icon does not appear<img width="0px"/> | Debugger Visualizers are not installed for the Visual Studio version you use. Check the [installation steps](#installing-debugger-visualizers) above. Make sure you select the correct version from the list. |
| Could not load this custom viewer.<br/>![Could not load this custom viewer.](https://kgysoft.net/images/DebuggerVisualizerTrShCouldNotLoadViewer.png) | Open _Debug / Options / Debugging / General_ and make sure that both _Use Managed Compatibility Mode_ and _Use legacy C# and VB expression evaluators_ are unchecked.<br/>![Debugging Options.](https://kgysoft.net/images/DebuggerVisualizerTrShCouldNotLoadSettings.png) |
| Unable to load the custom visualizer (serializer could not be resolved).<br/>![Unable to load the custom visualizer (serializer could not be resolved).](https://kgysoft.net/images/DebuggerVisualizerTrShUnableToLoadSerializer.png) | The project you debug references an unmatching version of the `Microsoft.VisualStudio.DebuggerVisualizers` to your actual VisualStudio version. |
| Unable to load the custom visualizer (The UI-side visualizer type must derive from 'DialogDebuggerVisualizer').<br/>![Unable to load the custom visualizer (The UI-side visualizer type must derive from 'DialogDebuggerVisualizer').](https://kgysoft.net/images/DebuggerVisualizerTrShUnableToLoadMustDeriveFrom.png) | The `Microsoft.VisualStudio.DebuggerVisualizers.dll` has been copied to the debugger visualizers installation folder. Recent Visual Studio versions can handle if a debugger visualizer references an unmatching version of this assembly but only if this assembly is not deployed along with the visualizers. |
| Object is currently in use elsewhere.<br/>![Object is currently in use elsewhere.](https://kgysoft.net/images/DebuggerVisualizerTrShObjectIsInUse.png) | You try to debug a `Graphics` instance, whose Device Context is in use (the `GetHdc` method has been called previously). This `Graphics` instance cannot be accessed until the `ReleaseHdc` method is called. |
| Function evaluation timed out | On slower computers with a slower Visual Studio (with many add-ons, for example) it can happen that the visualizer loads too slowly for the first time. Just try to click the magnifier icon again, which usually solves the problem. Alternatively, you can try to set a `DWORD` value in the Registry under the `HKEY_CURRENT_USER\Software\Microsoft\VisualStudio\[version]\Debugger` key, called `LocalsTimeout`. The value represents milliseconds. |
| An unhandled exception of type 'System.NullReferenceException' was thrown by the custom visualizer component in the process being debugged.<br/>![An unhandled exception of type 'System.Exception' was thrown by the UI-side custom visualizer component.](https://kgysoft.net/images/DebuggerVisualizerTrShExceptionNullRef.png) | Occurs when you clear the debugged image in Visual Studio 2019, which does not support nullifying the debugged value. |
| An unhandled exception of type 'System.Exception' was thrown by the UI-side custom visualizer component.<br/>![An unhandled exception of type 'System.Exception' was thrown by the UI-side custom visualizer component.](https://kgysoft.net/images/DebuggerVisualizerTrShException.png) | Occurs when you clear the debugged image in Visual Studio 2017, which does not support nullifying the debugged value. |
| Could not evaluate expression.<br/>![Could not evaluate expression.](https://kgysoft.net/images/DebuggerVisualizerTrShCouldNotEvaluate.png) | Occurs when you clear the debugged image in Visual Studio 2015, which does not support nullifying the debugged value. |
| Could not load file or assembly 'KGySoft.Drawing.DebuggerVisualizers.dll'.<br/>![Could not load file or assembly 'KGySoft.Drawing.DebuggerVisualizers.dll'.](https://kgysoft.net/images/DebuggerVisualizerTrShCouldNotLoadVisualizer.png) or one of its dependencies. | Visual Studio 2008 supports the .NET 3.5 version only. A similar error may occur even if some files are missing. Just [install](#installing-debugger-visualizers) a correct version again. |
| Value does not fall within the expected range.<br/>![Value does not fall within the expected range.](https://kgysoft.net/images/DebuggerVisualizerTrShValueUnexpectedRange.png) | Windows XP does not support the .NET 4.5 version. |

## Download
You can download the sources and the binaries as .zip archives [here](https://github.com/koszeggy/KGySoft.Drawing.Tools/releases).

## Debugger Visualizers Test Tool

A simple test application is also available in the download binaries. Though it was created mainly for testing purposes it also demonstrates the debugger visualizer and some `KGySoft.Drawing` features. If you are interested in using [KGy SOFT Drawing Libraries](https://kgysoft.net/drawing) (and [KGy SOFT Core Libraries](https://kgysoft.net/corelibraries)) as a developer, then it may worth checking its source code (especially the [`DebuggerTestFormViewModel`](https://github.com/koszeggy/KGySoft.Drawing.Tools/blob/master/KGySoft.Drawing.DebuggerVisualizers.Test/ViewModel/DebuggerTestFormViewModel.cs) class).

![Debugger Visualizer Test App](https://kgysoft.net/images/DebuggerVisualizerTest.png)

> _Note:_ The Debugger Visualizers Test Tool directly references a specific version of the `Microsoft.VisualStudio.DebuggerVisualizers` assembly, therefore Visual Studio will not able to display visualizers when debugging this project unless you use the very same version (Visual Studio 2013).

## Release Notes

See the [change log](https://github.com/koszeggy/KGySoft.Drawing.Tools/blob/master/changelog.txt).

## License
This repository is under the [CC BY-ND 4.0](https://creativecommons.org/licenses/by-nd/4.0/legalcode) license (see a short summary [here](https://creativecommons.org/licenses/by-nd/4.0)). It allows you to copy and redistribute the material in any medium or format for any purpose, even commercially. The only thing is not allowed is to distribute a modified material as yours.
