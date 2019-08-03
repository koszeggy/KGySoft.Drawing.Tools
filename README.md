[![KGy SOFT .net](http://docs.kgysoft.net/drawing/icons/logo.png)](https://kgysoft.net)

# KGy SOFT Drawing Tools

KGy SOFT Drawing Tools repository contains [Debugger Visualizers](#debugger-visualizers) for several `Ststem.Drawing` types such as `Bitmap`, `Metafile`, `Icon`, `BitmapData`, `Graphics`, etc. (see also below). The visualizers use [KGy SOFT Imaging Tools](#kgy-soft-imaging-tools) to display these types visually, which can be executed as a standalone application as well. Along with the [Debugger Visualizers Test Tool](#debugger-visualizers-test-tool) it can also be considered as a demonstration of the features of the [KGy SOFT Drawing Libraries](https://kgysoft.net/drawing).

[![Website](https://img.shields.io/website/https/kgysoft.net/corelibraries.svg)](https://kgysoft.net/drawingdebuggervisualizers)
[![Drawing Libraries Repo](https://img.shields.io/github/repo-size/koszeggy/KGySoft.Drawing.svg?label=DrawingLibraries)](https://github.com/koszeggy/KGySoft.Drawing)

## Table of Contents
<!--1. [KGy SOFT Imaging Tools](#kgy-soft-imaging-tools)
2. [Debugger Visualizers](#debugger-visualizers)
   - [Install](#install)
   - [Troubleshooting](#troubleshooting) 
3. [Debugger Visualizers Test Tool](#debugger-visualizers-test-tool)
4. [Download](#download)
5. [License](#license)-->
1. [KGy SOFT Imaging Tools](#kgy-soft-imaging-tools)
2. [Debugger Visualizers](#debugger-visualizers)
   - [Install](#install)
3. [Download](#download)
4. [License](#license)


## KGy SOFT Imaging Tools
![KGySoft.Drawing.ImagingTools](https://kgysoft.net/images/ImagingTools.png)

The Imaging Tools application makes possible to load images and icons from file, provides detailed information about images and you can convert them to other formats.

## Debugger Visualizers

Imaging tools is packed with some debugger visualizers for Visual Studio (compatible with all versions starting with Visual Studio 2008). When a type is debugged in Visual Studio and there is a debugger visualizer installed for that type, then a magnifier icon appears that you can click to open the visualizer.

![Debugger Visualizer Usage](https://kgysoft.net/images/DebuggerVisualizerUsage.png)

Either click the magnifier icon or choose a debugger visualizer from the drop down list (of more of them are applicable).

![Debugging Graphics](https://kgysoft.net/images/DebugGraphics.png)

The `KGySoft.Drawing.DebuggerVisualizers` assembly provides debugger visualizers for the following types:
- `System.Drawing.Image`: If executed for a non read-only variable or member of type `Image`, then the actual value can be replaced by any `Bitmap` or `Metafile`.
- `System.Drawing.Bitmap`: Supports multi-page, multi-resolution and animated `Bitmap` instances. The bitmap can be saved in many formats. In a non read-only context the bitmap can be replaced from file and its palette (for indexed bitmaps) can be edited.
- `System.Drawing.Imaging.Metafile`: Unlike many image debugger visualizer around the net, this one does not transform the metafile into a low-resolution PNG image because it is able to serialize the actual metafile content. The metafile can be saved into EMF/WMF formats. In a non read-only context the image can be replaced from file.
- `System.Drawing.Icon`: Supports compound and huge icons even in Windows XP. The icon can be saved, and in a non read-only context it can be replaced from file.
- `System.Drawing.Imaging.BitmapData`: Even for indexed data; however, the palette information cannot be retrieved.
- `System.Drawing.Graphics`: Supports both image and native window graphics.
- `System.Drawing.Imaging.ColorPalette`: In a non read-only context the colors can be edited.
- `System.Drawing.Color`: In a non read-only context the color can be replaced.

![Debugging Palette](https://kgysoft.net/images/DebugPalette.png)

### Install

1. [Download](#download) the binaries and extract the .zip file to any folder.
2. Open the folder with the extracted content. You will find three folders there:
  - `NET35` contains the .NET 3.5 version. Compatible with all Visual Studio versions starting with Visual Studio 2008.
  - `NET40` contains the .NET 4.0 version. You cannot use this one for Visual Studio 2008.
  - `NET45` contains the .NET 4.5 version. You cannot use this one for Windows XP and Visual Studio 2008/2010.
3. Execute `KGySoft.Drawing.ImagingTools.exe` from one of the folders listed above. Click the _Manage Debugger Visualizer Installations..._ button (the gear icon) on the toolbar.

![Select Install](https://kgysoft.net/images/InstallSelectMenu.png)

4. In the drop down list you will see the identified Visual Studio versions in you Documents folder. You can select either one of them or the _&lt;Custom Path...&gt;_ menu item to install the visualizer debuggers into a custom folder.

![Select Visual Studio version](https://kgysoft.net/images/InstallSelectVSVersion.png)

5. Click on the _Install_ button. On success the status will display the installed version.

![Installation Complete](https://kgysoft.net/images/InstallComplete.png)
<!--
### Troubleshooting

- Which version
- Steps 
### Debugger Visualizers Test Tool
-->
## Download
You can download the sources and the binaries as .zip archives [here](https://github.com/koszeggy/KGySoft.Drawing.Tools/releases).

## Release Notes

See the [change log](https://github.com/koszeggy/KGySoft.Drawing.Tools/blob/master/changelog.txt).

## License
This repository is under the [CC BY-ND 4.0](https://creativecommons.org/licenses/by-nd/4.0/legalcode) license (see a short summary [here](https://creativecommons.org/licenses/by-nd/4.0)). It allows you to copy and redistribute the material in any medium or format for any purpose, even commercially. The only thing is not allowed is to distribute a modified material as yours.
