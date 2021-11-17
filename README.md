[![KGy SOFT .net](https://user-images.githubusercontent.com/27336165/124292367-c93f3d00-db55-11eb-8003-6d943ee7d7fa.png)](https://kgysoft.net)

# KGy SOFT Drawing Tools

The KGy SOFT Drawing Tools repository contains a couple of applications and some [Debugger Visualizers](#debugger-visualizers) for several `System.Drawing` types such as `Bitmap`, `Metafile`, `Icon`, `BitmapData`, `Graphics`, etc. (see also below). The visualizers use [KGy SOFT Imaging Tools](#kgy-soft-imaging-tools) to display these types visually, which can be executed as a standalone application as well. Along with the [Debugger Visualizers Test Tool](#debugger-visualizers-test-tool) it can be considered also as a demonstration of the features of the [KGy SOFT Drawing Libraries](https://kgysoft.net/drawing).

[![Website](https://img.shields.io/website/https/kgysoft.net/corelibraries.svg)](https://kgysoft.net/drawing)
[![Drawing Libraries Repo](https://img.shields.io/github/repo-size/koszeggy/KGySoft.Drawing.svg?label=Drawing%20Libraries)](https://github.com/koszeggy/KGySoft.Drawing)

## Table of Contents
1. [KGy SOFT Imaging Tools](#kgy-soft-imaging-tools)
   - [Compatibility](#compatibility)
   - [Localization](#localization)
2. [Debugger Visualizers](#debugger-visualizers)
   - [Installing Debugger Visualizers](#installing-debugger-visualizers)
   - [Troubleshooting](#troubleshooting)
3. [Download](#download)
4. [Release Notes](#release-notes)
5. [Debugger Visualizers Test Tool](#debugger-visualizers-test-tool)
6. [License](#license)


## KGy SOFT Imaging Tools
<p align="center">
  <img alt="KGySoft Imaging Tools on Windows 10" src="https://user-images.githubusercontent.com/27336165/124250655-5e760d80-db25-11eb-824f-195e5e1dbcbe.png"/>
  <br/><em>KGy SOFT Imaging Tools on Windows 10</em>
</p>

The Imaging Tools application makes possible to load images and icons from file, manipulate and save them into various formats. Several modifications are possible such as rotating, resizing, changing pixel format with quantizing and dithering, adjusting brightness, contrast and gamma, or even editing the palette entries of indexed bitmaps.

<p align="center">
  <img alt="Changing Pixel Format with Quantizing and Dithering" src="https://user-images.githubusercontent.com/27336165/124250977-b3198880-db25-11eb-9f72-6fa51d54a9da.png"/>
  <br/><em>Changing Pixel Format with Quantizing and Dithering</em>
</p>

> _Tip:_ As a developer, you can access all of these image manipulaion functions by using [KGy SOFT Drawing Libraries](https://github.com/koszeggy/KGySoft.Drawing). It supports not just `System.Drawing` types but also completely managed and technology agnostic bitmap data manipulation as well.

### Compatibility

KGy SOFT Imaging Tools supports a wide range of platforms. Windows is supported starting with Windows XP but by using [Mono](https://www.mono-project.com/download/stable/) you can execute it also on Linux. See the [downloads](#download) for details.

<p align="center">
  <img alt="KGy SOFT Imaging Tools on Ubuntu Linux, using dark theme" src="https://user-images.githubusercontent.com/27336165/124265526-157a8500-db36-11eb-8d3a-84e66259ce03.png"/>
  <br/><em>KGy SOFT Imaging Tools on Ubuntu Linux, using dark theme</em>
</p>

### Localization

KGy SOFT Imaging Tools supports localization from .resx files. New language resources can be generated for any languages, and you can edit the texts within the application. The changes can be applied on-the-fly, without exiting the application. If you switch to a right-to-left language, then the layout is also immediately applied (at least on Windows).

> _Tip:_ As a developer, you can use [KGy SOFT Core Libraries](https://github.com/koszeggy/KGySoft.CoreLibraries#dynamic-resource-management) if you want something similar in your application.

The edited resources are saved in .resx files in the `Resources` subfolder of the application. Resources can be downloaded from within the application as well.

<p align="center">
  <img alt="Editing resources in KGy SOFT Imaging Tools" src="https://user-images.githubusercontent.com/27336165/124143008-0a1e4f80-da8b-11eb-8f85-572507b66154.png"/>
  <br/><em>Editing resources in KGy SOFT Imaging Tools</em>
</p>

> _Note:_ If you create a localization for your language feel free to [submit a new issue](https://github.com/koszeggy/KGySoft.Drawing.Tools/issues/new?assignees=&labels=&template=submit-resources.md&title=%5BRes%5D) and I will make it available for everyone. Don't forget to mention your name in the translated About menu.

#### Help, my resources are gone!

If you use Imaging Tools as debugger visualizers, then it can be executed from various locations. See the bottom of the [Troubleshooting](#troubleshooting) section below.

## Debugger Visualizers

Imaging Tools is packed with several debugger visualizers for Visual Studio (compatible with all versions starting with Visual Studio 2008, and supports even .NET Core 2.1 and newer platform targets). When a type is debugged in Visual Studio and there is a debugger visualizer installed for that type, then a magnifier icon appears that you can click to open the visualizer.

<p align="center">
  <img alt="Debugger Visualizer Usage" src="https://user-images.githubusercontent.com/27336165/124266849-c7ff1780-db37-11eb-9df8-f2149430da16.png"/>
  <br/><em>Debugger Visualizer Usage</em>
</p>

Either click the magnifier icon or choose a debugger visualizer from the drop down list (if more visualizers are applicable).

<p align="center">
  <img alt="Debugging a Graphics instance" src="https://user-images.githubusercontent.com/27336165/124266974-f54bc580-db37-11eb-98e1-207c48590afa.png"/>
  <br/><em>Debugging a Graphics instance</em>
</p>

The `KGySoft.Drawing.DebuggerVisualizers` assembly provides debugger visualizers for the following types:
- `System.Drawing.Image`: If executed for a non read-only variable or member of type `Image`, then the actual value can be replaced by any `Bitmap` or `Metafile`.
- `System.Drawing.Bitmap`: Supports multi-page, multi-resolution and animated `Bitmap` instances. The bitmap can be saved in many formats. In a non read-only context the bitmap can be replaced from file and its palette (for indexed bitmaps) can be edited.
- `System.Drawing.Imaging.Metafile`: Unlike many image debugger visualizers around the net, this one does not transform the metafile into a low-resolution PNG image because it is able to serialize the actual metafile content. The metafile can be saved into EMF/WMF formats. In a non read-only context the image can be replaced from file.
- `System.Drawing.Icon`: Supports compound and huge icons even in Windows XP. The icon can be saved, and in a non read-only context it can be replaced from file.
- `System.Drawing.Imaging.BitmapData`: Even for indexed data; however, the palette information cannot be retrieved.
- `System.Drawing.Graphics`: Supports both image and native window graphics.
- `System.Drawing.Imaging.ColorPalette`: In a non read-only context the colors can be edited.
- `System.Drawing.Color`: In a non read-only context the color can be replaced.

<p align="center">
  <img alt="Debugging a ColorPalette instance" src="https://user-images.githubusercontent.com/27336165/124268121-66d84380-db39-11eb-97f5-6ff569b01daa.png"/>
  <br/><em>Debugging a ColorPalette instance</em>
</p>

### Installing Debugger Visualizers

#### By VSIX Installer

If you use Visual Studio 2013 or newer, then you can perform the install directly from Visual Studio by the _Extensions/Manage Extensions_ (older Visual Studio versions: _Tools/Extensions and Updates..._) menu if you search for the "_KGy SOFT Drawing DebuggerVisualizers_" extension.

Alternatively, you can download the installer package from the VisualStudio Marketplace. There are two versions available:
* A [32-bit version](https://marketplace.visualstudio.com/items?itemName=KGySoft.drawing-debugger-visualizers) for Visual Studio 2013-2019
* And a [64-bit version](https://marketplace.visualstudio.com/items?itemName=KGySoft.drawing-debugger-visualizers-x64) for Visual Studio 2022

#### Manual Install

1. [Download](#download) the binaries and extract the .7z archive to any folder.
2. Open the folder with the extracted content. You will find four folders there:
  - `net35` contains the .NET Framework 3.5 version. Compatible with all Visual Studio versions starting with Visual Studio 2008 (tested with versions 2008-2019). Cannot be used to debug .NET Core applications. 
  - `net40` contains the .NET Framework 4.0 version. You cannot use this one for Visual Studio 2008. Cannot be used to debug .NET Core applications.
  - `net45` contains the .NET Framework 4.5 version. You cannot use this one for Windows XP and Visual Studio 2008/2010. With some limitations supports also .NET Core/.NET projects (in case of issues see the [Troubleshooting](#Troubleshooting) section).
  - `net5.0-windows` contains the .NET 5.0 binaries of the Imaging Tools application. Debugger visualizers are not included because it would not be recognized by Visual Studio anyway.
3. Execute `KGySoft.Drawing.ImagingTools.exe` from one of the folders listed above. Click the _Manage Debugger Visualizer Installations..._ button (the gear icon) on the toolbar.

<p align="center">
  <img alt="Installing Debugger Visualizers from Imaging Tools" src="https://user-images.githubusercontent.com/27336165/124270600-b53b1180-db3c-11eb-92d2-fcbdcbc76ca8.png"/>
  <br/><em>Installing Debugger Visualizers from Imaging Tools</em>
</p>

> _Note:_ Starting with version 2.1.0 the debugger visualizers can be used also for .NET Core projects from Visual Studio 2019, even though no .NET Core binaries are used.

4. In the drop down list you will see the identified Visual Studio versions in your Documents folder. You can select either one of them or the _&lt;Custom Path...&gt;_ menu item to install the visualizer debuggers into a custom folder.

<p align="center">
  <img alt="Selecting Visual Studio Version" src="https://user-images.githubusercontent.com/27336165/124272120-9dfd2380-db3e-11eb-896d-c244fdbd85bc.png"/>
  <br/><em>Selecting Visual Studio Version</em>
</p>

5. Click on the _Install_ button. On success the status will display the installed version.

<p align="center">
  <img alt="Installation Complete" src="https://user-images.githubusercontent.com/27336165/124272596-3c898480-db3f-11eb-92f9-b219ff7491f3.png"/>
  <br/><em>Installation Complete</em>
</p>

### Troubleshooting

If Visual Studio cannot load the visualizer or you have other debugger visualizer related problems check this table.

| **Issue** | **Solution** |
|-----------|--------------|
| The magnifier icon does not appear<img width="0px"/> | Debugger Visualizers are not installed for the Visual Studio version you use. Check the [installation steps](#installing-debugger-visualizers) above. Make sure you select the correct version from the list. |
| Could not load this custom viewer.<br/>![Could not load this custom viewer.](https://kgysoft.net/images/DebuggerVisualizerTrShCouldNotLoadViewer.png) | Open _Debug / Options / Debugging / General_ and make sure that both _Use Managed Compatibility Mode_ and _Use legacy C# and VB expression evaluators_ are unchecked.<br/>![Debugging Options.](https://kgysoft.net/images/DebuggerVisualizerTrShCouldNotLoadSettings.png) |
| An unhandled exception of type 'System.InvalidOperationException' was thrown by the custom visualizer component in the process being debugged. (Object is currently in use elsewhere.)<br/>![Object is currently in use elsewhere.](https://kgysoft.net/images/DebuggerVisualizerTrShObjectIsInUse.png) | You try to debug a `Graphics` instance, whose Device Context is in use (the `GetHdc` method has been called previously). This `Graphics` instance cannot be accessed until the `ReleaseHdc` method is called. |
| Unable to perform function evaluation on the process being debugged (Unable to evaluate expression because the code is optimized or a native frame is on top of the call stack.)<br/>![Unable to evaluate expression because the code is optimized or a native frame is on top of the call stack.](https://kgysoft.net/images/DebuggerVisualizerTrShOptimized.png) | Can occur even with debug build, typically when debugging a .NET Core project. When it happens, then likely all members in the debug window show a similar error message. Perform a Step Over operation (F10 by default)/Set Next Statement (Ctrl+Shift+F10), or restart the debugging session and try again. |
| Unable to perform function evaluation on the process being debugged (Function evaluation timed out)<br/>![Function evaluation timed out.](https://kgysoft.net/images/DebuggerVisualizerTrShTimeout.png) | It can happen that the visualizer loads too slowly for the first time. Just try to click the magnifier icon again, which usually solves the problem. If all members in the debug window show an error, then perform a Step Over operation first (F10 by default). |
| Unable to load the custom visualizer (Operation is not valid due to the current state of the object)<br/>![Operation is not valid due to the current state of the object.](https://kgysoft.net/images/DebuggerVisualizerTrShOperationIsNotValid.png) | Typically happens after timeout. Likely all members in the debug window show a similar error message. Perform a Step Over operation (F10 by default)/Set Next Statement (Ctrl+Shift+F10), or restart the debugging session and try again. |
| An unhandled exception of type 'System.NullReferenceException' was thrown by the custom visualizer component in the process being debugged.<br/>![An unhandled exception of type 'System.NullReferenceException' was thrown by the custom visualizer component in the process being debugged.](https://kgysoft.net/images/DebuggerVisualizerTrShExceptionNullRef.png) | • Occurs when you clear the debugged image in Visual Studio 2019, which does not support nullifying the debugged value.<br/>The bug has been [reported](https://developercommunity.visualstudio.com/content/problem/676481/visual-sudio-2019-throws-a-nullreferenceexception.html) to the VisualStudio team.<br/><br/> • Occurs in Visual Studio 2019 also when replacing a `Color` entry in an array by the visualizer.<br/>The bug has been [reported](https://developercommunity.visualstudio.com/content/problem/1142584/visual-sudio-2019-throws-a-nullreferenceexception-1.html) to the VisualStudio team. |
| The Color visualizer appears in read-only, even though the debugged value is in a read-write context (eg. local variable).<br/>![Color visualizer appears as read-only.](https://kgysoft.net/images/DebuggerVisualizerTrShColorReadOnly.png) | Occurs with some specific builds of Visual Studio 2019<br/>The bug has been [reported](https://developercommunity.visualstudio.com/content/problem/1142584/visual-sudio-2019-throws-a-nullreferenceexception-1.html) to the VisualStudio team as part of another issue. |
| Unable to load the custom visualizer. (Exception of type 'System.Exception' was thrown)<br/>![Exception of type 'System.Exception' was thrown.](https://kgysoft.net/images/DebuggerVisualizerTrShExceptionVS2019.png) | Occurs when you debug a project that targets .NET Framework 3.5 or earlier versions. You can either target at least .NET Framework 4.0 (at least temporarily, for debugging), or you can manually [install](#installing-debugger-visualizers) the .NET Framework 3.5 version of the Debugger Visualizers (which cannot be used to debug the .NET Core targets, though). |
| Unable to load the custom visualizer. (The debuggee-side visualizer assembly 'KGySoft.Drawing.DebuggerVisualizers, Version=[...]' was not found at path '&lt;Documents&gt;\Visual Studio &lt;version&gt;\Visualizers\netstandard2.0'.)<br/>![The debuggee-side visualizer assembly 'KGySoft.Drawing.DebuggerVisualizers, Version=[...]' was not found.](https://kgysoft.net/images/DebuggerVisualizerTrShNotFoundStandard20.png) | Occurs when you try to debug a .NET Core project with the .NET Framework 3.5/4.0 builds installed. To debug .NET Core projects you need to [install](#installing-debugger-visualizers) the .NET Framework 4.5 version. |
| An unhandled exception of type 'System.TypeInitializationException' was thrown by the custom visualizer component in the process being debugged. (The type initializer for 'KGySoft.CoreLibraries.XXXX' threw an exception.)<br/>![Exception of type 'System.TypeInitializerException' was thrown.](https://kgysoft.net/images/DebuggerVisualizerTrShExceptionTypeInitialization.png) | Occurs when you try to debug a .NET Core project while the .NET Framework 3.5 binaries of the debugger visualizers are deployed in the `netstandard2.0` subfolder. To debug .NET Core projects you need to [install](#installing-debugger-visualizers) the .NET Framework 4.5 version. |
| An unhandled exception of type 'System.IO.FileLoadException' was thrown by the custom visualizer component in the process being debugged. (Could not load file or assembly 'KGySoft.CoreLibraries / KGySoft.Drawing, Version=####'.)<br/>![Could not load file or assembly 'KGySoft.CoreLibraries'.](https://kgysoft.net/images/DebuggerVisualizerTrShExceptionFileLoad.png) | If you debug a .NET Core project that also references KGySoft assemblies, then the versions referenced by the debugger visualizers and your project must match; otherwise, you get this error. If the version in the message is higher than the one your project references, then simply upgrade the references of your project. Otherwise, as the issue does not occur when targeting .NET Framework, you can try to change the targeted framework in the .csproj file for the debugging. |
| An unhandled exception of type 'System.Exception' was thrown by the UI-side custom visualizer component.<br/>![An unhandled exception of type 'System.Exception' was thrown by the UI-side custom visualizer component.](https://kgysoft.net/images/DebuggerVisualizerTrShException.png) | Occurs when you clear the debugged image in Visual Studio 2017, which does not support nullifying the debugged value. |
| Could not evaluate expression.<br/>![Could not evaluate expression.](https://kgysoft.net/images/DebuggerVisualizerTrShCouldNotEvaluate.png) | Occurs when you clear the debugged image in Visual Studio 2015, which does not support nullifying the debugged value. |
| An unhandled exception of type 'System.TypeLoadException' / 'System.MissingMethodException' was thrown by the custom visualizer component in the process being debugged.<br/>![An unhandled exception of type 'System.TypeLoadException' was thrown by the custom visualizer component in the process being debugged.](https://kgysoft.net/images/DebuggerVisualizerTrShExceptionTypeLoad.png)<br/>![An unhandled exception of type 'System.MissingMethodException' was thrown by the custom visualizer component in the process being debugged.](https://kgysoft.net/images/DebuggerVisualizerTrShExceptionMissingMethod.png) | You are probably debugging a .NET Core 2.0 application referencing the `System.Drawing.Common` NuGet package. The project should target at least .NET Core 2.1 to be able to debug `System.Drawing` types. |
| Unable to load the custom visualizer (serializer could not be resolved).<br/>![Unable to load the custom visualizer (serializer could not be resolved).](https://kgysoft.net/images/DebuggerVisualizerTrShUnableToLoadSerializer.png) | The project you debug references an unmatching version of the `Microsoft.VisualStudio.DebuggerVisualizers` to your actual VisualStudio version. |
| Unable to load the custom visualizer (The UI-side visualizer type must derive from 'DialogDebuggerVisualizer').<br/>![Unable to load the custom visualizer (The UI-side visualizer type must derive from 'DialogDebuggerVisualizer').](https://kgysoft.net/images/DebuggerVisualizerTrShUnableToLoadMustDeriveFrom.png) | The `Microsoft.VisualStudio.DebuggerVisualizers.dll` has been copied to the debugger visualizers installation folder. Recent Visual Studio versions can handle if a debugger visualizer references an unmatching version of this assembly but only if this assembly is not deployed along with the visualizers. |
| Could not load file or assembly 'KGySoft.Drawing.DebuggerVisualizers.dll'.<br/>![Could not load file or assembly 'KGySoft.Drawing.DebuggerVisualizers.dll'.](https://kgysoft.net/images/DebuggerVisualizerTrShCouldNotLoadVisualizer.png) or one of its dependencies. | Visual Studio 2008 supports the .NET 3.5 version only. A similar error may occur even if some files are missing. Just [install](#installing-debugger-visualizers) a correct version again. |
| Value does not fall within the expected range.<br/>![Value does not fall within the expected range.](https://kgysoft.net/images/DebuggerVisualizerTrShValueUnexpectedRange.png) | Windows XP does not support the .NET 4.5 version. |
| The app looks blurry. | If you changed the DPI settins, you need to restart the application. Per-monitor DPI awareness is not supported. |
| The visual elements are scaled incorrectly.<br/>![Incorrectly scaled image](https://user-images.githubusercontent.com/27336165/124148578-0e993700-da90-11eb-9c67-4e06e522795b.png) | May happen if you use Imaging Tools from debugger visualizers, and you have just changed the DPI settings without signing out/in. However, signing in and out is not required if you execute the app directly. |
| I edited the language resource files but I cannot find them (or they appear to be gone) | The _Visual Studio/Tools/KGy SOFT Drawing Debugger Visualizers_ and clicking the magnifier icon executes the Imaging Tools from different locations. If you edit the language resources at one place they will not be automatically applied at the other place. Therefore, the saved resources might be at different possible locations:<br/>• If you execute a manually deployed version the resources will be in a `Resources` subfolder in the folder you executed the Imaging Tools from.<br/>• During debugging the tool is executed from the debugger visualizers folder: `Documents\Visual Studio <version>\Visualizers`<br/>• If you launch the tool from the Visual Studio Tools menu, then it is located under `ProgramData\Microsoft\VisualStudio\Packages\...` |

## Download

> _Tip:_ See [above](#by-vsix-installer) how to download the debugger visualizer installers

You can download the sources and the binaries as .7z/.zip archives at the [releases](https://github.com/koszeggy/KGySoft.Drawing.Tools/releases) page.

To support the widest possible range of platforms the binaries archive contains multiple builds in different folders.
* `net35`: This contains the .NET Framework 3.5 build and though it works on every platforms Imaging Tools supports, it is not really recommended to use as a standalone application. If you use Imaging Tools as [debugger visualizers](#installing-debugger-visualizers), then it is the only version you can use for Visual Studio 2008. For newer Visual Studio versions use it only if you want to debug a .NET Framework 2.0-3.5 application.
* `net40`: This is the .NET Framework 4.0 build. As a standalone application, it's basically recommended only for Windows XP.
* `net45`: This is the .NET Framework 4.5 build, which is the recommended version to use as a debugger visualizer for .NET Framework 4.x and .NET Core projects (including .NET 5 and newer platforms). As a standalone application, this is also the recommended version for Linux (though 3.5/4.0 also support it).
* `net5.0-windows`: This folder contains the .NET 5.0 build. As a standalone application this is the recommended version for Windows 7 and above. On the other hand, this one cannot be used as a debugger visualizer (even for .NET Core projects) and does not support Linux either.

## Debugger Visualizers Test Tool

A simple test application is also available in the download binaries. Though it was created mainly for testing purposes it also demonstrates how to use the public API of the Imaging Tools application and the DebuggerVisualizers from a consumer library or application.

<p align="center">
  <img alt="Debugger Visualizer Test Tool" src="https://user-images.githubusercontent.com/27336165/125045273-1a42ba00-e09d-11eb-8484-02b10dbafd8e.png"/>
  <br/><em>Debugger Visualizer Test Tool</em>
</p>

> _Note:_ The Debugger Visualizers Test Tool directly references a specific version of the `Microsoft.VisualStudio.DebuggerVisualizers` assembly, therefore Visual Studio will not able to display visualizers when debugging this project unless you use the very same version (Visual Studio 2022).

## Release Notes

See the [change log](https://github.com/koszeggy/KGySoft.Drawing.Tools/blob/master/changelog.txt).

## License
This repository is under the [KGy SOFT License 1.0](https://github.com/koszeggy/KGySoft.Drawing.Tools/blob/master/LICENSE), which is a permissive GPL-like license. It allows you to copy and redistribute the material in any medium or format for any purpose, even commercially. The only thing is not allowed is to distribute a modified material as yours: though you are free to change and re-use anything, do that by giving appropriate credit. See the [LICENSE](https://github.com/koszeggy/KGySoft.Drawing.Tools/blob/master/LICENSE) file for details.
